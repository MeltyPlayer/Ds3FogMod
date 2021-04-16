// Decompiled with JetBrains decompiler
// Type: SoulsFormats.KF4.CHR
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Runtime.InteropServices;

namespace SoulsFormats.KF4
{
  [ComVisible(true)]
  public class CHR : SoulsFile<CHR>
  {
    public int Unk00 { get; set; }

    public OM2 Om2 { get; set; }

    protected override void Read(BinaryReaderEx br)
    {
      this.Unk00 = br.ReadInt32();
      int num = br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      if (num == 0)
        return;
      br.Position = (long) num;
      this.Om2 = SoulsFile<OM2>.Read(br.ReadBytes(br.GetInt32(br.Position)));
    }
  }
}
