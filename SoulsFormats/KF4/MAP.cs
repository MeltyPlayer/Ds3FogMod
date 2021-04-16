// Decompiled with JetBrains decompiler
// Type: SoulsFormats.KF4.MAP
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats.KF4 {
  [ComVisible(true)]
  public class MAP : SoulsFile<MAP> {
    public List<MAP.Struct4> Struct4s { get; set; }

    protected override void Read(BinaryReaderEx br) {
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      int num1 = br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.AssertInt32(new int[1]);
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      int num2 = (int) br.ReadInt16();
      int num3 = (int) br.ReadInt16();
      int num4 = (int) br.ReadInt16();
      short num5 = br.ReadInt16();
      int num6 = (int) br.ReadInt16();
      int num7 = (int) br.ReadInt16();
      int num8 = (int) br.AssertInt16(new short[1]);
      int num9 = (int) br.ReadInt16();
      int num10 = (int) br.ReadInt16();
      int num11 = (int) br.ReadInt16();
      br.Position = (long) num1;
      this.Struct4s = new List<MAP.Struct4>((int) num5);
      for (int index = 0; index < (int) num5; ++index)
        this.Struct4s.Add(new MAP.Struct4(br));
    }

    public class Struct4 {
      public OM2 Om2 { get; set; }

      internal Struct4(BinaryReaderEx br) {
        byte[] bytes = br.ReadBytes(br.GetInt32(br.Position));
        br.ReadBytes(br.GetInt32(br.Position));
        this.Om2 = SoulsFile<OM2>.Read(bytes);
      }
    }
  }
}