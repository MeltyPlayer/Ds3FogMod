// Decompiled with JetBrains decompiler
// Type: SoulsFormats.IMsbPart
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public interface IMsbPart : IMsbEntry {
    string ModelName { get; set; }

    Vector3 Position { get; set; }

    Vector3 Rotation { get; set; }

    Vector3 Scale { get; set; }
  }
}