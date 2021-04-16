// Decompiled with JetBrains decompiler
// Type: SoulsFormats.ENFL
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class ENFL : SoulsFile<ENFL>
  {
    public List<ENFL.Struct1> Struct1s;
    public List<ENFL.Struct2> Struct2s;
    public List<string> Strings;

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == nameof (ENFL);
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      br.AssertASCII(nameof (ENFL));
      br.AssertInt32(66581);
      int compressedSize = br.ReadInt32();
      br.ReadInt32();
      br = new BinaryReaderEx(false, SFUtil.ReadZlib(br, compressedSize));
      br.AssertInt32(new int[1]);
      int capacity1 = br.ReadInt32();
      int capacity2 = br.ReadInt32();
      br.AssertInt32(new int[1]);
      this.Struct1s = new List<ENFL.Struct1>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Struct1s.Add(new ENFL.Struct1(br));
      br.Pad(16);
      this.Struct2s = new List<ENFL.Struct2>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Struct2s.Add(new ENFL.Struct2(br));
      br.Pad(16);
      int num = (int) br.AssertInt16(new short[1]);
      this.Strings = new List<string>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Strings.Add(br.ReadUTF16());
    }

    protected override void Write(BinaryWriterEx bw)
    {
      BinaryWriterEx bw1 = new BinaryWriterEx(false);
      bw1.WriteInt32(0);
      bw1.WriteInt32(this.Struct1s.Count);
      bw1.WriteInt32(this.Struct2s.Count);
      bw1.WriteInt32(0);
      foreach (ENFL.Struct1 struct1 in this.Struct1s)
        struct1.Write(bw1);
      bw1.Pad(16);
      foreach (ENFL.Struct2 struct2 in this.Struct2s)
        struct2.Write(bw1);
      bw1.Pad(16);
      bw1.WriteInt16((short) 0);
      foreach (string text in this.Strings)
        bw1.WriteUTF16(text, true);
      bw1.Pad(16);
      byte[] input = bw1.FinishBytes();
      bw.WriteASCII(nameof (ENFL), false);
      bw.WriteInt32(66581);
      bw.ReserveInt32("CompressedSize");
      bw.WriteInt32(input.Length);
      int num = SFUtil.WriteZlib(bw, (byte) 218, input);
      bw.FillInt32("CompressedSize", num);
    }

    public class Struct1
    {
      public short Step;
      public short Index;

      internal Struct1(BinaryReaderEx br)
      {
        this.Step = br.ReadInt16();
        this.Index = br.ReadInt16();
      }

      internal void Write(BinaryWriterEx bw)
      {
        bw.WriteInt16(this.Step);
        bw.WriteInt16(this.Index);
      }

      public override string ToString()
      {
        return string.Format("0x{0:X4} 0x{1:X4}", (object) this.Step, (object) this.Index);
      }
    }

    public class Struct2
    {
      public long Unk1;

      internal Struct2(BinaryReaderEx br)
      {
        this.Unk1 = br.ReadInt64();
      }

      internal void Write(BinaryWriterEx bw)
      {
        bw.WriteInt64(this.Unk1);
      }

      public override string ToString()
      {
        return string.Format("0x{0:X16}", (object) this.Unk1);
      }
    }
  }
}
