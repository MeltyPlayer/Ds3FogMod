// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BinderHashTable
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;

namespace SoulsFormats
{
  internal static class BinderHashTable
  {
    public static void Assert(BinaryReaderEx br)
    {
      br.ReadInt64();
      br.ReadInt32();
      int num1 = (int) br.AssertByte((byte) 16);
      int num2 = (int) br.AssertByte((byte) 8);
      int num3 = (int) br.AssertByte((byte) 8);
      int num4 = (int) br.AssertByte(new byte[1]);
    }

    public static void Write(BinaryWriterEx bw, List<BinderFileHeader> files)
    {
      uint num1 = 0;
      for (uint candidate = (uint) files.Count / 7U; candidate <= 100000U; ++candidate)
      {
        if (SFUtil.IsPrime(candidate))
        {
          num1 = candidate;
          break;
        }
      }
      if (num1 == 0U)
        throw new InvalidOperationException("Could not determine hash group count.");
      List<BinderHashTable.PathHash>[] pathHashListArray = new List<BinderHashTable.PathHash>[(int) num1];
      for (int index = 0; (long) index < (long) num1; ++index)
        pathHashListArray[index] = new List<BinderHashTable.PathHash>();
      for (int index = 0; index < files.Count; ++index)
      {
        BinderHashTable.PathHash pathHash = new BinderHashTable.PathHash(index, files[index].Name);
        uint num2 = pathHash.Hash % num1;
        pathHashListArray[(int) num2].Add(pathHash);
      }
      for (int index = 0; (long) index < (long) num1; ++index)
        pathHashListArray[index].Sort((Comparison<BinderHashTable.PathHash>) ((ph1, ph2) => ph1.Hash.CompareTo(ph2.Hash)));
      List<BinderHashTable.HashGroup> hashGroupList = new List<BinderHashTable.HashGroup>();
      List<BinderHashTable.PathHash> pathHashList1 = new List<BinderHashTable.PathHash>();
      int num3 = 0;
      foreach (List<BinderHashTable.PathHash> pathHashList2 in pathHashListArray)
      {
        int index = num3;
        foreach (BinderHashTable.PathHash pathHash in pathHashList2)
        {
          pathHashList1.Add(pathHash);
          ++num3;
        }
        hashGroupList.Add(new BinderHashTable.HashGroup(index, num3 - index));
      }
      bw.ReserveInt64("HashesOffset");
      bw.WriteUInt32(num1);
      bw.WriteByte((byte) 16);
      bw.WriteByte((byte) 8);
      bw.WriteByte((byte) 8);
      bw.WriteByte((byte) 0);
      foreach (BinderHashTable.HashGroup hashGroup in hashGroupList)
        hashGroup.Write(bw);
      bw.FillInt64("HashesOffset", bw.Position);
      foreach (BinderHashTable.PathHash pathHash in pathHashList1)
        pathHash.Write(bw);
    }

    private class PathHash
    {
      public int Index;
      public uint Hash;

      public PathHash(BinaryReaderEx br)
      {
        this.Hash = br.ReadUInt32();
        this.Index = br.ReadInt32();
      }

      public PathHash(int index, string path)
      {
        this.Index = index;
        this.Hash = SFUtil.FromPathHash(path);
      }

      public void Write(BinaryWriterEx bw)
      {
        bw.WriteUInt32(this.Hash);
        bw.WriteInt32(this.Index);
      }
    }

    private class HashGroup
    {
      public int Index;
      public int Length;

      public HashGroup(BinaryReaderEx br)
      {
        this.Length = br.ReadInt32();
        this.Index = br.ReadInt32();
      }

      public HashGroup(int index, int length)
      {
        this.Index = index;
        this.Length = length;
      }

      public void Write(BinaryWriterEx bw)
      {
        bw.WriteInt32(this.Length);
        bw.WriteInt32(this.Index);
      }
    }
  }
}
