// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MSB3
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

using FogMod.util.time;

namespace SoulsFormats {
  [ComVisible(true)]
  public class MSB3 : SoulsFile<MSB3>, IMsb {
    public MSB3.ModelParam Models { get; set; }

    IMsbParam<IMsbModel> IMsb.Models {
      get { return (IMsbParam<IMsbModel>) this.Models; }
    }

    public MSB3.EventParam Events { get; set; }

    public MSB3.PointParam Regions { get; set; }

    IMsbParam<IMsbRegion> IMsb.Regions {
      get { return (IMsbParam<IMsbRegion>) this.Regions; }
    }

    public MSB3.RouteParam Routes { get; set; }

    public MSB3.LayerParam Layers { get; set; }

    public MSB3.PartsParam Parts { get; set; }

    IMsbParam<IMsbPart> IMsb.Parts {
      get { return (IMsbParam<IMsbPart>) this.Parts; }
    }

    public MSB3.MapstudioPartsPose PartsPoses { get; set; }

    public MSB3.MapstudioBoneName BoneNames { get; set; }

    public MSB3() {
      this.Models = new MSB3.ModelParam(3);
      this.Events = new MSB3.EventParam(3);
      this.Regions = new MSB3.PointParam(3);
      this.Routes = new MSB3.RouteParam(3);
      this.Layers = new MSB3.LayerParam(3);
      this.Parts = new MSB3.PartsParam(3);
      this.PartsPoses = new MSB3.MapstudioPartsPose(0);
      this.BoneNames = new MSB3.MapstudioBoneName(0);
    }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "MSB ";
    }

    protected override void Read(BinaryReaderEx br) {
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      br.BigEndian = false;
      br.AssertASCII("MSB ");
      br.AssertInt32(1);
      br.AssertInt32(16);
      br.AssertBoolean(false);
      br.AssertBoolean(false);
      int num1 = (int) br.AssertByte((byte) 1);
      int num2 = (int) br.AssertByte(byte.MaxValue);
      stopwatch.ResetAndPrint("    Read msb3 0");

      MSB3.Entries entries = new MSB3.Entries();
      this.Models = new MSB3.ModelParam(3);
      entries.Models = this.Models.Read(br);
      stopwatch.ResetAndPrint("    Read msb3 models");

      this.Events = new MSB3.EventParam(3);
      entries.Events = this.Events.Read(br);
      stopwatch.ResetAndPrint("    Read msb3 events");

      this.Regions = new MSB3.PointParam(3);
      entries.Regions = this.Regions.Read(br);
      stopwatch.ResetAndPrint("    Read msb3 regions");

      this.Routes = new MSB3.RouteParam(3);
      entries.Routes = this.Routes.Read(br);
      stopwatch.ResetAndPrint("    Read msb3 routes");

      this.Layers = new MSB3.LayerParam(3);
      entries.Layers = this.Layers.Read(br);
      stopwatch.ResetAndPrint("    Read msb3 layers");

      this.Parts = new MSB3.PartsParam(3);
      entries.Parts = this.Parts.Read(br);
      stopwatch.ResetAndPrint("    Read msb3 parts");
      
      this.PartsPoses = new MSB3.MapstudioPartsPose(0);
      entries.PartsPoses = this.PartsPoses.Read(br);
      stopwatch.ResetAndPrint("    Read msb3 parts poses");

      this.BoneNames = new MSB3.MapstudioBoneName(0);
      entries.BoneNames = this.BoneNames.Read(br);
      stopwatch.ResetAndPrint("    Read msb3 bone names");

      if (br.Position != 0L)
        throw new InvalidDataException(
            "The next param offset of the final param should be 0, but it wasn't.");
      MSB.DisambiguateNames<MSB3.Model>(entries.Models);
      MSB.DisambiguateNames<MSB3.Part>(entries.Parts);
      MSB.DisambiguateNames<MSB3.Region>(entries.Regions);
      stopwatch.ResetAndPrint("    Read msb3 disambiguate names");

      foreach (MSB3.Event @event in entries.Events)
        @event.GetNames(this, entries);
      stopwatch.ResetAndPrint("    Read msb3 get event names");

      foreach (MSB3.Region region in entries.Regions)
        region.GetNames(this, entries);
      stopwatch.ResetAndPrint("    Read msb3 get region names");

      foreach (MSB3.Part part in entries.Parts)
        part.GetNames(this, entries);
      stopwatch.ResetAndPrint("    Read msb3 get part names");

      foreach (MSB3.PartsPose partsPose in entries.PartsPoses)
        partsPose.GetNames(this, entries);
      stopwatch.ResetAndPrint("    Read msb3 get parts pose names");
    }

    protected override void Write(BinaryWriterEx bw) {
      var stopwatch = new Stopwatch {EnableLogging = false};
      stopwatch.Start();

      bw.BigEndian = false;
      MSB3.Entries entries;
      entries.Models = this.Models.GetEntries();
      entries.Events = this.Events.GetEntries();
      entries.Regions = this.Regions.GetEntries();
      entries.Routes = this.Routes.GetEntries();
      entries.Layers = this.Layers.GetEntries();
      entries.Parts = this.Parts.GetEntries();
      entries.PartsPoses = this.PartsPoses.GetEntries();
      entries.BoneNames = this.BoneNames.GetEntries();

      var partCountByModelName = new ConcurrentDictionary<string, int>();
      foreach (var part in entries.Parts) {
        var modelName = part.ModelName;
        var partCount =
            partCountByModelName.GetOrAdd(modelName, _ => 0);
        partCountByModelName[modelName] = partCount + 1;
      }

      foreach (MSB3.Model model in entries.Models)
        model.CountInstances(partCountByModelName);
      stopwatch.ResetAndPrint("      Got model indices");

      foreach (MSB3.Event @event in entries.Events)
        @event.GetIndices(this, entries);
      stopwatch.ResetAndPrint("      Got event indices");

      foreach (MSB3.Region region in entries.Regions)
        region.GetIndices(this, entries);
      stopwatch.ResetAndPrint("      Got region indices");

      var models = entries.Models;
      var modelIndexByName = new Dictionary<string, int>();
      for (var modelIndex = 0; modelIndex < models.Count; ++modelIndex) {
        var model = models[modelIndex];
        var modelName = model.Name;

        if (!modelIndexByName.ContainsKey(modelName)) {
          modelIndexByName[modelName] = modelIndex;
        }
      }

      foreach (MSB3.Part part in entries.Parts) {
        part.GetIndices(this, entries, modelIndexByName);
      }
      stopwatch.ResetAndPrint("      Got part indices");

      foreach (MSB3.PartsPose partsPose in entries.PartsPoses)
        partsPose.GetIndices(this, entries);
      stopwatch.ResetAndPrint("      Got partPose indices");

      bw.WriteASCII("MSB ", false);
      bw.WriteInt32(1);
      bw.WriteInt32(16);
      bw.WriteBoolean(false);
      bw.WriteBoolean(false);
      bw.WriteByte((byte) 1);
      bw.WriteByte(byte.MaxValue);
      this.Models.Write(bw, entries.Models);
      stopwatch.ResetAndPrint("      Wrote models");

      bw.FillInt64("NextParamOffset", bw.Position);
      this.Events.Write(bw, entries.Events);
      stopwatch.ResetAndPrint("      Wrote events");

      bw.FillInt64("NextParamOffset", bw.Position);
      this.Regions.Write(bw, entries.Regions);
      stopwatch.ResetAndPrint("      Wrote regions");

      bw.FillInt64("NextParamOffset", bw.Position);
      this.Routes.Write(bw, entries.Routes);
      stopwatch.ResetAndPrint("      Wrote routes");

      bw.FillInt64("NextParamOffset", bw.Position);
      this.Layers.Write(bw, entries.Layers);
      stopwatch.ResetAndPrint("      Wrote layers");

      bw.FillInt64("NextParamOffset", bw.Position);
      this.Parts.Write(bw, entries.Parts);
      stopwatch.ResetAndPrint("      Wrote parts");

      bw.FillInt64("NextParamOffset", bw.Position);
      this.PartsPoses.Write(bw, entries.PartsPoses);
      stopwatch.ResetAndPrint("      Wrote partPoses");

      bw.FillInt64("NextParamOffset", bw.Position);
      this.BoneNames.Write(bw, entries.BoneNames);
      stopwatch.ResetAndPrint("      Wrote boneNames");

      bw.FillInt64("NextParamOffset", 0L);
    }

    public class EventParam : MSB3.Param<MSB3.Event> {
      public List<MSB3.Event.Treasure> Treasures;
      public List<MSB3.Event.Generator> Generators;
      public List<MSB3.Event.ObjAct> ObjActs;
      public List<MSB3.Event.MapOffset> MapOffsets;
      public List<MSB3.Event.PseudoMultiplayer> PseudoMultiplayers;
      public List<MSB3.Event.WalkRoute> WalkRoutes;
      public List<MSB3.Event.GroupTour> GroupTours;
      public List<MSB3.Event.Other> Others;

      internal override string Type {
        get { return "EVENT_PARAM_ST"; }
      }

      public EventParam(int unk1 = 3)
          : base(unk1) {
        this.Treasures = new List<MSB3.Event.Treasure>();
        this.Generators = new List<MSB3.Event.Generator>();
        this.ObjActs = new List<MSB3.Event.ObjAct>();
        this.MapOffsets = new List<MSB3.Event.MapOffset>();
        this.PseudoMultiplayers = new List<MSB3.Event.PseudoMultiplayer>();
        this.WalkRoutes = new List<MSB3.Event.WalkRoute>();
        this.GroupTours = new List<MSB3.Event.GroupTour>();
        this.Others = new List<MSB3.Event.Other>();
      }

      public override List<MSB3.Event> GetEntries() {
        return SFUtil.ConcatAll<MSB3.Event>(new IEnumerable<MSB3.Event>[8] {
            (IEnumerable<MSB3.Event>) this.Treasures,
            (IEnumerable<MSB3.Event>) this.Generators,
            (IEnumerable<MSB3.Event>) this.ObjActs,
            (IEnumerable<MSB3.Event>) this.MapOffsets,
            (IEnumerable<MSB3.Event>) this.PseudoMultiplayers,
            (IEnumerable<MSB3.Event>) this.WalkRoutes,
            (IEnumerable<MSB3.Event>) this.GroupTours,
            (IEnumerable<MSB3.Event>) this.Others
        });
      }

      internal override MSB3.Event ReadEntry(BinaryReaderEx br) {
        MSB3.EventType enum32 = br.GetEnum32<MSB3.EventType>(br.Position + 12L);
        switch (enum32) {
          case MSB3.EventType.Treasure:
            MSB3.Event.Treasure treasure = new MSB3.Event.Treasure(br);
            this.Treasures.Add(treasure);
            return (MSB3.Event) treasure;
          case MSB3.EventType.Generator:
            MSB3.Event.Generator generator = new MSB3.Event.Generator(br);
            this.Generators.Add(generator);
            return (MSB3.Event) generator;
          case MSB3.EventType.ObjAct:
            MSB3.Event.ObjAct objAct = new MSB3.Event.ObjAct(br);
            this.ObjActs.Add(objAct);
            return (MSB3.Event) objAct;
          case MSB3.EventType.MapOffset:
            MSB3.Event.MapOffset mapOffset = new MSB3.Event.MapOffset(br);
            this.MapOffsets.Add(mapOffset);
            return (MSB3.Event) mapOffset;
          case MSB3.EventType.PseudoMultiplayer:
            MSB3.Event.PseudoMultiplayer pseudoMultiplayer =
                new MSB3.Event.PseudoMultiplayer(br);
            this.PseudoMultiplayers.Add(pseudoMultiplayer);
            return (MSB3.Event) pseudoMultiplayer;
          case MSB3.EventType.WalkRoute:
            MSB3.Event.WalkRoute walkRoute = new MSB3.Event.WalkRoute(br);
            this.WalkRoutes.Add(walkRoute);
            return (MSB3.Event) walkRoute;
          case MSB3.EventType.GroupTour:
            MSB3.Event.GroupTour groupTour = new MSB3.Event.GroupTour(br);
            this.GroupTours.Add(groupTour);
            return (MSB3.Event) groupTour;
          case MSB3.EventType.Other:
            MSB3.Event.Other other = new MSB3.Event.Other(br);
            this.Others.Add(other);
            return (MSB3.Event) other;
          default:
            throw new NotImplementedException(
                string.Format("Unsupported event type: {0}", (object) enum32));
        }
      }

      internal override void WriteEntry(
          BinaryWriterEx bw,
          int id,
          MSB3.Event entry) {
        entry.Write(bw, id);
      }
    }

    internal enum EventType : uint {
      Light = 0,
      Sound = 1,
      SFX = 2,
      WindSFX = 3,
      Treasure = 4,
      Generator = 5,
      Message = 6,
      ObjAct = 7,
      SpawnPoint = 8,
      MapOffset = 9,
      Navimesh = 10,          // 0x0000000A
      Environment = 11,       // 0x0000000B
      PseudoMultiplayer = 12, // 0x0000000C
      Unk0D = 13,             // 0x0000000D
      WalkRoute = 14,         // 0x0000000E
      GroupTour = 15,         // 0x0000000F
      Unk10 = 16,             // 0x00000010
      Other = 4294967295,     // 0xFFFFFFFF
    }

    public abstract class Event : MSB3.Entry {
      public int EventID;
      public string PartName;
      private int PartIndex;
      public string PointName;
      private int PointIndex;
      public int EventEntityID;

      internal abstract MSB3.EventType Type { get; }

      public override string Name { get; set; }

      internal Event(string name) {
        this.Name = name;
        this.EventID = -1;
        this.EventEntityID = -1;
      }

      internal Event(MSB3.Event clone) {
        this.Name = clone.Name;
        this.EventID = clone.EventID;
        this.PartName = clone.PartName;
        this.PointName = clone.PointName;
        this.EventEntityID = clone.EventEntityID;
      }

      internal Event(BinaryReaderEx br) {
        long position = br.Position;
        long num1 = br.ReadInt64();
        this.EventID = br.ReadInt32();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        long num3 = br.ReadInt64();
        long num4 = br.ReadInt64();
        this.Name = br.GetUTF16(position + num1);
        br.Position = position + num3;
        this.PartIndex = br.ReadInt32();
        this.PointIndex = br.ReadInt32();
        this.EventEntityID = br.ReadInt32();
        br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        br.Position = position + num4;
        this.Read(br);
      }

      internal abstract void Read(BinaryReaderEx br);

      internal void Write(BinaryWriterEx bw, int id) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteInt32(this.EventID);
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
        bw.WriteInt32(this.PointIndex);
        bw.WriteInt32(this.EventEntityID);
        bw.WriteInt32(0);
        bw.FillInt64("TypeDataOffset", bw.Position - position);
        this.WriteSpecific(bw);
      }

      internal abstract void WriteSpecific(BinaryWriterEx bw);

      internal virtual void GetNames(MSB3 msb, MSB3.Entries entries) {
        this.PartName = MSB.FindName<MSB3.Part>(entries.Parts, this.PartIndex);
        this.PointName =
            MSB.FindName<MSB3.Region>(entries.Regions, this.PointIndex);
      }

      internal virtual void GetIndices(MSB3 msb, MSB3.Entries entries) {
        this.PartIndex = MSB.FindIndex<MSB3.Part>(entries.Parts, this.PartName);
        this.PointIndex =
            MSB.FindIndex<MSB3.Region>(entries.Regions, this.PointName);
      }

      public override string ToString() {
        return string.Format("{0} : {1}",
                             (object) this.Type,
                             (object) this.Name);
      }

      public class Treasure : MSB3.Event {
        public string PartName2;
        private int PartIndex2;
        public int ItemLot1;
        public int ItemLot2;
        public int ActionButtonParamID;
        public int PickupAnimID;
        public bool InChest;
        public bool StartDisabled;

        internal override MSB3.EventType Type {
          get { return MSB3.EventType.Treasure; }
        }

        public Treasure(string name)
            : base(name) {
          this.ItemLot1 = -1;
          this.ItemLot2 = -1;
          this.ActionButtonParamID = -1;
          this.PickupAnimID = -1;
        }

        public Treasure(MSB3.Event.Treasure clone)
            : base((MSB3.Event) clone) {
          this.PartName2 = clone.PartName2;
          this.ItemLot1 = clone.ItemLot1;
          this.ItemLot2 = clone.ItemLot2;
          this.ActionButtonParamID = clone.ActionButtonParamID;
          this.PickupAnimID = clone.PickupAnimID;
          this.InChest = clone.InChest;
          this.StartDisabled = clone.StartDisabled;
        }

        internal Treasure(BinaryReaderEx br)
            : base(br) {}

        internal override void Read(BinaryReaderEx br) {
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.PartIndex2 = br.ReadInt32();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.ItemLot1 = br.ReadInt32();
          this.ItemLot2 = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.ActionButtonParamID = br.ReadInt32();
          this.PickupAnimID = br.ReadInt32();
          this.InChest = br.ReadBoolean();
          this.StartDisabled = br.ReadBoolean();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.PartIndex2);
          bw.WriteInt32(0);
          bw.WriteInt32(this.ItemLot1);
          bw.WriteInt32(this.ItemLot2);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.ActionButtonParamID);
          bw.WriteInt32(this.PickupAnimID);
          bw.WriteBoolean(this.InChest);
          bw.WriteBoolean(this.StartDisabled);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.PartName2 =
              MSB.FindName<MSB3.Part>(entries.Parts, this.PartIndex2);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.PartIndex2 =
              MSB.FindIndex<MSB3.Part>(entries.Parts, this.PartName2);
        }
      }

      public class Generator : MSB3.Event {
        public short MaxNum;
        public short LimitNum;
        public short MinGenNum;
        public short MaxGenNum;
        public float MinInterval;
        public float MaxInterval;
        private int[] SpawnPointIndices;
        private int[] SpawnPartIndices;
        public int SessionCondition;
        public float UnkT14;
        public float UnkT18;

        internal override MSB3.EventType Type {
          get { return MSB3.EventType.Generator; }
        }

        public string[] SpawnPointNames { get; private set; }

        public string[] SpawnPartNames { get; private set; }

        public Generator(string name)
            : base(name) {
          this.SpawnPointNames = new string[8];
          this.SpawnPartNames = new string[32];
        }

        public Generator(MSB3.Event.Generator clone)
            : base((MSB3.Event) clone) {
          this.MaxNum = clone.MaxNum;
          this.LimitNum = clone.LimitNum;
          this.MinGenNum = clone.MinGenNum;
          this.MaxGenNum = clone.MaxGenNum;
          this.MinInterval = clone.MinInterval;
          this.MaxInterval = clone.MaxInterval;
          this.SessionCondition = clone.SessionCondition;
          this.UnkT14 = clone.UnkT14;
          this.UnkT18 = clone.UnkT18;
          this.SpawnPointNames = (string[]) clone.SpawnPointNames.Clone();
          this.SpawnPartNames = (string[]) clone.SpawnPartNames.Clone();
        }

        internal Generator(BinaryReaderEx br)
            : base(br) {}

        internal override void Read(BinaryReaderEx br) {
          this.MaxNum = br.ReadInt16();
          this.LimitNum = br.ReadInt16();
          this.MinGenNum = br.ReadInt16();
          this.MaxGenNum = br.ReadInt16();
          this.MinInterval = br.ReadSingle();
          this.MaxInterval = br.ReadSingle();
          this.SessionCondition = br.ReadInt32();
          this.UnkT14 = br.ReadSingle();
          this.UnkT18 = br.ReadSingle();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.SpawnPointIndices = br.ReadInt32s(8);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.SpawnPartIndices = br.ReadInt32s(32);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt16(this.MaxNum);
          bw.WriteInt16(this.LimitNum);
          bw.WriteInt16(this.MinGenNum);
          bw.WriteInt16(this.MaxGenNum);
          bw.WriteSingle(this.MinInterval);
          bw.WriteSingle(this.MaxInterval);
          bw.WriteInt32(this.SessionCondition);
          bw.WriteSingle(this.UnkT14);
          bw.WriteSingle(this.UnkT18);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32s((IList<int>) this.SpawnPointIndices);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32s((IList<int>) this.SpawnPartIndices);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.SpawnPointNames =
              MSB.FindNames<MSB3.Region>(entries.Regions,
                                         this.SpawnPointIndices);
          this.SpawnPartNames =
              MSB.FindNames<MSB3.Part>(entries.Parts, this.SpawnPartIndices);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.SpawnPointIndices =
              MSB.FindIndices<MSB3.Region>(entries.Regions,
                                           this.SpawnPointNames);
          this.SpawnPartIndices =
              MSB.FindIndices<MSB3.Part>(entries.Parts, this.SpawnPartNames);
        }
      }

      public class ObjAct : MSB3.Event {
        public int ObjActEntityID;
        public string PartName2;
        private int PartIndex2;
        public int ObjActParamID;
        public MSB3.Event.ObjAct.ObjActState ObjActStateType;
        public int EventFlagID;

        internal override MSB3.EventType Type {
          get { return MSB3.EventType.ObjAct; }
        }

        public ObjAct(string name)
            : base(name) {
          this.ObjActEntityID = -1;
          this.ObjActStateType = MSB3.Event.ObjAct.ObjActState.OneState;
        }

        public ObjAct(MSB3.Event.ObjAct clone)
            : base((MSB3.Event) clone) {
          this.ObjActEntityID = clone.ObjActEntityID;
          this.PartName2 = clone.PartName2;
          this.ObjActParamID = clone.ObjActParamID;
          this.ObjActStateType = clone.ObjActStateType;
          this.EventFlagID = clone.EventFlagID;
        }

        internal ObjAct(BinaryReaderEx br)
            : base(br) {}

        internal override void Read(BinaryReaderEx br) {
          this.ObjActEntityID = br.ReadInt32();
          this.PartIndex2 = br.ReadInt32();
          this.ObjActParamID = br.ReadInt32();
          this.ObjActStateType = br.ReadEnum8<MSB3.Event.ObjAct.ObjActState>();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertByte(new byte[1]);
          int num3 = (int) br.AssertByte(new byte[1]);
          this.EventFlagID = br.ReadInt32();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.ObjActEntityID);
          bw.WriteInt32(this.PartIndex2);
          bw.WriteInt32(this.ObjActParamID);
          bw.WriteByte((byte) this.ObjActStateType);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteByte((byte) 0);
          bw.WriteInt32(this.EventFlagID);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.PartName2 =
              MSB.FindName<MSB3.Part>(entries.Parts, this.PartIndex2);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.PartIndex2 =
              MSB.FindIndex<MSB3.Part>(entries.Parts, this.PartName2);
        }

        public enum ObjActState : byte {
          OneState,
          DoorState,
          OneLoopState,
          OneLoopState2,
          DoorState2,
        }
      }

      public class MapOffset : MSB3.Event {
        public Vector3 Position;
        public float Degree;

        internal override MSB3.EventType Type {
          get { return MSB3.EventType.MapOffset; }
        }

        public MapOffset(string name)
            : base(name) {
          this.Position = Vector3.Zero;
          this.Degree = 0.0f;
        }

        public MapOffset(MSB3.Event.MapOffset clone)
            : base((MSB3.Event) clone) {
          this.Position = clone.Position;
          this.Degree = clone.Degree;
        }

        internal MapOffset(BinaryReaderEx br)
            : base(br) {}

        internal override void Read(BinaryReaderEx br) {
          this.Position = br.ReadVector3();
          this.Degree = br.ReadSingle();
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteVector3(this.Position);
          bw.WriteSingle(this.Degree);
        }
      }

      public class PseudoMultiplayer : MSB3.Event {
        public int HostEventEntityID;
        public int InvasionEventEntityID;
        public int InvasionRegionIndex;
        public int SoundIDMaybe;
        public int MapEventIDMaybe;
        public int FlagsMaybe;
        public int UnkT18;

        internal override MSB3.EventType Type {
          get { return MSB3.EventType.PseudoMultiplayer; }
        }

        public PseudoMultiplayer(string name)
            : base(name) {
          this.HostEventEntityID = -1;
          this.InvasionEventEntityID = -1;
          this.InvasionRegionIndex = -1;
          this.SoundIDMaybe = -1;
          this.MapEventIDMaybe = -1;
        }

        public PseudoMultiplayer(MSB3.Event.PseudoMultiplayer clone)
            : base((MSB3.Event) clone) {
          this.HostEventEntityID = clone.HostEventEntityID;
          this.InvasionEventEntityID = clone.InvasionEventEntityID;
          this.InvasionRegionIndex = clone.InvasionRegionIndex;
          this.SoundIDMaybe = clone.SoundIDMaybe;
          this.MapEventIDMaybe = clone.MapEventIDMaybe;
          this.FlagsMaybe = clone.FlagsMaybe;
          this.UnkT18 = clone.UnkT18;
        }

        internal PseudoMultiplayer(BinaryReaderEx br)
            : base(br) {}

        internal override void Read(BinaryReaderEx br) {
          this.HostEventEntityID = br.ReadInt32();
          this.InvasionEventEntityID = br.ReadInt32();
          this.InvasionRegionIndex = br.ReadInt32();
          this.SoundIDMaybe = br.ReadInt32();
          this.MapEventIDMaybe = br.ReadInt32();
          this.FlagsMaybe = br.ReadInt32();
          this.UnkT18 = br.ReadInt32();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.HostEventEntityID);
          bw.WriteInt32(this.InvasionEventEntityID);
          bw.WriteInt32(this.InvasionRegionIndex);
          bw.WriteInt32(this.SoundIDMaybe);
          bw.WriteInt32(this.MapEventIDMaybe);
          bw.WriteInt32(this.FlagsMaybe);
          bw.WriteInt32(this.UnkT18);
          bw.WriteInt32(0);
        }
      }

      public class WalkRoute : MSB3.Event {
        public int UnkT00;
        private short[] WalkPointIndices;

        internal override MSB3.EventType Type {
          get { return MSB3.EventType.WalkRoute; }
        }

        public string[] WalkPointNames { get; private set; }

        public WalkRoute(string name)
            : base(name) {
          this.UnkT00 = 0;
          this.WalkPointNames = new string[32];
        }

        public WalkRoute(MSB3.Event.WalkRoute clone)
            : base((MSB3.Event) clone) {
          this.UnkT00 = clone.UnkT00;
          this.WalkPointNames = (string[]) clone.WalkPointNames.Clone();
        }

        internal WalkRoute(BinaryReaderEx br)
            : base(br) {}

        internal override void Read(BinaryReaderEx br) {
          this.UnkT00 = br.AssertInt32(0, 1, 2, 5);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.WalkPointIndices = br.ReadInt16s(32);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt16s((IList<short>) this.WalkPointIndices);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.WalkPointNames = new string[this.WalkPointIndices.Length];
          for (int index = 0; index < this.WalkPointIndices.Length; ++index)
            this.WalkPointNames[index] =
                MSB.FindName<MSB3.Region>(entries.Regions,
                                          (int) this.WalkPointIndices[index]);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.WalkPointIndices = new short[this.WalkPointNames.Length];
          for (int index = 0; index < this.WalkPointNames.Length; ++index)
            this.WalkPointIndices[index] =
                (short) MSB.FindIndex<MSB3.Region>(
                    entries.Regions,
                    this.WalkPointNames[index]);
        }
      }

      public class GroupTour : MSB3.Event {
        public int PlatoonIDScriptActivate;
        public int State;
        private int[] GroupPartsIndices;

        internal override MSB3.EventType Type {
          get { return MSB3.EventType.GroupTour; }
        }

        public string[] GroupPartsNames { get; private set; }

        public GroupTour(string name)
            : base(name) {
          this.GroupPartsNames = new string[32];
        }

        public GroupTour(MSB3.Event.GroupTour clone)
            : base((MSB3.Event) clone) {
          this.PlatoonIDScriptActivate = clone.PlatoonIDScriptActivate;
          this.State = clone.State;
          this.GroupPartsNames = (string[]) clone.GroupPartsNames.Clone();
        }

        internal GroupTour(BinaryReaderEx br)
            : base(br) {}

        internal override void Read(BinaryReaderEx br) {
          this.PlatoonIDScriptActivate = br.ReadInt32();
          this.State = br.ReadInt32();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.GroupPartsIndices = br.ReadInt32s(32);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.PlatoonIDScriptActivate);
          bw.WriteInt32(this.State);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32s((IList<int>) this.GroupPartsIndices);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.GroupPartsNames =
              MSB.FindNames<MSB3.Part>(entries.Parts, this.GroupPartsIndices);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.GroupPartsIndices =
              MSB.FindIndices<MSB3.Part>(entries.Parts, this.GroupPartsNames);
        }
      }

      public class Other : MSB3.Event {
        public int SoundTypeMaybe;
        public int SoundIDMaybe;

        internal override MSB3.EventType Type {
          get { return MSB3.EventType.Other; }
        }

        public Other(string name)
            : base(name) {
          this.SoundTypeMaybe = 0;
          this.SoundIDMaybe = 0;
        }

        public Other(MSB3.Event.Other clone)
            : base((MSB3.Event) clone) {
          this.SoundTypeMaybe = clone.SoundTypeMaybe;
          this.SoundIDMaybe = clone.SoundIDMaybe;
        }

        internal Other(BinaryReaderEx br)
            : base(br) {}

        internal override void Read(BinaryReaderEx br) {
          this.SoundTypeMaybe = br.ReadInt32();
          this.SoundIDMaybe = br.ReadInt32();
          for (int index = 0; index < 16; ++index)
            br.AssertInt32(-1);
        }

        internal override void WriteSpecific(BinaryWriterEx bw) {
          bw.WriteInt32(this.SoundTypeMaybe);
          bw.WriteInt32(this.SoundIDMaybe);
          for (int index = 0; index < 16; ++index)
            bw.WriteInt32(-1);
        }
      }
    }

    public class LayerParam : MSB3.Param<MSB3.Layer> {
      public List<MSB3.Layer> Layers;

      internal override string Type {
        get { return "LAYER_PARAM_ST"; }
      }

      public LayerParam(int unk1 = 3)
          : base(unk1) {
        this.Layers = new List<MSB3.Layer>();
      }

      public override List<MSB3.Layer> GetEntries() {
        return this.Layers;
      }

      internal override MSB3.Layer ReadEntry(BinaryReaderEx br) {
        MSB3.Layer layer = new MSB3.Layer(br);
        this.Layers.Add(layer);
        return layer;
      }

      internal override void WriteEntry(
          BinaryWriterEx bw,
          int index,
          MSB3.Layer entry) {
        entry.Write(bw);
      }
    }

    public class Layer {
      public string Name;
      public int Unk08;
      public int Unk0C;
      public int Unk10;

      public Layer() {
        this.Name = "";
      }

      internal Layer(BinaryReaderEx br) {
        long position = br.Position;
        long num = br.ReadInt64();
        this.Unk08 = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        this.Unk10 = br.ReadInt32();
        this.Name = br.GetUTF16(position + num);
      }

      internal void Write(BinaryWriterEx bw) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteInt32(this.Unk08);
        bw.WriteInt32(this.Unk0C);
        bw.WriteInt32(this.Unk10);
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(this.Name, true);
        bw.Pad(8);
      }

      public override string ToString() {
        return string.Format("{0} ({1}, {2}, {3})",
                             (object) this.Name,
                             (object) this.Unk08,
                             (object) this.Unk0C,
                             (object) this.Unk10);
      }
    }

    public class MapstudioBoneName : MSB3.Param<string> {
      public List<string> Names;

      internal override string Type {
        get { return "MAPSTUDIO_BONE_NAME_STRING"; }
      }

      public MapstudioBoneName(int unk1 = 0)
          : base(unk1) {
        this.Names = new List<string>();
      }

      public override List<string> GetEntries() {
        return this.Names;
      }

      internal override string ReadEntry(BinaryReaderEx br) {
        string str = br.ReadUTF16();
        this.Names.Add(str);
        return str;
      }

      internal override void WriteEntry(
          BinaryWriterEx bw,
          int id,
          string entry) {
        bw.WriteUTF16(entry, true);
        bw.Pad(8);
      }
    }

    public class MapstudioPartsPose : MSB3.Param<MSB3.PartsPose> {
      public List<MSB3.PartsPose> Poses;

      internal override string Type {
        get { return "MAPSTUDIO_PARTS_POSE_ST"; }
      }

      public MapstudioPartsPose(int unk1 = 0)
          : base(unk1) {
        this.Poses = new List<MSB3.PartsPose>();
      }

      public override List<MSB3.PartsPose> GetEntries() {
        return this.Poses;
      }

      internal override MSB3.PartsPose ReadEntry(BinaryReaderEx br) {
        MSB3.PartsPose partsPose = new MSB3.PartsPose(br);
        this.Poses.Add(partsPose);
        return partsPose;
      }

      internal override void WriteEntry(
          BinaryWriterEx bw,
          int index,
          MSB3.PartsPose entry) {
        entry.Write(bw);
      }
    }

    public class PartsPose {
      public string PartName;
      private short PartIndex;
      public List<MSB3.PartsPose.Bone> Bones;

      public PartsPose(string partName) {
        this.PartName = partName;
        this.Bones = new List<MSB3.PartsPose.Bone>();
      }

      internal PartsPose(BinaryReaderEx br) {
        this.PartIndex = br.ReadInt16();
        short num = br.ReadInt16();
        br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        br.AssertInt64(16L);
        this.Bones = new List<MSB3.PartsPose.Bone>((int) num);
        for (int index = 0; index < (int) num; ++index)
          this.Bones.Add(new MSB3.PartsPose.Bone(br));
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt16(this.PartIndex);
        bw.WriteInt16((short) this.Bones.Count);
        bw.WriteInt32(0);
        bw.WriteInt64(16L);
        foreach (MSB3.PartsPose.Bone bone in this.Bones)
          bone.Write(bw);
      }

      internal void GetNames(MSB3 msb, MSB3.Entries entries) {
        this.PartName =
            MSB.FindName<MSB3.Part>(entries.Parts, (int) this.PartIndex);
      }

      internal void GetIndices(MSB3 msb, MSB3.Entries entries) {
        this.PartIndex =
            (short) MSB.FindIndex<MSB3.Part>(entries.Parts, this.PartName);
      }

      public class Bone {
        public int BoneNamesIndex;
        public Vector3 Translation;
        public Vector3 Rotation;
        public Vector3 Scale;

        public Bone(int boneNamesIndex) {
          this.BoneNamesIndex = boneNamesIndex;
        }

        internal Bone(BinaryReaderEx br) {
          this.BoneNamesIndex = br.ReadInt32();
          this.Translation = br.ReadVector3();
          this.Rotation = br.ReadVector3();
          this.Scale = br.ReadVector3();
        }

        internal void Write(BinaryWriterEx bw) {
          bw.WriteInt32(this.BoneNamesIndex);
          bw.WriteVector3(this.Translation);
          bw.WriteVector3(this.Rotation);
          bw.WriteVector3(this.Scale);
        }

        public override string ToString() {
          return string.Format("{0} : {1} {2} {3}",
                               (object) this.BoneNamesIndex,
                               (object) this.Translation,
                               (object) this.Rotation,
                               (object) this.Scale);
        }
      }
    }

    public class ModelParam : MSB3.Param<MSB3.Model>, IMsbParam<IMsbModel> {
      public List<MSB3.Model.MapPiece> MapPieces;
      public List<MSB3.Model.Object> Objects;
      public List<MSB3.Model.Enemy> Enemies;
      public List<MSB3.Model.Player> Players;
      public List<MSB3.Model.Collision> Collisions;
      public List<MSB3.Model.Other> Others;

      internal override string Type {
        get { return "MODEL_PARAM_ST"; }
      }

      public ModelParam(int unk1 = 3)
          : base(unk1) {
        this.MapPieces = new List<MSB3.Model.MapPiece>();
        this.Objects = new List<MSB3.Model.Object>();
        this.Enemies = new List<MSB3.Model.Enemy>();
        this.Players = new List<MSB3.Model.Player>();
        this.Collisions = new List<MSB3.Model.Collision>();
        this.Others = new List<MSB3.Model.Other>();
      }

      public override List<MSB3.Model> GetEntries() {
        return SFUtil.ConcatAll<MSB3.Model>(new IEnumerable<MSB3.Model>[6] {
            (IEnumerable<MSB3.Model>) this.MapPieces,
            (IEnumerable<MSB3.Model>) this.Objects,
            (IEnumerable<MSB3.Model>) this.Enemies,
            (IEnumerable<MSB3.Model>) this.Players,
            (IEnumerable<MSB3.Model>) this.Collisions,
            (IEnumerable<MSB3.Model>) this.Others
        });
      }

      internal override MSB3.Model ReadEntry(BinaryReaderEx br) {
        MSB3.ModelType enum32 = br.GetEnum32<MSB3.ModelType>(br.Position + 8L);
        switch (enum32) {
          case MSB3.ModelType.MapPiece:
            MSB3.Model.MapPiece mapPiece = new MSB3.Model.MapPiece(br);
            this.MapPieces.Add(mapPiece);
            return (MSB3.Model) mapPiece;
          case MSB3.ModelType.Object:
            MSB3.Model.Object @object = new MSB3.Model.Object(br);
            this.Objects.Add(@object);
            return (MSB3.Model) @object;
          case MSB3.ModelType.Enemy:
            MSB3.Model.Enemy enemy = new MSB3.Model.Enemy(br);
            this.Enemies.Add(enemy);
            return (MSB3.Model) enemy;
          case MSB3.ModelType.Player:
            MSB3.Model.Player player = new MSB3.Model.Player(br);
            this.Players.Add(player);
            return (MSB3.Model) player;
          case MSB3.ModelType.Collision:
            MSB3.Model.Collision collision = new MSB3.Model.Collision(br);
            this.Collisions.Add(collision);
            return (MSB3.Model) collision;
          case MSB3.ModelType.Other:
            MSB3.Model.Other other = new MSB3.Model.Other(br);
            this.Others.Add(other);
            return (MSB3.Model) other;
          default:
            throw new NotImplementedException(
                string.Format("Unsupported model type: {0}", (object) enum32));
        }
      }

      internal override void WriteEntry(
          BinaryWriterEx bw,
          int id,
          MSB3.Model entry) {
        entry.Write(bw, id);
      }

      IReadOnlyList<IMsbModel> IMsbParam<IMsbModel>.GetEntries() {
        return (IReadOnlyList<IMsbModel>) this.GetEntries();
      }
    }

    internal enum ModelType : uint {
      MapPiece = 0,
      Object = 1,
      Enemy = 2,
      Item = 3,
      Player = 4,
      Collision = 5,
      Navmesh = 6,
      DummyObject = 7,
      DummyEnemy = 8,
      Other = 4294967295, // 0xFFFFFFFF
    }

    public abstract class Model : MSB3.Entry, IMsbModel, IMsbEntry {
      private int InstanceCount;

      internal abstract MSB3.ModelType Type { get; }

      internal abstract bool HasTypeData { get; }

      public override string Name { get; set; }

      public string Placeholder { get; set; }

      internal Model(string name) {
        this.Name = name;
        this.Placeholder = "";
      }

      internal Model(MSB3.Model clone) {
        this.Name = clone.Name;
        this.Placeholder = clone.Placeholder;
      }

      internal Model(BinaryReaderEx br) {
        long position = br.Position;
        long num1 = br.ReadInt64();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        long num3 = br.ReadInt64();
        this.InstanceCount = br.ReadInt32();
        br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        long num4 = br.ReadInt64();
        this.Name = br.GetUTF16(position + num1);
        this.Placeholder = br.GetUTF16(position + num3);
        br.Position = position + num4;
      }

      internal void Write(BinaryWriterEx bw, int id) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.ReserveInt64("PlaceholderOffset");
        bw.WriteInt32(this.InstanceCount);
        bw.WriteInt32(0);
        bw.ReserveInt64("TypeDataOffset");
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(MSB.ReambiguateName(this.Name), true);
        bw.FillInt64("PlaceholderOffset", bw.Position - position);
        bw.WriteUTF16(this.Placeholder, true);
        bw.Pad(8);
        if (this.HasTypeData) {
          bw.FillInt64("TypeDataOffset", bw.Position - position);
          this.WriteTypeData(bw);
        } else
          bw.FillInt64("TypeDataOffset", 0L);
      }

      internal virtual void WriteTypeData(BinaryWriterEx bw) {
        throw new InvalidOperationException(
            "Type data should not be written for models with no type data.");
      }

      internal void CountInstances(
          IDictionary<string, int> partCountByModelName) {
        this.InstanceCount =
            partCountByModelName.TryGetValue(this.Name, out var partCount)
                ? partCount
                : 0;
      }

      public override string ToString() {
        return string.Format("{0} : {1}",
                             (object) this.Type,
                             (object) this.Name);
      }

      public class MapPiece : MSB3.Model {
        public byte UnkT00;
        public byte UnkT01;
        public bool UnkT02;
        public bool UnkT03;

        internal override MSB3.ModelType Type {
          get { return MSB3.ModelType.MapPiece; }
        }

        internal override bool HasTypeData {
          get { return true; }
        }

        public MapPiece(string name)
            : base(name) {
          this.UnkT02 = true;
          this.UnkT03 = true;
        }

        public MapPiece(MSB3.Model.MapPiece clone)
            : base((MSB3.Model) clone) {
          this.UnkT00 = clone.UnkT00;
          this.UnkT01 = clone.UnkT01;
          this.UnkT02 = clone.UnkT02;
          this.UnkT03 = clone.UnkT03;
        }

        internal MapPiece(BinaryReaderEx br)
            : base(br) {
          this.UnkT00 = br.ReadByte();
          this.UnkT01 = br.ReadByte();
          this.UnkT02 = br.ReadBoolean();
          this.UnkT03 = br.ReadBoolean();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteByte(this.UnkT00);
          bw.WriteByte(this.UnkT01);
          bw.WriteBoolean(this.UnkT02);
          bw.WriteBoolean(this.UnkT03);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Object : MSB3.Model {
        public byte UnkT00;
        public byte UnkT01;
        public bool UnkT02;
        public bool UnkT03;

        internal override MSB3.ModelType Type {
          get { return MSB3.ModelType.Object; }
        }

        internal override bool HasTypeData {
          get { return true; }
        }

        public Object(string name)
            : base(name) {
          this.UnkT02 = true;
          this.UnkT03 = true;
        }

        public Object(MSB3.Model.Object clone)
            : base((MSB3.Model) clone) {
          this.UnkT00 = clone.UnkT00;
          this.UnkT01 = clone.UnkT01;
          this.UnkT02 = clone.UnkT02;
          this.UnkT03 = clone.UnkT03;
        }

        internal Object(BinaryReaderEx br)
            : base(br) {
          this.UnkT00 = br.ReadByte();
          this.UnkT01 = br.ReadByte();
          this.UnkT02 = br.ReadBoolean();
          this.UnkT03 = br.ReadBoolean();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteByte(this.UnkT00);
          bw.WriteByte(this.UnkT01);
          bw.WriteBoolean(this.UnkT02);
          bw.WriteBoolean(this.UnkT03);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Enemy : MSB3.Model {
        internal override MSB3.ModelType Type {
          get { return MSB3.ModelType.Enemy; }
        }

        internal override bool HasTypeData {
          get { return false; }
        }

        public Enemy(string name)
            : base(name) {}

        public Enemy(MSB3.Model.Enemy clone)
            : base((MSB3.Model) clone) {}

        internal Enemy(BinaryReaderEx br)
            : base(br) {}
      }

      public class Player : MSB3.Model {
        internal override MSB3.ModelType Type {
          get { return MSB3.ModelType.Player; }
        }

        internal override bool HasTypeData {
          get { return false; }
        }

        public Player(string name)
            : base(name) {}

        public Player(MSB3.Model.Player clone)
            : base((MSB3.Model) clone) {}

        internal Player(BinaryReaderEx br)
            : base(br) {}
      }

      public class Collision : MSB3.Model {
        internal override MSB3.ModelType Type {
          get { return MSB3.ModelType.Collision; }
        }

        internal override bool HasTypeData {
          get { return false; }
        }

        public Collision(string name)
            : base(name) {}

        public Collision(MSB3.Model.Collision clone)
            : base((MSB3.Model) clone) {}

        internal Collision(BinaryReaderEx br)
            : base(br) {}
      }

      public class Other : MSB3.Model {
        internal override MSB3.ModelType Type {
          get { return MSB3.ModelType.Other; }
        }

        internal override bool HasTypeData {
          get { return false; }
        }

        public Other(string name)
            : base(name) {}

        public Other(MSB3.Model.Other clone)
            : base((MSB3.Model) clone) {}

        internal Other(BinaryReaderEx br)
            : base(br) {}
      }
    }

    internal struct Entries {
      public List<MSB3.Model> Models;
      public List<MSB3.Event> Events;
      public List<MSB3.Region> Regions;
      public List<MSB3.Route> Routes;
      public List<MSB3.Layer> Layers;
      public List<MSB3.Part> Parts;
      public List<MSB3.PartsPose> PartsPoses;
      public List<string> BoneNames;
    }

    public abstract class Param<T> {
      public int Version { get; set; }

      internal abstract string Type { get; }

      internal Param(int version) {
        this.Version = version;
      }

      public abstract List<T> GetEntries();

      internal List<T> Read(BinaryReaderEx br) {
        this.Version = br.ReadInt32();
        int num1 = br.ReadInt32();
        long offset = br.ReadInt64();
        long[] numArray = br.ReadInt64s(num1 - 1);
        long num2 = br.ReadInt64();
        string utF16 = br.GetUTF16(offset);
        if (utF16 != this.Type)
          throw new InvalidDataException("Expected param \"" +
                                         this.Type +
                                         "\", got param \"" +
                                         utF16 +
                                         "\"");
        List<T> objList = new List<T>(num1 - 1);
        foreach (long num3 in numArray) {
          br.Position = num3;
          objList.Add(this.ReadEntry(br));
        }
        br.Position = num2;
        return objList;
      }

      internal abstract T ReadEntry(BinaryReaderEx br);

      internal void Write(BinaryWriterEx bw, List<T> entries) {
        bw.WriteInt32(this.Version);
        bw.WriteInt32(entries.Count + 1);
        bw.ReserveInt64("ParamNameOffset");
        for (int index = 0; index < entries.Count; ++index)
          bw.ReserveInt64(string.Format("EntryOffset{0}", (object) index));
        bw.ReserveInt64("NextParamOffset");
        bw.FillInt64("ParamNameOffset", bw.Position);
        bw.WriteUTF16(this.Type, true);
        bw.Pad(8);
        int id = 0;
        System.Type type = (System.Type) null;
        for (int index = 0; index < entries.Count; ++index) {
          if (type != entries[index].GetType()) {
            type = entries[index].GetType();
            id = 0;
          }
          bw.FillInt64(string.Format("EntryOffset{0}", (object) index),
                       bw.Position);
          this.WriteEntry(bw, id, entries[index]);
          ++id;
        }
      }

      internal abstract void WriteEntry(BinaryWriterEx bw, int id, T entry);

      public override string ToString() {
        return string.Format("{0}:{1}[{2}]",
                             (object) this.Type,
                             (object) this.Version,
                             (object) this.GetEntries().Count);
      }
    }

    public abstract class Entry : IMsbEntry {
      public abstract string Name { get; set; }
    }

    public class PartsParam : MSB3.Param<MSB3.Part>, IMsbParam<IMsbPart> {
      public List<MSB3.Part.MapPiece> MapPieces;
      public List<MSB3.Part.Object> Objects;
      public List<MSB3.Part.Enemy> Enemies;
      public List<MSB3.Part.Player> Players;
      public List<MSB3.Part.Collision> Collisions;
      public List<MSB3.Part.DummyObject> DummyObjects;
      public List<MSB3.Part.DummyEnemy> DummyEnemies;
      public List<MSB3.Part.ConnectCollision> ConnectCollisions;

      internal override string Type {
        get { return "PARTS_PARAM_ST"; }
      }

      public PartsParam(int unk1 = 3)
          : base(unk1) {
        this.MapPieces = new List<MSB3.Part.MapPiece>();
        this.Objects = new List<MSB3.Part.Object>();
        this.Enemies = new List<MSB3.Part.Enemy>();
        this.Players = new List<MSB3.Part.Player>();
        this.Collisions = new List<MSB3.Part.Collision>();
        this.DummyObjects = new List<MSB3.Part.DummyObject>();
        this.DummyEnemies = new List<MSB3.Part.DummyEnemy>();
        this.ConnectCollisions = new List<MSB3.Part.ConnectCollision>();
      }

      public override List<MSB3.Part> GetEntries() {
        return SFUtil.ConcatAll<MSB3.Part>(new IEnumerable<MSB3.Part>[8] {
            (IEnumerable<MSB3.Part>) this.MapPieces,
            (IEnumerable<MSB3.Part>) this.Objects,
            (IEnumerable<MSB3.Part>) this.Enemies,
            (IEnumerable<MSB3.Part>) this.Players,
            (IEnumerable<MSB3.Part>) this.Collisions,
            (IEnumerable<MSB3.Part>) this.DummyObjects,
            (IEnumerable<MSB3.Part>) this.DummyEnemies,
            (IEnumerable<MSB3.Part>) this.ConnectCollisions
        });
      }

      IReadOnlyList<IMsbPart> IMsbParam<IMsbPart>.GetEntries() {
        return (IReadOnlyList<IMsbPart>) this.GetEntries();
      }

      internal override MSB3.Part ReadEntry(BinaryReaderEx br) {
        var stopwatch = new Stopwatch {EnableLogging = false};
        stopwatch.Start();
       
        MSB3.PartsType enum32 = br.GetEnum32<MSB3.PartsType>(br.Position + 8L);
        switch (enum32) {
          case MSB3.PartsType.MapPiece:
            MSB3.Part.MapPiece mapPiece = new MSB3.Part.MapPiece(br);
            this.MapPieces.Add(mapPiece);
            stopwatch.ResetAndPrint("      Read map piece");

            return (MSB3.Part) mapPiece;
          case MSB3.PartsType.Object:
            MSB3.Part.Object @object = new MSB3.Part.Object(br);
            this.Objects.Add(@object);
            stopwatch.ResetAndPrint("      Read object");

            return (MSB3.Part) @object;
          case MSB3.PartsType.Enemy:
            MSB3.Part.Enemy enemy = new MSB3.Part.Enemy(br);
            this.Enemies.Add(enemy);
            stopwatch.ResetAndPrint("      Read enemy");
            
            return (MSB3.Part) enemy;
          case MSB3.PartsType.Player:
            MSB3.Part.Player player = new MSB3.Part.Player(br);
            this.Players.Add(player);
            stopwatch.ResetAndPrint("      Read player");

            return (MSB3.Part) player;
          case MSB3.PartsType.Collision:
            MSB3.Part.Collision collision = new MSB3.Part.Collision(br);
            this.Collisions.Add(collision);
            stopwatch.ResetAndPrint("      Read collision");

            return (MSB3.Part) collision;
          case MSB3.PartsType.DummyObject:
            MSB3.Part.DummyObject dummyObject = new MSB3.Part.DummyObject(br);
            this.DummyObjects.Add(dummyObject);
            stopwatch.ResetAndPrint("      Read dummy object");

            return (MSB3.Part) dummyObject;
          case MSB3.PartsType.DummyEnemy:
            MSB3.Part.DummyEnemy dummyEnemy = new MSB3.Part.DummyEnemy(br);
            this.DummyEnemies.Add(dummyEnemy);
            stopwatch.ResetAndPrint("      Read dummy enemy");

            return (MSB3.Part) dummyEnemy;
          case MSB3.PartsType.ConnectCollision:
            MSB3.Part.ConnectCollision connectCollision =
                new MSB3.Part.ConnectCollision(br);
            this.ConnectCollisions.Add(connectCollision);
            stopwatch.ResetAndPrint("      Read connect collision");

            return (MSB3.Part) connectCollision;
          default:
            throw new NotImplementedException(
                string.Format("Unsupported part type: {0}", (object) enum32));
        }
      }

      internal override void WriteEntry(
          BinaryWriterEx bw,
          int id,
          MSB3.Part entry) {
        entry.Write(bw, id);
      }
    }

    internal enum PartsType : uint {
      MapPiece,
      Object,
      Enemy,
      Item,
      Player,
      Collision,
      NPCWander,
      Protoboss,
      Navmesh,
      DummyObject,
      DummyEnemy,
      ConnectCollision,
    }

    public abstract class Part : MSB3.Entry, IMsbPart, IMsbEntry {
      public string Placeholder;
      private int modelIndex;
      public uint MapStudioLayer;
      public int EventEntityID;
      public sbyte OldLightID;
      public sbyte OldFogID;
      public sbyte OldScatterID;
      public sbyte OldLensFlareID;
      public sbyte LanternID;
      public sbyte LodParamID;
      public sbyte UnkB0E;
      public bool PointLightShadowSource;
      public bool ShadowSource;
      public bool ShadowDest;
      public bool IsShadowOnly;
      public bool DrawByReflectCam;
      public bool DrawOnlyReflectCam;
      public bool UseDepthBiasFloat;
      public bool DisablePointLightEffect;
      public bool UnkB17;
      public int UnkB18;

      internal abstract MSB3.PartsType Type { get; }

      internal abstract bool HasGparamConfig { get; }

      internal abstract bool HasUnk4 { get; }

      public override string Name { get; set; }

      public string ModelName { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      public Vector3 Scale { get; set; }

      public uint[] DrawGroups { get; private set; }

      public uint[] DispGroups { get; private set; }

      public uint[] BackreadGroups { get; private set; }

      public int[] EventEntityGroups { get; private set; }

      internal Part(string name) {
        this.Name = name;
        this.Scale = Vector3.One;
        this.DrawGroups = new uint[8] {
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue
        };
        this.DispGroups = new uint[8] {
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue
        };
        this.BackreadGroups = new uint[8] {
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue
        };
        this.EventEntityID = -1;
        this.EventEntityGroups = new int[8] {
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1
        };
      }

      internal Part(MSB3.Part clone) {
        this.Name = clone.Name;
        this.Placeholder = clone.Placeholder;
        this.ModelName = clone.ModelName;
        this.Position = clone.Position;
        this.Rotation = clone.Rotation;
        this.Scale = clone.Scale;
        this.MapStudioLayer = clone.MapStudioLayer;
        this.DrawGroups = (uint[]) clone.DrawGroups.Clone();
        this.DispGroups = (uint[]) clone.DispGroups.Clone();
        this.BackreadGroups = (uint[]) clone.BackreadGroups.Clone();
        this.EventEntityID = clone.EventEntityID;
        this.OldLightID = clone.OldLightID;
        this.OldFogID = clone.OldFogID;
        this.OldScatterID = clone.OldScatterID;
        this.OldLensFlareID = clone.OldLensFlareID;
        this.LanternID = clone.LanternID;
        this.LodParamID = clone.LodParamID;
        this.UnkB0E = clone.UnkB0E;
        this.PointLightShadowSource = clone.PointLightShadowSource;
        this.ShadowSource = clone.ShadowSource;
        this.ShadowDest = clone.ShadowDest;
        this.IsShadowOnly = clone.IsShadowOnly;
        this.DrawByReflectCam = clone.DrawByReflectCam;
        this.DrawOnlyReflectCam = clone.DrawOnlyReflectCam;
        this.UseDepthBiasFloat = clone.UseDepthBiasFloat;
        this.DisablePointLightEffect = clone.DisablePointLightEffect;
        this.UnkB17 = clone.UnkB17;
        this.UnkB18 = clone.UnkB18;
        this.EventEntityGroups = (int[]) clone.EventEntityGroups.Clone();
      }

      internal Part(BinaryReaderEx br) {
        long position = br.Position;
        long num1 = br.ReadInt64();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        this.modelIndex = br.ReadInt32();
        br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        long num3 = br.ReadInt64();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        br.AssertInt32(-1);
        this.MapStudioLayer = br.ReadUInt32();
        this.DrawGroups = br.ReadUInt32s(8);
        this.DispGroups = br.ReadUInt32s(8);
        this.BackreadGroups = br.ReadUInt32s(8);
        br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        long num4 = br.ReadInt64();
        long num5 = br.ReadInt64();
        long num6 = br.ReadInt64();
        long num7 = br.ReadInt64();
        this.Name = br.GetUTF16(position + num1);
        this.Placeholder = br.GetUTF16(position + num3);
        br.Position = position + num4;
        this.EventEntityID = br.ReadInt32();
        this.OldLightID = br.ReadSByte();
        this.OldFogID = br.ReadSByte();
        this.OldScatterID = br.ReadSByte();
        this.OldLensFlareID = br.ReadSByte();
        br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        this.LanternID = br.ReadSByte();
        this.LodParamID = br.ReadSByte();
        this.UnkB0E = br.ReadSByte();
        this.PointLightShadowSource = br.ReadBoolean();
        this.ShadowSource = br.ReadBoolean();
        this.ShadowDest = br.ReadBoolean();
        this.IsShadowOnly = br.ReadBoolean();
        this.DrawByReflectCam = br.ReadBoolean();
        this.DrawOnlyReflectCam = br.ReadBoolean();
        this.UseDepthBiasFloat = br.ReadBoolean();
        this.DisablePointLightEffect = br.ReadBoolean();
        this.UnkB17 = br.ReadBoolean();
        this.UnkB18 = br.ReadInt32();
        this.EventEntityGroups = br.ReadInt32s(8);
        br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        br.Position = position + num5;
        this.ReadTypeData(br);
        if (this.HasGparamConfig) {
          br.Position = position + num6;
          this.ReadGparamConfig(br);
        }
        if (!this.HasUnk4)
          return;
        br.Position = position + num7;
        this.ReadUnk4(br);
      }

      internal abstract void ReadTypeData(BinaryReaderEx br);

      internal virtual void ReadGparamConfig(BinaryReaderEx br) {
        throw new InvalidOperationException(
            "Gparam config should not be read for parts with no gparam config.");
      }

      internal virtual void ReadUnk4(BinaryReaderEx br) {
        throw new InvalidOperationException(
            "Unk struct 4 should not be read for parts with no unk struct 4.");
      }

      internal void Write(BinaryWriterEx bw, int id) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.WriteInt32(this.modelIndex);
        bw.WriteInt32(0);
        bw.ReserveInt64("PlaceholderOffset");
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.WriteVector3(this.Scale);
        bw.WriteInt32(-1);
        bw.WriteUInt32(this.MapStudioLayer);
        bw.WriteUInt32s((IList<uint>) this.DrawGroups);
        bw.WriteUInt32s((IList<uint>) this.DispGroups);
        bw.WriteUInt32s((IList<uint>) this.BackreadGroups);
        bw.WriteInt32(0);
        bw.ReserveInt64("BaseDataOffset");
        bw.ReserveInt64("TypeDataOffset");
        bw.ReserveInt64("GparamOffset");
        bw.ReserveInt64("UnkOffset4");
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(MSB.ReambiguateName(this.Name), true);
        bw.FillInt64("PlaceholderOffset", bw.Position - position);
        bw.WriteUTF16(this.Placeholder, true);
        if (this.Placeholder == "")
          bw.WritePattern(36, (byte) 0);
        bw.Pad(8);
        bw.FillInt64("BaseDataOffset", bw.Position - position);
        bw.WriteInt32(this.EventEntityID);
        bw.WriteSByte(this.OldLightID);
        bw.WriteSByte(this.OldFogID);
        bw.WriteSByte(this.OldScatterID);
        bw.WriteSByte(this.OldLensFlareID);
        bw.WriteInt32(0);
        bw.WriteSByte(this.LanternID);
        bw.WriteSByte(this.LodParamID);
        bw.WriteSByte(this.UnkB0E);
        bw.WriteBoolean(this.PointLightShadowSource);
        bw.WriteBoolean(this.ShadowSource);
        bw.WriteBoolean(this.ShadowDest);
        bw.WriteBoolean(this.IsShadowOnly);
        bw.WriteBoolean(this.DrawByReflectCam);
        bw.WriteBoolean(this.DrawOnlyReflectCam);
        bw.WriteBoolean(this.UseDepthBiasFloat);
        bw.WriteBoolean(this.DisablePointLightEffect);
        bw.WriteBoolean(this.UnkB17);
        bw.WriteInt32(this.UnkB18);
        bw.WriteInt32s((IList<int>) this.EventEntityGroups);
        bw.WriteInt32(0);
        bw.Pad(8);
        bw.FillInt64("TypeDataOffset", bw.Position - position);
        this.WriteTypeData(bw);
        if (this.HasGparamConfig) {
          bw.FillInt64("GparamOffset", bw.Position - position);
          this.WriteGparamConfig(bw);
        } else
          bw.FillInt64("GparamOffset", 0L);
        if (this.HasUnk4) {
          bw.FillInt64("UnkOffset4", bw.Position - position);
          this.WriteUnk4(bw);
        } else
          bw.FillInt64("UnkOffset4", 0L);
      }

      internal abstract void WriteTypeData(BinaryWriterEx bw);

      internal virtual void WriteGparamConfig(BinaryWriterEx bw) {
        throw new InvalidOperationException(
            "Gparam config should not be written for parts with no gparam config.");
      }

      internal virtual void WriteUnk4(BinaryWriterEx bw) {
        throw new InvalidOperationException(
            "Unk struct 4 should not be written for parts with no unk struct 4.");
      }

      internal virtual void GetNames(MSB3 msb, MSB3.Entries entries) {
        this.ModelName =
            MSB.FindName<MSB3.Model>(entries.Models, this.modelIndex);
      }

      internal virtual void GetIndices(MSB3 msb, MSB3.Entries entries) {}

      internal void GetIndices(
          MSB3 msb, 
          MSB3.Entries entries,
          IDictionary<string, int> modelIndexByName) {
        this.modelIndex = this.FindModelIndex(modelIndexByName);
        this.GetIndices(msb, entries);
      }

      private int FindModelIndex(IDictionary<string, int> modelIndexByName) {
        var modelName = this.ModelName;
        if (modelName == null) {
          return -1;
        }

        if (modelIndexByName.TryGetValue(modelName, out var index)) {
          return index;
        }

        throw new KeyNotFoundException("Name not found: " + modelName);
      }

      public override string ToString() {
        return string.Format("{0} : {1}",
                             (object) this.Type,
                             (object) this.Name);
      }

      public class GparamConfig {
        public int LightSetID { get; set; }

        public int FogParamID { get; set; }

        public int LightScatteringID { get; set; }

        public int EnvMapID { get; set; }

        public GparamConfig() {}

        public GparamConfig(MSB3.Part.GparamConfig clone) {
          this.LightSetID = clone.LightSetID;
          this.FogParamID = clone.FogParamID;
          this.LightScatteringID = clone.LightScatteringID;
          this.EnvMapID = clone.EnvMapID;
        }

        internal GparamConfig(BinaryReaderEx br) {
          this.LightSetID = br.ReadInt32();
          this.FogParamID = br.ReadInt32();
          this.LightScatteringID = br.ReadInt32();
          this.EnvMapID = br.ReadInt32();
          br.AssertPattern(16, (byte) 0);
        }

        internal void Write(BinaryWriterEx bw) {
          bw.WriteInt32(this.LightSetID);
          bw.WriteInt32(this.FogParamID);
          bw.WriteInt32(this.LightScatteringID);
          bw.WriteInt32(this.EnvMapID);
          bw.WritePattern(16, (byte) 0);
        }

        public override string ToString() {
          return string.Format("{0}, {1}, {2}, {3}",
                               (object) this.LightSetID,
                               (object) this.FogParamID,
                               (object) this.LightScatteringID,
                               (object) this.EnvMapID);
        }
      }

      public class UnkStruct4 {
        public int Unk3C { get; set; }

        public float Unk40 { get; set; }

        public UnkStruct4() {}

        public UnkStruct4(MSB3.Part.UnkStruct4 clone) {
          this.Unk3C = clone.Unk3C;
          this.Unk40 = clone.Unk40;
        }

        internal UnkStruct4(BinaryReaderEx br) {
          br.AssertPattern(60, (byte) 0);
          this.Unk3C = br.ReadInt32();
          this.Unk40 = br.ReadSingle();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal void Write(BinaryWriterEx bw) {
          bw.WritePattern(60, (byte) 0);
          bw.WriteInt32(this.Unk3C);
          bw.WriteSingle(this.Unk40);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class MapPiece : MSB3.Part {
        public MSB3.Part.GparamConfig Gparam;

        internal override MSB3.PartsType Type {
          get { return MSB3.PartsType.MapPiece; }
        }

        internal override bool HasGparamConfig {
          get { return true; }
        }

        internal override bool HasUnk4 {
          get { return false; }
        }

        public MapPiece(string name)
            : base(name) {
          this.Gparam = new MSB3.Part.GparamConfig();
        }

        public MapPiece(MSB3.Part.MapPiece clone)
            : base((MSB3.Part) clone) {
          this.Gparam = new MSB3.Part.GparamConfig(clone.Gparam);
        }

        internal MapPiece(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void ReadGparamConfig(BinaryReaderEx br) {
          this.Gparam = new MSB3.Part.GparamConfig(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void WriteGparamConfig(BinaryWriterEx bw) {
          this.Gparam.Write(bw);
        }
      }

      internal static readonly int[] SHARED_SINGLE = new int[1];

      public class Object : MSB3.Part {
        private int CollisionPartIndex;

        internal override MSB3.PartsType Type {
          get { return MSB3.PartsType.Object; }
        }

        internal override bool HasGparamConfig {
          get { return true; }
        }

        internal override bool HasUnk4 {
          get { return false; }
        }

        public MSB3.Part.GparamConfig Gparam { get; set; }

        public string CollisionName { get; set; }

        public byte UnkT0C { get; set; }

        public bool EnableObjAnimNetSyncStructure { get; set; }

        public bool CollisionFilter { get; set; }

        public bool SetMainObjStructureBooleans { get; set; }

        public short[] AnimIDs { get; private set; }

        public short[] ModelSfxParamRelativeIDs { get; private set; }

        public Object(string name)
            : base(name) {
          this.Gparam = new MSB3.Part.GparamConfig();
          this.AnimIDs = new short[4] {
              (short) -1,
              (short) -1,
              (short) -1,
              (short) -1
          };
          this.ModelSfxParamRelativeIDs = new short[4] {
              (short) -1,
              (short) -1,
              (short) -1,
              (short) -1
          };
        }

        public Object(MSB3.Part.Object clone)
            : base((MSB3.Part) clone) {
          this.Gparam = new MSB3.Part.GparamConfig(clone.Gparam);
          this.CollisionName = clone.CollisionName;
          this.UnkT0C = clone.UnkT0C;
          this.EnableObjAnimNetSyncStructure =
              clone.EnableObjAnimNetSyncStructure;
          this.CollisionFilter = clone.CollisionFilter;
          this.SetMainObjStructureBooleans = clone.SetMainObjStructureBooleans;
          this.AnimIDs = (short[]) clone.AnimIDs.Clone();
          this.ModelSfxParamRelativeIDs =
              (short[]) clone.ModelSfxParamRelativeIDs.Clone();
        }

        internal Object(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.CollisionPartIndex = br.ReadInt32();
          this.UnkT0C = br.ReadByte();
          this.EnableObjAnimNetSyncStructure = br.ReadBoolean();
          this.CollisionFilter = br.ReadBoolean();
          this.SetMainObjStructureBooleans = br.ReadBoolean();
          this.AnimIDs = br.ReadInt16s(4);
          this.ModelSfxParamRelativeIDs = br.ReadInt16s(4);
        }

        internal override void ReadGparamConfig(BinaryReaderEx br) {
          this.Gparam = new MSB3.Part.GparamConfig(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.CollisionPartIndex);
          bw.WriteByte(this.UnkT0C);
          bw.WriteBoolean(this.EnableObjAnimNetSyncStructure);
          bw.WriteBoolean(this.CollisionFilter);
          bw.WriteBoolean(this.SetMainObjStructureBooleans);
          bw.WriteInt16s((IList<short>) this.AnimIDs);
          bw.WriteInt16s((IList<short>) this.ModelSfxParamRelativeIDs);
        }

        internal override void WriteGparamConfig(BinaryWriterEx bw) {
          this.Gparam.Write(bw);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.CollisionName =
              MSB.FindName<MSB3.Part>(entries.Parts, this.CollisionPartIndex);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.CollisionPartIndex =
              MSB.FindIndex<MSB3.Part>(entries.Parts, this.CollisionName);
        }
      }

      public class Enemy : MSB3.Part {
        public MSB3.Part.GparamConfig Gparam;
        public string CollisionName;
        private int CollisionPartIndex;
        public int ThinkParamID;
        public int NPCParamID;
        public int TalkID;
        public int CharaInitID;
        public short UnkT04;
        public short ChrManipulatorAllocationParameter;
        public string WalkRouteName;
        private short WalkRouteIndex;
        public int BackupEventAnimID;
        public int UnkT78;
        public float UnkT84;

        internal override MSB3.PartsType Type {
          get { return MSB3.PartsType.Enemy; }
        }

        internal override bool HasGparamConfig {
          get { return true; }
        }

        internal override bool HasUnk4 {
          get { return false; }
        }

        public Enemy(string name)
            : base(name) {
          this.Gparam = new MSB3.Part.GparamConfig();
        }

        public Enemy(MSB3.Part.Enemy clone)
            : base((MSB3.Part) clone) {
          this.Gparam = new MSB3.Part.GparamConfig(clone.Gparam);
          this.ThinkParamID = clone.ThinkParamID;
          this.NPCParamID = clone.NPCParamID;
          this.TalkID = clone.TalkID;
          this.UnkT04 = clone.UnkT04;
          this.ChrManipulatorAllocationParameter =
              clone.ChrManipulatorAllocationParameter;
          this.CharaInitID = clone.CharaInitID;
          this.CollisionName = clone.CollisionName;
          this.WalkRouteName = clone.WalkRouteName;
          this.BackupEventAnimID = clone.BackupEventAnimID;
          this.UnkT78 = clone.UnkT78;
          this.UnkT84 = clone.UnkT84;
        }

        internal Enemy(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.ThinkParamID = br.ReadInt32();
          this.NPCParamID = br.ReadInt32();
          this.TalkID = br.ReadInt32();
          this.UnkT04 = br.ReadInt16();
          this.ChrManipulatorAllocationParameter = br.ReadInt16();
          this.CharaInitID = br.ReadInt32();
          this.CollisionPartIndex = br.ReadInt32();
          this.WalkRouteIndex = br.ReadInt16();
          int num1 = (int) br.AssertInt16(new short[1]);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.BackupEventAnimID = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.UnkT78 = br.ReadInt32();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          this.UnkT84 = br.ReadSingle();
          for (int index = 0; index < 5; ++index) {
            br.AssertInt32(-1);
            int num2 = (int) br.AssertInt16((short) -1);
            int num3 = (int) br.AssertInt16((short) 10);
          }
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void ReadGparamConfig(BinaryReaderEx br) {
          this.Gparam = new MSB3.Part.GparamConfig(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.ThinkParamID);
          bw.WriteInt32(this.NPCParamID);
          bw.WriteInt32(this.TalkID);
          bw.WriteInt16(this.UnkT04);
          bw.WriteInt16(this.ChrManipulatorAllocationParameter);
          bw.WriteInt32(this.CharaInitID);
          bw.WriteInt32(this.CollisionPartIndex);
          bw.WriteInt16(this.WalkRouteIndex);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.BackupEventAnimID);
          bw.WriteInt32(-1);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.UnkT78);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteSingle(this.UnkT84);
          for (int index = 0; index < 5; ++index) {
            bw.WriteInt32(-1);
            bw.WriteInt16((short) -1);
            bw.WriteInt16((short) 10);
          }
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void WriteGparamConfig(BinaryWriterEx bw) {
          this.Gparam.Write(bw);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.CollisionName =
              MSB.FindName<MSB3.Part>(entries.Parts, this.CollisionPartIndex);
          this.WalkRouteName =
              MSB.FindName<MSB3.Event.WalkRoute>(
                  msb.Events.WalkRoutes,
                  (int) this.WalkRouteIndex);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.CollisionPartIndex =
              MSB.FindIndex<MSB3.Part>(entries.Parts, this.CollisionName);
          this.WalkRouteIndex =
              (short) MSB.FindIndex<MSB3.Event.WalkRoute>(
                  msb.Events.WalkRoutes,
                  this.WalkRouteName);
        }
      }

      public class Player : MSB3.Part {
        internal override MSB3.PartsType Type {
          get { return MSB3.PartsType.Player; }
        }

        internal override bool HasGparamConfig {
          get { return false; }
        }

        internal override bool HasUnk4 {
          get { return false; }
        }

        public Player(string name)
            : base(name) {}

        public Player(MSB3.Part.Player clone)
            : base((MSB3.Part) clone) {}

        internal Player(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Collision : MSB3.Part {
        public MSB3.Part.GparamConfig Gparam;
        public MSB3.Part.UnkStruct4 Unk4;
        public byte HitFilterID;
        public MSB3.Part.Collision.SoundSpace SoundSpaceType;
        public short EnvLightMapSpotIndex;
        public float ReflectPlaneHeight;
        public short MapNameID;
        public bool DisableStart;
        public int DisableBonfireEntityID;
        public int PlayRegionID;
        public short LockCamID1;
        public short LockCamID2;
        public string UnkHitName;
        private int UnkHitIndex;
        public int ChameleonParamID;
        public byte UnkT34;
        public byte UnkT35;
        public byte UnkT36;
        public MSB3.Part.Collision.MapVisiblity MapVisType;

        internal override MSB3.PartsType Type {
          get { return MSB3.PartsType.Collision; }
        }

        internal override bool HasGparamConfig {
          get { return true; }
        }

        internal override bool HasUnk4 {
          get { return true; }
        }

        public Collision(string name)
            : base(name) {
          this.Gparam = new MSB3.Part.GparamConfig();
          this.Unk4 = new MSB3.Part.UnkStruct4();
          this.SoundSpaceType = MSB3.Part.Collision.SoundSpace.NoReverb;
          this.MapNameID = (short) -1;
          this.DisableStart = false;
          this.DisableBonfireEntityID = -1;
          this.MapVisType = MSB3.Part.Collision.MapVisiblity.Good;
          this.PlayRegionID = -1;
        }

        public Collision(MSB3.Part.Collision clone)
            : base((MSB3.Part) clone) {
          this.Gparam = new MSB3.Part.GparamConfig(clone.Gparam);
          this.Unk4 = new MSB3.Part.UnkStruct4(clone.Unk4);
          this.HitFilterID = clone.HitFilterID;
          this.SoundSpaceType = clone.SoundSpaceType;
          this.EnvLightMapSpotIndex = clone.EnvLightMapSpotIndex;
          this.ReflectPlaneHeight = clone.ReflectPlaneHeight;
          this.MapNameID = clone.MapNameID;
          this.DisableStart = clone.DisableStart;
          this.DisableBonfireEntityID = clone.DisableBonfireEntityID;
          this.ChameleonParamID = clone.ChameleonParamID;
          this.UnkHitName = clone.UnkHitName;
          this.UnkT34 = clone.UnkT34;
          this.UnkT35 = clone.UnkT35;
          this.UnkT36 = clone.UnkT36;
          this.MapVisType = clone.MapVisType;
          this.PlayRegionID = clone.PlayRegionID;
          this.LockCamID1 = clone.LockCamID1;
          this.LockCamID2 = clone.LockCamID2;
        }

        internal Collision(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.HitFilterID = br.ReadByte();
          this.SoundSpaceType = br.ReadEnum8<MSB3.Part.Collision.SoundSpace>();
          this.EnvLightMapSpotIndex = br.ReadInt16();
          this.ReflectPlaneHeight = br.ReadSingle();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.MapNameID = br.ReadInt16();
          this.DisableStart = br.AssertInt16((short) 0, (short) 1) == (short) 1;
          this.DisableBonfireEntityID = br.ReadInt32();
          this.ChameleonParamID = br.ReadInt32();
          this.UnkHitIndex = br.ReadInt32();
          this.UnkT34 = br.ReadByte();
          this.UnkT35 = br.ReadByte();
          this.UnkT36 = br.ReadByte();
          this.MapVisType = br.ReadEnum8<MSB3.Part.Collision.MapVisiblity>();
          this.PlayRegionID = br.ReadInt32();
          this.LockCamID1 = br.ReadInt16();
          this.LockCamID2 = br.ReadInt16();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void ReadGparamConfig(BinaryReaderEx br) {
          this.Gparam = new MSB3.Part.GparamConfig(br);
        }

        internal override void ReadUnk4(BinaryReaderEx br) {
          this.Unk4 = new MSB3.Part.UnkStruct4(br);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteByte(this.HitFilterID);
          bw.WriteByte((byte) this.SoundSpaceType);
          bw.WriteInt16(this.EnvLightMapSpotIndex);
          bw.WriteSingle(this.ReflectPlaneHeight);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt16(this.MapNameID);
          bw.WriteInt16(this.DisableStart ? (short) 1 : (short) 0);
          bw.WriteInt32(this.DisableBonfireEntityID);
          bw.WriteInt32(this.ChameleonParamID);
          bw.WriteInt32(this.UnkHitIndex);
          bw.WriteByte(this.UnkT34);
          bw.WriteByte(this.UnkT35);
          bw.WriteByte(this.UnkT36);
          bw.WriteByte((byte) this.MapVisType);
          bw.WriteInt32(this.PlayRegionID);
          bw.WriteInt16(this.LockCamID1);
          bw.WriteInt16(this.LockCamID2);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void WriteGparamConfig(BinaryWriterEx bw) {
          this.Gparam.Write(bw);
        }

        internal override void WriteUnk4(BinaryWriterEx bw) {
          this.Unk4.Write(bw);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.UnkHitName =
              MSB.FindName<MSB3.Part>(entries.Parts, this.UnkHitIndex);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.UnkHitIndex =
              MSB.FindIndex<MSB3.Part>(entries.Parts, this.UnkHitName);
        }

        public enum SoundSpace : byte {
          NoReverb,
          SmallReverbA,
          SmallReverbB,
          MiddleReverbA,
          MiddleReverbB,
          LargeReverbA,
          LargeReverbB,
          ExtraLargeReverbA,
          ExtraLargeReverbB,
        }

        public enum MapVisiblity : byte {
          Good,
          Dark,
          PitchDark,
        }
      }

      public class DummyObject : MSB3.Part.Object {
        internal override MSB3.PartsType Type {
          get { return MSB3.PartsType.DummyObject; }
        }

        public DummyObject(string name)
            : base(name) {}

        public DummyObject(MSB3.Part.DummyObject clone)
            : base((MSB3.Part.Object) clone) {}

        internal DummyObject(BinaryReaderEx br)
            : base(br) {}
      }

      public class DummyEnemy : MSB3.Part.Enemy {
        internal override MSB3.PartsType Type {
          get { return MSB3.PartsType.DummyEnemy; }
        }

        public DummyEnemy(string name)
            : base(name) {}

        public DummyEnemy(MSB3.Part.DummyEnemy clone)
            : base((MSB3.Part.Enemy) clone) {}

        internal DummyEnemy(BinaryReaderEx br)
            : base(br) {}
      }

      public class ConnectCollision : MSB3.Part {
        public string CollisionName;
        private int CollisionIndex;
        public byte MapID1;
        public byte MapID2;
        public byte MapID3;
        public byte MapID4;

        internal override MSB3.PartsType Type {
          get { return MSB3.PartsType.ConnectCollision; }
        }

        internal override bool HasGparamConfig {
          get { return false; }
        }

        internal override bool HasUnk4 {
          get { return false; }
        }

        public ConnectCollision(string name)
            : base(name) {}

        public ConnectCollision(MSB3.Part.ConnectCollision clone)
            : base((MSB3.Part) clone) {
          this.CollisionName = clone.CollisionName;
          this.MapID1 = clone.MapID1;
          this.MapID2 = clone.MapID2;
          this.MapID3 = clone.MapID3;
          this.MapID4 = clone.MapID4;
        }

        internal ConnectCollision(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.CollisionIndex = br.ReadInt32();
          this.MapID1 = br.ReadByte();
          this.MapID2 = br.ReadByte();
          this.MapID3 = br.ReadByte();
          this.MapID4 = br.ReadByte();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.CollisionIndex);
          bw.WriteByte(this.MapID1);
          bw.WriteByte(this.MapID2);
          bw.WriteByte(this.MapID3);
          bw.WriteByte(this.MapID4);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.CollisionName =
              MSB.FindName<MSB3.Part.Collision>(msb.Parts.Collisions,
                                                this.CollisionIndex);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.CollisionIndex =
              MSB.FindIndex<MSB3.Part.Collision>(
                  msb.Parts.Collisions,
                  this.CollisionName);
        }
      }
    }

    public class PointParam : MSB3.Param<MSB3.Region>, IMsbParam<IMsbRegion> {
      public List<MSB3.Region.General> General;
      public List<MSB3.Region.Unk00> Unk00s;
      public List<MSB3.Region.InvasionPoint> InvasionPoints;
      public List<MSB3.Region.EnvironmentMapPoint> EnvironmentMapPoints;
      public List<MSB3.Region.Sound> Sounds;
      public List<MSB3.Region.SFX> SFX;
      public List<MSB3.Region.WindSFX> WindSFX;
      public List<MSB3.Region.SpawnPoint> SpawnPoints;
      public List<MSB3.Region.Message> Messages;
      public List<MSB3.Region.WalkRoute> WalkRoutes;
      public List<MSB3.Region.Unk12> Unk12s;
      public List<MSB3.Region.WarpPoint> WarpPoints;
      public List<MSB3.Region.ActivationArea> ActivationAreas;
      public List<MSB3.Region.Event> Events;

      public List<MSB3.Region.EnvironmentMapEffectBox>
          EnvironmentMapEffectBoxes;

      public List<MSB3.Region.WindArea> WindAreas;
      public List<MSB3.Region.MufflingBox> MufflingBoxes;
      public List<MSB3.Region.MufflingPortal> MufflingPortals;

      internal override string Type {
        get { return "POINT_PARAM_ST"; }
      }

      public PointParam(int unk1 = 3)
          : base(unk1) {
        this.General = new List<MSB3.Region.General>();
        this.Unk00s = new List<MSB3.Region.Unk00>();
        this.InvasionPoints = new List<MSB3.Region.InvasionPoint>();
        this.EnvironmentMapPoints = new List<MSB3.Region.EnvironmentMapPoint>();
        this.Sounds = new List<MSB3.Region.Sound>();
        this.SFX = new List<MSB3.Region.SFX>();
        this.WindSFX = new List<MSB3.Region.WindSFX>();
        this.SpawnPoints = new List<MSB3.Region.SpawnPoint>();
        this.Messages = new List<MSB3.Region.Message>();
        this.WalkRoutes = new List<MSB3.Region.WalkRoute>();
        this.Unk12s = new List<MSB3.Region.Unk12>();
        this.WarpPoints = new List<MSB3.Region.WarpPoint>();
        this.ActivationAreas = new List<MSB3.Region.ActivationArea>();
        this.Events = new List<MSB3.Region.Event>();
        this.EnvironmentMapEffectBoxes =
            new List<MSB3.Region.EnvironmentMapEffectBox>();
        this.WindAreas = new List<MSB3.Region.WindArea>();
        this.MufflingBoxes = new List<MSB3.Region.MufflingBox>();
        this.MufflingPortals = new List<MSB3.Region.MufflingPortal>();
      }

      public override List<MSB3.Region> GetEntries() {
        return SFUtil.ConcatAll<MSB3.Region>(new IEnumerable<MSB3.Region>[18] {
            (IEnumerable<MSB3.Region>) this.InvasionPoints,
            (IEnumerable<MSB3.Region>) this.EnvironmentMapPoints,
            (IEnumerable<MSB3.Region>) this.Sounds,
            (IEnumerable<MSB3.Region>) this.SFX,
            (IEnumerable<MSB3.Region>) this.WindSFX,
            (IEnumerable<MSB3.Region>) this.SpawnPoints,
            (IEnumerable<MSB3.Region>) this.Messages,
            (IEnumerable<MSB3.Region>) this.WalkRoutes,
            (IEnumerable<MSB3.Region>) this.Unk12s,
            (IEnumerable<MSB3.Region>) this.WarpPoints,
            (IEnumerable<MSB3.Region>) this.ActivationAreas,
            (IEnumerable<MSB3.Region>) this.Events,
            (IEnumerable<MSB3.Region>) this.Unk00s,
            (IEnumerable<MSB3.Region>) this.EnvironmentMapEffectBoxes,
            (IEnumerable<MSB3.Region>) this.WindAreas,
            (IEnumerable<MSB3.Region>) this.MufflingBoxes,
            (IEnumerable<MSB3.Region>) this.MufflingPortals,
            (IEnumerable<MSB3.Region>) this.General
        });
      }

      IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries() {
        return (IReadOnlyList<IMsbRegion>) this.GetEntries();
      }

      internal override MSB3.Region ReadEntry(BinaryReaderEx br) {
        MSB3.RegionType enum32 =
            br.GetEnum32<MSB3.RegionType>(br.Position + 8L);
        switch (enum32) {
          case MSB3.RegionType.Unk00:
            MSB3.Region.Unk00 unk00 = new MSB3.Region.Unk00(br);
            this.Unk00s.Add(unk00);
            return (MSB3.Region) unk00;
          case MSB3.RegionType.InvasionPoint:
            MSB3.Region.InvasionPoint invasionPoint =
                new MSB3.Region.InvasionPoint(br);
            this.InvasionPoints.Add(invasionPoint);
            return (MSB3.Region) invasionPoint;
          case MSB3.RegionType.EnvironmentMapPoint:
            MSB3.Region.EnvironmentMapPoint environmentMapPoint =
                new MSB3.Region.EnvironmentMapPoint(br);
            this.EnvironmentMapPoints.Add(environmentMapPoint);
            return (MSB3.Region) environmentMapPoint;
          case MSB3.RegionType.Sound:
            MSB3.Region.Sound sound = new MSB3.Region.Sound(br);
            this.Sounds.Add(sound);
            return (MSB3.Region) sound;
          case MSB3.RegionType.SFX:
            MSB3.Region.SFX sfx = new MSB3.Region.SFX(br);
            this.SFX.Add(sfx);
            return (MSB3.Region) sfx;
          case MSB3.RegionType.WindSFX:
            MSB3.Region.WindSFX windSfx = new MSB3.Region.WindSFX(br);
            this.WindSFX.Add(windSfx);
            return (MSB3.Region) windSfx;
          case MSB3.RegionType.SpawnPoint:
            MSB3.Region.SpawnPoint spawnPoint = new MSB3.Region.SpawnPoint(br);
            this.SpawnPoints.Add(spawnPoint);
            return (MSB3.Region) spawnPoint;
          case MSB3.RegionType.Message:
            MSB3.Region.Message message = new MSB3.Region.Message(br);
            this.Messages.Add(message);
            return (MSB3.Region) message;
          case MSB3.RegionType.WalkRoute:
            MSB3.Region.WalkRoute walkRoute = new MSB3.Region.WalkRoute(br);
            this.WalkRoutes.Add(walkRoute);
            return (MSB3.Region) walkRoute;
          case MSB3.RegionType.Unk12:
            MSB3.Region.Unk12 unk12 = new MSB3.Region.Unk12(br);
            this.Unk12s.Add(unk12);
            return (MSB3.Region) unk12;
          case MSB3.RegionType.WarpPoint:
            MSB3.Region.WarpPoint warpPoint = new MSB3.Region.WarpPoint(br);
            this.WarpPoints.Add(warpPoint);
            return (MSB3.Region) warpPoint;
          case MSB3.RegionType.ActivationArea:
            MSB3.Region.ActivationArea activationArea =
                new MSB3.Region.ActivationArea(br);
            this.ActivationAreas.Add(activationArea);
            return (MSB3.Region) activationArea;
          case MSB3.RegionType.Event:
            MSB3.Region.Event @event = new MSB3.Region.Event(br);
            this.Events.Add(@event);
            return (MSB3.Region) @event;
          case MSB3.RegionType.EnvironmentMapEffectBox:
            MSB3.Region.EnvironmentMapEffectBox environmentMapEffectBox =
                new MSB3.Region.EnvironmentMapEffectBox(br);
            this.EnvironmentMapEffectBoxes.Add(environmentMapEffectBox);
            return (MSB3.Region) environmentMapEffectBox;
          case MSB3.RegionType.WindArea:
            MSB3.Region.WindArea windArea = new MSB3.Region.WindArea(br);
            this.WindAreas.Add(windArea);
            return (MSB3.Region) windArea;
          case MSB3.RegionType.MufflingBox:
            MSB3.Region.MufflingBox mufflingBox =
                new MSB3.Region.MufflingBox(br);
            this.MufflingBoxes.Add(mufflingBox);
            return (MSB3.Region) mufflingBox;
          case MSB3.RegionType.MufflingPortal:
            MSB3.Region.MufflingPortal mufflingPortal =
                new MSB3.Region.MufflingPortal(br);
            this.MufflingPortals.Add(mufflingPortal);
            return (MSB3.Region) mufflingPortal;
          case MSB3.RegionType.General:
            MSB3.Region.General general = new MSB3.Region.General(br);
            this.General.Add(general);
            return (MSB3.Region) general;
          default:
            throw new NotImplementedException(
                string.Format("Unsupported region type: {0}", (object) enum32));
        }
      }

      internal override void WriteEntry(
          BinaryWriterEx bw,
          int id,
          MSB3.Region entry) {
        entry.Write(bw, id);
      }
    }

    internal enum RegionType : uint {
      Unk00 = 0,
      InvasionPoint = 1,
      EnvironmentMapPoint = 2,
      Sound = 4,
      SFX = 5,
      WindSFX = 6,
      SpawnPoint = 8,
      Message = 9,
      WalkRoute = 11,               // 0x0000000B
      Unk12 = 12,                   // 0x0000000C
      WarpPoint = 13,               // 0x0000000D
      ActivationArea = 14,          // 0x0000000E
      Event = 15,                   // 0x0000000F
      EnvironmentMapEffectBox = 17, // 0x00000011
      WindArea = 18,                // 0x00000012
      MufflingBox = 20,             // 0x00000014
      MufflingPortal = 21,          // 0x00000015
      General = 4294967295,         // 0xFFFFFFFF
    }

    public abstract class Region : MSB3.Entry, IMsbRegion, IMsbEntry {
      public bool HasTypeData;
      public int Unk2;
      public MSB3.Shape Shape;
      public uint MapStudioLayer;
      public List<short> UnkA;
      public List<short> UnkB;
      public string ActivationPartName;
      private int ActivationPartIndex;
      public int EventEntityID;

      internal abstract MSB3.RegionType Type { get; }

      public override string Name { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      internal Region(string name, bool hasTypeData) {
        this.Name = name;
        this.Position = Vector3.Zero;
        this.Rotation = Vector3.Zero;
        this.Shape = (MSB3.Shape) new MSB3.Shape.Point();
        this.ActivationPartName = (string) null;
        this.EventEntityID = -1;
        this.UnkA = new List<short>();
        this.UnkB = new List<short>();
        this.HasTypeData = hasTypeData;
      }

      internal Region(MSB3.Region clone) {
        this.Name = clone.Name;
        this.Position = clone.Position;
        this.Rotation = clone.Rotation;
        this.Shape = clone.Shape.Clone();
        this.ActivationPartName = clone.ActivationPartName;
        this.EventEntityID = clone.EventEntityID;
        this.Unk2 = clone.Unk2;
        this.UnkA = new List<short>((IEnumerable<short>) clone.UnkA);
        this.UnkB = new List<short>((IEnumerable<short>) clone.UnkB);
        this.MapStudioLayer = clone.MapStudioLayer;
        this.HasTypeData = clone.HasTypeData;
      }

      internal Region(BinaryReaderEx br) {
        long position = br.Position;
        long num1 = br.ReadInt64();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        MSB3.ShapeType shapeType = br.ReadEnum32<MSB3.ShapeType>();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Unk2 = br.ReadInt32();
        long num3 = br.ReadInt64();
        long num4 = br.AssertInt64(num3 + 4L);
        br.AssertInt32(-1);
        this.MapStudioLayer = br.ReadUInt32();
        long num5 = br.ReadInt64();
        long num6 = br.ReadInt64();
        long num7 = br.ReadInt64();
        this.Name = br.GetUTF16(position + num1);
        br.Position = position + num3;
        short num8 = br.ReadInt16();
        this.UnkA =
            new List<short>((IEnumerable<short>) br.ReadInt16s((int) num8));
        br.Position = position + num4;
        short num9 = br.ReadInt16();
        this.UnkB =
            new List<short>((IEnumerable<short>) br.ReadInt16s((int) num9));
        br.Position = position + num5;
        switch (shapeType) {
          case MSB3.ShapeType.Point:
            this.Shape = (MSB3.Shape) new MSB3.Shape.Point();
            break;
          case MSB3.ShapeType.Circle:
            this.Shape = (MSB3.Shape) new MSB3.Shape.Circle(br);
            break;
          case MSB3.ShapeType.Sphere:
            this.Shape = (MSB3.Shape) new MSB3.Shape.Sphere(br);
            break;
          case MSB3.ShapeType.Cylinder:
            this.Shape = (MSB3.Shape) new MSB3.Shape.Cylinder(br);
            break;
          case MSB3.ShapeType.Box:
            this.Shape = (MSB3.Shape) new MSB3.Shape.Box(br);
            break;
          default:
            throw new NotImplementedException(
                string.Format("Unsupported shape type: {0}",
                              (object) shapeType));
        }
        br.Position = position + num6;
        this.ActivationPartIndex = br.ReadInt32();
        this.EventEntityID = br.ReadInt32();
        this.HasTypeData = num7 != 0L ||
                           this.Type == MSB3.RegionType.MufflingBox ||
                           this.Type == MSB3.RegionType.MufflingPortal;
        if (!this.HasTypeData)
          return;
        this.ReadSpecific(br);
      }

      internal abstract void ReadSpecific(BinaryReaderEx br);

      internal void Write(BinaryWriterEx bw, int id) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.WriteUInt32((uint) this.Shape.Type);
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.WriteInt32(this.Unk2);
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
        this.Shape.Write(bw, position);
        bw.FillInt64("BaseDataOffset3", bw.Position - position);
        bw.WriteInt32(this.ActivationPartIndex);
        bw.WriteInt32(this.EventEntityID);
        if (this.HasTypeData)
          this.WriteSpecific(bw, position);
        else
          bw.FillInt64("TypeDataOffset", 0L);
        bw.Pad(8);
      }

      internal abstract void WriteSpecific(BinaryWriterEx bw, long start);

      internal virtual void GetNames(MSB3 msb, MSB3.Entries entries) {
        this.ActivationPartName =
            MSB.FindName<MSB3.Part>(entries.Parts, this.ActivationPartIndex);
      }

      internal virtual void GetIndices(MSB3 msb, MSB3.Entries entries) {
        this.ActivationPartIndex =
            MSB.FindIndex<MSB3.Part>(entries.Parts, this.ActivationPartName);
      }

      public override string ToString() {
        return string.Format("{0} {1} : {2}",
                             (object) this.Type,
                             (object) this.Shape.Type,
                             (object) this.Name);
      }

      public abstract class SimpleRegion : MSB3.Region {
        internal SimpleRegion(string name)
            : base(name, false) {}

        internal SimpleRegion(MSB3.Region.SimpleRegion clone)
            : base((MSB3.Region) clone) {}

        internal SimpleRegion(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          throw new InvalidOperationException(
              "SimpleRegions should never have type data.");
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          throw new InvalidOperationException(
              "SimpleRegions should never have type data.");
        }
      }

      public class General : MSB3.Region.SimpleRegion {
        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.General; }
        }

        public General(string name)
            : base(name) {}

        public General(MSB3.Region.General clone)
            : base((MSB3.Region.SimpleRegion) clone) {}

        internal General(BinaryReaderEx br)
            : base(br) {}
      }

      public class Unk00 : MSB3.Region.SimpleRegion {
        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.Unk00; }
        }

        public Unk00(string name)
            : base(name) {}

        public Unk00(MSB3.Region.Unk00 clone)
            : base((MSB3.Region.SimpleRegion) clone) {}

        internal Unk00(BinaryReaderEx br)
            : base(br) {}
      }

      public class InvasionPoint : MSB3.Region {
        public int Priority;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.InvasionPoint; }
        }

        public InvasionPoint(string name)
            : base(name, true) {
          this.Priority = 0;
        }

        public InvasionPoint(MSB3.Region.InvasionPoint clone)
            : base((MSB3.Region) clone) {
          this.Priority = clone.Priority;
        }

        internal InvasionPoint(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.Priority = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", bw.Position - start);
          bw.WriteInt32(this.Priority);
        }
      }

      public class EnvironmentMapPoint : MSB3.Region {
        public int UnkFlags;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.EnvironmentMapPoint; }
        }

        public EnvironmentMapPoint(string name)
            : base(name, true) {
          this.UnkFlags = 0;
        }

        public EnvironmentMapPoint(MSB3.Region.EnvironmentMapPoint clone)
            : base((MSB3.Region) clone) {
          this.UnkFlags = clone.UnkFlags;
        }

        internal EnvironmentMapPoint(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.UnkFlags = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", bw.Position - start);
          bw.WriteInt32(this.UnkFlags);
        }
      }

      public class Sound : MSB3.Region {
        public MSB3.Region.Sound.SndType SoundType;
        public int SoundID;
        private int[] ChildRegionIndices;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.Sound; }
        }

        public string[] ChildRegionNames { get; private set; }

        public Sound(string name)
            : base(name, true) {
          this.SoundType = MSB3.Region.Sound.SndType.Environment;
          this.SoundID = 0;
          this.ChildRegionNames = new string[16];
        }

        public Sound(MSB3.Region.Sound clone)
            : base((MSB3.Region) clone) {
          this.SoundType = clone.SoundType;
          this.SoundID = clone.SoundID;
          this.ChildRegionNames = (string[]) clone.ChildRegionNames.Clone();
        }

        internal Sound(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.SoundType = br.ReadEnum32<MSB3.Region.Sound.SndType>();
          this.SoundID = br.ReadInt32();
          this.ChildRegionIndices = br.ReadInt32s(16);
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", bw.Position - start);
          bw.WriteUInt32((uint) this.SoundType);
          bw.WriteInt32(this.SoundID);
          bw.WriteInt32s((IList<int>) this.ChildRegionIndices);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.ChildRegionNames =
              MSB.FindNames<MSB3.Region>(entries.Regions,
                                         this.ChildRegionIndices);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.ChildRegionIndices =
              MSB.FindIndices<MSB3.Region>(entries.Regions,
                                           this.ChildRegionNames);
        }

        public enum SndType : uint {
          Environment = 0,
          BGM = 6,
          Voice = 7,
        }
      }

      public class SFX : MSB3.Region {
        public int FFXID;
        public bool StartDisabled;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.SFX; }
        }

        public SFX(string name)
            : base(name, true) {
          this.FFXID = -1;
          this.StartDisabled = false;
        }

        public SFX(MSB3.Region.SFX clone)
            : base((MSB3.Region) clone) {
          this.FFXID = clone.FFXID;
          this.StartDisabled = clone.StartDisabled;
        }

        internal SFX(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.FFXID = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.StartDisabled = br.AssertInt32(0, 1) == 1;
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", bw.Position - start);
          bw.WriteInt32(this.FFXID);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.StartDisabled ? 1 : 0);
        }
      }

      public class WindSFX : MSB3.Region {
        public int FFXID;
        public string WindAreaName;
        private int WindAreaIndex;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.WindSFX; }
        }

        public WindSFX(string name)
            : base(name, true) {
          this.FFXID = -1;
          this.WindAreaName = (string) null;
        }

        public WindSFX(MSB3.Region.WindSFX clone)
            : base((MSB3.Region) clone) {
          this.FFXID = clone.FFXID;
          this.WindAreaName = clone.WindAreaName;
        }

        internal WindSFX(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.FFXID = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.WindAreaIndex = br.ReadInt32();
          double num = (double) br.AssertSingle(-1f);
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", bw.Position - start);
          bw.WriteInt32(this.FFXID);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.WindAreaIndex);
          bw.WriteSingle(-1f);
        }

        internal override void GetNames(MSB3 msb, MSB3.Entries entries) {
          base.GetNames(msb, entries);
          this.WindAreaName =
              MSB.FindName<MSB3.Region>(entries.Regions, this.WindAreaIndex);
        }

        internal override void GetIndices(MSB3 msb, MSB3.Entries entries) {
          base.GetIndices(msb, entries);
          this.WindAreaIndex =
              MSB.FindIndex<MSB3.Region>(entries.Regions, this.WindAreaName);
        }
      }

      public class SpawnPoint : MSB3.Region {
        public int UnkT00;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.SpawnPoint; }
        }

        public SpawnPoint(string name)
            : base(name, true) {
          this.UnkT00 = -1;
        }

        public SpawnPoint(MSB3.Region.SpawnPoint clone)
            : base((MSB3.Region) clone) {
          this.UnkT00 = clone.UnkT00;
        }

        internal SpawnPoint(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", bw.Position - start);
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Message : MSB3.Region {
        public short MessageID;
        public short UnkT02;
        public bool Hidden;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.Message; }
        }

        public Message(string name)
            : base(name, true) {
          this.MessageID = (short) -1;
          this.UnkT02 = (short) 0;
          this.Hidden = false;
        }

        public Message(MSB3.Region.Message clone)
            : base((MSB3.Region) clone) {
          this.MessageID = clone.MessageID;
          this.UnkT02 = clone.UnkT02;
          this.Hidden = clone.Hidden;
        }

        internal Message(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.MessageID = br.ReadInt16();
          this.UnkT02 = br.ReadInt16();
          this.Hidden = br.AssertInt32(0, 1) == 1;
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", bw.Position - start);
          bw.WriteInt16(this.MessageID);
          bw.WriteInt16(this.UnkT02);
          bw.WriteInt32(this.Hidden ? 1 : 0);
        }
      }

      public class WalkRoute : MSB3.Region.SimpleRegion {
        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.WalkRoute; }
        }

        public WalkRoute(string name)
            : base(name) {}

        public WalkRoute(MSB3.Region.WalkRoute clone)
            : base((MSB3.Region.SimpleRegion) clone) {}

        internal WalkRoute(BinaryReaderEx br)
            : base(br) {}
      }

      public class Unk12 : MSB3.Region.SimpleRegion {
        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.Unk12; }
        }

        public Unk12(string name)
            : base(name) {}

        public Unk12(MSB3.Region.Unk12 clone)
            : base((MSB3.Region.SimpleRegion) clone) {}

        internal Unk12(BinaryReaderEx br)
            : base(br) {}
      }

      public class WarpPoint : MSB3.Region.SimpleRegion {
        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.WarpPoint; }
        }

        public WarpPoint(string name)
            : base(name) {}

        public WarpPoint(MSB3.Region.WarpPoint clone)
            : base((MSB3.Region.SimpleRegion) clone) {}

        internal WarpPoint(BinaryReaderEx br)
            : base(br) {}
      }

      public class ActivationArea : MSB3.Region.SimpleRegion {
        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.ActivationArea; }
        }

        public ActivationArea(string name)
            : base(name) {}

        public ActivationArea(MSB3.Region.ActivationArea clone)
            : base((MSB3.Region.SimpleRegion) clone) {}

        internal ActivationArea(BinaryReaderEx br)
            : base(br) {}
      }

      public class Event : MSB3.Region.SimpleRegion {
        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.Event; }
        }

        public Event(string name)
            : base(name) {}

        public Event(MSB3.Region.Event clone)
            : base((MSB3.Region.SimpleRegion) clone) {}

        internal Event(BinaryReaderEx br)
            : base(br) {}
      }

      public class EnvironmentMapEffectBox : MSB3.Region {
        public float UnkT00;
        public float Compare;
        public bool UnkT08;
        public byte UnkT09;
        public short UnkT0A;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.EnvironmentMapEffectBox; }
        }

        public EnvironmentMapEffectBox(string name)
            : base(name, true) {
          this.UnkT00 = 0.0f;
          this.Compare = 0.0f;
          this.UnkT08 = false;
          this.UnkT09 = (byte) 0;
          this.UnkT0A = (short) 0;
        }

        public EnvironmentMapEffectBox(
            MSB3.Region.EnvironmentMapEffectBox clone)
            : base((MSB3.Region) clone) {
          this.UnkT00 = clone.UnkT00;
          this.Compare = clone.Compare;
          this.UnkT08 = clone.UnkT08;
          this.UnkT09 = clone.UnkT09;
          this.UnkT0A = clone.UnkT0A;
        }

        internal EnvironmentMapEffectBox(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.UnkT00 = br.ReadSingle();
          this.Compare = br.ReadSingle();
          this.UnkT08 = br.ReadBoolean();
          this.UnkT09 = br.ReadByte();
          this.UnkT0A = br.ReadInt16();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", bw.Position - start);
          bw.WriteSingle(this.UnkT00);
          bw.WriteSingle(this.Compare);
          bw.WriteBoolean(this.UnkT08);
          bw.WriteByte(this.UnkT09);
          bw.WriteInt16(this.UnkT0A);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class WindArea : MSB3.Region.SimpleRegion {
        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.WindArea; }
        }

        public WindArea(string name)
            : base(name) {}

        public WindArea(MSB3.Region.WindArea clone)
            : base((MSB3.Region.SimpleRegion) clone) {}

        internal WindArea(BinaryReaderEx br)
            : base(br) {}
      }

      public class MufflingBox : MSB3.Region {
        public int UnkT00;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.MufflingBox; }
        }

        public MufflingBox(string name)
            : base(name, true) {}

        public MufflingBox(MSB3.Region.MufflingBox clone)
            : base((MSB3.Region) clone) {
          this.UnkT00 = clone.UnkT00;
        }

        internal MufflingBox(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", 0L);
          bw.WriteInt32(this.UnkT00);
        }
      }

      public class MufflingPortal : MSB3.Region {
        public int UnkT00;

        internal override MSB3.RegionType Type {
          get { return MSB3.RegionType.MufflingPortal; }
        }

        public MufflingPortal(string name)
            : base(name, true) {}

        public MufflingPortal(MSB3.Region.MufflingPortal clone)
            : base((MSB3.Region) clone) {
          this.UnkT00 = clone.UnkT00;
        }

        internal MufflingPortal(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadSpecific(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
          br.AssertInt32(MSB3.Part.SHARED_SINGLE);
        }

        internal override void WriteSpecific(BinaryWriterEx bw, long start) {
          bw.FillInt64("TypeDataOffset", 0L);
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }
    }

    public class RouteParam : MSB3.Param<MSB3.Route> {
      public List<MSB3.Route> Routes;

      internal override string Type {
        get { return "ROUTE_PARAM_ST"; }
      }

      public RouteParam(int unk1 = 3)
          : base(unk1) {
        this.Routes = new List<MSB3.Route>();
      }

      public override List<MSB3.Route> GetEntries() {
        return this.Routes;
      }

      internal override MSB3.Route ReadEntry(BinaryReaderEx br) {
        MSB3.Route route = new MSB3.Route(br);
        this.Routes.Add(route);
        return route;
      }

      internal override void WriteEntry(
          BinaryWriterEx bw,
          int id,
          MSB3.Route entry) {
        entry.Write(bw, id);
      }
    }

    public class Route {
      public string Name;
      public int Unk08;
      public int Unk0C;

      public Route() {
        this.Name = "";
      }

      internal Route(BinaryReaderEx br) {
        long position = br.Position;
        long num = br.ReadInt64();
        this.Unk08 = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        br.AssertInt32(4);
        br.ReadInt32();
        br.AssertPattern(104, (byte) 0);
        this.Name = br.GetUTF16(position + num);
      }

      internal void Write(BinaryWriterEx bw, int id) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteInt32(this.Unk08);
        bw.WriteInt32(this.Unk0C);
        bw.WriteInt32(4);
        bw.WriteInt32(id);
        bw.WritePattern(104, (byte) 0);
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(this.Name, true);
        bw.Pad(8);
      }

      public override string ToString() {
        return string.Format("\"{0}\" {1} {2}",
                             (object) this.Name,
                             (object) this.Unk08,
                             (object) this.Unk0C);
      }
    }

    public enum ShapeType : uint {
      Point,
      Circle,
      Sphere,
      Cylinder,
      Square,
      Box,
    }

    public abstract class Shape {
      public abstract MSB3.ShapeType Type { get; }

      internal abstract MSB3.Shape Clone();

      internal abstract void Write(BinaryWriterEx bw, long start);

      public class Point : MSB3.Shape {
        public override MSB3.ShapeType Type {
          get { return MSB3.ShapeType.Point; }
        }

        internal override MSB3.Shape Clone() {
          return (MSB3.Shape) new MSB3.Shape.Point();
        }

        internal override void Write(BinaryWriterEx bw, long start) {
          bw.FillInt64("ShapeDataOffset", 0L);
        }
      }

      public class Circle : MSB3.Shape {
        public float Radius;

        public override MSB3.ShapeType Type {
          get { return MSB3.ShapeType.Circle; }
        }

        public Circle()
            : this(1f) {}

        public Circle(float radius) {
          this.Radius = radius;
        }

        public Circle(MSB3.Shape.Circle clone)
            : this(clone.Radius) {}

        internal override MSB3.Shape Clone() {
          return (MSB3.Shape) new MSB3.Shape.Circle(this);
        }

        internal Circle(BinaryReaderEx br) {
          this.Radius = br.ReadSingle();
        }

        internal override void Write(BinaryWriterEx bw, long start) {
          bw.FillInt64("ShapeDataOffset", bw.Position - start);
          bw.WriteSingle(this.Radius);
        }
      }

      public class Sphere : MSB3.Shape {
        public float Radius;

        public override MSB3.ShapeType Type {
          get { return MSB3.ShapeType.Sphere; }
        }

        public Sphere()
            : this(1f) {}

        public Sphere(float radius) {
          this.Radius = radius;
        }

        public Sphere(MSB3.Shape.Sphere clone)
            : this(clone.Radius) {}

        internal override MSB3.Shape Clone() {
          return (MSB3.Shape) new MSB3.Shape.Sphere(this);
        }

        internal Sphere(BinaryReaderEx br) {
          this.Radius = br.ReadSingle();
        }

        internal override void Write(BinaryWriterEx bw, long start) {
          bw.FillInt64("ShapeDataOffset", bw.Position - start);
          bw.WriteSingle(this.Radius);
        }
      }

      public class Cylinder : MSB3.Shape {
        public float Radius;
        public float Height;

        public override MSB3.ShapeType Type {
          get { return MSB3.ShapeType.Cylinder; }
        }

        public Cylinder()
            : this(1f, 1f) {}

        public Cylinder(float radius, float height) {
          this.Radius = radius;
          this.Height = height;
        }

        public Cylinder(MSB3.Shape.Cylinder clone)
            : this(clone.Radius, clone.Height) {}

        internal override MSB3.Shape Clone() {
          return (MSB3.Shape) new MSB3.Shape.Cylinder(this);
        }

        internal Cylinder(BinaryReaderEx br) {
          this.Radius = br.ReadSingle();
          this.Height = br.ReadSingle();
        }

        internal override void Write(BinaryWriterEx bw, long start) {
          bw.FillInt64("ShapeDataOffset", bw.Position - start);
          bw.WriteSingle(this.Radius);
          bw.WriteSingle(this.Height);
        }
      }

      public class Box : MSB3.Shape {
        public float Width;
        public float Depth;
        public float Height;

        public override MSB3.ShapeType Type {
          get { return MSB3.ShapeType.Box; }
        }

        public Box()
            : this(1f, 1f, 1f) {}

        public Box(float width, float depth, float height) {
          this.Width = width;
          this.Depth = depth;
          this.Height = height;
        }

        public Box(MSB3.Shape.Box clone)
            : this(clone.Width, clone.Depth, clone.Height) {}

        internal override MSB3.Shape Clone() {
          return (MSB3.Shape) new MSB3.Shape.Box(this);
        }

        internal Box(BinaryReaderEx br) {
          this.Width = br.ReadSingle();
          this.Depth = br.ReadSingle();
          this.Height = br.ReadSingle();
        }

        internal override void Write(BinaryWriterEx bw, long start) {
          bw.FillInt64("ShapeDataOffset", bw.Position - start);
          bw.WriteSingle(this.Width);
          bw.WriteSingle(this.Depth);
          bw.WriteSingle(this.Height);
        }
      }
    }
  }
}