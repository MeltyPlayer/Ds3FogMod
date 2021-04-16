// Decompiled with JetBrains decompiler
// Type: SoulsFormats.ESD
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class ESD : SoulsFile<ESD>
  {
    public bool LongFormat;
    public int DarkSoulsCount;
    public string Name;
    public int Unk70;
    public int Unk74;
    public int Unk78;
    public int Unk7C;
    public Dictionary<long, Dictionary<long, ESD.State>> StateGroups;

    public ESD()
      : this(false, 1)
    {
    }

    public ESD(bool longFormat, int darkSoulsCount)
    {
      this.LongFormat = longFormat;
      this.DarkSoulsCount = darkSoulsCount;
      this.Name = (string) null;
      this.Unk70 = 0;
      this.Unk74 = 0;
      this.Unk78 = 0;
      this.Unk7C = 0;
      this.StateGroups = new Dictionary<long, Dictionary<long, ESD.State>>();
    }

    protected override bool Is(BinaryReaderEx br)
    {
      if (br.Length < 4L)
        return false;
      string ascii = br.GetASCII(0L, 4);
      return ascii == "fSSL" || ascii == "fsSL";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      this.LongFormat = br.AssertASCII("fSSL", "fsSL") == "fsSL";
      br.AssertInt32(1);
      this.DarkSoulsCount = br.AssertInt32(1, 2, 3);
      br.AssertInt32(this.DarkSoulsCount);
      br.AssertInt32(84);
      br.ReadInt32();
      br.AssertInt32(6);
      br.AssertInt32(this.LongFormat ? 72 : 44);
      br.AssertInt32(1);
      br.AssertInt32(this.LongFormat ? 32 : 16);
      int capacity1 = br.ReadInt32();
      int num1 = br.AssertInt32(this.LongFormat ? 72 : 36);
      int capacity2 = br.ReadInt32();
      br.AssertInt32(this.LongFormat ? 56 : 28);
      int capacity3 = br.ReadInt32();
      br.AssertInt32(this.LongFormat ? 24 : 16);
      br.ReadInt32();
      br.AssertInt32(this.LongFormat ? 16 : 8);
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      int num2 = br.ReadInt32();
      br.ReadInt32();
      br.AssertInt32(new int[1]);
      br.ReadInt32();
      br.AssertInt32(new int[1]);
      long position = br.Position;
      br.AssertInt32(1);
      this.Unk70 = br.ReadInt32();
      this.Unk74 = br.ReadInt32();
      this.Unk78 = br.ReadInt32();
      this.Unk7C = br.ReadInt32();
      if (this.LongFormat)
        br.AssertInt32(new int[1]);
      ESD.ReadVarint(br, this.LongFormat);
      ESD.AssertVarint(br, (this.LongFormat ? 1 : 0) != 0, (long) capacity1);
      long num3 = ESD.ReadVarint(br, this.LongFormat);
      ESD.AssertVarint(br, (this.LongFormat ? 1 : 0) != 0, (long) num2);
      long num4 = this.DarkSoulsCount == 1 ? 0L : -1L;
      ESD.AssertVarint(br, (this.LongFormat ? 1 : 0) != 0, num4);
      ESD.AssertVarint(br, (this.LongFormat ? 1 : 0) != 0, num4);
      this.Name = num2 <= 0 ? (string) null : br.GetUTF16(position + num3);
      Dictionary<long, long[]> dictionary1 = new Dictionary<long, long[]>(capacity1);
      for (int index = 0; index < capacity1; ++index)
      {
        long key = ESD.ReadVarint(br, this.LongFormat);
        long[] numArray = this.ReadStateGroup(br, this.LongFormat, position, (long) num1);
        if (dictionary1.ContainsKey(key))
          throw new FormatException("Duplicate state group ID.");
        dictionary1[key] = numArray;
      }
      Dictionary<long, ESD.State> states1 = new Dictionary<long, ESD.State>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        states1[br.Position - position] = new ESD.State(br, this.LongFormat, position);
      Dictionary<long, ESD.Condition> conditions = new Dictionary<long, ESD.Condition>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        conditions[br.Position - position] = new ESD.Condition(br, this.LongFormat, position);
      foreach (ESD.State state in states1.Values)
        state.GetConditions(conditions);
      this.StateGroups = new Dictionary<long, Dictionary<long, ESD.State>>(capacity1);
      Dictionary<long, Dictionary<long, long>> dictionary2 = new Dictionary<long, Dictionary<long, long>>();
      foreach (long key in dictionary1.Keys)
      {
        long[] stateOffsets = dictionary1[key];
        Dictionary<long, long> stateIDs;
        Dictionary<long, ESD.State> states2 = this.TakeStates((long) num1, stateOffsets, states1, out stateIDs);
        this.StateGroups[key] = states2;
        dictionary2[key] = stateIDs;
        foreach (ESD.State state in states2.Values)
        {
          foreach (ESD.Condition condition in state.Conditions)
            condition.GetStateAndConditions(stateIDs, conditions);
        }
      }
      if (states1.Count > 0)
        throw new FormatException("Orphaned states found.");
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bw.BigEndian = false;
      bw.WriteASCII(this.LongFormat ? "fsSL" : "fSSL", false);
      bw.WriteInt32(1);
      bw.WriteInt32(this.DarkSoulsCount);
      bw.WriteInt32(this.DarkSoulsCount);
      bw.WriteInt32(84);
      bw.ReserveInt32("DataSize");
      bw.WriteInt32(6);
      bw.WriteInt32(this.LongFormat ? 72 : 44);
      bw.WriteInt32(1);
      bw.WriteInt32(this.LongFormat ? 32 : 16);
      bw.WriteInt32(this.StateGroups.Count);
      int count = this.LongFormat ? 72 : 36;
      bw.WriteInt32(count);
      bw.WriteInt32(this.StateGroups.Values.Sum<Dictionary<long, ESD.State>>((Func<Dictionary<long, ESD.State>, int>) (sg => sg.Count + (sg.Count == 1 ? 0 : 1))));
      bw.WriteInt32(this.LongFormat ? 56 : 28);
      bw.ReserveInt32("ConditionCount");
      bw.WriteInt32(this.LongFormat ? 24 : 16);
      bw.ReserveInt32("CommandCallCount");
      bw.WriteInt32(this.LongFormat ? 16 : 8);
      bw.ReserveInt32("CommandArgCount");
      bw.ReserveInt32("ConditionOffsetsOffset");
      bw.ReserveInt32("ConditionOffsetsCount");
      bw.ReserveInt32("NameBlockOffset");
      bw.WriteInt32(this.Name == null ? 0 : this.Name.Length + 1);
      bw.ReserveInt32("UnkOffset1");
      bw.WriteInt32(0);
      bw.ReserveInt32("UnkOffset2");
      bw.WriteInt32(0);
      long position1 = bw.Position;
      bw.WriteInt32(1);
      bw.WriteInt32(this.Unk70);
      bw.WriteInt32(this.Unk74);
      bw.WriteInt32(this.Unk78);
      bw.WriteInt32(this.Unk7C);
      if (this.LongFormat)
        bw.WriteInt32(0);
      ESD.ReserveVarint(bw, this.LongFormat, "StateGroupsOffset");
      ESD.WriteVarint(bw, this.LongFormat, (long) this.StateGroups.Count);
      ESD.ReserveVarint(bw, this.LongFormat, "NameOffset");
      ESD.WriteVarint(bw, this.LongFormat, this.Name == null ? 0L : (long) (this.Name.Length + 1));
      long num1 = this.DarkSoulsCount == 1 ? 0L : -1L;
      ESD.WriteVarint(bw, this.LongFormat, num1);
      ESD.WriteVarint(bw, this.LongFormat, num1);
      List<long> list = this.StateGroups.Keys.ToList<long>();
      list.Sort();
      Dictionary<long, List<long>> dictionary1 = new Dictionary<long, List<long>>();
      foreach (long index in list)
      {
        dictionary1[index] = this.StateGroups[index].Keys.ToList<long>();
        dictionary1[index].Sort();
      }
      if (this.StateGroups.Count == 0)
      {
        ESD.FillVarint(bw, this.LongFormat, "StateGroupsOffset", -1L);
      }
      else
      {
        ESD.FillVarint(bw, this.LongFormat, "StateGroupsOffset", bw.Position - position1);
        foreach (long index in list)
        {
          ESD.WriteVarint(bw, this.LongFormat, index);
          ESD.ReserveVarint(bw, this.LongFormat, string.Format("StateGroup{0}:StatesOffset1", (object) index));
          ESD.WriteVarint(bw, this.LongFormat, (long) this.StateGroups[index].Count);
          ESD.ReserveVarint(bw, this.LongFormat, string.Format("StateGroup{0}:StatesOffset2", (object) index));
        }
      }
      Dictionary<long, Dictionary<long, long>> dictionary2 = new Dictionary<long, Dictionary<long, long>>();
      List<long[]> numArrayList = new List<long[]>();
      foreach (long groupID in list)
      {
        dictionary2[groupID] = new Dictionary<long, long>();
        ESD.FillVarint(bw, this.LongFormat, string.Format("StateGroup{0}:StatesOffset1", (object) groupID), bw.Position - position1);
        ESD.FillVarint(bw, this.LongFormat, string.Format("StateGroup{0}:StatesOffset2", (object) groupID), bw.Position - position1);
        long position2 = bw.Position;
        foreach (long stateID in dictionary1[groupID])
        {
          dictionary2[groupID][stateID] = bw.Position - position1;
          this.StateGroups[groupID][stateID].WriteHeader(bw, this.LongFormat, groupID, stateID);
        }
        if (this.StateGroups[groupID].Count > 1)
        {
          numArrayList.Add(new long[2]
          {
            position2,
            bw.Position
          });
          bw.Position += (long) count;
        }
      }
      
      var conditions = new Dictionary<long, List<ESD.Condition>>();

      foreach (var groupID in list)
      {
        void addCondition(ESD.Condition cond) {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          if (conditions[groupID].Any<ESD.Condition>((Func<ESD.Condition, bool>)(c => cond == c)))
            return;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          conditions[groupID].Add(cond);
          foreach (ESD.Condition subcondition in cond.Subconditions)
            addCondition(subcondition);
        }

        conditions[groupID] = new List<ESD.Condition>();
        // ISSUE: reference to a compiler-generated field
        foreach (ESD.State state in this.StateGroups[groupID].Values)
        {
          foreach (ESD.Condition condition in state.Conditions)
            addCondition(condition);
        }
      }
      // ISSUE: reference to a compiler-generated field
      bw.FillInt32("ConditionCount", conditions.Values.Sum<List<ESD.Condition>>((Func<List<ESD.Condition>, int>) (group => group.Count)));
      Dictionary<ESD.Condition, long> conditionOffsets = new Dictionary<ESD.Condition, long>();
      foreach (long groupID in list)
      {
        // ISSUE: reference to a compiler-generated field
        for (int index1 = 0; index1 < conditions[groupID].Count; ++index1)
        {
          // ISSUE: reference to a compiler-generated field
          ESD.Condition index2 = conditions[groupID][index1];
          conditionOffsets[index2] = bw.Position - position1;
          index2.WriteHeader(bw, this.LongFormat, groupID, index1, dictionary2[groupID]);
        }
      }
      List<ESD.CommandCall> commandCallList = new List<ESD.CommandCall>();
      foreach (long groupID in list)
      {
        foreach (long stateID in dictionary1[groupID])
          this.StateGroups[groupID][stateID].WriteCommandCalls(bw, this.LongFormat, groupID, stateID, position1, commandCallList);
        // ISSUE: reference to a compiler-generated field
        for (int index = 0; index < conditions[groupID].Count; ++index)
        {
          // ISSUE: reference to a compiler-generated field
          conditions[groupID][index].WriteCommandCalls(bw, this.LongFormat, groupID, index, position1, commandCallList);
        }
      }
      bw.FillInt32("CommandCallCount", commandCallList.Count);
      bw.FillInt32("CommandArgCount", commandCallList.Sum<ESD.CommandCall>((Func<ESD.CommandCall, int>) (command => command.Arguments.Count)));
      for (int index = 0; index < commandCallList.Count; ++index)
        commandCallList[index].WriteArgs(bw, this.LongFormat, index, position1);
      bw.FillInt32("ConditionOffsetsOffset", (int) (bw.Position - position1));
      int num3 = 0;
      foreach (long groupID in list)
      {
        foreach (long stateID in dictionary1[groupID])
          num3 += this.StateGroups[groupID][stateID].WriteConditionOffsets(bw, this.LongFormat, groupID, stateID, position1, conditionOffsets);
        // ISSUE: reference to a compiler-generated field
        for (int index = 0; index < conditions[groupID].Count; ++index)
        {
          // ISSUE: reference to a compiler-generated field
          num3 += conditions[groupID][index].WriteConditionOffsets(bw, this.LongFormat, groupID, index, position1, conditionOffsets);
        }
      }
      bw.FillInt32("ConditionOffsetsCount", num3);
      foreach (long groupID in list)
      {
        // ISSUE: reference to a compiler-generated field
        for (int index = 0; index < conditions[groupID].Count; ++index)
        {
          // ISSUE: reference to a compiler-generated field
          conditions[groupID][index].WriteEvaluator(bw, this.LongFormat, groupID, index, position1);
        }
      }
      for (int index = 0; index < commandCallList.Count; ++index)
        commandCallList[index].WriteBytecode(bw, this.LongFormat, index, position1);
      bw.FillInt32("NameBlockOffset", (int) (bw.Position - position1));
      if (this.Name == null)
      {
        ESD.FillVarint(bw, this.LongFormat, "NameOffset", -1L);
      }
      else
      {
        bw.Pad(2);
        ESD.FillVarint(bw, this.LongFormat, "NameOffset", bw.Position - position1);
        bw.WriteUTF16(this.Name, true);
      }
      bw.FillInt32("UnkOffset1", (int) (bw.Position - position1));
      bw.FillInt32("UnkOffset2", (int) (bw.Position - position1));
      bw.FillInt32("DataSize", (int) (bw.Position - position1));
      if (this.DarkSoulsCount == 1)
        bw.Pad(4);
      else if (this.DarkSoulsCount == 2)
        bw.Pad(16);
      foreach (long[] numArray1 in numArrayList)
      {
        bw.Position = numArray1[0];
        byte[] numArray2 = new byte[count];
        bw.Stream.Read(numArray2, 0, count);
        bw.Position = numArray1[1];
        bw.WriteBytes(numArray2);
      }
    }

    private long[] ReadStateGroup(
      BinaryReaderEx br,
      bool longFormat,
      long dataStart,
      long stateSize)
    {
      long num1 = ESD.ReadVarint(br, longFormat);
      long length = ESD.ReadVarint(br, longFormat);
      ESD.AssertVarint(br, (longFormat ? 1 : 0) != 0, num1);
      long[] numArray = new long[length];
      for (int index = 0; (long) index < length; ++index)
        numArray[index] = num1 + (long) index * stateSize;
      if (length > 1L)
      {
        byte[] bytes = br.GetBytes(dataStart + num1, (int) stateSize);
        br.StepIn(dataStart + num1 + stateSize * length);
        for (int index = 0; (long) index < stateSize; ++index)
        {
          int num2 = (int) br.AssertByte(bytes[index]);
        }
        br.StepOut();
      }
      return numArray;
    }

    private Dictionary<long, ESD.State> TakeStates(
      long stateSize,
      long[] stateOffsets,
      Dictionary<long, ESD.State> states,
      out Dictionary<long, long> stateIDs)
    {
      stateIDs = new Dictionary<long, long>(stateOffsets.Length + 1);
      if (stateOffsets.Length > 1)
      {
        long key = stateOffsets[0] + stateSize * (long) stateOffsets.Length;
        if (!states.Remove(key))
          throw new FormatException("Weird state not found.");
      }
      Dictionary<long, ESD.State> dictionary = new Dictionary<long, ESD.State>(stateOffsets.Length);
      foreach (long stateOffset in stateOffsets)
      {
        ESD.State state = states[stateOffset];
        if (dictionary.ContainsKey(state.ID))
          throw new FormatException("Duplicate state ID.");
        dictionary[state.ID] = state;
        states.Remove(stateOffset);
        stateIDs[stateOffset] = state.ID;
      }
      stateOffsets = (long[]) null;
      return dictionary;
    }

    private static long ReadVarint(BinaryReaderEx br, bool longFormat)
    {
      return longFormat ? br.ReadInt64() : (long) br.ReadInt32();
    }

    private static long[] ReadVarints(BinaryReaderEx br, bool longFormat, long count)
    {
      return longFormat ? br.ReadInt64s((int) count) : Array.ConvertAll<int, long>(br.ReadInt32s((int) count), (Converter<int, long>) (i => (long) i));
    }

    private static long AssertVarint(BinaryReaderEx br, bool longFormat, params long[] values)
    {
      return longFormat ? br.AssertInt64(values) : (long) br.AssertInt32(Array.ConvertAll<long, int>(values, (Converter<long, int>) (l => (int) l)));
    }

    private static void WriteVarint(BinaryWriterEx bw, bool longFormat, long value)
    {
      if (longFormat)
        bw.WriteInt64(value);
      else
        bw.WriteInt32((int) value);
    }

    private static void ReserveVarint(BinaryWriterEx bw, bool longFormat, string name)
    {
      if (longFormat)
        bw.ReserveInt64(name);
      else
        bw.ReserveInt32(name);
    }

    private static void FillVarint(BinaryWriterEx bw, bool longFormat, string name, long value)
    {
      if (longFormat)
        bw.FillInt64(name, value);
      else
        bw.FillInt32(name, (int) value);
    }

    public class State
    {
      public List<ESD.Condition> Conditions;
      public List<ESD.CommandCall> EntryCommands;
      public List<ESD.CommandCall> ExitCommands;
      public List<ESD.CommandCall> WhileCommands;
      internal long ID;
      private long[] conditionOffsets;

      public State()
      {
        this.Conditions = new List<ESD.Condition>();
        this.EntryCommands = new List<ESD.CommandCall>();
        this.ExitCommands = new List<ESD.CommandCall>();
        this.WhileCommands = new List<ESD.CommandCall>();
      }

      internal State(BinaryReaderEx br, bool longFormat, long dataStart)
      {
        this.ID = ESD.ReadVarint(br, longFormat);
        long num1 = ESD.ReadVarint(br, longFormat);
        long count = ESD.ReadVarint(br, longFormat);
        long num2 = ESD.ReadVarint(br, longFormat);
        long num3 = ESD.ReadVarint(br, longFormat);
        long num4 = ESD.ReadVarint(br, longFormat);
        long num5 = ESD.ReadVarint(br, longFormat);
        long num6 = ESD.ReadVarint(br, longFormat);
        long num7 = ESD.ReadVarint(br, longFormat);
        br.StepIn(0L);
        br.Position = dataStart + num1;
        this.conditionOffsets = ESD.ReadVarints(br, longFormat, count);
        br.Position = dataStart + num2;
        this.EntryCommands = new List<ESD.CommandCall>((int) num3);
        for (int index = 0; (long) index < num3; ++index)
          this.EntryCommands.Add(new ESD.CommandCall(br, longFormat, dataStart));
        br.Position = dataStart + num4;
        this.ExitCommands = new List<ESD.CommandCall>((int) num5);
        for (int index = 0; (long) index < num5; ++index)
          this.ExitCommands.Add(new ESD.CommandCall(br, longFormat, dataStart));
        br.Position = dataStart + num6;
        this.WhileCommands = new List<ESD.CommandCall>((int) num7);
        for (int index = 0; (long) index < num7; ++index)
          this.WhileCommands.Add(new ESD.CommandCall(br, longFormat, dataStart));
        br.StepOut();
      }

      internal void GetConditions(Dictionary<long, ESD.Condition> conditions)
      {
        this.Conditions = new List<ESD.Condition>(this.conditionOffsets.Length);
        foreach (long conditionOffset in this.conditionOffsets)
          this.Conditions.Add(conditions[conditionOffset]);
        this.conditionOffsets = (long[]) null;
      }

      internal void WriteHeader(BinaryWriterEx bw, bool longFormat, long groupID, long stateID)
      {
        ESD.WriteVarint(bw, longFormat, stateID);
        ESD.ReserveVarint(bw, longFormat, string.Format("State{0}-{1}:ConditionsOffset", (object) groupID, (object) stateID));
        ESD.WriteVarint(bw, longFormat, (long) this.Conditions.Count);
        ESD.ReserveVarint(bw, longFormat, string.Format("State{0}-{1}:EntryCommandsOffset", (object) groupID, (object) stateID));
        ESD.WriteVarint(bw, longFormat, (long) this.EntryCommands.Count);
        ESD.ReserveVarint(bw, longFormat, string.Format("State{0}-{1}:ExitCommandsOffset", (object) groupID, (object) stateID));
        ESD.WriteVarint(bw, longFormat, (long) this.ExitCommands.Count);
        ESD.ReserveVarint(bw, longFormat, string.Format("State{0}-{1}:WhileCommandsOffset", (object) groupID, (object) stateID));
        ESD.WriteVarint(bw, longFormat, (long) this.WhileCommands.Count);
      }

      internal void WriteCommandCalls(
        BinaryWriterEx bw,
        bool longFormat,
        long groupID,
        long stateID,
        long dataStart,
        List<ESD.CommandCall> commands)
      {
        if (this.EntryCommands.Count == 0)
        {
          ESD.FillVarint(bw, longFormat, string.Format("State{0}-{1}:EntryCommandsOffset", (object) groupID, (object) stateID), -1L);
        }
        else
        {
          ESD.FillVarint(bw, longFormat, string.Format("State{0}-{1}:EntryCommandsOffset", (object) groupID, (object) stateID), bw.Position - dataStart);
          foreach (ESD.CommandCall entryCommand in this.EntryCommands)
          {
            entryCommand.WriteHeader(bw, longFormat, commands.Count);
            commands.Add(entryCommand);
          }
        }
        if (this.ExitCommands.Count == 0)
        {
          ESD.FillVarint(bw, longFormat, string.Format("State{0}-{1}:ExitCommandsOffset", (object) groupID, (object) stateID), -1L);
        }
        else
        {
          ESD.FillVarint(bw, longFormat, string.Format("State{0}-{1}:ExitCommandsOffset", (object) groupID, (object) stateID), bw.Position - dataStart);
          foreach (ESD.CommandCall exitCommand in this.ExitCommands)
          {
            exitCommand.WriteHeader(bw, longFormat, commands.Count);
            commands.Add(exitCommand);
          }
        }
        if (this.WhileCommands.Count == 0)
        {
          ESD.FillVarint(bw, longFormat, string.Format("State{0}-{1}:WhileCommandsOffset", (object) groupID, (object) stateID), -1L);
        }
        else
        {
          ESD.FillVarint(bw, longFormat, string.Format("State{0}-{1}:WhileCommandsOffset", (object) groupID, (object) stateID), bw.Position - dataStart);
          foreach (ESD.CommandCall whileCommand in this.WhileCommands)
          {
            whileCommand.WriteHeader(bw, longFormat, commands.Count);
            commands.Add(whileCommand);
          }
        }
      }

      internal int WriteConditionOffsets(
        BinaryWriterEx bw,
        bool longFormat,
        long groupID,
        long stateID,
        long dataStart,
        Dictionary<ESD.Condition, long> conditionOffsets)
      {
        ESD.FillVarint(bw, longFormat, string.Format("State{0}-{1}:ConditionsOffset", (object) groupID, (object) stateID), bw.Position - dataStart);
        foreach (ESD.Condition condition in this.Conditions)
          ESD.WriteVarint(bw, longFormat, conditionOffsets[condition]);
        return this.Conditions.Count;
      }
    }

    public class Condition
    {
      public long? TargetState;
      public List<ESD.CommandCall> PassCommands;
      public List<ESD.Condition> Subconditions;
      public byte[] Evaluator;
      private long stateOffset;
      private long[] conditionOffsets;

      public Condition()
      {
        this.TargetState = new long?();
        this.PassCommands = new List<ESD.CommandCall>();
        this.Subconditions = new List<ESD.Condition>();
        this.Evaluator = new byte[0];
      }

      public Condition(long targetState, byte[] evaluator)
      {
        this.TargetState = new long?(targetState);
        this.PassCommands = new List<ESD.CommandCall>();
        this.Subconditions = new List<ESD.Condition>();
        this.Evaluator = evaluator;
      }

      internal Condition(BinaryReaderEx br, bool longFormat, long dataStart)
      {
        this.stateOffset = ESD.ReadVarint(br, longFormat);
        long num1 = ESD.ReadVarint(br, longFormat);
        long num2 = ESD.ReadVarint(br, longFormat);
        long num3 = ESD.ReadVarint(br, longFormat);
        long count = ESD.ReadVarint(br, longFormat);
        long num4 = ESD.ReadVarint(br, longFormat);
        long num5 = ESD.ReadVarint(br, longFormat);
        br.StepIn(0L);
        br.Position = dataStart + num1;
        this.PassCommands = new List<ESD.CommandCall>((int) num2);
        for (int index = 0; (long) index < num2; ++index)
          this.PassCommands.Add(new ESD.CommandCall(br, longFormat, dataStart));
        br.Position = dataStart + num3;
        this.conditionOffsets = ESD.ReadVarints(br, longFormat, count);
        this.Evaluator = br.GetBytes(dataStart + num4, (int) num5);
        br.StepOut();
      }

      internal void GetStateAndConditions(
        Dictionary<long, long> stateOffsets,
        Dictionary<long, ESD.Condition> conditions)
      {
        if (this.stateOffset == -2L)
          return;
        if (this.stateOffset == -1L)
        {
          this.TargetState = new long?();
        }
        else
        {
          if (!stateOffsets.ContainsKey(this.stateOffset))
            throw new FormatException("Condition target state not found.");
          this.TargetState = new long?(stateOffsets[this.stateOffset]);
        }
        this.stateOffset = -2L;
        this.Subconditions = new List<ESD.Condition>(this.conditionOffsets.Length);
        foreach (long conditionOffset in this.conditionOffsets)
          this.Subconditions.Add(conditions[conditionOffset]);
        this.conditionOffsets = (long[]) null;
        foreach (ESD.Condition subcondition in this.Subconditions)
          subcondition.GetStateAndConditions(stateOffsets, conditions);
      }

      internal void WriteHeader(
        BinaryWriterEx bw,
        bool longFormat,
        long groupID,
        int index,
        Dictionary<long, long> stateOffsets)
      {
        if (this.TargetState.HasValue)
          ESD.WriteVarint(bw, longFormat, stateOffsets[this.TargetState.Value]);
        else
          ESD.WriteVarint(bw, longFormat, -1L);
        ESD.ReserveVarint(bw, longFormat, string.Format("Condition{0}-{1}:PassCommandsOffset", (object) groupID, (object) index));
        ESD.WriteVarint(bw, longFormat, (long) this.PassCommands.Count);
        ESD.ReserveVarint(bw, longFormat, string.Format("Condition{0}-{1}:ConditionsOffset", (object) groupID, (object) index));
        ESD.WriteVarint(bw, longFormat, (long) this.Subconditions.Count);
        ESD.ReserveVarint(bw, longFormat, string.Format("Condition{0}-{1}:EvaluatorOffset", (object) groupID, (object) index));
        ESD.WriteVarint(bw, longFormat, (long) this.Evaluator.Length);
      }

      internal void WriteCommandCalls(
        BinaryWriterEx bw,
        bool longFormat,
        long groupID,
        int index,
        long dataStart,
        List<ESD.CommandCall> commands)
      {
        if (this.PassCommands.Count == 0)
        {
          ESD.FillVarint(bw, longFormat, string.Format("Condition{0}-{1}:PassCommandsOffset", (object) groupID, (object) index), -1L);
        }
        else
        {
          ESD.FillVarint(bw, longFormat, string.Format("Condition{0}-{1}:PassCommandsOffset", (object) groupID, (object) index), bw.Position - dataStart);
          foreach (ESD.CommandCall passCommand in this.PassCommands)
          {
            passCommand.WriteHeader(bw, longFormat, commands.Count);
            commands.Add(passCommand);
          }
        }
      }

      internal int WriteConditionOffsets(
        BinaryWriterEx bw,
        bool longFormat,
        long groupID,
        int index,
        long dataStart,
        Dictionary<ESD.Condition, long> conditionOffsets)
      {
        if (this.Subconditions.Count == 0)
        {
          ESD.FillVarint(bw, longFormat, string.Format("Condition{0}-{1}:ConditionsOffset", (object) groupID, (object) index), -1L);
        }
        else
        {
          ESD.FillVarint(bw, longFormat, string.Format("Condition{0}-{1}:ConditionsOffset", (object) groupID, (object) index), bw.Position - dataStart);
          foreach (ESD.Condition subcondition in this.Subconditions)
            ESD.WriteVarint(bw, longFormat, conditionOffsets[subcondition]);
        }
        return this.Subconditions.Count;
      }

      internal void WriteEvaluator(
        BinaryWriterEx bw,
        bool longFormat,
        long groupID,
        int index,
        long dataStart)
      {
        ESD.FillVarint(bw, longFormat, string.Format("Condition{0}-{1}:EvaluatorOffset", (object) groupID, (object) index), bw.Position - dataStart);
        bw.WriteBytes(this.Evaluator);
      }
    }

    public class CommandCall
    {
      public int CommandBank;
      public int CommandID;
      public List<byte[]> Arguments;

      public CommandCall()
      {
        this.CommandBank = 1;
        this.CommandID = 0;
        this.Arguments = new List<byte[]>();
      }

      public CommandCall(int commandBank, int commandID, params byte[][] arguments)
      {
        this.CommandBank = commandBank;
        this.CommandID = commandID;
        this.Arguments = ((IEnumerable<byte[]>) arguments).ToList<byte[]>();
      }

      internal CommandCall(BinaryReaderEx br, bool longFormat, long dataStart)
      {
        this.CommandBank = br.AssertInt32(1, 5, 6, 7);
        this.CommandID = br.ReadInt32();
        long num1 = ESD.ReadVarint(br, longFormat);
        long num2 = ESD.ReadVarint(br, longFormat);
        br.StepIn(dataStart + num1);
        this.Arguments = new List<byte[]>((int) num2);
        for (int index = 0; (long) index < num2; ++index)
        {
          long num3 = ESD.ReadVarint(br, longFormat);
          long num4 = ESD.ReadVarint(br, longFormat);
          this.Arguments.Add(br.GetBytes(dataStart + num3, (int) num4));
        }
        br.StepOut();
      }

      internal void WriteHeader(BinaryWriterEx bw, bool longFormat, int index)
      {
        bw.WriteInt32(this.CommandBank);
        bw.WriteInt32(this.CommandID);
        ESD.ReserveVarint(bw, longFormat, string.Format("Command{0}:ArgsOffset", (object) index));
        ESD.WriteVarint(bw, longFormat, (long) this.Arguments.Count);
      }

      internal void WriteArgs(BinaryWriterEx bw, bool longFormat, int index, long dataStart)
      {
        ESD.FillVarint(bw, longFormat, string.Format("Command{0}:ArgsOffset", (object) index), bw.Position - dataStart);
        for (int index1 = 0; index1 < this.Arguments.Count; ++index1)
        {
          ESD.ReserveVarint(bw, longFormat, string.Format("Command{0}-{1}:BytecodeOffset", (object) index, (object) index1));
          ESD.WriteVarint(bw, longFormat, (long) this.Arguments[index1].Length);
        }
      }

      internal void WriteBytecode(BinaryWriterEx bw, bool longFormat, int index, long dataStart)
      {
        for (int index1 = 0; index1 < this.Arguments.Count; ++index1)
        {
          ESD.FillVarint(bw, longFormat, string.Format("Command{0}-{1}:BytecodeOffset", (object) index, (object) index1), bw.Position - dataStart);
          bw.WriteBytes(this.Arguments[index1]);
        }
      }
    }
  }
}
