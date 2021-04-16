// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BinderFile
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class BinderFile
  {
    public Binder.FileFlags Flags { get; set; }

    public int ID { get; set; }

    public string Name { get; set; }

    public byte[] Bytes { get; set; }

    public DCX.Type CompressionType { get; set; }

    public BinderFile()
      : this(Binder.FileFlags.Flag1, -1, (string) null, new byte[0])
    {
    }

    public BinderFile(Binder.FileFlags flags, byte[] bytes)
      : this(flags, -1, (string) null, bytes)
    {
    }

    public BinderFile(Binder.FileFlags flags, int id, byte[] bytes)
      : this(flags, id, (string) null, bytes)
    {
    }

    public BinderFile(Binder.FileFlags flags, string name, byte[] bytes)
      : this(flags, -1, name, bytes)
    {
    }

    public BinderFile(Binder.FileFlags flags, int id, string name, byte[] bytes)
    {
      this.Flags = flags;
      this.ID = id;
      this.Name = name;
      this.Bytes = bytes;
      this.CompressionType = DCX.Type.Zlib;
    }

    public override string ToString()
    {
      return string.Format("Flags: 0x{0:X2} | ID: {1} | Name: {2} | Length: {3}", (object) (byte) this.Flags, (object) this.ID, (object) this.Name, (object) this.Bytes.Length);
    }
  }
}
