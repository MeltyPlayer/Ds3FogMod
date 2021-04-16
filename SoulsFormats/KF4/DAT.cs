// Decompiled with JetBrains decompiler
// Type: SoulsFormats.KF4.DAT
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats.KF4
{
  [ComVisible(true)]
  public class DAT : SoulsFile<DAT>
  {
    public List<DAT.File> Files;

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte((byte) 128);
      int num3 = (int) br.AssertByte((byte) 4);
      int num4 = (int) br.AssertByte((byte) 30);
      int capacity = br.ReadInt32();
      for (int index = 0; index < 56; ++index)
      {
        int num5 = (int) br.AssertByte(new byte[1]);
      }
      this.Files = new List<DAT.File>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Files.Add(new DAT.File(br));
    }

    public class File
    {
      public string Name;
      public byte[] Bytes;

      internal File(BinaryReaderEx br)
      {
        this.Name = br.ReadFixStr(52);
        int count = br.ReadInt32();
        br.ReadInt32();
        int num = br.ReadInt32();
        this.Bytes = br.GetBytes((long) num, count);
      }
    }
  }
}
