// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Binder
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public static class Binder
  {
    public static Binder.Format ReadFormat(BinaryReaderEx br, bool bitBigEndian)
    {
      byte num = br.ReadByte();
      return (bitBigEndian ? 1 : (((int) num & 1) == 0 ? 0 : (((int) num & 128) == 0 ? 1 : 0))) == 0 ? (Binder.Format) SFUtil.ReverseBits(num) : (Binder.Format) num;
    }

    public static void WriteFormat(BinaryWriterEx bw, bool bitBigEndian, Binder.Format format)
    {
      byte num = (bitBigEndian ? 1 : (Binder.ForceBigEndian(format) ? 1 : 0)) != 0 ? (byte) format : SFUtil.ReverseBits((byte) format);
      bw.WriteByte(num);
    }

    public static bool ForceBigEndian(Binder.Format format)
    {
      return (uint) (format & Binder.Format.BigEndian) > 0U;
    }

    public static bool HasIDs(Binder.Format format)
    {
      return (uint) (format & Binder.Format.IDs) > 0U;
    }

    public static bool HasNames(Binder.Format format)
    {
      return (uint) (format & (Binder.Format.Names1 | Binder.Format.Names2)) > 0U;
    }

    public static bool HasLongOffsets(Binder.Format format)
    {
      return (uint) (format & Binder.Format.LongOffsets) > 0U;
    }

    public static bool HasCompression(Binder.Format format)
    {
      return (uint) (format & Binder.Format.Compression) > 0U;
    }

    public static bool HasFlag6(Binder.Format format)
    {
      return (uint) (format & Binder.Format.Flag6) > 0U;
    }

    public static bool HasFlag7(Binder.Format format)
    {
      return (uint) (format & Binder.Format.Flag7) > 0U;
    }

    public static long GetBND4FileHeaderSize(Binder.Format format)
    {
      return (long) (16 + (Binder.HasLongOffsets(format) ? 8 : 4) + (Binder.HasCompression(format) ? 8 : 0) + (Binder.HasIDs(format) ? 4 : 0) + (Binder.HasNames(format) ? 4 : 0));
    }

    public static Binder.FileFlags ReadFileFlags(
      BinaryReaderEx br,
      bool bitBigEndian,
      Binder.Format format)
    {
      int num1 = bitBigEndian ? 1 : (Binder.ForceBigEndian(format) ? 1 : 0);
      byte num2 = br.ReadByte();
      return num1 == 0 ? (Binder.FileFlags) SFUtil.ReverseBits(num2) : (Binder.FileFlags) num2;
    }

    public static void WriteFileFlags(
      BinaryWriterEx bw,
      bool bitBigEndian,
      Binder.Format format,
      Binder.FileFlags flags)
    {
      byte num = (bitBigEndian ? 1 : (Binder.ForceBigEndian(format) ? 1 : 0)) != 0 ? (byte) flags : SFUtil.ReverseBits((byte) flags);
      bw.WriteByte(num);
    }

    public static bool IsCompressed(Binder.FileFlags flags)
    {
      return (uint) (flags & Binder.FileFlags.Compressed) > 0U;
    }

    [Flags]
    public enum Format : byte
    {
      None = 0,
      BigEndian = 1,
      IDs = 2,
      Names1 = 4,
      Names2 = 8,
      LongOffsets = 16, // 0x10
      Compression = 32, // 0x20
      Flag6 = 64, // 0x40
      Flag7 = 128, // 0x80
    }

    [Flags]
    public enum FileFlags : byte
    {
      None = 0,
      Compressed = 1,
      Flag1 = 2,
      Flag2 = 4,
      Flag3 = 8,
      Flag4 = 16, // 0x10
      Flag5 = 32, // 0x20
      Flag6 = 64, // 0x40
      Flag7 = 128, // 0x80
    }
  }
}
