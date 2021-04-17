using System;
using System.Linq;
using System.Collections.Generic;

using static FogMod.Util;
using static FogMod.AnnotationData;
using static FogMod.GraphChecker;
using static FogMod.Graph;

using FogMod.io;

using System.IO;

namespace FogMod {
  public class GraphConnector {
    private readonly RandomizerOptions options_;
    private readonly Graph graph_;
    private readonly AnnotationData annotationData_;

    enum EdgeSilo {
      PAIRED,
      UNPAIRED
    }

    public static void Randomize(
        RandomizerOptions options,
        Graph graph,
        AnnotationData annotationData)
      => new GraphConnector(options, graph, annotationData);

    private GraphConnector(
        RandomizerOptions options,
        Graph graph,
        AnnotationData annotationData) {
      this.options_ = options;
      this.graph_ = graph;
      this.annotationData_ = annotationData;

      this.Randomize_();
    }

    // Modify scaling values before they're actually used
    private void PostProcessScaling(ref float healthScaling, ref float damageScaling) {
      if (Flags.IsDs3) {
        healthScaling = Math.Min(healthScaling, 2);
        damageScaling = Math.Min(damageScaling, 2);
      }
    }
    
    private void Randomize_() {
      // TODO: Move this line?
      // TODO: Set this as an enum instead.
      Flags.IsDs1 = this.graph_.Areas.ContainsKey("asylum");
      Flags.IsDs3 = this.graph_.Areas.ContainsKey("firelink_cemetery");

      Dictionary<string, Node> graphNodes = graph_.Nodes;
      List<Edge> allFroms = graphNodes
                            .Values.SelectMany(
                                node => node.From.Where(e => e.From == null))
                            .ToList();
      List<Edge> allTos = graphNodes
                          .Values.SelectMany(
                              node => node.To.Where(e => e.To == null))
                          .ToList();
      Random shuffleRandom = new Random(options_.Seed);
      Shuffle(shuffleRandom, allFroms);
      Shuffle(shuffleRandom, allTos);

      // For now, try to connect one-way to one-way and have distinct silos.
      foreach (EdgeSilo siloType in Enum.GetValues(typeof(EdgeSilo))) {
        List<Edge> froms = allFroms
                           .Where(e => (e.Pair == null) ==
                                       (siloType == EdgeSilo.UNPAIRED))
                           .ToList();
        List<Edge> tos = allTos
                         .Where(e => (e.Pair == null) ==
                                     (siloType == EdgeSilo.UNPAIRED))
                         .ToList();
        if (options_["explain"])
          Console.WriteLine(
              $"Connecting silo {siloType}: {froms.Count} with no from, and {tos.Count} with no to");

        while (true) {
          if (options_["vanilla"]) break;
          Edge from = null;
          for (int i = 0; i < froms.Count; i++) {
            from = froms[i];
            if (from.From != null)
              throw new Exception($"Connected edge still left: {from}");
            froms.RemoveAt(i);
            tos.Remove(from.Pair);
            break;
          }
          if (from == null) break;
          Edge to = null;
          if (tos.Count == 0) {
            if (from.Pair != null) {
              // Have to connect edge to itself
              to = from.Pair;
            } else {
              throw new Exception($"Ran out of eligible edges");
            }
          }
          for (int i = 0; i < tos.Count; i++) {
            Edge cand = tos[i];
            if (cand.To != null)
              throw new Exception($"Connected edge still left: {cand}");
            // Avoid connecting to self
            if (from.Pair == cand) continue;
            if ((from.Pair == null) != (cand.Pair == null)) continue;
            to = cand;
            tos.RemoveAt(i);
            froms.Remove(to.Pair);
            break;
          }
          if (to == null) break;
          if (from.IsFixed || to.IsFixed)
            throw new Exception(
                $"Internal error: found fixed edges in randomization {from} ({from.IsFixed}) and {to} ({to.IsFixed})");
          this.graph_.Connect(to, from);
        }
        if (froms.Count > 0 || tos.Count > 0)
          throw new Exception(
              $"Internal error: unconnected edges after randomization:\nFrom edges: {string.Join(", ", froms)}\nTo edges: {string.Join(", ", tos)}");
      }

      if (options_["start"]) {
        this.graph_.Start =
            annotationData_.CustomStarts[
                new Random(options_.Seed - 1).Next(
                    annotationData_.CustomStarts.Count)];
      } else if (Flags.IsDs1) {
        this.graph_.Start = new CustomStart {
            Name = "Asylum",
            Area = "asylum",
            Respawn = "asylum 1812961",
        };
      } else if (Flags.IsDs3) {
        this.graph_.Start = new CustomStart {
            Name = "Cemetery of Ash",
            Area = "firelink_cemetery",
            Respawn = "firelink 1812961",
        };
      }
      string start = this.graph_.Start.Area;

      // Massive pile of edge-swapping heuristics incoming
      int tries = 0;
      GraphChecker checker = new GraphChecker();
      CheckRecord check = null;
      bool pairedOnly = !options_["unconnected"];
      List<string> triedSwaps = new List<string>();
      while (tries++ < 100) {
        if (options_["explain"])
          Console.WriteLine($"------------------------ Try {tries}");
        check = checker.Check(options_, this.graph_, start);
        if (check.Unvisited.Count == 0 && Flags.IsDs3) {
          // Try to minimize distance to Firelink Shrine in DS3
          // This is done by swapping equivalent pairs of areas, matching random cand edge count with random subst edge count, though preferably no additional fixed exits in cand.
          // The first priority is to have Firelink available. If it can be made accessible before the 10th nontrivial area (although not the first, if there's an option), this is done.
          // If not, or if in a high enough try, and tree skip is available, firelink_bellfront is made available instead.
          // Finally if not, it will just be placed as early as possible.
          // The second priority is to find coiled sword.
          // If in a map with random entrances, place it as early as possible without replacing Firelink.
          // If in Bell Tower, repeat for firelink_bellfront and Bell Tower key location
          bool didSwap = false;

          List<string> areaOrder = check
                                   .Records.Values.OrderBy(r => r.Dist)
                                   .Select(r => r.Area)
                                   .ToList();
          if (options_["explain"])
            Console.WriteLine(
                $"Trying to place Firelink now. Overall order: [{string.Join(",", areaOrder.Select((a, i) => $"{a}:{i}"))}]");
          Dictionary<string, int> areaIndex = areaOrder
                                              .Select((a, i) => (a, i))
                                              .ToDictionary(
                                                  a => a.Item1,
                                                  a => a.Item2);

          int nontrivialCount =
              areaOrder.Count(a => !this.graph_.Areas[a].HasTag("trivial"));
          string reasonable = areaOrder
                              .Where(a => !this.graph_.Areas[a]
                                               .HasTag("trivial"))
                              .Skip(nontrivialCount * 15 / 100)
                              .FirstOrDefault();
          int reasonableIndex = reasonable == null
                                    ? areaOrder.Count
                                    : areaOrder.IndexOf(reasonable);
          if (options_["explain"])
            Console.WriteLine(
                $"Last reasonable area for Firelink requisites: {reasonable}. Total count {areaOrder.Where(a => !this.graph_.Areas[a].HasTag("trivial")).Count()}");

          Dictionary<string, int> randomIn = new Dictionary<string, int>();
          Dictionary<int, List<string>> byRandomIn =
              new Dictionary<int, List<string>>();
          foreach (string area in areaOrder) {
            Node node = graphNodes[area];
            int count =
                node.From.Count(e => !e.IsFixed &&
                                     (options_["unconnected"] ||
                                      e.Pair != null));
            // Console.WriteLine($"end time: {area}. {node.From.Count(e => !e.IsFixed && e.Pair != null)}/{node.From.Count} in, {node.To.Count(e => !e.IsFixed && e.Pair != null)}/{node.To.Count} out, trivial {g.Areas[area].HasTag("trivial")}");
            randomIn[area] = count;
            AddMulti(byRandomIn, count, area);
          }

          bool tryPlace(
              string subst,
              bool reasonableOnly,
              List<string> root = null) {
            if (areaIndex[subst] <= reasonableIndex) return true;

            // Note: These should be in area order
            List<string> cands = byRandomIn[randomIn[subst]].ToList();
            cands.Remove(subst);
            if (root != null)
              cands.RemoveAll(c => root.Contains(c) &&
                                   areaIndex[c] < areaIndex[subst]);
            if (options_["explain"])
              Console.WriteLine(
                  $"Candidates for {subst} ({areaIndex[subst]}): {string.Join(",", cands.Select(c => $"{c}:{areaIndex[c]}"))}");

            // See if this is necessary
            // if (excludeSwapTry.ContainsKey(subst)) cands.RemoveAll(c => excludeSwapTry[subst].Contains(c));
            cands.RemoveAll(c => triedSwaps.Contains(
                                string.Join(",",
                                            new SortedSet<string> {subst, c})));
            if (options_["explain"])
              Console.WriteLine(
                  $"Candidates for {subst} without tried: {string.Join(",", cands)}");

            cands.RemoveAll(
                area => check.Records[area].InEdge.All(e => e.Key.IsFixed));
            if (options_["explain"])
              Console.WriteLine(
                  $"Candidates for {subst} with out edge: {string.Join(",", cands)}");
            if (cands.Count == 0) return false;

            List<string> reasonableCands =
                cands.Where(c => areaIndex[c] <= reasonableIndex).ToList();
            if (reasonableCands.Count == 0 && reasonableOnly) return false;

            string cand = reasonableCands.Count > 1 && areaIndex[cands[0]] <= 1
                              ? cands[1]
                              : cands[0];
            if (options_["explain"]) Console.WriteLine($"Final choice: {cand}");

            this.graph_.SwapConnectedAreas(subst, cand);
            triedSwaps.Add(
                string.Join(",", new SortedSet<string> {subst, cand}));
            didSwap = true;
            return true;
          }

          // Find all in-going areas for items
          // Given an area, return all fixed ways to get there, and items/areas required to traverse those paths. Does not need to be recursive.
          Dictionary<string, List<string>> getFixedIn(string area) {
            Dictionary<string, List<string>> fixedIn =
                new Dictionary<string, List<string>>();
            foreach (Edge fixedEntrance in graphNodes[area]
                                           .From.Where(e => e.IsFixed)) {
              List<string> reqs = fixedEntrance.LinkedExpr == null
                                      ? new List<string>()
                                      : fixedEntrance
                                        .LinkedExpr.FreeVars()
                                        .ToList();
              fixedIn[fixedEntrance.From] = reqs;
            }
            return fixedIn;
          }

          if (options_["latewarp"] || options_["instawarp"]) {
            // Guarantee Firelink Shrine placement but not Coiled Sword placement
            // (in instawarp case, this should be a no-op, and can ignore Coiled Sword logic either way)
            tryPlace("firelink", true);
          } else {
            // Try to place both Firelink and Coiled Sword location, including following item chains
            bool placedFirelink = tryPlace("firelink", true);
            // List<string> requiredAreas = new List<string>();
            List<string> accessibleAreas = new List<string>
                {"firelink_cemetery"};
            if (placedFirelink) {
              accessibleAreas.Add("firelink");
            }
            List<string> earlyItems = new List<string> {"coiledsword"};
            List<string> addedItems = new List<string>();
            List<string> earlyItemAreas = new List<string>();
            bool foundRoots;
            do {
              foreach (string item in earlyItems) {
                if (!addedItems.Contains(item)) {
                  addedItems.Add(item);
                  earlyItemAreas.AddRange(
                      this.graph_.ItemAreas[item].Except(earlyItemAreas));
                }
              }
              foundRoots = false;
              foreach (string area in earlyItemAreas.ToList()) {
                Node node = graphNodes[area];
                // If random entrances exist, we can try to get in through swapping, so no need to chase down roots.
                if (randomIn[area] > 0) continue;
                // If no fixed way to get in, that's probably bad, but nothing to do
                Dictionary<string, List<string>> fixedIn = getFixedIn(area);
                if (fixedIn.Count == 0) continue;
                string easyIn =
                    fixedIn.Keys.OrderBy(a => fixedIn[a].Count).First();
                // Is this always fine?
                if (!earlyItemAreas.Contains(easyIn) &&
                    !accessibleAreas.Contains(easyIn)) {
                  earlyItemAreas.Add(easyIn);
                  foundRoots = true;
                }
                foreach (string moreDep in fixedIn[easyIn]) {
                  if (this.graph_.ItemAreas.ContainsKey(moreDep) &&
                      !earlyItems.Contains(moreDep)) {
                    earlyItems.Add(moreDep);
                    foundRoots = true;
                  } else if (graphNodes.ContainsKey(moreDep) &&
                             !earlyItemAreas.Contains(moreDep)) {
                    earlyItemAreas.Add(moreDep);
                    foundRoots = true;
                  }
                }
              }
              if (options_["explain"])
                Console.WriteLine(
                    $"At end of iteration, have items {string.Join(",", earlyItems)} and areas {string.Join(",", earlyItemAreas)}, with adjustable {string.Join(",", earlyItemAreas.Where(a => !accessibleAreas.Contains(a) && randomIn[a] > 0))}");
            } while (foundRoots);
            List<string> placeAreas = earlyItemAreas
                                      .Where(
                                          a => !accessibleAreas.Contains(a) &&
                                               randomIn[a] > 0)
                                      .ToList();
            if (!placedFirelink) placeAreas.Insert(0, "firelink");
            foreach (string area in placeAreas) {
              tryPlace(area, false, accessibleAreas);
              accessibleAreas.Add(area);
            }
          }

          if (didSwap) {
            continue;
          }
          break;
        }

        Edge toFind = null;
        var unvisited = check.Unvisited.ToList();
        Shuffle(new Random(options_.Seed + tries), unvisited);
        bool hasCond = true;
        foreach (string area in unvisited) {
          foreach (Edge edge in graphNodes[area].From) {
            if (!edge.IsFixed && (edge.Pair != null) == pairedOnly) {
              if (edge.LinkedExpr == null) {
                toFind = edge;
                hasCond = false;
                break;
              } else if (toFind == null) {
                toFind = edge;
              }
            }
          }
          if (toFind != null && !hasCond) break;
        }
        if (toFind == null) {
          if (pairedOnly && options_["warp"]) {
            // Redo but with warp edges instead. Generally only happens with warp-only config.
            pairedOnly = false;
            continue;
          }
          throw new Exception(
              $"Could not find edge into unreachable areas [{string.Join(", ", check.Unvisited)}] starting from {start}");
        }

        (Edge, float) victim = (null, 0);
        Edge lastEdge = null;
        int lastCount = 0;
        foreach (NodeRecord rec in check.Records.Values.OrderBy(r => r.Dist)) {
          if (options_["explain"]) Console.WriteLine($"{rec.Area}: {rec.Dist}");
          foreach (var entry in rec.InEdge.OrderBy(e => e.Value)) {
            Edge e = entry.Key;
            if (options_["explain"])
              Console.WriteLine(
                  $"  From {e.From}{(e.IsFixed ? " (world)" : "")}: {entry.Value}");
          }
          var maxEdge = rec
                        .InEdge.OrderBy(e => e.Value)
                        .Where(e => !e.Key.IsFixed &&
                                    (e.Key.Pair !=
                                     null) ==
                                    pairedOnly)
                        .LastOrDefault();
          if (maxEdge.Key != null) {
            int inCount = graphNodes[rec.Area].From.Count;
            if (inCount > lastCount) {
              lastEdge = maxEdge.Key;
              lastCount = inCount;
            }
            KeyValuePair<Edge, float> minEdge =
                rec.InEdge.OrderBy(e => e.Value).First();
            if (minEdge.Key != maxEdge.Key) {
              if (options_["explain"])
                Console.WriteLine(
                    $"  Min {minEdge.Value}, Max editable {maxEdge.Value}");
              // Maybe max victim isn't always best for overall difficulty - or it depends on which edge to swap with is chosen.
              if (maxEdge.Value >= victim.Item2) {
                victim = (maxEdge.Key, maxEdge.Value);
              }
            }
          }
        }
        Edge victimEdge = victim.Item1;
        if (victimEdge == null) {
          // We can't preserve original graph structure really, so just pick arbitrary one to change
          if (lastEdge != null) {
            if (options_["explain"])
              Console.WriteLine(
                  "!!!!!!!!!!! Picking non-redundant edge, but last reachable");
            victimEdge = lastEdge;
          } else {
            // Or, completely pick one indiscriminately even if it goes somewhere important
            victimEdge = check.Records.Keys.SelectMany(a => graphNodes[a].To)
                              .Where(e => !e.IsFixed &&
                                          (e.Pair != null) == pairedOnly)
                              .LastOrDefault();
            if (options_["explain"])
              Console.WriteLine("!!!!!!!!!!! Picking any edge whatsoever");
            if (victimEdge == null)
              throw new Exception(
                  $"No swappable edge found to inaccessible areas. This can happen a lot with low # of randomized entrances.");
          }
        }
        if (options_["explain"]) {
          Console.WriteLine($"Swap unreached: {toFind}");
          Console.WriteLine($"Swap redundant: {victimEdge}");
        }

        // Swap thos edges
        this.graph_.SwapConnectedEdges(victimEdge, toFind);
        pairedOnly = !options_["unconnected"];
      }
      if (check == null || check.Unvisited.Count > 0)
        throw new Exception(
            $"Couldn't solve seed {options_.DisplaySeed} - try a different one");

      // Check succeeded, time to calculate scale and dump info
      float max = check.Records.Values.Where(r => !r.Area.StartsWith("kiln"))
                       .Select(r => r.Dist)
                       .Max();

      Dictionary<string, float> getCumCost(Dictionary<string, float> d) {
        Dictionary<string, float> total = new Dictionary<string, float>();
        float cost = 0;
        foreach (KeyValuePair<string, float> entry in d) {
          cost += entry.Value;
          total[entry.Key] = cost;
        }
        return total;
      }

      float getAreaCost(float dist) {
        float ratio = Math.Min(dist / max, 1);
        // return (float)Math.Pow(ratio, 0.75);
        return ratio;
      }

      // TODO: Should sqrt also be used in DS1? ... it seems a bit weird, but the curves and area sizes are rather different
      var distances = check
                      .Records.Values.OrderBy(r => r.Dist)
                      .ToDictionary(
                          r => r.Area,
                          r => getAreaCost(r.Dist));
      var thisDist = getCumCost(distances);
      var vCost = getCumCost(annotationData_.DefaultCost);
      var vCosts = annotationData_.DefaultCost.Select(t => t.Value)
                                  .OrderBy(t => t)
                                  .ToList();
      var ratios = new List<float>();

      string maybeName(string area)
        => this.graph_.Areas.TryGetValue(area, out Area a)
               ? (a.Text ?? area)
               : area;

      // Choose one blacksmith.
      // If any paths have <=4 areas, choose them
      // If any paths have no bosses unique to that path, choose them
      // Otherwise, choose shortest?
      var upgradeAreas = Flags.IsDs1
                             ? new List<string> {
                                 "parish_andre", "catacombs",
                                 "anorlondo_blacksmith"
                             } // "newlondo" doesn't sell titanite shards... although not that it matters with item rando
                             : new List<string> {"firelink"};
      var upgradeNodes = upgradeAreas
                         .Select(area => check.Records[area])
                         .OrderBy(r => r.Visited.Count)
                         .ToList();
      NodeRecord firstUpgrade;
      if (upgradeNodes[0].Visited.Count < 5) {
        firstUpgrade = upgradeNodes[0];
      } else {
        var commonAreas = new HashSet<string>(this.graph_.Areas.Keys);
        foreach (var rec in upgradeNodes) {
          commonAreas.IntersectWith(rec.Visited);
        }
        NodeRecord minBoss = upgradeNodes.Find(
            rec => commonAreas.IsSupersetOf(
                rec.Visited.Where(a => this.graph_.Areas[a].HasTag("boss"))));
        firstUpgrade = minBoss == null ? upgradeNodes[0] : minBoss;
      }
      var preUpgrade = firstUpgrade.Visited;
      if (!options_["skipprint"]) {
        Writers.SpoilerLogs.WriteLine(
            $"Areas required before {maybeName(firstUpgrade.Area)}: {string.Join("; ", preUpgrade.Select(maybeName))}");
        Writers.SpoilerLogs.WriteLine($"Other areas are not necessary to get there.");
        Writers.SpoilerLogs.WriteLine();
      }

      foreach (string area in upgradeAreas) {
        if (options_["explain"])
          Console.WriteLine(
              $"Blacksmith {area}: {string.Join(", ", check.Records[area].Visited)}");
      }
      this.graph_.AreaRatios = new Dictionary<string, (float, float)>();
      int k = 0;

      float getRatioMeasure(float cost, float maxRatio) {
        return 1 + (maxRatio - 1) * cost;
      }

      foreach (var rec in check.Records.Values.OrderBy(r => r.Dist)) {
        float desiredCost = k < vCosts.Count ? vCosts[k] : 1;
        if (!this.graph_.Areas[rec.Area].HasTag("optional")) k++;
        bool isBoss = this.graph_.Areas[rec.Area].HasTag("boss");
        bool preBlacksmithBoss = preUpgrade.Contains(rec.Area) && isBoss;
        if (preBlacksmithBoss && desiredCost > 0.05) desiredCost = 0.05f;
        float ratio = 1;
        float dmgRatio = 1;
        if (this.graph_.Areas[rec.Area].HasTag("end")) {
          // Keep ratio 1
        } else if (annotationData_.DefaultCost.TryGetValue(
            rec.Area,
            out float defaultCost)
        ) {
          // This scaling constant factor is a bit tricky to tune.
          // Originally used 400-1100, based on HP scaling over the course of a game. This seems to better match expected boss HP.
          // Possible ratio range: 0.3 to 3 in DS1
          ratio = getRatioMeasure(desiredCost, annotationData_.HealthScaling) /
                  getRatioMeasure(defaultCost, annotationData_.HealthScaling);
          // If it's randomized to past 70% of the way, don't make it easier.
          if (ratio < 1 && ((double) k / check.Records.Count) > 0.7) {
            ratio = 1;
          }
          // If it's early enough in vanilla (i.e. before expected access to blacksmith in DS1), don't make it easier either.
          else if (defaultCost <=
                   (annotationData_.DefaultCost.TryGetValue(
                        Flags.IsDs1
                            ? "parish_church"
                            : "settlement",
                        out float val)
                        ? val
                        : 0.25) &&
                   ratio < 1) {
            ratio = 1;
          } else {
            // Damage does not scale as much
            // Possible ratio range: 0.5 to 2 in DS1
            dmgRatio =
                getRatioMeasure(desiredCost, annotationData_.DamageScaling) /
                getRatioMeasure(defaultCost, annotationData_.DamageScaling);
          }
        }

        PostProcessScaling(ref ratio, ref dmgRatio);
        this.graph_.AreaRatios[rec.Area] = (ratio, dmgRatio);

        if (options_["skipprint"]) continue;
        // Print out the connectivity info for spoiler logs
        if (rec.Area == (Flags.IsDs1 ? "anorlondo_os" : "firelink"))
          Writers.SpoilerLogs.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        string areas = options_["debugareas"]
                           ? $" [{string.Join(",", new SortedSet<string>(rec.Visited))}]"
                           : "";
        string scaling =
            options_["scale"] ? $" (scaling: {ratio * 100:0.}%)" : "";
        string explainCost =
            options_["explain"] ? $" {desiredCost * 100:0.}%" : "";

        Writers.SpoilerLogs.WriteLine($"{maybeName(rec.Area)}{explainCost}" +
                                      scaling +
                                      areas +
                                      (isBoss ? " <----" : ""));
        foreach (var entry in rec.InEdge.OrderBy(e => e.Value)) {
          var e = entry.Key;
          var itemAreas = e.LinkedExpr == null
                              ? new List<string>()
                              : e.LinkedExpr.FreeVars()
                                 .SelectMany(
                                     a => graph_.ItemAreas.TryGetValue(
                                              a,
                                              out List<string> deps)
                                              ? deps
                                              : new List<string>())
                                 .Distinct()
                                 .ToList();
          var itemDeps = itemAreas.Count == 0
                             ? ""
                             : $", an item from {string.Join(" and ", itemAreas.Select(a => maybeName(a)))}";

          // Don't print entry.Value directly (distance of edge) - hard to visualize
          if (e.Text == e.Link.Text) {
            Writers.SpoilerLogs.WriteLine(
                $"  Preexisting: From {maybeName(e.From)} to {maybeName(rec.Area)} ({e.Text}{itemDeps})");
          } else {
            Writers.SpoilerLogs.WriteLine(
                $"  Random: From {maybeName(e.From)} ({e.Text}) to {maybeName(rec.Area)} ({e.Link.Text}{itemDeps})");
          }
        }
      }
      if (options_["dumpdist"]) {
        foreach (var entry in distances) {
          var area = this.graph_.Areas[entry.Key];
          if (area.HasTag("optional")) continue;
          Console.WriteLine(
              $"{entry.Key}: {entry.Value}  # SL {(int) (10 + (Flags.IsDs1 ? 60 : 70) * entry.Value)}");
        }
      }
      Writers.SpoilerLogs.WriteLine($"Finished {options_.DisplaySeed} at try {tries}");
      if (options_["explain"])
        Console.WriteLine(
            $"Pre-Blacksmith areas ({firstUpgrade.Area}): {string.Join(", ", preUpgrade)}");

      if (options_["dumpgraph"]) {
        this.DumpGraph_();
      }
    }

    private void DumpGraph_() {
      var graphNodes = this.graph_.Nodes;

      Console.WriteLine("Writing ../fog.dot");
      bool bi = false;
      TextWriter dot = File.CreateText(@"..\fog.dot");
      dot.WriteLine($"{(bi ? "di" : "")}graph {{");

      // dot.WriteLine("  nodesep=0.1; ranksep=0.1; ");
      string escape(object o) {
        if (o == null) return "";
        return o.ToString().Replace("\n", "\\l").Replace("\"", "\\\"") +
               "\\l";
      }

      foreach (var node in graphNodes.Values) {
        var label = node.Area;
        label = label == "" ? "(empty)" : label;
        dot.WriteLine(
            $"    \"{node.Area}\" [ shape=box,label=\"{escape(label)}\" ];");
      }
      var oneCons = new HashSet<Connection>();
      foreach (var from in graphNodes.Values) {
        foreach (var e in from.To) {
          var con = new Connection(e.From, e.To);
          if (oneCons.Contains(con)) {
            continue;
          }
          if (!bi) oneCons.Add(con);
          // Node to = e.To;
          var toKey = e.To;
          var style = "solid";
          string label = null; // $"{e.LinkedExpr}";
          dot.WriteLine(
              $"  \"{from.Area}\" -{(bi ? ">" : "-")} \"{toKey}\" [ style={style},labelloc=t,label=\"{escape(label)}\" ];");
        }
      }
      dot.WriteLine("}");
      dot.Close();
    }
  }
}