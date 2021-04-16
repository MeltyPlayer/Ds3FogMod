// Decompiled with JetBrains decompiler
// Type: SoulsFormats.LUAINFO
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class LUAINFO : SoulsFile<LUAINFO> {
    public bool BigEndian { get; set; }

    public bool LongFormat { get; set; }

    public List<LUAINFO.Goal> Goals { get; set; }

    public LUAINFO()
        : this(false, false) {}

    public LUAINFO(bool bigEndian, bool longFormat) {
      this.BigEndian = bigEndian;
      this.LongFormat = longFormat;
      this.Goals = new List<LUAINFO.Goal>();
    }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "LUAI";
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertASCII("LUAI");
      this.BigEndian = br.AssertInt32(1, 16777216) == 16777216;
      br.BigEndian = this.BigEndian;
      int capacity = br.ReadInt32();
      br.AssertInt32(new int[1]);
      if (capacity == 0)
        throw new NotSupportedException(
            "LUAINFO format cannot be detected on files with 0 goals.");
      if (capacity >= 2)
        this.LongFormat = br.GetInt32(36L) == 0;
      else if (br.GetInt32(24L) == 16 + 24 * capacity) {
        this.LongFormat = true;
      } else {
        if (br.GetInt32(20L) != 16 + 16 * capacity)
          throw new NotSupportedException("Could not detect LUAINFO format.");
        this.LongFormat = false;
      }
      this.Goals = new List<LUAINFO.Goal>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Goals.Add(new LUAINFO.Goal(br, this.LongFormat));
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = this.BigEndian;
      bw.WriteASCII("LUAI", false);
      bw.WriteInt32(1);
      bw.WriteInt32(this.Goals.Count);
      bw.WriteInt32(0);
      for (int index = 0; index < this.Goals.Count; ++index)
        this.Goals[index].Write(bw, this.LongFormat, index);
      for (int index = 0; index < this.Goals.Count; ++index)
        this.Goals[index].WriteStrings(bw, this.LongFormat, index);
      bw.Pad(16);
    }

    public class Goal {
      public int ID { get; set; }

      public string Name { get; set; }

      public bool BattleInterrupt { get; set; }

      public bool LogicInterrupt { get; set; }

      public string LogicInterruptName { get; set; }

      public Goal(
          int id,
          string name,
          bool battleInterrupt,
          bool logicInterrupt,
          string logicInterruptName = null) {
        this.ID = id;
        this.Name = name;
        this.BattleInterrupt = battleInterrupt;
        this.LogicInterrupt = logicInterrupt;
        this.LogicInterruptName = logicInterruptName;
      }

      internal Goal(BinaryReaderEx br, bool longFormat) {
        this.ID = br.ReadInt32();
        if (longFormat) {
          this.BattleInterrupt = br.ReadBoolean();
          this.LogicInterrupt = br.ReadBoolean();
          int num = (int) br.AssertInt16(new short[1]);
          long offset1 = br.ReadInt64();
          long offset2 = br.ReadInt64();
          this.Name = br.GetUTF16(offset1);
          if (offset2 == 0L)
            this.LogicInterruptName = (string) null;
          else
            this.LogicInterruptName = br.GetUTF16(offset2);
        } else {
          uint num1 = br.ReadUInt32();
          uint num2 = br.ReadUInt32();
          this.BattleInterrupt = br.ReadBoolean();
          this.LogicInterrupt = br.ReadBoolean();
          int num3 = (int) br.AssertInt16(new short[1]);
          this.Name = br.GetShiftJIS((long) num1);
          if (num2 == 0U)
            this.LogicInterruptName = (string) null;
          else
            this.LogicInterruptName = br.GetShiftJIS((long) num2);
        }
      }

      internal void Write(BinaryWriterEx bw, bool longFormat, int index) {
        bw.WriteInt32(this.ID);
        if (longFormat) {
          bw.WriteBoolean(this.BattleInterrupt);
          bw.WriteBoolean(this.LogicInterrupt);
          bw.WriteInt16((short) 0);
          bw.ReserveInt64(string.Format("NameOffset{0}", (object) index));
          bw.ReserveInt64(
              string.Format("LogicInterruptNameOffset{0}", (object) index));
        } else {
          bw.ReserveUInt32(string.Format("NameOffset{0}", (object) index));
          bw.ReserveUInt32(
              string.Format("LogicInterruptNameOffset{0}", (object) index));
          bw.WriteBoolean(this.BattleInterrupt);
          bw.WriteBoolean(this.LogicInterrupt);
          bw.WriteInt16((short) 0);
        }
      }

      internal void WriteStrings(
          BinaryWriterEx bw,
          bool longFormat,
          int index) {
        if (longFormat) {
          bw.FillInt64(string.Format("NameOffset{0}", (object) index),
                       bw.Position);
          bw.WriteUTF16(this.Name, true);
          if (this.LogicInterruptName == null) {
            bw.FillInt64(
                string.Format("LogicInterruptNameOffset{0}", (object) index),
                0L);
          } else {
            bw.FillInt64(
                string.Format("LogicInterruptNameOffset{0}", (object) index),
                bw.Position);
            bw.WriteUTF16(this.LogicInterruptName, true);
          }
        } else {
          bw.FillUInt32(string.Format("NameOffset{0}", (object) index),
                        (uint) bw.Position);
          bw.WriteShiftJIS(this.Name, true);
          if (this.LogicInterruptName == null) {
            bw.FillUInt32(
                string.Format("LogicInterruptNameOffset{0}", (object) index),
                0U);
          } else {
            bw.FillUInt32(
                string.Format("LogicInterruptNameOffset{0}", (object) index),
                (uint) bw.Position);
            bw.WriteShiftJIS(this.LogicInterruptName, true);
          }
        }
      }
    }
  }
}