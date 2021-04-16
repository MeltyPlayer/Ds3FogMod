// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MSBS
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class MSBS : SoulsFile<MSBS>, IMsb
  {
    public MSBS.ModelParam Models { get; set; }

    IMsbParam<IMsbModel> IMsb.Models
    {
      get
      {
        return (IMsbParam<IMsbModel>) this.Models;
      }
    }

    public MSBS.EventParam Events { get; set; }

    public MSBS.PointParam Regions { get; set; }

    IMsbParam<IMsbRegion> IMsb.Regions
    {
      get
      {
        return (IMsbParam<IMsbRegion>) this.Regions;
      }
    }

    public MSBS.RouteParam Routes { get; set; }

    public MSBS.PartsParam Parts { get; set; }

    IMsbParam<IMsbPart> IMsb.Parts
    {
      get
      {
        return (IMsbParam<IMsbPart>) this.Parts;
      }
    }

    public MSBS.EmptyParam Layers { get; set; }

    public MSBS.EmptyParam PartsPoses { get; set; }

    public MSBS.EmptyParam BoneNames { get; set; }

    public MSBS()
    {
      this.Models = new MSBS.ModelParam(35);
      this.Events = new MSBS.EventParam(35);
      this.Regions = new MSBS.PointParam(35);
      this.Routes = new MSBS.RouteParam(35);
      this.Parts = new MSBS.PartsParam(35);
      this.Layers = new MSBS.EmptyParam(35, "LAYER_PARAM_ST");
      this.PartsPoses = new MSBS.EmptyParam(0, "MAPSTUDIO_PARTS_POSE_ST");
      this.BoneNames = new MSBS.EmptyParam(0, "MAPSTUDIO_BONE_NAME_STRING");
    }

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "MSB ";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      br.AssertASCII("MSB ");
      br.AssertInt32(1);
      br.AssertInt32(16);
      br.AssertBoolean(false);
      br.AssertBoolean(false);
      int num1 = (int) br.AssertByte((byte) 1);
      int num2 = (int) br.AssertByte(byte.MaxValue);
      this.Models = new MSBS.ModelParam(35);
      MSBS.Entries entries;
      entries.Models = this.Models.Read(br);
      this.Events = new MSBS.EventParam(35);
      entries.Events = this.Events.Read(br);
      this.Regions = new MSBS.PointParam(35);
      entries.Regions = this.Regions.Read(br);
      this.Routes = new MSBS.RouteParam(35);
      entries.Routes = this.Routes.Read(br);
      this.Layers = new MSBS.EmptyParam(35, "LAYER_PARAM_ST");
      this.Layers.Read(br);
      this.Parts = new MSBS.PartsParam(35);
      entries.Parts = this.Parts.Read(br);
      this.PartsPoses = new MSBS.EmptyParam(0, "MAPSTUDIO_PARTS_POSE_ST");
      this.PartsPoses.Read(br);
      this.BoneNames = new MSBS.EmptyParam(0, "MAPSTUDIO_BONE_NAME_STRING");
      this.BoneNames.Read(br);
      if (br.Position != 0L)
        throw new InvalidDataException("The next param offset of the final param should be 0, but it wasn't.");
      MSB.DisambiguateNames<MSBS.Model>(entries.Models);
      MSB.DisambiguateNames<MSBS.Region>(entries.Regions);
      MSB.DisambiguateNames<MSBS.Part>(entries.Parts);
      foreach (MSBS.Event @event in entries.Events)
        @event.GetNames(this, entries);
      foreach (MSBS.Region region in entries.Regions)
        region.GetNames(entries);
      foreach (MSBS.Part part in entries.Parts)
        part.GetNames(this, entries);
    }

    protected override void Write(BinaryWriterEx bw)
    {
      MSBS.Entries entries;
      entries.Models = this.Models.GetEntries();
      entries.Events = this.Events.GetEntries();
      entries.Regions = this.Regions.GetEntries();
      entries.Routes = this.Routes.GetEntries();
      entries.Parts = this.Parts.GetEntries();
      foreach (MSBS.Model model in entries.Models)
        model.CountInstances(entries.Parts);
      foreach (MSBS.Event @event in entries.Events)
        @event.GetIndices(this, entries);
      foreach (MSBS.Region region in entries.Regions)
        region.GetIndices(entries);
      foreach (MSBS.Part part in entries.Parts)
        part.GetIndices(this, entries);
      bw.BigEndian = false;
      bw.WriteASCII("MSB ", false);
      bw.WriteInt32(1);
      bw.WriteInt32(16);
      bw.WriteBoolean(false);
      bw.WriteBoolean(false);
      bw.WriteByte((byte) 1);
      bw.WriteByte(byte.MaxValue);
      this.Models.Write(bw, entries.Models);
      bw.FillInt64("NextParamOffset", bw.Position);
      this.Events.Write(bw, entries.Events);
      bw.FillInt64("NextParamOffset", bw.Position);
      this.Regions.Write(bw, entries.Regions);
      bw.FillInt64("NextParamOffset", bw.Position);
      this.Routes.Write(bw, entries.Routes);
      bw.FillInt64("NextParamOffset", bw.Position);
      this.Layers.Write(bw, this.Layers.GetEntries());
      bw.FillInt64("NextParamOffset", bw.Position);
      this.Parts.Write(bw, entries.Parts);
      bw.FillInt64("NextParamOffset", bw.Position);
      this.PartsPoses.Write(bw, this.Layers.GetEntries());
      bw.FillInt64("NextParamOffset", bw.Position);
      this.BoneNames.Write(bw, this.Layers.GetEntries());
      bw.FillInt64("NextParamOffset", 0L);
    }

    public enum EventType : uint
    {
      Treasure = 4,
      Generator = 5,
      ObjAct = 7,
      MapOffset = 9,
      WalkRoute = 14, // 0x0000000E
      GroupTour = 15, // 0x0000000F
      Event17 = 17, // 0x00000011
      Event18 = 18, // 0x00000012
      Event20 = 20, // 0x00000014
      Event21 = 21, // 0x00000015
      PartsGroup = 22, // 0x00000016
      Talk = 23, // 0x00000017
      AutoDrawGroup = 24, // 0x00000018
      Other = 4294967295, // 0xFFFFFFFF
    }

    public class EventParam : MSBS.Param<MSBS.Event>
    {
      public List<MSBS.Event.Treasure> Treasures { get; set; }

      public List<MSBS.Event.Generator> Generators { get; set; }

      public List<MSBS.Event.ObjAct> ObjActs { get; set; }

      public List<MSBS.Event.MapOffset> MapOffsets { get; set; }

      public List<MSBS.Event.WalkRoute> WalkRoutes { get; set; }

      public List<MSBS.Event.GroupTour> GroupTours { get; set; }

      public List<MSBS.Event.Event17> Event17s { get; set; }

      public List<MSBS.Event.Event18> Event18s { get; set; }

      public List<MSBS.Event.Event20> Event20s { get; set; }

      public List<MSBS.Event.Event21> Event21s { get; set; }

      public List<MSBS.Event.PartsGroup> PartsGroups { get; set; }

      public List<MSBS.Event.Talk> Talks { get; set; }

      public List<MSBS.Event.AutoDrawGroup> AutoDrawGroups { get; set; }

      public List<MSBS.Event.Other> Others { get; set; }

      public EventParam(int unk00 = 35)
        : base(unk00, "EVENT_PARAM_ST")
      {
        this.Treasures = new List<MSBS.Event.Treasure>();
        this.Generators = new List<MSBS.Event.Generator>();
        this.ObjActs = new List<MSBS.Event.ObjAct>();
        this.MapOffsets = new List<MSBS.Event.MapOffset>();
        this.WalkRoutes = new List<MSBS.Event.WalkRoute>();
        this.GroupTours = new List<MSBS.Event.GroupTour>();
        this.Event17s = new List<MSBS.Event.Event17>();
        this.Event18s = new List<MSBS.Event.Event18>();
        this.Event20s = new List<MSBS.Event.Event20>();
        this.Event21s = new List<MSBS.Event.Event21>();
        this.PartsGroups = new List<MSBS.Event.PartsGroup>();
        this.Talks = new List<MSBS.Event.Talk>();
        this.AutoDrawGroups = new List<MSBS.Event.AutoDrawGroup>();
        this.Others = new List<MSBS.Event.Other>();
      }

      internal override MSBS.Event ReadEntry(BinaryReaderEx br)
      {
        MSBS.EventType enum32 = br.GetEnum32<MSBS.EventType>(br.Position + 12L);
        switch (enum32)
        {
          case MSBS.EventType.Treasure:
            MSBS.Event.Treasure treasure = new MSBS.Event.Treasure(br);
            this.Treasures.Add(treasure);
            return (MSBS.Event) treasure;
          case MSBS.EventType.Generator:
            MSBS.Event.Generator generator = new MSBS.Event.Generator(br);
            this.Generators.Add(generator);
            return (MSBS.Event) generator;
          case MSBS.EventType.ObjAct:
            MSBS.Event.ObjAct objAct = new MSBS.Event.ObjAct(br);
            this.ObjActs.Add(objAct);
            return (MSBS.Event) objAct;
          case MSBS.EventType.MapOffset:
            MSBS.Event.MapOffset mapOffset = new MSBS.Event.MapOffset(br);
            this.MapOffsets.Add(mapOffset);
            return (MSBS.Event) mapOffset;
          case MSBS.EventType.WalkRoute:
            MSBS.Event.WalkRoute walkRoute = new MSBS.Event.WalkRoute(br);
            this.WalkRoutes.Add(walkRoute);
            return (MSBS.Event) walkRoute;
          case MSBS.EventType.GroupTour:
            MSBS.Event.GroupTour groupTour = new MSBS.Event.GroupTour(br);
            this.GroupTours.Add(groupTour);
            return (MSBS.Event) groupTour;
          case MSBS.EventType.Event17:
            MSBS.Event.Event17 event17 = new MSBS.Event.Event17(br);
            this.Event17s.Add(event17);
            return (MSBS.Event) event17;
          case MSBS.EventType.Event18:
            MSBS.Event.Event18 event18 = new MSBS.Event.Event18(br);
            this.Event18s.Add(event18);
            return (MSBS.Event) event18;
          case MSBS.EventType.Event20:
            MSBS.Event.Event20 event20 = new MSBS.Event.Event20(br);
            this.Event20s.Add(event20);
            return (MSBS.Event) event20;
          case MSBS.EventType.Event21:
            MSBS.Event.Event21 event21 = new MSBS.Event.Event21(br);
            this.Event21s.Add(event21);
            return (MSBS.Event) event21;
          case MSBS.EventType.PartsGroup:
            MSBS.Event.PartsGroup partsGroup = new MSBS.Event.PartsGroup(br);
            this.PartsGroups.Add(partsGroup);
            return (MSBS.Event) partsGroup;
          case MSBS.EventType.Talk:
            MSBS.Event.Talk talk = new MSBS.Event.Talk(br);
            this.Talks.Add(talk);
            return (MSBS.Event) talk;
          case MSBS.EventType.AutoDrawGroup:
            MSBS.Event.AutoDrawGroup autoDrawGroup = new MSBS.Event.AutoDrawGroup(br);
            this.AutoDrawGroups.Add(autoDrawGroup);
            return (MSBS.Event) autoDrawGroup;
          case MSBS.EventType.Other:
            MSBS.Event.Other other = new MSBS.Event.Other(br);
            this.Others.Add(other);
            return (MSBS.Event) other;
          default:
            throw new NotImplementedException(string.Format("Unimplemented model type: {0}", (object) enum32));
        }
      }

      public override List<MSBS.Event> GetEntries()
      {
        return SFUtil.ConcatAll<MSBS.Event>(new IEnumerable<MSBS.Event>[14]
        {
          (IEnumerable<MSBS.Event>) this.Treasures,
          (IEnumerable<MSBS.Event>) this.Generators,
          (IEnumerable<MSBS.Event>) this.ObjActs,
          (IEnumerable<MSBS.Event>) this.MapOffsets,
          (IEnumerable<MSBS.Event>) this.WalkRoutes,
          (IEnumerable<MSBS.Event>) this.GroupTours,
          (IEnumerable<MSBS.Event>) this.Event17s,
          (IEnumerable<MSBS.Event>) this.Event18s,
          (IEnumerable<MSBS.Event>) this.Event20s,
          (IEnumerable<MSBS.Event>) this.Event21s,
          (IEnumerable<MSBS.Event>) this.PartsGroups,
          (IEnumerable<MSBS.Event>) this.Talks,
          (IEnumerable<MSBS.Event>) this.AutoDrawGroups,
          (IEnumerable<MSBS.Event>) this.Others
        });
      }
    }

    public abstract class Event : MSBS.Entry
    {
      private int PartIndex;
      private int RegionIndex;

      public abstract MSBS.EventType Type { get; }

      internal abstract bool HasTypeData { get; }

      public int EventIndex { get; set; }

      public string PartName { get; set; }

      public string RegionName { get; set; }

      public int EntityID { get; set; }

      internal Event()
      {
        this.Name = "";
        this.EventIndex = -1;
        this.EntityID = -1;
      }

      internal Event(BinaryReaderEx br)
      {
        long position = br.Position;
        long num1 = br.ReadInt64();
        this.EventIndex = br.ReadInt32();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        br.AssertInt32(new int[1]);
        long num3 = br.ReadInt64();
        long num4 = br.ReadInt64();
        this.Name = br.GetUTF16(position + num1);
        br.Position = position + num3;
        this.PartIndex = br.ReadInt32();
        this.RegionIndex = br.ReadInt32();
        this.EntityID = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.Position = position + num4;
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteInt32(this.EventIndex);
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.WriteInt32(0);
        bw.ReserveInt64("BaseDataOffset");
        bw.ReserveInt64("TypeDataOffset");
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(this.Name, true);
        bw.Pad(8);
        bw.FillInt64("BaseDataOffset", bw.Position - position);
        bw.WriteInt32(this.PartIndex);
        bw.WriteInt32(this.RegionIndex);
        bw.WriteInt32(this.EntityID);
        bw.WriteInt32(0);
        if (this.HasTypeData)
        {
          bw.FillInt64("TypeDataOffset", bw.Position - position);
          this.WriteTypeData(bw);
        }
        else
          bw.FillInt64("TypeDataOffset", 0L);
      }

      internal virtual void WriteTypeData(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Type data should not be written for events with no type data.");
      }

      internal virtual void GetNames(MSBS msb, MSBS.Entries entries)
      {
        this.PartName = MSB.FindName<MSBS.Part>(entries.Parts, this.PartIndex);
        this.RegionName = MSB.FindName<MSBS.Region>(entries.Regions, this.RegionIndex);
      }

      internal virtual void GetIndices(MSBS msb, MSBS.Entries entries)
      {
        this.PartIndex = MSB.FindIndex<MSBS.Part>(entries.Parts, this.PartName);
        this.RegionIndex = MSB.FindIndex<MSBS.Region>(entries.Regions, this.RegionName);
      }

      public override string ToString()
      {
        return string.Format("{0} {1}", (object) this.Type, (object) this.Name);
      }

      public class Treasure : MSBS.Event
      {
        private int TreasurePartIndex;

        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.Treasure;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public string TreasurePartName { get; set; }

        public int ItemLotID { get; set; }

        public int ActionButtonID { get; set; }

        public int PickupAnimID { get; set; }

        public bool InChest { get; set; }

        public bool StartDisabled { get; set; }

        public Treasure()
        {
          this.ItemLotID = -1;
          this.ActionButtonID = -1;
          this.PickupAnimID = -1;
        }

        internal Treasure(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.TreasurePartIndex = br.ReadInt32();
          br.AssertInt32(new int[1]);
          this.ItemLotID = br.ReadInt32();
          br.AssertPattern(36, byte.MaxValue);
          this.ActionButtonID = br.ReadInt32();
          this.PickupAnimID = br.ReadInt32();
          this.InChest = br.ReadBoolean();
          this.StartDisabled = br.ReadBoolean();
          int num = (int) br.AssertInt16(new short[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.TreasurePartIndex);
          bw.WriteInt32(0);
          bw.WriteInt32(this.ItemLotID);
          bw.WritePattern(36, byte.MaxValue);
          bw.WriteInt32(this.ActionButtonID);
          bw.WriteInt32(this.PickupAnimID);
          bw.WriteBoolean(this.InChest);
          bw.WriteBoolean(this.StartDisabled);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.TreasurePartName = MSB.FindName<MSBS.Part>(entries.Parts, this.TreasurePartIndex);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.TreasurePartIndex = MSB.FindIndex<MSBS.Part>(entries.Parts, this.TreasurePartName);
        }
      }

      public class Generator : MSBS.Event
      {
        private int[] SpawnRegionIndices;
        private int[] SpawnPartIndices;

        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.Generator;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public short MaxNum { get; set; }

        public short LimitNum { get; set; }

        public short MinGenNum { get; set; }

        public short MaxGenNum { get; set; }

        public float MinInterval { get; set; }

        public float MaxInterval { get; set; }

        public int SessionCondition { get; set; }

        public float UnkT14 { get; set; }

        public float UnkT18 { get; set; }

        public string[] SpawnRegionNames { get; private set; }

        public string[] SpawnPartNames { get; private set; }

        public Generator()
        {
          this.SpawnRegionNames = new string[8];
          this.SpawnPartNames = new string[32];
        }

        internal Generator(BinaryReaderEx br)
          : base(br)
        {
          this.MaxNum = br.ReadInt16();
          this.LimitNum = br.ReadInt16();
          this.MinGenNum = br.ReadInt16();
          this.MaxGenNum = br.ReadInt16();
          this.MinInterval = br.ReadSingle();
          this.MaxInterval = br.ReadSingle();
          this.SessionCondition = br.ReadInt32();
          this.UnkT14 = br.ReadSingle();
          this.UnkT18 = br.ReadSingle();
          br.AssertPattern(20, (byte) 0);
          this.SpawnRegionIndices = br.ReadInt32s(8);
          br.AssertPattern(16, (byte) 0);
          this.SpawnPartIndices = br.ReadInt32s(32);
          br.AssertPattern(32, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt16(this.MaxNum);
          bw.WriteInt16(this.LimitNum);
          bw.WriteInt16(this.MinGenNum);
          bw.WriteInt16(this.MaxGenNum);
          bw.WriteSingle(this.MinInterval);
          bw.WriteSingle(this.MaxInterval);
          bw.WriteInt32(this.SessionCondition);
          bw.WriteSingle(this.UnkT14);
          bw.WriteSingle(this.UnkT18);
          bw.WritePattern(20, (byte) 0);
          bw.WriteInt32s((IList<int>) this.SpawnRegionIndices);
          bw.WritePattern(16, (byte) 0);
          bw.WriteInt32s((IList<int>) this.SpawnPartIndices);
          bw.WritePattern(32, (byte) 0);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.SpawnRegionNames = MSB.FindNames<MSBS.Region>(entries.Regions, this.SpawnRegionIndices);
          this.SpawnPartNames = MSB.FindNames<MSBS.Part>(entries.Parts, this.SpawnPartIndices);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.SpawnRegionIndices = MSB.FindIndices<MSBS.Region>(entries.Regions, this.SpawnRegionNames);
          this.SpawnPartIndices = MSB.FindIndices<MSBS.Part>(entries.Parts, this.SpawnPartNames);
        }
      }

      public class ObjAct : MSBS.Event
      {
        private int ObjActPartIndex;

        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.ObjAct;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int ObjActEntityID { get; set; }

        public string ObjActPartName { get; set; }

        public int ObjActID { get; set; }

        public byte StateType { get; set; }

        public int EventFlagID { get; set; }

        public ObjAct()
        {
          this.ObjActEntityID = -1;
          this.ObjActID = -1;
          this.EventFlagID = -1;
        }

        internal ObjAct(BinaryReaderEx br)
          : base(br)
        {
          this.ObjActEntityID = br.ReadInt32();
          this.ObjActPartIndex = br.ReadInt32();
          this.ObjActID = br.ReadInt32();
          this.StateType = br.ReadByte();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertInt16(new short[1]);
          this.EventFlagID = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.ObjActEntityID);
          bw.WriteInt32(this.ObjActPartIndex);
          bw.WriteInt32(this.ObjActID);
          bw.WriteByte(this.StateType);
          bw.WriteByte((byte) 0);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(this.EventFlagID);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.ObjActPartName = MSB.FindName<MSBS.Part>(entries.Parts, this.ObjActPartIndex);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.ObjActPartIndex = MSB.FindIndex<MSBS.Part>(entries.Parts, this.ObjActPartName);
        }
      }

      public class MapOffset : MSBS.Event
      {
        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.MapOffset;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public Vector3 Position { get; set; }

        public float Degree { get; set; }

        public MapOffset()
        {
        }

        internal MapOffset(BinaryReaderEx br)
          : base(br)
        {
          this.Position = br.ReadVector3();
          this.Degree = br.ReadSingle();
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteVector3(this.Position);
          bw.WriteSingle(this.Degree);
        }
      }

      public class WalkRoute : MSBS.Event
      {
        private short[] WalkRegionIndices;

        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.WalkRoute;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public string[] WalkRegionNames { get; private set; }

        public MSBS.Event.WalkRoute.WREntry[] WREntries { get; set; }

        public WalkRoute()
        {
          this.WalkRegionNames = new string[32];
          this.WREntries = new MSBS.Event.WalkRoute.WREntry[5];
          for (int index = 0; index < 5; ++index)
            this.WREntries[index] = new MSBS.Event.WalkRoute.WREntry();
        }

        internal WalkRoute(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.WalkRegionIndices = br.ReadInt16s(32);
          this.WREntries = new MSBS.Event.WalkRoute.WREntry[5];
          for (int index = 0; index < 5; ++index)
            this.WREntries[index] = new MSBS.Event.WalkRoute.WREntry(br);
          br.AssertPattern(20, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt16s((IList<short>) this.WalkRegionIndices);
          for (int index = 0; index < 5; ++index)
            this.WREntries[index].Write(bw);
          bw.WritePattern(20, (byte) 0);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.WalkRegionNames = new string[this.WalkRegionIndices.Length];
          for (int index = 0; index < this.WalkRegionIndices.Length; ++index)
            this.WalkRegionNames[index] = MSB.FindName<MSBS.Region>(entries.Regions, (int) this.WalkRegionIndices[index]);
          foreach (MSBS.Event.WalkRoute.WREntry wrEntry in this.WREntries)
            wrEntry.GetNames(entries);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.WalkRegionIndices = new short[this.WalkRegionNames.Length];
          for (int index = 0; index < this.WalkRegionNames.Length; ++index)
            this.WalkRegionIndices[index] = (short) MSB.FindIndex<MSBS.Region>(entries.Regions, this.WalkRegionNames[index]);
          foreach (MSBS.Event.WalkRoute.WREntry wrEntry in this.WREntries)
            wrEntry.GetIndices(entries);
        }

        public class WREntry
        {
          private short RegionIndex;

          public string RegionName { get; set; }

          public int Unk04 { get; set; }

          public int Unk08 { get; set; }

          public WREntry()
          {
          }

          internal WREntry(BinaryReaderEx br)
          {
            this.RegionIndex = br.ReadInt16();
            int num = (int) br.AssertInt16(new short[1]);
            this.Unk04 = br.ReadInt32();
            this.Unk08 = br.ReadInt32();
          }

          internal void Write(BinaryWriterEx bw)
          {
            bw.WriteInt16(this.RegionIndex);
            bw.WriteInt16((short) 0);
            bw.WriteInt32(this.Unk04);
            bw.WriteInt32(this.Unk08);
          }

          internal void GetNames(MSBS.Entries entries)
          {
            this.RegionName = MSB.FindName<MSBS.Region>(entries.Regions, (int) this.RegionIndex);
          }

          internal void GetIndices(MSBS.Entries entries)
          {
            this.RegionIndex = (short) MSB.FindIndex<MSBS.Region>(entries.Regions, this.RegionName);
          }
        }
      }

      public class GroupTour : MSBS.Event
      {
        private int[] GroupPartIndices;

        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.GroupTour;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int PlatoonIDScriptActive { get; set; }

        public int State { get; set; }

        public string[] GroupPartNames { get; private set; }

        public GroupTour()
        {
          this.GroupPartNames = new string[32];
        }

        internal GroupTour(BinaryReaderEx br)
          : base(br)
        {
          this.PlatoonIDScriptActive = br.ReadInt32();
          this.State = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.GroupPartIndices = br.ReadInt32s(32);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.PlatoonIDScriptActive);
          bw.WriteInt32(this.State);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32s((IList<int>) this.GroupPartIndices);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.GroupPartNames = MSB.FindNames<MSBS.Part>(entries.Parts, this.GroupPartIndices);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.GroupPartIndices = MSB.FindIndices<MSBS.Part>(entries.Parts, this.GroupPartNames);
        }
      }

      public class Event17 : MSBS.Event
      {
        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.Event17;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public Event17()
        {
        }

        internal Event17(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
          br.AssertPattern(28, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
          bw.WritePattern(28, (byte) 0);
        }
      }

      public class Event18 : MSBS.Event
      {
        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.Event18;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public Event18()
        {
        }

        internal Event18(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
          br.AssertPattern(28, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
          bw.WritePattern(28, (byte) 0);
        }
      }

      public class Event20 : MSBS.Event
      {
        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.Event20;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public short UnkT04 { get; set; }

        public short UnkT06 { get; set; }

        public Event20()
        {
        }

        internal Event20(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadInt16();
          this.UnkT06 = br.ReadInt16();
          br.AssertPattern(24, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt16(this.UnkT04);
          bw.WriteInt16(this.UnkT06);
          bw.WritePattern(24, (byte) 0);
        }
      }

      public class Event21 : MSBS.Event
      {
        private int[] Event21PartIndices;

        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.Event21;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public string[] Event21PartNames { get; private set; }

        public Event21()
        {
          this.Event21PartNames = new string[32];
        }

        internal Event21(BinaryReaderEx br)
          : base(br)
        {
          this.Event21PartIndices = br.ReadInt32s(32);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32s((IList<int>) this.Event21PartIndices);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.Event21PartNames = MSB.FindNames<MSBS.Part>(entries.Parts, this.Event21PartIndices);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.Event21PartIndices = MSB.FindIndices<MSBS.Part>(entries.Parts, this.Event21PartNames);
        }
      }

      public class PartsGroup : MSBS.Event
      {
        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.PartsGroup;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public PartsGroup()
        {
        }

        internal PartsGroup(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class Talk : MSBS.Event
      {
        private int[] EnemyIndices;

        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.Talk;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public string[] EnemyNames { get; private set; }

        public int[] TalkIDs { get; private set; }

        public short UnkT44 { get; set; }

        public short UnkT46 { get; set; }

        public int UnkT48 { get; set; }

        public Talk()
        {
          this.EnemyNames = new string[8];
          this.TalkIDs = new int[8];
        }

        internal Talk(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
          this.EnemyIndices = br.ReadInt32s(8);
          this.TalkIDs = br.ReadInt32s(8);
          this.UnkT44 = br.ReadInt16();
          this.UnkT46 = br.ReadInt16();
          this.UnkT48 = br.ReadInt32();
          br.AssertPattern(52, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32s((IList<int>) this.EnemyIndices);
          bw.WriteInt32s((IList<int>) this.TalkIDs);
          bw.WriteInt16(this.UnkT44);
          bw.WriteInt16(this.UnkT46);
          bw.WriteInt32(this.UnkT48);
          bw.WritePattern(52, (byte) 0);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.EnemyNames = MSB.FindNames<MSBS.Part.Enemy>(msb.Parts.Enemies, this.EnemyIndices);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.EnemyIndices = MSB.FindIndices<MSBS.Part.Enemy>(msb.Parts.Enemies, this.EnemyNames);
        }
      }

      public class AutoDrawGroup : MSBS.Event
      {
        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.AutoDrawGroup;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public int UnkT04 { get; set; }

        public AutoDrawGroup()
        {
        }

        internal AutoDrawGroup(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadInt32();
          br.AssertPattern(24, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(this.UnkT04);
          bw.WritePattern(24, (byte) 0);
        }
      }

      public class Other : MSBS.Event
      {
        public override MSBS.EventType Type
        {
          get
          {
            return MSBS.EventType.Other;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Other()
        {
        }

        internal Other(BinaryReaderEx br)
          : base(br)
        {
        }
      }
    }

    public enum ModelType : uint
    {
      MapPiece = 0,
      Object = 1,
      Enemy = 2,
      Player = 4,
      Collision = 5,
    }

    public class ModelParam : MSBS.Param<MSBS.Model>, IMsbParam<IMsbModel>
    {
      public List<MSBS.Model.MapPiece> MapPieces { get; set; }

      public List<MSBS.Model.Object> Objects { get; set; }

      public List<MSBS.Model.Enemy> Enemies { get; set; }

      public List<MSBS.Model.Player> Players { get; set; }

      public List<MSBS.Model.Collision> Collisions { get; set; }

      public ModelParam(int unk00 = 35)
        : base(unk00, "MODEL_PARAM_ST")
      {
        this.MapPieces = new List<MSBS.Model.MapPiece>();
        this.Objects = new List<MSBS.Model.Object>();
        this.Enemies = new List<MSBS.Model.Enemy>();
        this.Players = new List<MSBS.Model.Player>();
        this.Collisions = new List<MSBS.Model.Collision>();
      }

      internal override MSBS.Model ReadEntry(BinaryReaderEx br)
      {
        MSBS.ModelType enum32 = br.GetEnum32<MSBS.ModelType>(br.Position + 8L);
        switch (enum32)
        {
          case MSBS.ModelType.MapPiece:
            MSBS.Model.MapPiece mapPiece = new MSBS.Model.MapPiece(br);
            this.MapPieces.Add(mapPiece);
            return (MSBS.Model) mapPiece;
          case MSBS.ModelType.Object:
            MSBS.Model.Object @object = new MSBS.Model.Object(br);
            this.Objects.Add(@object);
            return (MSBS.Model) @object;
          case MSBS.ModelType.Enemy:
            MSBS.Model.Enemy enemy = new MSBS.Model.Enemy(br);
            this.Enemies.Add(enemy);
            return (MSBS.Model) enemy;
          case MSBS.ModelType.Player:
            MSBS.Model.Player player = new MSBS.Model.Player(br);
            this.Players.Add(player);
            return (MSBS.Model) player;
          case MSBS.ModelType.Collision:
            MSBS.Model.Collision collision = new MSBS.Model.Collision(br);
            this.Collisions.Add(collision);
            return (MSBS.Model) collision;
          default:
            throw new NotImplementedException(string.Format("Unimplemented model type: {0}", (object) enum32));
        }
      }

      public override List<MSBS.Model> GetEntries()
      {
        return SFUtil.ConcatAll<MSBS.Model>(new IEnumerable<MSBS.Model>[5]
        {
          (IEnumerable<MSBS.Model>) this.MapPieces,
          (IEnumerable<MSBS.Model>) this.Objects,
          (IEnumerable<MSBS.Model>) this.Enemies,
          (IEnumerable<MSBS.Model>) this.Players,
          (IEnumerable<MSBS.Model>) this.Collisions
        });
      }

      IReadOnlyList<IMsbModel> IMsbParam<IMsbModel>.GetEntries()
      {
        return (IReadOnlyList<IMsbModel>) this.GetEntries();
      }
    }

    public abstract class Model : MSBS.Entry, IMsbModel, IMsbEntry
    {
      private int InstanceCount;

      public abstract MSBS.ModelType Type { get; }

      internal abstract bool HasTypeData { get; }

      public string Placeholder { get; set; }

      public int Unk1C { get; set; }

      internal Model()
      {
        this.Name = "";
        this.Placeholder = "";
      }

      internal Model(BinaryReaderEx br)
      {
        long position = br.Position;
        long num1 = br.ReadInt64();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        long num3 = br.ReadInt64();
        this.InstanceCount = br.ReadInt32();
        this.Unk1C = br.ReadInt32();
        long num4 = br.ReadInt64();
        this.Name = br.GetUTF16(position + num1);
        this.Placeholder = br.GetUTF16(position + num3);
        br.Position = position + num4;
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.ReserveInt64("SibOffset");
        bw.WriteInt32(this.InstanceCount);
        bw.WriteInt32(this.Unk1C);
        bw.ReserveInt64("TypeDataOffset");
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(MSB.ReambiguateName(this.Name), true);
        bw.FillInt64("SibOffset", bw.Position - position);
        bw.WriteUTF16(this.Placeholder, true);
        bw.Pad(8);
        if (this.HasTypeData)
        {
          bw.FillInt64("TypeDataOffset", bw.Position - position);
          this.WriteTypeData(bw);
        }
        else
          bw.FillInt64("TypeDataOffset", 0L);
      }

      internal virtual void WriteTypeData(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Type data should not be written for models with no type data.");
      }

      internal void CountInstances(List<MSBS.Part> parts)
      {
        this.InstanceCount = parts.Count<MSBS.Part>((Func<MSBS.Part, bool>) (p => p.ModelName == this.Name));
      }

      public override string ToString()
      {
        return string.Format("{0} {1}", (object) this.Type, (object) this.Name);
      }

      public class MapPiece : MSBS.Model
      {
        public override MSBS.ModelType Type
        {
          get
          {
            return MSBS.ModelType.MapPiece;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public bool UnkT00 { get; set; }

        public bool UnkT01 { get; set; }

        public bool UnkT02 { get; set; }

        public float UnkT04 { get; set; }

        public float UnkT08 { get; set; }

        public float UnkT0C { get; set; }

        public float UnkT10 { get; set; }

        public float UnkT14 { get; set; }

        public float UnkT18 { get; set; }

        public MapPiece()
        {
        }

        internal MapPiece(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadBoolean();
          this.UnkT01 = br.ReadBoolean();
          this.UnkT02 = br.ReadBoolean();
          int num = (int) br.AssertByte(new byte[1]);
          this.UnkT04 = br.ReadSingle();
          this.UnkT08 = br.ReadSingle();
          this.UnkT0C = br.ReadSingle();
          this.UnkT10 = br.ReadSingle();
          this.UnkT14 = br.ReadSingle();
          this.UnkT18 = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteBoolean(this.UnkT00);
          bw.WriteBoolean(this.UnkT01);
          bw.WriteBoolean(this.UnkT02);
          bw.WriteByte((byte) 0);
          bw.WriteSingle(this.UnkT04);
          bw.WriteSingle(this.UnkT08);
          bw.WriteSingle(this.UnkT0C);
          bw.WriteSingle(this.UnkT10);
          bw.WriteSingle(this.UnkT14);
          bw.WriteSingle(this.UnkT18);
          bw.WriteInt32(0);
        }
      }

      public class Object : MSBS.Model
      {
        public override MSBS.ModelType Type
        {
          get
          {
            return MSBS.ModelType.Object;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Object()
        {
        }

        internal Object(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class Enemy : MSBS.Model
      {
        public override MSBS.ModelType Type
        {
          get
          {
            return MSBS.ModelType.Enemy;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Enemy()
        {
        }

        internal Enemy(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class Player : MSBS.Model
      {
        public override MSBS.ModelType Type
        {
          get
          {
            return MSBS.ModelType.Player;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Player()
        {
        }

        internal Player(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class Collision : MSBS.Model
      {
        public override MSBS.ModelType Type
        {
          get
          {
            return MSBS.ModelType.Collision;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Collision()
        {
        }

        internal Collision(BinaryReaderEx br)
          : base(br)
        {
        }
      }
    }

    internal struct Entries
    {
      public List<MSBS.Model> Models;
      public List<MSBS.Event> Events;
      public List<MSBS.Region> Regions;
      public List<MSBS.Route> Routes;
      public List<MSBS.Part> Parts;
    }

    public abstract class Param<T> where T : MSBS.Entry
    {
      public int Unk00 { get; set; }

      public string Name { get; }

      internal Param(int unk00, string name)
      {
        this.Unk00 = unk00;
        this.Name = name;
      }

      internal List<T> Read(BinaryReaderEx br)
      {
        this.Unk00 = br.ReadInt32();
        int num1 = br.ReadInt32();
        long offset = br.ReadInt64();
        long[] numArray = br.ReadInt64s(num1 - 1);
        long num2 = br.ReadInt64();
        string utF16 = br.GetUTF16(offset);
        if (utF16 != this.Name)
          throw new InvalidDataException("Expected param \"" + this.Name + "\", got param \"" + utF16 + "\"");
        List<T> objList = new List<T>(num1 - 1);
        foreach (long num3 in numArray)
        {
          br.Position = num3;
          objList.Add(this.ReadEntry(br));
        }
        br.Position = num2;
        return objList;
      }

      internal abstract T ReadEntry(BinaryReaderEx br);

      internal virtual void Write(BinaryWriterEx bw, List<T> entries)
      {
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(entries.Count + 1);
        bw.ReserveInt64("ParamNameOffset");
        for (int index = 0; index < entries.Count; ++index)
          bw.ReserveInt64(string.Format("EntryOffset{0}", (object) index));
        bw.ReserveInt64("NextParamOffset");
        bw.FillInt64("ParamNameOffset", bw.Position);
        bw.WriteUTF16(this.Name, true);
        bw.Pad(8);
        int id = 0;
        System.Type type = (System.Type) null;
        for (int index = 0; index < entries.Count; ++index)
        {
          if (type != entries[index].GetType())
          {
            type = entries[index].GetType();
            id = 0;
          }
          bw.FillInt64(string.Format("EntryOffset{0}", (object) index), bw.Position);
          entries[index].Write(bw, id);
          ++id;
        }
      }

      public abstract List<T> GetEntries();

      public override string ToString()
      {
        return string.Format("0x{0:X2} {1}", (object) this.Unk00, (object) this.Name);
      }
    }

    public abstract class Entry : IMsbEntry
    {
      public string Name { get; set; }

      internal abstract void Write(BinaryWriterEx bw, int id);
    }

    public class EmptyParam : MSBS.Param<MSBS.Entry>
    {
      public EmptyParam(int unk00, string name)
        : base(unk00, name)
      {
      }

      internal override MSBS.Entry ReadEntry(BinaryReaderEx br)
      {
        throw new InvalidDataException("Expected param \"" + this.Name + "\" to be empty, but it wasn't.");
      }

      public override List<MSBS.Entry> GetEntries()
      {
        return new List<MSBS.Entry>();
      }
    }

    public enum PartType : uint
    {
      MapPiece = 0,
      Object = 1,
      Enemy = 2,
      Player = 4,
      Collision = 5,
      DummyObject = 9,
      DummyEnemy = 10, // 0x0000000A
      ConnectCollision = 11, // 0x0000000B
    }

    public class PartsParam : MSBS.Param<MSBS.Part>, IMsbParam<IMsbPart>
    {
      public List<MSBS.Part.MapPiece> MapPieces { get; set; }

      public List<MSBS.Part.Object> Objects { get; set; }

      public List<MSBS.Part.Enemy> Enemies { get; set; }

      public List<MSBS.Part.Player> Players { get; set; }

      public List<MSBS.Part.Collision> Collisions { get; set; }

      public List<MSBS.Part.DummyObject> DummyObjects { get; set; }

      public List<MSBS.Part.DummyEnemy> DummyEnemies { get; set; }

      public List<MSBS.Part.ConnectCollision> ConnectCollisions { get; set; }

      public PartsParam(int unk00 = 35)
        : base(unk00, "PARTS_PARAM_ST")
      {
        this.MapPieces = new List<MSBS.Part.MapPiece>();
        this.Objects = new List<MSBS.Part.Object>();
        this.Enemies = new List<MSBS.Part.Enemy>();
        this.Players = new List<MSBS.Part.Player>();
        this.Collisions = new List<MSBS.Part.Collision>();
        this.DummyObjects = new List<MSBS.Part.DummyObject>();
        this.DummyEnemies = new List<MSBS.Part.DummyEnemy>();
        this.ConnectCollisions = new List<MSBS.Part.ConnectCollision>();
      }

      internal override MSBS.Part ReadEntry(BinaryReaderEx br)
      {
        MSBS.PartType enum32 = br.GetEnum32<MSBS.PartType>(br.Position + 8L);
        switch (enum32)
        {
          case MSBS.PartType.MapPiece:
            MSBS.Part.MapPiece mapPiece = new MSBS.Part.MapPiece(br);
            this.MapPieces.Add(mapPiece);
            return (MSBS.Part) mapPiece;
          case MSBS.PartType.Object:
            MSBS.Part.Object @object = new MSBS.Part.Object(br);
            this.Objects.Add(@object);
            return (MSBS.Part) @object;
          case MSBS.PartType.Enemy:
            MSBS.Part.Enemy enemy = new MSBS.Part.Enemy(br);
            this.Enemies.Add(enemy);
            return (MSBS.Part) enemy;
          case MSBS.PartType.Player:
            MSBS.Part.Player player = new MSBS.Part.Player(br);
            this.Players.Add(player);
            return (MSBS.Part) player;
          case MSBS.PartType.Collision:
            MSBS.Part.Collision collision = new MSBS.Part.Collision(br);
            this.Collisions.Add(collision);
            return (MSBS.Part) collision;
          case MSBS.PartType.DummyObject:
            MSBS.Part.DummyObject dummyObject = new MSBS.Part.DummyObject(br);
            this.DummyObjects.Add(dummyObject);
            return (MSBS.Part) dummyObject;
          case MSBS.PartType.DummyEnemy:
            MSBS.Part.DummyEnemy dummyEnemy = new MSBS.Part.DummyEnemy(br);
            this.DummyEnemies.Add(dummyEnemy);
            return (MSBS.Part) dummyEnemy;
          case MSBS.PartType.ConnectCollision:
            MSBS.Part.ConnectCollision connectCollision = new MSBS.Part.ConnectCollision(br);
            this.ConnectCollisions.Add(connectCollision);
            return (MSBS.Part) connectCollision;
          default:
            throw new NotImplementedException(string.Format("Unimplemented part type: {0}", (object) enum32));
        }
      }

      public override List<MSBS.Part> GetEntries()
      {
        return SFUtil.ConcatAll<MSBS.Part>(new IEnumerable<MSBS.Part>[8]
        {
          (IEnumerable<MSBS.Part>) this.MapPieces,
          (IEnumerable<MSBS.Part>) this.Objects,
          (IEnumerable<MSBS.Part>) this.Enemies,
          (IEnumerable<MSBS.Part>) this.Players,
          (IEnumerable<MSBS.Part>) this.Collisions,
          (IEnumerable<MSBS.Part>) this.DummyObjects,
          (IEnumerable<MSBS.Part>) this.DummyEnemies,
          (IEnumerable<MSBS.Part>) this.ConnectCollisions
        });
      }

      IReadOnlyList<IMsbPart> IMsbParam<IMsbPart>.GetEntries()
      {
        return (IReadOnlyList<IMsbPart>) this.GetEntries();
      }
    }

    public abstract class Part : MSBS.Entry, IMsbPart, IMsbEntry
    {
      private int ModelIndex;

      public abstract MSBS.PartType Type { get; }

      internal abstract bool HasUnk1 { get; }

      internal abstract bool HasUnk2 { get; }

      internal abstract bool HasGparamConfig { get; }

      internal abstract bool HasUnk6 { get; }

      internal abstract bool HasUnk7 { get; }

      public string ModelName { get; set; }

      public string Placeholder { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      public Vector3 Scale { get; set; }

      public int EntityID { get; set; }

      public byte UnkE04 { get; set; }

      public byte UnkE05 { get; set; }

      public byte UnkE06 { get; set; }

      public byte LanternID { get; set; }

      public byte LodParamID { get; set; }

      public byte UnkE09 { get; set; }

      public bool IsPointLightShadowSrc { get; set; }

      public byte UnkE0B { get; set; }

      public bool IsShadowSrc { get; set; }

      public byte IsStaticShadowSrc { get; set; }

      public byte IsCascade3ShadowSrc { get; set; }

      public byte UnkE0F { get; set; }

      public byte UnkE10 { get; set; }

      public bool IsShadowDest { get; set; }

      public bool IsShadowOnly { get; set; }

      public bool DrawByReflectCam { get; set; }

      public bool DrawOnlyReflectCam { get; set; }

      public byte EnableOnAboveShadow { get; set; }

      public bool DisablePointLightEffect { get; set; }

      public byte UnkE17 { get; set; }

      public int UnkE18 { get; set; }

      public int[] EntityGroupIDs { get; private set; }

      public int UnkE3C { get; set; }

      public int UnkE40 { get; set; }

      internal Part()
      {
        this.Name = "";
        this.Placeholder = "";
        this.Scale = Vector3.One;
        this.EntityID = -1;
        this.EntityGroupIDs = new int[8];
        for (int index = 0; index < 8; ++index)
          this.EntityGroupIDs[index] = -1;
      }

      internal Part(MSBS.Part clone)
      {
        this.Name = clone.Name;
        this.ModelName = clone.ModelName;
        this.Placeholder = clone.Placeholder;
        this.Position = clone.Position;
        this.Rotation = clone.Rotation;
        this.Scale = clone.Scale;
        this.EntityID = clone.EntityID;
        this.UnkE04 = clone.UnkE04;
        this.UnkE05 = clone.UnkE05;
        this.UnkE06 = clone.UnkE06;
        this.LanternID = clone.LanternID;
        this.LodParamID = clone.LodParamID;
        this.UnkE09 = clone.UnkE09;
        this.IsPointLightShadowSrc = clone.IsPointLightShadowSrc;
        this.UnkE0B = clone.UnkE0B;
        this.IsShadowSrc = clone.IsShadowSrc;
        this.IsStaticShadowSrc = clone.IsStaticShadowSrc;
        this.IsCascade3ShadowSrc = clone.IsCascade3ShadowSrc;
        this.UnkE0F = clone.UnkE0F;
        this.UnkE10 = clone.UnkE10;
        this.IsShadowDest = clone.IsShadowDest;
        this.IsShadowOnly = clone.IsShadowOnly;
        this.DrawByReflectCam = clone.DrawByReflectCam;
        this.DrawOnlyReflectCam = clone.DrawOnlyReflectCam;
        this.EnableOnAboveShadow = clone.EnableOnAboveShadow;
        this.DisablePointLightEffect = clone.DisablePointLightEffect;
        this.UnkE17 = clone.UnkE17;
        this.UnkE18 = clone.UnkE18;
        this.EntityGroupIDs = (int[]) clone.EntityGroupIDs.Clone();
        this.UnkE3C = clone.UnkE3C;
        this.UnkE40 = clone.UnkE40;
      }

      internal Part(BinaryReaderEx br)
      {
        long position = br.Position;
        long num1 = br.ReadInt64();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        this.ModelIndex = br.ReadInt32();
        br.AssertInt32(new int[1]);
        long num3 = br.ReadInt64();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        br.AssertInt32(-1);
        br.AssertInt32(-1);
        br.AssertInt32(new int[1]);
        long num4 = br.ReadInt64();
        long num5 = br.ReadInt64();
        long num6 = br.ReadInt64();
        long num7 = br.ReadInt64();
        long num8 = br.ReadInt64();
        long num9 = br.ReadInt64();
        long num10 = br.ReadInt64();
        br.AssertInt64(new long[1]);
        br.AssertInt64(new long[1]);
        br.AssertInt64(new long[1]);
        this.Name = br.GetUTF16(position + num1);
        this.Placeholder = br.GetUTF16(position + num3);
        if (this.HasUnk1)
        {
          br.Position = position + num4;
          this.ReadUnk1(br);
        }
        if (this.HasUnk2)
        {
          br.Position = position + num5;
          this.ReadUnk2(br);
        }
        br.Position = position + num6;
        this.EntityID = br.ReadInt32();
        this.UnkE04 = br.ReadByte();
        this.UnkE05 = br.ReadByte();
        this.UnkE06 = br.ReadByte();
        this.LanternID = br.ReadByte();
        this.LodParamID = br.ReadByte();
        this.UnkE09 = br.ReadByte();
        this.IsPointLightShadowSrc = br.ReadBoolean();
        this.UnkE0B = br.ReadByte();
        this.IsShadowSrc = br.ReadBoolean();
        this.IsStaticShadowSrc = br.ReadByte();
        this.IsCascade3ShadowSrc = br.ReadByte();
        this.UnkE0F = br.ReadByte();
        this.UnkE10 = br.ReadByte();
        this.IsShadowDest = br.ReadBoolean();
        this.IsShadowOnly = br.ReadBoolean();
        this.DrawByReflectCam = br.ReadBoolean();
        this.DrawOnlyReflectCam = br.ReadBoolean();
        this.EnableOnAboveShadow = br.ReadByte();
        this.DisablePointLightEffect = br.ReadBoolean();
        this.UnkE17 = br.ReadByte();
        this.UnkE18 = br.ReadInt32();
        this.EntityGroupIDs = br.ReadInt32s(8);
        this.UnkE3C = br.ReadInt32();
        this.UnkE40 = br.ReadInt32();
        br.AssertPattern(16, (byte) 0);
        if (this.HasGparamConfig)
        {
          br.Position = position + num8;
          this.ReadGparamConfig(br);
        }
        if (this.HasUnk6)
        {
          br.Position = position + num9;
          this.ReadUnk6(br);
        }
        if (this.HasUnk7)
        {
          br.Position = position + num10;
          this.ReadUnk7(br);
        }
        br.Position = position + num7;
      }

      internal virtual void ReadUnk1(BinaryReaderEx br)
      {
        throw new InvalidOperationException("Unk struct 1 should not be read for parts with no unk struct 1.");
      }

      internal virtual void ReadUnk2(BinaryReaderEx br)
      {
        throw new InvalidOperationException("Unk struct 2 should not be read for parts with no unk struct 2.");
      }

      internal virtual void ReadGparamConfig(BinaryReaderEx br)
      {
        throw new InvalidOperationException("Unk struct 5 should not be read for parts with no unk struct 5.");
      }

      internal virtual void ReadUnk6(BinaryReaderEx br)
      {
        throw new InvalidOperationException("Unk struct 6 should not be read for parts with no unk struct 6.");
      }

      internal virtual void ReadUnk7(BinaryReaderEx br)
      {
        throw new InvalidOperationException("Unk struct 7 should not be read for parts with no unk struct 7.");
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.WriteInt32(this.ModelIndex);
        bw.WriteInt32(0);
        bw.ReserveInt64("SibOffset");
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.WriteVector3(this.Scale);
        bw.WriteInt32(-1);
        bw.WriteInt32(-1);
        bw.WriteInt32(0);
        bw.ReserveInt64("UnkOffset1");
        bw.ReserveInt64("UnkOffset2");
        bw.ReserveInt64("EntityDataOffset");
        bw.ReserveInt64("TypeDataOffset");
        bw.ReserveInt64("GparamOffset");
        bw.ReserveInt64("UnkOffset6");
        bw.ReserveInt64("UnkOffset7");
        bw.WriteInt64(0L);
        bw.WriteInt64(0L);
        bw.WriteInt64(0L);
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(MSB.ReambiguateName(this.Name), true);
        bw.FillInt64("SibOffset", bw.Position - position);
        bw.WriteUTF16(this.Placeholder, true);
        bw.Pad(8);
        if (this.HasUnk1)
        {
          bw.FillInt64("UnkOffset1", bw.Position - position);
          this.WriteUnk1(bw);
        }
        else
          bw.FillInt64("UnkOffset1", 0L);
        if (this.HasUnk2)
        {
          bw.FillInt64("UnkOffset2", bw.Position - position);
          this.WriteUnk2(bw);
        }
        else
          bw.FillInt64("UnkOffset2", 0L);
        bw.FillInt64("EntityDataOffset", bw.Position - position);
        bw.WriteInt32(this.EntityID);
        bw.WriteByte(this.UnkE04);
        bw.WriteByte(this.UnkE05);
        bw.WriteByte(this.UnkE06);
        bw.WriteByte(this.LanternID);
        bw.WriteByte(this.LodParamID);
        bw.WriteByte(this.UnkE09);
        bw.WriteBoolean(this.IsPointLightShadowSrc);
        bw.WriteByte(this.UnkE0B);
        bw.WriteBoolean(this.IsShadowSrc);
        bw.WriteByte(this.IsStaticShadowSrc);
        bw.WriteByte(this.IsCascade3ShadowSrc);
        bw.WriteByte(this.UnkE0F);
        bw.WriteByte(this.UnkE10);
        bw.WriteBoolean(this.IsShadowDest);
        bw.WriteBoolean(this.IsShadowOnly);
        bw.WriteBoolean(this.DrawByReflectCam);
        bw.WriteBoolean(this.DrawOnlyReflectCam);
        bw.WriteByte(this.EnableOnAboveShadow);
        bw.WriteBoolean(this.DisablePointLightEffect);
        bw.WriteByte(this.UnkE17);
        bw.WriteInt32(this.UnkE18);
        bw.WriteInt32s((IList<int>) this.EntityGroupIDs);
        bw.WriteInt32(this.UnkE3C);
        bw.WriteInt32(this.UnkE40);
        bw.WritePattern(16, (byte) 0);
        bw.Pad(8);
        bw.FillInt64("TypeDataOffset", bw.Position - position);
        this.WriteTypeData(bw);
        if (this.HasGparamConfig)
        {
          bw.FillInt64("GparamOffset", bw.Position - position);
          this.WriteGparamConfig(bw);
        }
        else
          bw.FillInt64("GparamOffset", 0L);
        if (this.HasUnk6)
        {
          bw.FillInt64("UnkOffset6", bw.Position - position);
          this.WriteUnk6(bw);
        }
        else
          bw.FillInt64("UnkOffset6", 0L);
        if (this.HasUnk7)
        {
          bw.FillInt64("UnkOffset7", bw.Position - position);
          this.WriteUnk7(bw);
        }
        else
          bw.FillInt64("UnkOffset7", 0L);
      }

      internal abstract void WriteTypeData(BinaryWriterEx bw);

      internal virtual void WriteUnk1(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Unk struct 1 should not be written for parts with no unk struct 1.");
      }

      internal virtual void WriteUnk2(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Unk struct 2 should not be written for parts with no unk struct 2.");
      }

      internal virtual void WriteGparamConfig(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Unk struct 5 should not be written for parts with no unk struct 5.");
      }

      internal virtual void WriteUnk6(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Unk struct 6 should not be written for parts with no unk struct 6.");
      }

      internal virtual void WriteUnk7(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Unk struct 7 should not be written for parts with no unk struct 7.");
      }

      internal virtual void GetNames(MSBS msb, MSBS.Entries entries)
      {
        this.ModelName = MSB.FindName<MSBS.Model>(entries.Models, this.ModelIndex);
      }

      internal virtual void GetIndices(MSBS msb, MSBS.Entries entries)
      {
        this.ModelIndex = MSB.FindIndex<MSBS.Model>(entries.Models, this.ModelName);
      }

      public override string ToString()
      {
        return string.Format("{0} {1}", (object) this.Type, (object) this.Name);
      }

      public class UnkStruct1
      {
        public uint[] CollisionMask { get; private set; }

        public byte Condition1 { get; set; }

        public byte Condition2 { get; set; }

        public UnkStruct1()
        {
          this.CollisionMask = new uint[48];
          this.Condition1 = (byte) 0;
          this.Condition2 = (byte) 0;
        }

        public UnkStruct1(MSBS.Part.UnkStruct1 clone)
        {
          this.CollisionMask = (uint[]) clone.CollisionMask.Clone();
          this.Condition1 = clone.Condition1;
          this.Condition2 = clone.Condition2;
        }

        internal UnkStruct1(BinaryReaderEx br)
        {
          this.CollisionMask = br.ReadUInt32s(48);
          this.Condition1 = br.ReadByte();
          this.Condition2 = br.ReadByte();
          int num = (int) br.AssertInt16(new short[1]);
          br.AssertPattern(192, (byte) 0);
        }

        internal void Write(BinaryWriterEx bw)
        {
          bw.WriteUInt32s((IList<uint>) this.CollisionMask);
          bw.WriteByte(this.Condition1);
          bw.WriteByte(this.Condition2);
          bw.WriteInt16((short) 0);
          bw.WritePattern(192, (byte) 0);
        }
      }

      public class UnkStruct2
      {
        public int Condition { get; set; }

        public int[] DispGroups { get; private set; }

        public short Unk24 { get; set; }

        public short Unk26 { get; set; }

        public UnkStruct2()
        {
          this.DispGroups = new int[8];
        }

        internal UnkStruct2(BinaryReaderEx br)
        {
          this.Condition = br.ReadInt32();
          this.DispGroups = br.ReadInt32s(8);
          this.Unk24 = br.ReadInt16();
          this.Unk26 = br.ReadInt16();
          br.AssertPattern(32, (byte) 0);
        }

        internal void Write(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.Condition);
          bw.WriteInt32s((IList<int>) this.DispGroups);
          bw.WriteInt16(this.Unk24);
          bw.WriteInt16(this.Unk26);
          bw.WritePattern(32, (byte) 0);
        }
      }

      public class GparamConfig
      {
        public int LightSetID { get; set; }

        public int FogParamID { get; set; }

        public int LightScatteringID { get; set; }

        public int EnvMapID { get; set; }

        public GparamConfig()
        {
        }

        public GparamConfig(MSBS.Part.GparamConfig clone)
        {
          this.LightSetID = clone.LightSetID;
          this.FogParamID = clone.FogParamID;
          this.LightScatteringID = clone.LightScatteringID;
          this.EnvMapID = clone.EnvMapID;
        }

        internal GparamConfig(BinaryReaderEx br)
        {
          this.LightSetID = br.ReadInt32();
          this.FogParamID = br.ReadInt32();
          this.LightScatteringID = br.ReadInt32();
          this.EnvMapID = br.ReadInt32();
          br.AssertPattern(16, (byte) 0);
        }

        internal void Write(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.LightSetID);
          bw.WriteInt32(this.FogParamID);
          bw.WriteInt32(this.LightScatteringID);
          bw.WriteInt32(this.EnvMapID);
          bw.WritePattern(16, (byte) 0);
        }

        public override string ToString()
        {
          return string.Format("{0}, {1}, {2}, {3}", (object) this.LightSetID, (object) this.FogParamID, (object) this.LightScatteringID, (object) this.EnvMapID);
        }
      }

      public class UnkStruct6
      {
        public sbyte[] EventIDs { get; private set; }

        public float Unk40 { get; set; }

        public UnkStruct6()
        {
          this.EventIDs = new sbyte[4];
        }

        internal UnkStruct6(BinaryReaderEx br)
        {
          br.AssertPattern(60, (byte) 0);
          this.EventIDs = br.ReadSBytes(4);
          this.Unk40 = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal void Write(BinaryWriterEx bw)
        {
          bw.WritePattern(60, (byte) 0);
          bw.WriteSBytes((IList<sbyte>) this.EventIDs);
          bw.WriteSingle(this.Unk40);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        public override string ToString()
        {
          return string.Format("EventID[{0,2}][{1,2}][{2,2}][{3,2}] {4:0.0}", (object) this.EventIDs[0], (object) this.EventIDs[1], (object) this.EventIDs[2], (object) this.EventIDs[3], (object) this.Unk40);
        }
      }

      public class UnkStruct7
      {
        public int Unk00 { get; set; }

        public int Unk04 { get; set; }

        public int GrassTypeParamID { get; set; }

        public int Unk0C { get; set; }

        public int Unk10 { get; set; }

        public int Unk14 { get; set; }

        public UnkStruct7()
        {
        }

        internal UnkStruct7(BinaryReaderEx br)
        {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.GrassTypeParamID = br.ReadInt32();
          this.Unk0C = br.ReadInt32();
          this.Unk10 = br.ReadInt32();
          this.Unk14 = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertInt32(new int[1]);
        }

        internal void Write(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.GrassTypeParamID);
          bw.WriteInt32(this.Unk0C);
          bw.WriteInt32(this.Unk10);
          bw.WriteInt32(this.Unk14);
          bw.WriteInt32(-1);
          bw.WriteInt32(0);
        }
      }

      public class MapPiece : MSBS.Part
      {
        public override MSBS.PartType Type
        {
          get
          {
            return MSBS.PartType.MapPiece;
          }
        }

        internal override bool HasUnk1
        {
          get
          {
            return true;
          }
        }

        internal override bool HasUnk2
        {
          get
          {
            return false;
          }
        }

        internal override bool HasGparamConfig
        {
          get
          {
            return true;
          }
        }

        internal override bool HasUnk6
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk7
        {
          get
          {
            return true;
          }
        }

        public MSBS.Part.UnkStruct1 Unk1 { get; set; }

        public MSBS.Part.GparamConfig Gparam { get; set; }

        public MSBS.Part.UnkStruct7 Unk7 { get; set; }

        public MapPiece()
        {
          this.Unk1 = new MSBS.Part.UnkStruct1();
          this.Gparam = new MSBS.Part.GparamConfig();
          this.Unk7 = new MSBS.Part.UnkStruct7();
        }

        internal MapPiece(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void ReadUnk1(BinaryReaderEx br)
        {
          this.Unk1 = new MSBS.Part.UnkStruct1(br);
        }

        internal override void ReadGparamConfig(BinaryReaderEx br)
        {
          this.Gparam = new MSBS.Part.GparamConfig(br);
        }

        internal override void ReadUnk7(BinaryReaderEx br)
        {
          this.Unk7 = new MSBS.Part.UnkStruct7(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void WriteUnk1(BinaryWriterEx bw)
        {
          this.Unk1.Write(bw);
        }

        internal override void WriteGparamConfig(BinaryWriterEx bw)
        {
          this.Gparam.Write(bw);
        }

        internal override void WriteUnk7(BinaryWriterEx bw)
        {
          this.Unk7.Write(bw);
        }
      }

      public class Object : MSBS.Part.DummyObject
      {
        public override MSBS.PartType Type
        {
          get
          {
            return MSBS.PartType.Object;
          }
        }

        internal override bool HasUnk1
        {
          get
          {
            return true;
          }
        }

        public MSBS.Part.UnkStruct1 Unk1 { get; set; }

        public Object()
        {
          this.Unk1 = new MSBS.Part.UnkStruct1();
        }

        public Object(MSBS.Part.Object clone)
          : base((MSBS.Part.DummyObject) clone)
        {
          this.Unk1 = new MSBS.Part.UnkStruct1(clone.Unk1);
        }

        public Object(MSBS.Part.DummyObject clone)
          : base(clone)
        {
          this.Unk1 = new MSBS.Part.UnkStruct1();
        }

        internal Object(BinaryReaderEx br)
          : base(br)
        {
        }

        internal override void ReadUnk1(BinaryReaderEx br)
        {
          this.Unk1 = new MSBS.Part.UnkStruct1(br);
        }

        internal override void WriteUnk1(BinaryWriterEx bw)
        {
          this.Unk1.Write(bw);
        }
      }

      public class Enemy : MSBS.Part.DummyEnemy
      {
        public override MSBS.PartType Type
        {
          get
          {
            return MSBS.PartType.Enemy;
          }
        }

        internal override bool HasUnk1
        {
          get
          {
            return true;
          }
        }

        public MSBS.Part.UnkStruct1 Unk1 { get; set; }

        public Enemy()
        {
          this.Unk1 = new MSBS.Part.UnkStruct1();
        }

        public Enemy(MSBS.Part.Enemy clone)
          : base((MSBS.Part.DummyEnemy) clone)
        {
          this.Unk1 = new MSBS.Part.UnkStruct1(clone.Unk1);
        }

        public Enemy(MSBS.Part.DummyEnemy clone)
          : base(clone)
        {
          this.Unk1 = new MSBS.Part.UnkStruct1();
        }

        internal Enemy(BinaryReaderEx br)
          : base(br)
        {
        }

        internal override void ReadUnk1(BinaryReaderEx br)
        {
          this.Unk1 = new MSBS.Part.UnkStruct1(br);
        }

        internal override void WriteUnk1(BinaryWriterEx bw)
        {
          this.Unk1.Write(bw);
        }
      }

      public class Player : MSBS.Part
      {
        public override MSBS.PartType Type
        {
          get
          {
            return MSBS.PartType.Player;
          }
        }

        internal override bool HasUnk1
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk2
        {
          get
          {
            return false;
          }
        }

        internal override bool HasGparamConfig
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk6
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk7
        {
          get
          {
            return false;
          }
        }

        public Player()
        {
        }

        internal Player(BinaryReaderEx br)
          : base(br)
        {
          br.AssertPattern(16, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WritePattern(16, (byte) 0);
        }
      }

      public class Collision : MSBS.Part
      {
        public override MSBS.PartType Type
        {
          get
          {
            return MSBS.PartType.Collision;
          }
        }

        internal override bool HasUnk1
        {
          get
          {
            return true;
          }
        }

        internal override bool HasUnk2
        {
          get
          {
            return true;
          }
        }

        internal override bool HasGparamConfig
        {
          get
          {
            return true;
          }
        }

        internal override bool HasUnk6
        {
          get
          {
            return true;
          }
        }

        internal override bool HasUnk7
        {
          get
          {
            return false;
          }
        }

        public MSBS.Part.UnkStruct1 Unk1 { get; set; }

        public MSBS.Part.UnkStruct2 Unk2 { get; set; }

        public MSBS.Part.GparamConfig Gparam { get; set; }

        public MSBS.Part.UnkStruct6 Unk6 { get; set; }

        public byte HitFilterID { get; set; }

        public byte SoundSpaceType { get; set; }

        public float ReflectPlaneHeight { get; set; }

        public short MapNameID { get; set; }

        public bool DisableStart { get; set; }

        public byte UnkT17 { get; set; }

        public int DisableBonfireEntityID { get; set; }

        public byte UnkT24 { get; set; }

        public byte UnkT25 { get; set; }

        public byte UnkT26 { get; set; }

        public byte MapVisibility { get; set; }

        public int PlayRegionID { get; set; }

        public short LockCamParamID { get; set; }

        public int UnkT3C { get; set; }

        public int UnkT40 { get; set; }

        public float UnkT44 { get; set; }

        public float UnkT48 { get; set; }

        public int UnkT4C { get; set; }

        public float UnkT50 { get; set; }

        public float UnkT54 { get; set; }

        public Collision()
        {
          this.Unk1 = new MSBS.Part.UnkStruct1();
          this.Unk2 = new MSBS.Part.UnkStruct2();
          this.Gparam = new MSBS.Part.GparamConfig();
          this.Unk6 = new MSBS.Part.UnkStruct6();
          this.DisableBonfireEntityID = -1;
        }

        internal Collision(BinaryReaderEx br)
          : base(br)
        {
          this.HitFilterID = br.ReadByte();
          this.SoundSpaceType = br.ReadByte();
          int num1 = (int) br.AssertInt16(new short[1]);
          this.ReflectPlaneHeight = br.ReadSingle();
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.MapNameID = br.ReadInt16();
          this.DisableStart = br.ReadBoolean();
          this.UnkT17 = br.ReadByte();
          this.DisableBonfireEntityID = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.UnkT24 = br.ReadByte();
          this.UnkT25 = br.ReadByte();
          this.UnkT26 = br.ReadByte();
          this.MapVisibility = br.ReadByte();
          this.PlayRegionID = br.ReadInt32();
          this.LockCamParamID = br.ReadInt16();
          int num2 = (int) br.AssertInt16((short) -1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.UnkT3C = br.ReadInt32();
          this.UnkT40 = br.ReadInt32();
          this.UnkT44 = br.ReadSingle();
          this.UnkT48 = br.ReadSingle();
          this.UnkT4C = br.ReadInt32();
          this.UnkT50 = br.ReadSingle();
          this.UnkT54 = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void ReadUnk1(BinaryReaderEx br)
        {
          this.Unk1 = new MSBS.Part.UnkStruct1(br);
        }

        internal override void ReadUnk2(BinaryReaderEx br)
        {
          this.Unk2 = new MSBS.Part.UnkStruct2(br);
        }

        internal override void ReadGparamConfig(BinaryReaderEx br)
        {
          this.Gparam = new MSBS.Part.GparamConfig(br);
        }

        internal override void ReadUnk6(BinaryReaderEx br)
        {
          this.Unk6 = new MSBS.Part.UnkStruct6(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteByte(this.HitFilterID);
          bw.WriteByte(this.SoundSpaceType);
          bw.WriteInt16((short) 0);
          bw.WriteSingle(this.ReflectPlaneHeight);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt16(this.MapNameID);
          bw.WriteBoolean(this.DisableStart);
          bw.WriteByte(this.UnkT17);
          bw.WriteInt32(this.DisableBonfireEntityID);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteByte(this.UnkT24);
          bw.WriteByte(this.UnkT25);
          bw.WriteByte(this.UnkT26);
          bw.WriteByte(this.MapVisibility);
          bw.WriteInt32(this.PlayRegionID);
          bw.WriteInt16(this.LockCamParamID);
          bw.WriteInt16((short) -1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.UnkT3C);
          bw.WriteInt32(this.UnkT40);
          bw.WriteSingle(this.UnkT44);
          bw.WriteSingle(this.UnkT48);
          bw.WriteInt32(this.UnkT4C);
          bw.WriteSingle(this.UnkT50);
          bw.WriteSingle(this.UnkT54);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void WriteUnk1(BinaryWriterEx bw)
        {
          this.Unk1.Write(bw);
        }

        internal override void WriteUnk2(BinaryWriterEx bw)
        {
          this.Unk2.Write(bw);
        }

        internal override void WriteGparamConfig(BinaryWriterEx bw)
        {
          this.Gparam.Write(bw);
        }

        internal override void WriteUnk6(BinaryWriterEx bw)
        {
          this.Unk6.Write(bw);
        }
      }

      public class DummyObject : MSBS.Part
      {
        private int ObjPartIndex1;
        private int ObjPartIndex2;
        private int ObjPartIndex3;

        public override MSBS.PartType Type
        {
          get
          {
            return MSBS.PartType.DummyObject;
          }
        }

        internal override bool HasUnk1
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk2
        {
          get
          {
            return false;
          }
        }

        internal override bool HasGparamConfig
        {
          get
          {
            return true;
          }
        }

        internal override bool HasUnk6
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk7
        {
          get
          {
            return false;
          }
        }

        public MSBS.Part.GparamConfig Gparam { get; set; }

        public string ObjPartName1 { get; set; }

        public byte UnkT0C { get; set; }

        public bool EnableObjAnimNetSyncStructure { get; set; }

        public byte UnkT0E { get; set; }

        public bool SetMainObjStructureBooleans { get; set; }

        public short AnimID { get; set; }

        public short UnkT18 { get; set; }

        public short UnkT1A { get; set; }

        public string ObjPartName2 { get; set; }

        public string ObjPartName3 { get; set; }

        public DummyObject()
        {
          this.Gparam = new MSBS.Part.GparamConfig();
        }

        public DummyObject(MSBS.Part.DummyObject clone)
          : base((MSBS.Part) clone)
        {
          this.Gparam = new MSBS.Part.GparamConfig(clone.Gparam);
          this.ObjPartName1 = clone.ObjPartName1;
          this.UnkT0C = clone.UnkT0C;
          this.EnableObjAnimNetSyncStructure = clone.EnableObjAnimNetSyncStructure;
          this.UnkT0E = clone.UnkT0E;
          this.SetMainObjStructureBooleans = clone.SetMainObjStructureBooleans;
          this.AnimID = clone.AnimID;
          this.UnkT18 = clone.UnkT18;
          this.UnkT1A = clone.UnkT1A;
          this.ObjPartName2 = clone.ObjPartName2;
          this.ObjPartName3 = clone.ObjPartName3;
        }

        internal DummyObject(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.ObjPartIndex1 = br.ReadInt32();
          this.UnkT0C = br.ReadByte();
          this.EnableObjAnimNetSyncStructure = br.ReadBoolean();
          this.UnkT0E = br.ReadByte();
          this.SetMainObjStructureBooleans = br.ReadBoolean();
          this.AnimID = br.ReadInt16();
          int num = (int) br.AssertInt16((short) -1);
          br.AssertInt32(-1);
          this.UnkT18 = br.ReadInt16();
          this.UnkT1A = br.ReadInt16();
          br.AssertInt32(-1);
          this.ObjPartIndex2 = br.ReadInt32();
          this.ObjPartIndex3 = br.ReadInt32();
        }

        internal override void ReadGparamConfig(BinaryReaderEx br)
        {
          this.Gparam = new MSBS.Part.GparamConfig(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.ObjPartIndex1);
          bw.WriteByte(this.UnkT0C);
          bw.WriteBoolean(this.EnableObjAnimNetSyncStructure);
          bw.WriteByte(this.UnkT0E);
          bw.WriteBoolean(this.SetMainObjStructureBooleans);
          bw.WriteInt16(this.AnimID);
          bw.WriteInt16((short) -1);
          bw.WriteInt32(-1);
          bw.WriteInt16(this.UnkT18);
          bw.WriteInt16(this.UnkT1A);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.ObjPartIndex2);
          bw.WriteInt32(this.ObjPartIndex3);
        }

        internal override void WriteGparamConfig(BinaryWriterEx bw)
        {
          this.Gparam.Write(bw);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.ObjPartName1 = MSB.FindName<MSBS.Part>(entries.Parts, this.ObjPartIndex1);
          this.ObjPartName2 = MSB.FindName<MSBS.Part>(entries.Parts, this.ObjPartIndex2);
          this.ObjPartName3 = MSB.FindName<MSBS.Part>(entries.Parts, this.ObjPartIndex3);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.ObjPartIndex1 = MSB.FindIndex<MSBS.Part>(entries.Parts, this.ObjPartName1);
          this.ObjPartIndex2 = MSB.FindIndex<MSBS.Part>(entries.Parts, this.ObjPartName2);
          this.ObjPartIndex3 = MSB.FindIndex<MSBS.Part>(entries.Parts, this.ObjPartName3);
        }
      }

      public class DummyEnemy : MSBS.Part
      {
        private int CollisionPartIndex;

        public override MSBS.PartType Type
        {
          get
          {
            return MSBS.PartType.DummyEnemy;
          }
        }

        internal override bool HasUnk1
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk2
        {
          get
          {
            return false;
          }
        }

        internal override bool HasGparamConfig
        {
          get
          {
            return true;
          }
        }

        internal override bool HasUnk6
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk7
        {
          get
          {
            return false;
          }
        }

        public MSBS.Part.GparamConfig Gparam { get; set; }

        public int ThinkParamID { get; set; }

        public int NPCParamID { get; set; }

        public int UnkT10 { get; set; }

        public short ChrManipulatorAllocationParameter { get; set; }

        public int CharaInitID { get; set; }

        public string CollisionPartName { get; set; }

        public short UnkT20 { get; set; }

        public short UnkT22 { get; set; }

        public int UnkT24 { get; set; }

        public int BackupEventAnimID { get; set; }

        public int EventFlagID { get; set; }

        public int EventFlagCompareState { get; set; }

        public int UnkT48 { get; set; }

        public int UnkT4C { get; set; }

        public int UnkT50 { get; set; }

        public int UnkT78 { get; set; }

        public float UnkT84 { get; set; }

        public DummyEnemy()
        {
          this.Gparam = new MSBS.Part.GparamConfig();
          this.ThinkParamID = -1;
          this.NPCParamID = -1;
          this.UnkT10 = -1;
          this.CharaInitID = -1;
          this.BackupEventAnimID = -1;
          this.EventFlagID = -1;
        }

        public DummyEnemy(MSBS.Part.DummyEnemy clone)
          : base((MSBS.Part) clone)
        {
          this.Gparam = new MSBS.Part.GparamConfig(clone.Gparam);
          this.ThinkParamID = clone.ThinkParamID;
          this.NPCParamID = clone.NPCParamID;
          this.UnkT10 = clone.UnkT10;
          this.ChrManipulatorAllocationParameter = clone.ChrManipulatorAllocationParameter;
          this.CharaInitID = clone.CharaInitID;
          this.CollisionPartName = clone.CollisionPartName;
          this.UnkT20 = clone.UnkT20;
          this.UnkT22 = clone.UnkT22;
          this.UnkT24 = clone.UnkT24;
          this.BackupEventAnimID = clone.BackupEventAnimID;
          this.EventFlagID = clone.EventFlagID;
          this.EventFlagCompareState = clone.EventFlagCompareState;
          this.UnkT48 = clone.UnkT48;
          this.UnkT4C = clone.UnkT4C;
          this.UnkT50 = clone.UnkT50;
          this.UnkT78 = clone.UnkT78;
          this.UnkT84 = clone.UnkT84;
        }

        internal DummyEnemy(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.ThinkParamID = br.ReadInt32();
          this.NPCParamID = br.ReadInt32();
          this.UnkT10 = br.ReadInt32();
          int num1 = (int) br.AssertInt16(new short[1]);
          this.ChrManipulatorAllocationParameter = br.ReadInt16();
          this.CharaInitID = br.ReadInt32();
          this.CollisionPartIndex = br.ReadInt32();
          this.UnkT20 = br.ReadInt16();
          this.UnkT22 = br.ReadInt16();
          this.UnkT24 = br.ReadInt32();
          br.AssertPattern(16, byte.MaxValue);
          this.BackupEventAnimID = br.ReadInt32();
          br.AssertInt32(-1);
          this.EventFlagID = br.ReadInt32();
          this.EventFlagCompareState = br.ReadInt32();
          this.UnkT48 = br.ReadInt32();
          this.UnkT4C = br.ReadInt32();
          this.UnkT50 = br.ReadInt32();
          br.AssertInt32(1);
          br.AssertInt32(-1);
          br.AssertInt32(1);
          br.AssertPattern(24, (byte) 0);
          this.UnkT78 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.UnkT84 = br.ReadSingle();
          for (int index = 0; index < 5; ++index)
          {
            br.AssertInt32(-1);
            int num2 = (int) br.AssertInt16((short) -1);
            int num3 = (int) br.AssertInt16((short) 10);
          }
          br.AssertPattern(16, (byte) 0);
        }

        internal override void ReadGparamConfig(BinaryReaderEx br)
        {
          this.Gparam = new MSBS.Part.GparamConfig(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.ThinkParamID);
          bw.WriteInt32(this.NPCParamID);
          bw.WriteInt32(this.UnkT10);
          bw.WriteInt16((short) 0);
          bw.WriteInt16(this.ChrManipulatorAllocationParameter);
          bw.WriteInt32(this.CharaInitID);
          bw.WriteInt32(this.CollisionPartIndex);
          bw.WriteInt16(this.UnkT20);
          bw.WriteInt16(this.UnkT22);
          bw.WriteInt32(this.UnkT24);
          bw.WritePattern(16, byte.MaxValue);
          bw.WriteInt32(this.BackupEventAnimID);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.EventFlagID);
          bw.WriteInt32(this.EventFlagCompareState);
          bw.WriteInt32(this.UnkT48);
          bw.WriteInt32(this.UnkT4C);
          bw.WriteInt32(this.UnkT50);
          bw.WriteInt32(1);
          bw.WriteInt32(-1);
          bw.WriteInt32(1);
          bw.WritePattern(24, (byte) 0);
          bw.WriteInt32(this.UnkT78);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteSingle(this.UnkT84);
          for (int index = 0; index < 5; ++index)
          {
            bw.WriteInt32(-1);
            bw.WriteInt16((short) -1);
            bw.WriteInt16((short) 10);
          }
          bw.WritePattern(16, (byte) 0);
        }

        internal override void WriteGparamConfig(BinaryWriterEx bw)
        {
          this.Gparam.Write(bw);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.CollisionPartName = MSB.FindName<MSBS.Part>(entries.Parts, this.CollisionPartIndex);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.CollisionPartIndex = MSB.FindIndex<MSBS.Part>(entries.Parts, this.CollisionPartName);
        }
      }

      public class ConnectCollision : MSBS.Part
      {
        private int CollisionIndex;

        public override MSBS.PartType Type
        {
          get
          {
            return MSBS.PartType.ConnectCollision;
          }
        }

        internal override bool HasUnk1
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk2
        {
          get
          {
            return true;
          }
        }

        internal override bool HasGparamConfig
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk6
        {
          get
          {
            return false;
          }
        }

        internal override bool HasUnk7
        {
          get
          {
            return false;
          }
        }

        public MSBS.Part.UnkStruct2 Unk2 { get; set; }

        public string CollisionName { get; set; }

        public byte[] MapID { get; private set; }

        public ConnectCollision()
        {
          this.Unk2 = new MSBS.Part.UnkStruct2();
          this.MapID = new byte[4];
        }

        internal ConnectCollision(BinaryReaderEx br)
          : base(br)
        {
          this.CollisionIndex = br.ReadInt32();
          this.MapID = br.ReadBytes(4);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void ReadUnk2(BinaryReaderEx br)
        {
          this.Unk2 = new MSBS.Part.UnkStruct2(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.CollisionIndex);
          bw.WriteBytes(this.MapID);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void WriteUnk2(BinaryWriterEx bw)
        {
          this.Unk2.Write(bw);
        }

        internal override void GetNames(MSBS msb, MSBS.Entries entries)
        {
          base.GetNames(msb, entries);
          this.CollisionName = MSB.FindName<MSBS.Part.Collision>(msb.Parts.Collisions, this.CollisionIndex);
        }

        internal override void GetIndices(MSBS msb, MSBS.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.CollisionIndex = MSB.FindIndex<MSBS.Part.Collision>(msb.Parts.Collisions, this.CollisionName);
        }
      }
    }

    public enum RegionType : uint
    {
      Region0 = 0,
      InvasionPoint = 1,
      EnvironmentMapPoint = 2,
      Sound = 4,
      SFX = 5,
      WindSFX = 6,
      SpawnPoint = 8,
      WalkRoute = 11, // 0x0000000B
      WarpPoint = 13, // 0x0000000D
      ActivationArea = 14, // 0x0000000E
      Event = 15, // 0x0000000F
      EnvironmentMapEffectBox = 17, // 0x00000011
      WindArea = 18, // 0x00000012
      MufflingBox = 20, // 0x00000014
      MufflingPortal = 21, // 0x00000015
      Region23 = 23, // 0x00000017
      Region24 = 24, // 0x00000018
      PartsGroup = 25, // 0x00000019
      AutoDrawGroup = 26, // 0x0000001A
      Other = 4294967295, // 0xFFFFFFFF
    }

    public class PointParam : MSBS.Param<MSBS.Region>, IMsbParam<IMsbRegion>
    {
      public List<MSBS.Region.Region0> Region0s { get; set; }

      public List<MSBS.Region.InvasionPoint> InvasionPoints { get; set; }

      public List<MSBS.Region.EnvironmentMapPoint> EnvironmentMapPoints { get; set; }

      public List<MSBS.Region.Sound> Sounds { get; set; }

      public List<MSBS.Region.SFX> SFXs { get; set; }

      public List<MSBS.Region.WindSFX> WindSFXs { get; set; }

      public List<MSBS.Region.SpawnPoint> SpawnPoints { get; set; }

      public List<MSBS.Region.WalkRoute> WalkRoutes { get; set; }

      public List<MSBS.Region.WarpPoint> WarpPoints { get; set; }

      public List<MSBS.Region.ActivationArea> ActivationAreas { get; set; }

      public List<MSBS.Region.Event> Events { get; set; }

      public List<MSBS.Region.EnvironmentMapEffectBox> EnvironmentMapEffectBoxes { get; set; }

      public List<MSBS.Region.WindArea> WindAreas { get; set; }

      public List<MSBS.Region.MufflingBox> MufflingBoxes { get; set; }

      public List<MSBS.Region.MufflingPortal> MufflingPortals { get; set; }

      public List<MSBS.Region.Region23> Region23s { get; set; }

      public List<MSBS.Region.Region24> Region24s { get; set; }

      public List<MSBS.Region.PartsGroup> PartsGroups { get; set; }

      public List<MSBS.Region.AutoDrawGroup> AutoDrawGroups { get; set; }

      public List<MSBS.Region.Other> Others { get; set; }

      public PointParam(int unk00 = 35)
        : base(unk00, "POINT_PARAM_ST")
      {
        this.Region0s = new List<MSBS.Region.Region0>();
        this.InvasionPoints = new List<MSBS.Region.InvasionPoint>();
        this.EnvironmentMapPoints = new List<MSBS.Region.EnvironmentMapPoint>();
        this.Sounds = new List<MSBS.Region.Sound>();
        this.SFXs = new List<MSBS.Region.SFX>();
        this.WindSFXs = new List<MSBS.Region.WindSFX>();
        this.SpawnPoints = new List<MSBS.Region.SpawnPoint>();
        this.WalkRoutes = new List<MSBS.Region.WalkRoute>();
        this.WarpPoints = new List<MSBS.Region.WarpPoint>();
        this.ActivationAreas = new List<MSBS.Region.ActivationArea>();
        this.Events = new List<MSBS.Region.Event>();
        this.EnvironmentMapEffectBoxes = new List<MSBS.Region.EnvironmentMapEffectBox>();
        this.WindAreas = new List<MSBS.Region.WindArea>();
        this.MufflingBoxes = new List<MSBS.Region.MufflingBox>();
        this.MufflingPortals = new List<MSBS.Region.MufflingPortal>();
        this.Region23s = new List<MSBS.Region.Region23>();
        this.Region24s = new List<MSBS.Region.Region24>();
        this.PartsGroups = new List<MSBS.Region.PartsGroup>();
        this.AutoDrawGroups = new List<MSBS.Region.AutoDrawGroup>();
        this.Others = new List<MSBS.Region.Other>();
      }

      internal override MSBS.Region ReadEntry(BinaryReaderEx br)
      {
        MSBS.RegionType enum32 = br.GetEnum32<MSBS.RegionType>(br.Position + 8L);
        switch (enum32)
        {
          case MSBS.RegionType.Region0:
            MSBS.Region.Region0 region0 = new MSBS.Region.Region0(br);
            this.Region0s.Add(region0);
            return (MSBS.Region) region0;
          case MSBS.RegionType.InvasionPoint:
            MSBS.Region.InvasionPoint invasionPoint = new MSBS.Region.InvasionPoint(br);
            this.InvasionPoints.Add(invasionPoint);
            return (MSBS.Region) invasionPoint;
          case MSBS.RegionType.EnvironmentMapPoint:
            MSBS.Region.EnvironmentMapPoint environmentMapPoint = new MSBS.Region.EnvironmentMapPoint(br);
            this.EnvironmentMapPoints.Add(environmentMapPoint);
            return (MSBS.Region) environmentMapPoint;
          case MSBS.RegionType.Sound:
            MSBS.Region.Sound sound = new MSBS.Region.Sound(br);
            this.Sounds.Add(sound);
            return (MSBS.Region) sound;
          case MSBS.RegionType.SFX:
            MSBS.Region.SFX sfx = new MSBS.Region.SFX(br);
            this.SFXs.Add(sfx);
            return (MSBS.Region) sfx;
          case MSBS.RegionType.WindSFX:
            MSBS.Region.WindSFX windSfx = new MSBS.Region.WindSFX(br);
            this.WindSFXs.Add(windSfx);
            return (MSBS.Region) windSfx;
          case MSBS.RegionType.SpawnPoint:
            MSBS.Region.SpawnPoint spawnPoint = new MSBS.Region.SpawnPoint(br);
            this.SpawnPoints.Add(spawnPoint);
            return (MSBS.Region) spawnPoint;
          case MSBS.RegionType.WalkRoute:
            MSBS.Region.WalkRoute walkRoute = new MSBS.Region.WalkRoute(br);
            this.WalkRoutes.Add(walkRoute);
            return (MSBS.Region) walkRoute;
          case MSBS.RegionType.WarpPoint:
            MSBS.Region.WarpPoint warpPoint = new MSBS.Region.WarpPoint(br);
            this.WarpPoints.Add(warpPoint);
            return (MSBS.Region) warpPoint;
          case MSBS.RegionType.ActivationArea:
            MSBS.Region.ActivationArea activationArea = new MSBS.Region.ActivationArea(br);
            this.ActivationAreas.Add(activationArea);
            return (MSBS.Region) activationArea;
          case MSBS.RegionType.Event:
            MSBS.Region.Event @event = new MSBS.Region.Event(br);
            this.Events.Add(@event);
            return (MSBS.Region) @event;
          case MSBS.RegionType.EnvironmentMapEffectBox:
            MSBS.Region.EnvironmentMapEffectBox environmentMapEffectBox = new MSBS.Region.EnvironmentMapEffectBox(br);
            this.EnvironmentMapEffectBoxes.Add(environmentMapEffectBox);
            return (MSBS.Region) environmentMapEffectBox;
          case MSBS.RegionType.WindArea:
            MSBS.Region.WindArea windArea = new MSBS.Region.WindArea(br);
            this.WindAreas.Add(windArea);
            return (MSBS.Region) windArea;
          case MSBS.RegionType.MufflingBox:
            MSBS.Region.MufflingBox mufflingBox = new MSBS.Region.MufflingBox(br);
            this.MufflingBoxes.Add(mufflingBox);
            return (MSBS.Region) mufflingBox;
          case MSBS.RegionType.MufflingPortal:
            MSBS.Region.MufflingPortal mufflingPortal = new MSBS.Region.MufflingPortal(br);
            this.MufflingPortals.Add(mufflingPortal);
            return (MSBS.Region) mufflingPortal;
          case MSBS.RegionType.Region23:
            MSBS.Region.Region23 region23 = new MSBS.Region.Region23(br);
            this.Region23s.Add(region23);
            return (MSBS.Region) region23;
          case MSBS.RegionType.Region24:
            MSBS.Region.Region24 region24 = new MSBS.Region.Region24(br);
            this.Region24s.Add(region24);
            return (MSBS.Region) region24;
          case MSBS.RegionType.PartsGroup:
            MSBS.Region.PartsGroup partsGroup = new MSBS.Region.PartsGroup(br);
            this.PartsGroups.Add(partsGroup);
            return (MSBS.Region) partsGroup;
          case MSBS.RegionType.AutoDrawGroup:
            MSBS.Region.AutoDrawGroup autoDrawGroup = new MSBS.Region.AutoDrawGroup(br);
            this.AutoDrawGroups.Add(autoDrawGroup);
            return (MSBS.Region) autoDrawGroup;
          case MSBS.RegionType.Other:
            MSBS.Region.Other other = new MSBS.Region.Other(br);
            this.Others.Add(other);
            return (MSBS.Region) other;
          default:
            throw new NotImplementedException(string.Format("Unimplemented region type: {0}", (object) enum32));
        }
      }

      public override List<MSBS.Region> GetEntries()
      {
        return SFUtil.ConcatAll<MSBS.Region>(new IEnumerable<MSBS.Region>[20]
        {
          (IEnumerable<MSBS.Region>) this.InvasionPoints,
          (IEnumerable<MSBS.Region>) this.EnvironmentMapPoints,
          (IEnumerable<MSBS.Region>) this.Sounds,
          (IEnumerable<MSBS.Region>) this.SFXs,
          (IEnumerable<MSBS.Region>) this.WindSFXs,
          (IEnumerable<MSBS.Region>) this.SpawnPoints,
          (IEnumerable<MSBS.Region>) this.WalkRoutes,
          (IEnumerable<MSBS.Region>) this.WarpPoints,
          (IEnumerable<MSBS.Region>) this.ActivationAreas,
          (IEnumerable<MSBS.Region>) this.Events,
          (IEnumerable<MSBS.Region>) this.Region0s,
          (IEnumerable<MSBS.Region>) this.EnvironmentMapEffectBoxes,
          (IEnumerable<MSBS.Region>) this.WindAreas,
          (IEnumerable<MSBS.Region>) this.MufflingBoxes,
          (IEnumerable<MSBS.Region>) this.MufflingPortals,
          (IEnumerable<MSBS.Region>) this.Region23s,
          (IEnumerable<MSBS.Region>) this.Region24s,
          (IEnumerable<MSBS.Region>) this.PartsGroups,
          (IEnumerable<MSBS.Region>) this.AutoDrawGroups,
          (IEnumerable<MSBS.Region>) this.Others
        });
      }

      IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries()
      {
        return (IReadOnlyList<IMsbRegion>) this.GetEntries();
      }
    }

    public abstract class Region : MSBS.Entry, IMsbRegion, IMsbEntry
    {
      private int ActivationPartIndex;

      public abstract MSBS.RegionType Type { get; }

      internal abstract bool HasTypeData { get; }

      public MSBS.Shape Shape { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      public int Unk2C { get; set; }

      public uint MapStudioLayer { get; set; }

      public List<short> UnkA { get; set; }

      public List<short> UnkB { get; set; }

      public string ActivationPartName { get; set; }

      public int EntityID { get; set; }

      internal Region()
      {
        this.Name = "";
        this.Shape = (MSBS.Shape) new MSBS.Shape.Point();
        this.MapStudioLayer = uint.MaxValue;
        this.UnkA = new List<short>();
        this.UnkB = new List<short>();
        this.EntityID = -1;
      }

      internal Region(BinaryReaderEx br)
      {
        long position = br.Position;
        long num1 = br.ReadInt64();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        MSBS.ShapeType shapeType = br.ReadEnum32<MSBS.ShapeType>();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Unk2C = br.ReadInt32();
        long num3 = br.ReadInt64();
        long num4 = br.ReadInt64();
        br.AssertInt32(-1);
        this.MapStudioLayer = br.ReadUInt32();
        long num5 = br.ReadInt64();
        long num6 = br.ReadInt64();
        long num7 = br.ReadInt64();
        this.Name = br.GetUTF16(position + num1);
        br.Position = position + num3;
        short num8 = br.ReadInt16();
        this.UnkA = new List<short>((IEnumerable<short>) br.ReadInt16s((int) num8));
        br.Position = position + num4;
        short num9 = br.ReadInt16();
        this.UnkB = new List<short>((IEnumerable<short>) br.ReadInt16s((int) num9));
        br.Position = position + num5;
        switch (shapeType)
        {
          case MSBS.ShapeType.Point:
            this.Shape = (MSBS.Shape) new MSBS.Shape.Point();
            break;
          case MSBS.ShapeType.Circle:
            this.Shape = (MSBS.Shape) new MSBS.Shape.Circle(br);
            break;
          case MSBS.ShapeType.Sphere:
            this.Shape = (MSBS.Shape) new MSBS.Shape.Sphere(br);
            break;
          case MSBS.ShapeType.Cylinder:
            this.Shape = (MSBS.Shape) new MSBS.Shape.Cylinder(br);
            break;
          case MSBS.ShapeType.Rect:
            this.Shape = (MSBS.Shape) new MSBS.Shape.Rect(br);
            break;
          case MSBS.ShapeType.Box:
            this.Shape = (MSBS.Shape) new MSBS.Shape.Box(br);
            break;
          case MSBS.ShapeType.Composite:
            this.Shape = (MSBS.Shape) new MSBS.Shape.Composite(br);
            break;
          default:
            throw new NotImplementedException(string.Format("Unimplemented shape type: {0}", (object) shapeType));
        }
        br.Position = position + num6;
        this.ActivationPartIndex = br.ReadInt32();
        this.EntityID = br.ReadInt32();
        br.Position = position + num7;
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.WriteUInt32((uint) this.Shape.Type);
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.WriteInt32(this.Unk2C);
        bw.ReserveInt64("BaseDataOffset1");
        bw.ReserveInt64("BaseDataOffset2");
        bw.WriteInt32(-1);
        bw.WriteUInt32(this.MapStudioLayer);
        bw.ReserveInt64("ShapeDataOffset");
        bw.ReserveInt64("BaseDataOffset3");
        bw.ReserveInt64("TypeDataOffset");
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(MSB.ReambiguateName(this.Name), true);
        bw.Pad(4);
        bw.FillInt64("BaseDataOffset1", bw.Position - position);
        bw.WriteInt16((short) this.UnkA.Count);
        bw.WriteInt16s((IList<short>) this.UnkA);
        bw.Pad(4);
        bw.FillInt64("BaseDataOffset2", bw.Position - position);
        bw.WriteInt16((short) this.UnkB.Count);
        bw.WriteInt16s((IList<short>) this.UnkB);
        bw.Pad(8);
        if (this.Shape.HasShapeData)
        {
          bw.FillInt64("ShapeDataOffset", bw.Position - position);
          this.Shape.WriteShapeData(bw);
        }
        else
          bw.FillInt64("ShapeDataOffset", 0L);
        bw.FillInt64("BaseDataOffset3", bw.Position - position);
        bw.WriteInt32(this.ActivationPartIndex);
        bw.WriteInt32(this.EntityID);
        if (this.HasTypeData)
        {
          if (this.Type == MSBS.RegionType.Region23 || this.Type == MSBS.RegionType.PartsGroup || this.Type == MSBS.RegionType.AutoDrawGroup)
            bw.Pad(8);
          bw.FillInt64("TypeDataOffset", bw.Position - position);
          this.WriteTypeData(bw);
        }
        else
          bw.FillInt64("TypeDataOffset", 0L);
        bw.Pad(8);
      }

      internal virtual void WriteTypeData(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Type data should not be written for regions with no type data.");
      }

      internal virtual void GetNames(MSBS.Entries entries)
      {
        this.ActivationPartName = MSB.FindName<MSBS.Part>(entries.Parts, this.ActivationPartIndex);
        if (!(this.Shape is MSBS.Shape.Composite shape))
          return;
        foreach (MSBS.Shape.Composite.Child child in shape.Children)
          child.GetNames(entries);
      }

      internal virtual void GetIndices(MSBS.Entries entries)
      {
        this.ActivationPartIndex = MSB.FindIndex<MSBS.Part>(entries.Parts, this.ActivationPartName);
        if (!(this.Shape is MSBS.Shape.Composite shape))
          return;
        foreach (MSBS.Shape.Composite.Child child in shape.Children)
          child.GetIndices(entries);
      }

      public override string ToString()
      {
        return string.Format("{0} {1} {2}", (object) this.Type, (object) this.Shape.Type, (object) this.Name);
      }

      public class Region0 : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.Region0;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Region0()
        {
        }

        internal Region0(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class InvasionPoint : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.InvasionPoint;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public InvasionPoint()
        {
        }

        internal InvasionPoint(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
        }
      }

      public class EnvironmentMapPoint : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.EnvironmentMapPoint;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public float UnkT00 { get; set; }

        public int UnkT04 { get; set; }

        public int UnkT0C { get; set; }

        public float UnkT10 { get; set; }

        public float UnkT14 { get; set; }

        public int UnkT18 { get; set; }

        public int UnkT1C { get; set; }

        public int UnkT20 { get; set; }

        public int UnkT24 { get; set; }

        public int UnkT28 { get; set; }

        public EnvironmentMapPoint()
        {
        }

        internal EnvironmentMapPoint(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadSingle();
          this.UnkT04 = br.ReadInt32();
          br.AssertInt32(-1);
          this.UnkT0C = br.ReadInt32();
          this.UnkT10 = br.ReadSingle();
          this.UnkT14 = br.ReadSingle();
          this.UnkT18 = br.ReadInt32();
          this.UnkT1C = br.ReadInt32();
          this.UnkT20 = br.ReadInt32();
          this.UnkT24 = br.ReadInt32();
          this.UnkT28 = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertPattern(16, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteSingle(this.UnkT00);
          bw.WriteInt32(this.UnkT04);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.UnkT0C);
          bw.WriteSingle(this.UnkT10);
          bw.WriteSingle(this.UnkT14);
          bw.WriteInt32(this.UnkT18);
          bw.WriteInt32(this.UnkT1C);
          bw.WriteInt32(this.UnkT20);
          bw.WriteInt32(this.UnkT24);
          bw.WriteInt32(this.UnkT28);
          bw.WriteInt32(-1);
          bw.WritePattern(16, (byte) 0);
        }
      }

      public class Sound : MSBS.Region
      {
        private int[] ChildRegionIndices;

        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.Sound;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int SoundType { get; set; }

        public int SoundID { get; set; }

        public string[] ChildRegionNames { get; private set; }

        public int UnkT48 { get; set; }

        public Sound()
        {
          this.ChildRegionNames = new string[16];
        }

        internal Sound(BinaryReaderEx br)
          : base(br)
        {
          this.SoundType = br.ReadInt32();
          this.SoundID = br.ReadInt32();
          this.ChildRegionIndices = br.ReadInt32s(16);
          this.UnkT48 = br.ReadInt32();
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.SoundType);
          bw.WriteInt32(this.SoundID);
          bw.WriteInt32s((IList<int>) this.ChildRegionIndices);
          bw.WriteInt32(this.UnkT48);
        }

        internal override void GetNames(MSBS.Entries entries)
        {
          base.GetNames(entries);
          this.ChildRegionNames = MSB.FindNames<MSBS.Region>(entries.Regions, this.ChildRegionIndices);
        }

        internal override void GetIndices(MSBS.Entries entries)
        {
          base.GetIndices(entries);
          this.ChildRegionIndices = MSB.FindIndices<MSBS.Region>(entries.Regions, this.ChildRegionNames);
        }
      }

      public class SFX : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.SFX;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int FFXID { get; set; }

        public int UnkT04 { get; set; }

        public int StartDisabled { get; set; }

        public SFX()
        {
        }

        internal SFX(BinaryReaderEx br)
          : base(br)
        {
          this.FFXID = br.ReadInt32();
          this.UnkT04 = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.StartDisabled = br.ReadInt32();
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.FFXID);
          bw.WriteInt32(this.UnkT04);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.StartDisabled);
        }
      }

      public class WindSFX : MSBS.Region
      {
        public string WindAreaName;
        private int WindAreaIndex;

        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.WindSFX;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int FFXID { get; set; }

        public float UnkT18 { get; set; }

        public WindSFX()
        {
        }

        internal WindSFX(BinaryReaderEx br)
          : base(br)
        {
          this.FFXID = br.ReadInt32();
          br.AssertPattern(16, byte.MaxValue);
          this.WindAreaIndex = br.ReadInt32();
          this.UnkT18 = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.FFXID);
          bw.WritePattern(16, byte.MaxValue);
          bw.WriteInt32(this.WindAreaIndex);
          bw.WriteSingle(this.UnkT18);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSBS.Entries entries)
        {
          base.GetNames(entries);
          this.WindAreaName = MSB.FindName<MSBS.Region>(entries.Regions, this.WindAreaIndex);
        }

        internal override void GetIndices(MSBS.Entries entries)
        {
          base.GetIndices(entries);
          this.WindAreaIndex = MSB.FindIndex<MSBS.Region>(entries.Regions, this.WindAreaName);
        }
      }

      public class SpawnPoint : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.SpawnPoint;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public SpawnPoint()
        {
        }

        internal SpawnPoint(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(-1);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(-1);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class WalkRoute : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.WalkRoute;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public WalkRoute()
        {
        }

        internal WalkRoute(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class WarpPoint : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.WarpPoint;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public WarpPoint()
        {
        }

        internal WarpPoint(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class ActivationArea : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.ActivationArea;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public ActivationArea()
        {
        }

        internal ActivationArea(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class Event : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.Event;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Event()
        {
        }

        internal Event(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class EnvironmentMapEffectBox : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.EnvironmentMapEffectBox;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public float UnkT00 { get; set; }

        public float Compare { get; set; }

        public byte UnkT08 { get; set; }

        public byte UnkT09 { get; set; }

        public short UnkT0A { get; set; }

        public int UnkT24 { get; set; }

        public float UnkT28 { get; set; }

        public float UnkT2C { get; set; }

        public EnvironmentMapEffectBox()
        {
        }

        internal EnvironmentMapEffectBox(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadSingle();
          this.Compare = br.ReadSingle();
          this.UnkT08 = br.ReadByte();
          this.UnkT09 = br.ReadByte();
          this.UnkT0A = br.ReadInt16();
          br.AssertPattern(24, (byte) 0);
          this.UnkT24 = br.ReadInt32();
          this.UnkT28 = br.ReadSingle();
          this.UnkT2C = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteSingle(this.UnkT00);
          bw.WriteSingle(this.Compare);
          bw.WriteByte(this.UnkT08);
          bw.WriteByte(this.UnkT09);
          bw.WriteInt16(this.UnkT0A);
          bw.WritePattern(24, (byte) 0);
          bw.WriteInt32(this.UnkT24);
          bw.WriteSingle(this.UnkT28);
          bw.WriteSingle(this.UnkT2C);
          bw.WriteInt32(0);
        }
      }

      public class WindArea : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.WindArea;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public WindArea()
        {
        }

        internal WindArea(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class MufflingBox : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.MufflingBox;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public MufflingBox()
        {
        }

        internal MufflingBox(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
        }
      }

      public class MufflingPortal : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.MufflingPortal;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public int UnkT00 { get; set; }

        public MufflingPortal()
        {
        }

        internal MufflingPortal(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(0);
        }
      }

      public class Region23 : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.Region23;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public long UnkT00 { get; set; }

        public Region23()
        {
        }

        internal Region23(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt64();
          br.AssertPattern(24, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt64(this.UnkT00);
          bw.WritePattern(24, (byte) 0);
        }
      }

      public class Region24 : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.Region24;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Region24()
        {
        }

        internal Region24(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class PartsGroup : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.PartsGroup;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public long UnkT00 { get; set; }

        public PartsGroup()
        {
        }

        internal PartsGroup(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt64();
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt64(this.UnkT00);
        }
      }

      public class AutoDrawGroup : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.AutoDrawGroup;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return true;
          }
        }

        public long UnkT00 { get; set; }

        public AutoDrawGroup()
        {
        }

        internal AutoDrawGroup(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt64();
          br.AssertPattern(24, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw)
        {
          bw.WriteInt64(this.UnkT00);
          bw.WritePattern(24, (byte) 0);
        }
      }

      public class Other : MSBS.Region
      {
        public override MSBS.RegionType Type
        {
          get
          {
            return MSBS.RegionType.Other;
          }
        }

        internal override bool HasTypeData
        {
          get
          {
            return false;
          }
        }

        public Other()
        {
        }

        internal Other(BinaryReaderEx br)
          : base(br)
        {
        }
      }
    }

    public enum RouteType : uint
    {
      MufflingPortalLink = 3,
      MufflingBoxLink = 4,
    }

    public class RouteParam : MSBS.Param<MSBS.Route>
    {
      public List<MSBS.Route.MufflingPortalLink> MufflingPortalLinks { get; set; }

      public List<MSBS.Route.MufflingBoxLink> MufflingBoxLinks { get; set; }

      public RouteParam(int unk00 = 35)
        : base(unk00, "ROUTE_PARAM_ST")
      {
        this.MufflingPortalLinks = new List<MSBS.Route.MufflingPortalLink>();
        this.MufflingBoxLinks = new List<MSBS.Route.MufflingBoxLink>();
      }

      internal override MSBS.Route ReadEntry(BinaryReaderEx br)
      {
        MSBS.RouteType enum32 = br.GetEnum32<MSBS.RouteType>(br.Position + 16L);
        switch (enum32)
        {
          case MSBS.RouteType.MufflingPortalLink:
            MSBS.Route.MufflingPortalLink mufflingPortalLink = new MSBS.Route.MufflingPortalLink(br);
            this.MufflingPortalLinks.Add(mufflingPortalLink);
            return (MSBS.Route) mufflingPortalLink;
          case MSBS.RouteType.MufflingBoxLink:
            MSBS.Route.MufflingBoxLink mufflingBoxLink = new MSBS.Route.MufflingBoxLink(br);
            this.MufflingBoxLinks.Add(mufflingBoxLink);
            return (MSBS.Route) mufflingBoxLink;
          default:
            throw new NotImplementedException(string.Format("Unimplemented route type: {0}", (object) enum32));
        }
      }

      public override List<MSBS.Route> GetEntries()
      {
        return SFUtil.ConcatAll<MSBS.Route>(new IEnumerable<MSBS.Route>[2]
        {
          (IEnumerable<MSBS.Route>) this.MufflingPortalLinks,
          (IEnumerable<MSBS.Route>) this.MufflingBoxLinks
        });
      }
    }

    public abstract class Route : MSBS.Entry
    {
      public abstract MSBS.RouteType Type { get; }

      public int Unk08 { get; set; }

      public int Unk0C { get; set; }

      internal Route()
      {
        this.Name = "";
      }

      internal Route(BinaryReaderEx br)
      {
        long position = br.Position;
        long num1 = br.ReadInt64();
        this.Unk08 = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        br.AssertPattern(104, (byte) 0);
        this.Name = br.GetUTF16(position + num1);
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteInt32(this.Unk08);
        bw.WriteInt32(this.Unk0C);
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.WritePattern(104, (byte) 0);
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(this.Name, true);
        bw.Pad(8);
      }

      public override string ToString()
      {
        return string.Format("\"{0}\" {1} {2}", (object) this.Name, (object) this.Unk08, (object) this.Unk0C);
      }

      public class MufflingPortalLink : MSBS.Route
      {
        public override MSBS.RouteType Type
        {
          get
          {
            return MSBS.RouteType.MufflingPortalLink;
          }
        }

        public MufflingPortalLink()
        {
        }

        internal MufflingPortalLink(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class MufflingBoxLink : MSBS.Route
      {
        public override MSBS.RouteType Type
        {
          get
          {
            return MSBS.RouteType.MufflingBoxLink;
          }
        }

        public MufflingBoxLink()
        {
        }

        internal MufflingBoxLink(BinaryReaderEx br)
          : base(br)
        {
        }
      }
    }

    public enum ShapeType : uint
    {
      Point,
      Circle,
      Sphere,
      Cylinder,
      Rect,
      Box,
      Composite,
    }

    public abstract class Shape
    {
      internal abstract MSBS.ShapeType Type { get; }

      internal abstract bool HasShapeData { get; }

      internal virtual void WriteShapeData(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Shape data should not be written for shapes with no shape data.");
      }

      public class Point : MSBS.Shape
      {
        internal override MSBS.ShapeType Type
        {
          get
          {
            return MSBS.ShapeType.Point;
          }
        }

        internal override bool HasShapeData
        {
          get
          {
            return false;
          }
        }
      }

      public class Circle : MSBS.Shape
      {
        internal override MSBS.ShapeType Type
        {
          get
          {
            return MSBS.ShapeType.Circle;
          }
        }

        internal override bool HasShapeData
        {
          get
          {
            return true;
          }
        }

        public float Radius { get; set; }

        public Circle()
        {
        }

        internal Circle(BinaryReaderEx br)
        {
          this.Radius = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw)
        {
          bw.WriteSingle(this.Radius);
        }
      }

      public class Sphere : MSBS.Shape
      {
        internal override MSBS.ShapeType Type
        {
          get
          {
            return MSBS.ShapeType.Sphere;
          }
        }

        internal override bool HasShapeData
        {
          get
          {
            return true;
          }
        }

        public float Radius { get; set; }

        public Sphere()
        {
        }

        internal Sphere(BinaryReaderEx br)
        {
          this.Radius = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw)
        {
          bw.WriteSingle(this.Radius);
        }
      }

      public class Cylinder : MSBS.Shape
      {
        internal override MSBS.ShapeType Type
        {
          get
          {
            return MSBS.ShapeType.Cylinder;
          }
        }

        internal override bool HasShapeData
        {
          get
          {
            return true;
          }
        }

        public float Radius { get; set; }

        public float Height { get; set; }

        public Cylinder()
        {
        }

        internal Cylinder(BinaryReaderEx br)
        {
          this.Radius = br.ReadSingle();
          this.Height = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw)
        {
          bw.WriteSingle(this.Radius);
          bw.WriteSingle(this.Height);
        }
      }

      public class Rect : MSBS.Shape
      {
        internal override MSBS.ShapeType Type
        {
          get
          {
            return MSBS.ShapeType.Rect;
          }
        }

        internal override bool HasShapeData
        {
          get
          {
            return true;
          }
        }

        public float Width { get; set; }

        public float Depth { get; set; }

        public Rect()
        {
        }

        internal Rect(BinaryReaderEx br)
        {
          this.Width = br.ReadSingle();
          this.Depth = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw)
        {
          bw.WriteSingle(this.Width);
          bw.WriteSingle(this.Depth);
        }
      }

      public class Box : MSBS.Shape
      {
        internal override MSBS.ShapeType Type
        {
          get
          {
            return MSBS.ShapeType.Box;
          }
        }

        internal override bool HasShapeData
        {
          get
          {
            return true;
          }
        }

        public float Width { get; set; }

        public float Depth { get; set; }

        public float Height { get; set; }

        public Box()
        {
        }

        internal Box(BinaryReaderEx br)
        {
          this.Width = br.ReadSingle();
          this.Depth = br.ReadSingle();
          this.Height = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw)
        {
          bw.WriteSingle(this.Width);
          bw.WriteSingle(this.Depth);
          bw.WriteSingle(this.Height);
        }
      }

      public class Composite : MSBS.Shape
      {
        internal override MSBS.ShapeType Type
        {
          get
          {
            return MSBS.ShapeType.Composite;
          }
        }

        internal override bool HasShapeData
        {
          get
          {
            return true;
          }
        }

        public MSBS.Shape.Composite.Child[] Children { get; private set; }

        public Composite()
        {
          this.Children = new MSBS.Shape.Composite.Child[8];
          for (int index = 0; index < 8; ++index)
            this.Children[index] = new MSBS.Shape.Composite.Child();
        }

        internal Composite(BinaryReaderEx br)
        {
          this.Children = new MSBS.Shape.Composite.Child[8];
          for (int index = 0; index < 8; ++index)
            this.Children[index] = new MSBS.Shape.Composite.Child(br);
        }

        internal override void WriteShapeData(BinaryWriterEx bw)
        {
          for (int index = 0; index < 8; ++index)
            this.Children[index].Write(bw);
        }

        public class Child
        {
          private int RegionIndex;

          public string RegionName { get; set; }

          public int Unk04 { get; set; }

          public Child()
          {
          }

          internal Child(BinaryReaderEx br)
          {
            this.RegionIndex = br.ReadInt32();
            this.Unk04 = br.ReadInt32();
          }

          internal void Write(BinaryWriterEx bw)
          {
            bw.WriteInt32(this.RegionIndex);
            bw.WriteInt32(this.Unk04);
          }

          internal void GetNames(MSBS.Entries entries)
          {
            this.RegionName = MSB.FindName<MSBS.Region>(entries.Regions, this.RegionIndex);
          }

          internal void GetIndices(MSBS.Entries entries)
          {
            this.RegionIndex = MSB.FindIndex<MSBS.Region>(entries.Regions, this.RegionName);
          }
        }
      }
    }
  }
}
