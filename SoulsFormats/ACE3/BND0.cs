// Decompiled with JetBrains decompiler
// Type: SoulsFormats.ACE3.BND0
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats.ACE3
{
  [ComVisible(true)]
  public class BND0 : SoulsFile<BND0>
  {
    public List<BND0.File> Files;
    public bool Lite;
    public byte Flag1;
    public byte Flag2;

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "BND\0";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      br.AssertASCII("BND\0");
      this.Lite = br.GetInt32(12L) == 0;
      int capacity;
      if (this.Lite)
      {
        br.ReadInt32();
        capacity = br.ReadInt32();
        br.AssertInt32(new int[1]);
      }
      else
      {
        br.AssertInt32(63487);
        br.AssertInt32(211);
        br.ReadInt32();
        capacity = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Flag1 = br.AssertByte((byte) 0, (byte) 32);
        this.Flag2 = br.AssertByte((byte) 0, (byte) 8);
        int num1 = (int) br.AssertByte((byte) 3);
        int num2 = (int) br.AssertByte(new byte[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
      }
      this.Files = new List<BND0.File>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Files.Add(new BND0.File(br, this.Lite));
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bw.BigEndian = false;
      bw.WriteASCII("BND\0", false);
      if (this.Lite)
      {
        bw.ReserveInt32("FileSize");
        bw.WriteInt32(this.Files.Count);
        bw.WriteInt32(0);
      }
      else
      {
        bw.WriteInt32(63487);
        bw.WriteInt32(211);
        bw.ReserveInt32("FileSize");
        bw.WriteInt32(this.Files.Count);
        bw.WriteInt32(0);
        bw.WriteByte(this.Flag1);
        bw.WriteByte(this.Flag2);
        bw.WriteByte((byte) 3);
        bw.WriteByte((byte) 0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
      }
      for (int index = 0; index < this.Files.Count; ++index)
        this.Files[index].Write(bw, this.Lite, index);
      for (int index = 0; index < this.Files.Count; ++index)
      {
        BND0.File file = this.Files[index];
        bw.Pad(32);
        bw.FillInt32(string.Format("FileOffset{0}", (object) index), (int) bw.Position);
        if (this.Lite)
        {
          bw.WriteInt32(file.Bytes.Length + 4);
          bw.WriteBytes(file.Bytes);
        }
        else
          bw.WriteBytes(file.Bytes);
      }
      bw.FillInt32("FileSize", (int) bw.Position);
    }

    public class File
    {
      public int ID;
      public byte[] Bytes;

      internal File(BinaryReaderEx br, bool lite)
      {
        this.ID = br.ReadInt32();
        int num = br.ReadInt32();
        int count;
        if (lite)
        {
          count = br.GetInt32((long) num) - 4;
          num += 4;
        }
        else
          count = br.ReadInt32();
        this.Bytes = br.GetBytes((long) num, count);
      }

      internal void Write(BinaryWriterEx bw, bool lite, int index)
      {
        bw.WriteInt32(this.ID);
        bw.ReserveInt32(string.Format("FileOffset{0}", (object) index));
        if (lite)
          return;
        bw.WriteInt32(this.Bytes.Length);
      }
    }
  }
}
