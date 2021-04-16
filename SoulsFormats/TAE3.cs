// Decompiled with JetBrains decompiler
// Type: SoulsFormats.TAE3
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class TAE3 : SoulsFile<TAE3> {
    public int ID;
    public string SkeletonName;
    public string SibName;
    public List<TAE3.Animation> Animations;
    public long Unk30;

    public byte[] Flags { get; private set; }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "TAE ";
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertASCII("TAE ");
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      int num3 = (int) br.AssertByte(new byte[1]);
      int num4 = (int) br.AssertByte(byte.MaxValue);
      br.AssertInt32(65548);
      br.ReadInt32();
      br.AssertInt64(64L);
      br.AssertInt64(1L);
      br.AssertInt64(80L);
      br.AssertInt64(128L);
      this.Unk30 = br.ReadInt64();
      br.AssertInt64(new long[1]);
      this.Flags = br.ReadBytes(8);
      br.AssertInt64(1L);
      this.ID = br.ReadInt32();
      int capacity = br.ReadInt32();
      long offset1 = br.ReadInt64();
      br.ReadInt64();
      br.AssertInt64(160L);
      br.AssertInt64((long) capacity);
      br.ReadInt64();
      br.AssertInt64(1L);
      br.AssertInt64(144L);
      br.AssertInt32(this.ID);
      br.AssertInt32(this.ID);
      br.AssertInt64(80L);
      br.AssertInt64(new long[1]);
      br.AssertInt64(176L);
      long offset2 = br.ReadInt64();
      long offset3 = br.ReadInt64();
      br.AssertInt64(new long[1]);
      br.AssertInt64(new long[1]);
      this.SkeletonName = br.GetUTF16(offset2);
      this.SibName = br.GetUTF16(offset3);
      br.StepIn(offset1);
      this.Animations = new List<TAE3.Animation>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Animations.Add(new TAE3.Animation(br));
      br.StepOut();
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.WriteASCII("TAE ", false);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteByte(byte.MaxValue);
      bw.WriteInt32(65548);
      bw.ReserveInt32("FileSize");
      bw.WriteInt64(64L);
      bw.WriteInt64(1L);
      bw.WriteInt64(80L);
      bw.WriteInt64(128L);
      bw.WriteInt64(this.Unk30);
      bw.WriteInt64(0L);
      bw.WriteBytes(this.Flags);
      bw.WriteInt64(1L);
      bw.WriteInt32(this.ID);
      bw.WriteInt32(this.Animations.Count);
      bw.ReserveInt64("AnimsOffset");
      bw.ReserveInt64("AnimGroupsOffset");
      bw.WriteInt64(160L);
      bw.WriteInt64((long) this.Animations.Count);
      bw.ReserveInt64("FirstAnimOffset");
      bw.WriteInt64(1L);
      bw.WriteInt64(144L);
      bw.WriteInt32(this.ID);
      bw.WriteInt32(this.ID);
      bw.WriteInt64(80L);
      bw.WriteInt64(0L);
      bw.WriteInt64(176L);
      bw.ReserveInt64("SkeletonName");
      bw.ReserveInt64("SibName");
      bw.WriteInt64(0L);
      bw.WriteInt64(0L);
      bw.FillInt64("SkeletonName", bw.Position);
      bw.WriteUTF16(this.SkeletonName, true);
      bw.Pad(16);
      bw.FillInt64("SibName", bw.Position);
      bw.WriteUTF16(this.SibName, true);
      bw.Pad(16);
      this.Animations.Sort(
          (Comparison<TAE3.Animation>) ((a1, a2)
                                             => a1.ID.CompareTo(a2.ID)));
      List<long> longList = new List<long>(this.Animations.Count);
      if (this.Animations.Count == 0) {
        bw.FillInt64("AnimsOffset", 0L);
      } else {
        bw.FillInt64("AnimsOffset", bw.Position);
        for (int i = 0; i < this.Animations.Count; ++i) {
          longList.Add(bw.Position);
          this.Animations[i].WriteHeader(bw, i);
        }
      }
      bw.FillInt64("AnimGroupsOffset", bw.Position);
      bw.ReserveInt64("AnimGroupsCount");
      bw.ReserveInt64("AnimGroupsOffset");
      int num = 0;
      long position = bw.Position;
      for (int index1 = 0; index1 < this.Animations.Count; ++index1) {
        int index2 = index1;
        bw.WriteInt32((int) this.Animations[index1].ID);
        while (index1 < this.Animations.Count - 1 &&
               this.Animations[index1 + 1].ID ==
               this.Animations[index1].ID + 1L)
          ++index1;
        bw.WriteInt32((int) this.Animations[index1].ID);
        bw.WriteInt64(longList[index2]);
        ++num;
      }
      bw.FillInt64("AnimGroupsCount", (long) num);
      if (num == 0)
        bw.FillInt64("AnimGroupsOffset", 0L);
      else
        bw.FillInt64("AnimGroupsOffset", position);
      if (this.Animations.Count == 0) {
        bw.FillInt64("FirstAnimOffset", 0L);
      } else {
        bw.FillInt64("FirstAnimOffset", bw.Position);
        for (int i = 0; i < this.Animations.Count; ++i)
          this.Animations[i].WriteBody(bw, i);
      }
      for (int index = 0; index < this.Animations.Count; ++index) {
        TAE3.Animation animation = this.Animations[index];
        animation.WriteAnimFile(bw, index);
        Dictionary<float, long> timeOffsets = animation.WriteTimes(bw, index);
        List<long> eventHeaderOffsets =
            animation.WriteEventHeaders(bw, index, timeOffsets);
        animation.WriteEventData(bw, index);
        animation.WriteEventGroupHeaders(bw, index);
        animation.WriteEventGroupData(bw, index, eventHeaderOffsets);
      }
      bw.FillInt32("FileSize", (int) bw.Position);
    }

    public class Animation {
      public long ID;
      public List<TAE3.Event> Events;
      public List<TAE3.EventGroup> EventGroups;
      public bool AnimFileReference;
      public int AnimFileUnk18;
      public int AnimFileUnk1C;
      public string AnimFileName;

      internal Animation(BinaryReaderEx br) {
        this.ID = br.ReadInt64();
        long offset1 = br.ReadInt64();
        br.StepIn(offset1);
        long offset2 = br.ReadInt64();
        long offset3 = br.ReadInt64();
        br.ReadInt64();
        long offset4 = br.ReadInt64();
        int capacity1 = br.ReadInt32();
        int capacity2 = br.ReadInt32();
        br.ReadInt32();
        br.AssertInt32(new int[1]);
        List<long> eventHeaderOffsets = new List<long>(capacity1);
        this.Events = new List<TAE3.Event>(capacity1);
        br.StepIn(offset2);
        for (int index = 0; index < capacity1; ++index) {
          eventHeaderOffsets.Add(br.Position);
          this.Events.Add(TAE3.Event.Read(br));
        }
        br.StepOut();
        this.EventGroups = new List<TAE3.EventGroup>(capacity2);
        br.StepIn(offset3);
        for (int index = 0; index < capacity2; ++index)
          this.EventGroups.Add(new TAE3.EventGroup(br, eventHeaderOffsets));
        br.StepOut();
        br.StepIn(offset4);
        this.AnimFileReference = br.AssertInt64(0L, 1L) == 1L;
        br.AssertInt64(br.Position + 8L);
        long offset5 = br.ReadInt64();
        this.AnimFileUnk18 = br.ReadInt32();
        this.AnimFileUnk1C = br.ReadInt32();
        br.AssertInt64(new long[1]);
        br.AssertInt64(new long[1]);
        this.AnimFileName = offset5 >= br.Length ? "" : br.GetUTF16(offset5);
        if (!this.AnimFileName.EndsWith(".hkt") &&
            !this.AnimFileName.EndsWith(".hkx"))
          this.AnimFileName = "";
        br.StepOut();
        br.StepOut();
      }

      internal void WriteHeader(BinaryWriterEx bw, int i) {
        bw.WriteInt64(this.ID);
        bw.ReserveInt64(string.Format("AnimationOffset{0}", (object) i));
      }

      internal void WriteBody(BinaryWriterEx bw, int i) {
        bw.FillInt64(string.Format("AnimationOffset{0}", (object) i),
                     bw.Position);
        bw.ReserveInt64(string.Format("EventHeadersOffset{0}", (object) i));
        bw.ReserveInt64(
            string.Format("EventGroupHeadersOffset{0}", (object) i));
        bw.ReserveInt64(string.Format("TimesOffset{0}", (object) i));
        bw.ReserveInt64(string.Format("AnimFileOffset{0}", (object) i));
        bw.WriteInt32(this.Events.Count);
        bw.WriteInt32(this.EventGroups.Count);
        bw.ReserveInt32(string.Format("TimesCount{0}", (object) i));
        bw.WriteInt32(0);
      }

      internal void WriteAnimFile(BinaryWriterEx bw, int i) {
        bw.FillInt64(string.Format("AnimFileOffset{0}", (object) i),
                     bw.Position);
        bw.WriteInt64(this.AnimFileReference ? 1L : 0L);
        bw.WriteInt64(bw.Position + 8L);
        bw.ReserveInt64("AnimFileNameOffset");
        bw.WriteInt32(this.AnimFileUnk18);
        bw.WriteInt32(this.AnimFileUnk1C);
        bw.WriteInt64(0L);
        bw.WriteInt64(0L);
        bw.FillInt64("AnimFileNameOffset", bw.Position);
        if (!(this.AnimFileName != ""))
          return;
        bw.WriteUTF16(this.AnimFileName, true);
        bw.Pad(16);
      }

      internal Dictionary<float, long> WriteTimes(
          BinaryWriterEx bw,
          int animIndex) {
        SortedSet<float> sortedSet = new SortedSet<float>();
        foreach (TAE3.Event @event in this.Events) {
          sortedSet.Add(@event.StartTime);
          sortedSet.Add(@event.EndTime);
        }
        bw.FillInt32(string.Format("TimesCount{0}", (object) animIndex),
                     sortedSet.Count);
        if (sortedSet.Count == 0)
          bw.FillInt64(string.Format("TimesOffset{0}", (object) animIndex), 0L);
        else
          bw.FillInt64(string.Format("TimesOffset{0}", (object) animIndex),
                       bw.Position);
        Dictionary<float, long> dictionary = new Dictionary<float, long>();
        foreach (float index in sortedSet) {
          dictionary[index] = bw.Position;
          bw.WriteSingle(index);
        }
        bw.Pad(16);
        return dictionary;
      }

      internal List<long> WriteEventHeaders(
          BinaryWriterEx bw,
          int animIndex,
          Dictionary<float, long> timeOffsets) {
        List<long> longList = new List<long>(this.Events.Count);
        if (this.Events.Count > 0) {
          bw.FillInt64(
              string.Format("EventHeadersOffset{0}", (object) animIndex),
              bw.Position);
          for (int eventIndex = 0;
               eventIndex < this.Events.Count;
               ++eventIndex) {
            longList.Add(bw.Position);
            this.Events[eventIndex]
                .WriteHeader(bw, animIndex, eventIndex, timeOffsets);
          }
        } else
          bw.FillInt64(
              string.Format("EventHeadersOffset{0}", (object) animIndex),
              0L);
        return longList;
      }

      internal void WriteEventData(BinaryWriterEx bw, int i) {
        for (int eventIndex = 0; eventIndex < this.Events.Count; ++eventIndex)
          this.Events[eventIndex].WriteData(bw, i, eventIndex);
      }

      internal void WriteEventGroupHeaders(BinaryWriterEx bw, int i) {
        if (this.EventGroups.Count > 0) {
          bw.FillInt64(string.Format("EventGroupHeadersOffset{0}", (object) i),
                       bw.Position);
          for (int j = 0; j < this.EventGroups.Count; ++j)
            this.EventGroups[j].WriteHeader(bw, i, j);
        } else
          bw.FillInt64(string.Format("EventGroupHeadersOffset{0}", (object) i),
                       0L);
      }

      internal void WriteEventGroupData(
          BinaryWriterEx bw,
          int i,
          List<long> eventHeaderOffsets) {
        for (int j = 0; j < this.EventGroups.Count; ++j)
          this.EventGroups[j].WriteData(bw, i, j, eventHeaderOffsets);
      }
    }

    public class EventGroup {
      public TAE3.EventType Type;
      public List<int> Indices;

      public EventGroup(TAE3.EventType type) {
        this.Type = type;
        this.Indices = new List<int>();
      }

      internal EventGroup(BinaryReaderEx br, List<long> eventHeaderOffsets) {
        long num = br.ReadInt64();
        long offset1 = br.ReadInt64();
        long offset2 = br.ReadInt64();
        br.AssertInt64(new long[1]);
        br.StepIn(offset2);
        this.Type = br.ReadEnum64<TAE3.EventType>();
        br.AssertInt64(new long[1]);
        br.StepOut();
        br.StepIn(offset1);
        this.Indices = ((IEnumerable<int>) br.ReadInt32s((int) num))
                       .Select<int, int>(
                           (Func<int, int>) (offset
                                                  => eventHeaderOffsets
                                                      .FindIndex(
                                                          (Predicate<long>)
                                                          (headerOffset
                                                                => headerOffset ==
                                                                   (long) offset
                                                          ))))
                       .ToList<int>();
        br.StepOut();
      }

      internal void WriteHeader(BinaryWriterEx bw, int i, int j) {
        bw.WriteInt64((long) this.Indices.Count);
        bw.ReserveInt64(string.Format("EventGroupValuesOffset{0}:{1}",
                                      (object) i,
                                      (object) j));
        bw.ReserveInt64(string.Format("EventGroupTypeOffset{0}:{1}",
                                      (object) i,
                                      (object) j));
        bw.WriteInt64(0L);
      }

      internal void WriteData(
          BinaryWriterEx bw,
          int i,
          int j,
          List<long> eventHeaderOffsets) {
        bw.FillInt64(
            string.Format("EventGroupTypeOffset{0}:{1}",
                          (object) i,
                          (object) j),
            bw.Position);
        bw.WriteUInt64((ulong) this.Type);
        bw.WriteInt64(0L);
        bw.FillInt64(string.Format("EventGroupValuesOffset{0}:{1}",
                                   (object) i,
                                   (object) j),
                     bw.Position);
        for (int index = 0; index < this.Indices.Count; ++index)
          bw.WriteInt32((int) eventHeaderOffsets[this.Indices[index]]);
        bw.Pad(16);
      }
    }

    public enum EventType : ulong {
      JumpTable = 0,
      Unk001 = 1,
      Unk002 = 2,
      Unk005 = 5,
      Unk016 = 16,                       // 0x0000000000000010
      Unk017 = 17,                       // 0x0000000000000011
      Unk024 = 24,                       // 0x0000000000000018
      SwitchWeapon1 = 32,                // 0x0000000000000020
      SwitchWeapon2 = 33,                // 0x0000000000000021
      Unk034 = 34,                       // 0x0000000000000022
      Unk035 = 35,                       // 0x0000000000000023
      Unk064 = 64,                       // 0x0000000000000040
      Unk065 = 65,                       // 0x0000000000000041
      CreateSpEffect1 = 66,              // 0x0000000000000042
      CreateSpEffect2 = 67,              // 0x0000000000000043
      PlayFFX = 96,                      // 0x0000000000000060
      Unk110 = 110,                      // 0x000000000000006E
      HitEffect = 112,                   // 0x0000000000000070
      Unk113 = 113,                      // 0x0000000000000071
      Unk114 = 114,                      // 0x0000000000000072
      Unk115 = 115,                      // 0x0000000000000073
      Unk116 = 116,                      // 0x0000000000000074
      Unk117 = 117,                      // 0x0000000000000075
      Unk118 = 118,                      // 0x0000000000000076
      Unk119 = 119,                      // 0x0000000000000077
      Unk120 = 120,                      // 0x0000000000000078
      Unk121 = 121,                      // 0x0000000000000079
      PlaySound1 = 128,                  // 0x0000000000000080
      PlaySound2 = 129,                  // 0x0000000000000081
      PlaySound3 = 130,                  // 0x0000000000000082
      PlaySound4 = 131,                  // 0x0000000000000083
      PlaySound5 = 132,                  // 0x0000000000000084
      Unk136 = 136,                      // 0x0000000000000088
      Unk137 = 137,                      // 0x0000000000000089
      CreateDecal = 138,                 // 0x000000000000008A
      Unk144 = 144,                      // 0x0000000000000090
      Unk145 = 145,                      // 0x0000000000000091
      Unk150 = 150,                      // 0x0000000000000096
      Unk151 = 151,                      // 0x0000000000000097
      Unk161 = 161,                      // 0x00000000000000A1
      Unk192 = 192,                      // 0x00000000000000C0
      FadeOut = 193,                     // 0x00000000000000C1
      Unk194 = 194,                      // 0x00000000000000C2
      Unk224 = 224,                      // 0x00000000000000E0
      DisableStaminaRegen = 225,         // 0x00000000000000E1
      Unk226 = 226,                      // 0x00000000000000E2
      Unk227 = 227,                      // 0x00000000000000E3
      RagdollReviveTime = 228,           // 0x00000000000000E4
      Unk229 = 229,                      // 0x00000000000000E5
      SetEventMessageID = 231,           // 0x00000000000000E7
      Unk232 = 232,                      // 0x00000000000000E8
      ChangeDrawMask = 233,              // 0x00000000000000E9
      RollDistanceReduction = 236,       // 0x00000000000000EC
      CreateAISound = 237,               // 0x00000000000000ED
      Unk300 = 300,                      // 0x000000000000012C
      Unk301 = 301,                      // 0x000000000000012D
      AddSpEffectDragonForm = 302,       // 0x000000000000012E
      PlayAnimation = 303,               // 0x000000000000012F
      BehaviorThing = 304,               // 0x0000000000000130
      Unk306 = 306,                      // 0x0000000000000132
      CreateBehaviorPC = 307,            // 0x0000000000000133
      Unk308 = 308,                      // 0x0000000000000134
      Unk310 = 310,                      // 0x0000000000000136
      Unk311 = 311,                      // 0x0000000000000137
      Unk312 = 312,                      // 0x0000000000000138
      Unk317 = 317,                      // 0x000000000000013D
      Unk320 = 320,                      // 0x0000000000000140
      Unk330 = 330,                      // 0x000000000000014A
      EffectDuringThrow = 331,           // 0x000000000000014B
      Unk332 = 332,                      // 0x000000000000014C
      CreateSpEffect = 401,              // 0x0000000000000191
      Unk500 = 500,                      // 0x00000000000001F4
      Unk510 = 510,                      // 0x00000000000001FE
      Unk520 = 520,                      // 0x0000000000000208
      KingOfTheStorm = 522,              // 0x000000000000020A
      Unk600 = 600,                      // 0x0000000000000258
      Unk601 = 601,                      // 0x0000000000000259
      DebugAnimSpeed = 603,              // 0x000000000000025B
      Unk605 = 605,                      // 0x000000000000025D
      Unk606 = 606,                      // 0x000000000000025E
      Unk700 = 700,                      // 0x00000000000002BC
      EnableTurningDirection = 703,      // 0x00000000000002BF
      FacingAngleCorrection = 705,       // 0x00000000000002C1
      Unk707 = 707,                      // 0x00000000000002C3
      HideWeapon = 710,                  // 0x00000000000002C6
      HideModelMask = 711,               // 0x00000000000002C7
      DamageLevelModule = 712,           // 0x00000000000002C8
      ModelMask = 713,                   // 0x00000000000002C9
      DamageLevelFunction = 714,         // 0x00000000000002CA
      Unk715 = 715,                      // 0x00000000000002CB
      CultStart = 720,                   // 0x00000000000002D0
      Unk730 = 730,                      // 0x00000000000002DA
      Unk740 = 740,                      // 0x00000000000002E4
      IFrameState = 760,                 // 0x00000000000002F8
      BonePos = 770,                     // 0x0000000000000302
      BoneFixOn1 = 771,                  // 0x0000000000000303
      BoneFixOn2 = 772,                  // 0x0000000000000304
      TurnLowerBody = 781,               // 0x000000000000030D
      Unk782 = 782,                      // 0x000000000000030E
      SpawnBulletByCultSacrifice1 = 785, // 0x0000000000000311
      Unk786 = 786,                      // 0x0000000000000312
      Unk790 = 790,                      // 0x0000000000000316
      Unk791 = 791,                      // 0x0000000000000317
      HitEffect2 = 792,                  // 0x0000000000000318
      CultSacrifice1 = 793,              // 0x0000000000000319
      SacrificeEmpty = 794,              // 0x000000000000031A
      Toughness = 795,                   // 0x000000000000031B
      BringCultMenu = 796,               // 0x000000000000031C
      CeremonyParamID = 797,             // 0x000000000000031D
      CultSingle = 798,                  // 0x000000000000031E
      CultEmpty2 = 799,                  // 0x000000000000031F
      Unk800 = 800,                      // 0x0000000000000320
      Unk900 = 900,                      // 0x0000000000000384
    }

    public abstract class Event {
      public float StartTime;
      public float EndTime;

      public abstract TAE3.EventType Type { get; }

      internal Event(float startTime, float endTime) {
        this.StartTime = startTime;
        this.EndTime = endTime;
      }

      internal void WriteHeader(
          BinaryWriterEx bw,
          int animIndex,
          int eventIndex,
          Dictionary<float, long> timeOffsets) {
        bw.WriteInt64(timeOffsets[this.StartTime]);
        bw.WriteInt64(timeOffsets[this.EndTime]);
        bw.ReserveInt64(string.Format("EventDataOffset{0}:{1}",
                                      (object) animIndex,
                                      (object) eventIndex));
      }

      internal void WriteData(
          BinaryWriterEx bw,
          int animIndex,
          int eventIndex) {
        bw.FillInt64(string.Format("EventDataOffset{0}:{1}",
                                   (object) animIndex,
                                   (object) eventIndex),
                     bw.Position);
        bw.WriteUInt64((ulong) this.Type);
        bw.WriteInt64(bw.Position + 8L);
        this.WriteSpecific(bw);
        bw.Pad(16);
      }

      internal abstract void WriteSpecific(BinaryWriterEx bw);

      public override string ToString() {
        return string.Format("{0:D3} - {1:D3} {2}",
                             (object) (int) Math.Round(
                                 (double) this.StartTime * 30.0),
                             (object) (int) Math.Round(
                                 (double) this.EndTime * 30.0),
                             (object) this.Type);
      }

      internal static TAE3.Event Read(BinaryReaderEx br) {
        long offset1 = br.ReadInt64();
        long offset2 = br.ReadInt64();
        long offset3 = br.ReadInt64();
        float single1 = br.GetSingle(offset1);
        float single2 = br.GetSingle(offset2);
        br.StepIn(offset3);
        TAE3.EventType eventType = br.ReadEnum64<TAE3.EventType>();
        br.AssertInt64(br.Position + 8L);
        TAE3.Event @event;
        if (eventType <= TAE3.EventType.CreateAISound) {
          if (eventType <= TAE3.EventType.CreateSpEffect2) {
            if (eventType <= TAE3.EventType.Unk017) {
              switch (eventType) {
                case TAE3.EventType.JumpTable:
                  @event =
                      (TAE3.Event) new TAE3.Event.JumpTable(
                          single1,
                          single2,
                          br);
                  goto label_147;
                case TAE3.EventType.Unk001:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk001(single1, single2, br);
                  goto label_147;
                case TAE3.EventType.Unk002:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk002(single1, single2, br);
                  goto label_147;
                case TAE3.EventType.Unk001 | TAE3.EventType.Unk002:
                case (TAE3.EventType) 4:
                  break;
                case TAE3.EventType.Unk005:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk005(single1, single2, br);
                  goto label_147;
                default:
                  if (eventType != TAE3.EventType.Unk016) {
                    if (eventType == TAE3.EventType.Unk017) {
                      @event =
                          (TAE3.Event) new TAE3.Event.Unk017(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    } else
                      break;
                  } else {
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk016(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  }
              }
            } else if (eventType != TAE3.EventType.Unk024) {
              switch (eventType - 32UL) {
                case TAE3.EventType.JumpTable:
                  @event =
                      (TAE3.Event) new TAE3.Event.SwitchWeapon1(
                          single1,
                          single2,
                          br);
                  goto label_147;
                case TAE3.EventType.Unk001:
                  @event =
                      (TAE3.Event) new TAE3.Event.SwitchWeapon2(
                          single1,
                          single2,
                          br);
                  goto label_147;
                case TAE3.EventType.Unk002:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk034(single1, single2, br);
                  goto label_147;
                case TAE3.EventType.Unk001 | TAE3.EventType.Unk002:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk035(single1, single2, br);
                  goto label_147;
                default:
                  switch (eventType - 64UL) {
                    case TAE3.EventType.JumpTable:
                      @event =
                          (TAE3.Event) new TAE3.Event.Unk064(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case TAE3.EventType.Unk001:
                      @event =
                          (TAE3.Event) new TAE3.Event.Unk065(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case TAE3.EventType.Unk002:
                      @event =
                          (TAE3.Event) new TAE3.Event.CreateSpEffect1(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case TAE3.EventType.Unk001 | TAE3.EventType.Unk002:
                      @event =
                          (TAE3.Event) new TAE3.Event.CreateSpEffect2(
                              single1,
                              single2,
                              br);
                      goto label_147;
                  }
                  break;
              }
            } else {
              @event = (TAE3.Event) new TAE3.Event.Unk024(single1, single2, br);
              goto label_147;
            }
          } else if (eventType <= TAE3.EventType.Unk161) {
            if (eventType != TAE3.EventType.PlayFFX) {
              long num = (long) (eventType - 110UL);
              if ((ulong) num <= 41UL) {
                switch ((uint) num) {
                  case 0:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk110(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 1:
                  case 12:
                  case 13:
                  case 14:
                  case 15:
                  case 16:
                  case 17:
                  case 23:
                  case 24:
                  case 25:
                  case 26:
                  case 29:
                  case 30:
                  case 31:
                  case 32:
                  case 33:
                  case 36:
                  case 37:
                  case 38:
                  case 39:
                    goto label_146;
                  case 2:
                    @event =
                        (TAE3.Event) new TAE3.Event.HitEffect(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 3:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk113(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 4:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk114(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 5:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk115(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 6:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk116(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 7:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk117(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 8:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk118(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 9:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk119(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 10:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk120(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 11:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk121(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 18:
                    @event =
                        (TAE3.Event) new TAE3.Event.PlaySound1(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 19:
                    @event =
                        (TAE3.Event) new TAE3.Event.PlaySound2(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 20:
                    @event =
                        (TAE3.Event) new TAE3.Event.PlaySound3(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 21:
                    @event =
                        (TAE3.Event) new TAE3.Event.PlaySound4(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 22:
                    @event =
                        (TAE3.Event) new TAE3.Event.PlaySound5(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 27:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk137(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 28:
                    @event =
                        (TAE3.Event) new TAE3.Event.CreateDecal(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 34:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk144(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 35:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk145(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 40:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk150(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 41:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk151(
                            single1,
                            single2,
                            br);
                    goto label_147;
                }
              }
              if (eventType == TAE3.EventType.Unk161) {
                @event =
                    (TAE3.Event) new TAE3.Event.Unk161(single1, single2, br);
                goto label_147;
              }
            } else {
              @event =
                  (TAE3.Event) new TAE3.Event.PlayFFX(single1, single2, br);
              goto label_147;
            }
          } else if (eventType != TAE3.EventType.FadeOut) {
            if (eventType != TAE3.EventType.Unk194) {
              long num = (long) (eventType - 224UL);
              if ((ulong) num <= 13UL) {
                switch ((uint) num) {
                  case 0:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk224(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 1:
                    @event =
                        (TAE3.Event) new TAE3.Event.DisableStaminaRegen(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 2:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk226(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 3:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk227(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 4:
                    @event =
                        (TAE3.Event) new TAE3.Event.RagdollReviveTime(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 5:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk229(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 7:
                    @event =
                        (TAE3.Event) new TAE3.Event.SetEventMessageID(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 8:
                    @event =
                        (TAE3.Event) new TAE3.Event.Unk232(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 9:
                    @event =
                        (TAE3.Event) new TAE3.Event.ChangeDrawMask(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 12:
                    @event =
                        (TAE3.Event) new TAE3.Event.RollDistanceReduction(
                            single1,
                            single2,
                            br);
                    goto label_147;
                  case 13:
                    @event =
                        (TAE3.Event) new TAE3.Event.CreateAISound(
                            single1,
                            single2,
                            br);
                    goto label_147;
                }
              }
            } else {
              @event = (TAE3.Event) new TAE3.Event.Unk194(single1, single2, br);
              goto label_147;
            }
          } else {
            @event = (TAE3.Event) new TAE3.Event.FadeOut(single1, single2, br);
            goto label_147;
          }
        } else if (eventType <= TAE3.EventType.Unk520) {
          if (eventType <= TAE3.EventType.CreateSpEffect) {
            long num = (long) (eventType - 300UL);
            if ((ulong) num <= 20UL) {
              switch ((uint) num) {
                case 0:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk300(single1, single2, br);
                  goto label_147;
                case 1:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk301(single1, single2, br);
                  goto label_147;
                case 2:
                  @event =
                      (TAE3.Event) new TAE3.Event.AddSpEffectDragonForm(
                          single1,
                          single2,
                          br);
                  goto label_147;
                case 3:
                  @event =
                      (TAE3.Event) new TAE3.Event.PlayAnimation(
                          single1,
                          single2,
                          br);
                  goto label_147;
                case 4:
                  @event =
                      (TAE3.Event) new TAE3.Event.BehaviorThing(
                          single1,
                          single2,
                          br);
                  goto label_147;
                case 5:
                case 6:
                case 9:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                  goto label_146;
                case 7:
                  @event =
                      (TAE3.Event) new TAE3.Event.CreateBehaviorPC(
                          single1,
                          single2,
                          br);
                  goto label_147;
                case 8:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk308(single1, single2, br);
                  goto label_147;
                case 10:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk310(single1, single2, br);
                  goto label_147;
                case 11:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk311(single1, single2, br);
                  goto label_147;
                case 12:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk312(single1, single2, br);
                  goto label_147;
                case 20:
                  @event =
                      (TAE3.Event) new TAE3.Event.Unk320(single1, single2, br);
                  goto label_147;
              }
            }
            switch (eventType - 330UL) {
              case TAE3.EventType.JumpTable:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk330(single1, single2, br);
                goto label_147;
              case TAE3.EventType.Unk001:
                @event =
                    (TAE3.Event) new TAE3.Event.EffectDuringThrow(
                        single1,
                        single2,
                        br);
                goto label_147;
              case TAE3.EventType.Unk002:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk332(single1, single2, br);
                goto label_147;
              default:
                if (eventType == TAE3.EventType.CreateSpEffect) {
                  @event =
                      (TAE3.Event) new TAE3.Event.CreateSpEffect(
                          single1,
                          single2,
                          br);
                  goto label_147;
                } else
                  break;
            }
          } else if (eventType != TAE3.EventType.Unk500) {
            if (eventType != TAE3.EventType.Unk510) {
              if (eventType == TAE3.EventType.Unk520) {
                @event =
                    (TAE3.Event) new TAE3.Event.Unk520(single1, single2, br);
                goto label_147;
              }
            } else {
              @event = (TAE3.Event) new TAE3.Event.Unk510(single1, single2, br);
              goto label_147;
            }
          } else {
            @event = (TAE3.Event) new TAE3.Event.Unk500(single1, single2, br);
            goto label_147;
          }
        } else if (eventType <= TAE3.EventType.CultStart) {
          if (eventType != TAE3.EventType.KingOfTheStorm) {
            switch (eventType - 600UL) {
              case TAE3.EventType.JumpTable:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk600(single1, single2, br);
                goto label_147;
              case TAE3.EventType.Unk001:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk601(single1, single2, br);
                goto label_147;
              case TAE3.EventType.Unk002:
              case (TAE3.EventType) 4:
                break;
              case TAE3.EventType.Unk001 | TAE3.EventType.Unk002:
                @event =
                    (TAE3.Event) new TAE3.Event.DebugAnimSpeed(
                        single1,
                        single2,
                        br);
                goto label_147;
              case TAE3.EventType.Unk005:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk605(single1, single2, br);
                goto label_147;
              case (TAE3.EventType) 6:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk606(single1, single2, br);
                goto label_147;
              default:
                long num1 = (long) (eventType - 700UL);
                if ((ulong) num1 <= 20UL) {
                  switch ((uint) num1) {
                    case 0:
                      @event =
                          (TAE3.Event) new TAE3.Event.Unk700(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 3:
                      @event =
                          (TAE3.Event) new TAE3.Event.EnableTurningDirection(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 5:
                      @event =
                          (TAE3.Event) new TAE3.Event.FacingAngleCorrection(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 7:
                      @event =
                          (TAE3.Event) new TAE3.Event.Unk707(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 10:
                      @event =
                          (TAE3.Event) new TAE3.Event.HideWeapon(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 11:
                      @event =
                          (TAE3.Event) new TAE3.Event.HideModelMask(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 12:
                      @event =
                          (TAE3.Event) new TAE3.Event.DamageLevelModule(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 13:
                      @event =
                          (TAE3.Event) new TAE3.Event.ModelMask(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 14:
                      @event =
                          (TAE3.Event) new TAE3.Event.DamageLevelFunction(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 15:
                      @event =
                          (TAE3.Event) new TAE3.Event.Unk715(
                              single1,
                              single2,
                              br);
                      goto label_147;
                    case 20:
                      @event =
                          (TAE3.Event) new TAE3.Event.CultStart(
                              single1,
                              single2,
                              br);
                      goto label_147;
                  }
                } else
                  break;
                break;
            }
          } else {
            @event =
                (TAE3.Event) new TAE3.Event.KingOfTheStorm(
                    single1,
                    single2,
                    br);
            goto label_147;
          }
        } else if (eventType <= TAE3.EventType.Unk740) {
          if (eventType != TAE3.EventType.Unk730) {
            if (eventType == TAE3.EventType.Unk740) {
              @event = (TAE3.Event) new TAE3.Event.Unk740(single1, single2, br);
              goto label_147;
            }
          } else {
            @event = (TAE3.Event) new TAE3.Event.Unk730(single1, single2, br);
            goto label_147;
          }
        } else if (eventType != TAE3.EventType.IFrameState) {
          long num2 = (long) (eventType - 770UL);
          if ((ulong) num2 <= 30UL) {
            switch ((uint) num2) {
              case 0:
                @event =
                    (TAE3.Event) new TAE3.Event.BonePos(single1, single2, br);
                goto label_147;
              case 1:
                @event =
                    (TAE3.Event) new TAE3.Event.BoneFixOn1(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 2:
                @event =
                    (TAE3.Event) new TAE3.Event.BoneFixOn2(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 11:
                @event =
                    (TAE3.Event) new TAE3.Event.TurnLowerBody(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 12:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk782(single1, single2, br);
                goto label_147;
              case 15:
                @event =
                    (TAE3.Event) new TAE3.Event.SpawnBulletByCultSacrifice1(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 16:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk786(single1, single2, br);
                goto label_147;
              case 20:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk790(single1, single2, br);
                goto label_147;
              case 21:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk791(single1, single2, br);
                goto label_147;
              case 22:
                @event =
                    (TAE3.Event) new TAE3.Event.HitEffect2(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 23:
                @event =
                    (TAE3.Event) new TAE3.Event.CultSacrifice1(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 24:
                @event =
                    (TAE3.Event) new TAE3.Event.SacrificeEmpty(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 25:
                @event =
                    (TAE3.Event) new TAE3.Event.Toughness(single1, single2, br);
                goto label_147;
              case 26:
                @event =
                    (TAE3.Event) new TAE3.Event.BringCultMenu(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 27:
                @event =
                    (TAE3.Event) new TAE3.Event.CeremonyParamID(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 28:
                @event =
                    (TAE3.Event) new TAE3.Event.CultSingle(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 29:
                @event =
                    (TAE3.Event) new TAE3.Event.CultEmpty2(
                        single1,
                        single2,
                        br);
                goto label_147;
              case 30:
                @event =
                    (TAE3.Event) new TAE3.Event.Unk800(single1, single2, br);
                goto label_147;
            }
          }
        } else {
          @event =
              (TAE3.Event) new TAE3.Event.IFrameState(single1, single2, br);
          goto label_147;
        }
        label_146:
        throw new NotImplementedException();
        label_147:
        if (@event.Type != eventType)
          throw new InvalidProgramException(
              "There is a typo in TAE3.Event.cs. Please bully me.");
        br.StepOut();
        return @event;
      }

      public class JumpTable : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.JumpTable; }
        }

        public int JumpTableID { get; set; }

        public int Unk04 { get; set; }

        public int Unk08 { get; set; }

        public short Unk0C { get; set; }

        public short Unk0E { get; set; }

        internal JumpTable(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.JumpTableID = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadInt16();
          this.Unk0E = br.ReadInt16();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.JumpTableID);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt16(this.Unk0C);
          bw.WriteInt16(this.Unk0E);
        }

        public override string ToString() {
          return string.Format("{0} : {1}",
                               (object) base.ToString(),
                               (object) this.JumpTableID);
        }
      }

      public class Unk001 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk001; }
        }

        public int Unk00 { get; set; }

        public int Unk04 { get; set; }

        public int Condition { get; set; }

        public byte Unk0C { get; set; }

        public byte Unk0D { get; set; }

        public short StateInfo { get; set; }

        public Unk001(
            float startTime,
            float endTime,
            int unk00,
            int unk04,
            int condition,
            byte unk0C,
            byte unk0D,
            short stateInfo)
            : base(startTime, endTime) {
          this.Unk00 = unk00;
          this.Unk04 = unk04;
          this.Condition = condition;
          this.Unk0C = unk0C;
          this.Unk0D = unk0D;
          this.StateInfo = stateInfo;
        }

        internal Unk001(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Condition = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          this.Unk0D = br.ReadByte();
          this.StateInfo = br.ReadInt16();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Condition);
          bw.WriteByte(this.Unk0C);
          bw.WriteByte(this.Unk0D);
          bw.WriteInt16(this.StateInfo);
        }
      }

      public class Unk002 : TAE3.Event {
        public int Unk00;
        public int Unk04;
        public int ChrAsmStyle;
        public byte Unk0C;
        public byte Unk0D;
        public ushort Unk0E;
        public ushort Unk10;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk002; }
        }

        internal Unk002(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.ChrAsmStyle = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          this.Unk0D = br.ReadByte();
          this.Unk0E = br.ReadUInt16();
          this.Unk10 = br.ReadUInt16();
          int num = (int) br.AssertInt16(new short[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.ChrAsmStyle);
          bw.WriteByte(this.Unk0C);
          bw.WriteByte(this.Unk0D);
          bw.WriteUInt16(this.Unk0E);
          bw.WriteUInt16(this.Unk10);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(0);
        }
      }

      public class Unk005 : TAE3.Event {
        public int Unk00;
        public int Unk04;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk005; }
        }

        internal Unk005(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
        }
      }

      public class Unk016 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk016; }
        }

        internal Unk016(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {}

        internal override void WriteSpecific(BinaryWriterEx bw) {}
      }

      public class Unk017 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk017; }
        }

        internal Unk017(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk024 : TAE3.Event {
        public int Unk00;
        public int Unk04;
        public int Unk08;
        public int Unk0C;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk024; }
        }

        internal Unk024(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(this.Unk0C);
        }
      }

      public class SwitchWeapon1 : TAE3.Event {
        public int SwitchState;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.SwitchWeapon1; }
        }

        internal SwitchWeapon1(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SwitchState = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SwitchState);
          bw.WriteInt32(0);
        }
      }

      public class SwitchWeapon2 : TAE3.Event {
        public int SwitchState;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.SwitchWeapon2; }
        }

        internal SwitchWeapon2(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SwitchState = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SwitchState);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk034 : TAE3.Event {
        public int State;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk034; }
        }

        internal Unk034(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.State = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.State);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk035 : TAE3.Event {
        public int State;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk035; }
        }

        internal Unk035(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.State = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.State);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk064 : TAE3.Event {
        public int Unk00;
        public ushort Unk04;
        public ushort Unk06;
        public byte Unk08;
        public byte Unk09;
        public byte Unk0A;
        public byte Unk0B;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk064; }
        }

        internal Unk064(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadUInt16();
          this.Unk06 = br.ReadUInt16();
          this.Unk08 = br.ReadByte();
          this.Unk09 = br.ReadByte();
          this.Unk0A = br.ReadByte();
          this.Unk0B = br.ReadByte();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteUInt16(this.Unk04);
          bw.WriteUInt16(this.Unk06);
          bw.WriteByte(this.Unk08);
          bw.WriteByte(this.Unk09);
          bw.WriteByte(this.Unk0A);
          bw.WriteByte(this.Unk0B);
          bw.WriteInt32(0);
        }
      }

      public class Unk065 : TAE3.Event {
        public int Unk00;
        public byte Unk04;
        public byte Unk05;
        public ushort Unk06;
        public int Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk065; }
        }

        internal Unk065(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadByte();
          this.Unk05 = br.ReadByte();
          this.Unk06 = br.ReadUInt16();
          this.Unk08 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteByte(this.Unk04);
          bw.WriteByte(this.Unk05);
          bw.WriteUInt16(this.Unk06);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(0);
        }
      }

      public class CreateSpEffect1 : TAE3.Event {
        public int SpEffectID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CreateSpEffect1; }
        }

        public CreateSpEffect1(float startTime, float endTime, int speffectID)
            : base(startTime, endTime) {
          this.SpEffectID = speffectID;
        }

        internal CreateSpEffect1(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SpEffectID = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SpEffectID);
          bw.WriteInt32(0);
        }
      }

      public class CreateSpEffect2 : TAE3.Event {
        public int SpEffectID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CreateSpEffect2; }
        }

        internal CreateSpEffect2(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SpEffectID = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SpEffectID);
          bw.WriteInt32(0);
        }
      }

      public class PlayFFX : TAE3.Event {
        public int FFXID;
        public int Unk04;
        public int Unk08;
        public sbyte State0;
        public sbyte State1;
        public sbyte GhostFFXCondition;
        public byte Unk0F;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.PlayFFX; }
        }

        internal PlayFFX(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.FFXID = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.State0 = br.ReadSByte();
          this.State1 = br.ReadSByte();
          this.GhostFFXCondition = br.ReadSByte();
          this.Unk0F = br.ReadByte();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.FFXID);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteSByte(this.State0);
          bw.WriteSByte(this.State1);
          bw.WriteSByte(this.GhostFFXCondition);
          bw.WriteByte(this.Unk0F);
        }
      }

      public class Unk110 : TAE3.Event {
        public int ID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk110; }
        }

        internal Unk110(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.ID = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.ID);
          bw.WriteInt32(0);
        }
      }

      public class HitEffect : TAE3.Event {
        public int Size;
        public int Unk04;
        public int Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.HitEffect; }
        }

        internal HitEffect(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Size = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Size);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(0);
        }
      }

      public class Unk113 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk113; }
        }

        internal Unk113(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk114 : TAE3.Event {
        public int Unk00;
        public ushort Unk04;
        public ushort Unk06;
        public int Unk08;
        public byte Unk0C;
        public sbyte Unk0D;
        public sbyte Unk0E;
        public byte Unk0F;
        public byte Unk10;
        public byte Unk11;
        public short Unk12;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk114; }
        }

        internal Unk114(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadUInt16();
          this.Unk06 = br.ReadUInt16();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          this.Unk0D = br.ReadSByte();
          this.Unk0E = br.ReadSByte();
          this.Unk0F = br.ReadByte();
          this.Unk10 = br.ReadByte();
          this.Unk11 = br.ReadByte();
          this.Unk12 = br.ReadInt16();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteUInt16(this.Unk04);
          bw.WriteUInt16(this.Unk06);
          bw.WriteInt32(this.Unk08);
          bw.WriteByte(this.Unk0C);
          bw.WriteSByte(this.Unk0D);
          bw.WriteSByte(this.Unk0E);
          bw.WriteByte(this.Unk0F);
          bw.WriteByte(this.Unk10);
          bw.WriteByte(this.Unk11);
          bw.WriteInt16(this.Unk12);
          bw.WriteInt32(0);
        }
      }

      public class Unk115 : TAE3.Event {
        public int Unk00;
        public ushort Unk04;
        public ushort Unk06;
        public int Unk08;
        public byte Unk0C;
        public byte Unk0D;
        public byte Unk0E;
        public byte Unk0F;
        public byte Unk10;
        public byte Unk11;
        public short Unk12;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk115; }
        }

        internal Unk115(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadUInt16();
          this.Unk06 = br.ReadUInt16();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          this.Unk0D = br.ReadByte();
          this.Unk0E = br.ReadByte();
          this.Unk0F = br.ReadByte();
          this.Unk10 = br.ReadByte();
          this.Unk11 = br.ReadByte();
          this.Unk12 = br.ReadInt16();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteUInt16(this.Unk04);
          bw.WriteUInt16(this.Unk06);
          bw.WriteInt32(this.Unk08);
          bw.WriteByte(this.Unk0C);
          bw.WriteByte(this.Unk0D);
          bw.WriteByte(this.Unk0E);
          bw.WriteByte(this.Unk0F);
          bw.WriteByte(this.Unk10);
          bw.WriteByte(this.Unk11);
          bw.WriteInt16(this.Unk12);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk116 : TAE3.Event {
        public int Unk00;
        public int Unk04;
        public int Unk08;
        public int Unk0C;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk116; }
        }

        internal Unk116(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(this.Unk0C);
        }
      }

      public class Unk117 : TAE3.Event {
        public int Unk00;
        public int Unk04;
        public int Unk08;
        public byte Unk0C;
        public byte Unk0D;
        public byte Unk0E;
        public byte Unk0F;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk117; }
        }

        internal Unk117(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          this.Unk0D = br.ReadByte();
          this.Unk0E = br.ReadByte();
          this.Unk0F = br.ReadByte();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteByte(this.Unk0C);
          bw.WriteByte(this.Unk0D);
          bw.WriteByte(this.Unk0E);
          bw.WriteByte(this.Unk0F);
        }
      }

      public class Unk118 : TAE3.Event {
        public int Unk00;
        public ushort Unk04;
        public ushort Unk06;
        public ushort Unk08;
        public ushort Unk0A;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk118; }
        }

        internal Unk118(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadUInt16();
          this.Unk06 = br.ReadUInt16();
          this.Unk08 = br.ReadUInt16();
          this.Unk0A = br.ReadUInt16();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteUInt16(this.Unk04);
          bw.WriteUInt16(this.Unk06);
          bw.WriteUInt16(this.Unk08);
          bw.WriteUInt16(this.Unk0A);
          bw.WriteInt32(0);
        }
      }

      public class Unk119 : TAE3.Event {
        public int Unk00;
        public int Unk04;
        public int Unk08;
        public byte Unk0C;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk119; }
        }

        internal Unk119(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteByte(this.Unk0C);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
        }
      }

      public class Unk120 : TAE3.Event {
        public int ChrType;
        public int Unk30;
        public int Unk34;
        public byte Unk38;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk120; }
        }

        public int[] FFXIDs { get; private set; }

        internal Unk120(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.ChrType = br.ReadInt32();
          this.FFXIDs = br.ReadInt32s(11);
          this.Unk30 = br.ReadInt32();
          this.Unk34 = br.ReadInt32();
          this.Unk38 = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.ChrType);
          bw.WriteInt32s((IList<int>) this.FFXIDs);
          bw.WriteInt32(this.Unk30);
          bw.WriteInt32(this.Unk34);
          bw.WriteByte(this.Unk38);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class Unk121 : TAE3.Event {
        public int Unk00;
        public ushort Unk04;
        public byte Unk06;
        public byte Unk07;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk121; }
        }

        internal Unk121(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadUInt16();
          this.Unk06 = br.ReadByte();
          this.Unk07 = br.ReadByte();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteUInt16(this.Unk04);
          bw.WriteByte(this.Unk06);
          bw.WriteByte(this.Unk07);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class PlaySound1 : TAE3.Event {
        public int SoundType;
        public int SoundID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.PlaySound1; }
        }

        internal PlaySound1(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SoundType = br.ReadInt32();
          this.SoundID = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SoundType);
          bw.WriteInt32(this.SoundID);
        }
      }

      public class PlaySound2 : TAE3.Event {
        public int SoundType;
        public int SoundID;
        public int Unk08;
        public int Unk0C;
        public int Unk10;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.PlaySound2; }
        }

        internal PlaySound2(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SoundType = br.ReadInt32();
          this.SoundID = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadInt32();
          this.Unk10 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SoundType);
          bw.WriteInt32(this.SoundID);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(this.Unk0C);
          bw.WriteInt32(this.Unk10);
          bw.WriteInt32(0);
        }
      }

      public class PlaySound3 : TAE3.Event {
        public int SoundType;
        public int SoundID;
        public float Unk08;
        public float Unk0C;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.PlaySound3; }
        }

        internal PlaySound3(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SoundType = br.ReadInt32();
          this.SoundID = br.ReadInt32();
          this.Unk08 = br.ReadSingle();
          this.Unk0C = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SoundType);
          bw.WriteInt32(this.SoundID);
          bw.WriteSingle(this.Unk08);
          bw.WriteSingle(this.Unk0C);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class PlaySound4 : TAE3.Event {
        public int SoundType;
        public int SoundID;
        public int Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.PlaySound4; }
        }

        internal PlaySound4(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SoundType = br.ReadInt32();
          this.SoundID = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SoundType);
          bw.WriteInt32(this.SoundID);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(0);
        }
      }

      public class PlaySound5 : TAE3.Event {
        public int SoundType;
        public int SoundID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.PlaySound5; }
        }

        internal PlaySound5(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SoundType = br.ReadInt32();
          this.SoundID = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SoundType);
          bw.WriteInt32(this.SoundID);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk137 : TAE3.Event {
        public int Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk137; }
        }

        internal Unk137(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(0);
        }
      }

      public class CreateDecal : TAE3.Event {
        public int DecalParamID;
        public int Unk04;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CreateDecal; }
        }

        internal CreateDecal(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.DecalParamID = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.DecalParamID);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk144 : TAE3.Event {
        public ushort Unk00;
        public ushort Unk02;
        public float Unk04;
        public float Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk144; }
        }

        internal Unk144(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadUInt16();
          this.Unk02 = br.ReadUInt16();
          this.Unk04 = br.ReadSingle();
          this.Unk08 = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteUInt16(this.Unk00);
          bw.WriteUInt16(this.Unk02);
          bw.WriteSingle(this.Unk04);
          bw.WriteSingle(this.Unk08);
          bw.WriteInt32(0);
        }
      }

      public class Unk145 : TAE3.Event {
        public short Unk00;
        public short Condition;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk145; }
        }

        internal Unk145(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt16();
          this.Condition = br.ReadInt16();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt16(this.Unk00);
          bw.WriteInt16(this.Condition);
          bw.WriteInt32(0);
        }
      }

      public class Unk150 : TAE3.Event {
        public int Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk150; }
        }

        internal Unk150(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk151 : TAE3.Event {
        public int DummyPointID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk151; }
        }

        internal Unk151(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.DummyPointID = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.DummyPointID);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk161 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk161; }
        }

        internal Unk161(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class FadeOut : TAE3.Event {
        public float GhostVal1;
        public float GhostVal2;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.FadeOut; }
        }

        internal FadeOut(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.GhostVal1 = br.ReadSingle();
          this.GhostVal2 = br.ReadSingle();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.GhostVal1);
          bw.WriteSingle(this.GhostVal2);
        }
      }

      public class Unk194 : TAE3.Event {
        public ushort Unk00;
        public ushort Unk02;
        public ushort Unk04;
        public ushort Unk06;
        public float Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk194; }
        }

        internal Unk194(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadUInt16();
          this.Unk02 = br.ReadUInt16();
          this.Unk04 = br.ReadUInt16();
          this.Unk06 = br.ReadUInt16();
          this.Unk08 = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteUInt16(this.Unk00);
          bw.WriteUInt16(this.Unk02);
          bw.WriteUInt16(this.Unk04);
          bw.WriteUInt16(this.Unk06);
          bw.WriteSingle(this.Unk08);
          bw.WriteInt32(0);
        }
      }

      public class Unk224 : TAE3.Event {
        public float Unk00;
        public int Unk04;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk224; }
        }

        internal Unk224(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          this.Unk04 = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteInt32(this.Unk04);
        }
      }

      public class DisableStaminaRegen : TAE3.Event {
        public byte State;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.DisableStaminaRegen; }
        }

        internal DisableStaminaRegen(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.State = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.State);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class Unk226 : TAE3.Event {
        public byte State;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk226; }
        }

        internal Unk226(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.State = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.State);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class Unk227 : TAE3.Event {
        public int Mask;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk227; }
        }

        internal Unk227(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Mask = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Mask);
          bw.WriteInt32(0);
        }
      }

      public class RagdollReviveTime : TAE3.Event {
        public float Unk00;
        public float ReviveTimer;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.RagdollReviveTime; }
        }

        internal RagdollReviveTime(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          this.ReviveTimer = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteSingle(this.ReviveTimer);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk229 : TAE3.Event {
        public int Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk229; }
        }

        internal Unk229(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(0);
        }
      }

      public class SetEventMessageID : TAE3.Event {
        public int EventMessageID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.SetEventMessageID; }
        }

        internal SetEventMessageID(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.EventMessageID = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.EventMessageID);
          bw.WriteInt32(0);
        }
      }

      public class Unk232 : TAE3.Event {
        public byte Unk00;
        public byte Unk01;
        public byte Unk02;
        public byte Unk03;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk232; }
        }

        internal Unk232(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          this.Unk02 = br.ReadByte();
          this.Unk03 = br.ReadByte();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte(this.Unk02);
          bw.WriteByte(this.Unk03);
          bw.WriteInt32(0);
        }
      }

      public class ChangeDrawMask : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.ChangeDrawMask; }
        }

        public byte[] DrawMask { get; private set; }

        internal ChangeDrawMask(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.DrawMask = br.ReadBytes(32);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteBytes(this.DrawMask);
        }
      }

      public class RollDistanceReduction : TAE3.Event {
        public float Unk00;
        public float Unk04;
        public bool RollType;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.RollDistanceReduction; }
        }

        internal RollDistanceReduction(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          this.Unk04 = br.ReadSingle();
          this.RollType = br.ReadBoolean();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteSingle(this.Unk04);
          bw.WriteBoolean(this.RollType);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class CreateAISound : TAE3.Event {
        public int AISoundID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CreateAISound; }
        }

        internal CreateAISound(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.AISoundID = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.AISoundID);
          bw.WriteInt32(0);
        }
      }

      public class Unk300 : TAE3.Event {
        public short JumpTableID1;
        public short JumpTableID2;
        public float Unk04;
        public float Unk08;
        public int Unk0C;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk300; }
        }

        internal Unk300(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.JumpTableID1 = br.ReadInt16();
          this.JumpTableID2 = br.ReadInt16();
          this.Unk04 = br.ReadSingle();
          this.Unk08 = br.ReadSingle();
          this.Unk0C = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt16(this.JumpTableID1);
          bw.WriteInt16(this.JumpTableID2);
          bw.WriteSingle(this.Unk04);
          bw.WriteSingle(this.Unk08);
          bw.WriteInt32(this.Unk0C);
        }
      }

      public class Unk301 : TAE3.Event {
        public int Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk301; }
        }

        internal Unk301(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(0);
        }
      }

      public class AddSpEffectDragonForm : TAE3.Event {
        public int SpEffectID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.AddSpEffectDragonForm; }
        }

        internal AddSpEffectDragonForm(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SpEffectID = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SpEffectID);
          bw.WriteInt32(0);
        }
      }

      public class PlayAnimation : TAE3.Event {
        public int AnimationID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.PlayAnimation; }
        }

        internal PlayAnimation(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.AnimationID = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.AnimationID);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class BehaviorThing : TAE3.Event {
        public ushort Unk00;
        public short Unk02;
        public int BehaviorListID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.BehaviorThing; }
        }

        internal BehaviorThing(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadUInt16();
          this.Unk02 = br.ReadInt16();
          this.BehaviorListID = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteUInt16(this.Unk00);
          bw.WriteInt16(this.Unk02);
          bw.WriteInt32(this.BehaviorListID);
        }
      }

      public class CreateBehaviorPC : TAE3.Event {
        public short Unk00;
        public short Unk02;
        public int Condition;
        public int Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CreateBehaviorPC; }
        }

        internal CreateBehaviorPC(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt16();
          this.Unk02 = br.ReadInt16();
          this.Condition = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt16(this.Unk00);
          bw.WriteInt16(this.Unk02);
          bw.WriteInt32(this.Condition);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(0);
        }
      }

      public class Unk308 : TAE3.Event {
        public float Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk308; }
        }

        internal Unk308(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk310 : TAE3.Event {
        public byte Unk00;
        public byte Unk01;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk310; }
        }

        internal Unk310(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class Unk311 : TAE3.Event {
        public byte Unk00;
        public byte Unk01;
        public byte Unk02;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk311; }
        }

        internal Unk311(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          this.Unk02 = br.ReadByte();
          int num = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte(this.Unk02);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk312 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk312; }
        }

        public byte[] BehaviorMask { get; private set; }

        internal Unk312(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.BehaviorMask = br.ReadBytes(32);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteBytes(this.BehaviorMask);
        }
      }

      public class Unk320 : TAE3.Event {
        public bool Unk00;
        public bool Unk01;
        public bool Unk02;
        public bool Unk03;
        public bool Unk04;
        public bool Unk05;
        public bool Unk06;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk320; }
        }

        internal Unk320(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadBoolean();
          this.Unk01 = br.ReadBoolean();
          this.Unk02 = br.ReadBoolean();
          this.Unk03 = br.ReadBoolean();
          this.Unk04 = br.ReadBoolean();
          this.Unk05 = br.ReadBoolean();
          this.Unk06 = br.ReadBoolean();
          int num = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteBoolean(this.Unk00);
          bw.WriteBoolean(this.Unk01);
          bw.WriteBoolean(this.Unk02);
          bw.WriteBoolean(this.Unk03);
          bw.WriteBoolean(this.Unk04);
          bw.WriteBoolean(this.Unk05);
          bw.WriteBoolean(this.Unk06);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk330 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk330; }
        }

        internal Unk330(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class EffectDuringThrow : TAE3.Event {
        public int SpEffectID1;
        public int SpEffectID2;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.EffectDuringThrow; }
        }

        internal EffectDuringThrow(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SpEffectID1 = br.ReadInt32();
          this.SpEffectID2 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SpEffectID1);
          bw.WriteInt32(this.SpEffectID2);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk332 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk332; }
        }

        internal Unk332(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class CreateSpEffect : TAE3.Event {
        public int SpEffectID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CreateSpEffect; }
        }

        public CreateSpEffect(float startTime, float endTime, int effectId)
            : base(startTime, endTime) {
          this.SpEffectID = effectId;
        }

        internal CreateSpEffect(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SpEffectID = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SpEffectID);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk500 : TAE3.Event {
        public byte Unk00;
        public byte Unk01;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk500; }
        }

        internal Unk500(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class Unk510 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk510; }
        }

        internal Unk510(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk520 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk520; }
        }

        internal Unk520(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class KingOfTheStorm : TAE3.Event {
        public float Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.KingOfTheStorm; }
        }

        internal KingOfTheStorm(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk600 : TAE3.Event {
        public int Mask;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk600; }
        }

        internal Unk600(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Mask = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Mask);
          bw.WriteInt32(0);
        }
      }

      public class Unk601 : TAE3.Event {
        public int StayAnimType;
        public float Unk04;
        public float Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk601; }
        }

        internal Unk601(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.StayAnimType = br.ReadInt32();
          this.Unk04 = br.ReadSingle();
          this.Unk08 = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.StayAnimType);
          bw.WriteSingle(this.Unk04);
          bw.WriteSingle(this.Unk08);
          bw.WriteInt32(0);
        }
      }

      public class DebugAnimSpeed : TAE3.Event {
        public uint AnimSpeed;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.DebugAnimSpeed; }
        }

        internal DebugAnimSpeed(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.AnimSpeed = br.ReadUInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteUInt32(this.AnimSpeed);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk605 : TAE3.Event {
        public bool Unk00;
        public byte Unk01;
        public byte Unk02;
        public byte Unk03;
        public int Unk04;
        public float Unk08;
        public float Unk0C;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk605; }
        }

        internal Unk605(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadBoolean();
          this.Unk01 = br.ReadByte();
          this.Unk02 = br.ReadByte();
          this.Unk03 = br.ReadByte();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadSingle();
          this.Unk0C = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteBoolean(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte(this.Unk02);
          bw.WriteByte(this.Unk03);
          bw.WriteInt32(this.Unk04);
          bw.WriteSingle(this.Unk08);
          bw.WriteSingle(this.Unk0C);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk606 : TAE3.Event {
        public byte Unk00;
        public byte Unk04;
        public byte Unk06;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk606; }
        }

        internal Unk606(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          this.Unk04 = br.ReadByte();
          int num4 = (int) br.AssertByte(new byte[1]);
          this.Unk06 = br.ReadByte();
          int num5 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte(this.Unk04);
          bw.WriteByte((byte) 0);
          bw.WriteByte(this.Unk06);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk700 : TAE3.Event {
        public float Unk00;
        public float Unk04;
        public float Unk08;
        public float Unk0C;
        public int Unk10;
        public sbyte Unk14;
        public float Unk18;
        public float Unk1C;
        public float Unk20;
        public float Unk24;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk700; }
        }

        internal Unk700(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          this.Unk04 = br.ReadSingle();
          this.Unk08 = br.ReadSingle();
          this.Unk0C = br.ReadSingle();
          this.Unk10 = br.ReadInt32();
          this.Unk14 = br.ReadSByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          this.Unk18 = br.ReadSingle();
          this.Unk1C = br.ReadSingle();
          this.Unk20 = br.ReadSingle();
          this.Unk24 = br.ReadSingle();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteSingle(this.Unk04);
          bw.WriteSingle(this.Unk08);
          bw.WriteSingle(this.Unk0C);
          bw.WriteInt32(this.Unk10);
          bw.WriteSByte(this.Unk14);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteSingle(this.Unk18);
          bw.WriteSingle(this.Unk1C);
          bw.WriteSingle(this.Unk20);
          bw.WriteSingle(this.Unk24);
        }
      }

      public class EnableTurningDirection : TAE3.Event {
        public byte State;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.EnableTurningDirection; }
        }

        internal EnableTurningDirection(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.State = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.State);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class FacingAngleCorrection : TAE3.Event {
        public float CorrectionRate;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.FacingAngleCorrection; }
        }

        internal FacingAngleCorrection(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.CorrectionRate = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.CorrectionRate);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk707 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk707; }
        }

        internal Unk707(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class HideWeapon : TAE3.Event {
        public byte Unk00;
        public byte Unk01;
        public byte Unk02;
        public byte Unk03;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.HideWeapon; }
        }

        internal HideWeapon(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          this.Unk02 = br.ReadByte();
          this.Unk03 = br.ReadByte();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte(this.Unk02);
          bw.WriteByte(this.Unk03);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class HideModelMask : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.HideModelMask; }
        }

        public byte[] Mask { get; private set; }

        internal HideModelMask(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Mask = br.ReadBytes(32);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteBytes(this.Mask);
        }
      }

      public class DamageLevelModule : TAE3.Event {
        public byte Unk10;
        public byte Unk11;
        public byte Unk12;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.DamageLevelModule; }
        }

        public byte[] Mask { get; private set; }

        internal DamageLevelModule(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Mask = br.ReadBytes(16);
          this.Unk10 = br.ReadByte();
          this.Unk11 = br.ReadByte();
          this.Unk12 = br.ReadByte();
          int num = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteBytes(this.Mask);
          bw.WriteByte(this.Unk10);
          bw.WriteByte(this.Unk11);
          bw.WriteByte(this.Unk12);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class ModelMask : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.ModelMask; }
        }

        public byte[] Mask { get; private set; }

        internal ModelMask(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Mask = br.ReadBytes(32);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteBytes(this.Mask);
        }
      }

      public class DamageLevelFunction : TAE3.Event {
        public byte Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.DamageLevelFunction; }
        }

        internal DamageLevelFunction(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class Unk715 : TAE3.Event {
        public byte Unk00;
        public byte Unk01;
        public byte Unk02;
        public byte Unk03;
        public byte Unk04;
        public byte Unk05;
        public byte Unk06;
        public byte Unk07;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk715; }
        }

        internal Unk715(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          this.Unk02 = br.ReadByte();
          this.Unk03 = br.ReadByte();
          this.Unk04 = br.ReadByte();
          this.Unk05 = br.ReadByte();
          this.Unk06 = br.ReadByte();
          this.Unk07 = br.ReadByte();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte(this.Unk02);
          bw.WriteByte(this.Unk03);
          bw.WriteByte(this.Unk04);
          bw.WriteByte(this.Unk05);
          bw.WriteByte(this.Unk06);
          bw.WriteByte(this.Unk07);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class CultStart : TAE3.Event {
        public byte CultType;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CultStart; }
        }

        internal CultStart(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.CultType = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.CultType);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk730 : TAE3.Event {
        public int Unk00;
        public int Unk04;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk730; }
        }

        internal Unk730(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk740 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk740; }
        }

        internal Unk740(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class IFrameState : TAE3.Event {
        public byte Unk00;
        public byte Unk01;
        public byte Unk02;
        public byte Unk03;
        public float Unk04;
        public float Unk08;
        public float Unk0C;
        public float Unk10;
        public float Unk14;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.IFrameState; }
        }

        internal IFrameState(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          this.Unk02 = br.ReadByte();
          this.Unk03 = br.ReadByte();
          this.Unk04 = br.ReadSingle();
          this.Unk08 = br.ReadSingle();
          this.Unk0C = br.ReadSingle();
          this.Unk10 = br.ReadSingle();
          this.Unk14 = br.ReadSingle();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte(this.Unk02);
          bw.WriteByte(this.Unk03);
          bw.WriteSingle(this.Unk04);
          bw.WriteSingle(this.Unk08);
          bw.WriteSingle(this.Unk0C);
          bw.WriteSingle(this.Unk10);
          bw.WriteSingle(this.Unk14);
        }
      }

      public class BonePos : TAE3.Event {
        public int Unk00;
        public float Unk04;
        public byte Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.BonePos; }
        }

        internal BonePos(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadSingle();
          this.Unk08 = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteSingle(this.Unk04);
          bw.WriteByte(this.Unk08);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class BoneFixOn1 : TAE3.Event {
        public byte BoneID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.BoneFixOn1; }
        }

        internal BoneFixOn1(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.BoneID = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.BoneID);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class BoneFixOn2 : TAE3.Event {
        public int Unk00;
        public float Unk04;
        public byte Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.BoneFixOn2; }
        }

        internal BoneFixOn2(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadSingle();
          this.Unk08 = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteSingle(this.Unk04);
          bw.WriteByte(this.Unk08);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
        }
      }

      public class TurnLowerBody : TAE3.Event {
        public byte TurnState;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.TurnLowerBody; }
        }

        internal TurnLowerBody(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.TurnState = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.TurnState);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk782 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk782; }
        }

        internal Unk782(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class SpawnBulletByCultSacrifice1 : TAE3.Event {
        public float Unk00;
        public int DummyPointID;
        public int BulletID;
        public byte Unk0C;
        public byte Unk0D;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.SpawnBulletByCultSacrifice1; }
        }

        internal SpawnBulletByCultSacrifice1(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          this.DummyPointID = br.ReadInt32();
          this.BulletID = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          this.Unk0D = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteInt32(this.DummyPointID);
          bw.WriteInt32(this.BulletID);
          bw.WriteByte(this.Unk0C);
          bw.WriteByte(this.Unk0D);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
        }
      }

      public class Unk786 : TAE3.Event {
        public float Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk786; }
        }

        internal Unk786(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteInt32(0);
        }
      }

      public class Unk790 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk790; }
        }

        internal Unk790(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk791 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk791; }
        }

        internal Unk791(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class HitEffect2 : TAE3.Event {
        public short Unk00;
        public int Unk04;
        public int Unk08;
        public byte Unk0C;
        public byte Unk0D;
        public byte Unk0E;
        public byte Unk0F;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.HitEffect2; }
        }

        internal HitEffect2(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadInt16();
          int num = (int) br.AssertInt16(new short[1]);
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          this.Unk0D = br.ReadByte();
          this.Unk0E = br.ReadByte();
          this.Unk0F = br.ReadByte();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt16(this.Unk00);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteByte(this.Unk0C);
          bw.WriteByte(this.Unk0D);
          bw.WriteByte(this.Unk0E);
          bw.WriteByte(this.Unk0F);
        }
      }

      public class CultSacrifice1 : TAE3.Event {
        public int SacrificeValue;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CultSacrifice1; }
        }

        internal CultSacrifice1(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.SacrificeValue = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SacrificeValue);
          bw.WriteInt32(0);
        }
      }

      public class SacrificeEmpty : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.SacrificeEmpty; }
        }

        internal SacrificeEmpty(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Toughness : TAE3.Event {
        public byte ToughnessParamID;
        public bool IsToughnessEffective;
        public float ToughnessRate;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Toughness; }
        }

        public Toughness(
            float startTime,
            float endTime,
            byte toughnessParamID,
            bool isToughnessEffective,
            float toughnessRate)
            : base(startTime, endTime) {
          this.ToughnessParamID = toughnessParamID;
          this.IsToughnessEffective = isToughnessEffective;
          this.ToughnessRate = toughnessRate;
        }

        internal Toughness(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.ToughnessParamID = br.ReadByte();
          this.IsToughnessEffective = br.ReadBoolean();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          this.ToughnessRate = br.ReadSingle();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.ToughnessParamID);
          bw.WriteBoolean(this.IsToughnessEffective);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteSingle(this.ToughnessRate);
        }
      }

      public class BringCultMenu : TAE3.Event {
        public byte MenuType;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.BringCultMenu; }
        }

        internal BringCultMenu(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.MenuType = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteByte(this.MenuType);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class CeremonyParamID : TAE3.Event {
        public int ParamID;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CeremonyParamID; }
        }

        internal CeremonyParamID(
            float startTime,
            float endTime,
            BinaryReaderEx br)
            : base(startTime, endTime) {
          this.ParamID = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.ParamID);
          bw.WriteInt32(0);
        }
      }

      public class CultSingle : TAE3.Event {
        public float Unk00;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.CultSingle; }
        }

        internal CultSingle(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.Unk00 = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.Unk00);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class CultEmpty2 : TAE3.Event {
        public override TAE3.EventType Type {
          get { return TAE3.EventType.CultEmpty2; }
        }

        internal CultEmpty2(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Unk800 : TAE3.Event {
        public float MetersPerTick;
        public float MetersOnTurn;
        public float Unk08;

        public override TAE3.EventType Type {
          get { return TAE3.EventType.Unk800; }
        }

        internal Unk800(float startTime, float endTime, BinaryReaderEx br)
            : base(startTime, endTime) {
          this.MetersPerTick = br.ReadSingle();
          this.MetersOnTurn = br.ReadSingle();
          this.Unk08 = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteSingle(this.MetersPerTick);
          bw.WriteSingle(this.MetersOnTurn);
          bw.WriteSingle(this.Unk08);
          bw.WriteInt32(0);
        }
      }
    }
  }
}