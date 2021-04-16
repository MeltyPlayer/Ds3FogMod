// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Headerizer
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulsFormats {
  internal static class Headerizer {
    private static Dictionary<byte, int> CompressedBPB =
        new Dictionary<byte, int>() {
            [(byte) 0] = 8,
            [(byte) 1] = 8,
            [(byte) 3] = 16,
            [(byte) 5] = 16,
            [(byte) 23] = 16,
            [(byte) 24] = 8,
            [(byte) 25] = 8,
            [(byte) 33] = 16,
            [(byte) 100] = 16,
            [(byte) 102] = 16,
            [(byte) 103] = 8,
            [(byte) 104] = 16,
            [(byte) 106] = 16,
            [(byte) 107] = 16,
            [(byte) 108] = 8,
            [(byte) 109] = 8,
            [(byte) 110] = 16,
            [(byte) 112] = 16,
            [(byte) 113] = 16
        };

    private static Dictionary<byte, int> UncompressedBPP =
        new Dictionary<byte, int>() {
            [(byte) 6] = 2,
            [(byte) 9] = 4,
            [(byte) 10] = 4,
            [(byte) 16] = 1,
            [(byte) 22] = 8,
            [(byte) 105] = 4
        };

    private static Dictionary<byte, string> FourCC =
        new Dictionary<byte, string>() {
            [(byte) 0] = "DXT1",
            [(byte) 1] = "DXT1",
            [(byte) 3] = "DXT3",
            [(byte) 5] = "DXT5",
            [(byte) 22] = "q\0\0\0",
            [(byte) 23] = "DXT5",
            [(byte) 24] = "DXT1",
            [(byte) 25] = "DXT1",
            [(byte) 33] = "DXT5",
            [(byte) 103] = "ATI1",
            [(byte) 104] = "ATI2",
            [(byte) 108] = "DXT1",
            [(byte) 109] = "DXT1",
            [(byte) 110] = "DXT5"
        };

    private static byte[] DX10Formats = new byte[7] {
        (byte) 6,
        (byte) 100,
        (byte) 102,
        (byte) 106,
        (byte) 107,
        (byte) 112,
        (byte) 113
    };

    public static byte[] Headerize(TPF.Texture texture) {
      if (SFEncoding.ASCII.GetString(texture.Bytes, 0, 4) == "DDS ")
        return texture.Bytes;
      DDS dds = new DDS();
      byte format = texture.Format;
      short width = texture.Header.Width;
      short height = texture.Header.Height;
      int mipCount = (int) texture.Mipmaps;
      TPF.TexType type = texture.Type;
      dds.dwFlags = DDS.DDSD.CAPS |
                    DDS.DDSD.HEIGHT |
                    DDS.DDSD.WIDTH |
                    DDS.DDSD.PIXELFORMAT |
                    DDS.DDSD.MIPMAPCOUNT;
      if (Headerizer.CompressedBPB.ContainsKey(format))
        dds.dwFlags |= DDS.DDSD.PITCH;
      else if (Headerizer.UncompressedBPP.ContainsKey(format))
        dds.dwFlags |= DDS.DDSD.LINEARSIZE;
      dds.dwHeight = (int) height;
      dds.dwWidth = (int) width;
      if (Headerizer.CompressedBPB.ContainsKey(format))
        dds.dwPitchOrLinearSize = Math.Max(1, ((int) width + 3) / 4) *
                                  Headerizer.CompressedBPB[format];
      else if (Headerizer.UncompressedBPP.ContainsKey(format))
        dds.dwPitchOrLinearSize =
            ((int) width * Headerizer.UncompressedBPP[format] + 7) / 8;
      if (mipCount == 0)
        mipCount = Headerizer.DetermineMipCount((int) width, (int) height);
      dds.dwMipMapCount = mipCount;
      dds.dwCaps = DDS.DDSCAPS.TEXTURE;
      if (type == TPF.TexType.Cubemap)
        dds.dwCaps |= DDS.DDSCAPS.COMPLEX;
      if (mipCount > 1)
        dds.dwCaps |= DDS.DDSCAPS.COMPLEX | DDS.DDSCAPS.MIPMAP;
      switch (type) {
        case TPF.TexType.Cubemap:
          dds.dwCaps2 = DDS.DDSCAPS2.CUBEMAP |
                        DDS.DDSCAPS2.CUBEMAP_POSITIVEX |
                        DDS.DDSCAPS2.CUBEMAP_NEGATIVEX |
                        DDS.DDSCAPS2.CUBEMAP_POSITIVEY |
                        DDS.DDSCAPS2.CUBEMAP_NEGATIVEY |
                        DDS.DDSCAPS2.CUBEMAP_POSITIVEZ |
                        DDS.DDSCAPS2.CUBEMAP_NEGATIVEZ;
          break;
        case TPF.TexType.Volume:
          dds.dwCaps2 = DDS.DDSCAPS2.VOLUME;
          break;
      }
      DDS.PIXELFORMAT ddspf = dds.ddspf;
      if (Headerizer.FourCC.ContainsKey(format) ||
          ((IEnumerable<byte>) Headerizer.DX10Formats).Contains<byte>(format))
        ddspf.dwFlags = DDS.DDPF.FOURCC;
      switch (format) {
        case 6:
          ddspf.dwFlags |= DDS.DDPF.ALPHAPIXELS | DDS.DDPF.RGB;
          break;
        case 9:
          ddspf.dwFlags |= DDS.DDPF.ALPHAPIXELS | DDS.DDPF.RGB;
          break;
        case 10:
          ddspf.dwFlags |= DDS.DDPF.RGB;
          break;
        case 16:
          ddspf.dwFlags |= DDS.DDPF.ALPHA;
          break;
        case 105:
          ddspf.dwFlags |= DDS.DDPF.ALPHAPIXELS | DDS.DDPF.RGB;
          break;
      }
      if (Headerizer.FourCC.ContainsKey(format))
        ddspf.dwFourCC = Headerizer.FourCC[format];
      else if (((IEnumerable<byte>) Headerizer.DX10Formats).Contains<byte>(
          format))
        ddspf.dwFourCC = "DX10";
      switch (format) {
        case 6:
          ddspf.dwRGBBitCount = 16;
          ddspf.dwRBitMask = 31744U;
          ddspf.dwGBitMask = 992U;
          ddspf.dwBBitMask = 31U;
          ddspf.dwABitMask = 32768U;
          break;
        case 9:
          ddspf.dwRGBBitCount = 32;
          ddspf.dwRBitMask = 16711680U;
          ddspf.dwGBitMask = 65280U;
          ddspf.dwBBitMask = (uint) byte.MaxValue;
          ddspf.dwABitMask = 4278190080U;
          break;
        case 10:
          ddspf.dwRGBBitCount = 24;
          ddspf.dwRBitMask = 16711680U;
          ddspf.dwGBitMask = 65280U;
          ddspf.dwBBitMask = (uint) byte.MaxValue;
          break;
        case 16:
          ddspf.dwRGBBitCount = 8;
          ddspf.dwABitMask = (uint) byte.MaxValue;
          break;
        case 105:
          ddspf.dwRGBBitCount = 32;
          ddspf.dwRBitMask = (uint) byte.MaxValue;
          ddspf.dwGBitMask = 65280U;
          ddspf.dwBBitMask = 16711680U;
          ddspf.dwABitMask = 4278190080U;
          break;
      }
      if (((IEnumerable<byte>) Headerizer.DX10Formats).Contains<byte>(format)) {
        dds.header10 = new DDS.HEADER_DXT10();
        dds.header10.dxgiFormat = (DDS.DXGI_FORMAT) texture.Header.DXGIFormat;
        if (type == TPF.TexType.Cubemap)
          dds.header10.miscFlag = DDS.RESOURCE_MISC.TEXTURECUBE;
      }
      byte[] numArray1 = texture.Bytes;
      int imageCount = type == TPF.TexType.Cubemap ? 6 : 1;
      List<Headerizer.Image> images = (List<Headerizer.Image>) null;
      if (Headerizer.CompressedBPB.ContainsKey(format))
        images = Headerizer.Image.ReadCompressed(
            numArray1,
            (int) width,
            (int) height,
            imageCount,
            mipCount,
            128,
            Headerizer.CompressedBPB[format]);
      else if (Headerizer.UncompressedBPP.ContainsKey(format))
        images = Headerizer.Image.ReadUncompressed(
            numArray1,
            (int) width,
            (int) height,
            imageCount,
            mipCount,
            128,
            Headerizer.UncompressedBPP[format]);
      if (format == (byte) 10) {
        int texelSize = -1;
        int num = -1;
        if (format == (byte) 10) {
          texelSize = 4;
          num = (int) width;
        }
        foreach (Headerizer.Image image in images) {
          for (int index1 = 0; index1 < image.MipLevels.Count; ++index1) {
            byte[] numArray2 = Headerizer.DeswizzlePS3(
                image.MipLevels[index1],
                texelSize,
                num / (int) Math.Pow(2.0, (double) index1));
            if (format == (byte) 10) {
              byte[] numArray3 = new byte[numArray2.Length / 4 * 3];
              for (int index2 = 0; index2 < numArray2.Length / 4; ++index2) {
                Array.Reverse((Array) numArray2, index2 * 4, 4);
                Array.Copy((Array) numArray2,
                           index2 * 4,
                           (Array) numArray3,
                           index2 * 3,
                           3);
              }
              numArray2 = numArray3;
            }
            image.MipLevels[index1] = numArray2;
          }
        }
      }
      if (images != null)
        numArray1 = Headerizer.Image.Write(images);
      return dds.Write(numArray1);
    }

    private static int DetermineMipCount(int width, int height) {
      return (int) Math.Ceiling(
                 Math.Log((double) Math.Max(width, height), 2.0)) +
             1;
    }

    private static byte[] DeswizzlePS3(
        byte[] swizzled,
        int texelSize,
        int texelWidth) {
      byte[] numArray = new byte[swizzled.Length];
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < swizzled.Length / texelSize; ++index) {
        Array.Copy((Array) swizzled,
                   index * texelSize,
                   (Array) numArray,
                   num2 * texelWidth * texelSize + num1 * texelSize,
                   texelSize);
        int num3 = num1 & num2;
        int num4 = num3 ^ num3 + 1;
        int num5 = num1 & num4;
        num1 ^= num4;
        num2 ^= num5;
      }
      return numArray;
    }

    private class Image {
      public List<byte[]> MipLevels;

      public Image() {
        this.MipLevels = new List<byte[]>();
      }

      public static byte[] Write(List<Headerizer.Image> images) {
        BinaryWriterEx binaryWriterEx = new BinaryWriterEx(false);
        foreach (Headerizer.Image image in images) {
          foreach (byte[] mipLevel in image.MipLevels)
            binaryWriterEx.WriteBytes(mipLevel);
        }
        return binaryWriterEx.FinishBytes();
      }

      public static List<Headerizer.Image> ReadUncompressed(
          byte[] bytes,
          int width,
          int height,
          int imageCount,
          int mipCount,
          int padBetween,
          int bytesPerPixel) {
        List<Headerizer.Image> imageList =
            new List<Headerizer.Image>(imageCount);
        BinaryReaderEx binaryReaderEx = new BinaryReaderEx(false, bytes);
        for (int index1 = 0; index1 < imageCount; ++index1) {
          Headerizer.Image image = new Headerizer.Image();
          binaryReaderEx.Pad(padBetween);
          for (int index2 = 0; index2 < mipCount; ++index2) {
            int num1 = (int) Math.Pow(2.0, (double) index2);
            int num2 = width / num1;
            int num3 = height / num1;
            image.MipLevels.Add(
                binaryReaderEx.ReadBytes(num2 * num3 * bytesPerPixel));
          }
          imageList.Add(image);
        }
        return imageList;
      }

      public static List<Headerizer.Image> ReadCompressed(
          byte[] bytes,
          int width,
          int height,
          int imageCount,
          int mipCount,
          int padBetween,
          int bytesPerBlock) {
        List<Headerizer.Image> imageList =
            new List<Headerizer.Image>(imageCount);
        BinaryReaderEx binaryReaderEx = new BinaryReaderEx(false, bytes);
        for (int index1 = 0; index1 < imageCount; ++index1) {
          Headerizer.Image image = new Headerizer.Image();
          binaryReaderEx.Pad(padBetween);
          for (int index2 = 0; index2 < mipCount; ++index2) {
            int num1 = (int) Math.Pow(2.0, (double) index2);
            int num2 = (int) Math.Max(1f, (float) (width / num1) / 4f) *
                       (int) Math.Max(1f, (float) (height / num1) / 4f);
            image.MipLevels.Add(binaryReaderEx.ReadBytes(num2 * bytesPerBlock));
          }
          imageList.Add(image);
        }
        return imageList;
      }
    }
  }
}