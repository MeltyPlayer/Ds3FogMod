// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BTAB
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class BTAB : SoulsFile<BTAB>
  {
    public List<BTAB.Entry> Entries;

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      br.AssertInt32(1);
      br.AssertInt32(new int[1]);
      int capacity = br.ReadInt32();
      int num = br.ReadInt32();
      br.AssertInt32(new int[1]);
      br.AssertInt32(40);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      long position = br.Position;
      br.Position = position + (long) num;
      this.Entries = new List<BTAB.Entry>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Entries.Add(new BTAB.Entry(br, position));
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bw.BigEndian = false;
      bw.WriteInt32(1);
      bw.WriteInt32(0);
      bw.WriteInt32(this.Entries.Count);
      bw.ReserveInt32("NameSize");
      bw.WriteInt32(0);
      bw.WriteInt32(40);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      long position = bw.Position;
      List<int> intList = new List<int>(this.Entries.Count * 2);
      foreach (BTAB.Entry entry in this.Entries)
      {
        int num1 = (int) (bw.Position - position);
        intList.Add(num1);
        bw.WriteUTF16(entry.MSBPartName, true);
        if (num1 % 16 != 0)
        {
          for (int index = 0; index < 16 - num1 % 16; ++index)
            bw.WriteByte((byte) 0);
        }
        int num2 = (int) (bw.Position - position);
        intList.Add(num2);
        bw.WriteUTF16(entry.FLVERMaterialName, true);
        if (num2 % 16 != 0)
        {
          for (int index = 0; index < 16 - num2 % 16; ++index)
            bw.WriteByte((byte) 0);
        }
      }
      bw.FillInt32("NameSize", (int) (bw.Position - position));
      for (int index = 0; index < this.Entries.Count; ++index)
        this.Entries[index].Write(bw, intList[index * 2], intList[index * 2 + 1]);
    }

    public class Entry
    {
      public string MSBPartName;
      public string FLVERMaterialName;
      public int Unk1C;
      public float Unk20;
      public float Unk24;
      public float Unk28;
      public float Unk2C;

      internal Entry(BinaryReaderEx br, long nameStart)
      {
        int num1 = br.ReadInt32();
        this.MSBPartName = br.GetUTF16(nameStart + (long) num1);
        br.AssertInt32(new int[1]);
        int num2 = br.ReadInt32();
        this.FLVERMaterialName = br.GetUTF16(nameStart + (long) num2);
        br.AssertInt32(new int[1]);
        this.Unk1C = br.ReadInt32();
        this.Unk20 = br.ReadSingle();
        this.Unk24 = br.ReadSingle();
        this.Unk28 = br.ReadSingle();
        this.Unk2C = br.ReadSingle();
        br.AssertInt32(new int[1]);
      }

      internal void Write(BinaryWriterEx bw, int nameOffset, int nameOffset2)
      {
        bw.WriteInt32(nameOffset);
        bw.WriteInt32(0);
        bw.WriteInt32(nameOffset2);
        bw.WriteInt32(0);
        bw.WriteInt32(this.Unk1C);
        bw.WriteSingle(this.Unk20);
        bw.WriteSingle(this.Unk24);
        bw.WriteSingle(this.Unk28);
        bw.WriteSingle(this.Unk2C);
        bw.WriteInt32(0);
      }

      public override string ToString()
      {
        return this.MSBPartName + " : " + this.FLVERMaterialName;
      }
    }
  }
}
