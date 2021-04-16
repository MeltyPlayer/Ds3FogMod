// Decompiled with JetBrains decompiler
// Type: SoulsFormats.TPF
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class TPF : SoulsFile<TPF>, IEnumerable<TPF.Texture>, IEnumerable
  {
    public List<TPF.Texture> Textures { get; set; }

    public TPF.TPFPlatform Platform { get; set; }

    public byte Encoding { get; set; }

    public byte Flag2 { get; set; }

    public TPF()
    {
      this.Textures = new List<TPF.Texture>();
      this.Platform = TPF.TPFPlatform.PC;
      this.Encoding = (byte) 1;
      this.Flag2 = (byte) 3;
    }

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "TPF\0";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      br.AssertASCII("TPF\0");
      this.Platform = br.GetEnum8<TPF.TPFPlatform>(12L);
      br.BigEndian = this.Platform == TPF.TPFPlatform.Xbox360 || this.Platform == TPF.TPFPlatform.PS3;
      br.ReadInt32();
      int capacity = br.ReadInt32();
      br.Skip(1);
      this.Flag2 = br.AssertByte((byte) 0, (byte) 1, (byte) 2, (byte) 3);
      this.Encoding = br.AssertByte((byte) 0, (byte) 1, (byte) 2);
      int num = (int) br.AssertByte(new byte[1]);
      this.Textures = new List<TPF.Texture>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Textures.Add(new TPF.Texture(br, this.Platform, this.Flag2, this.Encoding));
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bw.BigEndian = this.Platform == TPF.TPFPlatform.Xbox360 || this.Platform == TPF.TPFPlatform.PS3;
      bw.WriteASCII("TPF\0", false);
      bw.ReserveInt32("DataSize");
      bw.WriteInt32(this.Textures.Count);
      bw.WriteByte((byte) this.Platform);
      bw.WriteByte(this.Flag2);
      bw.WriteByte(this.Encoding);
      bw.WriteByte((byte) 0);
      for (int index = 0; index < this.Textures.Count; ++index)
        this.Textures[index].WriteHeader(bw, index, this.Platform, this.Flag2);
      for (int index = 0; index < this.Textures.Count; ++index)
        this.Textures[index].WriteName(bw, index, this.Encoding);
      long position = bw.Position;
      for (int index = 0; index < this.Textures.Count; ++index)
      {
        if (this.Textures[index].Bytes.Length != 0)
          bw.Pad(4);
        this.Textures[index].WriteData(bw, index);
      }
      bw.FillInt32("DataSize", (int) (bw.Position - position));
    }

    public IEnumerator<TPF.Texture> GetEnumerator()
    {
      return (IEnumerator<TPF.Texture>) this.Textures.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public class Texture
    {
      public int Unk24;

      public string Name { get; set; }

      public byte Format { get; set; }

      public TPF.TexType Type { get; set; }

      public byte Mipmaps { get; set; }

      public byte Flags1 { get; set; }

      public int Flags2 { get; set; }

      public int Unk20 { get; set; }

      public float Unk28 { get; set; }

      public byte[] Bytes { get; set; }

      public TPF.TexHeader Header { get; set; }

      public Texture(string name, byte format, byte flags1, int flags2, byte[] bytes)
      {
        this.Name = name;
        this.Format = format;
        this.Flags1 = flags1;
        this.Flags2 = flags2;
        this.Bytes = bytes;
        this.Header = (TPF.TexHeader) null;
        DDS dds = new DDS(bytes);
        this.Type = !dds.dwCaps2.HasFlag((System.Enum) DDS.DDSCAPS2.CUBEMAP) ? (!dds.dwCaps2.HasFlag((System.Enum) DDS.DDSCAPS2.VOLUME) ? TPF.TexType.Texture : TPF.TexType.Volume) : TPF.TexType.Cubemap;
        this.Mipmaps = (byte) dds.dwMipMapCount;
      }

      internal Texture(BinaryReaderEx br, TPF.TPFPlatform platform, byte flag2, byte encoding)
      {
        uint num1 = br.ReadUInt32();
        int count = br.ReadInt32();
        this.Format = br.ReadByte();
        this.Type = br.ReadEnum8<TPF.TexType>();
        this.Mipmaps = br.ReadByte();
        this.Flags1 = br.AssertByte((byte) 0, (byte) 1, (byte) 2, (byte) 3);
        uint num2 = uint.MaxValue;
        if (platform == TPF.TPFPlatform.PC)
        {
          this.Header = (TPF.TexHeader) null;
          num2 = br.ReadUInt32();
          this.Flags2 = br.AssertInt32(0, 1);
        }
        else
        {
          this.Header = new TPF.TexHeader();
          this.Header.Width = br.ReadInt16();
          this.Header.Height = br.ReadInt16();
          if (platform == TPF.TPFPlatform.Xbox360)
          {
            br.AssertInt32(new int[1]);
            num2 = br.ReadUInt32();
            br.AssertInt32(new int[1]);
          }
          else if (platform == TPF.TPFPlatform.PS3)
          {
            this.Header.Unk1 = br.ReadInt32();
            if (flag2 != (byte) 0)
              this.Header.Unk2 = br.AssertInt32(0, 43748);
            num2 = br.ReadUInt32();
            this.Flags2 = br.AssertInt32(0, 1);
            if (this.Flags2 == 1)
            {
              this.Unk20 = br.ReadInt32();
              this.Unk24 = br.ReadInt32();
              this.Unk28 = br.ReadSingle();
            }
          }
          else if (platform == TPF.TPFPlatform.PS4 || platform == TPF.TPFPlatform.Xbone)
          {
            this.Header.TextureCount = br.AssertInt32(1, 6);
            this.Header.Unk2 = br.AssertInt32(13);
            num2 = br.ReadUInt32();
            this.Flags2 = br.AssertInt32(0, 1);
            this.Header.DXGIFormat = br.ReadInt32();
          }
        }
        this.Bytes = br.GetBytes((long) num1, count);
        if (this.Flags1 == (byte) 2 || this.Flags1 == (byte) 3)
        {
          DCX.Type type;
          this.Bytes = DCX.Decompress(this.Bytes, out type);
          if (type != DCX.Type.DCP_EDGE)
            throw new NotImplementedException(string.Format("TPF compression is expected to be DCP_EDGE, but it was {0}", (object) type));
        }
        switch (encoding)
        {
          case 0:
          case 2:
            this.Name = br.GetShiftJIS((long) num2);
            break;
          case 1:
            this.Name = br.GetUTF16((long) num2);
            break;
        }
      }

      internal void WriteHeader(
        BinaryWriterEx bw,
        int index,
        TPF.TPFPlatform platform,
        byte flag2)
      {
        if (platform == TPF.TPFPlatform.PC)
        {
          DDS dds = new DDS(this.Bytes);
          this.Type = !dds.dwCaps2.HasFlag((System.Enum) DDS.DDSCAPS2.CUBEMAP) ? (!dds.dwCaps2.HasFlag((System.Enum) DDS.DDSCAPS2.VOLUME) ? TPF.TexType.Texture : TPF.TexType.Volume) : TPF.TexType.Cubemap;
          this.Mipmaps = (byte) dds.dwMipMapCount;
        }
        bw.ReserveUInt32(string.Format("FileData{0}", (object) index));
        bw.ReserveInt32(string.Format("FileSize{0}", (object) index));
        bw.WriteByte(this.Format);
        bw.WriteByte((byte) this.Type);
        bw.WriteByte(this.Mipmaps);
        bw.WriteByte(this.Flags1);
        if (platform == TPF.TPFPlatform.PC)
        {
          bw.ReserveUInt32(string.Format("FileName{0}", (object) index));
          bw.WriteInt32(this.Flags2);
        }
        else
        {
          bw.WriteInt16(this.Header.Width);
          bw.WriteInt16(this.Header.Height);
          if (platform == TPF.TPFPlatform.Xbox360)
          {
            bw.WriteInt32(0);
            bw.ReserveUInt32(string.Format("FileName{0}", (object) index));
            bw.WriteInt32(0);
          }
          else if (platform == TPF.TPFPlatform.PS3)
          {
            bw.WriteInt32(this.Header.Unk1);
            if (flag2 != (byte) 0)
              bw.WriteInt32(this.Header.Unk2);
            bw.ReserveUInt32(string.Format("FileName{0}", (object) index));
            bw.WriteInt32(this.Flags2);
            if (this.Flags2 != 1)
              return;
            bw.WriteInt32(this.Unk20);
            bw.WriteInt32(this.Unk24);
            bw.WriteSingle(this.Unk28);
          }
          else
          {
            if (platform != TPF.TPFPlatform.PS4 && platform != TPF.TPFPlatform.Xbone)
              return;
            bw.WriteInt32(this.Header.TextureCount);
            bw.WriteInt32(this.Header.Unk2);
            bw.ReserveUInt32(string.Format("FileName{0}", (object) index));
            bw.WriteInt32(this.Flags2);
            bw.WriteInt32(this.Header.DXGIFormat);
          }
        }
      }

      internal void WriteName(BinaryWriterEx bw, int index, byte encoding)
      {
        bw.FillUInt32(string.Format("FileName{0}", (object) index), (uint) bw.Position);
        switch (encoding)
        {
          case 0:
          case 2:
            bw.WriteShiftJIS(this.Name, true);
            break;
          case 1:
            bw.WriteUTF16(this.Name, true);
            break;
        }
      }

      internal void WriteData(BinaryWriterEx bw, int index)
      {
        bw.FillUInt32(string.Format("FileData{0}", (object) index), (uint) bw.Position);
        byte[] numArray = this.Bytes;
        if (this.Flags1 == (byte) 2 || this.Flags2 == 3)
          numArray = DCX.Compress(numArray, DCX.Type.DCP_EDGE);
        bw.FillInt32(string.Format("FileSize{0}", (object) index), numArray.Length);
        bw.WriteBytes(numArray);
      }

      public byte[] Headerize()
      {
        return Headerizer.Headerize(this);
      }

      public override string ToString()
      {
        return string.Format("[{0} {1}] {2}", (object) this.Format, (object) this.Type, (object) this.Name);
      }
    }

    public enum TPFPlatform : byte
    {
      PC = 0,
      Xbox360 = 1,
      PS3 = 2,
      PS4 = 4,
      Xbone = 5,
    }

    public enum TexType : byte
    {
      Texture,
      Cubemap,
      Volume,
    }

    public class TexHeader
    {
      public short Width { get; set; }

      public short Height { get; set; }

      public int TextureCount { get; set; }

      public int Unk1 { get; set; }

      public int Unk2 { get; set; }

      public int DXGIFormat { get; set; }
    }
  }
}
