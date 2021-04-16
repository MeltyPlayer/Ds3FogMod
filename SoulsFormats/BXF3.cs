// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BXF3
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class BXF3 : IBinder, IBXF3 {
    public static bool IsBHD(byte[] bytes) {
      return BXF3.IsBHD(
          SFUtil.GetDecompressedBR(new BinaryReaderEx(false, bytes),
                                   out DCX.Type _));
    }

    public static bool IsBHD(string path) {
      using (FileStream fileStream = File.OpenRead(path))
        return BXF3.IsBHD(SFUtil.GetDecompressedBR(
                              new BinaryReaderEx(false, (Stream) fileStream),
                              out DCX.Type _));
    }

    public static bool IsBDT(byte[] bytes) {
      return BXF3.IsBDT(
          SFUtil.GetDecompressedBR(new BinaryReaderEx(false, bytes),
                                   out DCX.Type _));
    }

    public static bool IsBDT(string path) {
      using (FileStream fileStream = File.OpenRead(path))
        return BXF3.IsBDT(SFUtil.GetDecompressedBR(
                              new BinaryReaderEx(false, (Stream) fileStream),
                              out DCX.Type _));
    }

    public static BXF3 Read(byte[] bhdBytes, byte[] bdtBytes) {
      return new BXF3(new BinaryReaderEx(false, bhdBytes),
                      new BinaryReaderEx(false, bdtBytes));
    }

    public static BXF3 Read(byte[] bhdBytes, string bdtPath) {
      using (FileStream fileStream = File.OpenRead(bdtPath))
        return new BXF3(new BinaryReaderEx(false, bhdBytes),
                        new BinaryReaderEx(false, (Stream) fileStream));
    }

    public static BXF3 Read(string bhdPath, byte[] bdtBytes) {
      using (FileStream fileStream = File.OpenRead(bhdPath))
        return new BXF3(new BinaryReaderEx(false, (Stream) fileStream),
                        new BinaryReaderEx(false, bdtBytes));
    }

    public static BXF3 Read(string bhdPath, string bdtPath) {
      using (FileStream fileStream1 = File.OpenRead(bhdPath)) {
        using (FileStream fileStream2 = File.OpenRead(bdtPath))
          return new BXF3(new BinaryReaderEx(false, (Stream) fileStream1),
                          new BinaryReaderEx(false, (Stream) fileStream2));
      }
    }

    public void Write(out byte[] bhdBytes, out byte[] bdtBytes) {
      BinaryWriterEx bhdWriter = new BinaryWriterEx(false);
      BinaryWriterEx bdtWriter = new BinaryWriterEx(false);
      this.Write(bhdWriter, bdtWriter);
      bhdBytes = bhdWriter.FinishBytes();
      bdtBytes = bdtWriter.FinishBytes();
    }

    public void Write(out byte[] bhdBytes, string bdtPath) {
      Directory.CreateDirectory(Path.GetDirectoryName(bdtPath));
      using (FileStream fileStream = File.Create(bdtPath)) {
        BinaryWriterEx bhdWriter = new BinaryWriterEx(false);
        BinaryWriterEx bdtWriter =
            new BinaryWriterEx(false, (Stream) fileStream);
        this.Write(bhdWriter, bdtWriter);
        bdtWriter.Finish();
        bhdBytes = bhdWriter.FinishBytes();
      }
    }

    public void Write(string bhdPath, out byte[] bdtBytes) {
      Directory.CreateDirectory(Path.GetDirectoryName(bhdPath));
      using (FileStream fileStream = File.Create(bhdPath)) {
        BinaryWriterEx bhdWriter =
            new BinaryWriterEx(false, (Stream) fileStream);
        BinaryWriterEx bdtWriter = new BinaryWriterEx(false);
        this.Write(bhdWriter, bdtWriter);
        bhdWriter.Finish();
        bdtBytes = bdtWriter.FinishBytes();
      }
    }

    public void Write(string bhdPath, string bdtPath) {
      Directory.CreateDirectory(Path.GetDirectoryName(bhdPath));
      Directory.CreateDirectory(Path.GetDirectoryName(bdtPath));
      using (FileStream fileStream1 = File.Create(bhdPath)) {
        using (FileStream fileStream2 = File.Create(bdtPath)) {
          BinaryWriterEx bhdWriter =
              new BinaryWriterEx(false, (Stream) fileStream1);
          BinaryWriterEx bdtWriter =
              new BinaryWriterEx(false, (Stream) fileStream2);
          this.Write(bhdWriter, bdtWriter);
          bhdWriter.Finish();
          bdtWriter.Finish();
        }
      }
    }

    public List<BinderFile> Files { get; set; }

    public string Version { get; set; }

    public Binder.Format Format { get; set; }

    public bool BigEndian { get; set; }

    public bool BitBigEndian { get; set; }

    public BXF3() {
      this.Files = new List<BinderFile>();
      this.Version = SFUtil.DateToBinderTimestamp(DateTime.Now);
      this.Format = Binder.Format.IDs |
                    Binder.Format.Names1 |
                    Binder.Format.Names2 |
                    Binder.Format.Compression;
    }

    private static bool IsBHD(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "BHF3";
    }

    private static bool IsBDT(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "BDF3";
    }

    private BXF3(BinaryReaderEx bhdReader, BinaryReaderEx bdtReader) {
      BXF3.ReadBDFHeader(bdtReader);
      List<BinderFileHeader> binderFileHeaderList =
          BXF3.ReadBHFHeader((IBXF3) this, bhdReader);
      this.Files = new List<BinderFile>(binderFileHeaderList.Count);
      foreach (BinderFileHeader binderFileHeader in binderFileHeaderList)
        this.Files.Add(binderFileHeader.ReadFileData(bdtReader));
    }

    internal static void ReadBDFHeader(BinaryReaderEx br) {
      br.AssertASCII("BDF3");
      br.ReadFixStr(8);
      br.AssertInt32(new int[1]);
    }

    internal static List<BinderFileHeader> ReadBHFHeader(
        IBXF3 bxf,
        BinaryReaderEx br) {
      br.AssertASCII("BHF3");
      bxf.Version = br.ReadFixStr(8);
      bxf.BitBigEndian = br.GetBoolean(14L);
      bxf.Format = Binder.ReadFormat(br, bxf.BitBigEndian);
      bxf.BigEndian = br.ReadBoolean();
      br.AssertBoolean(bxf.BitBigEndian);
      int num = (int) br.AssertByte(new byte[1]);
      br.BigEndian = bxf.BigEndian || Binder.ForceBigEndian(bxf.Format);
      int capacity = br.ReadInt32();
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      List<BinderFileHeader> binderFileHeaderList =
          new List<BinderFileHeader>(capacity);
      for (int index = 0; index < capacity; ++index)
        binderFileHeaderList.Add(
            BinderFileHeader.ReadBinder3FileHeader(
                br,
                bxf.Format,
                bxf.BitBigEndian));
      return binderFileHeaderList;
    }

    private void Write(BinaryWriterEx bhdWriter, BinaryWriterEx bdtWriter) {
      List<BinderFileHeader> fileHeaders =
          new List<BinderFileHeader>(this.Files.Count);
      foreach (BinderFile file in this.Files)
        fileHeaders.Add(new BinderFileHeader(file));
      BXF3.WriteBDFHeader((IBXF3) this, bdtWriter);
      BXF3.WriteBHFHeader((IBXF3) this, bhdWriter, fileHeaders);
      for (int index = 0; index < this.Files.Count; ++index)
        fileHeaders[index]
            .WriteBinder3FileData(bhdWriter,
                                  bdtWriter,
                                  this.Format,
                                  index,
                                  this.Files[index].Bytes);
    }

    internal static void WriteBDFHeader(IBXF3 bxf, BinaryWriterEx bw) {
      bw.WriteASCII("BDF3", false);
      bw.WriteFixStr(bxf.Version, 8, (byte) 0);
      bw.WriteInt32(0);
    }

    internal static void WriteBHFHeader(
        IBXF3 bxf,
        BinaryWriterEx bw,
        List<BinderFileHeader> fileHeaders) {
      bw.BigEndian = bxf.BigEndian || Binder.ForceBigEndian(bxf.Format);
      bw.WriteASCII("BHF3", false);
      bw.WriteFixStr(bxf.Version, 8, (byte) 0);
      Binder.WriteFormat(bw, bxf.BitBigEndian, bxf.Format);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(fileHeaders.Count);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      for (int index = 0; index < fileHeaders.Count; ++index)
        fileHeaders[index]
            .WriteBinder3FileHeader(bw, bxf.Format, bxf.BitBigEndian, index);
      for (int index = 0; index < fileHeaders.Count; ++index)
        fileHeaders[index].WriteFileName(bw, bxf.Format, false, index);
    }
  }
}