// Decompiled with JetBrains decompiler
// Type: SoulsFormats.FMG
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class FMG : SoulsFile<FMG> {
    public List<FMG.Entry> Entries;
    public FMG.FMGVersion Version;
    public bool BigEndian;

    public FMG() {
      this.Entries = new List<FMG.Entry>();
      this.Version = FMG.FMGVersion.DarkSouls1;
      this.BigEndian = false;
    }

    public FMG(FMG.FMGVersion version) {
      this.Entries = new List<FMG.Entry>();
      this.Version = version;
      this.BigEndian = this.Version == FMG.FMGVersion.DemonsSouls;
    }

    protected override void Read(BinaryReaderEx br) {
      int num1 = (int) br.AssertByte(new byte[1]);
      this.BigEndian = br.ReadBoolean();
      this.Version = br.ReadEnum8<FMG.FMGVersion>();
      int num2 = (int) br.AssertByte(new byte[1]);
      br.BigEndian = this.BigEndian;
      bool flag = this.Version == FMG.FMGVersion.DarkSouls3;
      br.ReadInt32();
      int num3 = (int) br.AssertByte((byte) 1);
      int num4 = (int) br.AssertByte(this.Version == FMG.FMGVersion.DemonsSouls
                                         ? byte.MaxValue
                                         : (byte) 0);
      int num5 = (int) br.AssertByte(new byte[1]);
      int num6 = (int) br.AssertByte(new byte[1]);
      int capacity = br.ReadInt32();
      br.ReadInt32();
      if (flag)
        br.AssertInt32((int) byte.MaxValue);
      long num7 = !flag ? (long) br.ReadInt32() : br.ReadInt64();
      if (flag)
        br.AssertInt64(new long[1]);
      else
        br.AssertInt32(new int[1]);
      this.Entries = new List<FMG.Entry>(capacity);
      for (int index1 = 0; index1 < capacity; ++index1) {
        int num8 = br.ReadInt32();
        int num9 = br.ReadInt32();
        int num10 = br.ReadInt32();
        if (flag)
          br.AssertInt32(new int[1]);
        br.StepIn(num7 + (long) (num8 * (flag ? 8 : 4)));
        for (int index2 = 0; index2 < num10 - num9 + 1; ++index2) {
          long offset = !flag ? (long) br.ReadInt32() : br.ReadInt64();
          this.Entries.Add(new FMG.Entry(num9 + index2,
                                         offset != 0L
                                             ? br.GetUTF16(offset)
                                             : (string) null));
        }
        br.StepOut();
      }
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = this.BigEndian;
      bool flag = this.Version == FMG.FMGVersion.DarkSouls3;
      bw.WriteByte((byte) 0);
      bw.WriteBoolean(bw.BigEndian);
      bw.WriteByte((byte) this.Version);
      bw.WriteByte((byte) 0);
      bw.ReserveInt32("FileSize");
      bw.WriteByte((byte) 1);
      bw.WriteByte(this.Version == FMG.FMGVersion.DemonsSouls
                       ? byte.MaxValue
                       : (byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.ReserveInt32("GroupCount");
      bw.WriteInt32(this.Entries.Count);
      if (flag)
        bw.WriteInt32((int) byte.MaxValue);
      if (flag)
        bw.ReserveInt64("StringOffsets");
      else
        bw.ReserveInt32("StringOffsets");
      if (flag)
        bw.WriteInt64(0L);
      else
        bw.WriteInt32(0);
      int num = 0;
      this.Entries.Sort(
          (Comparison<FMG.Entry>) ((e1, e2) => e1.ID.CompareTo(e2.ID)));
      for (int index = 0; index < this.Entries.Count; ++index) {
        bw.WriteInt32(index);
        bw.WriteInt32(this.Entries[index].ID);
        while (index < this.Entries.Count - 1 &&
               this.Entries[index + 1].ID == this.Entries[index].ID + 1)
          ++index;
        bw.WriteInt32(this.Entries[index].ID);
        if (flag)
          bw.WriteInt32(0);
        ++num;
      }
      bw.FillInt32("GroupCount", num);
      if (flag)
        bw.FillInt64("StringOffsets", bw.Position);
      else
        bw.FillInt32("StringOffsets", (int) bw.Position);
      for (int index = 0; index < this.Entries.Count; ++index) {
        if (flag)
          bw.ReserveInt64(string.Format("StringOffset{0}", (object) index));
        else
          bw.ReserveInt32(string.Format("StringOffset{0}", (object) index));
      }
      for (int index = 0; index < this.Entries.Count; ++index) {
        string text = this.Entries[index].Text;
        if (flag)
          bw.FillInt64(string.Format("StringOffset{0}", (object) index),
                       text == null ? 0L : bw.Position);
        else
          bw.FillInt32(string.Format("StringOffset{0}", (object) index),
                       text == null ? 0 : (int) bw.Position);
        if (text != null)
          bw.WriteUTF16(this.Entries[index].Text, true);
      }
      bw.FillInt32("FileSize", (int) bw.Position);
    }

    public string this[int id] {
      get {
        return this
               .Entries.Find((Predicate<FMG.Entry>) (entry => entry.ID == id))
               ?.Text;
      }
      set {
        if (this.Entries.Any<FMG.Entry>(
            (Func<FMG.Entry, bool>) (entry => entry.ID == id)))
          this.Entries.Find((Predicate<FMG.Entry>) (entry => entry.ID == id))
              .Text = value;
        else
          this.Entries.Add(new FMG.Entry(id, value));
      }
    }

    public class Entry {
      public int ID;
      public string Text;

      public Entry(int id, string text) {
        this.ID = id;
        this.Text = text;
      }

      public override string ToString() {
        return string.Format("{0}: {1}",
                             (object) this.ID,
                             (object) (this.Text ?? "<null>"));
      }
    }

    public enum FMGVersion : byte {
      DemonsSouls,
      DarkSouls1,
      DarkSouls3,
    }
  }
}