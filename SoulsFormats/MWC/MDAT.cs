// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MWC.MDAT
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats.MWC {
  [ComVisible(true)]
  public class MDAT : SoulsFile<MDAT> {
    public int Unk1C;
    public byte[] Data1;
    public byte[] Data2;
    public byte[] Data3;
    public byte[] Data5;
    public byte[] Data6;

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      int num1 = br.ReadInt32();
      int num2 = br.ReadInt32();
      int num3 = br.ReadInt32();
      int num4 = br.ReadInt32();
      br.AssertInt32(new int[1]);
      int num5 = br.ReadInt32();
      int num6 = br.ReadInt32();
      this.Unk1C = br.ReadInt32();
      List<int> intList = new List<int>() {
          num1,
          num2,
          num3,
          num4,
          num5,
          num6
      };
      intList.Sort();
      if (num2 != 0)
        this.Data1 = br.GetBytes((long) num2,
                                 intList[intList.IndexOf(num2) + 1] - num2);
      if (num3 != 0)
        this.Data2 = br.GetBytes((long) num3,
                                 intList[intList.IndexOf(num3) + 1] - num3);
      if (num4 != 0)
        this.Data3 = br.GetBytes((long) num4,
                                 intList[intList.IndexOf(num4) + 1] - num4);
      if (num5 != 0)
        this.Data5 = br.GetBytes((long) num5,
                                 intList[intList.IndexOf(num5) + 1] - num5);
      if (num6 == 0)
        return;
      this.Data6 = br.GetBytes((long) num6,
                               intList[intList.IndexOf(num6) + 1] - num6);
    }
  }
}