// Decompiled with JetBrains decompiler
// Type: SoulsFormats.EMELD
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class EMELD : SoulsFile<EMELD>
  {
    public EMEVD.Game Format { get; set; }

    public List<EMELD.Event> Events { get; set; }

    public EMELD()
      : this(EMEVD.Game.DarkSouls1)
    {
    }

    public EMELD(EMEVD.Game format)
    {
      this.Format = format;
      this.Events = new List<EMELD.Event>();
    }

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "ELD\0";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.AssertASCII("ELD\0");
      bool flag1 = br.ReadBoolean();
      bool flag2 = br.AssertSByte((sbyte) 0, (sbyte) -1) == (sbyte) -1;
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      br.BigEndian = flag1;
      br.VarintLong = flag2;
      int num3 = (int) br.AssertInt16((short) 101);
      int num4 = (int) br.AssertInt16((short) 204);
      br.ReadInt32();
      if (!flag1 && !flag2)
        this.Format = EMEVD.Game.DarkSouls1;
      else if (flag1 && !flag2)
      {
        this.Format = EMEVD.Game.DarkSouls1BE;
      }
      else
      {
        if (!(!flag1 & flag2))
          throw new NotSupportedException(string.Format("Unknown EMELD format: BigEndian={0} Is64Bit={1}", (object) flag1, (object) flag2));
        this.Format = EMEVD.Game.Bloodborne;
      }
      long num5 = br.ReadVarint();
      long num6 = br.ReadVarint();
      br.AssertVarint(new long[1]);
      br.ReadVarint();
      br.AssertVarint(new long[1]);
      br.ReadVarint();
      br.ReadVarint();
      long stringsOffset = br.ReadVarint();
      if (!flag2)
      {
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
      }
      br.Position = num6;
      this.Events = new List<EMELD.Event>((int) num5);
      for (int index = 0; (long) index < num5; ++index)
        this.Events.Add(new EMELD.Event(br, this.Format, stringsOffset));
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bool flag1 = this.Format == EMEVD.Game.DarkSouls1BE;
      bool flag2 = this.Format >= EMEVD.Game.Bloodborne;
      bw.WriteASCII("ELD\0", false);
      bw.WriteBoolean(flag1);
      bw.WriteSByte(flag2 ? (sbyte) -1 : (sbyte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.BigEndian = flag1;
      bw.VarintLong = flag2;
      bw.WriteInt16((short) 101);
      bw.WriteInt16((short) 204);
      bw.ReserveInt32("FileSize");
      bw.WriteVarint((long) this.Events.Count);
      bw.ReserveVarint("EventsOffset");
      bw.WriteVarint(0L);
      bw.ReserveVarint("Offset2");
      bw.WriteVarint(0L);
      bw.ReserveVarint("Offset3");
      bw.ReserveVarint("StringsLength");
      bw.ReserveVarint("StringsOffset");
      if (!flag2)
      {
        bw.WriteInt32(0);
        bw.WriteInt32(0);
      }
      bw.FillVarint("EventsOffset", bw.Position);
      for (int index = 0; index < this.Events.Count; ++index)
        this.Events[index].Write(bw, this.Format, index);
      bw.FillVarint("Offset2", bw.Position);
      bw.FillVarint("Offset3", bw.Position);
      long position = bw.Position;
      bw.FillVarint("StringsOffset", bw.Position);
      for (int index = 0; index < this.Events.Count; ++index)
        this.Events[index].WriteName(bw, index, position);
      if ((bw.Position - position) % 16L > 0L)
        bw.WritePattern(16 - (int) (bw.Position - position) % 16, (byte) 0);
      bw.FillVarint("StringsLength", bw.Position - position);
      bw.FillInt32("FileSize", (int) bw.Position);
    }

    public class Event
    {
      public long ID { get; set; }

      public string Name { get; set; }

      public Event(long id, string name)
      {
        this.ID = id;
        this.Name = name;
      }

      internal Event(BinaryReaderEx br, EMEVD.Game format, long stringsOffset)
      {
        this.ID = br.ReadVarint();
        long num = br.ReadVarint();
        if (format < EMEVD.Game.Bloodborne)
          br.AssertInt32(new int[1]);
        this.Name = br.GetUTF16(stringsOffset + num);
      }

      internal void Write(BinaryWriterEx bw, EMEVD.Game format, int index)
      {
        bw.WriteVarint(this.ID);
        bw.ReserveVarint(string.Format("Event{0}NameOffset", (object) index));
        if (format >= EMEVD.Game.Bloodborne)
          return;
        bw.WriteInt32(0);
      }

      internal void WriteName(BinaryWriterEx bw, int index, long stringsOffset)
      {
        bw.FillVarint(string.Format("Event{0}NameOffset", (object) index), bw.Position - stringsOffset);
        bw.WriteUTF16(this.Name, true);
      }
    }
  }
}
