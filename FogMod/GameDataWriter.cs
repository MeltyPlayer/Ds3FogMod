// Decompiled with JetBrains decompiler
// Type: FogMod.GameDataWriter
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsFormats;
using SoulsIds;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace FogMod
{
  public class GameDataWriter
  {
    private int tempColEventBase = 5350;
    private HashSet<int> enemyRandoEvents = new HashSet<int>(((IEnumerable<int>) new int[17]
    {
      11009000,
      11019000,
      11029000,
      11109000,
      11209000,
      11219000,
      11309000,
      11319000,
      11329000,
      11409000,
      11419000,
      11509000,
      11519000,
      11609000,
      11709000,
      11809000,
      11819000
    }).SelectMany<int, int>((Func<int, IEnumerable<int>>) (i => (IEnumerable<int>) new int[3]
    {
      i - 200,
      i,
      i + 200
    })).Concat<int>((IEnumerable<int>) new int[4]
    {
      11211600,
      11505600,
      11515600,
      11700800
    }));

    public async Task WriteAsync(
      RandomizerOptions opt,
      AnnotationData ann,
      Graph g,
      string gameDir,
      GameSpec.FromGame game)
    {
      GameEditor gameEditor = new GameEditor(game);
      bool flag1 = game == GameSpec.FromGame.DS1R;
      string str1 = string.Format("dist\\{0}", (object) game);
      gameEditor.Spec.GameDir = str1;
      Dictionary<string, MSB1> dictionary1 = gameEditor.Load<MSB1>("map\\MapStudio", (Func<string, MSB1>) (path => !ann.Specs.ContainsKey(Path.GetFileNameWithoutExtension(path)) ? (MSB1) null : SoulsFile<MSB1>.Read(path)), "*.msb");
      Dictionary<string, PARAM.Layout> layouts = gameEditor.LoadLayouts();
      Dictionary<string, PARAM> dictionary2 = gameEditor.LoadParams(layouts, false);
      gameEditor.Spec.GameDir = gameDir;
      Dictionary<string, MSB1> dictionary3 = gameEditor.Load<MSB1>("map\\MapStudio", (Func<string, MSB1>) (path => !ann.Specs.ContainsKey(Path.GetFileNameWithoutExtension(path)) ? (MSB1) null : SoulsFile<MSB1>.Read(path)), "*.msb");
      Dictionary<string, PARAM> diffData1 = gameEditor.LoadParams(layouts, false);
      string str2 = flag1 ? ".dcx" : "";
      string fmgEvent = flag1 ? "Event_text_" : "イベントテキスト";
      Dictionary<string, FMG> diffData2 = gameEditor.LoadBnd<FMG>(gameEditor.Spec.GameDir + "\\msg\\" + opt.Language + "\\menu.msgbnd" + str2, (Func<byte[], string, FMG>) ((data, name) => !(name == fmgEvent) ? (FMG) null : SoulsFile<FMG>.Read(data)), (string) null);
      foreach (string allBaseFile in GameDataWriter.GetAllBaseFiles(game))
      {
        string str3 = gameEditor.Spec.GameDir + "\\" + allBaseFile;
        if (File.Exists(str3))
          SFUtil.Backup(str3, false);
      }
      Dictionary<string, MSB1> msbs = new Dictionary<string, MSB1>();
      Dictionary<string, int> players = new Dictionary<string, int>();
      foreach (KeyValuePair<string, MSB1> keyValuePair in dictionary3)
      {
        AnnotationData.MapSpec mapSpec;
        if (ann.Specs.TryGetValue(keyValuePair.Key, out mapSpec))
        {
          MSB1 msB1 = keyValuePair.Value;
          string name = mapSpec.Name;
          msbs[name] = msB1;
          players[name] = 0;
          msB1.Regions.Regions.RemoveAll((Predicate<MSB1.Region>) (r =>
          {
            if (!r.Name.StartsWith("Boss start for ") && !r.Name.StartsWith("FR: ") && (!r.Name.StartsWith("BR: ") && !r.Name.StartsWith("Region for ")))
              return false;
            if (opt["msbinfo"])
              Console.WriteLine(string.Format("Removing region in {0}: {1} #{2}", (object) name, (object) r.Name, (object) r.EntityID));
            return true;
          }));
          msB1.Parts.Players.RemoveAll((Predicate<MSB1.Part.Player>) (p =>
          {
            int result;
            if (!p.Name.StartsWith("c0000_") || !int.TryParse(p.Name.Substring(6), out result) || result < 50)
              return false;
            if (opt["msbinfo"])
              Console.WriteLine(string.Format("Removing player in {0}: {1} #{2}", (object) name, (object) p.Name, (object) p.EntityID));
            return true;
          }));
        }
      }
      HashSet<string> stringSet1 = new HashSet<string>()
      {
        "c1000"
      };
      if (opt["dumpcols"])
      {
        Dictionary<string, string> models = gameEditor.LoadNames<string>("ModelName", (Func<string, string>) (n => n), false);
        Dictionary<int, string> chrs = gameEditor.LoadNames<int>("CharaInitParam", (Func<string, int>) (n => int.Parse(n)), false);
        List<AnnotationData.EnemyCol> enemyColList = new List<AnnotationData.EnemyCol>();
        foreach (KeyValuePair<string, MSB1> keyValuePair1 in msbs)
        {
          string key = keyValuePair1.Key;
          MSB1 msB1 = keyValuePair1.Value;
          Dictionary<string, List<MSB1.Part.Enemy>> dictionary4 = new Dictionary<string, List<MSB1.Part.Enemy>>();
          foreach (MSB1.Part.Enemy enemy in msB1.Parts.Enemies)
          {
            if (!stringSet1.Contains(enemy.ModelName) && enemy.CollisionName != null)
              Util.AddMulti<string, MSB1.Part.Enemy>((IDictionary<string, List<MSB1.Part.Enemy>>) dictionary4, enemy.CollisionName, enemy);
          }
          foreach (KeyValuePair<string, List<MSB1.Part.Enemy>> keyValuePair2 in dictionary4)
          {
            List<string> list = keyValuePair2.Value.Select<MSB1.Part.Enemy, string>((Func<MSB1.Part.Enemy, string>) (e =>
            {
              string str3;
              string str4 = chrs.TryGetValue(e.NPCParamID / 10 * 10, out str3) ? " " + str3 : "";
              string str5 = e.EntityID > 0 ? string.Format(" @{0}", (object) e.EntityID) : "";
              string str6;
              return string.Format("{0} ({1}). NPC {2}{3}{4}", (object) e.Name, models.TryGetValue(e.ModelName, out str6) ? (object) str6 : (object) e.ModelName, (object) e.NPCParamID, (object) str4, (object) str5);
            })).ToList<string>();
            enemyColList.Add(new AnnotationData.EnemyCol()
            {
              Col = key + " " + keyValuePair2.Key,
              Area = key,
              Includes = list
            });
          }
        }
        new SerializerBuilder().Build().Serialize(Console.Out, (object) enemyColList);
      }
      Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
      Dictionary<(string, string), List<AnnotationData.EnemyCol>> dictionary6 = new Dictionary<(string, string), List<AnnotationData.EnemyCol>>();
      foreach (AnnotationData.EnemyCol enemy in ann.Enemies)
      {
        string[] strArray = enemy.Col.Split(' ');
        if (!ann.NameSpecs.ContainsKey(strArray[0]))
          throw new Exception("Unknown map in " + enemy.Col);
        if (!g.Areas.ContainsKey(enemy.Area))
          throw new Exception("Unknown area " + enemy.Area + " in " + enemy.Col);
        if (strArray.Length == 1)
        {
          dictionary5[strArray[0]] = enemy.Area;
        }
        else
        {
          if (strArray.Length != 2)
            throw new Exception("Bad format " + enemy.Col);
          Util.AddMulti<(string, string), AnnotationData.EnemyCol>((IDictionary<(string, string), List<AnnotationData.EnemyCol>>) dictionary6, (strArray[0], strArray[1]), enemy);
        }
      }
      diffData1["SpEffectParam"].Rows = diffData1["SpEffectParam"].Rows.Where<PARAM.Row>((Func<PARAM.Row, bool>) (r => r.ID < 7200L || r.ID >= 7400L)).ToList<PARAM.Row>();
      HashSet<int> enemyRandoBossCopies = new HashSet<int>(((IEnumerable<int>) new int[29]
      {
        223000,
        223100,
        223200,
        224000,
        225000,
        232000,
        236000,
        236001,
        273000,
        323000,
        332000,
        343000,
        347100,
        410000,
        450000,
        451000,
        520000,
        521000,
        522000,
        525000,
        526000,
        527000,
        527100,
        528000,
        529000,
        532000,
        535000,
        537000,
        539001
      }).Select<int, int>((Func<int, int>) (c => c + 50)));
      Dictionary<int, PARAM.Row> baseNpcs = dictionary2["NpcParam"].Rows.ToDictionary<PARAM.Row, int, PARAM.Row>((Func<PARAM.Row, int>) (r => (int) r.ID), (Func<PARAM.Row, PARAM.Row>) (r => r));
      diffData1["GameAreaParam"] = dictionary2["GameAreaParam"];
      foreach (KeyValuePair<string, MSB1> keyValuePair in msbs)
      {
        string key = keyValuePair.Key;
        foreach (MSB1.Part.Enemy enemy1 in keyValuePair.Value.Parts.Enemies)
        {
          MSB1.Part.Enemy e = enemy1;
          int npcParamId = e.NPCParamID;
          if (npcParamId >= 1000 && !baseNpcs.ContainsKey(npcParamId) && !enemyRandoBossCopies.Contains(npcParamId))
          {
            PARAM.Row original = diffData1["NpcParam"][npcParamId];
            if (original != null && (byte) original["pcAttrB"].Value != (byte) 0)
            {
              int int32 = BitConverter.ToInt32("BWLR".Select<char, byte>((Func<char, byte>) (c => (byte) original[string.Format("pcAttr{0}", (object) c)].Value)).ToArray<byte>(), 0);
              if (baseNpcs.ContainsKey(int32))
              {
                e.NPCParamID = int32;
                continue;
              }
            }
            MSB1.Part.Enemy enemy2 = dictionary1[ann.NameSpecs[key].Map].Parts.Enemies.Find((Predicate<MSB1.Part.Enemy>) (c => c.Name == e.Name));
            if (enemy2 != null && e.ModelName == enemy2.ModelName)
              e.NPCParamID = enemy2.NPCParamID;
          }
        }
      }
      diffData1["NpcParam"].Rows.RemoveAll((Predicate<PARAM.Row>) (r => !baseNpcs.ContainsKey((int) r.ID) && !enemyRandoBossCopies.Contains((int) r.ID)));
      if (opt["scale"])
      {
        Dictionary<int, PARAM.Row> dictionary4 = diffData1["NpcParam"].Rows.ToDictionary<PARAM.Row, int, PARAM.Row>((Func<PARAM.Row, int>) (r => (int) r.ID), (Func<PARAM.Row, PARAM.Row>) (r => r));
        HashSet<string> stringSet2 = new HashSet<string>((IEnumerable<string>) "maxHpRate maxStaminaRate physicsAttackPowerRate magicAttackPowerRate fireAttackPowerRate thunderAttackPowerRate physicsDiffenceRate magicDiffenceRate fireDiffenceRate thunderDiffenceRate staminaAttackRate".Split(' '));
        HashSet<string> stringSet3 = new HashSet<string>((IEnumerable<string>) "physicsAttackPowerRate magicAttackPowerRate fireAttackPowerRate thunderAttackPowerRate".Split(' '));
        Dictionary<int, PARAM.Row> dictionary7 = dictionary2["SpEffectParam"].Rows.Where<PARAM.Row>((Func<PARAM.Row, bool>) (r => r.ID >= 7001L && r.ID <= 7015L)).ToDictionary<PARAM.Row, int, PARAM.Row>((Func<PARAM.Row, int>) (r => (int) r.ID), (Func<PARAM.Row, PARAM.Row>) (r => r));
        PARAM.Row row1 = dictionary7[7015];

        var rankRatios = new List<float>();
        int findSpEffect(float val) {
          // ISSUE: reference to a compiler-generated field
          for (int index = 0; index < rankRatios.Count; ++index) {
            // ISSUE: reference to a compiler-generated field
            if ((double)rankRatios[index] >= (double)val)
              return 7200 + index;
          }
          // ISSUE: reference to a compiler-generated field
          return 7200 + rankRatios.Count - 1;
        }

        List<float> floatList = new List<float>()
        {
          1f,
          1.15f,
          1.3f,
          0.9f,
          0.8f
        };
        for (int index1 = 0; index1 < floatList.Count; ++index1)
        {
          for (int index2 = 0; index2 <= 40; ++index2)
          {
            PARAM.Row row2 = new PARAM.Row((long) (7200 + index2 + 40 * index1), (string) null, diffData1["SpEffectParam"].AppliedParamdef);
            float num1 = (float) Math.Pow(4.0, 1.0 * (double) (index2 - 20) / 20.0);
            // ISSUE: reference to a compiler-generated field
            rankRatios.Add(num1);
            foreach (PARAM.Cell cell1 in (IEnumerable<PARAM.Cell>) row2.Cells)
            {
              PARAM.Cell cell2 = row1[cell1.Def.InternalName];
              if (stringSet2.Contains(cell1.Def.InternalName))
              {
                float num2 = (float) cell2.Value / 2.5f;
                float num3 = num1;
                float num4 = (double) num3 < 1.0 ? num3 / num2 : num3 * num2;
                if (stringSet3.Contains(cell1.Def.InternalName))
                  num4 *= floatList[index1];
                cell1.Value = (object) num4;
              }
              else
                cell1.Value = cell2.Value;
            }
            diffData1["SpEffectParam"].Rows.Add(row2);
          }
        }
        Dictionary<int, Dictionary<string, List<MSB1.Part.Enemy>>> dictionary8 = new Dictionary<int, Dictionary<string, List<MSB1.Part.Enemy>>>();
        foreach (KeyValuePair<string, MSB1> keyValuePair in msbs)
        {
          string key1 = keyValuePair.Key;
          MSB1 msB1 = keyValuePair.Value;
          Dictionary<string, List<MSB1.Part.Enemy>> dictionary9 = new Dictionary<string, List<MSB1.Part.Enemy>>();
          foreach (MSB1.Part.Enemy enemy in msB1.Parts.Enemies)
          {
            MSB1.Part.Enemy e = enemy;
            if (!stringSet1.Contains(e.ModelName) && e.CollisionName != null && e.NPCParamID > 0)
            {
              List<AnnotationData.EnemyCol> enemyColList;
              string key2;
              if (dictionary6.TryGetValue((key1, e.CollisionName), out enemyColList))
              {
                key2 = enemyColList[0].Area;
                if (enemyColList.Count > 1)
                {
                  foreach (AnnotationData.EnemyCol enemyCol in enemyColList)
                  {
                    if (enemyCol.Includes.Any<string>((Func<string, bool>) (name => name.Split(' ')[0] == e.Name)))
                    {
                      key2 = enemyCol.Area;
                      break;
                    }
                  }
                }
              }
              else
              {
                string str3;
                key2 = !dictionary5.TryGetValue(key1, out str3) ? (string) null : str3;
              }
              if (!dictionary8.ContainsKey(e.NPCParamID))
                dictionary8[e.NPCParamID] = new Dictionary<string, List<MSB1.Part.Enemy>>();
              Util.AddMulti<string, MSB1.Part.Enemy>((IDictionary<string, List<MSB1.Part.Enemy>>) dictionary8[e.NPCParamID], key2, e);
            }
          }
        }
        foreach (KeyValuePair<int, Dictionary<string, List<MSB1.Part.Enemy>>> keyValuePair1 in dictionary8)
        {
          int key1 = keyValuePair1.Key;
          foreach (KeyValuePair<string, List<MSB1.Part.Enemy>> keyValuePair2 in keyValuePair1.Value)
          {
            string key2 = keyValuePair2.Key;
            (float, float) valueTuple;
            (float baseRatio6, float num8) = key2 == null || !g.AreaRatios.TryGetValue(key2, out valueTuple) ? (1f, 1f) : valueTuple;
            if ((double) Math.Abs(baseRatio6 - 1f) >= 0.00999999977648258)
            {
              float num2 = 1f;
              int key3 = -1;
              PARAM.Row row2;
              if (baseNpcs.TryGetValue(key1, out row2))
              {
                if (key1 >= 120000)
                {
                  key3 = (int) row2["spEffectID4"].Value;
                  PARAM.Row row3;
                  if (dictionary7.TryGetValue(key3, out row3))
                    num2 = (float) row3["physicsAttackPowerRate"].Value;
                }
                float val = baseRatio6 * num2;
                int sp = findSpEffect(val);
                if (key1 == 528000 && sp < 7220)
                  sp = 7220;
                else if ((double) num8 / (double) baseRatio6 > 1.15)
                {
                  sp += 40;
                  if ((double) num8 / (double) baseRatio6 > 1.3)
                    sp += 40;
                }
                else if ((double) num8 / (double) baseRatio6 < 0.9)
                {
                  sp += 120;
                  if ((double) num8 / 40.0 < 0.8)
                    sp += 40;
                }
                PARAM.Row row4;
                if (keyValuePair1.Value.Count == 1)
                {
                  row4 = dictionary4[key1];
                  if (key1 == 528000 && (double) val < 1.0)
                    row4["hp"].Value = (object) (uint) ((double) (uint) row2["hp"].Value * (double) baseRatio6);
                }
                else
                {
                  int key4 = key1;
                  while (dictionary4.ContainsKey(key4))
                    ++key4;
                  row4 = new PARAM.Row((long) key4, (string) null, diffData1["NpcParam"].AppliedParamdef);
                  foreach (PARAM.Cell cell in (IEnumerable<PARAM.Cell>) row4.Cells)
                    cell.Value = row2[cell.Def.InternalName].Value;
                  dictionary4[key4] = row4;
                  byte[] bytes = BitConverter.GetBytes(key1);
                  for (int index = 0; index < bytes.Length; ++index)
                    row4[string.Format("pcAttr{0}", (object) "BWLR"[index])].Value = (object) bytes[index];
                  diffData1["NpcParam"].Rows.Add(row4);
                  foreach (MSB1.Part.Enemy enemy in keyValuePair2.Value)
                    enemy.NPCParamID = key4;
                }
                setScalingSp(row4, sp);
                uint baseSouls = (uint) baseNpcs[key1]["getSoul"].Value;
                uint num3 = 0;
                if (baseSouls > 0U)
                {
                  num3 = getNewSouls(baseSouls, baseRatio6);
                  row4["getSoul"].Value = (object) num3;
                }
                else
                {
                  foreach (MSB1.Part.Enemy enemy in keyValuePair2.Value)
                  {
                    if (enemy.EntityID > 0)
                    {
                      PARAM.Row row3 = dictionary2["GameAreaParam"][enemy.EntityID];
                      if (row3 != null)
                      {
                        baseSouls = (uint) row3["bonusSoul_single"].Value;
                        num3 = getNewSouls(baseSouls, baseRatio6);
                        PARAM.Row row5 = diffData1["GameAreaParam"][enemy.EntityID];
                        row5["bonusSoul_single"].Value = (object) num3;
                        row5["bonusSoul_multi"].Value = (object) num3;
                      }
                    }
                  }
                }
                if (opt["debugscale"])
                  Console.WriteLine(string.Format("Change for {0} {1}: {2} * {3} = {4}, dmg ratio {5}, sp {6} -> {7}. ", (object) key1, (object) key2, (object) baseRatio6, (object) num2, (object) (float) ((double) baseRatio6 * (double) num2), (object) (float) ((double) num8 / (double) baseRatio6), (object) key3, (object) sp) + string.Format("Souls {0} -> {1}. {2} {3}", (object) baseSouls, (object) num3, keyValuePair1.Value.Count == 1 ? (object) " UNIQUE" : (object) "", num3 > 50000U ? (object) "BIG" : (object) ""));
              }
            }
          }
        }
      }
      else
      {
        foreach (PARAM.Row row1 in diffData1["NpcParam"].Rows)
        {
          PARAM.Row row2;
          if (baseNpcs.TryGetValue((int) row1.ID, out row2))
          {
            row1["getSoul"].Value = row2["getSoul"].Value;
            if (row1.ID >= 120000L)
            {
              row1["spEffectID4"].Value = row2["spEffectID4"].Value;
            }
            else
            {
              for (int index = 0; index < 8; ++index)
              {
                int num1 = (int) row1[string.Format("spEffectID{0}", (object) index)].Value;
                if (num1 >= 7200 && num1 < 7400)
                  row1[string.Format("spEffectID{0}", (object) index)].Value = row2[string.Format("spEffectID{0}", (object) index)].Value;
              }
            }
          }
        }
      }
      int mk = 1815800;
      int slot = 0;
      List<List<object>> events = new List<List<object>>();
      Dictionary<string, int> dictionary10 = ann.Entrances.SelectMany<AnnotationData.Entrance, AnnotationData.Side>((Func<AnnotationData.Entrance, IEnumerable<AnnotationData.Side>>) (e => (IEnumerable<AnnotationData.Side>) e.Sides())).Where<AnnotationData.Side>((Func<AnnotationData.Side, bool>) (s => (uint) s.BossTrigger > 0U)).ToDictionary<AnnotationData.Side, string, int>((Func<AnnotationData.Side, string>) (s => s.Area), (Func<AnnotationData.Side, int>) (s => s.BossTrigger));
      Dictionary<int, List<int>> dictionary11 = new Dictionary<int, List<int>>();
      foreach (AnnotationData.Entrance entrance in ann.Entrances)
      {
        AnnotationData.Entrance e = entrance;
        if (!e.HasTag("unused"))
        {
          if (e.HasTag("door"))
          {
            AddWarpEvent((byte) 1, e.Area, new List<int>()
            {
              e.ID,
              e.ID + 1,
              e.HasTag("lordvessel") ? 11800100 : 0
            }, (string) null);
          }
          else
          {
            for (int index1 = 0; index1 <= 1; ++index1)
            {
              bool flag2 = index1 == 0;
              AnnotationData.Side side = flag2 ? e.ASide : e.BSide;
              if (side != null)
              {
                string index2 = side.DestinationMap ?? e.Area;
                MSB1 msB1_1 = msbs[index2];
                MSB1.Part.Object @object = msbs[e.Area].Parts.Objects.Find((Predicate<MSB1.Part.Object>) (o => o.Name == e.Name));
                if (side.BossTrigger != 0 && side.BossTriggerArea != null)
                {
                  if (dictionary10[side.Area] != side.BossTrigger)
                    throw new Exception("Non-unique boss trigger for " + side.Area);
                  MSB1.Region region = new MSB1.Region();
                  region.Name = "Boss start for " + side.Area;
                  region.EntityID = mk++;
                  List<float> pos = getPos(side.BossTriggerArea);
                  region.Position = new Vector3(pos[0], pos[1], pos[2]);
                  region.Rotation = new Vector3(0.0f, pos[3], 0.0f);
                  region.Shape = (MSB1.Shape) new MSB1.Shape.Box()
                  {
                    Width = pos[4],
                    Height = pos[5],
                    Depth = pos[6]
                  };
                  msB1_1.Regions.Regions.Insert(0, region);
                  Util.AddMulti<int, int>((IDictionary<int, List<int>>) dictionary11, side.BossTrigger, region.EntityID);
                }
                if (!g.Ignore.Contains((e.Name, side.Area)))
                {
                  Vector3 position = @object.Position;
                  float dist = 1f;
                  MSB1.Region region = new MSB1.Region();
                  MSB1.Shape.Box box = new MSB1.Shape.Box();
                  float num1 = @object.Rotation.Y + 180f;
                  float y = (double) num1 >= 180.0 ? num1 - 360f : num1;
                  Vector3 vector3_1 = new Vector3(@object.Rotation.X, y, @object.Rotation.Z);
                  int num2;
                  if (side.ActionRegion == 0)
                  {
                    region.EntityID = mk++;
                    num2 = region.EntityID;
                    region.Name = (flag2 ? "FR" : "BR") + ": " + e.Text;
                    region.Position = position;
                    region.Rotation = @object.Rotation;
                    if (!flag2)
                      region.Rotation = vector3_1;
                    region.Position = MoveInDirection(region.Position, region.Rotation, 1f);
                    region.Position = new Vector3(region.Position.X, region.Position.Y - (e.HasTag("world") ? 0.0f : 1f), region.Position.Z);
                    box.Width = side.CustomActionWidth == 0 ? 1.5f : (float) side.CustomActionWidth;
                    box.Height = 3f;
                    box.Depth = 1.5f;
                    region.Shape = (MSB1.Shape) box;
                    msB1_1.Regions.Regions.Insert(0, region);
                  }
                  else
                    num2 = side.ActionRegion;
                  int num3 = mk++;
                  if (side.CustomWarp == null)
                  {
                    Vector3 vector3_2;
                    Vector3 r;
                    if (flag2)
                    {
                      vector3_2 = vector3_1;
                      r = @object.Rotation;
                    }
                    else
                    {
                      vector3_2 = @object.Rotation;
                      r = vector3_1;
                    }
                    Vector3 vector3_3 = MoveInDirection(position, r, dist);
                    if (side.HasTag("higher"))
                      vector3_3 = new Vector3(vector3_3.X, vector3_3.Y + 1f, vector3_3.Z);
                    MSB1.Part.Player player = new MSB1.Part.Player();
                    player.Name = string.Format("c0000_{0:d4}", (object) (50 + players[index2]++));
                    player.ModelName = "c0000";
                    player.EntityID = num3;
                    player.Position = vector3_3;
                    player.Rotation = vector3_2;
                    player.Scale = new Vector3(1f, 1f, 1f);
                    msB1_1.Parts.Players.Add(player);
                    side.Warp = new Graph.WarpPoint()
                    {
                      ID = e.ID,
                      Map = index2,
                      Action = num2,
                      Player = num3
                    };
                  }
                  else
                  {
                    string[] strArray = side.CustomWarp.Split(' ');
                    string index3 = strArray[0];
                    MSB1 msB1_2 = msbs[index3];
                    List<float> list = ((IEnumerable<string>) strArray).Skip<string>(1).Select<string, float>((Func<string, float>) (c => float.Parse(c, (IFormatProvider) CultureInfo.InvariantCulture))).ToList<float>();
                    MSB1.Part.Player player = new MSB1.Part.Player();
                    player.Name = string.Format("c0000_{0:d4}", (object) (50 + players[index3]++));
                    player.ModelName = "c0000";
                    player.EntityID = num3;
                    player.Position = new Vector3(list[0], list[1], list[2]);
                    player.Rotation = new Vector3(0.0f, list[3], 0.0f);
                    player.Scale = new Vector3(1f, 1f, 1f);
                    msB1_2.Parts.Players.Add(player);
                    side.Warp = new Graph.WarpPoint()
                    {
                      ID = e.ID,
                      Map = index3,
                      Action = num2,
                      Player = num3
                    };
                  }
                  int key;
                  if (side.BossTrigger == 0 && dictionary10.TryGetValue(side.Area, out key))
                    Util.AddMulti<int, int>((IDictionary<int, List<int>>) dictionary11, key, num2);
                }
              }
            }
          }
        }
      }
      foreach (AnnotationData.Entrance warp in ann.Warps)
      {
        if (!warp.HasTag("unused") && !warp.HasTag("norandom"))
        {
          warp.ASide.Warp = new Graph.WarpPoint()
          {
            ID = warp.ID,
            Map = warp.ASide.DestinationMap ?? warp.Area,
            Cutscene = warp.ASide.Cutscene
          };
          warp.BSide.Warp = new Graph.WarpPoint()
          {
            ID = warp.ID,
            Map = warp.BSide.DestinationMap ?? warp.Area,
            Cutscene = warp.BSide.Cutscene
          };
          if (warp.BSide.HasTag("player"))
            warp.BSide.Warp.Player = warp.ID;
          else
            warp.BSide.Warp.Region = warp.ID;
        }
      }
      int num9 = mk++;
      string[] strArray1 = g.Start.Respawn.Split(' ');
      string map1 = strArray1[0];
      int startRespawn = int.Parse(strArray1[1]);
      MSB1 msB1_3 = msbs[map1];
      MSB1.Part.Player player1 = new MSB1.Part.Player();
      player1.Name = string.Format("c0000_{0:d4}", (object) (50 + players[map1]++));
      player1.ModelName = "c0000";
      player1.EntityID = num9;
      MSB1.Event.SpawnPoint spawn = msB1_3.Events.SpawnPoints.Find((Predicate<MSB1.Event.SpawnPoint>) (e => e.EntityID == startRespawn));
      if (spawn == null)
        throw new Exception(string.Format("Bad custom start {0}, can't find spawn point {1}", (object) g.Start.Respawn, (object) startRespawn));
      MSB1.Region region1 = msB1_3.Regions.Regions.Find((Predicate<MSB1.Region>) (e => e.Name == spawn.SpawnPointName));
      if (region1 == null)
        throw new Exception("Bad custom start " + g.Start.Respawn + ", can't find region " + spawn.SpawnPointName);
      player1.Position = region1.Position;
      player1.Rotation = region1.Rotation;
      player1.Scale = new Vector3(1f, 1f, 1f);
      msB1_3.Parts.Players.Add(player1);
      List<int> intList1 = new List<int>();
      Dictionary<string, string> dictionary12 = new Dictionary<string, string>();
      foreach (KeyValuePair<string, MSB1> keyValuePair in msbs)
      {
        string key = keyValuePair.Key;
        MSB1 msB1_1 = keyValuePair.Value;
        foreach (MSB1.Part.Collision collision in msB1_1.Parts.Collisions)
        {
          if (collision.PlayRegionID < 10)
          {
            if (key == "firelink" && collision.Name == "h0017B2_0000")
              collision.PlayRegionID = -69696978;
            if (key == "firelink" && (collision.Name == "h0017B2_0000" || collision.Name == "h0015B2_0000"))
            {
              collision.PlayRegionID = -2;
            }
            else
            {
              int num1;
              if (ann.DefaultFlagCols.TryGetValue(key + "_" + collision.Name, out num1))
              {
                int num2 = intList1.IndexOf(num1);
                if (num2 == -1)
                {
                  num2 = intList1.Count;
                  intList1.Add(num1);
                }
                collision.PlayRegionID = -(this.tempColEventBase + num2) - 10;
              }
              else
              {
                int playRegionId = collision.PlayRegionID;
              }
            }
          }
        }
        if (key == "demonruins")
          msB1_1.Parts.Collisions = msB1_1.Parts.Collisions.Where<MSB1.Part.Collision>((Func<MSB1.Part.Collision, bool>) (c => c.Name != "h9950B1")).ToList<MSB1.Part.Collision>();
        else if (key == "dlc")
          msB1_1.Parts.Collisions = msB1_1.Parts.Collisions.Where<MSB1.Part.Collision>((Func<MSB1.Part.Collision, bool>) (c => c.Name != "h7800B1")).ToList<MSB1.Part.Collision>();
        else if (key == "totg")
        {
          MSB1.Region region2 = msB1_1.Regions.Regions.Find((Predicate<MSB1.Region>) (c => c.EntityID == 1312998));
          region2.Position = new Vector3(-118.186f, -250.591f, -31.893f);
          if (!(region2.Shape is MSB1.Shape.Box shape))
            throw new Exception("Unexpected region");
          shape.Width = 10f;
          shape.Height = 5f;
        }
        else if (key == "kiln")
        {
          if (opt["patchkiln"])
          {
            MSB1.Part.Player player2 = msB1_1.Parts.Players.Find((Predicate<MSB1.Part.Player>) (p => p.Name == "c0000_0000"));
            player2.Position = new Vector3(50.3f, -63.27f, 106.1f);
            player2.Rotation = new Vector3(0.0f, -105f, 0.0f);
          }
        }
        else if (key == "anorlondo")
        {
          MSB1.Part.Object @object = msB1_1.Parts.Objects.Find((Predicate<MSB1.Part.Object>) (e => e.Name == "o0500_0006"));
          if (g.EntranceIds["o5869_0000"].IsFixed)
          {
            @object.Position = new Vector3(448.49f, 144.11f, 269.42f);
            @object.Rotation = new Vector3(11f, 83f, -4f);
            @object.CollisionName = "h0111B1_0000";
            @object.UnkT0C = (short) 33;
          }
          else
          {
            @object.Position = new Vector3(444.106f, 160.258f, 255.887f);
            @object.Rotation = new Vector3(-3f, -90f, 0.0f);
            @object.CollisionName = "h0025B1_0000";
            @object.UnkT0C = (short) 50;
          }
        }
        else if (key == "depths")
          msB1_1.Events.ObjActs.Find((Predicate<MSB1.Event.ObjAct>) (o => o.ObjActEntityID == 11000120)).ObjActParamID = !(g.Start.Area == "depths") ? (short) 11315 : (short) -1;
        else if (key == "asylum")
        {
          msB1_1.Regions.Regions.Find((Predicate<MSB1.Region>) (e => e.EntityID == 1812998)).Position = new Vector3(3.2f, 209f, -33.089f);
          HashSet<int> intSet = new HashSet<int>()
          {
            1811613,
            1811616,
            1811619,
            1811622
          };
          MSB1.Part.Object source = (MSB1.Part.Object) null;
          foreach (MSB1.Part.Object @object in msB1_1.Parts.Objects)
          {
            if (intSet.Contains(@object.EntityID))
            {
              @object.Position = new Vector3(8.934f, 202f, 18.512f);
              @object.CollisionName = "h0010B1";
              source = @object;
            }
          }
          if (source == null)
            throw new Exception("Can't find asylum treasure to base estus treasure on");
          if (!msB1_1.Parts.Objects.Any<MSB1.Part.Object>((Func<MSB1.Part.Object, bool>) (p => p.Name == "o0500_0050")))
          {
            MSB1.Part.Object target = new MSB1.Part.Object();
            Util.CopyAll<MSB1.Part.Object>(source, target);
            target.Name = "o0500_0050";
            target.ModelName = "o0500";
            target.UnkT0C = (short) 50;
            target.Position = new Vector3(13.279f, 202.015f, 20.8f);
            target.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
            target.EntityID = -1;
            msB1_1.Parts.Objects.Add(target);
            MSB1.Event.Treasure treasure = new MSB1.Event.Treasure();
            treasure.EventID = 69;
            treasure.EntityID = -1;
            treasure.ItemLots[0] = 1082;
            treasure.TreasurePartName = "o0500_0050";
            treasure.Name = "New Estus";
            msB1_1.Events.Treasures.Add(treasure);
          }
        }
        else if (key == "dukes")
        {
          if (!msB1_1.Parts.Objects.Any<MSB1.Part.Object>((Func<MSB1.Part.Object, bool>) (p => p.Name == "o7500_0001")))
          {
            MSB1.Part.Object source = msB1_1.Parts.Objects.Find((Predicate<MSB1.Part.Object>) (e => e.EntityID == 1701800));
            MSB1.Part.Object @object = new MSB1.Part.Object();
            MSB1.Part.Object target = @object;
            Util.CopyAll<MSB1.Part.Object>(source, target);
            @object.Name = "o7500_0001";
            @object.Position = new Vector3(284.108f, 388.313f, 520.228f);
            @object.EntityID = 1701801;
            @object.DrawGroups[0] = 2147483648U;
            @object.DrawGroups[1] = 15U;
            @object.DrawGroups[2] = 0U;
            @object.DrawGroups[3] = 0U;
            msB1_1.Parts.Objects.Insert(0, @object);
          }
          msB1_1.Regions.Regions.Find((Predicate<MSB1.Region>) (r => r.Name == "復活ポイント（一時用）")).EntityID = 1702901;
        }
      }
      int num10 = 6920;
      HashSet<string> stringSet4 = new HashSet<string>();
      foreach (Graph.Node node in g.Nodes.Values)
      {
        foreach (Graph.Edge edge in node.To)
        {
          Graph.Edge link = edge.Link;
          if (link == null)
            throw new Exception(string.Format("Internal error: Unlinked {0}", (object) edge));
          Graph.WarpPoint warp1 = edge.Side.Warp;
          Graph.WarpPoint warp2 = link.Side.Warp;
          if (warp1 == null || warp2 == null)
          {
            if (!edge.IsFixed || !link.IsFixed)
              throw new Exception(string.Format("Missing warps - {0} {1} for {2} -> {3}", (object) (warp1 == null), (object) (warp2 == null), (object) edge, (object) link));
          }
          else
          {
            if (edge.Name == link.Name && edge.IsFixed && !opt["alwaysshow"])
            {
              AnnotationData.Entrance entranceId = g.EntranceIds[edge.Name];
              if (!stringSet4.Contains(entranceId.FullName))
              {
                if (entranceId.HasTag("pvp"))
                {
                  AddWarpEvent((byte) 1, entranceId.Area, new List<int>()
                  {
                    entranceId.ID,
                    entranceId.ID + 1
                  }, (string) null);
                  stringSet4.Add(entranceId.FullName);
                  continue;
                }
                if (entranceId.HasTag("world") && edge.Pair != null)
                {
                  Graph.WarpPoint warp3 = edge.Pair.Side.Warp;
                  Graph.WarpPoint warp4 = link.Pair.Side.Warp;
                  if (warp3 == null || warp4 == null)
                    throw new Exception(string.Format("Missing warp info - {0} {1} for {2} -> {3}", (object) (warp1 == null), (object) (warp2 == null), (object) edge.Pair, (object) link.Pair));
                  AddWarpEvent((byte) 3, entranceId.Area, new List<int>()
                  {
                    entranceId.ID,
                    entranceId.ID + 1,
                    num10,
                    warp1.Action,
                    warp4.Action
                  }, (string) null);
                  ++num10;
                  stringSet4.Add(entranceId.FullName);
                  continue;
                }
              }
              else
                continue;
            }
            List<int> p = new List<int>()
            {
              0,
              getPlayer(warp2),
              edge.Side.TrapFlag,
              0,
              edge.Side.Flag,
              link.Side.EntryFlag,
              link.Side.BeforeWarpFlag
            };
            string map2 = edge.Side.Warp.Map;
            string col = edge.Side.Col;
            if (opt["pacifist"])
            {
              p[2] = 0;
              p[4] = 0;
            }
            if (warp1.Action == 0)
            {
              p[0] = edge.Side.WarpFlag;
              AddWarpEvent((byte) 2, warp2.Map, p, (string) null);
            }
            else
            {
              p[0] = warp1.ID;
              p[3] = warp1.Action;
              AddWarpEvent((byte) 0, warp2.Map, p, (string) null);
            }
          }
        }
      }
      if (events.Count > 400)
        throw new Exception("Internal error: too many warps");
      List<int> intList2 = new List<int>()
      {
        11205382,
        11015382,
        11015396,
        11705396,
        11415392,
        11415397,
        11410250,
        11305100
      };
      foreach (string file in Directory.GetFiles(str1 + "\\event", "*.emevd*"))
      {
        string str3 = GameEditor.BaseName(file);
        EMEVD emevd1 = SoulsFile<EMEVD>.Read(file);
        string path = gameEditor.Spec.GameDir + "\\event\\" + Path.GetFileName(file);
        EMEVD emevd2 = SoulsFile<EMEVD>.Read(path);
        List<EMEVD.Instruction> instructionList = new List<EMEVD.Instruction>();
        Dictionary<int, EMEVD.Event> dictionary4 = new Dictionary<int, EMEVD.Event>();
        bool flag2 = false;
        foreach (EMEVD.Event @event in emevd2.Events)
        {
          if (this.enemyRandoEvents.Contains((int) @event.ID))
            emevd1.Events.Add(@event);
          else if (intList2.Contains((int) @event.ID))
            dictionary4[(int) @event.ID] = @event;
          else if (@event.ID == 11810310L)
            flag2 = !@event.Instructions.Any<EMEVD.Instruction>((Func<EMEVD.Instruction, bool>) (instr => instr.Bank == 2003 && instr.ID == 18));
          else if (@event.ID == 0L)
          {
            for (int index = @event.Instructions.Count - 1; index >= 0; --index)
            {
              EMEVD.Instruction instruction = @event.Instructions[index];
              if (instruction.Bank == 2004 && instruction.ID == 2)
                instructionList.Add(instruction);
              else if (instruction.Bank == 2000 && instruction.ID == 0 && this.enemyRandoEvents.Contains((int) instruction.UnpackArgs(Enumerable.Repeat<EMEVD.Instruction.ArgType>(EMEVD.Instruction.ArgType.Int32, instruction.ArgData.Length / 4), false)[1]))
                instructionList.Add(instruction);
              else
                break;
            }
          }
        }
        foreach (EMEVD.Event event1 in emevd1.Events)
        {
          if (event1.ID == 0L)
          {
            instructionList.Reverse();
            event1.Instructions.AddRange((IEnumerable<EMEVD.Instruction>) instructionList);
          }
          EMEVD.Event event2;
          if (dictionary4.TryGetValue((int) event1.ID, out event2))
          {
            event1.Instructions = event2.Instructions;
            event1.Parameters = event2.Parameters;
          }
          if (event1.ID == 11810310L & flag2)
            event1.Instructions.RemoveAll((Predicate<EMEVD.Instruction>) (instr =>
            {
              if (instr.Bank == 2003 && instr.ID == 18)
                return true;
              return instr.Bank == 2004 && instr.ID == 41;
            }));
          if (event1.ID == 0L && str3 == "common")
          {
            foreach (List<object> objectList in events)
              event1.Instructions.Add(new EMEVD.Instruction(2000, 0, (IEnumerable<object>) objectList));
            (byte num17, byte num18) = GetDest(map1);
            event1.Instructions.Add(new EMEVD.Instruction(2000, 0, (IEnumerable<object>) new List<object>()
            {
              (object) 0,
              (object) 8900U,
              (object) num17,
              (object) num18,
              (object) num9
            }));
            for (int index = 0; index < intList1.Count; ++index)
              event1.Instructions.Add(new EMEVD.Instruction(2000, 0, (IEnumerable<object>) new List<object>()
              {
                (object) index,
                (object) 8901U,
                (object) (this.tempColEventBase + index),
                (object) intList1[index]
              }));
            if (opt["start"])
              event1.Instructions.Add(new EMEVD.Instruction(2000, 0, (IEnumerable<object>) new List<object>()
              {
                (object) 0,
                (object) 8950U,
                (object) num17,
                (object) num18,
                (object) num9,
                (object) startRespawn
              }));
          }
          if (event1.ID == 0L && str3 == "m14_01_00_00" && opt["bboc"])
          {
            foreach (EMEVD.Instruction instruction in event1.Instructions)
            {
              if (instruction.Bank == 2000 && instruction.ID == 0)
              {
                int int32 = BitConverter.ToInt32(instruction.ArgData, 4);
                switch (int32)
                {
                  case 11410200:
                  case 11410201:
                    Array.Copy((Array) BitConverter.GetBytes(int32 + 20), 0, (Array) instruction.ArgData, 4, 4);
                    continue;
                  default:
                    continue;
                }
              }
            }
          }
          int index1 = 0;
          List<object> objectList1 = (List<object>) null;
          foreach (EMEVD.Instruction instruction in event1.Instructions)
          {
            if (instruction.Bank == 3 && instruction.ID == 2)
            {
              List<object> objectList2 = instruction.UnpackArgs((IEnumerable<EMEVD.Instruction.ArgType>) new List<EMEVD.Instruction.ArgType>()
              {
                EMEVD.Instruction.ArgType.SByte,
                EMEVD.Instruction.ArgType.Byte,
                EMEVD.Instruction.ArgType.Int32,
                EMEVD.Instruction.ArgType.Int32
              }, false);
              if (dictionary11.ContainsKey((int) objectList2[3]))
              {
                objectList1 = objectList2;
                break;
              }
            }
            ++index1;
          }
          if (objectList1 != null)
          {
            sbyte num1 = (sbyte) objectList1[0];
            int index2 = (int) objectList1[3];
            event1.Instructions.RemoveAt(index1);
            event1.Instructions.Insert(index1, new EMEVD.Instruction(0, 0, (IEnumerable<object>) new List<object>()
            {
              (object) num1,
              (object) (byte) 1,
              (object) (sbyte) -7,
              (object) (byte) 0
            }));
            foreach (int num2 in ((IEnumerable<int>) new int[1]
            {
              index2
            }).Concat<int>((IEnumerable<int>) dictionary11[index2]))
              event1.Instructions.Insert(index1, new EMEVD.Instruction(3, 2, (IEnumerable<object>) new List<object>()
              {
                (object) (sbyte) -7,
                (object) (byte) 1,
                (object) 10000,
                (object) num2
              }));
          }
        }
        Console.WriteLine("Writing " + path);
        emevd1.Write(path);
      }
      foreach (KeyValuePair<string, MSB1> keyValuePair in msbs)
      {
        string map2 = ann.NameSpecs[keyValuePair.Key].Map;
        string path = gameEditor.Spec.GameDir + "\\map\\MapStudio\\" + map2 + ".msb";
        Console.WriteLine("Writing " + path);
        keyValuePair.Value.Write(path);
      }
      Console.WriteLine("Writing " + gameEditor.Spec.GameDir + "\\" + gameEditor.Spec.ParamFile);
      await gameEditor.OverrideBnd<PARAM>(gameEditor.Spec.GameDir + "\\" + gameEditor.Spec.ParamFile, "param\\GameParam", diffData1, (Func<PARAM, byte[]>) (f => f.Write()), (string) null);
      Console.WriteLine("Copying ESDs to " + gameEditor.Spec.GameDir + "\\" + gameEditor.Spec.EsdDir);
      foreach (string file in Directory.GetFiles(str1 + "\\" + gameEditor.Spec.EsdDir, "*.talkesdbnd*"))
        File.Copy(file, gameEditor.Spec.GameDir + "\\" + gameEditor.Spec.EsdDir + "\\" + Path.GetFileName(file), true);
      Console.WriteLine("Writing messages to " + gameEditor.Spec.GameDir + "\\msg\\" + opt.Language);
      diffData2[fmgEvent][15000280] = "Return to " + g.Start.Name;
      diffData2[fmgEvent][15000281] = "Sealed in New Londo Ruins";
      diffData2[fmgEvent][15000282] = "Fog Gate Randomizer breaks when online.\nChange Launch setting in Network settings and then reload.";
      diffData2[fmgEvent][15000283] = g.EntranceIds["1702901"].IsFixed ? "Go to jail" : "Go to jail (randomized warp)";
      await gameEditor.OverrideBnd<FMG>(gameEditor.Spec.GameDir + "\\msg\\" + opt.Language + "\\menu.msgbnd" + str2, "msg\\" + opt.Language, diffData2, (Func<FMG, byte[]>) (fmg => fmg.Write()), (string) null);

      void setScalingSp(PARAM.Row row, int sp)
      {
        if (row.ID >= 120000L)
        {
          row["spEffectID4"].Value = (object) sp;
        }
        else
        {
          for (int index = 0; index < 8; ++index)
          {
            if ((int) row[string.Format("spEffectID{0}", (object) index)].Value == 0)
            {
              row[string.Format("spEffectID{0}", (object) index)].Value = (object) sp;
              break;
            }
          }
        }
      }

      uint getNewSouls(uint baseSouls, float baseRatio)
      {
        int num1 = Math.Min(60000, (int) Math.Pow(Math.Pow((double) baseSouls, 0.5) * (double) baseRatio, 2.0));
        int num2 = 0;
        if (baseSouls % 1000U == 0U && num1 >= 5000)
          num2 = 1000;
        else if (baseSouls % 500U == 0U && num1 >= 2000)
          num2 = 500;
        else if (baseSouls % 100U == 0U && num1 >= 300)
          num2 = 100;
        if (num2 > 0)
          num1 -= num1 % num2;
        return (uint) num1;
      }

      List<float> getPos(string at)
      {
        return ((IEnumerable<string>) at.Split(' ')).Select<string, float>((Func<string, float>) (c => float.Parse(c, (IFormatProvider) CultureInfo.InvariantCulture))).ToList<float>();
      }

      Vector3 MoveInDirection(Vector3 v, Vector3 r, float dist)
      {
        float num = (float) ((double) r.Y * 3.14159274101257 / 180.0);
        return new Vector3(v.X + (float) Math.Sin((double) num) * dist, v.Y, v.Z + (float) Math.Cos((double) num) * dist);
      }

      (byte, byte) GetDest(string map)
      {
        AnnotationData.MapSpec nameSpec = ann.NameSpecs[map];
        return (byte.Parse(nameSpec.Map.Substring(1, 2)), byte.Parse(nameSpec.Map.Substring(4, 2)));
      }

      void AddWarpEvent(byte mode, string toArea, List<int> p, string col)
      {
        (byte num19, byte num20) = GetDest(toArea);
        while (p.Count < 7)
          p.Add(0);
        events.Add(new List<object>()
        {
          (object) slot++,
          (object) 5700U,
          (object) mode,
          (object) num19,
          (object) num20,
          (object) (byte) (p[6] > 0 ? 1 : 0),
          (object) p[0],
          (object) p[1],
          (object) p[2],
          (object) p[3],
          (object) p[4],
          (object) p[5],
          (object) Math.Abs(p[6])
        });
      }

      int getPlayer(Graph.WarpPoint warp)
      {
        if (warp.Player != 0)
          return warp.Player;
        MSB1 msB1 = msbs[warp.Map];
        MSB1.Region region = msB1.Regions.Regions.Where<MSB1.Region>((Func<MSB1.Region, bool>) (r => r.EntityID == warp.Region)).FirstOrDefault<MSB1.Region>();
        if (region == null)
          throw new Exception(string.Format("Cutscene warp destination {0} not found in {1}", (object) warp.Region, (object) warp.Map));
        MSB1.Part.Player player = new MSB1.Part.Player();
        player.Name = string.Format("c0000_{0:d4}", (object) (50 + players[warp.Map]++));
        player.ModelName = "c0000";
        player.EntityID = mk++;
        player.Position = region.Position;
        player.Rotation = region.Rotation;
        player.Scale = new Vector3(1f, 1f, 1f);
        msB1.Parts.Players.Add(player);
        warp.Player = player.EntityID;
        return warp.Player;
      }
    }

    public static List<string> GetAllBaseFiles(GameSpec.FromGame game)
    {
      List<string> stringList = new List<string>();
      string path = string.Format("dist\\{0}", (object) game);
      if (!Directory.Exists(path))
        return stringList;
      foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
      {
        if (file.EndsWith(".dcx") || file.EndsWith(".msb") || (file.EndsWith(".emevd") || file.EndsWith("bnd")))
          stringList.Add(file.Replace(path + "\\", ""));
      }
      return stringList;
    }
  }
}
