// Decompiled with JetBrains decompiler
// Type: SoulsFormats.DDS
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class DDS {
    public DDS.DDSD dwFlags;
    public int dwHeight;
    public int dwWidth;
    public int dwPitchOrLinearSize;
    public int dwDepth;
    public int dwMipMapCount;
    public int[] dwReserved1;
    public DDS.PIXELFORMAT ddspf;
    public DDS.DDSCAPS dwCaps;
    public DDS.DDSCAPS2 dwCaps2;
    public int dwCaps3;
    public int dwCaps4;
    public int dwReserved2;
    public DDS.HEADER_DXT10 header10;

    public const DDS.DDSD HEADER_FLAGS_TEXTURE =
        DDS.DDSD.CAPS | DDS.DDSD.HEIGHT | DDS.DDSD.WIDTH | DDS.DDSD.PIXELFORMAT;

    public const DDS.DDSCAPS2 CUBEMAP_ALLFACES =
        DDS.DDSCAPS2.CUBEMAP |
        DDS.DDSCAPS2.CUBEMAP_POSITIVEX |
        DDS.DDSCAPS2.CUBEMAP_NEGATIVEX |
        DDS.DDSCAPS2.CUBEMAP_POSITIVEY |
        DDS.DDSCAPS2.CUBEMAP_NEGATIVEY |
        DDS.DDSCAPS2.CUBEMAP_POSITIVEZ |
        DDS.DDSCAPS2.CUBEMAP_NEGATIVEZ;

    public int DataOffset {
      get { return !(this.ddspf.dwFourCC == "DX10") ? 128 : 148; }
    }

    public DDS() {
      this.dwFlags = DDS.DDSD.CAPS |
                     DDS.DDSD.HEIGHT |
                     DDS.DDSD.WIDTH |
                     DDS.DDSD.PIXELFORMAT;
      this.dwReserved1 = new int[11];
      this.ddspf = new DDS.PIXELFORMAT();
      this.dwCaps = DDS.DDSCAPS.TEXTURE;
    }

    public DDS(byte[] bytes) {
      BinaryReaderEx br = new BinaryReaderEx(false, bytes);
      br.AssertASCII("DDS ");
      br.AssertInt32(124);
      this.dwFlags = (DDS.DDSD) br.ReadUInt32();
      this.dwHeight = br.ReadInt32();
      this.dwWidth = br.ReadInt32();
      this.dwPitchOrLinearSize = br.ReadInt32();
      this.dwDepth = br.ReadInt32();
      this.dwMipMapCount = br.ReadInt32();
      this.dwReserved1 = br.ReadInt32s(11);
      this.ddspf = new DDS.PIXELFORMAT(br);
      this.dwCaps = (DDS.DDSCAPS) br.ReadUInt32();
      this.dwCaps2 = (DDS.DDSCAPS2) br.ReadUInt32();
      this.dwCaps3 = br.ReadInt32();
      this.dwCaps4 = br.ReadInt32();
      this.dwReserved2 = br.ReadInt32();
      if (this.ddspf.dwFourCC == "DX10")
        this.header10 = new DDS.HEADER_DXT10(br);
      else
        this.header10 = (DDS.HEADER_DXT10) null;
    }

    public byte[] Write(byte[] pixelData) {
      BinaryWriterEx bw = new BinaryWriterEx(false);
      bw.WriteASCII("DDS ", false);
      bw.WriteInt32(124);
      bw.WriteUInt32((uint) this.dwFlags);
      bw.WriteInt32(this.dwHeight);
      bw.WriteInt32(this.dwWidth);
      bw.WriteInt32(this.dwPitchOrLinearSize);
      bw.WriteInt32(this.dwDepth);
      bw.WriteInt32(this.dwMipMapCount);
      bw.WriteInt32s((IList<int>) this.dwReserved1);
      this.ddspf.Write(bw);
      bw.WriteUInt32((uint) this.dwCaps);
      bw.WriteUInt32((uint) this.dwCaps2);
      bw.WriteInt32(this.dwCaps3);
      bw.WriteInt32(this.dwCaps4);
      bw.WriteInt32(this.dwReserved2);
      if (this.ddspf.dwFourCC == "DX10")
        this.header10.Write(bw);
      bw.WriteBytes(pixelData);
      return bw.FinishBytes();
    }

    public class PIXELFORMAT {
      public DDS.DDPF dwFlags;
      public string dwFourCC;
      public int dwRGBBitCount;
      public uint dwRBitMask;
      public uint dwGBitMask;
      public uint dwBBitMask;
      public uint dwABitMask;

      public PIXELFORMAT() {
        this.dwFourCC = "\0\0\0\0";
      }

      internal PIXELFORMAT(BinaryReaderEx br) {
        br.AssertInt32(32);
        this.dwFlags = (DDS.DDPF) br.ReadUInt32();
        this.dwFourCC = br.ReadASCII(4);
        this.dwRGBBitCount = br.ReadInt32();
        this.dwRBitMask = br.ReadUInt32();
        this.dwGBitMask = br.ReadUInt32();
        this.dwBBitMask = br.ReadUInt32();
        this.dwABitMask = br.ReadUInt32();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(32);
        bw.WriteUInt32((uint) this.dwFlags);
        bw.WriteASCII((this.dwFourCC ?? "")
                      .PadRight(4, char.MinValue)
                      .Substring(0, 4),
                      false);
        bw.WriteInt32(this.dwRGBBitCount);
        bw.WriteUInt32(this.dwRBitMask);
        bw.WriteUInt32(this.dwGBitMask);
        bw.WriteUInt32(this.dwBBitMask);
        bw.WriteUInt32(this.dwABitMask);
      }
    }

    public class HEADER_DXT10 {
      public DDS.DXGI_FORMAT dxgiFormat;
      public DDS.DIMENSION resourceDimension;
      public DDS.RESOURCE_MISC miscFlag;
      public uint arraySize;
      public DDS.ALPHA_MODE miscFlags2;

      public HEADER_DXT10() {
        this.dxgiFormat = DDS.DXGI_FORMAT.UNKNOWN;
        this.resourceDimension = DDS.DIMENSION.TEXTURE2D;
        this.arraySize = 1U;
        this.miscFlags2 = DDS.ALPHA_MODE.UNKNOWN;
      }

      internal HEADER_DXT10(BinaryReaderEx br) {
        this.dxgiFormat = br.ReadEnum32<DDS.DXGI_FORMAT>();
        this.resourceDimension = br.ReadEnum32<DDS.DIMENSION>();
        this.miscFlag = (DDS.RESOURCE_MISC) br.ReadUInt32();
        this.arraySize = br.ReadUInt32();
        this.miscFlags2 = br.ReadEnum32<DDS.ALPHA_MODE>();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteUInt32((uint) this.dxgiFormat);
        bw.WriteUInt32((uint) this.resourceDimension);
        bw.WriteUInt32((uint) this.miscFlag);
        bw.WriteUInt32(this.arraySize);
        bw.WriteUInt32((uint) this.miscFlags2);
      }
    }

    [Flags]
    public enum DDSD : uint {
      CAPS = 1,
      HEIGHT = 2,
      WIDTH = 4,
      PITCH = 8,
      PIXELFORMAT = 4096,   // 0x00001000
      MIPMAPCOUNT = 131072, // 0x00020000
      LINEARSIZE = 524288,  // 0x00080000
      DEPTH = 8388608,      // 0x00800000
    }

    [Flags]
    public enum DDSCAPS : uint {
      COMPLEX = 8,
      TEXTURE = 4096,   // 0x00001000
      MIPMAP = 4194304, // 0x00400000
    }

    [Flags]
    public enum DDSCAPS2 : uint {
      CUBEMAP = 512,             // 0x00000200
      CUBEMAP_POSITIVEX = 1024,  // 0x00000400
      CUBEMAP_NEGATIVEX = 2048,  // 0x00000800
      CUBEMAP_POSITIVEY = 4096,  // 0x00001000
      CUBEMAP_NEGATIVEY = 8192,  // 0x00002000
      CUBEMAP_POSITIVEZ = 16384, // 0x00004000
      CUBEMAP_NEGATIVEZ = 32768, // 0x00008000
      VOLUME = 2097152,          // 0x00200000
    }

    [Flags]
    public enum DDPF : uint {
      ALPHAPIXELS = 1,
      ALPHA = 2,
      FOURCC = 4,
      RGB = 64,           // 0x00000040
      YUV = 512,          // 0x00000200
      LUMINANCE = 131072, // 0x00020000
    }

    public enum DIMENSION : uint {
      TEXTURE1D = 2,
      TEXTURE2D = 3,
      TEXTURE3D = 4,
    }

    [Flags]
    public enum RESOURCE_MISC : uint {
      TEXTURECUBE = 4,
    }

    public enum ALPHA_MODE : uint {
      UNKNOWN,
      STRAIGHT,
      PREMULTIPLIED,
      OPAQUE,
      CUSTOM,
    }

    public enum DXGI_FORMAT : uint {
      UNKNOWN,
      R32G32B32A32_TYPELESS,
      R32G32B32A32_FLOAT,
      R32G32B32A32_UINT,
      R32G32B32A32_SINT,
      R32G32B32_TYPELESS,
      R32G32B32_FLOAT,
      R32G32B32_UINT,
      R32G32B32_SINT,
      R16G16B16A16_TYPELESS,
      R16G16B16A16_FLOAT,
      R16G16B16A16_UNORM,
      R16G16B16A16_UINT,
      R16G16B16A16_SNORM,
      R16G16B16A16_SINT,
      R32G32_TYPELESS,
      R32G32_FLOAT,
      R32G32_UINT,
      R32G32_SINT,
      R32G8X24_TYPELESS,
      D32_FLOAT_S8X24_UINT,
      R32_FLOAT_X8X24_TYPELESS,
      X32_TYPELESS_G8X24_UINT,
      R10G10B10A2_TYPELESS,
      R10G10B10A2_UNORM,
      R10G10B10A2_UINT,
      R11G11B10_FLOAT,
      R8G8B8A8_TYPELESS,
      R8G8B8A8_UNORM,
      R8G8B8A8_UNORM_SRGB,
      R8G8B8A8_UINT,
      R8G8B8A8_SNORM,
      R8G8B8A8_SINT,
      R16G16_TYPELESS,
      R16G16_FLOAT,
      R16G16_UNORM,
      R16G16_UINT,
      R16G16_SNORM,
      R16G16_SINT,
      R32_TYPELESS,
      D32_FLOAT,
      R32_FLOAT,
      R32_UINT,
      R32_SINT,
      R24G8_TYPELESS,
      D24_UNORM_S8_UINT,
      R24_UNORM_X8_TYPELESS,
      X24_TYPELESS_G8_UINT,
      R8G8_TYPELESS,
      R8G8_UNORM,
      R8G8_UINT,
      R8G8_SNORM,
      R8G8_SINT,
      R16_TYPELESS,
      R16_FLOAT,
      D16_UNORM,
      R16_UNORM,
      R16_UINT,
      R16_SNORM,
      R16_SINT,
      R8_TYPELESS,
      R8_UNORM,
      R8_UINT,
      R8_SNORM,
      R8_SINT,
      A8_UNORM,
      R1_UNORM,
      R9G9B9E5_SHAREDEXP,
      R8G8_B8G8_UNORM,
      G8R8_G8B8_UNORM,
      BC1_TYPELESS,
      BC1_UNORM,
      BC1_UNORM_SRGB,
      BC2_TYPELESS,
      BC2_UNORM,
      BC2_UNORM_SRGB,
      BC3_TYPELESS,
      BC3_UNORM,
      BC3_UNORM_SRGB,
      BC4_TYPELESS,
      BC4_UNORM,
      BC4_SNORM,
      BC5_TYPELESS,
      BC5_UNORM,
      BC5_SNORM,
      B5G6R5_UNORM,
      B5G5R5A1_UNORM,
      B8G8R8A8_UNORM,
      B8G8R8X8_UNORM,
      R10G10B10_XR_BIAS_A2_UNORM,
      B8G8R8A8_TYPELESS,
      B8G8R8A8_UNORM_SRGB,
      B8G8R8X8_TYPELESS,
      B8G8R8X8_UNORM_SRGB,
      BC6H_TYPELESS,
      BC6H_UF16,
      BC6H_SF16,
      BC7_TYPELESS,
      BC7_UNORM,
      BC7_UNORM_SRGB,
      AYUV,
      Y410,
      Y416,
      NV12,
      P010,
      P016,
      OPAQUE_420,
      YUY2,
      Y210,
      Y216,
      NV11,
      AI44,
      IA44,
      P8,
      A8P8,
      B4G4R4A4_UNORM,
      P208,
      V208,
      V408,
      FORCE_UINT,
    }
  }
}