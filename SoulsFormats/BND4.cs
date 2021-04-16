// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BND4
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class BND4 : SoulsFile<BND4>, IBinder, IBND4 {
    public List<BinderFile> Files { get; set; }

    public string Version { get; set; }

    public Binder.Format Format { get; set; }

    public bool Unk04 { get; set; }

    public bool Unk05 { get; set; }

    public bool BigEndian { get; set; }

    public bool BitBigEndian { get; set; }

    public bool Unicode { get; set; }

    public byte Extended { get; set; }

    public BND4() {
      this.Files = new List<BinderFile>();
      this.Version = SFUtil.DateToBinderTimestamp(DateTime.Now);
      this.Format = Binder.Format.IDs |
                    Binder.Format.Names1 |
                    Binder.Format.Names2 |
                    Binder.Format.Compression;
      this.Unicode = true;
      this.Extended = (byte) 4;
    }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == nameof(BND4);
    }

    protected override void Read(BinaryReaderEx br) {
      List<BinderFileHeader> binderFileHeaderList =
          BND4.ReadHeader((IBND4) this, br);
      this.Files = new List<BinderFile>(binderFileHeaderList.Count);
      foreach (BinderFileHeader binderFileHeader in binderFileHeaderList)
        this.Files.Add(binderFileHeader.ReadFileData(br));
    }

    internal static List<BinderFileHeader> ReadHeader(
        IBND4 bnd,
        BinaryReaderEx br) {
      br.AssertASCII(nameof(BND4));
      bnd.Unk04 = br.ReadBoolean();
      bnd.Unk05 = br.ReadBoolean();
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      int num3 = (int) br.AssertByte(new byte[1]);
      bnd.BigEndian = br.ReadBoolean();
      bnd.BitBigEndian = !br.ReadBoolean();
      int num4 = (int) br.AssertByte(new byte[1]);
      br.BigEndian = bnd.BigEndian;
      int capacity = br.ReadInt32();
      br.AssertInt64(64L);
      bnd.Version = br.ReadFixStr(8);
      long num5 = br.ReadInt64();
      br.ReadInt64();
      bnd.Unicode = br.ReadBoolean();
      bnd.Format = Binder.ReadFormat(br, bnd.BitBigEndian);
      bnd.Extended = br.AssertByte((byte) 0, (byte) 1, (byte) 4, (byte) 128);
      int num6 = (int) br.AssertByte(new byte[1]);
      br.AssertInt32(new int[1]);
      if (bnd.Extended == (byte) 4) {
        long offset = br.ReadInt64();
        br.StepIn(offset);
        BinderHashTable.Assert(br);
        br.StepOut();
      } else
        br.AssertInt64(new long[1]);
      if (num5 != Binder.GetBND4FileHeaderSize(bnd.Format))
        throw new FormatException(string.Format(
                                      "File header size for format {0} is expected to be 0x{1:X}, but was 0x{2:X}",
                                      (object) bnd.Format,
                                      (object) Binder.GetBND4FileHeaderSize(
                                          bnd.Format),
                                      (object) num5));
      List<BinderFileHeader> binderFileHeaderList =
          new List<BinderFileHeader>(capacity);
      for (int index = 0; index < capacity; ++index)
        binderFileHeaderList.Add(
            BinderFileHeader.ReadBinder4FileHeader(
                br,
                bnd.Format,
                bnd.BitBigEndian,
                bnd.Unicode));
      return binderFileHeaderList;
    }

    protected override void Write(BinaryWriterEx bw) {
      List<BinderFileHeader> fileHeaders =
          new List<BinderFileHeader>(this.Files.Count);
      foreach (BinderFile file in this.Files)
        fileHeaders.Add(new BinderFileHeader(file));
      BND4.WriteHeader((IBND4) this, bw, fileHeaders);
      for (int index = 0; index < this.Files.Count; ++index)
        fileHeaders[index]
            .WriteBinder4FileData(bw,
                                  bw,
                                  this.Format,
                                  index,
                                  this.Files[index].Bytes);
    }

    internal static void WriteHeader(
        IBND4 bnd,
        BinaryWriterEx bw,
        List<BinderFileHeader> fileHeaders) {
      bw.BigEndian = bnd.BigEndian;
      bw.WriteASCII(nameof(BND4), false);
      bw.WriteBoolean(bnd.Unk04);
      bw.WriteBoolean(bnd.Unk05);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteBoolean(bnd.BigEndian);
      bw.WriteBoolean(!bnd.BitBigEndian);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(fileHeaders.Count);
      bw.WriteInt64(64L);
      bw.WriteFixStr(bnd.Version, 8, (byte) 0);
      bw.WriteInt64(Binder.GetBND4FileHeaderSize(bnd.Format));
      bw.ReserveInt64("HeadersEnd");
      bw.WriteBoolean(bnd.Unicode);
      Binder.WriteFormat(bw, bnd.BitBigEndian, bnd.Format);
      bw.WriteByte(bnd.Extended);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(0);
      bw.ReserveInt64("HashTableOffset");
      for (int index = 0; index < fileHeaders.Count; ++index)
        fileHeaders[index]
            .WriteBinder4FileHeader(bw, bnd.Format, bnd.BitBigEndian, index);
      for (int index = 0; index < fileHeaders.Count; ++index)
        fileHeaders[index].WriteFileName(bw, bnd.Format, bnd.Unicode, index);
      if (bnd.Extended == (byte) 4) {
        bw.Pad(8);
        bw.FillInt64("HashTableOffset", bw.Position);
        BinderHashTable.Write(bw, fileHeaders);
      } else
        bw.FillInt64("HashTableOffset", 0L);
      bw.FillInt64("HeadersEnd", bw.Position);
    }
  }
}