﻿// Decompiled with JetBrains decompiler
// Type: SoulsFormats.IMsb
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public interface IMsb
  {
    IMsbParam<IMsbModel> Models { get; }

    IMsbParam<IMsbPart> Parts { get; }

    IMsbParam<IMsbRegion> Regions { get; }
  }
}
