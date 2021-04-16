// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MWC.TDAT
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats.MWC {
  [ComVisible(true)]
  public class TDAT : SoulsFile<TDAT> {
    public int Unk1C;
    public List<TDAT.Texture> Textures;

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.ReadInt32();
      br.AssertPattern(12, (byte) 0);
      int capacity = br.ReadInt32();
      br.AssertPattern(8, (byte) 0);
      this.Unk1C = br.ReadInt32();
      this.Textures = new List<TDAT.Texture>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Textures.Add(new TDAT.Texture(br));
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