// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Oodle26
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  internal static class Oodle26
  {
    public const int OODLELZ_FAILED = 0;

    [DllImport("oo2core_6_win64.dll")]
    private static extern uint OodleLZ_Compress(
      Oodle26.Compressor compressor,
      byte[] src_buf,
      ulong src_len,
      byte[] dst_buf,
      Oodle26.CompressionLevel level,
      Oodle26.CompressOptions options,
      ulong offs,
      ulong unused,
      IntPtr scratch,
      ulong scratch_size);

    [DllImport("oo2core_6_win64.dll")]
    private static extern IntPtr OodleLZ_CompressOptions_GetDefault(
      Oodle26.Compressor compressor,
      Oodle26.CompressionLevel compressionLevel);

    [DllImport("oo2core_6_win64.dll")]
    private static extern uint OodleLZ_Decompress(
      byte[] compBuf,
      ulong src_len,
      byte[] decodeTo,
      ulong dst_size,
      Oodle26.FuzzSafe fuzzSafe,
      int crc,
      int verbose,
      IntPtr dst_base,
      ulong e,
      IntPtr cb,
      IntPtr cb_ctx,
      IntPtr scratch,
      ulong scratch_size,
      int threadPhase);

    [DllImport("oo2core_6_win64.dll")]
    private static extern uint OodleLZ_GetCompressedBufferSizeNeeded(ulong src_len);

    public static byte[] Compress(
      byte[] source,
      Oodle26.Compressor compressor,
      Oodle26.CompressionLevel level)
    {
      int bufferSizeNeeded = (int) Oodle26.OodleLZ_GetCompressedBufferSizeNeeded((ulong) source.Length);
      Oodle26.CompressOptions options = Oodle26.CompressOptions.GetDefault(compressor, level);
      byte[] array = new byte[bufferSizeNeeded];
      uint num = Oodle26.OodleLZ_Compress(compressor, source, (ulong) source.Length, array, level, options, 0UL, 0UL, IntPtr.Zero, 0UL);
      Array.Resize<byte>(ref array, (int) num);
      return array;
    }

    public static byte[] Decompress(byte[] source, ulong uncompressedSize)
    {
      byte[] decodeTo = new byte[uncompressedSize];
      int num = (int) Oodle26.OodleLZ_Decompress(source, (ulong) source.Length, decodeTo, uncompressedSize, Oodle26.FuzzSafe.Yes, 0, 0, IntPtr.Zero, 0UL, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0UL, 0);
      return decodeTo;
    }

    public enum Compressor
    {
      LZH,
      LZHLW,
      LZNIB,
      None,
      LZB16,
      LZBLW,
      LZA,
      LZNA,
      Kraken,
      Mermaid,
      BitKnit,
      Selkie,
      Hydra,
      Leviathan,
    }

    public enum CompressionLevel
    {
      None,
      SuperFast,
      VeryFast,
      Fast,
      Normal,
      Optimal1,
      Optimal2,
      Optimal3,
      Optimal4,
      TooHigh,
    }

    public enum FuzzSafe
    {
      No,
      Yes,
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CompressOptions
    {
      private int Unk00;
      private int Unk04;
      private int Unk08;
      private int Unk0C;
      private int Unk10;
      private int Unk14;
      private int SpaceSpeedTradeoffBytes;
      private int Unk1C;
      private int Unk20;
      private int DictionarySize;
      private int Unk28;
      private int Unk2C;

      public static Oodle26.CompressOptions GetDefault(
        Oodle26.Compressor compressor,
        Oodle26.CompressionLevel compressionLevel)
      {
        return Marshal.PtrToStructure<Oodle26.CompressOptions>(Oodle26.OodleLZ_CompressOptions_GetDefault(compressor, compressionLevel));
      }
    }
  }
}
