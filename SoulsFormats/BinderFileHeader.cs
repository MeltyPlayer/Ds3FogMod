// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BinderFileHeader
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class BinderFileHeader {
    public Binder.FileFlags Flags { get; set; }

    public int ID { get; set; }

    public string Name { get; set; }

    public DCX.Type CompressionType { get; set; }

    public long CompressedSize { get; set; }

    public long UncompressedSize { get; set; }

    public long DataOffset { get; set; }

    public BinderFileHeader(int id, string name)
        : this(Binder.FileFlags.Flag1, id, name) {}

    public BinderFileHeader(Binder.FileFlags flags, int id, string name)
        : this(flags, id, name, -1L, -1L, -1L) {}

    internal BinderFileHeader(BinderFile file)
        : this(file.Flags, file.ID, file.Name, -1L, -1L, -1L) {
      this.CompressionType = file.CompressionType;
    }

    private BinderFileHeader(
        Binder.FileFlags flags,
        int id,
        string name,
        long compressedSize,
        long uncompressedSize,
        long dataOffset) {
      this.Flags = flags;
      this.ID = id;
      this.Name = name;
      this.CompressionType = DCX.Type.Zlib;
      this.CompressedSize = compressedSize;
      this.UncompressedSize = uncompressedSize;
      this.DataOffset = dataOffset;
    }

    public override string ToString() {
      return string.Format("{0} {1}", (object) this.ID, (object) this.Name);
    }

    internal static BinderFileHeader ReadBinder3FileHeader(
        BinaryReaderEx br,
        Binder.Format format,
        bool bitBigEndian) {
      Binder.FileFlags flags = Binder.ReadFileFlags(br, bitBigEndian, format);
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      int num3 = (int) br.AssertByte(new byte[1]);
      int num4 = br.ReadInt32();
      long dataOffset = !Binder.HasLongOffsets(format)
                            ? (long) br.ReadUInt32()
                            : br.ReadInt64();
      int id = -1;
      if (Binder.HasIDs(format))
        id = br.ReadInt32();
      string name = (string) null;
      if (Binder.HasNames(format)) {
        int num5 = br.ReadInt32();
        name = br.GetShiftJIS((long) num5);
      }
      int num6 = -1;
      if (Binder.HasCompression(format))
        num6 = br.ReadInt32();
      return new BinderFileHeader(flags,
                                  id,
                                  name,
                                  (long) num4,
                                  (long) num6,
                                  dataOffset);
    }

    internal static BinderFileHeader ReadBinder4FileHeader(
        BinaryReaderEx br,
        Binder.Format format,
        bool bitBigEndian,
        bool unicode) {
      Binder.FileFlags flags = Binder.ReadFileFlags(br, bitBigEndian, format);
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      int num3 = (int) br.AssertByte(new byte[1]);
      br.AssertInt32(-1);
      long compressedSize = br.ReadInt64();
      long uncompressedSize = -1;
      if (Binder.HasCompression(format))
        uncompressedSize = br.ReadInt64();
      long dataOffset = !Binder.HasLongOffsets(format)
                            ? (long) br.ReadUInt32()
                            : br.ReadInt64();
      int id = -1;
      if (Binder.HasIDs(format))
        id = br.ReadInt32();
      string name = (string) null;
      if (Binder.HasNames(format)) {
        uint num4 = br.ReadUInt32();
        name = !unicode
                   ? br.GetShiftJIS((long) num4)
                   : br.GetUTF16((long) num4);
      }
      return new BinderFileHeader(flags,
                                  id,
                                  name,
                                  compressedSize,
                                  uncompressedSize,
                                  dataOffset);
    }

    internal BinderFile ReadFileData(BinaryReaderEx br) {
      DCX.Type type = DCX.Type.Zlib;
      return new BinderFile(this.Flags,
                            this.ID,
                            this.Name,
                            !Binder.IsCompressed(this.Flags)
                                ? br.GetBytes(this.DataOffset,
                                              (int) this.CompressedSize)
                                : DCX.Decompress(
                                    br.GetBytes(
                                        this.DataOffset,
                                        (int) this.CompressedSize),
                                    out type)) {
          CompressionType = type
      };
    }

    internal void WriteBinder3FileHeader(
        BinaryWriterEx bw,
        Binder.Format format,
        bool bitBigEndian,
        int index) {
      Binder.WriteFileFlags(bw, bitBigEndian, format, this.Flags);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.ReserveInt32(string.Format("FileCompressedSize{0}", (object) index));
      if (Binder.HasLongOffsets(format))
        bw.ReserveInt64(string.Format("FileDataOffset{0}", (object) index));
      else
        bw.ReserveUInt32(string.Format("FileDataOffset{0}", (object) index));
      if (Binder.HasIDs(format))
        bw.WriteInt32(this.ID);
      if (Binder.HasNames(format))
        bw.ReserveInt32(string.Format("FileNameOffset{0}", (object) index));
      if (!Binder.HasCompression(format))
        return;
      bw.ReserveInt32(string.Format("FileUncompressedSize{0}", (object) index));
    }

    internal void WriteBinder4FileHeader(
        BinaryWriterEx bw,
        Binder.Format format,
        bool bitBigEndian,
        int index) {
      Binder.WriteFileFlags(bw, bitBigEndian, format, this.Flags);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(-1);
      bw.ReserveInt64(string.Format("FileCompressedSize{0}", (object) index));
      if (Binder.HasCompression(format))
        bw.ReserveInt64(
            string.Format("FileUncompressedSize{0}", (object) index));
      if (Binder.HasLongOffsets(format))
        bw.ReserveInt64(string.Format("FileDataOffset{0}", (object) index));
      else
        bw.ReserveUInt32(string.Format("FileDataOffset{0}", (object) index));
      if (Binder.HasIDs(format))
        bw.WriteInt32(this.ID);
      if (!Binder.HasNames(format))
        return;
      bw.ReserveInt32(string.Format("FileNameOffset{0}", (object) index));
    }

    private void WriteFileData(BinaryWriterEx bw, byte[] bytes) {
      if (bytes.Length > 0)
        bw.Pad(16);
      this.DataOffset = bw.Position;
      this.UncompressedSize = (long) bytes.Length;
      if (Binder.IsCompressed(this.Flags)) {
        byte[] bytes1 = DCX.Compress(bytes, this.CompressionType);
        this.CompressedSize = (long) bytes1.Length;
        bw.WriteBytes(bytes1);
      } else {
        this.CompressedSize = (long) bytes.Length;
        bw.WriteBytes(bytes);
      }
    }

    internal void WriteBinder3FileData(
        BinaryWriterEx bwHeader,
        BinaryWriterEx bwData,
        Binder.Format format,
        int index,
        byte[] bytes) {
      this.WriteFileData(bwData, bytes);
      bwHeader.FillInt32(string.Format("FileCompressedSize{0}", (object) index),
                         (int) this.CompressedSize);
      if (Binder.HasCompression(format))
        bwHeader.FillInt32(
            string.Format("FileUncompressedSize{0}", (object) index),
            (int) this.UncompressedSize);
      if (Binder.HasLongOffsets(format))
        bwHeader.FillInt64(string.Format("FileDataOffset{0}", (object) index),
                           this.DataOffset);
      else
        bwHeader.FillUInt32(string.Format("FileDataOffset{0}", (object) index),
                            (uint) this.DataOffset);
    }

    internal void WriteBinder4FileData(
        BinaryWriterEx bwHeader,
        BinaryWriterEx bwData,
        Binder.Format format,
        int index,
        byte[] bytes) {
      this.WriteFileData(bwData, bytes);
      bwHeader.FillInt64(string.Format("FileCompressedSize{0}", (object) index),
                         this.CompressedSize);
      if (Binder.HasCompression(format))
        bwHeader.FillInt64(
            string.Format("FileUncompressedSize{0}", (object) index),
            this.UncompressedSize);
      if (Binder.HasLongOffsets(format))
        bwHeader.FillInt64(string.Format("FileDataOffset{0}", (object) index),
                           this.DataOffset);
      else
        bwHeader.FillUInt32(string.Format("FileDataOffset{0}", (object) index),
                            (uint) this.DataOffset);
    }

    internal void WriteFileName(
        BinaryWriterEx bw,
        Binder.Format format,
        bool unicode,
        int index) {
      if (!Binder.HasNames(format))
        return;
      bw.FillInt32(string.Format("FileNameOffset{0}", (object) index),
                   (int) bw.Position);
      if (unicode)
        bw.WriteUTF16(this.Name, true);
      else
        bw.WriteShiftJIS(this.Name, true);
    }
  }
}