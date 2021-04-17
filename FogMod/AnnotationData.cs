// Decompiled with JetBrains decompiler
// Type: FogMod.AnnotationData
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsIds;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace FogMod
{
  public class AnnotationData
  {
    public static readonly List<AnnotationData.MapSpec> DS1Specs = new List<AnnotationData.MapSpec>()
    {
      AnnotationData.MapSpec.Of("m10_00_00_00", "depths", 1400, 1420),
      AnnotationData.MapSpec.Of("m10_01_00_00", "parish", 1403, 1421),
      AnnotationData.MapSpec.Of("m10_02_00_00", "firelink", 0, 0),
      AnnotationData.MapSpec.Of("m11_00_00_00", "paintedworld", 1600, 1604),
      AnnotationData.MapSpec.Of("m12_00_00_01", "darkroot", 2900, 2908),
      AnnotationData.MapSpec.Of("m12_01_00_00", "dlc", 2909, 2920),
      AnnotationData.MapSpec.Of("m13_00_00_00", "catacombs", 3951, 3954),
      AnnotationData.MapSpec.Of("m13_01_00_00", "totg", 3961, 3964),
      AnnotationData.MapSpec.Of("m13_02_00_00", "greathollow", 3970, 3971),
      AnnotationData.MapSpec.Of("m14_00_00_00", "blighttown", 4950, 4956),
      AnnotationData.MapSpec.Of("m14_01_00_00", "demonruins", 4960, 4973),
      AnnotationData.MapSpec.Of("m15_00_00_00", "sens", 5150, 5153),
      AnnotationData.MapSpec.Of("m15_01_00_00", "anorlondo", 5860, 5871),
      AnnotationData.MapSpec.Of("m16_00_00_00", "newlondo", 6601, 6606),
      AnnotationData.MapSpec.Of("m17_00_00_00", "dukes", 7900, 7908),
      AnnotationData.MapSpec.Of("m18_00_00_00", "kiln", 8050, 8051),
      AnnotationData.MapSpec.Of("m18_01_00_00", "asylum", 8950, 8953)
    };
    public static readonly List<AnnotationData.MapSpec> DS3Specs = new List<AnnotationData.MapSpec>()
    {
      AnnotationData.MapSpec.Of("m30_00_00_00", "highwall", 400, 402),
      AnnotationData.MapSpec.Of("m30_01_00_00", "lothric", 400, 402),
      AnnotationData.MapSpec.Of("m31_00_00_00", "settlement", 400, 402),
      AnnotationData.MapSpec.Of("m32_00_00_00", "archdragon", 400, 402),
      AnnotationData.MapSpec.Of("m33_00_00_00", "farronkeep", 400, 402),
      AnnotationData.MapSpec.Of("m34_01_00_00", "archives", 400, 402),
      AnnotationData.MapSpec.Of("m35_00_00_00", "cathedral", 400, 402),
      AnnotationData.MapSpec.Of("m37_00_00_00", "irithyll", 400, 402),
      AnnotationData.MapSpec.Of("m38_00_00_00", "catacombs", 400, 402),
      AnnotationData.MapSpec.Of("m39_00_00_00", "dungeon", 400, 402),
      AnnotationData.MapSpec.Of("m40_00_00_00", "firelink", 400, 402),
      AnnotationData.MapSpec.Of("m41_00_00_00", "kiln", 400, 402),
      AnnotationData.MapSpec.Of("m45_00_00_00", "ariandel", 400, 402),
      AnnotationData.MapSpec.Of("m50_00_00_00", "dregheap", 400, 402),
      AnnotationData.MapSpec.Of("m51_00_00_00", "ringedcity", 400, 402),
      AnnotationData.MapSpec.Of("m51_01_00_00", "filianore", 400, 402)
    };

    public List<AnnotationData.ConfigAnnotation> Options { get; set; }

    public float HealthScaling { get; set; }

    public float DamageScaling { get; set; }

    public List<AnnotationData.Area> Areas { get; set; }

    public List<AnnotationData.Item> KeyItems { get; set; }

    public List<AnnotationData.Entrance> Warps { get; set; }

    public List<AnnotationData.Entrance> Entrances { get; set; }

    public List<AnnotationData.GameObject> Objects { get; set; }

    public List<AnnotationData.CustomStart> CustomStarts { get; set; }

    public Dictionary<string, float> DefaultCost { get; set; }

    public Dictionary<string, float> MaxScaling { get; set; }

    public List<AnnotationData.EnemyCol> Enemies { get; set; }

    public Dictionary<int, string> LotLocations { get; set; }

    public Dictionary<string, int> DefaultFlagCols { get; set; }

    [YamlIgnore]
    public AnnotationData.FogLocations Locations { get; set; }

    [YamlIgnore]
    public Dictionary<string, AnnotationData.MapSpec> Specs { get; set; }

    [YamlIgnore]
    public Dictionary<string, AnnotationData.MapSpec> NameSpecs { get; set; }

    public void SetGame(GameSpec.FromGame game)
    {
      List<AnnotationData.MapSpec> source = game == GameSpec.FromGame.DS3 ? AnnotationData.DS3Specs : AnnotationData.DS1Specs;
      this.Specs = source.ToDictionary<AnnotationData.MapSpec, string, AnnotationData.MapSpec>((Func<AnnotationData.MapSpec, string>) (s => s.Map), (Func<AnnotationData.MapSpec, AnnotationData.MapSpec>) (s => s));
      this.NameSpecs = source.ToDictionary<AnnotationData.MapSpec, string, AnnotationData.MapSpec>((Func<AnnotationData.MapSpec, string>) (s => s.Name), (Func<AnnotationData.MapSpec, AnnotationData.MapSpec>) (s => s));
    }

    public static AnnotationData.Expr ParseExpr(string s)
    {
      if (s == null)
        return (AnnotationData.Expr) null;
      string[] strArray = s.Split(' ');
      if (strArray.Length == 1)
        return AnnotationData.Expr.Named(strArray[0]);
      if (strArray.Length > 1 && strArray[0] == "AND")
        return new AnnotationData.Expr(((IEnumerable<string>) strArray).Skip<string>(1).Select<string, AnnotationData.Expr>((Func<string, AnnotationData.Expr>) (w => AnnotationData.Expr.Named(w))).ToList<AnnotationData.Expr>(), true, (string) null);
      if (strArray.Length > 1 && strArray[0] == "OR")
        return new AnnotationData.Expr(((IEnumerable<string>) strArray).Skip<string>(1).Select<string, AnnotationData.Expr>((Func<string, AnnotationData.Expr>) (w => AnnotationData.Expr.Named(w))).ToList<AnnotationData.Expr>(), false, (string) null);
      throw new Exception("Unknown cond " + s);
    }

    public class ConfigAnnotation
    {
      public string Opt { get; set; }

      public string TrueOpt { get; set; }

      public void UpdateOptions(RandomizerOptions options)
      {
        if (this.Opt != null)
          options[this.Opt] = false;
        if (this.TrueOpt == null)
          return;
        options[this.TrueOpt] = true;
      }
    }

    public abstract class Taggable
    {
      [YamlIgnore]
      public List<string> TagList = new List<string>();
      [YamlIgnore]
      private string tags;

      public string Tags
      {
        get
        {
          return this.tags;
        }
        set
        {
          this.tags = value;
          if (this.tags == null)
            this.TagList = new List<string>();
          else
            this.TagList = ((IEnumerable<string>) this.Tags.Split(' ')).ToList<string>();
        }
      }

      public bool HasTag(string tag)
      {
        return this.TagList.Contains(tag);
      }
    }

    public class Area : AnnotationData.Taggable
    {
      public string Name { get; set; }

      public string Text { get; set; }

      public string ScalingBase { get; set; }

      public int BossTrigger { get; set; }

      public int DefeatFlag { get; set; }

      public int TrapFlag { get; set; }

      public List<AnnotationData.Side> To { get; set; }
    }

    public class EnemyCol
    {
      public string Col { get; set; }

      public string Area { get; set; }

      public List<string> Includes { get; set; }
    }

    public class Item : AnnotationData.Taggable
    {
      public string Name { get; set; }

      public string ID { get; set; }

      public string Area { get; set; }
    }

    public class CustomStart
    {
      public string Name { get; set; }

      public string Area { get; set; }

      public string Respawn { get; set; }
    }

    public class Entrance : AnnotationData.Taggable
    {
      public string Name { get; set; }

      public int ID { get; set; }

      public string Area { get; set; }

      public string Text { get; set; }

      public string Comment { get; set; }

      public string DoorCond { get; set; }

      public float AdjustHeight { get; set; }

      public AnnotationData.Side ASide { get; set; }

      public AnnotationData.Side BSide { get; set; }

      [YamlIgnore]
      public bool IsFixed { get; set; }

      [YamlIgnore]
      public string FullName
      {
        get
        {
          return this.Area + "_" + this.ID.ToString();
        }
      }

      public List<AnnotationData.Side> Sides()
      {
        List<AnnotationData.Side> sideList = new List<AnnotationData.Side>();
        if (this.ASide != null)
          sideList.Add(this.ASide);
        if (this.BSide != null)
          sideList.Add(this.BSide);
        return sideList;
      }
    }

    public class Side : AnnotationData.Taggable
    {
      public string Area { get; set; }

      public string Text { get; set; }

      public int Flag { get; set; }

      public int TrapFlag { get; set; }

      public int EntryFlag { get; set; }

      public int BeforeWarpFlag { get; set; }

      public int BossTrigger { get; set; }

      public string BossTriggerArea { get; set; }

      public int WarpFlag { get; set; }

      public string BossDefeatName { get; set; }

      public string BossTrapName { get; set; }

      public string BossTriggerName { get; set; }

      public int Cutscene { get; set; }

      public string Cond { get; set; }

      public string CustomWarp { get; set; }

      public int CustomActionWidth { get; set; }

      public string Col { get; set; }

      public int ActionRegion { get; set; }

      public string ExcludeIfRandomized { get; set; }

      public string DestinationMap { get; set; }

      public float AdjustHeight { get; set; }

      [YamlIgnore]
      public AnnotationData.Expr Expr { get; set; }

      [YamlIgnore]
      public Graph.WarpPoint Warp { get; set; }
    }

    public class GameObject : AnnotationData.Taggable
    {
      public string Area { get; set; }

      public string ID { get; set; }

      public string Text { get; set; }
    }

    public class MapSpec
    {
      public static AnnotationData.MapSpec Of(
        string Map,
        string Name,
        int Start,
        int End)
      {
        return new AnnotationData.MapSpec()
        {
          Map = Map,
          Name = Name,
          Start = Start,
          End = End
        };
      }

      public string Map { get; set; }

      public string Name { get; set; }

      public int Start { get; set; }

      public int End { get; set; }
    }

    public class Expr
    {
      public static readonly AnnotationData.Expr TRUE = new AnnotationData.Expr(new List<AnnotationData.Expr>(), true, (string) null);
      public static readonly AnnotationData.Expr FALSE = new AnnotationData.Expr(new List<AnnotationData.Expr>(), false, (string) null);
      private readonly List<AnnotationData.Expr> exprs;
      private readonly bool every;
      private readonly string name;

      public Expr(List<AnnotationData.Expr> exprs, bool every = true, string name = null)
      {
        if (exprs.Count<AnnotationData.Expr>() > 0 && name != null)
          throw new Exception("Incorrect construction");
        this.exprs = exprs;
        this.every = every;
        this.name = name;
      }

      public static AnnotationData.Expr Named(string name)
      {
        return new AnnotationData.Expr(new List<AnnotationData.Expr>(), true, name);
      }

      public bool IsTrue()
      {
        return this.name == null && this.exprs.Count<AnnotationData.Expr>() == 0 && this.every;
      }

      public bool IsFalse()
      {
        return this.name == null && this.exprs.Count<AnnotationData.Expr>() == 0 && !this.every;
      }

      public SortedSet<string> FreeVars()
      {
        if (this.name == null)
          return new SortedSet<string>(this.exprs.SelectMany<AnnotationData.Expr, string>((Func<AnnotationData.Expr, IEnumerable<string>>) (e => (IEnumerable<string>) e.FreeVars())));
        return new SortedSet<string>() { this.name };
      }

      public bool Needs(string check)
      {
        if (check == this.name)
          return true;
        return this.every ? this.exprs.Any<AnnotationData.Expr>((Func<AnnotationData.Expr, bool>) (e => e.Needs(check))) : this.exprs.All<AnnotationData.Expr>((Func<AnnotationData.Expr, bool>) (e => e.Needs(check)));
      }

      public AnnotationData.Expr Substitute(
        Dictionary<string, AnnotationData.Expr> config)
      {
        if (this.name == null)
          return new AnnotationData.Expr(this.exprs.Select<AnnotationData.Expr, AnnotationData.Expr>((Func<AnnotationData.Expr, AnnotationData.Expr>) (e => e.Substitute(config))).ToList<AnnotationData.Expr>(), this.every, (string) null);
        return config.ContainsKey(this.name) ? config[this.name] : this;
      }

      public AnnotationData.Expr Flatten(Func<string, IEnumerable<string>> nameMapper)
      {
        return this.name != null ? new AnnotationData.Expr(nameMapper(this.name).Select<string, AnnotationData.Expr>((Func<string, AnnotationData.Expr>) (n => AnnotationData.Expr.Named(n))).ToList<AnnotationData.Expr>(), true, (string) null) : (AnnotationData.Expr) null;
      }

      public int Count(Func<string, int> func)
      {
        if (this.name != null)
          return func(this.name);
        IEnumerable<int> source = this.exprs.Select<AnnotationData.Expr, int>((Func<AnnotationData.Expr, int>) (e => e.Count(func)));
        return !this.every ? source.Max() : source.Sum();
      }

      public (List<string>, float) Cost(Func<string, float> cost)
      {
        if (this.name != null)
          return (new List<string>() { this.name }, cost(this.name));
        if (this.exprs.Count == 0)
          return (new List<string>(), 0.0f);
        IEnumerable<(List<string>, float)> source = this.exprs.Select<AnnotationData.Expr, (List<string>, float)>((Func<AnnotationData.Expr, (List<string>, float)>) (e => e.Cost(cost)));
        return !this.every ? source.OrderBy<(List<string>, float), float>((Func<(List<string>, float), float>) (c => c.Item2)).First<(List<string>, float)>() : source.Aggregate<(List<string>, float)>((Func<(List<string>, float), (List<string>, float), (List<string>, float)>) ((c1, c2) => (c1.Item1.Concat<string>((IEnumerable<string>) c2.Item1).ToList<string>(), c1.Item2 + c2.Item2)));
      }

      public AnnotationData.Expr Simplify()
      {
        if (this.name != null)
          return this;
        List<AnnotationData.Expr> exprList = new List<AnnotationData.Expr>();
        HashSet<string> stringSet = new HashSet<string>();
        foreach (AnnotationData.Expr expr1 in this.exprs)
        {
          AnnotationData.Expr expr2 = expr1.Simplify();
          if (expr2.name != null)
          {
            if (!stringSet.Contains(expr2.name))
            {
              stringSet.Add(expr2.name);
              exprList.Add(expr2);
            }
          }
          else if (this.every == expr2.every)
          {
            exprList.AddRange((IEnumerable<AnnotationData.Expr>) expr2.exprs);
          }
          else
          {
            if (expr2.exprs.Count<AnnotationData.Expr>() == 0)
              return expr2.every ? AnnotationData.Expr.TRUE : AnnotationData.Expr.FALSE;
            exprList.Add(expr2);
          }
        }
        return exprList.Count<AnnotationData.Expr>() == 1 ? exprList[0] : new AnnotationData.Expr(exprList, this.every, (string) null);
      }

      public override string ToString()
      {
        if (this.name != null)
          return this.name;
        return this.exprs.Count<AnnotationData.Expr>() == 0 ? (!this.every ? "false" : "true") : (this.every ? "(" + string.Join<AnnotationData.Expr>(" AND ", (IEnumerable<AnnotationData.Expr>) this.exprs) + ")" : "(" + string.Join<AnnotationData.Expr>(" OR ", (IEnumerable<AnnotationData.Expr>) this.exprs) + ")");
      }
    }

    public class FogLocations
    {
      public List<AnnotationData.KeyItemLoc> Items = new List<AnnotationData.KeyItemLoc>();
      public List<AnnotationData.EnemyLoc> Enemies = new List<AnnotationData.EnemyLoc>();
    }

    public class KeyItemLoc
    {
      public string Key { get; set; }

      public List<string> DebugText { get; set; }

      public string Area { get; set; }

      public string Lots { get; set; }

      public string Shops { get; set; }
    }

    public class EnemyLoc
    {
      public string ID { get; set; }

      public string DebugText { get; set; }

      public string Area { get; set; }
    }
  }
}
