// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Kuon.BND0
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats.Kuon {
  [ComVisible(true)]
  public class BND0 : SoulsFile<BND0> {
    public List<BND0.File> Files;
    public int Unk04;

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "BND\0";
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertASCII("BND\0");
      this.Unk04 = br.AssertInt32(200, 202);
      int num = br.ReadInt32();
      int capacity = br.ReadInt32();
      this.Files = new List<BND0.File>(capacity);
      for (int index = 0; index < capacity; ++index) {
        int nextOffset = num;
        if (index < capacity - 1)
          nextOffset = br.GetInt32(br.Position + 12L + 4L);
        this.Files.Add(new BND0.File(br, nextOffset));
      }
    }

    public class File {
      public int ID;
      public string Name;
      public byte[] Bytes;

      internal File(BinaryReaderEx br, int nextOffset) {
        this.ID = br.ReadInt32();
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        this.Name = br.GetShiftJIS((long) num2);
        this.Bytes = br.GetBytes((long) num1, nextOffset - num1);
      }
    }
  }
}