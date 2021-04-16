// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BHD5
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SoulsFormats {
  [ComVisible(true)]
  public class BHD5 {
    public BHD5.Game Format { get; set; }

    public bool BigEndian { get; set; }

    public bool Unk05 { get; set; }

    public string Salt { get; set; }

    public List<BHD5.Bucket> Buckets { get; set; }

    public static BHD5 Read(Stream bhdStream, BHD5.Game game) {
      return new BHD5(new BinaryReaderEx(false, bhdStream), game);
    }

    public void Write(Stream bhdStream) {
      BinaryWriterEx bw = new BinaryWriterEx(false, bhdStream);
      this.Write(bw);
      bw.Finish();
    }

    public BHD5(BHD5.Game game) {
      this.Format = game;
      this.Salt = "";
      this.Buckets = new List<BHD5.Bucket>();
    }

    private BHD5(BinaryReaderEx br, BHD5.Game game) {
      this.Format = game;
      br.AssertASCII(nameof(BHD5));
      this.BigEndian = br.AssertSByte((sbyte) 0, (sbyte) -1) == (sbyte) 0;
      br.BigEndian = this.BigEndian;
      this.Unk05 = br.ReadBoolean();
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      br.AssertInt32(1);
      br.ReadInt32();
      int capacity = br.ReadInt32();
      int num3 = br.ReadInt32();
      if (game >= BHD5.Game.DarkSouls2) {
        int length = br.ReadInt32();
        this.Salt = br.ReadASCII(length);
      }
      br.Position = (long) num3;
      this.Buckets = new List<BHD5.Bucket>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Buckets.Add(new BHD5.Bucket(br, game));
    }

    private void Write(BinaryWriterEx bw) {
      bw.BigEndian = this.BigEndian;
      bw.WriteASCII(nameof(BHD5), false);
      bw.WriteSByte(this.BigEndian ? (sbyte) 0 : (sbyte) -1);
      bw.WriteBoolean(this.Unk05);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(1);
      bw.ReserveInt32("FileSize");
      bw.WriteInt32(this.Buckets.Count);
      bw.ReserveInt32("BucketsOffset");
      if (this.Format >= BHD5.Game.DarkSouls2) {
        bw.WriteInt32(this.Salt.Length);
        bw.WriteASCII(this.Salt, false);
      }
      bw.FillInt32("BucketsOffset", (int) bw.Position);
      for (int index = 0; index < this.Buckets.Count; ++index)
        this.Buckets[index].Write(bw, index);
      for (int index = 0; index < this.Buckets.Count; ++index)
        this.Buckets[index].WriteFileHeaders(bw, this.Format, index);
      for (int bucketIndex = 0;
           bucketIndex < this.Buckets.Count;
           ++bucketIndex) {
        for (int fileIndex = 0;
             fileIndex < this.Buckets[bucketIndex].Count;
             ++fileIndex)
          this.Buckets[bucketIndex][fileIndex]
              .WriteHashAndKey(bw, this.Format, bucketIndex, fileIndex);
      }
      bw.FillInt32("FileSize", (int) bw.Position);
    }

    public enum Game {
      DarkSouls1,
      DarkSouls2,
      DarkSouls3,
      Sekiro,
    }

    public class Bucket : List<BHD5.FileHeader> {
      public Bucket() {}

      internal Bucket(BinaryReaderEx br, BHD5.Game game) {
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        this.Capacity = num1;
        br.StepIn((long) num2);
        for (int index = 0; index < num1; ++index)
          this.Add(new BHD5.FileHeader(br, game));
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw, int index) {
        bw.WriteInt32(this.Count);
        bw.ReserveInt32(string.Format("FileHeadersOffset{0}", (object) index));
      }

      internal void WriteFileHeaders(
          BinaryWriterEx bw,
          BHD5.Game game,
          int index) {
        bw.FillInt32(string.Format("FileHeadersOffset{0}", (object) index),
                     (int) bw.Position);
        for (int fileIndex = 0; fileIndex < this.Count; ++fileIndex)
          this[fileIndex].Write(bw, game, index, fileIndex);
      }
    }

    public class FileHeader {
      public uint FileNameHash { get; set; }

      public int PaddedFileSize { get; set; }

      public long UnpaddedFileSize { get; set; }

      public long FileOffset { get; set; }

      public BHD5.SHAHash SHAHash { get; set; }

      public BHD5.AESKey AESKey { get; set; }

      public FileHeader() {}

      internal FileHeader(BinaryReaderEx br, BHD5.Game game) {
        this.FileNameHash = br.ReadUInt32();
        this.PaddedFileSize = br.ReadInt32();
        this.FileOffset = br.ReadInt64();
        if (game >= BHD5.Game.DarkSouls2) {
          long offset1 = br.ReadInt64();
          long offset2 = br.ReadInt64();
          if (offset1 != 0L) {
            br.StepIn(offset1);
            this.SHAHash = new BHD5.SHAHash(br);
            br.StepOut();
          }
          if (offset2 != 0L) {
            br.StepIn(offset2);
            this.AESKey = new BHD5.AESKey(br);
            br.StepOut();
          }
        }
        this.UnpaddedFileSize = -1L;
        if (game < BHD5.Game.DarkSouls3)
          return;
        this.UnpaddedFileSize = br.ReadInt64();
      }

      internal void Write(
          BinaryWriterEx bw,
          BHD5.Game game,
          int bucketIndex,
          int fileIndex) {
        bw.WriteUInt32(this.FileNameHash);
        bw.WriteInt32(this.PaddedFileSize);
        bw.WriteInt64(this.FileOffset);
        if (game >= BHD5.Game.DarkSouls2) {
          bw.ReserveInt64(string.Format("SHAHashOffset{0}:{1}",
                                        (object) bucketIndex,
                                        (object) fileIndex));
          bw.ReserveInt64(string.Format("AESKeyOffset{0}:{1}",
                                        (object) bucketIndex,
                                        (object) fileIndex));
        }
        if (game < BHD5.Game.DarkSouls3)
          return;
        bw.WriteInt64(this.UnpaddedFileSize);
      }

      internal void WriteHashAndKey(
          BinaryWriterEx bw,
          BHD5.Game game,
          int bucketIndex,
          int fileIndex) {
        if (game < BHD5.Game.DarkSouls2)
          return;
        if (this.SHAHash == null) {
          bw.FillInt64(string.Format("SHAHashOffset{0}:{1}",
                                     (object) bucketIndex,
                                     (object) fileIndex),
                       0L);
        } else {
          bw.FillInt64(string.Format("SHAHashOffset{0}:{1}",
                                     (object) bucketIndex,
                                     (object) fileIndex),
                       bw.Position);
          this.SHAHash.Write(bw);
        }
        if (this.AESKey == null) {
          bw.FillInt64(string.Format("AESKeyOffset{0}:{1}",
                                     (object) bucketIndex,
                                     (object) fileIndex),
                       0L);
        } else {
          bw.FillInt64(string.Format("AESKeyOffset{0}:{1}",
                                     (object) bucketIndex,
                                     (object) fileIndex),
                       bw.Position);
          this.AESKey.Write(bw);
        }
      }

      public byte[] ReadFile(FileStream bdtStream) {
        byte[] numArray = new byte[this.PaddedFileSize];
        bdtStream.Position = this.FileOffset;
        bdtStream.Read(numArray, 0, this.PaddedFileSize);
        this.AESKey?.Decrypt(numArray);
        return numArray;
      }
    }

    public class SHAHash {
      public byte[] Hash { get; set; }

      public List<BHD5.Range> Ranges { get; set; }

      public SHAHash() {
        this.Hash = new byte[32];
        this.Ranges = new List<BHD5.Range>();
      }

      internal SHAHash(BinaryReaderEx br) {
        this.Hash = br.ReadBytes(32);
        int capacity = br.ReadInt32();
        this.Ranges = new List<BHD5.Range>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Ranges.Add(new BHD5.Range(br));
      }

      internal void Write(BinaryWriterEx bw) {
        if (this.Hash.Length != 32)
          throw new InvalidDataException("SHA hash must be 32 bytes long.");
        bw.WriteBytes(this.Hash);
        bw.WriteInt32(this.Ranges.Count);
        foreach (BHD5.Range range in this.Ranges)
          range.Write(bw);
      }
    }

    public class AESKey {
      private static AesManaged AES;

      public byte[] Key { get; set; }

      public List<BHD5.Range> Ranges { get; set; }

      public AESKey() {
        this.Key = new byte[16];
        this.Ranges = new List<BHD5.Range>();
      }

      internal AESKey(BinaryReaderEx br) {
        this.Key = br.ReadBytes(16);
        int capacity = br.ReadInt32();
        this.Ranges = new List<BHD5.Range>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Ranges.Add(new BHD5.Range(br));
      }

      internal void Write(BinaryWriterEx bw) {
        if (this.Key.Length != 16)
          throw new InvalidDataException("AES key must be 16 bytes long.");
        bw.WriteBytes(this.Key);
        bw.WriteInt32(this.Ranges.Count);
        foreach (BHD5.Range range in this.Ranges)
          range.Write(bw);
      }

      public void Decrypt(byte[] bytes) {
        using (ICryptoTransform decryptor =
            BHD5.AESKey.AES.CreateDecryptor(this.Key, new byte[16])) {
          foreach (BHD5.Range range in this.Ranges.Where<BHD5.Range>(
              (Func<BHD5.Range, bool>)
              (r => r.StartOffset != -1L &&
                    r.EndOffset != -1L &&
                    r.StartOffset !=
                    r.EndOffset))) {
            int startOffset = (int) range.StartOffset;
            int inputCount = (int) (range.EndOffset - range.StartOffset);
            decryptor.TransformBlock(bytes,
                                     startOffset,
                                     inputCount,
                                     bytes,
                                     startOffset);
          }
        }
      }

      static AESKey() {
        AesManaged aesManaged = new AesManaged();
        aesManaged.Mode = CipherMode.ECB;
        aesManaged.Padding = PaddingMode.None;
        aesManaged.KeySize = 128;
        BHD5.AESKey.AES = aesManaged;
      }
    }

    public struct Range {
      public long StartOffset { get; set; }

      public long EndOffset { get; set; }

      public Range(long startOffset, long endOffset) {
        this.StartOffset = startOffset;
        this.EndOffset = endOffset;
      }

      internal Range(BinaryReaderEx br) {
        this.StartOffset = br.ReadInt64();
        this.EndOffset = br.ReadInt64();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt64(this.StartOffset);
        bw.WriteInt64(this.EndOffset);
      }
    }
  }
}