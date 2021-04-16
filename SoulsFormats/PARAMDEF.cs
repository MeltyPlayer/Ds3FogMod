// Decompiled with JetBrains decompiler
// Type: SoulsFormats.PARAMDEF
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsFormats.XmlExtensions;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace SoulsFormats {
  [ComVisible(true)]
  public class PARAMDEF : SoulsFile<PARAMDEF> {
    private const int XML_VERSION = 0;

    public short Unk06 { get; set; }

    public string ParamType { get; set; }

    public bool BigEndian { get; set; }

    public bool Unicode { get; set; }

    public short Version { get; set; }

    public List<PARAMDEF.Field> Fields { get; set; }

    public PARAMDEF() {
      this.ParamType = "AI_STANDARD_INFO_BANK";
      this.Version = (short) 104;
      this.Fields = new List<PARAMDEF.Field>();
    }

    protected override void Read(BinaryReaderEx br) {
      this.BigEndian = br.GetSByte(44L) == (sbyte) -1;
      br.BigEndian = this.BigEndian;
      br.ReadInt32();
      short num1 = br.AssertInt16((short) 48, (short) byte.MaxValue);
      this.Unk06 = br.ReadInt16();
      short num2 = br.ReadInt16();
      short num3 = br.AssertInt16((short) 108,
                                  (short) 140,
                                  (short) 172,
                                  (short) 176,
                                  (short) 208);
      this.ParamType = br.ReadFixStr(32);
      int num4 = (int) br.ReadByte();
      this.Unicode = br.ReadBoolean();
      this.Version = br.AssertInt16((short) 101,
                                    (short) 102,
                                    (short) 103,
                                    (short) 104,
                                    (short) 201);
      if (this.Version >= (short) 201)
        br.AssertInt64(56L);
      if ((this.Version >= (short) 200 || num1 != (short) 48) &&
          (this.Version < (short) 200 || num1 != (short) byte.MaxValue))
        throw new InvalidDataException(
            string.Format("Unexpected unk04 0x{0:X} for version {1}.",
                          (object) num1,
                          (object) this.Version));
      if ((this.Version != (short) 101 || num3 != (short) 140) &&
          (this.Version != (short) 102 || num3 != (short) 172) &&
          ((this.Version != (short) 103 || num3 != (short) 108) &&
           (this.Version != (short) 104 || num3 != (short) 176)) &&
          (this.Version != (short) 201 || num3 != (short) 208))
        throw new InvalidDataException(
            string.Format("Unexpected field size 0x{0:X} for version {1}.",
                          (object) num3,
                          (object) this.Version));
      this.Fields = new List<PARAMDEF.Field>((int) num2);
      for (int index = 0; index < (int) num2; ++index)
        this.Fields.Add(new PARAMDEF.Field(br, this));
    }

    public override bool Validate(out Exception ex) {
      if (this.Version != (short) 101 &&
          this.Version != (short) 102 &&
          (this.Version != (short) 103 && this.Version != (short) 104) &&
          this.Version != (short) 201) {
        ex = (Exception) new InvalidDataException(
            string.Format("Unknown version: {0}", (object) this.Version));
        return false;
      }
      if (!SoulsFile<PARAMDEF>.ValidateNull((object) this.ParamType,
                                            "ParamType may not be null.",
                                            out ex) ||
          !SoulsFile<PARAMDEF>.ValidateNull((object) this.Fields,
                                            "Fields may not be null.",
                                            out ex))
        return false;
      for (int index = 0; index < this.Fields.Count; ++index) {
        PARAMDEF.Field field = this.Fields[index];
        string str =
            string.Format("{0}[{1}]", (object) "Fields", (object) index);
        if (
            !SoulsFile<PARAMDEF>.ValidateNull((object) field,
                                              str + ": Field may not be null.",
                                              out ex) ||
            !SoulsFile<PARAMDEF>.ValidateNull((object) field.DisplayName,
                                              str +
                                              ": DisplayName may not be null.",
                                              out ex) ||
            (!SoulsFile<PARAMDEF>.ValidateNull((object) field.DisplayFormat,
                                               str +
                                               ": DisplayFormat may not be null.",
                                               out ex) ||
             !SoulsFile<PARAMDEF>.ValidateNull((object) field.InternalType,
                                               str +
                                               ": InternalType may not be null.",
                                               out ex)) ||
            this.Version >= (short) 102 &&
            !SoulsFile<PARAMDEF>.ValidateNull((object) field.InternalName,
                                              string.Format(
                                                  "{0}: {1} may not be null on version {2}.",
                                                  (object) str,
                                                  (object) "InternalName",
                                                  (object) this.Version),
                                              out ex))
          return false;
      }
      ex = (Exception) null;
      return true;
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = this.BigEndian;
      bw.ReserveInt32("FileSize");
      bw.WriteInt16(this.Version >= (short) 201
                        ? (short) byte.MaxValue
                        : (short) 48);
      bw.WriteInt16(this.Unk06);
      bw.WriteInt16((short) this.Fields.Count);
      if (this.Version == (short) 101)
        bw.WriteInt16((short) 140);
      else if (this.Version == (short) 102)
        bw.WriteInt16((short) 172);
      else if (this.Version == (short) 103)
        bw.WriteInt16((short) 108);
      else if (this.Version == (short) 104)
        bw.WriteInt16((short) 176);
      else if (this.Version == (short) 201)
        bw.WriteInt16((short) 208);
      bw.WriteFixStr(this.ParamType,
                     32,
                     this.Version >= (short) 201 ? (byte) 0 : (byte) 32);
      bw.WriteSByte(this.BigEndian ? (sbyte) -1 : (sbyte) 0);
      bw.WriteBoolean(this.Unicode);
      bw.WriteInt16(this.Version);
      if (this.Version >= (short) 201)
        bw.WriteInt64(56L);
      for (int index = 0; index < this.Fields.Count; ++index)
        this.Fields[index].Write(bw, this, index);
      long position = bw.Position;
      for (int index = 0; index < this.Fields.Count; ++index)
        this.Fields[index].WriteDescription(bw, this, index);
      if (this.Version >= (short) 104) {
        long num = bw.Position - position;
        if (num % 16L != 0L)
          bw.WritePattern((int) (16L - num % 16L), (byte) 0);
      } else
        bw.Pad(16);
      bw.FillInt32("FileSize", (int) bw.Position);
    }

    public int GetRowSize() {
      int num = 0;
      for (int index = 0; index < this.Fields.Count; ++index) {
        PARAMDEF.Field field1 = this.Fields[index];
        PARAMDEF.DefType displayType1 = field1.DisplayType;
        if (ParamUtil.IsArrayType(displayType1))
          num += ParamUtil.GetValueSize(displayType1) * field1.ArrayLength;
        else
          num += ParamUtil.GetValueSize(displayType1);
        if (ParamUtil.IsBitType(displayType1) && field1.BitSize != -1) {
          int bitSize = field1.BitSize;
          PARAMDEF.DefType type = displayType1 == PARAMDEF.DefType.dummy8
                                      ? PARAMDEF.DefType.u8
                                      : displayType1;
          int bitLimit = ParamUtil.GetBitLimit(type);
          for (; index < this.Fields.Count - 1; ++index) {
            PARAMDEF.Field field2 = this.Fields[index + 1];
            PARAMDEF.DefType displayType2 = field2.DisplayType;
            if (ParamUtil.IsBitType(displayType2) &&
                field2.BitSize != -1 &&
                bitSize + field2.BitSize <= bitLimit &&
                (displayType2 == PARAMDEF.DefType.dummy8
                     ? PARAMDEF.DefType.u8
                     : displayType2) ==
                type)
              bitSize += field2.BitSize;
            else
              break;
          }
        }
      }
      return num;
    }

    public static PARAMDEF XmlDeserialize(string path) {
      XmlDocument xml = new XmlDocument();
      xml.Load(path);
      return new PARAMDEF(xml);
    }

    private PARAMDEF(XmlDocument xml) {
      XmlNode node = xml.SelectSingleNode(nameof(PARAMDEF));
      int num = int.Parse(node.Attributes["XmlVersion"].InnerText);
      if (num != 0)
        throw new InvalidDataException(
            string.Format(
                "Mismatched XML version; current version: {0}, file version: {1}",
                (object) 0,
                (object) num));
      this.ParamType = node.SelectSingleNode(nameof(ParamType)).InnerText;
      this.Unk06 = node.ReadInt16(nameof(Unk06));
      this.BigEndian = node.ReadBoolean(nameof(BigEndian));
      this.Unicode = node.ReadBoolean(nameof(Unicode));
      this.Version = node.ReadInt16(nameof(Version));
      this.Fields = new List<PARAMDEF.Field>();
      foreach (XmlNode selectNode in node.SelectNodes("Fields/Field"))
        this.Fields.Add(new PARAMDEF.Field(selectNode));
    }

    public void XmlSerialize(string path) {
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      XmlWriterSettings settings = new XmlWriterSettings() {
          Indent = true
      };
      using (XmlWriter xw = XmlWriter.Create(path, settings))
        this.XmlSerialize(xw);
    }

    private void XmlSerialize(XmlWriter xw) {
      xw.WriteStartDocument();
      xw.WriteStartElement(nameof(PARAMDEF));
      xw.WriteAttributeString("XmlVersion", 0.ToString());
      xw.WriteElementString("ParamType", this.ParamType);
      xw.WriteElementString("Unk06", this.Unk06.ToString());
      xw.WriteElementString("BigEndian", this.BigEndian.ToString());
      xw.WriteElementString("Unicode", this.Unicode.ToString());
      xw.WriteElementString("Version", this.Version.ToString());
      xw.WriteStartElement("Fields");
      foreach (PARAMDEF.Field field in this.Fields) {
        xw.WriteStartElement("Field");
        XmlWriter xw1 = xw;
        field.XmlSerialize(xw1);
        xw.WriteEndElement();
      }
      xw.WriteEndElement();
      xw.WriteEndElement();
    }

    [System.Flags]
    public enum EditFlags {
      None = 0,
      Wrap = 1,
      Lock = 4,
    }

    public enum DefType {
      s8,
      u8,
      s16,
      u16,
      s32,
      u32,
      f32,
      dummy8,
      fixstr,
      fixstrW,
    }

    public class Field {
      private static readonly Regex arrayLengthRx =
          new Regex("^(?<name>.+?)\\s*\\[\\s*(?<length>\\d+)\\s*\\]\\s*$");

      private static readonly Regex bitSizeRx =
          new Regex("^(?<name>.+?)\\s*\\:\\s*(?<size>\\d+)\\s*$");

      private static readonly Regex defOuterRx =
          new Regex(
              "^(?<type>\\S+)\\s+(?<name>.+?)(?:\\s*=\\s*(?<default>\\S+))?$");

      private static readonly Regex defBitRx =
          new Regex("^(?<name>.+?)\\s*:\\s*(?<size>\\d+)$");

      private static readonly Regex defArrayRx =
          new Regex("^(?<name>.+?)\\s*\\[\\s*(?<length>\\d+)\\]$");

      public string DisplayName { get; set; }

      public PARAMDEF.DefType DisplayType { get; set; }

      public string DisplayFormat { get; set; }

      public float Default { get; set; }

      public float Minimum { get; set; }

      public float Maximum { get; set; }

      public float Increment { get; set; }

      public PARAMDEF.EditFlags EditFlags { get; set; }

      public int ArrayLength { get; set; }

      public string Description { get; set; }

      public string InternalType { get; set; }

      public string InternalName { get; set; }

      public int BitSize { get; set; }

      public int SortID { get; set; }

      public Field()
          : this(PARAMDEF.DefType.f32, "placeholder") {}

      public Field(PARAMDEF.DefType displayType, string internalName) {
        this.DisplayName = internalName;
        this.DisplayType = displayType;
        this.DisplayFormat = ParamUtil.GetDefaultFormat(this.DisplayType);
        this.Minimum = ParamUtil.GetDefaultMinimum(this.DisplayType);
        this.Maximum = ParamUtil.GetDefaultMaximum(this.DisplayType);
        this.Increment = ParamUtil.GetDefaultIncrement(this.DisplayType);
        this.EditFlags = ParamUtil.GetDefaultEditFlags(this.DisplayType);
        this.ArrayLength = 1;
        this.InternalType = this.DisplayType.ToString();
        this.InternalName = internalName;
        this.BitSize = -1;
      }

      internal Field(BinaryReaderEx br, PARAMDEF def) {
        this.DisplayName =
            !def.Unicode ? br.ReadFixStr(64) : br.ReadFixStrW(64);
        this.DisplayType =
            (PARAMDEF.DefType) System.Enum.Parse(typeof(PARAMDEF.DefType),
                                                 br.ReadFixStr(8));
        this.DisplayFormat = br.ReadFixStr(8);
        this.Default = br.ReadSingle();
        this.Minimum = br.ReadSingle();
        this.Maximum = br.ReadSingle();
        this.Increment = br.ReadSingle();
        this.EditFlags = (PARAMDEF.EditFlags) br.ReadInt32();
        int num = br.ReadInt32();
        if (!ParamUtil.IsArrayType(this.DisplayType) &&
            num != ParamUtil.GetValueSize(this.DisplayType) ||
            ParamUtil.IsArrayType(this.DisplayType) &&
            num % ParamUtil.GetValueSize(this.DisplayType) != 0)
          throw new InvalidDataException(
              string.Format("Unexpected byte count {0} for type {1}.",
                            (object) num,
                            (object) this.DisplayType));
        this.ArrayLength = num / ParamUtil.GetValueSize(this.DisplayType);
        long offset = def.Version < (short) 201
                          ? (long) br.ReadInt32()
                          : br.ReadInt64();
        this.InternalType = br.ReadFixStr(32);
        this.BitSize = -1;
        if (def.Version >= (short) 102) {
          this.InternalName = br.ReadFixStr(32).Trim();
          Match match1 = PARAMDEF.Field.bitSizeRx.Match(this.InternalName);
          if (match1.Success) {
            this.InternalName = match1.Groups["name"].Value;
            this.BitSize = int.Parse(match1.Groups["size"].Value);
          }
          if (ParamUtil.IsArrayType(this.DisplayType)) {
            Match match2 =
                PARAMDEF.Field.arrayLengthRx.Match(this.InternalName);
            if ((match2.Success
                     ? int.Parse(match2.Groups["length"].Value)
                     : 1) !=
                this.ArrayLength)
              throw new InvalidDataException(
                  string.Format(
                      "Mismatched array length in {0} with byte count {1}.",
                      (object) this.InternalName,
                      (object) num));
            if (match2.Success)
              this.InternalName = match2.Groups["name"].Value;
          }
        }
        if (def.Version >= (short) 104)
          this.SortID = br.ReadInt32();
        if (def.Version >= (short) 201)
          br.AssertPattern(28, (byte) 0);
        if (offset == 0L)
          return;
        if (def.Unicode)
          this.Description = br.GetUTF16(offset);
        else
          this.Description = br.GetShiftJIS(offset);
      }

      internal void Write(BinaryWriterEx bw, PARAMDEF def, int index) {
        if (def.Unicode)
          bw.WriteFixStrW(this.DisplayName,
                          64,
                          def.Version >= (short) 104 ? (byte) 0 : (byte) 32);
        else
          bw.WriteFixStr(this.DisplayName,
                         64,
                         def.Version >= (short) 104 ? (byte) 0 : (byte) 32);
        byte padding = def.Version >= (short) 201 ? (byte) 0 : (byte) 32;
        bw.WriteFixStr(this.DisplayType.ToString(), 8, padding);
        bw.WriteFixStr(this.DisplayFormat, 8, padding);
        bw.WriteSingle(this.Default);
        bw.WriteSingle(this.Minimum);
        bw.WriteSingle(this.Maximum);
        bw.WriteSingle(this.Increment);
        bw.WriteInt32((int) this.EditFlags);
        bw.WriteInt32(ParamUtil.GetValueSize(this.DisplayType) *
                      (ParamUtil.IsArrayType(this.DisplayType)
                           ? this.ArrayLength
                           : 1));
        if (def.Version >= (short) 201)
          bw.ReserveInt64(
              string.Format("DescriptionOffset{0}", (object) index));
        else
          bw.ReserveInt32(
              string.Format("DescriptionOffset{0}", (object) index));
        bw.WriteFixStr(this.InternalType, 32, padding);
        if (def.Version >= (short) 102) {
          string text = this.InternalName;
          if (this.BitSize != -1)
            text = string.Format("{0}:{1}",
                                 (object) text,
                                 (object) this.BitSize);
          else if (ParamUtil.IsArrayType(this.DisplayType))
            text = string.Format("{0}[{1}]",
                                 (object) text,
                                 (object) this.ArrayLength);
          bw.WriteFixStr(text, 32, padding);
        }
        if (def.Version >= (short) 104)
          bw.WriteInt32(this.SortID);
        if (def.Version < (short) 201)
          return;
        bw.WritePattern(28, (byte) 0);
      }

      internal void WriteDescription(
          BinaryWriterEx bw,
          PARAMDEF def,
          int index) {
        long num = 0;
        if (this.Description != null) {
          num = bw.Position;
          if (def.Unicode)
            bw.WriteUTF16(this.Description, true);
          else
            bw.WriteShiftJIS(this.Description, true);
        }
        if (def.Version >= (short) 201)
          bw.FillInt64(string.Format("DescriptionOffset{0}", (object) index),
                       num);
        else
          bw.FillInt32(string.Format("DescriptionOffset{0}", (object) index),
                       (int) num);
      }

      public override string ToString() {
        if (ParamUtil.IsBitType(this.DisplayType) && this.BitSize != -1)
          return string.Format("{0} {1}:{2}",
                               (object) this.DisplayType,
                               (object) this.InternalName,
                               (object) this.BitSize);
        return ParamUtil.IsArrayType(this.DisplayType)
                   ? string.Format("{0} {1}[{2}]",
                                   (object) this.DisplayType,
                                   (object) this.InternalName,
                                   (object) this.ArrayLength)
                   : string.Format("{0} {1}",
                                   (object) this.DisplayType,
                                   (object) this.InternalName);
      }

      internal Field(XmlNode node) {
        string innerText = node.Attributes["Def"].InnerText;
        Match match1 = PARAMDEF.Field.defOuterRx.Match(innerText);
        this.DisplayType =
            (PARAMDEF.DefType) System.Enum.Parse(typeof(PARAMDEF.DefType),
                                                 match1.Groups["type"]
                                                       .Value.Trim());
        if (match1.Groups["default"].Success)
          this.Default = float.Parse(match1.Groups["default"].Value,
                                     (IFormatProvider) CultureInfo
                                         .InvariantCulture);
        string input = match1.Groups["name"].Value.Trim();
        Match match2 = PARAMDEF.Field.defBitRx.Match(input);
        Match match3 = PARAMDEF.Field.defArrayRx.Match(input);
        this.BitSize = -1;
        this.ArrayLength = 1;
        if (ParamUtil.IsBitType(this.DisplayType) && match2.Success) {
          this.BitSize = int.Parse(match2.Groups["size"].Value);
          input = match2.Groups["name"].Value;
        } else if (ParamUtil.IsArrayType(this.DisplayType)) {
          this.ArrayLength = int.Parse(match3.Groups["length"].Value);
          input = match3.Groups["name"].Value;
        }
        this.InternalName = input;
        this.DisplayName =
            node.ReadStringOrDefault(nameof(DisplayName), this.InternalName);
        this.InternalType =
            node.ReadStringOrDefault("Enum", this.DisplayType.ToString());
        this.Description =
            node.ReadStringOrDefault(nameof(Description), (string) null);
        this.DisplayFormat = node.ReadStringOrDefault(
            nameof(DisplayFormat),
            ParamUtil.GetDefaultFormat(this.DisplayType));
        this.EditFlags = (PARAMDEF.EditFlags) System.Enum.Parse(
            typeof(PARAMDEF.
                EditFlags),
            node
                .ReadStringOrDefault(
                    nameof(
                        EditFlags),
                    ParamUtil
                        .GetDefaultEditFlags(
                            this
                                .DisplayType)
                        .ToString()));
        this.Minimum = node.ReadSingleOrDefault(
            nameof(Minimum),
            ParamUtil.GetDefaultMinimum(this.DisplayType),
            (IFormatProvider) CultureInfo.InvariantCulture);
        this.Maximum = node.ReadSingleOrDefault(
            nameof(Maximum),
            ParamUtil.GetDefaultMaximum(this.DisplayType),
            (IFormatProvider) CultureInfo.InvariantCulture);
        this.Increment = node.ReadSingleOrDefault(
            nameof(Increment),
            ParamUtil.GetDefaultIncrement(this.DisplayType),
            (IFormatProvider) CultureInfo.InvariantCulture);
        this.SortID = node.ReadInt32OrDefault(nameof(SortID), 0);
      }

      internal void XmlSerialize(XmlWriter xw) {
        string str1 = string.Format("{0} {1}",
                                    (object) this.DisplayType,
                                    (object) this.InternalName);
        if (ParamUtil.IsBitType(this.DisplayType) && this.BitSize != -1)
          str1 += string.Format(":{0}", (object) this.BitSize);
        else if (ParamUtil.IsArrayType(this.DisplayType))
          str1 += string.Format("[{0}]", (object) this.ArrayLength);
        if ((double) this.Default != 0.0)
          str1 = str1 +
                 " = " +
                 this.Default.ToString("R",
                                       (IFormatProvider) CultureInfo
                                           .InvariantCulture);
        xw.WriteAttributeString("Def", str1);
        xw.WriteDefaultElement<string>("DisplayName",
                                       this.DisplayName,
                                       this.InternalName);
        xw.WriteDefaultElement<string>("Enum",
                                       this.InternalType,
                                       this.DisplayType.ToString());
        xw.WriteDefaultElement<string>("Description",
                                       this.Description,
                                       (string) null);
        xw.WriteDefaultElement<string>("DisplayFormat",
                                       this.DisplayFormat,
                                       ParamUtil.GetDefaultFormat(
                                           this.DisplayType));
        XmlWriter xw1 = xw;
        PARAMDEF.EditFlags editFlags = this.EditFlags;
        string str2 = editFlags.ToString();
        editFlags = ParamUtil.GetDefaultEditFlags(this.DisplayType);
        string defaultValue = editFlags.ToString();
        xw1.WriteDefaultElement<string>("EditFlags", str2, defaultValue);
        xw.WriteDefaultElement("Minimum",
                               this.Minimum,
                               ParamUtil.GetDefaultMinimum(this.DisplayType),
                               "R",
                               (IFormatProvider) CultureInfo.InvariantCulture);
        xw.WriteDefaultElement("Maximum",
                               this.Maximum,
                               ParamUtil.GetDefaultMaximum(this.DisplayType),
                               "R",
                               (IFormatProvider) CultureInfo.InvariantCulture);
        xw.WriteDefaultElement("Increment",
                               this.Increment,
                               ParamUtil.GetDefaultIncrement(this.DisplayType),
                               "R",
                               (IFormatProvider) CultureInfo.InvariantCulture);
        xw.WriteDefaultElement<int>("SortID", this.SortID, 0);
      }
    }
  }
}