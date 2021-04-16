// Decompiled with JetBrains decompiler
// Type: SoulsFormats.IBND3
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

namespace SoulsFormats {
  internal interface IBND3 {
    string Version { get; set; }

    Binder.Format Format { get; set; }

    bool BigEndian { get; set; }

    bool BitBigEndian { get; set; }

    int Unk18 { get; set; }
  }
}