// Decompiled with JetBrains decompiler
// Type: SoulsFormats.SFEncoding
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Text;

namespace SoulsFormats
{
  internal static class SFEncoding
  {
    public static readonly Encoding ASCII = Encoding.ASCII;
    public static readonly Encoding ShiftJIS = Encoding.GetEncoding("shift-jis");
    public static readonly Encoding UTF16 = Encoding.Unicode;
    public static readonly Encoding UTF16BE = Encoding.BigEndianUnicode;
  }
}
