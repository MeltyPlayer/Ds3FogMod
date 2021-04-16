// Decompiled with JetBrains decompiler
// Type: SoulsFormats.DCX
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public static class DCX
  {
    internal static bool Is(BinaryReaderEx br)
    {
      if (br.Stream.Length < 4L)
        return false;
      string ascii = br.GetASCII(0L, 4);
      return ascii == "DCP\0" || ascii == "DCX\0";
    }

    public static bool Is(byte[] bytes)
    {
      return DCX.Is(new BinaryReaderEx(true, bytes));
    }

    public static bool Is(string path)
    {
      using (FileStream fileStream = File.OpenRead(path))
        return DCX.Is(new BinaryReaderEx(true, (Stream) fileStream));
    }

    public static byte[] Decompress(byte[] data, out DCX.Type type)
    {
      return DCX.Decompress(new BinaryReaderEx(true, data), out type);
    }

    public static byte[] Decompress(byte[] data)
    {
      return DCX.Decompress(data, out DCX.Type _);
    }

    public static byte[] Decompress(string path, out DCX.Type type)
    {
      using (FileStream fileStream = File.OpenRead(path))
        return DCX.Decompress(new BinaryReaderEx(true, (Stream) fileStream), out type);
    }

    public static byte[] Decompress(string path)
    {
      return DCX.Decompress(path, out DCX.Type _);
    }

    internal static byte[] Decompress(BinaryReaderEx br, out DCX.Type type)
    {
      br.BigEndian = true;
      type = DCX.Type.Unknown;
      string str = br.ReadASCII(4);
      if (str == "DCP\0")
      {
        string ascii = br.GetASCII(4L, 4);
        if (ascii == "DFLT")
          type = DCX.Type.DCP_DFLT;
        else if (ascii == "EDGE")
          type = DCX.Type.DCP_EDGE;
      }
      else if (str == "DCX\0")
      {
        string ascii = br.GetASCII(40L, 4);
        if (ascii == "EDGE")
          type = DCX.Type.DCX_EDGE;
        else if (ascii == "DFLT")
        {
          int int32_1 = br.GetInt32(4L);
          int int32_2 = br.GetInt32(16L);
          int int32_3 = br.GetInt32(48L);
          switch (int32_2)
          {
            case 36:
              type = DCX.Type.DCX_DFLT_10000_24_9;
              break;
            case 68:
              switch (int32_1)
              {
                case 65536:
                  type = DCX.Type.DCX_DFLT_10000_44_9;
                  break;
                case 69632:
                  switch (int32_3)
                  {
                    case 134217728:
                      type = DCX.Type.DCX_DFLT_11000_44_8;
                      break;
                    case 150994944:
                      type = DCX.Type.DCX_DFLT_11000_44_9;
                      break;
                  }
                  break;
              }
              break;
          }
        }
        else if (ascii == "KRAK")
          type = DCX.Type.DCX_KRAK;
      }
      else
      {
        int num1 = (int) br.GetByte(0L);
        byte num2 = br.GetByte(1L);
        if (num1 == 120 && (num2 == (byte) 1 || num2 == (byte) 94 || (num2 == (byte) 156 || num2 == (byte) 218)))
          type = DCX.Type.Zlib;
      }
      br.Position = 0L;
      if (type == DCX.Type.Zlib)
        return SFUtil.ReadZlib(br, (int) br.Length);
      if (type == DCX.Type.DCP_EDGE)
        return DCX.DecompressDCPEDGE(br);
      if (type == DCX.Type.DCP_DFLT)
        return DCX.DecompressDCPDFLT(br);
      if (type == DCX.Type.DCX_EDGE)
        return DCX.DecompressDCXEDGE(br);
      if (type == DCX.Type.DCX_DFLT_10000_24_9 || type == DCX.Type.DCX_DFLT_10000_44_9 || (type == DCX.Type.DCX_DFLT_11000_44_8 || type == DCX.Type.DCX_DFLT_11000_44_9))
        return DCX.DecompressDCXDFLT(br, type);
      if (type == DCX.Type.DCX_KRAK)
        return DCX.DecompressDCXKRAK(br);
      throw new FormatException("Unknown DCX format.");
    }

    private static byte[] DecompressDCPDFLT(BinaryReaderEx br)
    {
      br.AssertASCII("DCP\0");
      br.AssertASCII("DFLT");
      br.AssertInt32(32);
      br.AssertInt32(150994944);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(65792);
      br.AssertASCII("DCS\0");
      br.ReadInt32();
      int compressedSize = br.ReadInt32();
      byte[] numArray = SFUtil.ReadZlib(br, compressedSize);
      br.AssertASCII("DCA\0");
      br.AssertInt32(8);
      return numArray;
    }

    private static byte[] DecompressDCPEDGE(BinaryReaderEx br)
    {
      br.AssertASCII("DCP\0");
      br.AssertASCII("EDGE");
      br.AssertInt32(32);
      br.AssertInt32(150994944);
      br.AssertInt32(65536);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(1048832);
      br.AssertASCII("DCS\0");
      int length = br.ReadInt32();
      int count1 = br.ReadInt32();
      br.AssertInt32(new int[1]);
      long position = br.Position;
      br.Skip(count1);
      br.AssertASCII("DCA\0");
      br.ReadInt32();
      br.AssertASCII("EgdT");
      br.AssertInt32(65536);
      br.AssertInt32(32);
      br.AssertInt32(16);
      br.AssertInt32(65536);
      int num1 = br.ReadInt32();
      int num2 = br.ReadInt32();
      br.AssertInt32(1048576);
      if (num1 != 32 + num2 * 16)
        throw new InvalidDataException("Unexpected EgdT size in EDGE DCX.");
      byte[] buffer = new byte[length];
      using (MemoryStream memoryStream1 = new MemoryStream(buffer))
      {
        for (int index = 0; index < num2; ++index)
        {
          br.AssertInt32(new int[1]);
          int num3 = br.ReadInt32();
          int count2 = br.ReadInt32();
          int num4 = br.AssertInt32(0, 1) == 1 ? 1 : 0;
          byte[] bytes = br.GetBytes(position + (long) num3, count2);
          if (num4 != 0)
          {
            using (MemoryStream memoryStream2 = new MemoryStream(bytes))
            {
              using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream2, CompressionMode.Decompress))
                deflateStream.CopyTo((Stream) memoryStream1);
            }
          }
          else
            memoryStream1.Write(bytes, 0, bytes.Length);
        }
      }
      return buffer;
    }

    private static byte[] DecompressDCXEDGE(BinaryReaderEx br)
    {
      br.AssertASCII("DCX\0");
      br.AssertInt32(65536);
      br.AssertInt32(24);
      br.AssertInt32(36);
      br.AssertInt32(36);
      int num1 = br.ReadInt32();
      br.AssertASCII("DCS\0");
      int length = br.ReadInt32();
      br.ReadInt32();
      br.AssertASCII("DCP\0");
      br.AssertASCII("EDGE");
      br.AssertInt32(32);
      br.AssertInt32(150994944);
      br.AssertInt32(65536);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(1048832);
      long position = br.Position;
      br.AssertASCII("DCA\0");
      int num2 = br.ReadInt32();
      br.AssertASCII("EgdT");
      br.AssertInt32(65792);
      br.AssertInt32(36);
      br.AssertInt32(16);
      br.AssertInt32(65536);
      br.AssertInt32(length % 65536, 65536);
      int num3 = br.ReadInt32();
      int num4 = br.ReadInt32();
      br.AssertInt32(1048576);
      if (num1 != 80 + num4 * 16)
        throw new InvalidDataException("Unexpected unk1 value in EDGE DCX.");
      if (num3 != 36 + num4 * 16)
        throw new InvalidDataException("Unexpected EgdT size in EDGE DCX.");
      byte[] buffer = new byte[length];
      using (MemoryStream memoryStream1 = new MemoryStream(buffer))
      {
        for (int index = 0; index < num4; ++index)
        {
          br.AssertInt32(new int[1]);
          int num5 = br.ReadInt32();
          int count = br.ReadInt32();
          int num6 = br.AssertInt32(0, 1) == 1 ? 1 : 0;
          byte[] bytes = br.GetBytes(position + (long) num2 + (long) num5, count);
          if (num6 != 0)
          {
            using (MemoryStream memoryStream2 = new MemoryStream(bytes))
            {
              using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream2, CompressionMode.Decompress))
                deflateStream.CopyTo((Stream) memoryStream1);
            }
          }
          else
            memoryStream1.Write(bytes, 0, bytes.Length);
        }
      }
      return buffer;
    }

    private static byte[] DecompressDCXDFLT(BinaryReaderEx br, DCX.Type type)
    {
      br.AssertASCII("DCX\0");
      switch (type)
      {
        case DCX.Type.DCX_DFLT_10000_24_9:
        case DCX.Type.DCX_DFLT_10000_44_9:
          br.AssertInt32(65536);
          break;
        case DCX.Type.DCX_DFLT_11000_44_8:
        case DCX.Type.DCX_DFLT_11000_44_9:
          br.AssertInt32(69632);
          break;
      }
      br.AssertInt32(24);
      br.AssertInt32(36);
      if (type == DCX.Type.DCX_DFLT_10000_24_9)
      {
        br.AssertInt32(36);
        br.AssertInt32(44);
      }
      else if (type == DCX.Type.DCX_DFLT_10000_44_9 || type == DCX.Type.DCX_DFLT_11000_44_8 || type == DCX.Type.DCX_DFLT_11000_44_9)
      {
        br.AssertInt32(68);
        br.AssertInt32(76);
      }
      br.AssertASCII("DCS\0");
      br.ReadInt32();
      int compressedSize = br.ReadInt32();
      br.AssertASCII("DCP\0");
      br.AssertASCII("DFLT");
      br.AssertInt32(32);
      if (type == DCX.Type.DCX_DFLT_10000_24_9 || type == DCX.Type.DCX_DFLT_10000_44_9 || type == DCX.Type.DCX_DFLT_11000_44_9)
        br.AssertInt32(150994944);
      else if (type == DCX.Type.DCX_DFLT_11000_44_8)
        br.AssertInt32(134217728);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(65792);
      br.AssertASCII("DCA\0");
      br.ReadInt32();
      return SFUtil.ReadZlib(br, compressedSize);
    }

    private static byte[] DecompressDCXKRAK(BinaryReaderEx br)
    {
      br.AssertASCII("DCX\0");
      br.AssertInt32(69632);
      br.AssertInt32(24);
      br.AssertInt32(36);
      br.AssertInt32(68);
      br.AssertInt32(76);
      br.AssertASCII("DCS\0");
      uint num1 = br.ReadUInt32();
      uint num2 = br.ReadUInt32();
      br.AssertASCII("DCP\0");
      br.AssertASCII("KRAK");
      br.AssertInt32(32);
      br.AssertInt32(100663296);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(65792);
      br.AssertASCII("DCA\0");
      br.AssertInt32(8);
      return Oodle26.Decompress(br.ReadBytes((int) num2), (ulong) num1);
    }

    public static byte[] Compress(byte[] data, DCX.Type type)
    {
      BinaryWriterEx bw = new BinaryWriterEx(true);
      DCX.Compress(data, bw, type);
      return bw.FinishBytes();
    }

    public static void Compress(byte[] data, DCX.Type type, string path)
    {
      using (FileStream fileStream = File.Create(path))
      {
        BinaryWriterEx bw = new BinaryWriterEx(true, (Stream) fileStream);
        DCX.Compress(data, bw, type);
        bw.Finish();
      }
    }

    internal static void Compress(byte[] data, BinaryWriterEx bw, DCX.Type type)
    {
      if (type == DCX.Type.DCX_KRAK)
        type = DCX.Type.DCX_DFLT_11000_44_9;
      bw.BigEndian = true;
      if (type == DCX.Type.Zlib)
        SFUtil.WriteZlib(bw, (byte) 218, data);
      else if (type == DCX.Type.DCP_DFLT)
        DCX.CompressDCPDFLT(data, bw);
      else if (type == DCX.Type.DCX_EDGE)
        DCX.CompressDCXEDGE(data, bw);
      else if (type == DCX.Type.DCX_DFLT_10000_24_9 || type == DCX.Type.DCX_DFLT_10000_44_9 || (type == DCX.Type.DCX_DFLT_11000_44_8 || type == DCX.Type.DCX_DFLT_11000_44_9))
        DCX.CompressDCXDFLT(data, bw, type);
      else if (type == DCX.Type.DCX_KRAK)
      {
        DCX.CompressDCXKRAK(data, bw);
      }
      else
      {
        if (type == DCX.Type.Unknown)
          throw new ArgumentException("You cannot compress a DCX with an unknown type.");
        throw new NotImplementedException("Compression for the given type is not implemented.");
      }
    }

    private static void CompressDCPDFLT(byte[] data, BinaryWriterEx bw)
    {
      bw.WriteASCII("DCP\0", false);
      bw.WriteASCII("DFLT", false);
      bw.WriteInt32(32);
      bw.WriteInt32(150994944);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(65792);
      bw.WriteASCII("DCS\0", false);
      bw.WriteInt32(data.Length);
      bw.ReserveInt32("CompressedSize");
      int num = SFUtil.WriteZlib(bw, (byte) 218, data);
      bw.FillInt32("CompressedSize", num);
      bw.WriteASCII("DCA\0", false);
      bw.WriteInt32(8);
    }

    private static void CompressDCXEDGE(byte[] data, BinaryWriterEx bw)
    {
      int num1 = data.Length / 65536;
      if (data.Length % 65536 > 0)
        ++num1;
      bw.WriteASCII("DCX\0", false);
      bw.WriteInt32(65536);
      bw.WriteInt32(24);
      bw.WriteInt32(36);
      bw.WriteInt32(36);
      bw.WriteInt32(80 + num1 * 16);
      bw.WriteASCII("DCS\0", false);
      bw.WriteInt32(data.Length);
      bw.ReserveInt32("CompressedSize");
      bw.WriteASCII("DCP\0", false);
      bw.WriteASCII("EDGE", false);
      bw.WriteInt32(32);
      bw.WriteInt32(150994944);
      bw.WriteInt32(65536);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(1048832);
      long position1 = bw.Position;
      bw.WriteASCII("DCA\0", false);
      bw.ReserveInt32("DCASize");
      long position2 = bw.Position;
      bw.WriteASCII("EgdT", false);
      bw.WriteInt32(65792);
      bw.WriteInt32(36);
      bw.WriteInt32(16);
      bw.WriteInt32(65536);
      bw.WriteInt32(data.Length % 65536);
      bw.ReserveInt32("EGDTSize");
      bw.WriteInt32(num1);
      bw.WriteInt32(1048576);
      for (int index = 0; index < num1; ++index)
      {
        bw.WriteInt32(0);
        bw.ReserveInt32(string.Format("ChunkOffset{0}", (object) index));
        bw.ReserveInt32(string.Format("ChunkSize{0}", (object) index));
        bw.ReserveInt32(string.Format("ChunkCompressed{0}", (object) index));
      }
      bw.FillInt32("DCASize", (int) (bw.Position - position1));
      bw.FillInt32("EGDTSize", (int) (bw.Position - position2));
      long position3 = bw.Position;
      int num2 = 0;
      for (int index = 0; index < num1; ++index)
      {
        int count = 65536;
        if (index == num1 - 1)
          count = data.Length % 65536;
        byte[] bytes;
        using (MemoryStream memoryStream1 = new MemoryStream())
        {
          using (MemoryStream memoryStream2 = new MemoryStream(data, index * 65536, count))
          {
            DeflateStream deflateStream = new DeflateStream((Stream) memoryStream1, CompressionMode.Compress);
            memoryStream2.CopyTo((Stream) deflateStream);
            deflateStream.Close();
            bytes = memoryStream1.ToArray();
          }
        }
        if (bytes.Length < count)
        {
          bw.FillInt32(string.Format("ChunkCompressed{0}", (object) index), 1);
        }
        else
        {
          bw.FillInt32(string.Format("ChunkCompressed{0}", (object) index), 0);
          bytes = data;
        }
        num2 += bytes.Length;
        bw.FillInt32(string.Format("ChunkOffset{0}", (object) index), (int) (bw.Position - position3));
        bw.FillInt32(string.Format("ChunkSize{0}", (object) index), bytes.Length);
        bw.WriteBytes(bytes);
        bw.Pad(16);
      }
      bw.FillInt32("CompressedSize", num2);
    }

    private static void CompressDCXDFLT(byte[] data, BinaryWriterEx bw, DCX.Type type)
    {
      bw.WriteASCII("DCX\0", false);
      switch (type)
      {
        case DCX.Type.DCX_DFLT_10000_24_9:
        case DCX.Type.DCX_DFLT_10000_44_9:
          bw.WriteInt32(65536);
          break;
        case DCX.Type.DCX_DFLT_11000_44_8:
        case DCX.Type.DCX_DFLT_11000_44_9:
          bw.WriteInt32(69632);
          break;
      }
      bw.WriteInt32(24);
      bw.WriteInt32(36);
      if (type == DCX.Type.DCX_DFLT_10000_24_9)
      {
        bw.WriteInt32(36);
        bw.WriteInt32(44);
      }
      else if (type == DCX.Type.DCX_DFLT_10000_44_9 || type == DCX.Type.DCX_DFLT_11000_44_8 || type == DCX.Type.DCX_DFLT_11000_44_9)
      {
        bw.WriteInt32(68);
        bw.WriteInt32(76);
      }
      bw.WriteASCII("DCS\0", false);
      bw.WriteInt32(data.Length);
      bw.ReserveInt32("CompressedSize");
      bw.WriteASCII("DCP\0", false);
      bw.WriteASCII("DFLT", false);
      bw.WriteInt32(32);
      if (type == DCX.Type.DCX_DFLT_10000_24_9 || type == DCX.Type.DCX_DFLT_10000_44_9 || type == DCX.Type.DCX_DFLT_11000_44_9)
        bw.WriteInt32(150994944);
      else if (type == DCX.Type.DCX_DFLT_11000_44_8)
        bw.WriteInt32(134217728);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(65792);
      bw.WriteASCII("DCA\0", false);
      bw.WriteInt32(8);
      long position = bw.Position;
      SFUtil.WriteZlib(bw, (byte) 218, data);
      bw.FillInt32("CompressedSize", (int) (bw.Position - position));
    }

    private static void CompressDCXKRAK(byte[] data, BinaryWriterEx bw)
    {
      byte[] bytes = Oodle26.Compress(data, Oodle26.Compressor.Kraken, Oodle26.CompressionLevel.Optimal2);
      bw.WriteASCII("DCX\0", false);
      bw.WriteInt32(69632);
      bw.WriteInt32(24);
      bw.WriteInt32(36);
      bw.WriteInt32(68);
      bw.WriteInt32(76);
      bw.WriteASCII("DCS\0", false);
      bw.WriteUInt32((uint) data.Length);
      bw.WriteUInt32((uint) bytes.Length);
      bw.WriteASCII("DCP\0", false);
      bw.WriteASCII("KRAK", false);
      bw.WriteInt32(32);
      bw.WriteInt32(100663296);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(65792);
      bw.WriteASCII("DCA\0", false);
      bw.WriteInt32(8);
      bw.WriteBytes(bytes);
      bw.Pad(16);
    }

    public enum Type
    {
      Unknown,
      None,
      Zlib,
      DCP_EDGE,
      DCP_DFLT,
      DCX_EDGE,
      DCX_DFLT_10000_24_9,
      DCX_DFLT_10000_44_9,
      DCX_DFLT_11000_44_8,
      DCX_DFLT_11000_44_9,
      DCX_KRAK,
    }

    public enum DefaultType
    {
      DemonsSouls = 5,
      DarkSouls1 = 6,
      DarkSouls2 = 6,
      Bloodborne = 7,
      DarkSouls3 = 7,
      Sekiro = 10, // 0x0000000A
    }
  }
}
