// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BND3
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class BND3 : SoulsFile<BND3>, IBinder, IBND3 {
    public List<BinderFile> Files { get; set; }

    public string Version { get; set; }

    public Binder.Format Format { get; set; }

    public bool BigEndian { get; set; }

    public bool BitBigEndian { get; set; }

    public int Unk18 { get; set; }

    public BND3() {
      this.Files = new List<BinderFile>();
      this.Version = SFUtil.DateToBinderTimestamp(DateTime.Now);
      this.Format = Binder.Format.IDs |
                    Binder.Format.Names1 |
                    Binder.Format.Names2 |
                    Binder.Format.Compression;
    }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == nameof(BND3);
    }

    protected override void Read(BinaryReaderEx br) {
      List<BinderFileHeader> binderFileHeaderList =
          BND3.ReadHeader((IBND3) this, br);
      this.Files = new List<BinderFile>(binderFileHeaderList.Count);
      foreach (BinderFileHeader binderFileHeader in binderFileHeaderList)
        this.Files.Add(binderFileHeader.ReadFileData(br));
    }

    internal static List<BinderFileHeader> ReadHeader(
        IBND3 bnd,
        BinaryReaderEx br) {
      br.AssertASCII(nameof(BND3));
      bnd.Version = br.ReadFixStr(8);
      bnd.BitBigEndian = br.GetBoolean(14L);
      bnd.Format = Binder.ReadFormat(br, bnd.BitBigEndian);
      bnd.BigEndian = br.ReadBoolean();
      br.AssertBoolean(bnd.BitBigEndian);
      int num = (int) br.AssertByte(new byte[1]);
      br.BigEndian = bnd.BigEndian || Binder.ForceBigEndian(bnd.Format);
      int capacity = br.ReadInt32();
      br.ReadInt32();
      bnd.Unk18 = br.AssertInt32(0, int.MinValue);
      br.AssertInt32(new int[1]);
      List<BinderFileHeader> binderFileHeaderList =
          new List<BinderFileHeader>(capacity);
      for (int index = 0; index < capacity; ++index)
        binderFileHeaderList.Add(
            BinderFileHeader.ReadBinder3FileHeader(
                br,
                bnd.Format,
                bnd.BitBigEndian));
      return binderFileHeaderList;
    }

    protected override void Write(BinaryWriterEx bw) {
      List<BinderFileHeader> fileHeaders =
          new List<BinderFileHeader>(this.Files.Count);
      foreach (BinderFile file in this.Files)
        fileHeaders.Add(new BinderFileHeader(file));
      BND3.WriteHeader((IBND3) this, bw, fileHeaders);
      for (int index = 0; index < this.Files.Count; ++index)
        fileHeaders[index]
            .WriteBinder3FileData(bw,
                                  bw,
                                  this.Format,
                                  index,
                                  this.Files[index].Bytes);
    }

    internal static void WriteHeader(
        IBND3 bnd,
        BinaryWriterEx bw,
        List<BinderFileHeader> fileHeaders) {
      bw.BigEndian = bnd.BigEndian || Binder.ForceBigEndian(bnd.Format);
      bw.WriteASCII(nameof(BND3), false);
      bw.WriteFixStr(bnd.Version, 8, (byte) 0);
      Binder.WriteFormat(bw, bnd.BigEndian, bnd.Format);
      bw.WriteBoolean(bnd.BigEndian);
      bw.WriteBoolean(bnd.BitBigEndian);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(fileHeaders.Count);
      bw.ReserveInt32("FileHeadersEnd");
      bw.WriteInt32(bnd.Unk18);
      bw.WriteInt32(0);
      for (int index = 0; index < fileHeaders.Count; ++index)
        fileHeaders[index]
            .WriteBinder3FileHeader(bw, bnd.Format, bnd.BitBigEndian, index);
      for (int index = 0; index < fileHeaders.Count; ++index)
        fileHeaders[index].WriteFileName(bw, bnd.Format, false, index);
      bw.FillInt32("FileHeadersEnd", (int) bw.Position);
    }
  }
}