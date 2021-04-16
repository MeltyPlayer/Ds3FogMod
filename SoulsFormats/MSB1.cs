// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MSB1
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
  public class MSB1 : SoulsFile<MSB1>, IMsb
  {
    public MSB1.ModelParam Models { get; set; }

    IMsbParam<IMsbModel> IMsb.Models
    {
      get
      {
        return (IMsbParam<IMsbModel>) this.Models;
      }
    }

    public MSB1.EventParam Events { get; set; }

    public MSB1.PointParam Regions { get; set; }

    IMsbParam<IMsbRegion> IMsb.Regions
    {
      get
      {
        return (IMsbParam<IMsbRegion>) this.Regions;
      }
    }

    public MSB1.PartsParam Parts { get; set; }

    IMsbParam<IMsbPart> IMsb.Parts
    {
      get
      {
        return (IMsbParam<IMsbPart>) this.Parts;
      }
    }

    public MSB1()
    {
      this.Models = new MSB1.ModelParam();
      this.Events = new MSB1.EventParam();
      this.Regions = new MSB1.PointParam();
      this.Parts = new MSB1.PartsParam();
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      this.Models = new MSB1.ModelParam();
      MSB1.Entries entries;
      entries.Models = this.Models.Read(br);
      this.Events = new MSB1.EventParam();
      entries.Events = this.Events.Read(br);
      this.Regions = new MSB1.PointParam();
      entries.Regions = this.Regions.Read(br);
      this.Parts = new MSB1.PartsParam();
      entries.Parts = this.Parts.Read(br);
      if (br.Position != 0L)
        throw new InvalidDataException("The next param offset of the final param should be 0, but it wasn't.");
      MSB.DisambiguateNames<MSB1.Model>(entries.Models);
      MSB.DisambiguateNames<MSB1.Region>(entries.Regions);
      MSB.DisambiguateNames<MSB1.Part>(entries.Parts);
      foreach (MSB1.Event @event in entries.Events)
        @event.GetNames(this, entries);
      foreach (MSB1.Part part in entries.Parts)
        part.GetNames(this, entries);
    }

    protected override void Write(BinaryWriterEx bw)
    {
      MSB1.Entries entries;
      entries.Models = this.Models.GetEntries();
      entries.Events = this.Events.GetEntries();
      entries.Regions = this.Regions.GetEntries();
      entries.Parts = this.Parts.GetEntries();
      this.Models.DiscriminateModels();
      foreach (MSB1.Model model in entries.Models)
        model.CountInstances(entries.Parts);
      foreach (MSB1.Event @event in entries.Events)
        @event.GetIndices(this, entries);
      foreach (MSB1.Part part in entries.Parts)
        part.GetIndices(this, entries);
      bw.BigEndian = false;
      this.Models.Write(bw, entries.Models);
      bw.FillInt32("NextParamOffset", (int) bw.Position);
      this.Events.Write(bw, entries.Events);
      bw.FillInt32("NextParamOffset", (int) bw.Position);
      this.Regions.Write(bw, entries.Regions);
      bw.FillInt32("NextParamOffset", (int) bw.Position);
      this.Parts.Write(bw, entries.Parts);
      bw.FillInt32("NextParamOffset", 0);
    }

    public enum EventType : uint
    {
      Light,
      Sound,
      SFX,
      WindSFX,
      Treasure,
      Generator,
      Message,
      ObjAct,
      SpawnPoint,
      MapOffset,
      Navmesh,
      Environment,
      PseudoMultiplayer,
    }

    public class EventParam : MSB1.Param<MSB1.Event>
    {
      internal override string Name
      {
        get
        {
          return "EVENT_PARAM_ST";
        }
      }

      public List<MSB1.Event.Light> Lights { get; set; }

      public List<MSB1.Event.Sound> Sounds { get; set; }

      public List<MSB1.Event.SFX> SFXs { get; set; }

      public List<MSB1.Event.WindSFX> WindSFXs { get; set; }

      public List<MSB1.Event.Treasure> Treasures { get; set; }

      public List<MSB1.Event.Generator> Generators { get; set; }

      public List<MSB1.Event.Message> Messages { get; set; }

      public List<MSB1.Event.ObjAct> ObjActs { get; set; }

      public List<MSB1.Event.SpawnPoint> SpawnPoints { get; set; }

      public List<MSB1.Event.MapOffset> MapOffsets { get; set; }

      public List<MSB1.Event.Navmesh> Navmeshes { get; set; }

      public List<MSB1.Event.Environment> Environments { get; set; }

      public List<MSB1.Event.PseudoMultiplayer> PseudoMultiplayers { get; set; }

      public EventParam()
      {
        this.Lights = new List<MSB1.Event.Light>();
        this.Sounds = new List<MSB1.Event.Sound>();
        this.SFXs = new List<MSB1.Event.SFX>();
        this.WindSFXs = new List<MSB1.Event.WindSFX>();
        this.Treasures = new List<MSB1.Event.Treasure>();
        this.Generators = new List<MSB1.Event.Generator>();
        this.Messages = new List<MSB1.Event.Message>();
        this.ObjActs = new List<MSB1.Event.ObjAct>();
        this.SpawnPoints = new List<MSB1.Event.SpawnPoint>();
        this.MapOffsets = new List<MSB1.Event.MapOffset>();
        this.Navmeshes = new List<MSB1.Event.Navmesh>();
        this.Environments = new List<MSB1.Event.Environment>();
        this.PseudoMultiplayers = new List<MSB1.Event.PseudoMultiplayer>();
      }

      public override List<MSB1.Event> GetEntries()
      {
        return SFUtil.ConcatAll<MSB1.Event>(new IEnumerable<MSB1.Event>[13]
        {
          (IEnumerable<MSB1.Event>) this.Lights,
          (IEnumerable<MSB1.Event>) this.Sounds,
          (IEnumerable<MSB1.Event>) this.SFXs,
          (IEnumerable<MSB1.Event>) this.WindSFXs,
          (IEnumerable<MSB1.Event>) this.Treasures,
          (IEnumerable<MSB1.Event>) this.Generators,
          (IEnumerable<MSB1.Event>) this.Messages,
          (IEnumerable<MSB1.Event>) this.ObjActs,
          (IEnumerable<MSB1.Event>) this.SpawnPoints,
          (IEnumerable<MSB1.Event>) this.MapOffsets,
          (IEnumerable<MSB1.Event>) this.Navmeshes,
          (IEnumerable<MSB1.Event>) this.Environments,
          (IEnumerable<MSB1.Event>) this.PseudoMultiplayers
        });
      }

      internal override MSB1.Event ReadEntry(BinaryReaderEx br)
      {
        MSB1.EventType enum32 = br.GetEnum32<MSB1.EventType>(br.Position + 8L);
        switch (enum32)
        {
          case MSB1.EventType.Light:
            MSB1.Event.Light light = new MSB1.Event.Light(br);
            this.Lights.Add(light);
            return (MSB1.Event) light;
          case MSB1.EventType.Sound:
            MSB1.Event.Sound sound = new MSB1.Event.Sound(br);
            this.Sounds.Add(sound);
            return (MSB1.Event) sound;
          case MSB1.EventType.SFX:
            MSB1.Event.SFX sfx = new MSB1.Event.SFX(br);
            this.SFXs.Add(sfx);
            return (MSB1.Event) sfx;
          case MSB1.EventType.WindSFX:
            MSB1.Event.WindSFX windSfx = new MSB1.Event.WindSFX(br);
            this.WindSFXs.Add(windSfx);
            return (MSB1.Event) windSfx;
          case MSB1.EventType.Treasure:
            MSB1.Event.Treasure treasure = new MSB1.Event.Treasure(br);
            this.Treasures.Add(treasure);
            return (MSB1.Event) treasure;
          case MSB1.EventType.Generator:
            MSB1.Event.Generator generator = new MSB1.Event.Generator(br);
            this.Generators.Add(generator);
            return (MSB1.Event) generator;
          case MSB1.EventType.Message:
            MSB1.Event.Message message = new MSB1.Event.Message(br);
            this.Messages.Add(message);
            return (MSB1.Event) message;
          case MSB1.EventType.ObjAct:
            MSB1.Event.ObjAct objAct = new MSB1.Event.ObjAct(br);
            this.ObjActs.Add(objAct);
            return (MSB1.Event) objAct;
          case MSB1.EventType.SpawnPoint:
            MSB1.Event.SpawnPoint spawnPoint = new MSB1.Event.SpawnPoint(br);
            this.SpawnPoints.Add(spawnPoint);
            return (MSB1.Event) spawnPoint;
          case MSB1.EventType.MapOffset:
            MSB1.Event.MapOffset mapOffset = new MSB1.Event.MapOffset(br);
            this.MapOffsets.Add(mapOffset);
            return (MSB1.Event) mapOffset;
          case MSB1.EventType.Navmesh:
            MSB1.Event.Navmesh navmesh = new MSB1.Event.Navmesh(br);
            this.Navmeshes.Add(navmesh);
            return (MSB1.Event) navmesh;
          case MSB1.EventType.Environment:
            MSB1.Event.Environment environment = new MSB1.Event.Environment(br);
            this.Environments.Add(environment);
            return (MSB1.Event) environment;
          case MSB1.EventType.PseudoMultiplayer:
            MSB1.Event.PseudoMultiplayer pseudoMultiplayer = new MSB1.Event.PseudoMultiplayer(br);
            this.PseudoMultiplayers.Add(pseudoMultiplayer);
            return (MSB1.Event) pseudoMultiplayer;
          default:
            throw new NotImplementedException(string.Format("Unsupported event type: {0}", (object) enum32));
        }
      }
    }

    public abstract class Event : MSB1.Entry
    {
      private int PartIndex;
      private int RegionIndex;

      public int EventID { get; set; }

      public abstract MSB1.EventType Type { get; }

      public string PartName { get; set; }

      public string RegionName { get; set; }

      public int EntityID { get; set; }

      internal Event()
      {
        this.Name = "";
        this.EventID = -1;
        this.EntityID = -1;
      }

      internal Event(BinaryReaderEx br)
      {
        long position = br.Position;
        int num1 = br.ReadInt32();
        this.EventID = br.ReadInt32();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        int num3 = br.ReadInt32();
        int num4 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Name = br.GetShiftJIS(position + (long) num1);
        br.Position = position + (long) num3;
        this.PartIndex = br.ReadInt32();
        this.RegionIndex = br.ReadInt32();
        this.EntityID = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.Position = position + (long) num4;
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position = bw.Position;
        bw.ReserveInt32("NameOffset");
        bw.WriteInt32(this.EventID);
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.ReserveInt32("BaseDataOffset");
        bw.ReserveInt32("TypeDataOffset");
        bw.WriteInt32(0);
        bw.FillInt32("NameOffset", (int) (bw.Position - position));
        bw.WriteShiftJIS(this.Name, true);
        bw.Pad(4);
        bw.FillInt32("BaseDataOffset", (int) (bw.Position - position));
        bw.WriteInt32(this.PartIndex);
        bw.WriteInt32(this.RegionIndex);
        bw.WriteInt32(this.EntityID);
        bw.WriteInt32(0);
        bw.FillInt32("TypeDataOffset", (int) (bw.Position - position));
      }

      internal virtual void GetNames(MSB1 msb, MSB1.Entries entries)
      {
        this.PartName = MSB.FindName<MSB1.Part>(entries.Parts, this.PartIndex);
        this.RegionName = MSB.FindName<MSB1.Region>(entries.Regions, this.RegionIndex);
      }

      internal virtual void GetIndices(MSB1 msb, MSB1.Entries entries)
      {
        this.PartIndex = MSB.FindIndex<MSB1.Part>(entries.Parts, this.PartName);
        this.RegionIndex = MSB.FindIndex<MSB1.Region>(entries.Regions, this.RegionName);
      }

      public override string ToString()
      {
        return string.Format("{0} {1}", (object) this.Type, (object) this.Name);
      }

      public class Light : MSB1.Event
      {
        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.Light;
          }
        }

        public int UnkT00 { get; set; }

        public Light()
        {
        }

        internal Light(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.UnkT00);
        }
      }

      public class Sound : MSB1.Event
      {
        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.Sound;
          }
        }

        public int SoundType { get; set; }

        public int SoundID { get; set; }

        public Sound()
        {
        }

        internal Sound(BinaryReaderEx br)
          : base(br)
        {
          this.SoundType = br.ReadInt32();
          this.SoundID = br.ReadInt32();
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.SoundType);
          bw.WriteInt32(this.SoundID);
        }
      }

      public class SFX : MSB1.Event
      {
        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.SFX;
          }
        }

        public int FFXID { get; set; }

        public SFX()
        {
        }

        internal SFX(BinaryReaderEx br)
          : base(br)
        {
          this.FFXID = br.ReadInt32();
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.FFXID);
        }
      }

      public class WindSFX : MSB1.Event
      {
        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.WindSFX;
          }
        }

        public float UnkT00 { get; set; }

        public float UnkT04 { get; set; }

        public float UnkT08 { get; set; }

        public float UnkT0C { get; set; }

        public float UnkT10 { get; set; }

        public float UnkT14 { get; set; }

        public float UnkT18 { get; set; }

        public float UnkT1C { get; set; }

        public float UnkT20 { get; set; }

        public float UnkT24 { get; set; }

        public float UnkT28 { get; set; }

        public float UnkT2C { get; set; }

        public float UnkT30 { get; set; }

        public float UnkT34 { get; set; }

        public float UnkT38 { get; set; }

        public float UnkT3C { get; set; }

        public WindSFX()
        {
        }

        internal WindSFX(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadSingle();
          this.UnkT04 = br.ReadSingle();
          this.UnkT08 = br.ReadSingle();
          this.UnkT0C = br.ReadSingle();
          this.UnkT10 = br.ReadSingle();
          this.UnkT14 = br.ReadSingle();
          this.UnkT18 = br.ReadSingle();
          this.UnkT1C = br.ReadSingle();
          this.UnkT20 = br.ReadSingle();
          this.UnkT24 = br.ReadSingle();
          this.UnkT28 = br.ReadSingle();
          this.UnkT2C = br.ReadSingle();
          this.UnkT30 = br.ReadSingle();
          this.UnkT34 = br.ReadSingle();
          this.UnkT38 = br.ReadSingle();
          this.UnkT3C = br.ReadSingle();
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteSingle(this.UnkT00);
          bw.WriteSingle(this.UnkT04);
          bw.WriteSingle(this.UnkT08);
          bw.WriteSingle(this.UnkT0C);
          bw.WriteSingle(this.UnkT10);
          bw.WriteSingle(this.UnkT14);
          bw.WriteSingle(this.UnkT18);
          bw.WriteSingle(this.UnkT1C);
          bw.WriteSingle(this.UnkT20);
          bw.WriteSingle(this.UnkT24);
          bw.WriteSingle(this.UnkT28);
          bw.WriteSingle(this.UnkT2C);
          bw.WriteSingle(this.UnkT30);
          bw.WriteSingle(this.UnkT34);
          bw.WriteSingle(this.UnkT38);
          bw.WriteSingle(this.UnkT3C);
        }
      }

      public class Treasure : MSB1.Event
      {
        private int TreasurePartIndex;

        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.Treasure;
          }
        }

        public string TreasurePartName { get; set; }

        public int[] ItemLots { get; private set; }

        public bool InChest { get; set; }

        public bool StartDisabled { get; set; }

        public Treasure()
        {
          this.ItemLots = new int[5]{ -1, -1, -1, -1, -1 };
        }

        internal Treasure(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          this.TreasurePartIndex = br.ReadInt32();
          this.ItemLots = new int[5];
          for (int index = 0; index < 5; ++index)
          {
            this.ItemLots[index] = br.ReadInt32();
            br.AssertInt32(-1);
          }
          this.InChest = br.ReadBoolean();
          this.StartDisabled = br.ReadBoolean();
          int num = (int) br.AssertInt16(new short[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(0);
          bw.WriteInt32(this.TreasurePartIndex);
          for (int index = 0; index < 5; ++index)
          {
            bw.WriteInt32(this.ItemLots[index]);
            bw.WriteInt32(-1);
          }
          bw.WriteBoolean(this.InChest);
          bw.WriteBoolean(this.StartDisabled);
          bw.WriteInt16((short) 0);
        }

        internal override void GetNames(MSB1 msb, MSB1.Entries entries)
        {
          base.GetNames(msb, entries);
          this.TreasurePartName = MSB.FindName<MSB1.Part>(entries.Parts, this.TreasurePartIndex);
        }

        internal override void GetIndices(MSB1 msb, MSB1.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.TreasurePartIndex = MSB.FindIndex<MSB1.Part>(entries.Parts, this.TreasurePartName);
        }
      }

      public class Generator : MSB1.Event
      {
        private int[] SpawnPointIndices;
        private int[] SpawnPartIndices;

        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.Generator;
          }
        }

        public short MaxNum { get; set; }

        public short LimitNum { get; set; }

        public short MinGenNum { get; set; }

        public short MaxGenNum { get; set; }

        public float MinInterval { get; set; }

        public float MaxInterval { get; set; }

        public int InitialSpawnCount { get; set; }

        public string[] SpawnPointNames { get; private set; }

        public string[] SpawnPartNames { get; private set; }

        public Generator()
        {
          this.SpawnPointNames = new string[4];
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
          this.InitialSpawnCount = br.ReadInt32();
          br.AssertPattern(28, (byte) 0);
          this.SpawnPointIndices = br.ReadInt32s(4);
          this.SpawnPartIndices = br.ReadInt32s(32);
          br.AssertPattern(64, (byte) 0);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt16(this.MaxNum);
          bw.WriteInt16(this.LimitNum);
          bw.WriteInt16(this.MinGenNum);
          bw.WriteInt16(this.MaxGenNum);
          bw.WriteSingle(this.MinInterval);
          bw.WriteSingle(this.MaxInterval);
          bw.WriteInt32(this.InitialSpawnCount);
          bw.WritePattern(28, (byte) 0);
          bw.WriteInt32s((IList<int>) this.SpawnPointIndices);
          bw.WriteInt32s((IList<int>) this.SpawnPartIndices);
          bw.WritePattern(64, (byte) 0);
        }

        internal override void GetNames(MSB1 msb, MSB1.Entries entries)
        {
          base.GetNames(msb, entries);
          this.SpawnPointNames = MSB.FindNames<MSB1.Region>(entries.Regions, this.SpawnPointIndices);
          this.SpawnPartNames = MSB.FindNames<MSB1.Part>(entries.Parts, this.SpawnPartIndices);
        }

        internal override void GetIndices(MSB1 msb, MSB1.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.SpawnPointIndices = MSB.FindIndices<MSB1.Region>(entries.Regions, this.SpawnPointNames);
          this.SpawnPartIndices = MSB.FindIndices<MSB1.Part>(entries.Parts, this.SpawnPartNames);
        }
      }

      public class Message : MSB1.Event
      {
        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.Message;
          }
        }

        public short MessageID { get; set; }

        public short UnkT02 { get; set; }

        public bool Hidden { get; set; }

        public Message()
        {
        }

        internal Message(BinaryReaderEx br)
          : base(br)
        {
          this.MessageID = br.ReadInt16();
          this.UnkT02 = br.ReadInt16();
          this.Hidden = br.ReadBoolean();
          int num1 = (int) br.AssertByte(new byte[1]);
          int num2 = (int) br.AssertInt16(new short[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt16(this.MessageID);
          bw.WriteInt16(this.UnkT02);
          bw.WriteBoolean(this.Hidden);
          bw.WriteByte((byte) 0);
          bw.WriteInt16((short) 0);
        }
      }

      public class ObjAct : MSB1.Event
      {
        private int ObjActPartIndex;

        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.ObjAct;
          }
        }

        public int ObjActEntityID { get; set; }

        public string ObjActPartName { get; set; }

        public short ObjActParamID { get; set; }

        public short UnkT0A { get; set; }

        public int EventFlagID { get; set; }

        public ObjAct()
        {
          this.ObjActEntityID = -1;
          this.ObjActParamID = (short) -1;
          this.EventFlagID = -1;
        }

        internal ObjAct(BinaryReaderEx br)
          : base(br)
        {
          this.ObjActEntityID = br.ReadInt32();
          this.ObjActPartIndex = br.ReadInt32();
          this.ObjActParamID = br.ReadInt16();
          this.UnkT0A = br.ReadInt16();
          this.EventFlagID = br.ReadInt32();
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.ObjActEntityID);
          bw.WriteInt32(this.ObjActPartIndex);
          bw.WriteInt16(this.ObjActParamID);
          bw.WriteInt16(this.UnkT0A);
          bw.WriteInt32(this.EventFlagID);
        }

        internal override void GetNames(MSB1 msb, MSB1.Entries entries)
        {
          base.GetNames(msb, entries);
          this.ObjActPartName = MSB.FindName<MSB1.Part>(entries.Parts, this.ObjActPartIndex);
        }

        internal override void GetIndices(MSB1 msb, MSB1.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.ObjActPartIndex = MSB.FindIndex<MSB1.Part>(entries.Parts, this.ObjActPartName);
        }
      }

      public class SpawnPoint : MSB1.Event
      {
        private int SpawnPointIndex;

        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.SpawnPoint;
          }
        }

        public string SpawnPointName { get; set; }

        public SpawnPoint()
        {
        }

        internal SpawnPoint(BinaryReaderEx br)
          : base(br)
        {
          this.SpawnPointIndex = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.SpawnPointIndex);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSB1 msb, MSB1.Entries entries)
        {
          base.GetNames(msb, entries);
          this.SpawnPointName = MSB.FindName<MSB1.Region>(entries.Regions, this.SpawnPointIndex);
        }

        internal override void GetIndices(MSB1 msb, MSB1.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.SpawnPointIndex = MSB.FindIndex<MSB1.Region>(entries.Regions, this.SpawnPointName);
        }
      }

      public class MapOffset : MSB1.Event
      {
        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.MapOffset;
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

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteVector3(this.Position);
          bw.WriteSingle(this.Degree);
        }
      }

      public class Navmesh : MSB1.Event
      {
        private int NavmeshRegionIndex;

        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.Navmesh;
          }
        }

        public string NavmeshRegionName { get; set; }

        public Navmesh()
        {
        }

        internal Navmesh(BinaryReaderEx br)
          : base(br)
        {
          this.NavmeshRegionIndex = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.NavmeshRegionIndex);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSB1 msb, MSB1.Entries entries)
        {
          base.GetNames(msb, entries);
          this.NavmeshRegionName = MSB.FindName<MSB1.Region>(entries.Regions, this.NavmeshRegionIndex);
        }

        internal override void GetIndices(MSB1 msb, MSB1.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.NavmeshRegionIndex = MSB.FindIndex<MSB1.Region>(entries.Regions, this.NavmeshRegionName);
        }
      }

      public class Environment : MSB1.Event
      {
        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.Environment;
          }
        }

        public int UnkT00 { get; set; }

        public float UnkT04 { get; set; }

        public float UnkT08 { get; set; }

        public float UnkT0C { get; set; }

        public float UnkT10 { get; set; }

        public float UnkT14 { get; set; }

        public Environment()
        {
        }

        internal Environment(BinaryReaderEx br)
          : base(br)
        {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadSingle();
          this.UnkT08 = br.ReadSingle();
          this.UnkT0C = br.ReadSingle();
          this.UnkT10 = br.ReadSingle();
          this.UnkT14 = br.ReadSingle();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.UnkT00);
          bw.WriteSingle(this.UnkT04);
          bw.WriteSingle(this.UnkT08);
          bw.WriteSingle(this.UnkT0C);
          bw.WriteSingle(this.UnkT10);
          bw.WriteSingle(this.UnkT14);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class PseudoMultiplayer : MSB1.Event
      {
        public override MSB1.EventType Type
        {
          get
          {
            return MSB1.EventType.PseudoMultiplayer;
          }
        }

        public int HostEntityID { get; set; }

        public int EventFlagID { get; set; }

        public int ActivateGoodsID { get; set; }

        public PseudoMultiplayer()
        {
          this.HostEntityID = -1;
          this.EventFlagID = -1;
        }

        internal PseudoMultiplayer(BinaryReaderEx br)
          : base(br)
        {
          this.HostEntityID = br.ReadInt32();
          this.EventFlagID = br.ReadInt32();
          this.ActivateGoodsID = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.HostEntityID);
          bw.WriteInt32(this.EventFlagID);
          bw.WriteInt32(this.ActivateGoodsID);
          bw.WriteInt32(0);
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
      Navmesh = 6,
    }

    public class ModelParam : MSB1.Param<MSB1.Model>, IMsbParam<IMsbModel>
    {
      internal override string Name
      {
        get
        {
          return "MODEL_PARAM_ST";
        }
      }

      public List<MSB1.Model> MapPieces { get; set; }

      public List<MSB1.Model> Objects { get; set; }

      public List<MSB1.Model> Enemies { get; set; }

      public List<MSB1.Model> Players { get; set; }

      public List<MSB1.Model> Collisions { get; set; }

      public List<MSB1.Model> Navmeshes { get; set; }

      public ModelParam()
      {
        this.MapPieces = new List<MSB1.Model>();
        this.Objects = new List<MSB1.Model>();
        this.Enemies = new List<MSB1.Model>();
        this.Players = new List<MSB1.Model>();
        this.Collisions = new List<MSB1.Model>();
        this.Navmeshes = new List<MSB1.Model>();
      }

      internal override MSB1.Model ReadEntry(BinaryReaderEx br)
      {
        MSB1.ModelType enum32 = br.GetEnum32<MSB1.ModelType>(br.Position + 4L);
        MSB1.Model model = new MSB1.Model(br);
        switch (enum32)
        {
          case MSB1.ModelType.MapPiece:
            this.MapPieces.Add(model);
            break;
          case MSB1.ModelType.Object:
            this.Objects.Add(model);
            break;
          case MSB1.ModelType.Enemy:
            this.Enemies.Add(model);
            break;
          case MSB1.ModelType.Player:
            this.Players.Add(model);
            break;
          case MSB1.ModelType.Collision:
            this.Collisions.Add(model);
            break;
          case MSB1.ModelType.Navmesh:
            this.Navmeshes.Add(model);
            break;
          default:
            throw new NotImplementedException(string.Format("Unimplemented model type: {0}", (object) enum32));
        }
        return model;
      }

      public override List<MSB1.Model> GetEntries()
      {
        return SFUtil.ConcatAll<MSB1.Model>(new IEnumerable<MSB1.Model>[6]
        {
          (IEnumerable<MSB1.Model>) this.MapPieces,
          (IEnumerable<MSB1.Model>) this.Objects,
          (IEnumerable<MSB1.Model>) this.Enemies,
          (IEnumerable<MSB1.Model>) this.Players,
          (IEnumerable<MSB1.Model>) this.Collisions,
          (IEnumerable<MSB1.Model>) this.Navmeshes
        });
      }

      IReadOnlyList<IMsbModel> IMsbParam<IMsbModel>.GetEntries()
      {
        return (IReadOnlyList<IMsbModel>) this.GetEntries();
      }

      internal void DiscriminateModels()
      {
        for (int id = 0; id < this.MapPieces.Count; ++id)
          this.MapPieces[id].Discriminate(MSB1.ModelType.MapPiece, id);
        for (int id = 0; id < this.Objects.Count; ++id)
          this.Objects[id].Discriminate(MSB1.ModelType.Object, id);
        for (int id = 0; id < this.Enemies.Count; ++id)
          this.Enemies[id].Discriminate(MSB1.ModelType.Enemy, id);
        for (int id = 0; id < this.Players.Count; ++id)
          this.Players[id].Discriminate(MSB1.ModelType.Player, id);
        for (int id = 0; id < this.Collisions.Count; ++id)
          this.Collisions[id].Discriminate(MSB1.ModelType.Collision, id);
        for (int id = 0; id < this.Navmeshes.Count; ++id)
          this.Navmeshes[id].Discriminate(MSB1.ModelType.Navmesh, id);
      }
    }

    public class Model : MSB1.Entry, IMsbModel, IMsbEntry
    {
      private MSB1.ModelType Type;
      private int ID;
      private int InstanceCount;

      public string Placeholder { get; set; }

      public Model()
      {
        this.Name = "";
        this.Placeholder = "";
      }

      internal Model(BinaryReaderEx br)
      {
        long position = br.Position;
        int num1 = br.ReadInt32();
        int num2 = (int) br.ReadEnum32<MSB1.ModelType>();
        br.ReadInt32();
        int num3 = br.ReadInt32();
        this.InstanceCount = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Name = br.GetShiftJIS(position + (long) num1);
        this.Placeholder = br.GetShiftJIS(position + (long) num3);
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position = bw.Position;
        bw.ReserveInt32("NameOffset");
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(this.ID);
        bw.ReserveInt32("SibOffset");
        bw.WriteInt32(this.InstanceCount);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.FillInt32("NameOffset", (int) (bw.Position - position));
        bw.WriteShiftJIS(MSB.ReambiguateName(this.Name), true);
        bw.FillInt32("SibOffset", (int) (bw.Position - position));
        bw.WriteShiftJIS(this.Placeholder, true);
        bw.Pad(4);
      }

      internal void CountInstances(List<MSB1.Part> parts)
      {
        this.InstanceCount = parts.Count<MSB1.Part>((Func<MSB1.Part, bool>) (p => p.ModelName == this.Name));
      }

      internal void Discriminate(MSB1.ModelType type, int id)
      {
        this.Type = type;
        this.ID = id;
      }
    }

    internal struct Entries
    {
      public List<MSB1.Model> Models;
      public List<MSB1.Event> Events;
      public List<MSB1.Region> Regions;
      public List<MSB1.Part> Parts;
    }

    public abstract class Param<T> where T : MSB1.Entry
    {
      internal abstract string Name { get; }

      internal List<T> Read(BinaryReaderEx br)
      {
        br.AssertInt32(new int[1]);
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int[] numArray = br.ReadInt32s(num2 - 1);
        int num3 = br.ReadInt32();
        string ascii = br.GetASCII((long) num1);
        if (ascii != this.Name)
          throw new InvalidDataException("Expected param \"" + this.Name + "\", got param \"" + ascii + "\"");
        List<T> objList = new List<T>(num2 - 1);
        foreach (int num4 in numArray)
        {
          br.Position = (long) num4;
          objList.Add(this.ReadEntry(br));
        }
        br.Position = (long) num3;
        return objList;
      }

      internal abstract T ReadEntry(BinaryReaderEx br);

      internal virtual void Write(BinaryWriterEx bw, List<T> entries)
      {
        bw.WriteInt32(0);
        bw.ReserveInt32("ParamNameOffset");
        bw.WriteInt32(entries.Count + 1);
        for (int index = 0; index < entries.Count; ++index)
          bw.ReserveInt32(string.Format("EntryOffset{0}", (object) index));
        bw.ReserveInt32("NextParamOffset");
        bw.FillInt32("ParamNameOffset", (int) bw.Position);
        bw.WriteASCII(this.Name, true);
        bw.Pad(4);
        int id = 0;
        System.Type type = (System.Type) null;
        for (int index = 0; index < entries.Count; ++index)
        {
          if (type != entries[index].GetType())
          {
            type = entries[index].GetType();
            id = 0;
          }
          bw.FillInt32(string.Format("EntryOffset{0}", (object) index), (int) bw.Position);
          entries[index].Write(bw, id);
          ++id;
        }
      }

      public abstract List<T> GetEntries();

      public override string ToString()
      {
        return this.Name ?? "";
      }
    }

    public abstract class Entry : IMsbEntry
    {
      public string Name { get; set; }

      internal abstract void Write(BinaryWriterEx bw, int id);
    }

    public enum PartType : uint
    {
      MapPiece = 0,
      Object = 1,
      Enemy = 2,
      Player = 4,
      Collision = 5,
      Navmesh = 8,
      DummyObject = 9,
      DummyEnemy = 10, // 0x0000000A
      ConnectCollision = 11, // 0x0000000B
    }

    public class PartsParam : MSB1.Param<MSB1.Part>, IMsbParam<IMsbPart>
    {
      internal override string Name
      {
        get
        {
          return "PARTS_PARAM_ST";
        }
      }

      public List<MSB1.Part.MapPiece> MapPieces { get; set; }

      public List<MSB1.Part.Object> Objects { get; set; }

      public List<MSB1.Part.Enemy> Enemies { get; set; }

      public List<MSB1.Part.Player> Players { get; set; }

      public List<MSB1.Part.Collision> Collisions { get; set; }

      public List<MSB1.Part.Navmesh> Navmeshes { get; set; }

      public List<MSB1.Part.DummyObject> DummyObjects { get; set; }

      public List<MSB1.Part.DummyEnemy> DummyEnemies { get; set; }

      public List<MSB1.Part.ConnectCollision> ConnectCollisions { get; set; }

      public PartsParam()
      {
        this.MapPieces = new List<MSB1.Part.MapPiece>();
        this.Objects = new List<MSB1.Part.Object>();
        this.Enemies = new List<MSB1.Part.Enemy>();
        this.Players = new List<MSB1.Part.Player>();
        this.Collisions = new List<MSB1.Part.Collision>();
        this.Navmeshes = new List<MSB1.Part.Navmesh>();
        this.DummyObjects = new List<MSB1.Part.DummyObject>();
        this.DummyEnemies = new List<MSB1.Part.DummyEnemy>();
        this.ConnectCollisions = new List<MSB1.Part.ConnectCollision>();
      }

      internal override MSB1.Part ReadEntry(BinaryReaderEx br)
      {
        MSB1.PartType enum32 = br.GetEnum32<MSB1.PartType>(br.Position + 4L);
        switch (enum32)
        {
          case MSB1.PartType.MapPiece:
            MSB1.Part.MapPiece mapPiece = new MSB1.Part.MapPiece(br);
            this.MapPieces.Add(mapPiece);
            return (MSB1.Part) mapPiece;
          case MSB1.PartType.Object:
            MSB1.Part.Object @object = new MSB1.Part.Object(br);
            this.Objects.Add(@object);
            return (MSB1.Part) @object;
          case MSB1.PartType.Enemy:
            MSB1.Part.Enemy enemy = new MSB1.Part.Enemy(br);
            this.Enemies.Add(enemy);
            return (MSB1.Part) enemy;
          case MSB1.PartType.Player:
            MSB1.Part.Player player = new MSB1.Part.Player(br);
            this.Players.Add(player);
            return (MSB1.Part) player;
          case MSB1.PartType.Collision:
            MSB1.Part.Collision collision = new MSB1.Part.Collision(br);
            this.Collisions.Add(collision);
            return (MSB1.Part) collision;
          case MSB1.PartType.Navmesh:
            MSB1.Part.Navmesh navmesh = new MSB1.Part.Navmesh(br);
            this.Navmeshes.Add(navmesh);
            return (MSB1.Part) navmesh;
          case MSB1.PartType.DummyObject:
            MSB1.Part.DummyObject dummyObject = new MSB1.Part.DummyObject(br);
            this.DummyObjects.Add(dummyObject);
            return (MSB1.Part) dummyObject;
          case MSB1.PartType.DummyEnemy:
            MSB1.Part.DummyEnemy dummyEnemy = new MSB1.Part.DummyEnemy(br);
            this.DummyEnemies.Add(dummyEnemy);
            return (MSB1.Part) dummyEnemy;
          case MSB1.PartType.ConnectCollision:
            MSB1.Part.ConnectCollision connectCollision = new MSB1.Part.ConnectCollision(br);
            this.ConnectCollisions.Add(connectCollision);
            return (MSB1.Part) connectCollision;
          default:
            throw new NotImplementedException(string.Format("Unimplemented part type: {0}", (object) enum32));
        }
      }

      public override List<MSB1.Part> GetEntries()
      {
        return SFUtil.ConcatAll<MSB1.Part>(new IEnumerable<MSB1.Part>[9]
        {
          (IEnumerable<MSB1.Part>) this.MapPieces,
          (IEnumerable<MSB1.Part>) this.Objects,
          (IEnumerable<MSB1.Part>) this.Enemies,
          (IEnumerable<MSB1.Part>) this.Players,
          (IEnumerable<MSB1.Part>) this.Collisions,
          (IEnumerable<MSB1.Part>) this.Navmeshes,
          (IEnumerable<MSB1.Part>) this.DummyObjects,
          (IEnumerable<MSB1.Part>) this.DummyEnemies,
          (IEnumerable<MSB1.Part>) this.ConnectCollisions
        });
      }

      IReadOnlyList<IMsbPart> IMsbParam<IMsbPart>.GetEntries()
      {
        return (IReadOnlyList<IMsbPart>) this.GetEntries();
      }
    }

    public abstract class Part : MSB1.Entry, IMsbPart, IMsbEntry
    {
      private int ModelIndex;

      public abstract MSB1.PartType Type { get; }

      public string ModelName { get; set; }

      public string Placeholder { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      public Vector3 Scale { get; set; }

      public uint[] DrawGroups { get; private set; }

      public uint[] DispGroups { get; private set; }

      public int EntityID { get; set; }

      public byte LightID { get; set; }

      public byte FogID { get; set; }

      public byte ScatterID { get; set; }

      public byte LensFlareID { get; set; }

      public byte ShadowID { get; set; }

      public byte DofID { get; set; }

      public byte ToneMapID { get; set; }

      public byte ToneCorrectID { get; set; }

      public byte LanternID { get; set; }

      public byte LodParamID { get; set; }

      public byte IsShadowSrc { get; set; }

      public byte IsShadowDest { get; set; }

      public byte IsShadowOnly { get; set; }

      public byte DrawByReflectCam { get; set; }

      public byte DrawOnlyReflectCam { get; set; }

      public byte UseDepthBiasFloat { get; set; }

      public byte DisablePointLightEffect { get; set; }

      internal Part()
      {
        this.Name = "";
        this.Placeholder = "";
        this.Scale = Vector3.One;
        this.DrawGroups = new uint[4]
        {
          uint.MaxValue,
          uint.MaxValue,
          uint.MaxValue,
          uint.MaxValue
        };
        this.DispGroups = new uint[4]
        {
          uint.MaxValue,
          uint.MaxValue,
          uint.MaxValue,
          uint.MaxValue
        };
        this.EntityID = -1;
      }

      internal Part(BinaryReaderEx br)
      {
        long position = br.Position;
        int num1 = br.ReadInt32();
        int num2 = (int) br.AssertUInt32((uint) this.Type);
        br.ReadInt32();
        this.ModelIndex = br.ReadInt32();
        int num3 = br.ReadInt32();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        this.DrawGroups = br.ReadUInt32s(4);
        this.DispGroups = br.ReadUInt32s(4);
        int num4 = br.ReadInt32();
        int num5 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Name = br.GetShiftJIS(position + (long) num1);
        this.Placeholder = br.GetShiftJIS(position + (long) num3);
        br.Position = position + (long) num4;
        this.EntityID = br.ReadInt32();
        this.LightID = br.ReadByte();
        this.FogID = br.ReadByte();
        this.ScatterID = br.ReadByte();
        this.LensFlareID = br.ReadByte();
        this.ShadowID = br.ReadByte();
        this.DofID = br.ReadByte();
        this.ToneMapID = br.ReadByte();
        this.ToneCorrectID = br.ReadByte();
        this.LanternID = br.ReadByte();
        this.LodParamID = br.ReadByte();
        int num6 = (int) br.AssertByte(new byte[1]);
        this.IsShadowSrc = br.ReadByte();
        this.IsShadowDest = br.ReadByte();
        this.IsShadowOnly = br.ReadByte();
        this.DrawByReflectCam = br.ReadByte();
        this.DrawOnlyReflectCam = br.ReadByte();
        this.UseDepthBiasFloat = br.ReadByte();
        this.DisablePointLightEffect = br.ReadByte();
        int num7 = (int) br.AssertByte(new byte[1]);
        int num8 = (int) br.AssertByte(new byte[1]);
        br.Position = position + (long) num5;
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position1 = bw.Position;
        bw.ReserveInt32("NameOffset");
        bw.WriteUInt32((uint) this.Type);
        bw.WriteInt32(id);
        bw.WriteInt32(this.ModelIndex);
        bw.ReserveInt32("SibOffset");
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.WriteVector3(this.Scale);
        bw.WriteUInt32s((IList<uint>) this.DrawGroups);
        bw.WriteUInt32s((IList<uint>) this.DispGroups);
        bw.ReserveInt32("EntityDataOffset");
        bw.ReserveInt32("TypeDataOffset");
        bw.WriteInt32(0);
        long position2 = bw.Position;
        bw.FillInt32("NameOffset", (int) (bw.Position - position1));
        bw.WriteShiftJIS(MSB.ReambiguateName(this.Name), true);
        bw.FillInt32("SibOffset", (int) (bw.Position - position1));
        bw.WriteShiftJIS(this.Placeholder, true);
        bw.Pad(4);
        if (bw.Position - position2 < 20L)
          bw.WritePattern((int) (20L - (bw.Position - position2)), (byte) 0);
        bw.FillInt32("EntityDataOffset", (int) (bw.Position - position1));
        bw.WriteInt32(this.EntityID);
        bw.WriteByte(this.LightID);
        bw.WriteByte(this.FogID);
        bw.WriteByte(this.ScatterID);
        bw.WriteByte(this.LensFlareID);
        bw.WriteByte(this.ShadowID);
        bw.WriteByte(this.DofID);
        bw.WriteByte(this.ToneMapID);
        bw.WriteByte(this.ToneCorrectID);
        bw.WriteByte(this.LanternID);
        bw.WriteByte(this.LodParamID);
        bw.WriteByte((byte) 0);
        bw.WriteByte(this.IsShadowSrc);
        bw.WriteByte(this.IsShadowDest);
        bw.WriteByte(this.IsShadowOnly);
        bw.WriteByte(this.DrawByReflectCam);
        bw.WriteByte(this.DrawOnlyReflectCam);
        bw.WriteByte(this.UseDepthBiasFloat);
        bw.WriteByte(this.DisablePointLightEffect);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 0);
        bw.FillInt32("TypeDataOffset", (int) (bw.Position - position1));
      }

      internal virtual void GetNames(MSB1 msb, MSB1.Entries entries)
      {
        this.ModelName = MSB.FindName<MSB1.Model>(entries.Models, this.ModelIndex);
      }

      internal virtual void GetIndices(MSB1 msb, MSB1.Entries entries)
      {
        this.ModelIndex = MSB.FindIndex<MSB1.Model>(entries.Models, this.ModelName);
      }

      public class MapPiece : MSB1.Part
      {
        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.MapPiece;
          }
        }

        public MapPiece()
        {
        }

        internal MapPiece(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Object : MSB1.Part
      {
        private int CollisionIndex;

        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.Object;
          }
        }

        public string CollisionName { get; set; }

        public int UnkT08 { get; set; }

        public short UnkT0C { get; set; }

        public short UnkT0E { get; set; }

        public int UnkT10 { get; set; }

        public Object()
        {
        }

        internal Object(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          this.CollisionIndex = br.ReadInt32();
          this.UnkT08 = br.ReadInt32();
          this.UnkT0C = br.ReadInt16();
          this.UnkT0E = br.ReadInt16();
          this.UnkT10 = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(0);
          bw.WriteInt32(this.CollisionIndex);
          bw.WriteInt32(this.UnkT08);
          bw.WriteInt16(this.UnkT0C);
          bw.WriteInt16(this.UnkT0E);
          bw.WriteInt32(this.UnkT10);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSB1 msb, MSB1.Entries entries)
        {
          base.GetNames(msb, entries);
          this.CollisionName = MSB.FindName<MSB1.Part>(entries.Parts, this.CollisionIndex);
        }

        internal override void GetIndices(MSB1 msb, MSB1.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.CollisionIndex = MSB.FindIndex<MSB1.Part>(entries.Parts, this.CollisionName);
        }
      }

      public class Enemy : MSB1.Part
      {
        private int CollisionIndex;
        private short[] MovePointIndices;

        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.Enemy;
          }
        }

        public int ThinkParamID { get; set; }

        public int NPCParamID { get; set; }

        public int TalkID { get; set; }

        public float UnkT14 { get; set; }

        public int CharaInitID { get; set; }

        public string CollisionName { get; set; }

        public string[] MovePointNames { get; private set; }

        public int UnkT38 { get; set; }

        public int UnkT3C { get; set; }

        public Enemy()
        {
          this.ThinkParamID = -1;
          this.NPCParamID = -1;
          this.TalkID = -1;
          this.CharaInitID = -1;
          this.MovePointNames = new string[8];
        }

        internal Enemy(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.ThinkParamID = br.ReadInt32();
          this.NPCParamID = br.ReadInt32();
          this.TalkID = br.ReadInt32();
          this.UnkT14 = br.ReadSingle();
          this.CharaInitID = br.ReadInt32();
          this.CollisionIndex = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.MovePointIndices = br.ReadInt16s(8);
          this.UnkT38 = br.ReadInt32();
          this.UnkT3C = br.ReadInt32();
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.ThinkParamID);
          bw.WriteInt32(this.NPCParamID);
          bw.WriteInt32(this.TalkID);
          bw.WriteSingle(this.UnkT14);
          bw.WriteInt32(this.CharaInitID);
          bw.WriteInt32(this.CollisionIndex);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt16s((IList<short>) this.MovePointIndices);
          bw.WriteInt32(this.UnkT38);
          bw.WriteInt32(this.UnkT3C);
        }

        internal override void GetNames(MSB1 msb, MSB1.Entries entries)
        {
          base.GetNames(msb, entries);
          this.CollisionName = MSB.FindName<MSB1.Part>(entries.Parts, this.CollisionIndex);
          this.MovePointNames = new string[this.MovePointIndices.Length];
          for (int index = 0; index < this.MovePointIndices.Length; ++index)
            this.MovePointNames[index] = MSB.FindName<MSB1.Region>(entries.Regions, (int) this.MovePointIndices[index]);
        }

        internal override void GetIndices(MSB1 msb, MSB1.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.CollisionIndex = MSB.FindIndex<MSB1.Part>(entries.Parts, this.CollisionName);
          this.MovePointIndices = new short[this.MovePointNames.Length];
          for (int index = 0; index < this.MovePointNames.Length; ++index)
            this.MovePointIndices[index] = (short) MSB.FindIndex<MSB1.Region>(entries.Regions, this.MovePointNames[index]);
        }
      }

      public class Player : MSB1.Part
      {
        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.Player;
          }
        }

        public Player()
        {
        }

        internal Player(BinaryReaderEx br)
          : base(br)
        {
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Collision : MSB1.Part
      {
        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.Collision;
          }
        }

        public byte HitFilterID { get; set; }

        public byte SoundSpaceType { get; set; }

        public short EnvLightMapSpotIndex { get; set; }

        public float ReflectPlaneHeight { get; set; }

        public uint[] NvmGroups { get; private set; }

        public int[] VagrantEntityIDs { get; private set; }

        public short MapNameID { get; set; }

        public short DisableStart { get; set; }

        public int DisableBonfireEntityID { get; set; }

        public int PlayRegionID { get; set; }

        public short LockCamParamID1 { get; set; }

        public short LockCamParamID2 { get; set; }

        public Collision()
        {
          this.NvmGroups = new uint[4]
          {
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue
          };
          this.VagrantEntityIDs = new int[3]{ -1, -1, -1 };
          this.MapNameID = (short) -1;
          this.DisableBonfireEntityID = -1;
          this.LockCamParamID1 = (short) -1;
          this.LockCamParamID2 = (short) -1;
        }

        internal Collision(BinaryReaderEx br)
          : base(br)
        {
          this.HitFilterID = br.ReadByte();
          this.SoundSpaceType = br.ReadByte();
          this.EnvLightMapSpotIndex = br.ReadInt16();
          this.ReflectPlaneHeight = br.ReadSingle();
          this.NvmGroups = br.ReadUInt32s(4);
          this.VagrantEntityIDs = br.ReadInt32s(3);
          this.MapNameID = br.ReadInt16();
          this.DisableStart = br.ReadInt16();
          this.DisableBonfireEntityID = br.ReadInt32();
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          br.AssertInt32(-1);
          this.PlayRegionID = br.ReadInt32();
          this.LockCamParamID1 = br.ReadInt16();
          this.LockCamParamID2 = br.ReadInt16();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteByte(this.HitFilterID);
          bw.WriteByte(this.SoundSpaceType);
          bw.WriteInt16(this.EnvLightMapSpotIndex);
          bw.WriteSingle(this.ReflectPlaneHeight);
          bw.WriteUInt32s((IList<uint>) this.NvmGroups);
          bw.WriteInt32s((IList<int>) this.VagrantEntityIDs);
          bw.WriteInt16(this.MapNameID);
          bw.WriteInt16(this.DisableStart);
          bw.WriteInt32(this.DisableBonfireEntityID);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(-1);
          bw.WriteInt32(this.PlayRegionID);
          bw.WriteInt16(this.LockCamParamID1);
          bw.WriteInt16(this.LockCamParamID2);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Navmesh : MSB1.Part
      {
        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.Navmesh;
          }
        }

        public uint[] NvmGroups { get; private set; }

        public Navmesh()
        {
          this.NvmGroups = new uint[4]
          {
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue
          };
        }

        internal Navmesh(BinaryReaderEx br)
          : base(br)
        {
          this.NvmGroups = br.ReadUInt32s(4);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteUInt32s((IList<uint>) this.NvmGroups);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class DummyObject : MSB1.Part.Object
      {
        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.DummyObject;
          }
        }

        public DummyObject()
        {
        }

        internal DummyObject(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class DummyEnemy : MSB1.Part.Enemy
      {
        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.DummyEnemy;
          }
        }

        public DummyEnemy()
        {
        }

        internal DummyEnemy(BinaryReaderEx br)
          : base(br)
        {
        }
      }

      public class ConnectCollision : MSB1.Part
      {
        private int CollisionIndex;

        public override MSB1.PartType Type
        {
          get
          {
            return MSB1.PartType.ConnectCollision;
          }
        }

        public string CollisionName { get; set; }

        public byte[] MapID { get; private set; }

        public ConnectCollision()
        {
          this.MapID = new byte[4]
          {
            (byte) 10,
            (byte) 2,
            (byte) 0,
            (byte) 0
          };
        }

        internal ConnectCollision(BinaryReaderEx br)
          : base(br)
        {
          this.CollisionIndex = br.ReadInt32();
          this.MapID = br.ReadBytes(4);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(BinaryWriterEx bw, int id)
        {
          base.Write(bw, id);
          bw.WriteInt32(this.CollisionIndex);
          bw.WriteBytes(this.MapID);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }

        internal override void GetNames(MSB1 msb, MSB1.Entries entries)
        {
          base.GetNames(msb, entries);
          this.CollisionName = MSB.FindName<MSB1.Part.Collision>(msb.Parts.Collisions, this.CollisionIndex);
        }

        internal override void GetIndices(MSB1 msb, MSB1.Entries entries)
        {
          base.GetIndices(msb, entries);
          this.CollisionIndex = MSB.FindIndex<MSB1.Part.Collision>(msb.Parts.Collisions, this.CollisionName);
        }
      }
    }

    public class PointParam : MSB1.Param<MSB1.Region>, IMsbParam<IMsbRegion>
    {
      internal override string Name
      {
        get
        {
          return "POINT_PARAM_ST";
        }
      }

      public List<MSB1.Region> Regions { get; set; }

      public PointParam()
      {
        this.Regions = new List<MSB1.Region>();
      }

      public override List<MSB1.Region> GetEntries()
      {
        return this.Regions;
      }

      IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries()
      {
        return (IReadOnlyList<IMsbRegion>) this.GetEntries();
      }

      internal override MSB1.Region ReadEntry(BinaryReaderEx br)
      {
        MSB1.Region region = new MSB1.Region(br);
        this.Regions.Add(region);
        return region;
      }
    }

    public class Region : MSB1.Entry, IMsbRegion, IMsbEntry
    {
      public MSB1.Shape Shape { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      public int EntityID { get; set; }

      public Region()
      {
        this.Name = "";
        this.Shape = (MSB1.Shape) new MSB1.Shape.Point();
        this.EntityID = -1;
      }

      internal Region(BinaryReaderEx br)
      {
        long position = br.Position;
        int num1 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.ReadInt32();
        MSB1.ShapeType shapeType = br.ReadEnum32<MSB1.ShapeType>();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        int num2 = br.ReadInt32();
        int num3 = br.ReadInt32();
        int num4 = br.ReadInt32();
        int num5 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Name = br.GetShiftJIS(position + (long) num1);
        br.Position = position + (long) num2;
        br.AssertInt32(new int[1]);
        br.Position = position + (long) num3;
        br.AssertInt32(new int[1]);
        br.Position = position + (long) num4;
        switch (shapeType)
        {
          case MSB1.ShapeType.Point:
            this.Shape = (MSB1.Shape) new MSB1.Shape.Point();
            break;
          case MSB1.ShapeType.Circle:
            this.Shape = (MSB1.Shape) new MSB1.Shape.Circle(br);
            break;
          case MSB1.ShapeType.Sphere:
            this.Shape = (MSB1.Shape) new MSB1.Shape.Sphere(br);
            break;
          case MSB1.ShapeType.Cylinder:
            this.Shape = (MSB1.Shape) new MSB1.Shape.Cylinder(br);
            break;
          case MSB1.ShapeType.Rect:
            this.Shape = (MSB1.Shape) new MSB1.Shape.Rect(br);
            break;
          case MSB1.ShapeType.Box:
            this.Shape = (MSB1.Shape) new MSB1.Shape.Box(br);
            break;
          default:
            throw new NotImplementedException(string.Format("Unimplemented shape type: {0}", (object) shapeType));
        }
        br.Position = position + (long) num5;
        this.EntityID = br.ReadInt32();
      }

      internal override void Write(BinaryWriterEx bw, int id)
      {
        long position = bw.Position;
        bw.ReserveInt32("NameOffset");
        bw.WriteInt32(0);
        bw.WriteInt32(id);
        bw.WriteUInt32((uint) this.Shape.Type);
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.ReserveInt32("UnkOffsetA");
        bw.ReserveInt32("UnkOffsetB");
        bw.ReserveInt32("ShapeDataOffset");
        bw.ReserveInt32("EntityDataOffset");
        bw.WriteInt32(0);
        bw.FillInt32("NameOffset", (int) (bw.Position - position));
        bw.WriteShiftJIS(MSB.ReambiguateName(this.Name), true);
        bw.Pad(4);
        bw.FillInt32("UnkOffsetA", (int) (bw.Position - position));
        bw.WriteInt32(0);
        bw.FillInt32("UnkOffsetB", (int) (bw.Position - position));
        bw.WriteInt32(0);
        if (this.Shape.HasShapeData)
        {
          bw.FillInt32("ShapeDataOffset", (int) (bw.Position - position));
          this.Shape.WriteShapeData(bw);
        }
        else
          bw.FillInt32("ShapeDataOffset", 0);
        bw.FillInt32("EntityDataOffset", (int) (bw.Position - position));
        bw.WriteInt32(this.EntityID);
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
    }

    public abstract class Shape
    {
      public abstract MSB1.ShapeType Type { get; }

      internal abstract bool HasShapeData { get; }

      internal virtual void WriteShapeData(BinaryWriterEx bw)
      {
        throw new InvalidOperationException("Shape data should not be written for shapes with no shape data.");
      }

      public class Point : MSB1.Shape
      {
        public override MSB1.ShapeType Type
        {
          get
          {
            return MSB1.ShapeType.Point;
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

      public class Circle : MSB1.Shape
      {
        public override MSB1.ShapeType Type
        {
          get
          {
            return MSB1.ShapeType.Circle;
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

      public class Sphere : MSB1.Shape
      {
        public override MSB1.ShapeType Type
        {
          get
          {
            return MSB1.ShapeType.Sphere;
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

      public class Cylinder : MSB1.Shape
      {
        public override MSB1.ShapeType Type
        {
          get
          {
            return MSB1.ShapeType.Cylinder;
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

      public class Rect : MSB1.Shape
      {
        public override MSB1.ShapeType Type
        {
          get
          {
            return MSB1.ShapeType.Rect;
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

      public class Box : MSB1.Shape
      {
        public override MSB1.ShapeType Type
        {
          get
          {
            return MSB1.ShapeType.Box;
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
    }
  }
}
