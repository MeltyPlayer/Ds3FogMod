// Decompiled with JetBrains decompiler
// Type: SoulsFormats.F2TR
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class F2TR : SoulsFile<F2TR> {
    public bool BigEndian { get; set; }

    public List<F2TR.Entry> Entries { get; set; }

    public F2TR() {
      this.Entries = new List<F2TR.Entry>();
    }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == nameof(F2TR);
    }

    protected override void Read(BinaryReaderEx br) {
      br.AssertASCII(nameof(F2TR));
      this.BigEndian = br.AssertByte((byte) 0, byte.MaxValue) == byte.MaxValue;
      br.BigEndian = this.BigEndian;
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertInt16((short) 1);
      int num3 = (int) br.AssertInt16(new short[1]);
      int num4 = (int) br.AssertInt16((short) 16);
      int capacity = (int) br.ReadInt16();
      int num5 = (int) br.AssertInt16((short) 12);
      this.Entries = new List<F2TR.Entry>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Entries.Add(new F2TR.Entry(br));
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = this.BigEndian;
      bw.WriteASCII(nameof(F2TR), false);
      bw.WriteByte(this.BigEndian ? byte.MaxValue : (byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteInt16((short) 1);
      bw.WriteInt16((short) 0);
      bw.WriteInt16((short) 16);
      bw.WriteInt16((short) this.Entries.Count);
      bw.WriteInt16((short) 12);
      for (int index = 0; index < this.Entries.Count; ++index)
        this.Entries[index].Write(bw, index);
      for (int index = 0; index < this.Entries.Count; ++index)
        this.Entries[index].WriteIndices(bw, index);
      for (int index = 0; index < this.Entries.Count; ++index)
        this.Entries[index].WriteName(bw, index);
    }

    public class Entry {
      public string Name { get; set; }

      public List<short> Indices { get; set; }

      public Entry() {
        this.Name = "";
        this.Indices = new List<short>();
      }

      internal Entry(BinaryReaderEx br) {
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        short num3 = br.ReadInt16();
        int num4 = (int) br.AssertInt16(new short[1]);
        this.Name = br.GetUTF16((long) num1);
        this.Indices =
            new List<short>(
                (IEnumerable<short>) br.GetInt16s((long) num2, (int) num3));
      }

      internal void Write(BinaryWriterEx bw, int index) {
        bw.ReserveInt32(string.Format("NameOffset{0}", (object) index));
        bw.ReserveInt32(string.Format("IndicesOffset{0}", (object) index));
        bw.WriteInt16((short) this.Indices.Count);
        bw.WriteInt16((short) 0);
      }

      internal void WriteIndices(BinaryWriterEx bw, int index) {
        bw.FillInt32(string.Format("IndicesOffset{0}", (object) index),
                     (int) bw.Position);
        bw.WriteInt16s((IList<short>) this.Indices);
      }

      internal void WriteName(BinaryWriterEx bw, int index) {
        bw.FillInt32(string.Format("NameOffset{0}", (object) index),
                     (int) bw.Position);
        bw.WriteUTF16(this.Name, true);
      }
    }
  }
}