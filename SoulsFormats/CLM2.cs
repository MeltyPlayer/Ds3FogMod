// Decompiled with JetBrains decompiler
// Type: SoulsFormats.CLM2
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class CLM2 : SoulsFile<CLM2>
  {
    public List<CLM2.Mesh> Meshes { get; set; }

    public CLM2()
    {
      this.Meshes = new List<CLM2.Mesh>();
    }

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == nameof (CLM2);
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.AssertASCII(nameof (CLM2));
      br.AssertInt32(new int[1]);
      int num1 = (int) br.AssertInt16((short) 1);
      int num2 = (int) br.AssertInt16((short) 1);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      int capacity = br.ReadInt32();
      br.AssertInt32(40);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(40);
      this.Meshes = new List<CLM2.Mesh>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Meshes.Add(new CLM2.Mesh(br));
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bw.WriteASCII(nameof (CLM2), false);
      bw.WriteInt32(0);
      bw.WriteInt16((short) 1);
      bw.WriteInt16((short) 1);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(this.Meshes.Count);
      bw.WriteInt32(40);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(40);
      for (int index = 0; index < this.Meshes.Count; ++index)
        this.Meshes[index].WriteHeader(bw, index);
      for (int index = 0; index < this.Meshes.Count; ++index)
        this.Meshes[index].WriteEntries(bw, index);
    }

    public class Mesh : List<CLM2.Mesh.Entry>
    {
      internal Mesh(BinaryReaderEx br)
      {
        br.AssertInt32(new int[1]);
        int num1 = br.ReadInt32();
        uint num2 = br.ReadUInt32();
        br.AssertInt32(new int[1]);
        br.StepIn((long) num2);
        for (int index = 0; index < num1; ++index)
          this.Add(new CLM2.Mesh.Entry(br));
        br.StepOut();
      }

      internal void WriteHeader(BinaryWriterEx bw, int index)
      {
        bw.WriteInt32(0);
        bw.WriteInt32(this.Count);
        bw.ReserveUInt32(string.Format("EntriesOffset{0}", (object) index));
        bw.WriteInt32(0);
      }

      internal void WriteEntries(BinaryWriterEx bw, int index)
      {
        if (this.Count == 0)
        {
          bw.FillUInt32(string.Format("EntriesOffset{0}", (object) index), 0U);
        }
        else
        {
          bw.FillUInt32(string.Format("EntriesOffset{0}", (object) index), (uint) bw.Position);
          foreach (CLM2.Mesh.Entry entry in (List<CLM2.Mesh.Entry>) this)
            entry.Write(bw);
          bw.Pad(8);
        }
      }

      public class Entry
      {
        public short Unk00 { get; set; }

        public short Unk02 { get; set; }

        public Entry(short unk00, short unk02)
        {
          this.Unk00 = unk00;
          this.Unk02 = unk02;
        }

        internal Entry(BinaryReaderEx br)
        {
          this.Unk00 = br.ReadInt16();
          this.Unk02 = br.ReadInt16();
        }

        internal void Write(BinaryWriterEx bw)
        {
          bw.WriteInt16(this.Unk00);
          bw.WriteInt16(this.Unk02);
        }

        public override string ToString()
        {
          return string.Format("{0} - {1}", (object) this.Unk00, (object) this.Unk02);
        }
      }
    }
  }
}
