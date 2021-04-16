// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Kuon.DVDBND0
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats.Kuon {
  [ComVisible(true)]
  public class DVDBND0 : SoulsFile<DVDBND0> {
    public List<DVDBND0.File> Files;

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertASCII("BND\0");
      br.AssertInt32(202);
      br.ReadInt32();
      int capacity = br.ReadInt32();
      this.Files = new List<DVDBND0.File>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Files.Add(new DVDBND0.File(br));
    }

    public class File {
      public int ID;
      public string Name;
      public byte[] Bytes;

      internal File(BinaryReaderEx br) {
        this.ID = br.ReadInt32();
        int num1 = br.ReadInt32();
        int count = br.ReadInt32();
        int num2 = br.ReadInt32();
        this.Name = br.GetShiftJIS((long) num2);
        this.Bytes = br.GetBytes((long) num1, count);
      }
    }
  }
}