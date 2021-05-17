// Decompiled with JetBrains decompiler
// Type: FogMod.ItemReader
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using FogMod.util.time;
using SoulsFormats;

using SoulsIds;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FogMod {
  public class ItemReader {
    public async Task<ItemReader.Result> FindItems(
        RandomizerOptions opt,
        AnnotationData ann,
        Graph g,
        Events events,
        string gameDir,
        GameEditor gameEditor) {
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      var dictionary1 =
          ann.KeyItems.ToDictionary(item => item.ID, item => item.Name);
      var itemAreas = g.ItemAreas;

      var layouts = await gameEditor.LoadLayouts();

      if (ann.LotLocations != null) {
        Dictionary<int, PARAM.Row> dictionary2 =
            (await gameEditor.LoadParams(layouts, false))
                      ["ItemLotParam"]
                      .Rows.ToDictionary<PARAM.Row, int, PARAM.Row>(
                          (Func<PARAM.Row, int>) (r => (int) r.ID),
                          (Func<PARAM.Row, PARAM.Row>) (r => r));
        using (Dictionary<int, string>.Enumerator enumerator =
            ann.LotLocations.GetEnumerator()) {
          label_18:
          while (enumerator.MoveNext()) {
            KeyValuePair<int, string> current = enumerator.Current;
            int key1 = current.Key;
            if (!g.Areas.ContainsKey(current.Value))
              throw new Exception(string.Format(
                                      "Internal error in lot config for {0}: {1} does not exist",
                                      (object) current.Key,
                                      (object) current.Value));
            while (true) {
              PARAM.Row row;
              if (dictionary2.TryGetValue(key1, out row)) {
                for (int index1 = 1; index1 <= 8; ++index1) {
                  int num1 =
                      (int) row[string.Format("lotItemId0{0}", (object) index1)]
                          .Value;
                  if (num1 != 0) {
                    int num2 =
                        (int) row[
                                string.Format("lotItemCategory0{0}",
                                              (object) index1)]
                            .Value;
                    int num3;
                    int num4 =
                        Universe.LotTypes.TryGetValue((uint) num2, out num3)
                            ? num3
                            : -1;
                    if (num4 != -1) {
                      string key2 =
                          $"{(object) num4}:{(object) num1}";
                      if (opt["debuglots"])
                        Console.WriteLine(
                            $"{current.Key} in {current.Value} has {key2}");
                      string index2;
                      if (dictionary1.TryGetValue(key2, out index2)) {
                        if (itemAreas[index2].Count > 0 &&
                            itemAreas[index2][0] != current.Value)
                          throw new Exception(
                              "Item " +
                              index2 +
                              " found in both " +
                              itemAreas[index2][0] +
                              " and " +
                              current.Value);
                        itemAreas[index2] = new List<string>() {
                            current.Value
                        };
                      }
                    }
                  }
                }
                ++key1;
              } else
                goto label_18;
            }
          }
        }
      }
      stopwatch.ResetAndPrint("  Working w/ lotlocations");

      if (ann.Locations != null) {
        var locationStopwatch = new Stopwatch();
        locationStopwatch.Start();

        int num1 = -1;
        Dictionary<string, PARAM> dictionary2 =
            await ParamsManager.Get(gameDir, events, gameEditor);
        locationStopwatch.ResetAndPrint("    Locations 0");

        if (gameDir != null) {
          string path2 = gameDir + "\\event\\common.emevd.dcx";
          if (File.Exists(path2)) {
            EMEVD.Event @event = SoulsFile<EMEVD>
                                 .Read(path2)
                                 .Events.Find(
                                     (Predicate<EMEVD.Event>) (e => e.ID ==
                                                                    13000904L));
            if (@event != null) {
              num1 = (int) events.Parse(@event.Instructions[1],
                                        false,
                                        false)[3];
              if (opt["debuglots"])
                Console.WriteLine(
                    string.Format("Dragon flag: {0}", (object) num1));
            }
          }
        }
        locationStopwatch.ResetAndPrint("    Locations 1");

        Dictionary<int, PARAM.Row> dictionary3 = dictionary2["ItemLotParam"]
                                                 .Rows.ToDictionary<PARAM.Row,
                                                     int, PARAM.Row>(
                                                     (Func<PARAM.Row, int>)
                                                     (r => (int) r.ID),
                                                     (Func<PARAM.Row, PARAM.Row>
                                                     ) (r => r));
        Dictionary<int, PARAM.Row> dictionary4 = dictionary2["ShopLineupParam"]
                                                 .Rows.ToDictionary<PARAM.Row,
                                                     int, PARAM.Row>(
                                                     (Func<PARAM.Row, int>)
                                                     (r => (int) r.ID),
                                                     (Func<PARAM.Row, PARAM.Row>
                                                     ) (r => r));
        locationStopwatch.ResetAndPrint("    Locations 2");

        foreach (AnnotationData.KeyItemLoc keyItemLoc in ann.Locations.Items) {
          List<string> list = ((IEnumerable<string>) keyItemLoc.Area.Split(' '))
              .ToList<string>();
          if (!list.All<string>(
                  (Func<string, bool>) (a => g.Areas.ContainsKey(a) ||
                                             itemAreas.ContainsKey(a))))
            throw new Exception("Warning: Areas not found for " +
                                keyItemLoc.Area +
                                " - " +
                                keyItemLoc.DebugText[0]);
          List<int> intList1;
          if (keyItemLoc.Lots != null)
            intList1 = ((IEnumerable<string>) keyItemLoc.Lots.Split(' '))
                       .Select<string, int>(
                           (Func<string, int>) (i => int.Parse(i)))
                       .ToList<int>();
          else
            intList1 = new List<int>();
          using (List<int>.Enumerator enumerator = intList1.GetEnumerator()) {
            label_51:
            while (enumerator.MoveNext()) {
              int current = enumerator.Current;
              while (true) {
                PARAM.Row row;
                if (dictionary3.TryGetValue(current, out row)) {
                  for (int index = 1; index <= 8; ++index) {
                    int num2 =
                        (int) row[string.Format("ItemLotId{0}", (object) index)]
                            .Value;
                    if (num2 != 0) {
                      uint key1 =
                          (uint) row[
                                  string.Format("LotItemCategory0{0}",
                                                (object) index)]
                              .Value;
                      int num3;
                      if (Universe.LotTypes.TryGetValue(key1, out num3)) {
                        string key2 =
                            string.Format("{0}:{1}",
                                          (object) num3,
                                          (object) num2);
                        if (opt["debuglots"])
                          Console.WriteLine(
                              string.Format("lot {0} in {1} has {2}",
                                            (object) current,
                                            (object) keyItemLoc.Area,
                                            (object) key2));
                        string itemName;
                        if (dictionary1.TryGetValue(key2, out itemName))
                          setArea(itemName, list);
                      }
                    }
                  }
                  if (num1 > 0 && (int) row["getItemFlagId"].Value == num1)
                    setArea("pathofthedragon", list);
                  ++current;
                } else
                  goto label_51;
              }
            }
          }
          List<int> intList2;
          if (keyItemLoc.Shops != null)
            intList2 = ((IEnumerable<string>) keyItemLoc.Shops.Split(' '))
                       .Select<string, int>(
                           (Func<string, int>) (i => int.Parse(i)))
                       .ToList<int>();
          else
            intList2 = new List<int>();
          foreach (int key1 in intList2) {
            PARAM.Row row;
            if (dictionary4.TryGetValue(key1, out row)) {
              int num2 = (int) row["EquipId"].Value;
              string key2 = string.Format("{0}:{1}",
                                          (object) (int) (byte) row["equipType"]
                                              .Value,
                                          (object) num2);
              if (opt["debuglots"])
                Console.WriteLine(string.Format("shop {0} in {1} has {2}",
                                                (object) key1,
                                                (object) keyItemLoc.Area,
                                                (object) key2));
              string itemName;
              if (dictionary1.TryGetValue(key2, out itemName))
                setArea(itemName, list);
              if (num1 > 0 && (int) row["EventFlag"].Value == num1)
                setArea("pathofthedragon", list);
            }
          }
        }
        locationStopwatch.ResetAndPrint("    Locations 3");
      }
      stopwatch.ResetAndPrint("  Working w/ locations");

      bool flag1;
      do {
        flag1 = false;
        foreach (KeyValuePair<string, List<string>> keyValuePair in itemAreas) {
          foreach (string key in keyValuePair.Value.ToList<string>()) {
            List<string> stringList;
            if (itemAreas.TryGetValue(key, out stringList)) {
              keyValuePair.Value.Remove(key);
              keyValuePair.Value.AddRange((IEnumerable<string>) stringList);
              flag1 = true;
            }
          }
        }
      } while (flag1);
      stopwatch.ResetAndPrint("  Working w/ item areas");

      if (opt["explain"] || opt["debuglots"]) {
        foreach (AnnotationData.Item keyItem in ann.KeyItems)
          Console.WriteLine(keyItem.Name +
                            " " +
                            keyItem.ID +
                            ": default " +
                            keyItem.Area +
                            ", found [" +
                            string.Join(", ",
                                        (IEnumerable<string>) itemAreas[
                                            keyItem.Name]) +
                            "]");
      }
      stopwatch.ResetAndPrint("  Printing about item areas");

      SortedSet<string> sortedSet = new SortedSet<string>();
      bool flag2 = false;
      foreach (AnnotationData.Item keyItem in ann.KeyItems) {
        if (itemAreas[keyItem.Name].Count == 0) {
          if (keyItem.HasTag("randomonly")) {
            itemAreas[keyItem.Name] = new List<string>() {
                keyItem.Area
            };
          } else {
            if (!keyItem.HasTag("hard") || opt["hard"])
              throw new Exception("Couldn't find " +
                                  keyItem.Name +
                                  " in item lots");
            continue;
          }
        }
        List<string> stringList = itemAreas[keyItem.Name];
        foreach (string index in stringList)
          g.Nodes[index].Items.Add(keyItem.Name);
        if (!keyItem.HasTag("randomonly")) {
          if (stringList.Count > 1 || stringList[0] != keyItem.Area)
            flag2 = true;
          sortedSet.Add(keyItem.Name +
                        "=" +
                        string.Join(",", (IEnumerable<string>) stringList));
        }
      }
      stopwatch.ResetAndPrint("  Gathering items?");

      return new ItemReader.Result() {
          Randomized = flag2,
          ItemHash =
              (RandomizerOptions.JavaStringHash(
                   string.Join(";", (IEnumerable<string>) sortedSet) ?? "") %
               99999U).ToString()
                      .PadLeft(5, '0')
      };

      void setArea(string itemName, List<string> areas) {
        if (opt["debuglots"])
          Console.WriteLine("-- name: " + itemName);
        if (itemAreas[itemName].Count > 0 &&
            !itemAreas[itemName]
                .SequenceEqual<string>((IEnumerable<string>) areas))
          throw new Exception("Item " +
                              itemName +
                              " found in both " +
                              string.Join(",",
                                          (IEnumerable<string>) itemAreas[
                                              itemName]) +
                              " and " +
                              string.Join(",", (IEnumerable<string>) areas));
        itemAreas[itemName] = areas;
      }
    }

    public class Result {
      public bool Randomized { get; set; }

      public string ItemHash { get; set; }
    }
  }
}