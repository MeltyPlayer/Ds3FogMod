// Decompiled with JetBrains decompiler
// Type: SoulsFormats.PARAMTDF
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class PARAMTDF {
    private PARAMDEF.DefType type;

    public string Name { get; set; }

    public PARAMDEF.DefType Type {
      get { return this.type; }
      set {
        if (value != PARAMDEF.DefType.s8 &&
            value != PARAMDEF.DefType.u8 &&
            (value != PARAMDEF.DefType.s16 && value != PARAMDEF.DefType.u16) &&
            (value != PARAMDEF.DefType.s32 && value != PARAMDEF.DefType.u32))
          throw new ArgumentException(
              string.Format(
                  "TDF type may only be s8, u8, s16, u16, s32, or u32, but {0} was given.",
                  (object) value));
        this.type = value;
      }
    }

    public List<PARAMTDF.Entry> Entries { get; set; }

    public object this[string name] {
      get {
        return this
               .Entries.Find((Predicate<PARAMTDF.Entry>) (e => e.Name == name))
               .Value;
      }
    }

    public string this[object value] {
      get {
        return this
               .Entries
               .Find((Predicate<PARAMTDF.Entry>) (e => e.Value == value))
               .Name;
      }
    }

    public PARAMTDF() {
      this.Name = "UNSPECIFIED";
      this.Type = PARAMDEF.DefType.s32;
      this.Entries = new List<PARAMTDF.Entry>();
    }

    public PARAMTDF(string text) {
      string[] strArray1 = text.Split(new char[2] {
                                          '\r',
                                          '\n'
                                      },
                                      StringSplitOptions.RemoveEmptyEntries);
      this.Name = strArray1[0].Trim('"');
      this.Type =
          (PARAMDEF.DefType) System.Enum.Parse(typeof(PARAMDEF.DefType),
                                               strArray1[1].Trim('"'));
      this.Entries = new List<PARAMTDF.Entry>(strArray1.Length - 2);
      for (int index = 2; index < strArray1.Length; ++index) {
        string[] strArray2 = strArray1[index].Split(',');
        string s = strArray2[1].Trim('"');
        object obj;
        switch (this.Type) {
          case PARAMDEF.DefType.s8:
            obj = (object) sbyte.Parse(s);
            break;
          case PARAMDEF.DefType.u8:
            obj = (object) byte.Parse(s);
            break;
          case PARAMDEF.DefType.s16:
            obj = (object) short.Parse(s);
            break;
          case PARAMDEF.DefType.u16:
            obj = (object) ushort.Parse(s);
            break;
          case PARAMDEF.DefType.s32:
            obj = (object) int.Parse(s);
            break;
          case PARAMDEF.DefType.u32:
            obj = (object) uint.Parse(s);
            break;
          default:
            throw new NotImplementedException(
                string.Format("Parsing not implemented for type {0}.",
                              (object) this.Type));
        }
        if (strArray2[0] == "")
          this.Entries.Add(new PARAMTDF.Entry((string) null, obj));
        else
          this.Entries.Add(new PARAMTDF.Entry(strArray2[0].Trim('"'), obj));
      }
    }

    public override string ToString() {
      return string.Format("{0} {1}", (object) this.Type, (object) this.Name);
    }

    public class Entry {
      public string Name { get; set; }

      public object Value { get; set; }

      public Entry(string name, object value) {
        this.Name = name;
        this.Value = value;
      }

      public override string ToString() {
        return string.Format("{0} = {1}",
                             (object) (this.Name ?? "<null>"),
                             this.Value);
      }
    }
  }
}