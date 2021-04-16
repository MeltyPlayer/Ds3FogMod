// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Otogi2.DAT
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats.Otogi2 {
  [ComVisible(true)]
  public class DAT : SoulsFile<DAT> {
    public byte[] Data1;
    public byte[] Data2;
    public byte[] Data3;
    public List<DAT.Texture> Textures;

    protected override void Read(BinaryReaderEx br) {
      br.ReadInt32();
      int num1 = br.ReadInt32();
      int num2 = br.ReadInt32();
      int num3 = br.ReadInt32();
      int capacity = br.ReadInt32();
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      this.Textures = new List<DAT.Texture>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Textures.Add(new DAT.Texture(br));
      if (num1 != 0)
        this.Data1 = br.GetBytes((long) num1, br.GetInt32((long) num1));
      if (num2 != 0)
        this.Data2 = br.GetBytes((long) num2, br.GetInt32((long) num2));
      if (num3 == 0)
        return;
      this.Data3 = br.GetBytes((long) num3, br.GetInt32((long) num3));
    }

    public class Texture {
      public string Name;
      public byte[] Data;

      internal Texture(BinaryReaderEx br) {
        int count = br.ReadInt32();
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        this.Name = br.GetShiftJIS((long) num2);
        this.Data = br.GetBytes((long) num1, count);
      }
    }
  }
}