// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BinderReader
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public abstract class BinderReader : IDisposable
  {
    protected BinaryReaderEx DataBR;
    private bool disposedValue;

    public string Version { get; set; }

    public Binder.Format Format { get; set; }

    public bool BigEndian { get; set; }

    public bool BitBigEndian { get; set; }

    public List<BinderFileHeader> Files { get; set; }

    public byte[] ReadFile(int index)
    {
      return this.ReadFile(this.Files[index]);
    }

    public byte[] ReadFile(BinderFileHeader fileHeader)
    {
      return fileHeader.ReadFileData(this.DataBR).Bytes;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
        this.DataBR?.Stream?.Dispose();
      this.disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
    }
  }
}
