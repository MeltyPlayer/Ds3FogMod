// Decompiled with JetBrains decompiler
// Type: FogMod.Graph
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsIds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FogMod
{
  public class Graph
  {
    public Dictionary<string, AnnotationData.Area> Areas { get; set; }

    public Dictionary<string, AnnotationData.Entrance> EntranceIds { get; set; }

    public Dictionary<string, List<string>> ItemAreas { get; set; }

    public Dictionary<string, Graph.Node> Nodes { get; set; }

    public AnnotationData.CustomStart Start { get; set; }

    public List<(string, string)> Ignore { get; set; }

    public Dictionary<string, (float, float)> AreaRatios { get; set; }

    public (Graph.Edge, Graph.Edge) AddNode(
      AnnotationData.Side side,
      AnnotationData.Entrance e,
      bool from,
      bool to)
    {
      string fullName = e?.FullName;
      string str = e == null ? side.Text ?? (side.HasTag("hard") ? "hard skip" : "in map") : e.Text;
      bool flag = e == null || e.IsFixed;
      Graph.Edge edge1;
      if (!from)
      {
        edge1 = (Graph.Edge) null;
      }
      else
      {
        edge1 = new Graph.Edge();
        edge1.Expr = side.Expr;
        edge1.From = side.Area;
        edge1.Name = fullName;
        edge1.Text = str;
        edge1.IsFixed = flag;
        edge1.Side = side;
        edge1.Type = Graph.EdgeType.Exit;
      }
      Graph.Edge edge2 = edge1;
      Graph.Edge edge3;
      if (!to)
      {
        edge3 = (Graph.Edge) null;
      }
      else
      {
        edge3 = new Graph.Edge();
        edge3.Expr = side.Expr;
        edge3.To = side.Area;
        edge3.Name = fullName;
        edge3.Text = str;
        edge3.IsFixed = flag;
        edge3.Side = side;
        edge3.Type = Graph.EdgeType.Entrance;
      }
      Graph.Edge edge4 = edge3;
      if (edge4 != null)
      {
        edge4.Pair = edge2;
        this.Nodes[side.Area].From.Add(edge4);
      }
      if (edge2 != null)
      {
        edge2.Pair = edge4;
        this.Nodes[side.Area].To.Add(edge2);
      }
      return (edge2, edge4);
    }

    public void Connect(Graph.Edge exit, Graph.Edge entrance)
    {
      if (exit.To != null || entrance.From != null || (exit.Link != null || entrance.Link != null))
        throw new Exception("Already matched");
      exit.To = entrance.To;
      entrance.From = exit.From;
      entrance.Link = exit;
      exit.Link = entrance;
      AnnotationData.Expr expr1;
      if (entrance.Expr == null)
        expr1 = exit.Expr;
      else if (exit.Expr == null)
      {
        expr1 = entrance.Expr;
      }
      else
      {
        AnnotationData.Expr expr2;
        if (exit.Expr != entrance.Expr)
          expr2 = new AnnotationData.Expr(new List<AnnotationData.Expr>()
          {
            exit.Expr,
            entrance.Expr
          }, true, (string) null).Simplify();
        else
          expr2 = exit.Expr;
        expr1 = expr2;
      }
      entrance.LinkedExpr = exit.LinkedExpr = expr1;
      if (exit == entrance.Pair)
        return;
      if (exit.Pair != null)
      {
        if (exit.Pair.From != null || exit.Pair.Link != null)
          throw new Exception("Already matched pair");
        exit.Pair.From = exit.To;
      }
      if (entrance.Pair != null)
      {
        if (entrance.Pair.To != null || entrance.Pair.Link != null)
          throw new Exception("Already matched pair");
        entrance.Pair.To = entrance.From;
      }
      if (exit.Pair == null || entrance.Pair == null)
        return;
      exit.Pair.Link = entrance.Pair;
      entrance.Pair.Link = exit.Pair;
      exit.Pair.LinkedExpr = entrance.Pair.LinkedExpr = expr1;
    }

    public void Disconnect(Graph.Edge exit, bool forPair = false)
    {
      Graph.Edge link = exit.Link;
      if (link == null)
        throw new Exception(string.Format("Can't disconnect {0}{1}", (object) exit, forPair ? (object) " as pair" : (object) ""));
      exit.Link = (Graph.Edge) null;
      exit.To = (string) null;
      link.Link = (Graph.Edge) null;
      link.From = (string) null;
      link.LinkedExpr = exit.LinkedExpr = (AnnotationData.Expr) null;
      if (forPair || exit.Pair == null || (link.Pair == null || exit.Pair == link))
        return;
      this.Disconnect(link.Pair, true);
    }

    public void SwapConnectedEdges(Graph.Edge oldExitEdge, Graph.Edge newEntranceEdge)
    {
      Graph.Edge entrance = newEntranceEdge;
      Graph.Edge link1 = newEntranceEdge.Link;
      Graph.Edge exit = oldExitEdge;
      Graph.Edge link2 = oldExitEdge.Link;
      this.Disconnect(link1, false);
      this.Disconnect(exit, false);
      if (entrance == link1.Pair && link2 == exit.Pair)
        this.Connect(exit, entrance);
      else if (entrance == link1.Pair)
      {
        if (link2.Pair != null)
        {
          this.Connect(link2.Pair, link2);
          this.Connect(exit, entrance);
        }
        else
        {
          if (exit.Pair == null)
            throw new Exception(string.Format("Bad seed: Can't find edge to self-link to reach {0}", (object) entrance));
          this.Connect(exit, exit.Pair);
          this.Connect(link1, link2);
        }
      }
      else if (link2 == exit.Pair)
      {
        if (entrance.Pair != null)
        {
          this.Connect(entrance.Pair, entrance);
          this.Connect(link1, link2);
        }
        else
        {
          if (link1.Pair == null)
            throw new Exception(string.Format("Bad seed: Can't find edge to self-link to reach {0}", (object) entrance));
          this.Connect(link1, link1.Pair);
          this.Connect(exit, entrance);
        }
      }
      else
      {
        this.Connect(exit, entrance);
        this.Connect(link1, link2);
      }
    }

    public void SwapConnectedAreas(string name1, string name2)
    {
      Graph.Node node1 = this.Nodes[name1];
      Graph.Node node2 = this.Nodes[name2];
      for (int index1 = 0; index1 <= 1; ++index1)
      {
        bool unpaired = index1 == 0;
        List<Graph.Edge> list1 = node1.From.Where<Graph.Edge>((Func<Graph.Edge, bool>) (e => !e.IsFixed && e.Pair == null == unpaired)).ToList<Graph.Edge>();
        List<Graph.Edge> list2 = node2.From.Where<Graph.Edge>((Func<Graph.Edge, bool>) (e => !e.IsFixed && e.Pair == null == unpaired)).ToList<Graph.Edge>();
        list2.Reverse();
        for (int index2 = 0; index2 < Math.Min(list1.Count, list2.Count); ++index2)
          this.SwapConnectedEdges(list1[index2].Link, list2[index2]);
      }
    }

    public void Construct(RandomizerOptions opt, AnnotationData ann)
    {
      this.Areas = ann.Areas.ToDictionary<AnnotationData.Area, string, AnnotationData.Area>((Func<AnnotationData.Area, string>) (a => a.Name), (Func<AnnotationData.Area, AnnotationData.Area>) (a => a));
      this.ItemAreas = ann.KeyItems.ToDictionary<AnnotationData.Item, string, List<string>>((Func<AnnotationData.Item, string>) (item => item.Name), (Func<AnnotationData.Item, List<string>>) (item => new List<string>()));
      foreach (AnnotationData.Area area in ann.Areas)
      {
        if (area.To != null)
        {
          foreach (AnnotationData.Side side in area.To)
          {
            if (!this.Areas.ContainsKey(side.Area))
              throw new Exception(area.Name + " goes to nonexistent " + side.Area);
            side.Expr = getExpr(side.Cond);
          }
        }
      }
      this.EntranceIds = new Dictionary<string, AnnotationData.Entrance>();
      foreach (AnnotationData.Entrance entrance in ann.Entrances.Concat<AnnotationData.Entrance>((IEnumerable<AnnotationData.Entrance>) ann.Warps))
      {
        string fullName = entrance.FullName;
        if (this.EntranceIds.ContainsKey(fullName))
          throw new Exception("Duplicate id " + fullName);
        this.EntranceIds[fullName] = entrance;
        if (!entrance.HasTag("unused") && entrance.Sides().Count < 2)
          throw new Exception(entrance.FullName + " has insufficient sides");
      }
      foreach (AnnotationData.Entrance warp in ann.Warps)
      {
        if (!warp.HasTag("unused") && (warp.ASide == null || warp.BSide == null))
          throw new Exception(warp.FullName + " warp missing both sides");
      }
      Dictionary<string, List<string>> dictionary1 = new Dictionary<string, List<string>>();
      foreach (AnnotationData.Entrance entrance in ann.Entrances)
      {
        if (!entrance.HasTag("unused"))
        {
          if (opt.Game == GameSpec.FromGame.DS3)
          {
            if (entrance.HasTag("norandom"))
              entrance.IsFixed = true;
            else if (entrance.HasTag("door"))
              entrance.IsFixed = true;
            else if (opt["lords"] && entrance.HasTag("kiln"))
              entrance.IsFixed = true;
            else if (!opt["boss"] && entrance.HasTag("boss"))
            {
              entrance.IsFixed = true;
              if (opt["dumptext"])
                Util.AddMulti<string, string>((IDictionary<string, List<string>>) dictionary1, "boss", entrance.Text);
            }
            else if (!opt["pvp"] && entrance.HasTag("pvp"))
            {
              entrance.IsFixed = true;
              if (opt["dumptext"])
                Util.AddMulti<string, string>((IDictionary<string, List<string>>) dictionary1, "pvp", entrance.Text);
            }
          }
          else
          {
            if (!opt["lordvessel"] && entrance.HasTag("lordvessel"))
            {
              entrance.Tags += " door";
              entrance.DoorCond = "AND lordvessel kiln_start";
              if (opt["dumptext"])
                Util.AddMulti<string, string>((IDictionary<string, List<string>>) dictionary1, "lordvessel", entrance.Text);
            }
            if (entrance.HasTag("door"))
              entrance.IsFixed = true;
            else if (opt["lords"] && entrance.Area == "kiln")
              entrance.IsFixed = true;
            else if (!opt["world"] && entrance.HasTag("world"))
            {
              entrance.IsFixed = true;
              if (opt["dumptext"])
                Util.AddMulti<string, string>((IDictionary<string, List<string>>) dictionary1, "world", entrance.Text);
            }
            else if (!opt["boss"] && entrance.HasTag("boss"))
            {
              entrance.IsFixed = true;
              if (opt["dumptext"])
                Util.AddMulti<string, string>((IDictionary<string, List<string>>) dictionary1, "boss", entrance.Text);
            }
            else if (!opt["minor"] && entrance.HasTag("pvp") && !entrance.HasTag("major"))
            {
              entrance.IsFixed = true;
              if (opt["dumptext"])
                Util.AddMulti<string, string>((IDictionary<string, List<string>>) dictionary1, "minor", entrance.Text);
            }
            else if (!opt["major"] && entrance.HasTag("pvp") && entrance.HasTag("major"))
            {
              entrance.IsFixed = true;
              if (opt["dumptext"])
                Util.AddMulti<string, string>((IDictionary<string, List<string>>) dictionary1, "major", entrance.Text);
            }
          }
        }
      }
      foreach (AnnotationData.Entrance warp in ann.Warps)
      {
        if (warp.HasTag("highwall"))
        {
          if (!opt["pvp"] && !opt["boss"])
            warp.TagList.Add("norandom");
          else
            warp.TagList.Add("unused");
        }
        if (!warp.HasTag("unused"))
        {
          if (warp.HasTag("norandom"))
            warp.IsFixed = true;
          else if (!opt["warp"])
          {
            warp.IsFixed = true;
            if (opt["dumptext"])
              Util.AddMulti<string, string>((IDictionary<string, List<string>>) dictionary1, "warp", warp.Text);
          }
          if (opt["lords"] && warp.HasTag("kiln"))
            warp.IsFixed = true;
        }
      }
      if (opt["dumptext"] && dictionary1.Count > 0)
      {
        foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary1)
        {
          Console.WriteLine(keyValuePair.Key);
          foreach (string str in keyValuePair.Value)
            Console.WriteLine("- " + str);
          Console.WriteLine();
        }
      }
      this.Ignore = new List<(string, string)>();
      foreach (AnnotationData.Entrance entrance in ann.Entrances.Concat<AnnotationData.Entrance>((IEnumerable<AnnotationData.Entrance>) ann.Warps))
      {
        foreach (AnnotationData.Side side in entrance.Sides())
        {
          if (!this.Areas.ContainsKey(side.Area))
            throw new Exception(entrance.FullName + " goes to nonexistent " + side.Area);
          side.Expr = getExpr(side.Cond);
          if (!entrance.IsFixed && side.ExcludeIfRandomized != null && !this.EntranceIds[side.ExcludeIfRandomized].IsFixed)
            this.Ignore.Add((entrance.FullName, side.Area));
        }
      }
      this.Nodes = this.Areas.ToDictionary<KeyValuePair<string, AnnotationData.Area>, string, Graph.Node>((Func<KeyValuePair<string, AnnotationData.Area>, string>) (e => e.Key), (Func<KeyValuePair<string, AnnotationData.Area>, Graph.Node>) (e => new Graph.Node()
      {
        Area = e.Key,
        Cost = areaCost(e.Value),
        ScalingBase = opt["dumpdist"] || opt["scalingbase"] ? e.Value.ScalingBase : (string) null
      }));
      foreach (AnnotationData.Area area in ann.Areas)
      {
        if (area.To != null)
        {
          foreach (AnnotationData.Side side1 in area.To)
          {
            if (!side1.HasTag("temp") && (!side1.HasTag("hard") || opt["hard"]) && ((!side1.HasTag("treeskip") || opt["treeskip"]) && (!side1.HasTag("instawarp") || opt["instawarp"])))
            {
              AnnotationData.Side side2 = new AnnotationData.Side();
              side2.Area = area.Name;
              side2.Text = side1.Text;
              side2.Expr = side1.Expr;
              side2.Tags = side1.Tags;
              AnnotationData.Side side3 = side2;
              AnnotationData.Side side4 = new AnnotationData.Side();
              side4.Area = side1.Area;
              side4.Text = side1.Text;
              side4.Tags = side1.HasTag("hard") ? "hard" : (string) null;
              AnnotationData.Side side5 = side4;
              bool flag = side1.HasTag("shortcut");
              if (flag)
              {
                AnnotationData.Expr expr = AnnotationData.Expr.Named(area.Name);
                if (side1.Expr != null)
                  expr = new AnnotationData.Expr(new List<AnnotationData.Expr>()
                  {
                    expr,
                    side1.Expr
                  }, true, (string) null).Simplify();
                side5.Expr = expr;
              }
              this.Connect(this.AddNode(side3, (AnnotationData.Entrance) null, true, flag).Item1, this.AddNode(side5, (AnnotationData.Entrance) null, flag, true).Item2);
            }
          }
        }
      }
      Dictionary<Graph.Connection, List<(Graph.Edge, Graph.Edge)>> dictionary2 = new Dictionary<Graph.Connection, List<(Graph.Edge, Graph.Edge)>>();
      foreach (AnnotationData.Entrance warp in ann.Warps)
      {
        if (!warp.ASide.HasTag("temp") && !warp.HasTag("unused"))
        {
          Graph.Edge exit = this.AddNode(warp.ASide, warp, true, false).Item1;
          Graph.Edge entrance = this.AddNode(warp.BSide, warp, false, true).Item2;
          if (warp.IsFixed)
            this.Connect(exit, entrance);
          else if (!warp.HasTag("unique"))
            Util.AddMulti<Graph.Connection, (Graph.Edge, Graph.Edge)>((IDictionary<Graph.Connection, List<(Graph.Edge, Graph.Edge)>>) dictionary2, new Graph.Connection(warp.ASide.Area, warp.BSide.Area), (exit, entrance));
        }
      }
      foreach (KeyValuePair<Graph.Connection, List<(Graph.Edge, Graph.Edge)>> keyValuePair in dictionary2)
      {
        if (keyValuePair.Value.Count != 2)
          throw new Exception(string.Format("Bidirectional warp expected for {0} - non-bidirectional should be marked unique", (object) keyValuePair.Key));
        if (!opt["unconnected"])
        {
          (Graph.Edge edge17, Graph.Edge edge18) = keyValuePair.Value[0];
          (Graph.Edge edge19, Graph.Edge edge20) = keyValuePair.Value[1];
          if (edge17.From == edge19.From)
            throw new Exception(string.Format("Duplicate warp {0} and {1} - should be marked unique", (object) edge17, (object) edge19));
          if (edge17.From != edge20.To)
            throw new Exception(string.Format("Internal error: warp {0} and {1} not equivalent", (object) edge17, (object) edge20));
          edge17.Pair = edge20;
          edge20.Pair = edge17;
          edge19.Pair = edge18;
          edge18.Pair = edge19;
        }
      }
      HashSet<Graph.Connection> connectionSet = new HashSet<Graph.Connection>();
      foreach (AnnotationData.Entrance entrance1 in ann.Entrances)
      {
        if (!entrance1.HasTag("unused"))
        {
          List<AnnotationData.Side> sideList = entrance1.Sides();
          if (sideList.Count == 1)
          {
            if (entrance1.HasTag("door"))
              throw new Exception(entrance1.FullName + " has one-sided door");
            this.AddNode(sideList[0], entrance1, true, true);
          }
          else
          {
            AnnotationData.Side side1 = sideList[0];
            AnnotationData.Side side2 = sideList[1];
            if (entrance1.HasTag("door"))
            {
              Graph.Connection connection = new Graph.Connection(side1.Area, side2.Area);
              if (!connectionSet.Contains(connection))
              {
                connectionSet.Add(connection);
                AnnotationData.Expr expr1 = getExpr(entrance1.DoorCond);
                if (side1.Expr != null || side2.Expr != null)
                  throw new Exception(string.Format("Door cond {0} and cond {1} {2} together for {3}", (object) expr1, (object) side1.Expr, (object) side2.Expr, (object) entrance1.FullName));
                AnnotationData.Side side3 = side1;
                AnnotationData.Expr expr2;
                if (!side1.HasTag("dnofts"))
                  expr2 = expr1;
                else if (expr1 != null)
                  expr2 = new AnnotationData.Expr(new List<AnnotationData.Expr>()
                  {
                    expr1,
                    AnnotationData.Expr.Named(side2.Area)
                  }, true, (string) null).Simplify();
                else
                  expr2 = AnnotationData.Expr.Named(side2.Area);
                side3.Expr = expr2;
                AnnotationData.Side side4 = side2;
                AnnotationData.Expr expr3;
                if (!side2.HasTag("dnofts"))
                  expr3 = expr1;
                else if (expr1 != null)
                  expr3 = new AnnotationData.Expr(new List<AnnotationData.Expr>()
                  {
                    expr1,
                    AnnotationData.Expr.Named(side1.Area)
                  }, true, (string) null).Simplify();
                else
                  expr3 = AnnotationData.Expr.Named(side1.Area);
                side4.Expr = expr3;
                this.Connect(this.AddNode(side1, entrance1, true, true).Item1, this.AddNode(side2, entrance1, true, true).Item2);
              }
            }
            else if (entrance1.IsFixed || !opt["unconnected"])
            {
              Graph.Edge exit = this.Ignore.Contains((entrance1.FullName, side2.Area)) ? (Graph.Edge) null : this.AddNode(side2, entrance1, true, true).Item1;
              Graph.Edge entrance2 = this.Ignore.Contains((entrance1.FullName, side1.Area)) ? (Graph.Edge) null : this.AddNode(side1, entrance1, true, true).Item2;
              if (exit != null && entrance2 != null && entrance1.IsFixed)
                this.Connect(exit, entrance2);
            }
            else
            {
              if (!this.Ignore.Contains((entrance1.FullName, side2.Area)))
              {
                this.AddNode(side2, entrance1, true, false);
                this.AddNode(side2, entrance1, false, true);
              }
              if (!this.Ignore.Contains((entrance1.FullName, side1.Area)))
              {
                this.AddNode(side1, entrance1, true, false);
                this.AddNode(side1, entrance1, false, true);
              }
            }
          }
        }
      }

      int areaCost(AnnotationData.Area a)
      {
        if (a.HasTag("trivial"))
          return 0;
        return a.HasTag("small") ? 1 : 3;
      }

      AnnotationData.Expr getExpr(string cond)
      {
        AnnotationData.Expr expr = AnnotationData.ParseExpr(cond);
        if (expr == null)
          return (AnnotationData.Expr) null;
        foreach (string freeVar in expr.FreeVars())
        {
          if (!this.Areas.ContainsKey(freeVar) && !this.ItemAreas.ContainsKey(freeVar))
            throw new Exception("Condition " + cond + " has unknown variable " + freeVar);
        }
        return expr;
      }
    }

    public class WarpPoint
    {
      public int ID { get; set; }

      public string Map { get; set; }

      public int Action { get; set; }

      public int Cutscene { get; set; }

      public bool ToFront { get; set; }

      public float Height { get; set; }

      public int Player { get; set; }

      public int Region { get; set; }
    }

    public class Connection
    {
      public string A { get; set; }

      public string B { get; set; }

      public Connection(string a, string b)
      {
        this.A = a.CompareTo(b) < 0 ? a : b;
        this.B = a.CompareTo(b) < 0 ? b : a;
      }

      public override bool Equals(object obj)
      {
        return obj is Graph.Connection o && this.Equals(o);
      }

      public bool Equals(Graph.Connection o)
      {
        return this.A == o.A && this.B == o.B;
      }

      public override int GetHashCode()
      {
        return this.A.GetHashCode() ^ this.B.GetHashCode();
      }

      public override string ToString()
      {
        return "(" + this.A + ", " + this.B + ")";
      }
    }

    public class Node
    {
      public List<string> Items = new List<string>();
      public List<Graph.Edge> To = new List<Graph.Edge>();
      public List<Graph.Edge> From = new List<Graph.Edge>();

      public string Area { get; set; }

      public int Cost { get; set; }

      public string ScalingBase { get; set; }
    }

    public class Edge
    {
      private Graph.Edge pair;
      private Graph.Edge link;

      public Graph.EdgeType Type { get; set; }

      public string From { get; set; }

      public string To { get; set; }

      public AnnotationData.Expr Expr { get; set; }

      public AnnotationData.Expr LinkedExpr { get; set; }

      public bool IsFixed { get; set; }

      public string Name { get; set; }

      public AnnotationData.Side Side { get; set; }

      public string Text { get; set; }

      public Graph.Edge Pair
      {
        get
        {
          return this.pair;
        }
        set
        {
          if (value != null && this.Type == value.Type)
            throw new Exception(string.Format("Cannot pair {0} and {1}", (object) this, (object) value));
          this.pair = value;
        }
      }

      public Graph.Edge Link
      {
        get
        {
          return this.link;
        }
        set
        {
          if (value != null && this.Type == value.Type)
            throw new Exception(string.Format("Cannot link {0} and {1}", (object) this, (object) value));
          this.link = value;
        }
      }

      public override string ToString()
      {
        return string.Format("Edge[Name={0}, From={1}, To={2}, Expr={3}]", (object) this.Name, (object) this.From, (object) this.To, (object) this.Expr);
      }
    }

    public enum EdgeType
    {
      Unknown,
      Exit,
      Entrance,
    }
  }
}
