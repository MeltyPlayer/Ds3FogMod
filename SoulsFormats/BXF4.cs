// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BXF4
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class BXF4 : IBinder, IBXF4
  {
    public static bool IsBHD(byte[] bytes)
    {
      return BXF4.IsBHD(SFUtil.GetDecompressedBR(new BinaryReaderEx(false, bytes), out DCX.Type _));
    }

    public static bool IsBHD(string path)
    {
      using (FileStream fileStream = File.OpenRead(path))
        return BXF4.IsBHD(SFUtil.GetDecompressedBR(new BinaryReaderEx(false, (Stream) fileStream), out DCX.Type _));
    }

    public static bool IsBDT(byte[] bytes)
    {
      return BXF4.IsBDT(SFUtil.GetDecompressedBR(new BinaryReaderEx(false, bytes), out DCX.Type _));
    }

    public static bool IsBDT(string path)
    {
      using (FileStream fileStream = File.OpenRead(path))
        return BXF4.IsBDT(SFUtil.GetDecompressedBR(new BinaryReaderEx(false, (Stream) fileStream), out DCX.Type _));
    }

    public static BXF4 Read(byte[] bhdBytes, byte[] bdtBytes)
    {
      return new BXF4(new BinaryReaderEx(false, bhdBytes), new BinaryReaderEx(false, bdtBytes));
    }

    public static BXF4 Read(byte[] bhdBytes, string bdtPath)
    {
      using (FileStream fileStream = File.OpenRead(bdtPath))
        return new BXF4(new BinaryReaderEx(false, bhdBytes), new BinaryReaderEx(false, (Stream) fileStream));
    }

    public static BXF4 Read(string bhdPath, byte[] bdtBytes)
    {
      using (FileStream fileStream = File.OpenRead(bhdPath))
        return new BXF4(new BinaryReaderEx(false, (Stream) fileStream), new BinaryReaderEx(false, bdtBytes));
    }

    public static BXF4 Read(string bhdPath, string bdtPath)
    {
      using (FileStream fileStream1 = File.OpenRead(bhdPath))
      {
        using (FileStream fileStream2 = File.OpenRead(bdtPath))
          return new BXF4(new BinaryReaderEx(false, (Stream) fileStream1), new BinaryReaderEx(false, (Stream) fileStream2));
      }
    }

    public void Write(out byte[] bhdBytes, out byte[] bdtBytes)
    {
      BinaryWriterEx bhdWriter = new BinaryWriterEx(false);
      BinaryWriterEx bdtWriter = new BinaryWriterEx(false);
      this.Write(bhdWriter, bdtWriter);
      bhdBytes = bhdWriter.FinishBytes();
      bdtBytes = bdtWriter.FinishBytes();
    }

    public void Write(out byte[] bhdBytes, string bdtPath)
    {
      Directory.CreateDirectory(Path.GetDirectoryName(bdtPath));
      using (FileStream fileStream = File.Create(bdtPath))
      {
        BinaryWriterEx bhdWriter = new BinaryWriterEx(false);
        BinaryWriterEx bdtWriter = new BinaryWriterEx(false, (Stream) fileStream);
        this.Write(bhdWriter, bdtWriter);
        bdtWriter.Finish();
        bhdBytes = bhdWriter.FinishBytes();
      }
    }

    public void Write(string bhdPath, out byte[] bdtBytes)
    {
      Directory.CreateDirectory(Path.GetDirectoryName(bhdPath));
      using (FileStream fileStream = File.Create(bhdPath))
      {
        BinaryWriterEx bhdWriter = new BinaryWriterEx(false, (Stream) fileStream);
        BinaryWriterEx bdtWriter = new BinaryWriterEx(false);
        this.Write(bhdWriter, bdtWriter);
        bhdWriter.Finish();
        bdtBytes = bdtWriter.FinishBytes();
      }
    }

    public void Write(string bhdPath, string bdtPath)
    {
      Directory.CreateDirectory(Path.GetDirectoryName(bhdPath));
      Directory.CreateDirectory(Path.GetDirectoryName(bdtPath));
      using (FileStream fileStream1 = File.Create(bhdPath))
      {
        using (FileStream fileStream2 = File.Create(bdtPath))
        {
          BinaryWriterEx bhdWriter = new BinaryWriterEx(false, (Stream) fileStream1);
          BinaryWriterEx bdtWriter = new BinaryWriterEx(false, (Stream) fileStream2);
          this.Write(bhdWriter, bdtWriter);
          bhdWriter.Finish();
          bdtWriter.Finish();
        }
      }
    }

    public List<BinderFile> Files { get; set; }

    public string Version { get; set; }

    public Binder.Format Format { get; set; }

    public bool Unk04 { get; set; }

    public bool Unk05 { get; set; }

    public bool BigEndian { get; set; }

    public bool BitBigEndian { get; set; }

    public bool Unicode { get; set; }

    public byte Extended { get; set; }

    public BXF4()
    {
      this.Files = new List<BinderFile>();
      this.Version = SFUtil.DateToBinderTimestamp(DateTime.Now);
      this.Unicode = true;
      this.Format = Binder.Format.IDs | Binder.Format.Names1 | Binder.Format.Names2 | Binder.Format.Compression;
      this.Extended = (byte) 4;
    }

    private static bool IsBHD(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "BHF4";
    }

    private static bool IsBDT(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "BDF4";
    }

    private BXF4(BinaryReaderEx bhdReader, BinaryReaderEx bdtReader)
    {
      BXF4.ReadBDFHeader(bdtReader);
      List<BinderFileHeader> binderFileHeaderList = BXF4.ReadBHFHeader((IBXF4) this, bhdReader);
      this.Files = new List<BinderFile>(binderFileHeaderList.Count);
      foreach (BinderFileHeader binderFileHeader in binderFileHeaderList)
        this.Files.Add(binderFileHeader.ReadFileData(bdtReader));
    }

    internal static void ReadBDFHeader(BinaryReaderEx br)
    {
      br.AssertASCII("BDF4");
      br.ReadBoolean();
      br.ReadBoolean();
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      int num3 = (int) br.AssertByte(new byte[1]);
      br.BigEndian = br.ReadBoolean();
      br.ReadBoolean();
      int num4 = (int) br.AssertByte(new byte[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt64(48L, 64L);
      br.ReadFixStr(8);
      br.AssertInt64(new long[1]);
      br.AssertInt64(new long[1]);
    }

    internal static List<BinderFileHeader> ReadBHFHeader(
      IBXF4 bxf,
      BinaryReaderEx br)
    {
      br.AssertASCII("BHF4");
      bxf.Unk04 = br.ReadBoolean();
      bxf.Unk05 = br.ReadBoolean();
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      int num3 = (int) br.AssertByte(new byte[1]);
      bxf.BigEndian = br.ReadBoolean();
      bxf.BitBigEndian = !br.ReadBoolean();
      int num4 = (int) br.AssertByte(new byte[1]);
      br.BigEndian = bxf.BigEndian;
      int capacity = br.ReadInt32();
      br.AssertInt64(64L);
      bxf.Version = br.ReadFixStr(8);
      long num5 = br.ReadInt64();
      br.AssertInt64(new long[1]);
      bxf.Unicode = br.ReadBoolean();
      bxf.Format = Binder.ReadFormat(br, bxf.BitBigEndian);
      bxf.Extended = br.AssertByte((byte) 0, (byte) 4);
      int num6 = (int) br.AssertByte(new byte[1]);
      if (num5 != Binder.GetBND4FileHeaderSize(bxf.Format))
        throw new FormatException(string.Format("File header size for format {0} is expected to be 0x{1:X}, but was 0x{2:X}", (object) bxf.Format, (object) Binder.GetBND4FileHeaderSize(bxf.Format), (object) num5));
      br.AssertInt32(new int[1]);
      if (bxf.Extended == (byte) 4)
      {
        long offset = br.ReadInt64();
        br.StepIn(offset);
        BinderHashTable.Assert(br);
        br.StepOut();
      }
      else
        br.AssertInt64(new long[1]);
      List<BinderFileHeader> binderFileHeaderList = new List<BinderFileHeader>(capacity);
      for (int index = 0; index < capacity; ++index)
        binderFileHeaderList.Add(BinderFileHeader.ReadBinder4FileHeader(br, bxf.Format, bxf.BitBigEndian, bxf.Unicode));
      return binderFileHeaderList;
    }

    private void Write(BinaryWriterEx bhdWriter, BinaryWriterEx bdtWriter)
    {
      List<BinderFileHeader> fileHeaders = new List<BinderFileHeader>(this.Files.Count);
      foreach (BinderFile file in this.Files)
        fileHeaders.Add(new BinderFileHeader(file));
      BXF4.WriteBDFHeader((IBXF4) this, bdtWriter);
      BXF4.WriteBHFHeader((IBXF4) this, bhdWriter, fileHeaders);
      for (int index = 0; index < this.Files.Count; ++index)
        fileHeaders[index].WriteBinder4FileData(bhdWriter, bdtWriter, this.Format, index, this.Files[index].Bytes);
    }

    internal static void WriteBDFHeader(IBXF4 bxf, BinaryWriterEx bw)
    {
      bw.BigEndian = bxf.BigEndian;
      bw.WriteASCII("BDF4", false);
      bw.WriteBoolean(bxf.Unk04);
      bw.WriteBoolean(bxf.Unk05);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteBoolean(bxf.BigEndian);
      bw.WriteBoolean(!bxf.BitBigEndian);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(0);
      bw.WriteInt64(48L);
      bw.WriteFixStr(bxf.Version, 8, (byte) 0);
      bw.WriteInt64(0L);
      bw.WriteInt64(0L);
    }

    internal static void WriteBHFHeader(
      IBXF4 bxf,
      BinaryWriterEx bw,
      List<BinderFileHeader> fileHeaders)
    {
      bw.BigEndian = bxf.BigEndian;
      bw.WriteASCII("BHF4", false);
      bw.WriteBoolean(bxf.Unk04);
      bw.WriteBoolean(bxf.Unk05);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteBoolean(bxf.BigEndian);
      bw.WriteBoolean(!bxf.BitBigEndian);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(fileHeaders.Count);
      bw.WriteInt64(64L);
      bw.WriteFixStr(bxf.Version, 8, (byte) 0);
      bw.WriteInt64(Binder.GetBND4FileHeaderSize(bxf.Format));
      bw.WriteInt64(0L);
      bw.WriteBoolean(bxf.Unicode);
      Binder.WriteFormat(bw, bxf.BitBigEndian, bxf.Format);
      bw.WriteByte(bxf.Extended);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(0);
      bw.ReserveInt64("HashTableOffset");
      for (int index = 0; index < fileHeaders.Count; ++index)
        fileHeaders[index].WriteBinder4FileHeader(bw, bxf.Format, bxf.BitBigEndian, index);
      for (int index = 0; index < fileHeaders.Count; ++index)
        fileHeaders[index].WriteFileName(bw, bxf.Format, bxf.Unicode, index);
      if (bxf.Extended == (byte) 4)
      {
        bw.Pad(8);
        bw.FillInt64("HashTableOffset", bw.Position);
        BinderHashTable.Write(bw, fileHeaders);
      }
      else
        bw.FillInt64("HashTableOffset", 0L);
    }
  }
}
