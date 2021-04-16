// Decompiled with JetBrains decompiler
// Type: SoulsFormats.EDD
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class EDD : SoulsFile<EDD>
  {
    public bool LongFormat { get; set; }

    public List<EDD.FunctionSpec> FunctionSpecs { get; set; }

    public List<EDD.CommandSpec> CommandSpecs { get; set; }

    public List<EDD.MachineDesc> Machines { get; set; }

    public int Unk80 { get; set; }

    public int[] UnkB0 { get; private set; }

    public EDD()
    {
      this.LongFormat = false;
      this.FunctionSpecs = new List<EDD.FunctionSpec>();
      this.CommandSpecs = new List<EDD.CommandSpec>();
      this.Machines = new List<EDD.MachineDesc>();
      this.UnkB0 = new int[4];
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      this.LongFormat = br.AssertASCII("fSSL", "fsSL") == "fsSL";
      br.VarintLong = this.LongFormat;
      br.AssertInt32(1);
      br.AssertInt32(1);
      br.AssertInt32(1);
      br.AssertInt32(124);
      int num1 = br.ReadInt32();
      br.AssertInt32(11);
      br.AssertInt32(this.LongFormat ? 88 : 52);
      br.AssertInt32(1);
      br.AssertInt32(this.LongFormat ? 16 : 8);
      int num2 = br.ReadInt32();
      br.AssertInt32(4);
      br.AssertInt32(new int[1]);
      br.AssertInt32(8);
      int num3 = br.ReadInt32();
      int conditionSize = br.AssertInt32(this.LongFormat ? 16 : 8);
      int num4 = br.ReadInt32();
      br.AssertInt32(this.LongFormat ? 16 : 8);
      br.AssertInt32(new int[1]);
      br.AssertInt32(this.LongFormat ? 24 : 16);
      int num5 = br.ReadInt32();
      int commandSize = br.AssertInt32(4);
      int num6 = br.ReadInt32();
      int passCommandSize = br.AssertInt32(this.LongFormat ? 16 : 8);
      int num7 = br.ReadInt32();
      int stateSize = br.AssertInt32(this.LongFormat ? 120 : 60);
      int num8 = br.ReadInt32();
      br.AssertInt32(this.LongFormat ? 72 : 48);
      int num9 = br.ReadInt32();
      int num10 = br.ReadInt32();
      br.AssertInt32(new int[1]);
      br.AssertInt32(num10);
      this.Unk80 = br.ReadInt32();
      br.AssertInt32(num1);
      br.AssertInt32(new int[1]);
      br.AssertInt32(num1);
      br.AssertInt32(new int[1]);
      long position = br.Position;
      br.AssertVarint(new long[1]);
      br.ReadVarint();
      br.AssertVarint((long) num5);
      br.ReadVarint();
      br.AssertVarint((long) num3);
      br.ReadVarint();
      br.AssertInt32(num9);
      this.UnkB0 = br.ReadInt32s(4);
      if (this.LongFormat)
        br.AssertInt32(new int[1]);
      br.AssertVarint(this.LongFormat ? 88L : 52L);
      br.AssertVarint((long) num2);
      List<string> strings = new List<string>();
      for (int index = 0; index < num2; ++index)
      {
        long num11 = br.ReadVarint();
        br.ReadVarint();
        string utF16 = br.GetUTF16(position + num11);
        strings.Add(utF16);
      }
      this.FunctionSpecs = new List<EDD.FunctionSpec>();
      for (int index = 0; index < num3; ++index)
        this.FunctionSpecs.Add(new EDD.FunctionSpec(br, strings));
      Dictionary<long, EDD.ConditionDesc> conditions = new Dictionary<long, EDD.ConditionDesc>();
      for (int index1 = 0; index1 < num4; ++index1)
      {
        long index2 = br.Position - position;
        conditions[index2] = new EDD.ConditionDesc(br);
      }
      this.CommandSpecs = new List<EDD.CommandSpec>();
      for (int index = 0; index < num5; ++index)
        this.CommandSpecs.Add(new EDD.CommandSpec(br, strings));
      Dictionary<long, EDD.CommandDesc> commands = new Dictionary<long, EDD.CommandDesc>();
      for (int index1 = 0; index1 < num6; ++index1)
      {
        long index2 = br.Position - position;
        commands[index2] = new EDD.CommandDesc(br, strings);
      }
      if (this.LongFormat)
      {
        long num11 = br.Position - position;
        if (num11 % 8L > 0L)
          br.Skip(8 - (int) (num11 % 8L));
      }
      Dictionary<long, EDD.PassCommandDesc> passCommands = new Dictionary<long, EDD.PassCommandDesc>();
      for (int index1 = 0; index1 < num7; ++index1)
      {
        long index2 = br.Position - position;
        passCommands[index2] = new EDD.PassCommandDesc(br, commands, commandSize);
      }
      Dictionary<long, EDD.StateDesc> states = new Dictionary<long, EDD.StateDesc>();
      for (int index1 = 0; index1 < num8; ++index1)
      {
        long index2 = br.Position - position;
        states[index2] = new EDD.StateDesc(br, strings, position, conditions, conditionSize, commands, commandSize, passCommands, passCommandSize);
      }
      this.Machines = new List<EDD.MachineDesc>();
      for (int index = 0; index < num9; ++index)
        this.Machines.Add(new EDD.MachineDesc(br, strings, states, stateSize));
      if (conditions.Count > 0 || commands.Count > 0 || (passCommands.Count > 0 || states.Count > 0))
        throw new FormatException("Orphaned ESD descriptions found");
    }

    private static List<T> GetUniqueOffsetList<T>(
      long offset,
      long count,
      Dictionary<long, T> offsets,
      int objSize)
    {
      List<T> objList = new List<T>();
      for (int index = 0; (long) index < count; ++index)
      {
        if (!offsets.ContainsKey(offset))
          throw new FormatException(string.Format("Nonexistent or reused {0} at index {1}/{2}, offset {3}", (object) typeof (T), (object) index, (object) count, (object) offset));
        objList.Add(offsets[offset]);
        offsets.Remove(offset);
        offset += (long) objSize;
      }
      return objList;
    }

    public class FunctionSpec
    {
      public int ID { get; set; }

      public string Name { get; set; }

      public byte Unk06 { get; set; }

      public byte Unk07 { get; set; }

      public FunctionSpec(int id = 0, string name = null)
      {
        this.ID = id;
        this.Name = name;
      }

      internal FunctionSpec(BinaryReaderEx br, List<string> strings)
      {
        this.ID = br.ReadInt32();
        short num = br.ReadInt16();
        this.Unk06 = br.ReadByte();
        this.Unk07 = br.ReadByte();
        this.Name = strings[(int) num];
      }
    }

    public class ConditionDesc
    {
      public ConditionDesc()
      {
      }

      internal ConditionDesc(BinaryReaderEx br)
      {
        br.AssertVarint(-1L);
        br.AssertVarint(new long[1]);
      }
    }

    public class CommandSpec
    {
      public long ID { get; set; }

      public string Name { get; set; }

      public short Unk0E { get; set; }

      public CommandSpec(long id = 0, string name = null)
      {
        this.ID = id;
        this.Name = name;
      }

      internal CommandSpec(BinaryReaderEx br, List<string> strings)
      {
        this.ID = br.ReadVarint();
        br.AssertVarint(-1L);
        br.AssertInt32(new int[1]);
        short num = br.ReadInt16();
        this.Unk0E = br.ReadInt16();
        this.Name = strings[(int) num];
      }
    }

    public class CommandDesc
    {
      public string Name { get; set; }

      public CommandDesc(string name = null)
      {
        this.Name = name;
      }

      internal CommandDesc(BinaryReaderEx br, List<string> strings)
      {
        short num1 = br.ReadInt16();
        int num2 = (int) br.AssertByte((byte) 1);
        int num3 = (int) br.AssertByte(byte.MaxValue);
        this.Name = strings[(int) num1];
      }
    }

    public class PassCommandDesc
    {
      public List<EDD.CommandDesc> PassCommands { get; set; }

      public PassCommandDesc()
      {
        this.PassCommands = new List<EDD.CommandDesc>();
      }

      internal PassCommandDesc(
        BinaryReaderEx br,
        Dictionary<long, EDD.CommandDesc> commands,
        int commandSize)
      {
        this.PassCommands = EDD.GetUniqueOffsetList<EDD.CommandDesc>((long) br.ReadInt32(), (long) br.ReadInt32(), commands, commandSize);
      }
    }

    public class StateDesc
    {
      public long ID { get; set; }

      public string Name { get; set; }

      public List<EDD.CommandDesc> EntryCommands { get; set; }

      public List<EDD.CommandDesc> ExitCommands { get; set; }

      public List<EDD.CommandDesc> WhileCommands { get; set; }

      public List<EDD.PassCommandDesc> PassCommands { get; set; }

      public List<EDD.ConditionDesc> Conditions { get; set; }

      public StateDesc(long id = 0, string name = null)
      {
        this.ID = id;
        this.Name = name;
        this.EntryCommands = new List<EDD.CommandDesc>();
        this.ExitCommands = new List<EDD.CommandDesc>();
        this.WhileCommands = new List<EDD.CommandDesc>();
        this.PassCommands = new List<EDD.PassCommandDesc>();
        this.Conditions = new List<EDD.ConditionDesc>();
      }

      internal StateDesc(
        BinaryReaderEx br,
        List<string> strings,
        long dataStart,
        Dictionary<long, EDD.ConditionDesc> conditions,
        int conditionSize,
        Dictionary<long, EDD.CommandDesc> commands,
        int commandSize,
        Dictionary<long, EDD.PassCommandDesc> passCommands,
        int passCommandSize)
      {
        this.ID = br.ReadVarint();
        long num = br.ReadVarint();
        br.AssertVarint(1L);
        long offset1 = br.ReadVarint();
        long count1 = br.ReadVarint();
        long offset2 = br.ReadVarint();
        long count2 = br.ReadVarint();
        long offset3 = br.ReadVarint();
        long count3 = br.ReadVarint();
        long offset4 = br.ReadVarint();
        long count4 = br.ReadVarint();
        long offset5 = br.ReadVarint();
        long count5 = br.ReadVarint();
        br.AssertVarint(-1L);
        br.AssertVarint(new long[1]);
        short int16 = br.GetInt16(dataStart + num);
        this.Name = strings[(int) int16];
        this.EntryCommands = EDD.GetUniqueOffsetList<EDD.CommandDesc>(offset1, count1, commands, commandSize);
        this.ExitCommands = EDD.GetUniqueOffsetList<EDD.CommandDesc>(offset2, count2, commands, commandSize);
        this.WhileCommands = EDD.GetUniqueOffsetList<EDD.CommandDesc>(offset3, count3, commands, commandSize);
        this.PassCommands = EDD.GetUniqueOffsetList<EDD.PassCommandDesc>(offset4, count4, passCommands, passCommandSize);
        this.Conditions = EDD.GetUniqueOffsetList<EDD.ConditionDesc>(offset5, count5, conditions, conditionSize);
      }
    }

    public class MachineDesc
    {
      public int ID { get; set; }

      public string Name { get; set; }

      public short Unk06 { get; set; }

      public string[] ParamNames { get; private set; }

      public List<EDD.StateDesc> States { get; set; }

      public MachineDesc(int id = 0, string name = null)
      {
        this.ID = id;
        this.Name = name;
        this.ParamNames = new string[8];
        this.States = new List<EDD.StateDesc>();
      }

      internal MachineDesc(
        BinaryReaderEx br,
        List<string> strings,
        Dictionary<long, EDD.StateDesc> states,
        int stateSize)
      {
        this.ID = br.ReadInt32();
        short num = br.ReadInt16();
        this.Unk06 = br.ReadInt16();
        short[] numArray = br.ReadInt16s(8);
        br.AssertVarint(-1L);
        br.AssertVarint(new long[1]);
        br.AssertVarint(-1L);
        br.AssertVarint(new long[1]);
        this.States = EDD.GetUniqueOffsetList<EDD.StateDesc>(br.ReadVarint(), br.ReadVarint(), states, stateSize);
        this.Name = strings[(int) num];
        this.ParamNames = new string[8];
        for (int index = 0; index < 8; ++index)
        {
          if (numArray[index] >= (short) 0)
            this.ParamNames[index] = strings[(int) numArray[index]];
        }
      }
    }
  }
}
