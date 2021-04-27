// Decompiled with JetBrains decompiler
// Type: SoulsFormats.PARAM
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;

using FogMod.util.time;

namespace SoulsFormats {
  [ComVisible(true)]
  public class PARAM : SoulsFile<PARAM> {
    private BinaryReaderEx RowReader;

    public bool BigEndian { get; set; }

    public byte Format2D { get; set; }

    public byte Format2E { get; set; }

    public byte Format2F { get; set; }

    public short Unk06 { get; set; }

    public short Unk08 { get; set; }

    public string ParamType { get; set; }

    public long DetectedSize { get; private set; }

    public List<PARAM.Row> Rows { get; set; }

    public PARAMDEF AppliedParamdef { get; private set; }

    protected override void Read(BinaryReaderEx br) {
      br.Position = 44L;
      this.BigEndian = br.AssertByte((byte) 0, byte.MaxValue) == byte.MaxValue;
      this.Format2D = br.ReadByte();
      this.Format2E = br.ReadByte();
      this.Format2F = br.AssertByte((byte) 0, byte.MaxValue);
      br.Position = 0L;
      br.BigEndian = this.BigEndian;
      this.RowReader =
          new BinaryReaderEx(this.BigEndian,
                             br.GetBytes(0L, (int) br.Stream.Length));
      long num1;
      ushort num2;
      if (((int) this.Format2D & (int) sbyte.MaxValue) < 3) {
        num1 = (long) br.ReadUInt32();
        int num3 = (int) br.ReadUInt16();
        this.Unk06 = br.ReadInt16();
        this.Unk08 = br.ReadInt16();
        num2 = br.ReadUInt16();
        this.ParamType = br.ReadFixStr(32);
        br.Skip(4);
      } else if (((int) this.Format2D & (int) sbyte.MaxValue) == 3) {
        num1 = (long) br.ReadUInt32();
        int num3 = (int) br.AssertInt16(new short[1]);
        this.Unk06 = br.ReadInt16();
        this.Unk08 = br.ReadInt16();
        num2 = br.ReadUInt16();
        this.ParamType = br.ReadFixStr(32);
        br.Skip(4);
        int num4 = (int) br.ReadUInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
      } else if (((int) this.Format2D & (int) sbyte.MaxValue) == 4) {
        num1 = (long) br.ReadUInt32();
        int num3 = (int) br.AssertInt16(new short[1]);
        this.Unk06 = br.ReadInt16();
        this.Unk08 = br.ReadInt16();
        num2 = br.ReadUInt16();
        this.ParamType = br.ReadFixStr(32);
        br.Skip(4);
        br.ReadInt64();
        br.AssertInt64(new long[1]);
      } else {
        long num3 = (long) br.ReadUInt32();
        int num4 = (int) br.AssertInt16(new short[1]);
        this.Unk06 = br.ReadInt16();
        this.Unk08 = br.ReadInt16();
        num2 = br.ReadUInt16();
        br.AssertInt32(new int[1]);
        long offset = br.ReadInt64();
        br.AssertPattern(20, (byte) 0);
        br.Skip(4);
        br.ReadInt64();
        br.AssertInt64(new long[1]);
        this.ParamType = br.GetASCII(offset);
        num1 = offset;
      }
      this.Rows = new List<PARAM.Row>((int) num2);
      for (int index = 0; index < (int) num2; ++index)
        this.Rows.Add(new PARAM.Row(br, this.Format2D, this.Format2E));
      if (this.Rows.Count > 1)
        this.DetectedSize = this.Rows[1].DataOffset - this.Rows[0].DataOffset;
      else
        this.DetectedSize = num1 - this.Rows[0].DataOffset;
    }

    protected override void Write(BinaryWriterEx bw) {
      if (this.AppliedParamdef == null)
        throw new InvalidOperationException(
            "Params cannot be written without applying a paramdef.");

      // ISSUE: reference to a compiler-generated field
      bw.BigEndian = this.BigEndian;

      void WriteFormat() {
        bw.WriteByte(this.BigEndian ? byte.MaxValue : (byte) 0);
        bw.WriteByte(this.Format2D);
        bw.WriteByte(this.Format2E);
        bw.WriteByte(this.Format2F);
      }

      if (((int) this.Format2D & (int) sbyte.MaxValue) < 3) {
        // ISSUE: reference to a compiler-generated field
        bw.ReserveUInt32("StringsOffset");
        // ISSUE: reference to a compiler-generated field
        bw.ReserveUInt16("DataStart");
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16(this.Unk06);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16(this.Unk08);
        // ISSUE: reference to a compiler-generated field
        bw.WriteUInt16((ushort) this.Rows.Count);
        // ISSUE: reference to a compiler-generated field
        bw.WriteFixStr(this.ParamType,
                       32,
                       ((int) this.Format2D & (int) sbyte.MaxValue) < 2
                           ? (byte) 32
                           : (byte) 0);
        WriteFormat();
      } else if (((int) this.Format2D & (int) sbyte.MaxValue) == 3) {
        // ISSUE: reference to a compiler-generated field
        bw.ReserveUInt32("StringsOffset");
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16((short) 0);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16(this.Unk06);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16(this.Unk08);
        // ISSUE: reference to a compiler-generated field
        bw.WriteUInt16((ushort) this.Rows.Count);
        // ISSUE: reference to a compiler-generated field
        bw.WriteFixStr(this.ParamType, 32, (byte) 32);
        WriteFormat();
        // ISSUE: reference to a compiler-generated field
        bw.ReserveUInt32("DataStart");
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt32(0);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt32(0);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt32(0);
      } else if (((int) this.Format2D & (int) sbyte.MaxValue) == 4) {
        // ISSUE: reference to a compiler-generated field
        bw.ReserveUInt32("StringsOffset");
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16((short) 0);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16(this.Unk06);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16(this.Unk08);
        // ISSUE: reference to a compiler-generated field
        bw.WriteUInt16((ushort) this.Rows.Count);
        // ISSUE: reference to a compiler-generated field
        bw.WriteFixStr(this.ParamType, 32, (byte) 0);
        WriteFormat();
        // ISSUE: reference to a compiler-generated field
        bw.ReserveInt64("DataStart");
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt64(0L);
      } else {
        // ISSUE: reference to a compiler-generated field
        bw.ReserveUInt32("StringsOffset");
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16((short) 0);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16(this.Unk06);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt16(this.Unk08);
        // ISSUE: reference to a compiler-generated field
        bw.WriteUInt16((ushort) this.Rows.Count);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt32(0);
        // ISSUE: reference to a compiler-generated field
        bw.ReserveInt64("IDOffset");
        // ISSUE: reference to a compiler-generated field
        bw.WritePattern(20, (byte) 0);
        WriteFormat();
        // ISSUE: reference to a compiler-generated field
        bw.ReserveInt64("DataStart");
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt64(0L);
      }
      for (int i = 0; i < this.Rows.Count; ++i) {
        // ISSUE: reference to a compiler-generated field
        this.Rows[i].WriteHeader(bw, this.Format2D, i);
      }
      if (((int) this.Format2D & (int) sbyte.MaxValue) < 2) {
        // ISSUE: reference to a compiler-generated field
        bw.WritePattern(32, (byte) 0);
      }
      if (((int) this.Format2D & (int) sbyte.MaxValue) < 3) {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        bw.FillUInt16("DataStart", (ushort) bw.Position);
      } else if (((int) this.Format2D & (int) sbyte.MaxValue) == 3) {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        bw.FillUInt32("DataStart", (uint) bw.Position);
      } else {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        bw.FillInt64("DataStart", bw.Position);
      }
      for (int index = 0; index < this.Rows.Count; ++index) {
        // ISSUE: reference to a compiler-generated field
        this.Rows[index].WriteCells(bw, this.Format2D, index);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.FillUInt32("StringsOffset", (uint) bw.Position);
      if (((int) this.Format2D & (int) sbyte.MaxValue) > 4) {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        bw.FillInt64("IDOffset", bw.Position);
        // ISSUE: reference to a compiler-generated field
        bw.WriteASCII(this.ParamType, true);
      }
      for (int i = 0; i < this.Rows.Count; ++i) {
        // ISSUE: reference to a compiler-generated field
        this.Rows[i].WriteName(bw, this.Format2D, this.Format2E, i);
      }
    }

    public void ApplyParamdef(PARAMDEF paramdef) {
      this.AppliedParamdef = paramdef;
      foreach (PARAM.Row row in this.Rows) {
        row.ScheduleUpdateCells(this.RowReader, this.AppliedParamdef);
      }
    }

    public PARAM.Row this[int id] {
      get {
        return this.Rows.Find(
            (Predicate<PARAM.Row>) (row => row.ID == (long) id));
      }
    }

    public class Enum : List<PARAM.Enum.Item> {
      public PARAM.CellType Type { get; }

      public Enum(PARAM.CellType type) {
        this.Type = type;
      }

      internal Enum(XmlNode node) {
        this.Type =
            (PARAM.CellType) System.Enum.Parse(typeof(PARAM.CellType),
                                               node.Attributes["type"]
                                                   .InnerText);
        foreach (XmlNode selectNode in node.SelectNodes("item"))
          this.Add(new PARAM.Enum.Item(selectNode.Attributes["name"].InnerText,
                                       PARAM.Layout.ParseParamValue(
                                           this.Type,
                                           selectNode
                                               .Attributes["value"]
                                               .InnerText)));
      }

      public PARAMTDF ToParamtdf(string name) {
        PARAMDEF.DefType defType;
        switch (this.Type) {
          case PARAM.CellType.u8:
          case PARAM.CellType.x8:
            defType = PARAMDEF.DefType.u8;
            break;
          case PARAM.CellType.s8:
            defType = PARAMDEF.DefType.s8;
            break;
          case PARAM.CellType.u16:
          case PARAM.CellType.x16:
            defType = PARAMDEF.DefType.u16;
            break;
          case PARAM.CellType.s16:
            defType = PARAMDEF.DefType.s16;
            break;
          case PARAM.CellType.u32:
          case PARAM.CellType.x32:
            defType = PARAMDEF.DefType.u32;
            break;
          case PARAM.CellType.s32:
            defType = PARAMDEF.DefType.s32;
            break;
          default:
            throw new InvalidDataException(
                string.Format("Layout.Enum type {0} may not be used in a TDF.",
                              (object) this.Type));
        }
        PARAMTDF paramtdf = new PARAMTDF() {
            Name = name,
            Type = defType
        };
        foreach (PARAM.Enum.Item obj in (List<PARAM.Enum.Item>) this)
          paramtdf.Entries.Add(new PARAMTDF.Entry(obj.Name, obj.Value));
        return paramtdf;
      }

      public class Item {
        public string Name { get; }

        public object Value { get; }

        public Item(string name, object value) {
          this.Name = name;
          this.Value = value;
        }
      }
    }

    public class Layout : List<PARAM.Layout.Entry> {
      public Dictionary<string, PARAM.Enum> Enums;

      public int Size {
        get {
          int size = 0;
          for (var i = 0; i < this.Count; i++) {
            PARAM.CellType type = this[i].Type;

            void ConsumeBools(
                PARAM.CellType boolType,
                int fieldSize
            ) {
              size += fieldSize;
              int num = 0;
              while (num < fieldSize * 8 &&
                     (i + num < this.Count &&
                      this[i + num].Type == boolType))
                ++num;
              i += num - 1;
            }

            switch (type) {
              case PARAM.CellType.b8:
                ConsumeBools(type, 1);
                break;
              case PARAM.CellType.b16:
                ConsumeBools(type, 2);
                break;
              case PARAM.CellType.b32:
                ConsumeBools(type, 4);
                break;
              default:
                size += this[i].Size;
                break;
            }
          }
          return size;
        }
      }

      public static PARAM.Layout ReadXMLFile(string path) {
        XmlDocument xml = new XmlDocument();
        xml.Load(path);
        return new PARAM.Layout(xml);
      }

      public static PARAM.Layout ReadXMLText(string text) {
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(text);
        return new PARAM.Layout(xml);
      }

      public static PARAM.Layout ReadXMLDoc(XmlDocument xml) {
        return new PARAM.Layout(xml);
      }

      public Layout() {
        this.Enums = new Dictionary<string, PARAM.Enum>();
      }

      private Layout(XmlDocument xml) {
        this.Enums = new Dictionary<string, PARAM.Enum>();
        foreach (XmlNode selectNode in xml.SelectNodes("/layout/enum"))
          this.Enums[selectNode.Attributes["name"].InnerText] =
              new PARAM.Enum(selectNode);
        foreach (XmlNode selectNode in xml.SelectNodes("/layout/entry"))
          this.Add(new PARAM.Layout.Entry(selectNode));
      }

      public void Write(string path) {
        XmlWriterSettings settings = new XmlWriterSettings() {
            Indent = true
        };
        XmlWriter xw = XmlWriter.Create(path, settings);
        xw.WriteStartElement("layout");
        foreach (PARAM.Layout.Entry entry in (List<PARAM.Layout.Entry>) this)
          entry.Write(xw);
        xw.WriteEndElement();
        xw.Close();
      }

      public PARAMDEF ToParamdef(
          string paramType,
          out List<PARAMTDF> paramtdfs) {
        paramtdfs = new List<PARAMTDF>(this.Enums.Count);
        foreach (string key in this.Enums.Keys)
          paramtdfs.Add(this.Enums[key].ToParamtdf(key));
        PARAMDEF paramdef = new PARAMDEF() {
            ParamType = paramType,
            Unicode = true,
            Version = 201
        };
        foreach (PARAM.Layout.Entry entry in (List<PARAM.Layout.Entry>) this) {
          PARAMDEF.DefType displayType;
          switch (entry.Type) {
            case PARAM.CellType.dummy8:
              displayType = PARAMDEF.DefType.dummy8;
              break;
            case PARAM.CellType.b8:
            case PARAM.CellType.u8:
            case PARAM.CellType.x8:
              displayType = PARAMDEF.DefType.u8;
              break;
            case PARAM.CellType.b16:
            case PARAM.CellType.u16:
            case PARAM.CellType.x16:
              displayType = PARAMDEF.DefType.u16;
              break;
            case PARAM.CellType.b32:
            case PARAM.CellType.u32:
            case PARAM.CellType.x32:
              displayType = PARAMDEF.DefType.u32;
              break;
            case PARAM.CellType.s8:
              displayType = PARAMDEF.DefType.s8;
              break;
            case PARAM.CellType.s16:
              displayType = PARAMDEF.DefType.s16;
              break;
            case PARAM.CellType.s32:
              displayType = PARAMDEF.DefType.s32;
              break;
            case PARAM.CellType.f32:
              displayType = PARAMDEF.DefType.f32;
              break;
            case PARAM.CellType.fixstr:
              displayType = PARAMDEF.DefType.fixstr;
              break;
            case PARAM.CellType.fixstrW:
              displayType = PARAMDEF.DefType.fixstrW;
              break;
            default:
              throw new NotImplementedException(
                  string.Format("DefType not specified for CellType {0}.",
                                (object) entry.Type));
          }
          PARAMDEF.Field field = new PARAMDEF.Field(displayType, entry.Name);
          field.Description = entry.Description;
          if (entry.Enum != null)
            field.InternalType = entry.Enum;
          if (entry.Type == PARAM.CellType.s8)
            field.Default = (float) (sbyte) entry.Default;
          else if (entry.Type == PARAM.CellType.u8 ||
                   entry.Type == PARAM.CellType.x8)
            field.Default = (float) (byte) entry.Default;
          else if (entry.Type == PARAM.CellType.s16)
            field.Default = (float) (short) entry.Default;
          else if (entry.Type == PARAM.CellType.u16 ||
                   entry.Type == PARAM.CellType.x16)
            field.Default = (float) (ushort) entry.Default;
          else if (entry.Type == PARAM.CellType.s32)
            field.Default = (float) (int) entry.Default;
          else if (entry.Type == PARAM.CellType.u32 ||
                   entry.Type == PARAM.CellType.x32)
            field.Default = (float) (uint) entry.Default;
          else if (entry.Type == PARAM.CellType.dummy8 ||
                   entry.Type == PARAM.CellType.fixstr)
            field.ArrayLength = entry.Size;
          else if (entry.Type == PARAM.CellType.fixstrW)
            field.ArrayLength = entry.Size / 2;
          else if (entry.Type == PARAM.CellType.b8 ||
                   entry.Type == PARAM.CellType.b16 ||
                   entry.Type == PARAM.CellType.b32) {
            field.Default = (bool) entry.Default ? 1f : 0.0f;
            field.BitSize = 1;
          }
          paramdef.Fields.Add(field);
        }
        return paramdef;
      }

      public static object ParseParamValue(
          PARAM.CellType type,
          string value,
          CultureInfo culture) {
        switch (type) {
          case PARAM.CellType.b8:
          case PARAM.CellType.b16:
          case PARAM.CellType.b32:
            return (object) bool.Parse(value);
          case PARAM.CellType.u8:
            return (object) byte.Parse(value);
          case PARAM.CellType.x8:
            return (object) Convert.ToByte(value, 16);
          case PARAM.CellType.s8:
            return (object) sbyte.Parse(value);
          case PARAM.CellType.u16:
            return (object) ushort.Parse(value);
          case PARAM.CellType.x16:
            return (object) Convert.ToUInt16(value, 16);
          case PARAM.CellType.s16:
            return (object) short.Parse(value);
          case PARAM.CellType.u32:
            return (object) uint.Parse(value);
          case PARAM.CellType.x32:
            return (object) Convert.ToUInt32(value, 16);
          case PARAM.CellType.s32:
            return (object) int.Parse(value);
          case PARAM.CellType.f32:
            return (object) float.Parse(value, (IFormatProvider) culture);
          case PARAM.CellType.fixstr:
          case PARAM.CellType.fixstrW:
            return (object) value;
          default:
            throw new InvalidCastException("Unparsable type: " + (object) type);
        }
      }

      public static object ParseParamValue(PARAM.CellType type, string value) {
        return PARAM.Layout.ParseParamValue(type,
                                            value,
                                            CultureInfo.InvariantCulture);
      }

      public static string ParamValueToString(
          PARAM.CellType type,
          object value,
          CultureInfo culture) {
        switch (type) {
          case PARAM.CellType.x8:
            return string.Format("0x{0:X2}", value);
          case PARAM.CellType.x16:
            return string.Format("0x{0:X4}", value);
          case PARAM.CellType.x32:
            return string.Format("0x{0:X8}", value);
          case PARAM.CellType.f32:
            return Convert.ToString(value, (IFormatProvider) culture);
          default:
            return value.ToString();
        }
      }

      public static string ParamValueToString(
          PARAM.CellType type,
          object value) {
        return PARAM.Layout.ParamValueToString(type,
                                               value,
                                               CultureInfo.InvariantCulture);
      }

      public class Entry {
        private int size;
        private object def;
        public string Description;
        public string Enum;

        public PARAM.CellType Type { get; set; }

        public string Name { get; set; }

        public int Size {
          get {
            if (this.IsVariableSize)
              return this.size;
            if (this.Type == PARAM.CellType.s8 ||
                this.Type == PARAM.CellType.u8 ||
                this.Type == PARAM.CellType.x8)
              return 1;
            if (this.Type == PARAM.CellType.s16 ||
                this.Type == PARAM.CellType.u16 ||
                this.Type == PARAM.CellType.x16)
              return 2;
            if (this.Type == PARAM.CellType.s32 ||
                this.Type == PARAM.CellType.u32 ||
                (this.Type == PARAM.CellType.x32 ||
                 this.Type == PARAM.CellType.f32))
              return 4;
            if (this.Type == PARAM.CellType.b8 ||
                this.Type == PARAM.CellType.b16 ||
                this.Type == PARAM.CellType.b32)
              return 0;
            throw new InvalidCastException(
                "Unknown type: " + (object) this.Type);
          }
          set {
            if (!this.IsVariableSize)
              throw new InvalidOperationException(
                  "Size may only be set for variable-width types: fixstr, fixstrW, and dummy8.");
            this.size = value;
          }
        }

        public object Default {
          get {
            return this.Type == PARAM.CellType.dummy8
                       ? (object) new byte[this.Size]
                       : this.def;
          }
          set {
            if (this.Type == PARAM.CellType.dummy8)
              throw new InvalidOperationException(
                  "Default may not be set for dummy8.");
            this.def = value;
          }
        }

        public bool IsVariableSize {
          get {
            return this.Type == PARAM.CellType.fixstr ||
                   this.Type == PARAM.CellType.fixstrW ||
                   this.Type == PARAM.CellType.dummy8;
          }
        }

        public Entry(PARAM.CellType type, string name, object def) {
          this.Type = type;
          this.Name = name;
          this.Default = def;
        }

        public Entry(PARAM.CellType type, string name, int size, object def) {
          this.Type = type;
          this.Name = name;
          this.Size = size;
          this.def = this.Type == PARAM.CellType.dummy8 ? (object) null : def;
        }

        internal Entry(XmlNode node) {
          this.Name = node.SelectSingleNode("name").InnerText;
          this.Type =
              (PARAM.CellType) System.Enum.Parse(typeof(PARAM.CellType),
                                                 node.SelectSingleNode("type")
                                                     .InnerText,
                                                 true);
          if (this.IsVariableSize)
            this.size =
                int.Parse(node.SelectSingleNode(nameof(size)).InnerText);
          if (this.Type != PARAM.CellType.dummy8)
            this.Default =
                PARAM.Layout.ParseParamValue(this.Type,
                                             node.SelectSingleNode("default")
                                                 .InnerText);
          this.Description = node.SelectSingleNode("description")?.InnerText;
          this.Enum = node.SelectSingleNode("enum")?.InnerText;
        }

        internal void Write(XmlWriter xw) {
          xw.WriteStartElement("entry");
          xw.WriteElementString("name", this.Name);
          xw.WriteElementString("type", this.Type.ToString());
          if (this.IsVariableSize)
            xw.WriteElementString("size", this.Size.ToString());
          if (this.Type != PARAM.CellType.dummy8)
            xw.WriteElementString("default",
                                  PARAM.Layout.ParamValueToString(
                                      this.Type,
                                      this.Default));
          if (this.Description != null)
            xw.WriteElementString("description", this.Description);
          if (this.Enum != null)
            xw.WriteElementString("enum", this.Enum);
          xw.WriteEndElement();
        }
      }
    }

    public enum CellType {
      dummy8,
      b8,
      b16,
      b32,
      u8,
      x8,
      s8,
      u16,
      x16,
      s16,
      u32,
      x32,
      s32,
      f32,
      fixstr,
      fixstrW,
    }

    public class Row {
      private BinaryReaderEx reader_;
      private PARAMDEF appliedParamdef_;
      private int expectedByteCount_;

      private bool streamable_;

      internal long DataOffset;

      public long ID { get; set; }

      public string Name { get; set; }

      public IReadOnlyList<PARAM.Cell> Cells {
        get {
          if (this.reader_ != null) {
            this.ReadCells_(this.reader_, this.appliedParamdef_);
            this.reader_ = null;
            this.appliedParamdef_ = null;
            this.streamable_ = false;
          }

          return this.cells_;
        }
      }

      private Cell[] cells_;

      public Row(long id, string name, PARAMDEF paramdef) {
        this.ID = id;
        this.Name = name;
        PARAM.Cell[] cellArray = new PARAM.Cell[paramdef.Fields.Count];
        for (int index = 0; index < paramdef.Fields.Count; ++index) {
          PARAMDEF.Field field = paramdef.Fields[index];
          object obj = ParamUtil.CastDefaultValue(field);
          cellArray[index] = new PARAM.Cell(field, obj);
        }
        this.cells_ = cellArray;
      }

      internal Row(BinaryReaderEx br, byte format2D, byte format2E) {
        long offset;
        if (((int) format2D & (int) sbyte.MaxValue) < 4) {
          this.ID = (long) br.ReadUInt32();
          this.DataOffset = (long) br.ReadUInt32();
          offset = (long) br.ReadUInt32();
        } else {
          this.ID = br.ReadInt64();
          this.DataOffset = br.ReadInt64();
          offset = br.ReadInt64();
        }
        if (offset == 0L)
          return;
        if (format2E < (byte) 7)
          this.Name = br.GetShiftJIS(offset);
        else
          this.Name = br.GetUTF16(offset);
      }

      internal void ScheduleUpdateCells(BinaryReaderEx br, PARAMDEF paramdef) {
        this.reader_ = br;
        this.appliedParamdef_ = paramdef;

        var byteCount = 0;
        int num1 = -1;
        PARAMDEF.DefType defType = PARAMDEF.DefType.u8;
        for (int index = 0; index < paramdef.Fields.Count; ++index) {
          PARAMDEF.Field field = paramdef.Fields[index];
          var found = false;

          PARAMDEF.DefType displayType = field.DisplayType;
          switch (displayType) {
            case PARAMDEF.DefType.s8:
              found = true;
              byteCount += 1;
              break;
            case PARAMDEF.DefType.s16:
              found = true;
              byteCount += 2;
              break;
            case PARAMDEF.DefType.s32:
            case PARAMDEF.DefType.f32:
              found = true;
              byteCount += 4;
              break;
            case PARAMDEF.DefType.fixstr:
              found = true;
              byteCount += field.ArrayLength;
              break;
            case PARAMDEF.DefType.fixstrW:
              found = true;
              byteCount += field.ArrayLength * 2;
              break;
            default:
              if (!ParamUtil.IsBitType(displayType))
                throw new NotImplementedException(
                    string.Format("Unsupported field type: {0}",
                                  (object)displayType));
              if (field.BitSize == -1) {
                switch (displayType) {
                  case PARAMDEF.DefType.u8:
                    found = true;
                    byteCount += 1;
                    break;
                  case PARAMDEF.DefType.u16:
                    found = true;
                    byteCount += 2;
                    break;
                  case PARAMDEF.DefType.u32:
                    found = true;
                    byteCount += 4;
                    break;
                  case PARAMDEF.DefType.dummy8:
                    found = true;
                    byteCount += field.ArrayLength;
                    break;
                }
              }
              break;
          }

          if (found) {
            num1 = -1;
          } else {
            PARAMDEF.DefType type = displayType == PARAMDEF.DefType.dummy8
                                        ? PARAMDEF.DefType.u8
                                        : displayType;
            int bitLimit = ParamUtil.GetBitLimit(type);
            if (field.BitSize == 0)
              throw new NotImplementedException("Bit size 0 is not supported.");
            if (field.BitSize > bitLimit)
              throw new InvalidDataException(
                  string.Format("Bit size {0} is too large to fit in type {1}.",
                                (object)field.BitSize,
                                (object)type));
            if (num1 == -1 ||
                type != defType ||
                num1 + field.BitSize > bitLimit) {
              num1 = 0;
              defType = type;
              switch (defType) {
                case PARAMDEF.DefType.u8:
                  byteCount += 1;
                  break;
                case PARAMDEF.DefType.u16:
                  byteCount += 2;
                  break;
                case PARAMDEF.DefType.u32:
                  byteCount += 4;
                  break;
              }
            }

            num1 += field.BitSize;
          }
        }

        this.expectedByteCount_ = byteCount;
        br.Position += byteCount;

        this.streamable_ = true;
      }

      private void ReadCells_(BinaryReaderEx br, PARAMDEF paramdef) {
        if (this.DataOffset == 0L)
          return;

        br.Position = this.DataOffset;
        PARAM.Cell[] cellArray = new PARAM.Cell[paramdef.Fields.Count];

        int num1 = -1;
        PARAMDEF.DefType defType = PARAMDEF.DefType.u8;
        uint num2 = 0;
        for (int index = 0; index < paramdef.Fields.Count; ++index) {
          PARAMDEF.Field field = paramdef.Fields[index];
          object obj = (object) null;
          PARAMDEF.DefType displayType = field.DisplayType;
          switch (displayType) {
            case PARAMDEF.DefType.s8:
              obj = (object) br.ReadSByte();
              break;
            case PARAMDEF.DefType.s16:
              obj = (object) br.ReadInt16();
              break;
            case PARAMDEF.DefType.s32:
              obj = (object) br.ReadInt32();
              break;
            case PARAMDEF.DefType.f32:
              obj = (object) br.ReadSingle();
              break;
            case PARAMDEF.DefType.fixstr:
              obj = (object) br.ReadFixStr(field.ArrayLength);
              break;
            case PARAMDEF.DefType.fixstrW:
              obj = (object) br.ReadFixStrW(field.ArrayLength * 2);
              break;
            default:
              if (!ParamUtil.IsBitType(displayType))
                throw new NotImplementedException(
                    string.Format("Unsupported field type: {0}",
                                  (object) displayType));
              if (field.BitSize == -1) {
                switch (displayType) {
                  case PARAMDEF.DefType.u8:
                    obj = (object) br.ReadByte();
                    break;
                  case PARAMDEF.DefType.u16:
                    obj = (object) br.ReadUInt16();
                    break;
                  case PARAMDEF.DefType.u32:
                    obj = (object) br.ReadUInt32();
                    break;
                  case PARAMDEF.DefType.dummy8:
                    obj = (object) br.ReadBytes(field.ArrayLength);
                    break;
                }
              } else
                break;
              break;
          }
          if (obj != null) {
            num1 = -1;
          } else {
            PARAMDEF.DefType type = displayType == PARAMDEF.DefType.dummy8
                                        ? PARAMDEF.DefType.u8
                                        : displayType;
            int bitLimit = ParamUtil.GetBitLimit(type);
            if (field.BitSize == 0)
              throw new NotImplementedException("Bit size 0 is not supported.");
            if (field.BitSize > bitLimit)
              throw new InvalidDataException(
                  string.Format("Bit size {0} is too large to fit in type {1}.",
                                (object) field.BitSize,
                                (object) type));
            if (num1 == -1 ||
                type != defType ||
                num1 + field.BitSize > bitLimit) {
              num1 = 0;
              defType = type;
              switch (defType) {
                case PARAMDEF.DefType.u8:
                  num2 = (uint) br.ReadByte();
                  break;
                case PARAMDEF.DefType.u16:
                  num2 = (uint) br.ReadUInt16();
                  break;
                case PARAMDEF.DefType.u32:
                  num2 = br.ReadUInt32();
                  break;
              }
            }
            uint num3 = num2 << 32 - field.BitSize - num1 >> 32 - field.BitSize;
            num1 += field.BitSize;
            if (defType == PARAMDEF.DefType.u8)
              obj = (object) (byte) num3;
            else if (defType == PARAMDEF.DefType.u16)
              obj = (object) (ushort) num3;
            else if (defType == PARAMDEF.DefType.u32)
              obj = (object) num3;
          }
          cellArray[index] = new PARAM.Cell(field, obj);
        }
        this.cells_ = cellArray;

        var actualReadByteCount = br.Position - this.DataOffset;
        if (actualReadByteCount != this.expectedByteCount_) {
          throw new Exception(
              $"Predicted wrong # of bytes. Expected {this.expectedByteCount_}, but got {actualReadByteCount}.");
        }
      }

      internal void WriteHeader(BinaryWriterEx bw, byte format2D, int i) {
        if (((int) format2D & (int) sbyte.MaxValue) < 4) {
          bw.WriteUInt32((uint) this.ID);
          bw.ReserveUInt32(string.Format("RowOffset{0}", (object) i));
          bw.ReserveUInt32(string.Format("NameOffset{0}", (object) i));
        } else {
          bw.WriteInt64(this.ID);
          bw.ReserveInt64(string.Format("RowOffset{0}", (object) i));
          bw.ReserveInt64(string.Format("NameOffset{0}", (object) i));
        }
      }

      internal void WriteCells(BinaryWriterEx bw, byte format2D, int index) {
        if (((int) format2D & (int) sbyte.MaxValue) < 4)
          bw.FillUInt32(string.Format("RowOffset{0}", (object) index),
                        (uint) bw.Position);
        else
          bw.FillInt64(string.Format("RowOffset{0}", (object) index),
                       bw.Position);

        if (this.streamable_) {
          if (this.DataOffset == 0L)
            return;

          this.reader_.Position = this.DataOffset;
          bw.WriteBytes(this.reader_.ReadBytes(this.expectedByteCount_));
          return;
        }

        int num1 = -1;
        PARAMDEF.DefType type = PARAMDEF.DefType.u8;
        uint num2 = 0;

        var cells = this.Cells;
        for (int index1 = 0; index1 < cells.Count; ++index1) {
          PARAM.Cell cell = cells[index1];
          object obj = cell.Value;
          PARAMDEF.Field def1 = cell.Def;
          PARAMDEF.DefType displayType1 = def1.DisplayType;

          switch (displayType1) {
            case PARAMDEF.DefType.s8:
              bw.WriteSByte((sbyte) obj);
              break;
            case PARAMDEF.DefType.s16:
              bw.WriteInt16((short) obj);
              break;
            case PARAMDEF.DefType.s32:
              bw.WriteInt32((int) obj);
              break;
            case PARAMDEF.DefType.f32:
              bw.WriteSingle((float) obj);
              break;
            case PARAMDEF.DefType.fixstr:
              bw.WriteFixStr((string) obj, def1.ArrayLength, (byte) 0);
              break;
            case PARAMDEF.DefType.fixstrW:
              bw.WriteFixStrW((string) obj, def1.ArrayLength * 2, (byte) 0);
              break;
            default:
              if (!ParamUtil.IsBitType(displayType1))
                throw new NotImplementedException(
                    string.Format("Unsupported field type: {0}",
                                  (object) displayType1));
              if (def1.BitSize == -1) {
                switch (displayType1) {
                  case PARAMDEF.DefType.u8:
                    bw.WriteByte((byte) obj);
                    continue;
                  case PARAMDEF.DefType.u16:
                    bw.WriteUInt16((ushort) obj);
                    continue;
                  case PARAMDEF.DefType.u32:
                    bw.WriteUInt32((uint) obj);
                    continue;
                  case PARAMDEF.DefType.dummy8:
                    bw.WriteBytes((byte[]) obj);
                    continue;
                  default:
                    continue;
                }
              } else {
                if (num1 == -1) {
                  num1 = 0;
                  type = displayType1 == PARAMDEF.DefType.dummy8
                             ? PARAMDEF.DefType.u8
                             : displayType1;
                  num2 = 0U;
                }
                uint num3 = 0;
                switch (type) {
                  case PARAMDEF.DefType.u8:
                    num3 = (uint) (byte) obj;
                    break;
                  case PARAMDEF.DefType.u16:
                    num3 = (uint) (ushort) obj;
                    break;
                  case PARAMDEF.DefType.u32:
                    num3 = (uint) obj;
                    break;
                }
                uint num4 =
                    num3 << 32 - def1.BitSize >> 32 - def1.BitSize - num1;
                num2 |= num4;
                num1 += def1.BitSize;
                bool flag = false;
                if (index1 == cells.Count - 1) {
                  flag = true;
                } else {
                  PARAMDEF.Field def2 = cells[index1 + 1].Def;
                  PARAMDEF.DefType displayType2 = def2.DisplayType;
                  int bitLimit = ParamUtil.GetBitLimit(type);
                  if (!ParamUtil.IsBitType(displayType2) ||
                      def2.BitSize == -1 ||
                      num1 + def2.BitSize > bitLimit ||
                      (displayType2 == PARAMDEF.DefType.dummy8
                           ? PARAMDEF.DefType.u8
                           : displayType2) !=
                      type)
                    flag = true;
                }
                if (flag) {
                  num1 = -1;
                  switch (type) {
                    case PARAMDEF.DefType.u8:
                      bw.WriteByte((byte) num2);
                      continue;
                    case PARAMDEF.DefType.u16:
                      bw.WriteUInt16((ushort) num2);
                      continue;
                    case PARAMDEF.DefType.u32:
                      bw.WriteUInt32(num2);
                      continue;
                    default:
                      continue;
                  }
                } else
                  break;
              }
          }
        }
      }

      internal void WriteName(
          BinaryWriterEx bw,
          byte format2D,
          byte format2E,
          int i) {
        long num = 0;
        if (this.Name != null) {
          num = bw.Position;
          if (format2E < (byte) 7)
            bw.WriteShiftJIS(this.Name, true);
          else
            bw.WriteUTF16(this.Name, true);
        }
        if (((int) format2D & (int) sbyte.MaxValue) < 4)
          bw.FillUInt32(string.Format("NameOffset{0}", (object) i), (uint) num);
        else
          bw.FillInt64(string.Format("NameOffset{0}", (object) i), num);
      }

      public override string ToString() {
        return string.Format("{0} {1}", (object) this.ID, (object) this.Name);
      }

      public PARAM.Cell this[string name] {
        get {
          return this.Cells.First<PARAM.Cell>(
              (Func<PARAM.Cell, bool>)
              (cell => cell.Def.InternalName == name));
        }
      }
    }

    public class Cell {
      public PARAMDEF.Field Def { get; }

      public object Value { get; set; }

      internal Cell(PARAMDEF.Field def, object value) {
        this.Def = def;
        this.Value = value;
      }

      public override string ToString() {
        return string.Format("{0} {1} = {2}",
                             (object) this.Def.DisplayType,
                             (object) this.Def.InternalName,
                             this.Value);
      }
    }
  }
}