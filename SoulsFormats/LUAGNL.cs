// Decompiled with JetBrains decompiler
// Type: SoulsFormats.LUAGNL
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class LUAGNL : SoulsFile<LUAGNL>
  {
    public bool BigEndian { get; set; }

    public bool LongFormat { get; set; }

    public List<string> Globals { get; set; }

    public LUAGNL()
      : this(false, false)
    {
    }

    public LUAGNL(bool bigEndian, bool longFormat)
    {
      this.BigEndian = bigEndian;
      this.LongFormat = longFormat;
      this.Globals = new List<string>();
    }

    protected override void Read(BinaryReaderEx br)
    {
      this.BigEndian = br.GetInt16(0L) == (short) 0;
      br.BigEndian = this.BigEndian;
      this.LongFormat = br.GetInt32(this.BigEndian ? 0L : 4L) == 0;
      this.Globals = new List<string>();
      long offset;
      do
      {
        offset = this.LongFormat ? br.ReadInt64() : (long) br.ReadUInt32();
        if (offset != 0L)
          this.Globals.Add(this.LongFormat ? br.GetUTF16(offset) : br.GetShiftJIS(offset));
      }
      while (offset != 0L);
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bw.BigEndian = this.BigEndian;
      for (int index = 0; index < this.Globals.Count; ++index)
      {
        if (this.LongFormat)
          bw.ReserveInt64(string.Format("Offset{0}", (object) index));
        else
          bw.ReserveUInt32(string.Format("Offset{0}", (object) index));
      }
      if (this.LongFormat)
        bw.WriteInt64(0L);
      else
        bw.WriteUInt32(0U);
      for (int index = 0; index < this.Globals.Count; ++index)
      {
        if (this.LongFormat)
        {
          bw.FillInt64(string.Format("Offset{0}", (object) index), bw.Position);
          bw.WriteUTF16(this.Globals[index], true);
        }
        else
        {
          bw.FillUInt32(string.Format("Offset{0}", (object) index), (uint) bw.Position);
          bw.WriteShiftJIS(this.Globals[index], true);
        }
      }
      bw.Pad(16);
    }
  }
}
