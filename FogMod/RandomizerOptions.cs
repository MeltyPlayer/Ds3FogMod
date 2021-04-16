// Decompiled with JetBrains decompiler
// Type: FogMod.RandomizerOptions
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsIds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FogMod
{
  public class RandomizerOptions
  {
    private SortedDictionary<string, bool> opt = new SortedDictionary<string, bool>()
    {
      {
        "v4",
        true
      }
    };
    public string Language = "ENGLISH";

    public RandomizerOptions Copy()
    {
      return new RandomizerOptions()
      {
        opt = new SortedDictionary<string, bool>((IDictionary<string, bool>) this.opt),
        Seed = this.Seed,
        Game = this.Game
      };
    }

    public bool this[string name]
    {
      get
      {
        return this.opt.ContainsKey(name) && this.opt[name];
      }
      set
      {
        this.opt[name] = value;
      }
    }

    public int Seed { get; set; }

    public GameSpec.FromGame Game { get; set; }

    public uint DisplaySeed
    {
      get
      {
        return (uint) this.Seed;
      }
    }

    public SortedSet<string> GetEnabled()
    {
      return new SortedSet<string>(this.opt.Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (e => e.Value)).Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (e => e.Key)));
    }

    public override string ToString()
    {
      return string.Format("{0} {1}", (object) string.Join(" ", (IEnumerable<string>) this.GetEnabled()), (object) this.DisplaySeed);
    }

    public string ConfigHash()
    {
      return (RandomizerOptions.JavaStringHash(string.Join(" ", (IEnumerable<string>) this.GetEnabled()) ?? "") % 99999U).ToString().PadLeft(5, '0');
    }

    public static uint JavaStringHash(string s)
    {
      uint num = 0;
      foreach (char ch in s)
        num = num * 31U + (uint) ch;
      return num;
    }
  }
}
