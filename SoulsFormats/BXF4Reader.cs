// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BXF4Reader
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class BXF4Reader : BinderReader, IBXF4
  {
    public bool Unk04 { get; set; }

    public bool Unk05 { get; set; }

    public bool Unicode { get; set; }

    public byte Extended { get; set; }

    public BXF4Reader(string bhdPath, string bdtPath)
    {
      using (FileStream fileStream1 = File.OpenRead(bhdPath))
      {
        FileStream fileStream2 = File.OpenRead(bdtPath);
        this.Read(new BinaryReaderEx(false, (Stream) fileStream1), new BinaryReaderEx(false, (Stream) fileStream2));
      }
    }

    public BXF4Reader(string bhdPath, byte[] bdtBytes)
    {
      using (FileStream fileStream = File.OpenRead(bhdPath))
      {
        MemoryStream memoryStream = new MemoryStream(bdtBytes);
        this.Read(new BinaryReaderEx(false, (Stream) fileStream), new BinaryReaderEx(false, (Stream) memoryStream));
      }
    }

    public BXF4Reader(byte[] bhdBytes, string bdtPath)
    {
      using (MemoryStream memoryStream = new MemoryStream(bhdBytes))
      {
        FileStream fileStream = File.OpenRead(bdtPath);
        this.Read(new BinaryReaderEx(false, (Stream) memoryStream), new BinaryReaderEx(false, (Stream) fileStream));
      }
    }

    public BXF4Reader(byte[] bhdBytes, byte[] bdtBytes)
    {
      using (MemoryStream memoryStream1 = new MemoryStream(bhdBytes))
      {
        MemoryStream memoryStream2 = new MemoryStream(bdtBytes);
        this.Read(new BinaryReaderEx(false, (Stream) memoryStream1), new BinaryReaderEx(false, (Stream) memoryStream2));
      }
    }

    private void Read(BinaryReaderEx brHeader, BinaryReaderEx brData)
    {
      BXF4.ReadBDFHeader(brData);
      this.Files = BXF4.ReadBHFHeader((IBXF4) this, brHeader);
      this.DataBR = brData;
    }
  }
}
