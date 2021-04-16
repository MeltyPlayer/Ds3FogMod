// Decompiled with JetBrains decompiler
// Type: SoulsIds.EMEDF
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsIds
{
  public class EMEDF
  {
    [JsonProperty(PropertyName = "unknown")]
    public long UNK;
    [JsonProperty(PropertyName = "main_classes")]
    public List<EMEDF.ClassDoc> Classes;
    [JsonProperty(PropertyName = "enums")]
    public EMEDF.EnumDoc[] Enums;

    public EMEDF.ClassDoc this[int classIndex]
    {
      get
      {
        return this.Classes.Find((Predicate<EMEDF.ClassDoc>) (c => c.Index == (long) classIndex));
      }
    }

    public static EMEDF ReadText(string input)
    {
      return JsonConvert.DeserializeObject<EMEDF>(input);
    }

    public static EMEDF ReadFile(string path)
    {
      return EMEDF.ReadText(File.ReadAllText(path));
    }

    public class ClassDoc
    {
      [JsonProperty(PropertyName = "name")]
      public string Name { get; set; }

      [JsonProperty(PropertyName = "index")]
      public long Index { get; set; }

      [JsonProperty(PropertyName = "instrs")]
      public List<EMEDF.InstrDoc> Instructions { get; set; }

      public EMEDF.InstrDoc this[int instructionIndex]
      {
        get
        {
          return this.Instructions.Find((Predicate<EMEDF.InstrDoc>) (ins => ins.Index == (long) instructionIndex));
        }
      }
    }

    public class InstrDoc
    {
      [JsonProperty(PropertyName = "name")]
      public string Name { get; set; }

      [JsonProperty(PropertyName = "index")]
      public long Index { get; set; }

      [JsonProperty(PropertyName = "args")]
      public EMEDF.ArgDoc[] Arguments { get; set; }

      public EMEDF.ArgDoc this[uint i]
      {
        get
        {
          return this.Arguments[(int) i];
        }
      }
    }

    public class ArgDoc
    {
      [JsonProperty(PropertyName = "name")]
      public string Name { get; set; }

      [JsonProperty(PropertyName = "type")]
      public long Type { get; set; }

      [JsonProperty(PropertyName = "enum_name")]
      public string EnumName { get; set; }

      [JsonProperty(PropertyName = "default")]
      public long Default { get; set; }

      [JsonProperty(PropertyName = "min")]
      public long Min { get; set; }

      [JsonProperty(PropertyName = "max")]
      public long Max { get; set; }

      [JsonProperty(PropertyName = "increment")]
      public long Increment { get; set; }

      [JsonProperty(PropertyName = "format_string")]
      public string FormatString { get; set; }

      [JsonProperty(PropertyName = "unk1")]
      private long UNK1 { get; set; }

      [JsonProperty(PropertyName = "unk2")]
      private long UNK2 { get; set; }

      [JsonProperty(PropertyName = "unk3")]
      private long UNK3 { get; set; }

      [JsonProperty(PropertyName = "unk4")]
      private long UNK4 { get; set; }
    }

    public class EnumDoc
    {
      [JsonProperty(PropertyName = "name")]
      public string Name { get; set; }

      [JsonProperty(PropertyName = "values")]
      public Dictionary<string, string> Values { get; set; }
    }
  }
}
