// Decompiled with JetBrains decompiler
// Type: SoulsFormats.AC4.Zero3
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats.AC4
{
  [ComVisible(true)]
  public class Zero3
  {
    public List<Zero3.File> Files { get; }

    public static bool Is(string path)
    {
      using (FileStream fileStream = System.IO.File.OpenRead(path))
      {
        BinaryReaderEx binaryReaderEx = new BinaryReaderEx(true, (Stream) fileStream);
        if (binaryReaderEx.Length < 80L || binaryReaderEx.GetInt32(4L) != 16 || (binaryReaderEx.GetInt32(8L) != 16 || binaryReaderEx.GetInt32(12L) != 8388608))
          return false;
        for (int index = 0; index < 16; ++index)
        {
          if (binaryReaderEx.GetInt32((long) (16 + index * 4)) != 0)
            return false;
        }
        return true;
      }
    }

    public static Zero3 Read(string path)
    {
      List<BinaryReaderEx> containers = new List<BinaryReaderEx>();
      int num = 0;
      for (string path1 = Path.ChangeExtension(path, num.ToString("D3")); System.IO.File.Exists(path1); path1 = Path.ChangeExtension(path, num.ToString("D3")))
      {
        containers.Add(new BinaryReaderEx(true, (Stream) System.IO.File.OpenRead(path1)));
        ++num;
      }
      Zero3 zero3 = new Zero3(containers[0], containers);
      foreach (BinaryReaderEx binaryReaderEx in containers)
        binaryReaderEx.Stream.Close();
      return zero3;
    }

    internal Zero3(BinaryReaderEx br, List<BinaryReaderEx> containers)
    {
      br.BigEndian = true;
      int capacity = br.ReadInt32();
      br.AssertInt32(16);
      br.AssertInt32(16);
      br.AssertInt32(8388608);
      br.AssertPattern(64, (byte) 0);
      this.Files = new List<Zero3.File>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Files.Add(new Zero3.File(br, containers));
    }

    public class File
    {
      public string Name { get; }

      public byte[] Bytes { get; }

      internal File(BinaryReaderEx br, List<BinaryReaderEx> containers)
      {
        this.Name = br.ReadFixStr(64);
        int index = br.ReadInt32();
        uint num = br.ReadUInt32();
        br.ReadInt32();
        int count = br.ReadInt32();
        this.Bytes = containers[index].GetBytes((long) (num * 16U), count);
      }
    }
  }
}
