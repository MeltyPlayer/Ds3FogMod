// Decompiled with JetBrains decompiler
// Type: SoulsFormats.SoulsFile`1
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public abstract class SoulsFile<TFormat>
      where TFormat : SoulsFile<TFormat>, new() {
    public DCX.Type Compression = DCX.Type.None;

    protected virtual bool Is(BinaryReaderEx br) {
      throw new NotImplementedException(
          "Is is not implemented for this format.");
    }

    public static bool Is(byte[] bytes) {
      return bytes.Length != 0 &&
             new TFormat().Is(SFUtil.GetDecompressedBR(
                                  new BinaryReaderEx(false, bytes),
                                  out DCX.Type _));
    }

    public static bool Is(string path) {
      using (FileStream fileStream = File.OpenRead(path))
        return fileStream.Length != 0L &&
               new TFormat().Is(SFUtil.GetDecompressedBR(
                                    new BinaryReaderEx(
                                        false,
                                        (Stream) fileStream),
                                    out DCX.Type _));
    }

    protected virtual void Read(BinaryReaderEx br) {
      throw new NotImplementedException(
          "Read is not implemented for this format.");
    }

    public static TFormat Read(byte[] bytes) {
      BinaryReaderEx br = new BinaryReaderEx(false, bytes);
      TFormat format = new TFormat();
      BinaryReaderEx decompressedBr =
          SFUtil.GetDecompressedBR(br, out format.Compression);
      format.Read(decompressedBr);
      return format;
    }

    public static TFormat Read(string path) {
      using FileStream fileStream = File.OpenRead(path);
      BinaryReaderEx br = new BinaryReaderEx(false, fileStream);
      TFormat format = new TFormat();
      BinaryReaderEx decompressedBr =
          SFUtil.GetDecompressedBR(br, out format.Compression);
      format.Read(decompressedBr);
      return format;
    }

    private static bool IsRead(BinaryReaderEx br, out TFormat file) {
      TFormat format = new TFormat();
      br = SFUtil.GetDecompressedBR(br, out format.Compression);
      if (format.Is(br)) {
        br.Position = 0L;
        format.Read(br);
        file = format;
        return true;
      }
      file = default(TFormat);
      return false;
    }

    public static bool IsRead(byte[] bytes, out TFormat file) {
      return SoulsFile<TFormat>.IsRead(new BinaryReaderEx(false, bytes),
                                       out file);
    }

    public static bool IsRead(string path, out TFormat file) {
      using (FileStream fileStream = File.OpenRead(path))
        return SoulsFile<TFormat>.IsRead(
            new BinaryReaderEx(false, (Stream) fileStream),
            out file);
    }

    public virtual bool Validate(out Exception ex) {
      ex = (Exception) null;
      return true;
    }

    protected static bool ValidateNull(
        object obj,
        string message,
        out Exception ex) {
      if (obj == null) {
        ex = (Exception) new NullReferenceException(message);
        return false;
      }
      ex = (Exception) null;
      return true;
    }

    protected static bool ValidateIndex(
        long count,
        long index,
        string message,
        out Exception ex) {
      if (index < 0L || index >= count) {
        ex = (Exception) new IndexOutOfRangeException(message);
        return false;
      }
      ex = (Exception) null;
      return true;
    }

    protected virtual void Write(BinaryWriterEx bw) {
      throw new NotImplementedException(
          "Write is not implemented for this format.");
    }

    private void Write(BinaryWriterEx bw, DCX.Type compression) {
      if (compression == DCX.Type.None) {
        this.Write(bw);
      } else {
        BinaryWriterEx bw1 = new BinaryWriterEx(false);
        this.Write(bw1);
        DCX.Compress(bw1.FinishBytes(), bw, compression);
      }
    }

    public byte[] Write() {
      return this.Write(this.Compression);
    }

    public byte[] Write(DCX.Type compression) {
      Exception ex;
      if (!this.Validate(out ex))
        throw ex;
      BinaryWriterEx bw = new BinaryWriterEx(false);
      this.Write(bw, compression);
      return bw.FinishBytes();
    }

    public void Write(string path) {
      this.Write(path, this.Compression);
    }

    public void Write(string path, DCX.Type compression) {
      Exception ex;
      if (!this.Validate(out ex))
        throw ex;
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      using (FileStream fileStream = File.Create(path)) {
        BinaryWriterEx bw = new BinaryWriterEx(false, (Stream) fileStream);
        this.Write(bw, compression);
        bw.Finish();
      }
    }
  }
}