// Decompiled with JetBrains decompiler
// Type: SoulsFormats.EMEVD
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class EMEVD : SoulsFile<EMEVD>
  {
    public EMEVD.Game Format { get; set; }

    public List<EMEVD.Event> Events { get; set; }

    public List<long> LinkedFileOffsets { get; set; }

    public byte[] StringData { get; set; }

    public EMEVD()
      : this(EMEVD.Game.DarkSouls1)
    {
    }

    public EMEVD(EMEVD.Game format)
    {
      this.Format = format;
      this.Events = new List<EMEVD.Event>();
      this.LinkedFileOffsets = new List<long>();
      this.StringData = new byte[0];
    }

    public void ImportEMELD(EMELD eld, bool overwrite = false)
    {
      Dictionary<long, string> dictionary = new Dictionary<long, string>(eld.Events.Count);
      foreach (EMELD.Event @event in eld.Events)
        dictionary[@event.ID] = @event.Name;
      foreach (EMEVD.Event @event in this.Events)
      {
        if ((overwrite || @event.Name == null) && dictionary.ContainsKey(@event.ID))
          @event.Name = dictionary[@event.ID];
      }
    }

    public EMELD ExportEMELD()
    {
      EMELD emeld = new EMELD(this.Format);
      foreach (EMEVD.Event @event in this.Events)
      {
        if (@event.Name != null)
          emeld.Events.Add(new EMELD.Event(@event.ID, @event.Name));
      }
      return emeld;
    }

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "EVD\0";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.AssertASCII("EVD\0");
      bool flag1 = br.ReadBoolean();
      bool flag2 = br.AssertSByte((sbyte) 0, (sbyte) -1) == (sbyte) -1;
      bool flag3 = br.ReadBoolean();
      bool flag4 = br.AssertSByte((sbyte) 0, (sbyte) -1) == (sbyte) -1;
      br.BigEndian = flag1;
      br.VarintLong = flag2;
      int num1 = br.AssertInt32(204, 205);
      br.ReadInt32();
      if (!flag1 && !flag2 && (!flag3 && !flag4) && num1 == 204)
        this.Format = EMEVD.Game.DarkSouls1;
      else if (flag1 && !flag2 && (!flag3 && !flag4) && num1 == 204)
        this.Format = EMEVD.Game.DarkSouls1BE;
      else if (!flag1 & flag2 && !flag3 && (!flag4 && num1 == 204))
        this.Format = EMEVD.Game.Bloodborne;
      else if (!flag1 & flag2 & flag3 && !flag4 && num1 == 205)
        this.Format = EMEVD.Game.DarkSouls3;
      else if (!flag1 & flag2 & flag3 & flag4 && num1 == 205)
        this.Format = EMEVD.Game.Sekiro;
      else
        throw new NotSupportedException(string.Format("Unknown EMEVD format: BigEndian={0} Is64Bit={1} Unicode={2} Unk07={3} Version=0x{4:X}", (object) flag1, (object) flag2, (object) flag3, (object) flag4, (object) num1));
      long num2 = br.ReadVarint();
      EMEVD.Offsets offsets;
      offsets.Events = br.ReadVarint();
      br.ReadVarint();
      offsets.Instructions = br.ReadVarint();
      br.AssertVarint(new long[1]);
      br.ReadVarint();
      br.ReadVarint();
      offsets.Layers = br.ReadVarint();
      br.ReadVarint();
      offsets.Parameters = br.ReadVarint();
      long num3 = br.ReadVarint();
      offsets.LinkedFiles = br.ReadVarint();
      br.ReadVarint();
      offsets.Arguments = br.ReadVarint();
      long num4 = br.ReadVarint();
      offsets.Strings = br.ReadVarint();
      if (!flag2)
        br.AssertInt32(new int[1]);
      br.Position = offsets.Events;
      this.Events = new List<EMEVD.Event>((int) num2);
      for (int index = 0; (long) index < num2; ++index)
        this.Events.Add(new EMEVD.Event(br, this.Format, offsets));
      br.Position = offsets.LinkedFiles;
      this.LinkedFileOffsets = new List<long>((IEnumerable<long>) br.ReadVarints((int) num3));
      br.Position = offsets.Strings;
      this.StringData = br.ReadBytes((int) num4);
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bool flag1 = this.Format == EMEVD.Game.DarkSouls1BE;
      bool flag2 = this.Format >= EMEVD.Game.Bloodborne;
      bool flag3 = this.Format >= EMEVD.Game.DarkSouls3;
      bool flag4 = this.Format >= EMEVD.Game.Sekiro;
      int num1 = this.Format < EMEVD.Game.DarkSouls3 ? 204 : 205;
      List<uint> uintList1 = new List<uint>();
      foreach (EMEVD.Event @event in this.Events)
      {
        foreach (EMEVD.Instruction instruction in @event.Instructions)
        {
          uint? layer = instruction.Layer;
          if (layer.HasValue)
          {
            List<uint> uintList2 = uintList1;
            layer = instruction.Layer;
            int num2 = (int) layer.Value;
            if (!uintList2.Contains((uint) num2))
            {
              List<uint> uintList3 = uintList1;
              layer = instruction.Layer;
              int num3 = (int) layer.Value;
              uintList3.Add((uint) num3);
            }
          }
        }
      }
      bw.WriteASCII("EVD\0", false);
      bw.WriteBoolean(flag1);
      bw.WriteSByte(flag2 ? (sbyte) -1 : (sbyte) 0);
      bw.WriteBoolean(flag3);
      bw.WriteSByte(flag4 ? (sbyte) -1 : (sbyte) 0);
      bw.BigEndian = flag1;
      bw.VarintLong = flag2;
      bw.WriteInt32(num1);
      bw.ReserveInt32("FileSize");
      EMEVD.Offsets offsets = new EMEVD.Offsets();
      bw.WriteVarint((long) this.Events.Count);
      bw.ReserveVarint("EventsOffset");
      bw.WriteVarint((long) this.Events.Sum<EMEVD.Event>((Func<EMEVD.Event, int>) (e => e.Instructions.Count)));
      bw.ReserveVarint("InstructionsOffset");
      bw.WriteVarint(0L);
      bw.ReserveVarint("Offset3");
      bw.WriteVarint((long) uintList1.Count);
      bw.ReserveVarint("LayersOffset");
      bw.WriteVarint((long) this.Events.Sum<EMEVD.Event>((Func<EMEVD.Event, int>) (e => e.Parameters.Count)));
      bw.ReserveVarint("ParametersOffset");
      bw.WriteVarint((long) this.LinkedFileOffsets.Count);
      bw.ReserveVarint("LinkedFilesOffset");
      bw.ReserveVarint("ArgumentsLength");
      bw.ReserveVarint("ArgumentsOffset");
      bw.WriteVarint((long) this.StringData.Length);
      bw.ReserveVarint("StringsOffset");
      if (!flag2)
        bw.WriteInt32(0);
      offsets.Events = bw.Position;
      bw.FillVarint("EventsOffset", bw.Position);
      for (int eventIndex = 0; eventIndex < this.Events.Count; ++eventIndex)
        this.Events[eventIndex].Write(bw, this.Format, eventIndex);
      offsets.Instructions = bw.Position;
      bw.FillVarint("InstructionsOffset", bw.Position);
      for (int eventIndex = 0; eventIndex < this.Events.Count; ++eventIndex)
        this.Events[eventIndex].WriteInstructions(bw, this.Format, offsets, eventIndex);
      bw.FillVarint("Offset3", bw.Position);
      offsets.Layers = bw.Position;
      bw.FillVarint("LayersOffset", bw.Position);
      Dictionary<uint, long> layerOffsets = new Dictionary<uint, long>(uintList1.Count);
      foreach (uint layer in uintList1)
      {
        layerOffsets[layer] = bw.Position - offsets.Layers;
        EMEVD.Layer.Write(bw, layer);
      }
      for (int eventIndex = 0; eventIndex < this.Events.Count; ++eventIndex)
      {
        EMEVD.Event @event = this.Events[eventIndex];
        for (int instrIndex = 0; instrIndex < @event.Instructions.Count; ++instrIndex)
          @event.Instructions[instrIndex].FillLayerOffset(bw, this.Format, eventIndex, instrIndex, layerOffsets);
      }
      offsets.Arguments = bw.Position;
      bw.FillVarint("ArgumentsOffset", bw.Position);
      for (int eventIndex = 0; eventIndex < this.Events.Count; ++eventIndex)
      {
        EMEVD.Event @event = this.Events[eventIndex];
        for (int instrIndex = 0; instrIndex < @event.Instructions.Count; ++instrIndex)
          @event.Instructions[instrIndex].WriteArgs(bw, this.Format, offsets, eventIndex, instrIndex);
      }
      if ((bw.Position - offsets.Arguments) % 16L > 0L)
        bw.WritePattern(16 - (int) (bw.Position - offsets.Arguments) % 16, (byte) 0);
      bw.FillVarint("ArgumentsLength", bw.Position - offsets.Arguments);
      offsets.Parameters = bw.Position;
      bw.FillVarint("ParametersOffset", bw.Position);
      for (int eventIndex = 0; eventIndex < this.Events.Count; ++eventIndex)
        this.Events[eventIndex].WriteParameters(bw, this.Format, offsets, eventIndex);
      offsets.LinkedFiles = bw.Position;
      bw.FillVarint("LinkedFilesOffset", bw.Position);
      foreach (long linkedFileOffset in this.LinkedFileOffsets)
        bw.WriteVarint((long) (int) linkedFileOffset);
      offsets.Strings = bw.Position;
      bw.FillVarint("StringsOffset", bw.Position);
      bw.WriteBytes(this.StringData);
      bw.FillInt32("FileSize", (int) bw.Position);
    }

    public enum Game
    {
      DarkSouls1,
      DarkSouls1BE,
      Bloodborne,
      DarkSouls3,
      Sekiro,
    }

    internal struct Offsets
    {
      public long Events;
      public long Instructions;
      public long Layers;
      public long Parameters;
      public long LinkedFiles;
      public long Arguments;
      public long Strings;
    }

    public class Event
    {
      public long ID { get; set; }

      public List<EMEVD.Instruction> Instructions { get; set; }

      public List<EMEVD.Parameter> Parameters { get; set; }

      public EMEVD.Event.RestBehaviorType RestBehavior { get; set; }

      public string Name { get; set; }

      public Event(long id = 0, EMEVD.Event.RestBehaviorType restBehavior = EMEVD.Event.RestBehaviorType.Default)
      {
        this.ID = id;
        this.Instructions = new List<EMEVD.Instruction>();
        this.Parameters = new List<EMEVD.Parameter>();
        this.RestBehavior = restBehavior;
      }

      internal Event(BinaryReaderEx br, EMEVD.Game format, EMEVD.Offsets offsets)
      {
        this.ID = br.ReadVarint();
        long num1 = br.ReadVarint();
        long num2 = br.ReadVarint();
        long num3 = br.ReadVarint();
        long num4 = br.ReadVarint();
        this.RestBehavior = br.ReadEnum32<EMEVD.Event.RestBehaviorType>();
        br.AssertInt32(new int[1]);
        this.Instructions = new List<EMEVD.Instruction>((int) num1);
        if (num1 > 0L)
        {
          br.StepIn(offsets.Instructions + num2);
          for (int index = 0; (long) index < num1; ++index)
            this.Instructions.Add(new EMEVD.Instruction(br, format, offsets));
          br.StepOut();
        }
        this.Parameters = new List<EMEVD.Parameter>((int) num3);
        if (num3 <= 0L)
          return;
        br.StepIn(offsets.Parameters + num4);
        for (int index = 0; (long) index < num3; ++index)
          this.Parameters.Add(new EMEVD.Parameter(br, format));
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw, EMEVD.Game format, int eventIndex)
      {
        bw.WriteVarint(this.ID);
        bw.WriteVarint((long) this.Instructions.Count);
        bw.ReserveVarint(string.Format("Event{0}InstrsOffset", (object) eventIndex));
        bw.WriteVarint((long) this.Parameters.Count);
        if (format < EMEVD.Game.Bloodborne)
          bw.ReserveInt32(string.Format("Event{0}ParamsOffset", (object) eventIndex));
        else if (format < EMEVD.Game.DarkSouls3)
        {
          bw.ReserveInt32(string.Format("Event{0}ParamsOffset", (object) eventIndex));
          bw.WriteInt32(0);
        }
        else
          bw.ReserveInt64(string.Format("Event{0}ParamsOffset", (object) eventIndex));
        bw.WriteUInt32((uint) this.RestBehavior);
        bw.WriteInt32(0);
      }

      internal void WriteInstructions(
        BinaryWriterEx bw,
        EMEVD.Game format,
        EMEVD.Offsets offsets,
        int eventIndex)
      {
        long num = this.Instructions.Count > 0 ? bw.Position - offsets.Instructions : -1L;
        bw.FillVarint(string.Format("Event{0}InstrsOffset", (object) eventIndex), num);
        for (int instrIndex = 0; instrIndex < this.Instructions.Count; ++instrIndex)
          this.Instructions[instrIndex].Write(bw, format, eventIndex, instrIndex);
      }

      internal void WriteParameters(
        BinaryWriterEx bw,
        EMEVD.Game format,
        EMEVD.Offsets offsets,
        int eventIndex)
      {
        long num = this.Parameters.Count > 0 ? bw.Position - offsets.Parameters : -1L;
        if (format < EMEVD.Game.DarkSouls3)
          bw.FillInt32(string.Format("Event{0}ParamsOffset", (object) eventIndex), (int) num);
        else
          bw.FillInt64(string.Format("Event{0}ParamsOffset", (object) eventIndex), num);
        for (int index = 0; index < this.Parameters.Count; ++index)
          this.Parameters[index].Write(bw, format);
      }

      public enum RestBehaviorType : uint
      {
        Default,
        Restart,
        End,
      }
    }

    public class Instruction
    {
      public int Bank { get; set; }

      public int ID { get; set; }

      public byte[] ArgData { get; set; }

      public uint? Layer { get; set; }

      public Instruction()
      {
        this.Bank = 0;
        this.ID = 0;
        this.Layer = new uint?();
        this.ArgData = new byte[0];
      }

      public Instruction(int bank, int id)
      {
        this.Bank = bank;
        this.ID = id;
        this.Layer = new uint?();
        this.ArgData = new byte[0];
      }

      public Instruction(int bank, int id, byte[] args)
      {
        this.Bank = bank;
        this.ID = id;
        this.Layer = new uint?();
        this.ArgData = args;
      }

      public Instruction(int bank, int id, IEnumerable<object> args)
      {
        this.Bank = bank;
        this.ID = id;
        this.Layer = new uint?();
        this.PackArgs(args, false);
      }

      public Instruction(int bank, int id, params object[] args)
      {
        this.Bank = bank;
        this.ID = id;
        this.Layer = new uint?();
        this.PackArgs((IEnumerable<object>) args, false);
      }

      public Instruction(int bank, int id, uint layerMask, byte[] args)
      {
        this.Bank = bank;
        this.ID = id;
        this.Layer = new uint?(layerMask);
        this.ArgData = args;
      }

      public Instruction(int bank, int id, uint layerMask, IEnumerable<object> args)
      {
        this.Bank = bank;
        this.ID = id;
        this.Layer = new uint?(layerMask);
        this.PackArgs(args, false);
      }

      public Instruction(int bank, int id, uint layerMask, params object[] args)
      {
        this.Bank = bank;
        this.ID = id;
        this.Layer = new uint?(layerMask);
        this.PackArgs((IEnumerable<object>) args, false);
      }

      internal Instruction(BinaryReaderEx br, EMEVD.Game format, EMEVD.Offsets offsets)
      {
        this.Bank = br.ReadInt32();
        this.ID = br.ReadInt32();
        long num1 = br.ReadVarint();
        long num2 = br.ReadVarint();
        long num3;
        if (format < EMEVD.Game.DarkSouls3)
        {
          num3 = (long) br.ReadInt32();
          br.AssertInt32(new int[1]);
        }
        else
          num3 = br.ReadInt64();
        this.ArgData = num1 <= 0L ? new byte[0] : br.GetBytes(offsets.Arguments + num2, (int) num1);
        if (num3 == -1L)
          return;
        br.StepIn(offsets.Layers + num3);
        this.Layer = new uint?(EMEVD.Layer.Read(br));
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw, EMEVD.Game format, int eventIndex, int instrIndex)
      {
        bw.WriteInt32(this.Bank);
        bw.WriteInt32(this.ID);
        bw.WriteVarint((long) this.ArgData.Length);
        if (format < EMEVD.Game.Bloodborne)
          bw.ReserveInt32(string.Format("Event{0}Instr{1}ArgsOffset", (object) eventIndex, (object) instrIndex));
        else if (format < EMEVD.Game.Sekiro)
        {
          bw.ReserveInt32(string.Format("Event{0}Instr{1}ArgsOffset", (object) eventIndex, (object) instrIndex));
          bw.WriteInt32(0);
        }
        else
          bw.ReserveInt64(string.Format("Event{0}Instr{1}ArgsOffset", (object) eventIndex, (object) instrIndex));
        if (format < EMEVD.Game.DarkSouls3)
        {
          bw.ReserveInt32(string.Format("Event{0}Instr{1}LayerOffset", (object) eventIndex, (object) instrIndex));
          bw.WriteInt32(0);
        }
        else
          bw.ReserveInt64(string.Format("Event{0}Instr{1}LayerOffset", (object) eventIndex, (object) instrIndex));
      }

      internal void WriteArgs(
        BinaryWriterEx bw,
        EMEVD.Game format,
        EMEVD.Offsets offsets,
        int eventIndex,
        int instrIndex)
      {
        long num = this.ArgData.Length != 0 ? bw.Position - offsets.Arguments : -1L;
        if (format < EMEVD.Game.Sekiro)
          bw.FillInt32(string.Format("Event{0}Instr{1}ArgsOffset", (object) eventIndex, (object) instrIndex), (int) num);
        else
          bw.FillInt64(string.Format("Event{0}Instr{1}ArgsOffset", (object) eventIndex, (object) instrIndex), num);
        bw.WriteBytes(this.ArgData);
        bw.Pad(4);
      }

      internal void FillLayerOffset(
        BinaryWriterEx bw,
        EMEVD.Game format,
        int eventIndex,
        int instrIndex,
        Dictionary<uint, long> layerOffsets)
      {
        long num = this.Layer.HasValue ? layerOffsets[this.Layer.Value] : -1L;
        if (format < EMEVD.Game.DarkSouls3)
          bw.FillInt32(string.Format("Event{0}Instr{1}LayerOffset", (object) eventIndex, (object) instrIndex), (int) num);
        else
          bw.FillInt64(string.Format("Event{0}Instr{1}LayerOffset", (object) eventIndex, (object) instrIndex), num);
      }

      public void PackArgs(IEnumerable<object> args, bool bigEndian = false)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          BinaryWriterEx binaryWriterEx = new BinaryWriterEx(bigEndian, (Stream) memoryStream);
          foreach (object obj1 in args)
          {
            object obj2 = obj1;
            if (obj2 != null)
            {
              object obj3;
              if ((obj3 = obj2) is byte)
              {
                byte num3 = (byte) obj3;
                binaryWriterEx.WriteByte(num3);
                continue;
              }
              object obj4;
              if ((obj4 = obj2) is ushort)
              {
                ushort num4 = (ushort) obj4;
                binaryWriterEx.Pad(2);
                binaryWriterEx.WriteUInt16(num4);
                continue;
              }
              object obj5;
              if ((obj5 = obj2) is uint)
              {
                uint num5 = (uint) obj5;
                binaryWriterEx.Pad(4);
                binaryWriterEx.WriteUInt32(num5);
                continue;
              }
              object obj6;
              if ((obj6 = obj2) is sbyte)
              {
                sbyte num6 = (sbyte) obj6;
                binaryWriterEx.WriteSByte(num6);
                continue;
              }
              object obj7;
              if ((obj7 = obj2) is short)
              {
                short num7 = (short) obj7;
                binaryWriterEx.Pad(2);
                binaryWriterEx.WriteInt16(num7);
                continue;
              }
              if (obj2 is int num)
              {
                binaryWriterEx.Pad(4);
                binaryWriterEx.WriteInt32(num);
                continue;
              }
              object obj8;
              if ((obj8 = obj2) is float)
              {
                float num8 = (float) obj8;
                binaryWriterEx.Pad(4);
                binaryWriterEx.WriteSingle(num8);
                continue;
              }
            }
            throw new NotSupportedException(string.Format("Unsupported argument type: {0}", (object) obj1.GetType()));
          }
          binaryWriterEx.Pad(4);
          this.ArgData = binaryWriterEx.FinishBytes();
        }
      }

      public List<object> UnpackArgs(
        IEnumerable<EMEVD.Instruction.ArgType> argStruct,
        bool bigEndian = false)
      {
        List<object> objectList = new List<object>();
        using (MemoryStream memoryStream = new MemoryStream(this.ArgData))
        {
          BinaryReaderEx binaryReaderEx = new BinaryReaderEx(bigEndian, (Stream) memoryStream);
          foreach (EMEVD.Instruction.ArgType argType in argStruct)
          {
            switch (argType)
            {
              case EMEVD.Instruction.ArgType.Byte:
                objectList.Add((object) binaryReaderEx.ReadByte());
                continue;
              case EMEVD.Instruction.ArgType.UInt16:
                binaryReaderEx.Pad(2);
                objectList.Add((object) binaryReaderEx.ReadUInt16());
                continue;
              case EMEVD.Instruction.ArgType.UInt32:
                binaryReaderEx.Pad(4);
                objectList.Add((object) binaryReaderEx.ReadUInt32());
                continue;
              case EMEVD.Instruction.ArgType.SByte:
                objectList.Add((object) binaryReaderEx.ReadSByte());
                continue;
              case EMEVD.Instruction.ArgType.Int16:
                binaryReaderEx.Pad(2);
                objectList.Add((object) binaryReaderEx.ReadInt16());
                continue;
              case EMEVD.Instruction.ArgType.Int32:
                binaryReaderEx.Pad(4);
                objectList.Add((object) binaryReaderEx.ReadInt32());
                continue;
              case EMEVD.Instruction.ArgType.Single:
                binaryReaderEx.Pad(4);
                objectList.Add((object) binaryReaderEx.ReadSingle());
                continue;
              default:
                throw new NotImplementedException(string.Format("Unimplemented argument type: {0}", (object) argType));
            }
          }
        }
        return objectList;
      }

      public enum ArgType
      {
        Byte,
        UInt16,
        UInt32,
        SByte,
        Int16,
        Int32,
        Single,
      }
    }

    private static class Layer
    {
      public static uint Read(BinaryReaderEx br)
      {
        br.AssertInt32(2);
        uint num = br.ReadUInt32();
        br.AssertVarint(new long[1]);
        br.AssertVarint(-1L);
        br.AssertVarint(1L);
        return num;
      }

      public static void Write(BinaryWriterEx bw, uint layer)
      {
        bw.WriteInt32(2);
        bw.WriteUInt32(layer);
        bw.WriteVarint(0L);
        bw.WriteVarint(-1L);
        bw.WriteVarint(1L);
      }
    }

    public class Parameter
    {
      public long InstructionIndex { get; set; }

      public long TargetStartByte { get; set; }

      public long SourceStartByte { get; set; }

      public int ByteCount { get; set; }

      public int UnkID { get; set; }

      public Parameter()
      {
      }

      public Parameter(long instrIndex, long targetStartByte, long srcStartByte, int byteCount)
      {
        this.InstructionIndex = instrIndex;
        this.TargetStartByte = targetStartByte;
        this.SourceStartByte = srcStartByte;
        this.ByteCount = byteCount;
      }

      internal Parameter(BinaryReaderEx br, EMEVD.Game format)
      {
        this.InstructionIndex = br.ReadVarint();
        this.TargetStartByte = br.ReadVarint();
        this.SourceStartByte = br.ReadVarint();
        this.ByteCount = br.ReadInt32();
        this.UnkID = br.ReadInt32();
      }

      internal void Write(BinaryWriterEx bw, EMEVD.Game format)
      {
        bw.WriteVarint(this.InstructionIndex);
        bw.WriteVarint(this.TargetStartByte);
        bw.WriteVarint(this.SourceStartByte);
        bw.WriteInt32(this.ByteCount);
        bw.WriteInt32(this.UnkID);
      }
    }
  }
}
