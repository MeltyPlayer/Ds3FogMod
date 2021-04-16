// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BND3Reader
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class BND3Reader : BinderReader, IBND3
  {
    public int Unk18 { get; set; }

    public DCX.Type Compression { get; set; }

    public BND3Reader(string path)
    {
      this.Read(new BinaryReaderEx(false, (Stream) File.OpenRead(path)));
    }

    public BND3Reader(byte[] bytes)
    {
      this.Read(new BinaryReaderEx(false, (Stream) new MemoryStream(bytes)));
    }

    private void Read(BinaryReaderEx br)
    {
      DCX.Type compression;
      br = SFUtil.GetDecompressedBR(br, out compression);
      this.Compression = compression;
      this.Files = BND3.ReadHeader((IBND3) this, br);
      this.DataBR = br;
    }
  }
}
