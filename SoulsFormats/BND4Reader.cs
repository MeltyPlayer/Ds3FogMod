// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BND4Reader
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class BND4Reader : BinderReader, IBND4
  {
    public bool Unk04 { get; set; }

    public bool Unk05 { get; set; }

    public bool Unicode { get; set; }

    public byte Extended { get; set; }

    public DCX.Type Compression { get; set; }

    public BND4Reader(string path)
    {
      this.Read(new BinaryReaderEx(false, (Stream) File.OpenRead(path)));
    }

    public BND4Reader(byte[] bytes)
    {
      this.Read(new BinaryReaderEx(false, (Stream) new MemoryStream(bytes)));
    }

    private void Read(BinaryReaderEx br)
    {
      DCX.Type compression;
      br = SFUtil.GetDecompressedBR(br, out compression);
      this.Compression = compression;
      this.Files = BND4.ReadHeader((IBND4) this, br);
      this.DataBR = br;
    }
  }
}
