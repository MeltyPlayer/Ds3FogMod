// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MSB2
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class MSB2 : SoulsFile<MSB2>, IMsb {
    public MSB2.ModelParam Models { get; set; }

    IMsbParam<IMsbModel> IMsb.Models {
      get { return (IMsbParam<IMsbModel>) this.Models; }
    }

    public MSB2.EventParam Events { get; set; }

    public MSB2.PointParam Regions { get; set; }

    IMsbParam<IMsbRegion> IMsb.Regions {
      get { return (IMsbParam<IMsbRegion>) this.Regions; }
    }

    public MSB2.PartsParam Parts { get; set; }

    IMsbParam<IMsbPart> IMsb.Parts {
      get { return (IMsbParam<IMsbPart>) this.Parts; }
    }

    public List<MSB2.PartPose> PartPoses { get; set; }

    public MSB2() {
      this.Models = new MSB2.ModelParam();
      this.Events = new MSB2.EventParam();
      this.Regions = new MSB2.PointParam();
      this.Parts = new MSB2.PartsParam();
      this.PartPoses = new List<MSB2.PartPose>();
    }

    protected override bool Is(BinaryReaderEx br) {
      if (br.Length < 20L)
        return false;
      br.BigEndian = false;
      string ascii = br.GetASCII(0L, 4);
      int int32 = br.GetInt32(16L);
      return ascii == "MSB " && int32 == 5;
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertASCII("MSB ");
      br.AssertInt32(1);
      br.AssertInt32(16);
      br.AssertBoolean(false);
      br.AssertBoolean(false);
      int num1 = (int) br.AssertByte((byte) 1);
      int num2 = (int) br.AssertSByte((sbyte) -1);
      this.Models = new MSB2.ModelParam();
      MSB2.Entries entries;
      entries.Models = this.Models.Read(br);
      this.Events = new MSB2.EventParam();
      entries.Events = this.Events.Read(br);
      this.Regions = new MSB2.PointParam();
      entries.Regions = this.Regions.Read(br);
      new MSB2.RouteParam().Read(br);
      new MSB2.LayerParam().Read(br);
      this.Parts = new MSB2.PartsParam();
      entries.Parts = this.Parts.Read(br);
      this.PartPoses = new MSB2.MapstudioPartsPose().Read(br);
      entries.BoneNames = new MSB2.MapstudioBoneName().Read(br);
      if (br.Position != 0L)
        throw new InvalidDataException(
            string.Format(
                "The next param offset of the final param should be 0, but it was 0x{0:X}.",
                (object) br.Position));
      MSB.DisambiguateNames<MSB2.Model>(entries.Models);
      MSB.DisambiguateNames<MSB2.Part>(entries.Parts);
      MSB.DisambiguateNames<MSB2.BoneName>(entries.BoneNames);
      foreach (MSB2.Part part in entries.Parts)
        part.GetNames(this, entries);
      foreach (MSB2.PartPose partPose in this.PartPoses)
        partPose.GetNames(entries);
    }

    protected override void Write(BinaryWriterEx bw) {
      MSB2.Entries entries;
      entries.Models = this.Models.GetEntries();
      entries.Events = this.Events.GetEntries();
      entries.Regions = this.Regions.GetEntries();
      entries.Parts = this.Parts.GetEntries();
      entries.BoneNames = new List<MSB2.BoneName>();
      MSB2.Lookups lookups;
      lookups.Models = MSB2.MakeNameLookup<MSB2.Model>(entries.Models);
      lookups.Parts = MSB2.MakeNameLookup<MSB2.Part>(entries.Parts);
      lookups.Collisions =
          MSB2.MakeNameLookup<MSB2.Part.Collision>(this.Parts.Collisions);
      lookups.BoneNames = new Dictionary<string, int>();
      this.Models.DiscriminateModels();
      foreach (MSB2.Part part in entries.Parts)
        part.GetIndices(lookups);
      foreach (MSB2.PartPose partPose in this.PartPoses)
        partPose.GetIndices(lookups, entries);
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
      new MSB2.RouteParam().Write(bw, new List<MSB2.Entry>());
      bw.FillInt64("NextParamOffset", bw.Position);
      new MSB2.LayerParam().Write(bw, new List<MSB2.Entry>());
      bw.FillInt64("NextParamOffset", bw.Position);
      this.Parts.Write(bw, entries.Parts);
      bw.FillInt64("NextParamOffset", bw.Position);
      new MSB2.MapstudioPartsPose().Write(bw, this.PartPoses);
      bw.FillInt64("NextParamOffset", bw.Position);
      new MSB2.MapstudioBoneName().Write(bw, entries.BoneNames);
      bw.FillInt64("NextParamOffset", 0L);
    }

    private static int FindIndex(Dictionary<string, int> lookup, string name) {
      if (name == null)
        return -1;
      if (!lookup.ContainsKey(name))
        throw new KeyNotFoundException("Name not found: " + name);
      return lookup[name];
    }

    private static Dictionary<string, int> MakeNameLookup<T>(List<T> list)
        where T : MSB2.NamedEntry {
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      for (int index = 0; index < list.Count; ++index)
        dictionary[list[index].Name] = index;
      return dictionary;
    }

    public enum EventType : ushort {
      Light = 1,
      Shadow = 2,
      Fog = 3,
      BGColor = 4,
      MapOffset = 5,
      Warp = 6,
      CheapMode = 7,
    }

    public class EventParam : MSB2.Param<MSB2.Event> {
      internal override string Name {
        get { return "EVENT_PARAM_ST"; }
      }

      internal override int Version {
        get { return 5; }
      }

      public List<MSB2.Event.Light> Lights { get; set; }

      public List<MSB2.Event.Shadow> Shadows { get; set; }

      public List<MSB2.Event.Fog> Fogs { get; set; }

      public List<MSB2.Event.BGColor> BGColors { get; set; }

      public List<MSB2.Event.MapOffset> MapOffsets { get; set; }

      public List<MSB2.Event.Warp> Warps { get; set; }

      public List<MSB2.Event.CheapMode> CheapModes { get; set; }

      public EventParam() {
        this.Lights = new List<MSB2.Event.Light>();
        this.Shadows = new List<MSB2.Event.Shadow>();
        this.Fogs = new List<MSB2.Event.Fog>();
        this.BGColors = new List<MSB2.Event.BGColor>();
        this.MapOffsets = new List<MSB2.Event.MapOffset>();
        this.Warps = new List<MSB2.Event.Warp>();
        this.CheapModes = new List<MSB2.Event.CheapMode>();
      }

      internal override MSB2.Event ReadEntry(BinaryReaderEx br) {
        MSB2.EventType enum16 = br.GetEnum16<MSB2.EventType>(br.Position + 12L);
        switch (enum16) {
          case MSB2.EventType.Light:
            MSB2.Event.Light light = new MSB2.Event.Light(br);
            this.Lights.Add(light);
            return (MSB2.Event) light;
          case MSB2.EventType.Shadow:
            MSB2.Event.Shadow shadow = new MSB2.Event.Shadow(br);
            this.Shadows.Add(shadow);
            return (MSB2.Event) shadow;
          case MSB2.EventType.Fog:
            MSB2.Event.Fog fog = new MSB2.Event.Fog(br);
            this.Fogs.Add(fog);
            return (MSB2.Event) fog;
          case MSB2.EventType.BGColor:
            MSB2.Event.BGColor bgColor = new MSB2.Event.BGColor(br);
            this.BGColors.Add(bgColor);
            return (MSB2.Event) bgColor;
          case MSB2.EventType.MapOffset:
            MSB2.Event.MapOffset mapOffset = new MSB2.Event.MapOffset(br);
            this.MapOffsets.Add(mapOffset);
            return (MSB2.Event) mapOffset;
          case MSB2.EventType.Warp:
            MSB2.Event.Warp warp = new MSB2.Event.Warp(br);
            this.Warps.Add(warp);
            return (MSB2.Event) warp;
          case MSB2.EventType.CheapMode:
            MSB2.Event.CheapMode cheapMode = new MSB2.Event.CheapMode(br);
            this.CheapModes.Add(cheapMode);
            return (MSB2.Event) cheapMode;
          default:
            throw new NotImplementedException(
                string.Format("Unimplemented event type: {0}",
                              (object) enum16));
        }
      }

      public override List<MSB2.Event> GetEntries() {
        return SFUtil.ConcatAll<MSB2.Event>(new IEnumerable<MSB2.Event>[7] {
            (IEnumerable<MSB2.Event>) this.Lights,
            (IEnumerable<MSB2.Event>) this.Shadows,
            (IEnumerable<MSB2.Event>) this.Fogs,
            (IEnumerable<MSB2.Event>) this.BGColors,
            (IEnumerable<MSB2.Event>) this.MapOffsets,
            (IEnumerable<MSB2.Event>) this.Warps,
            (IEnumerable<MSB2.Event>) this.CheapModes
        });
      }
    }

    public abstract class Event : MSB2.NamedEntry {
      public abstract MSB2.EventType Type { get; }

      public int EventID { get; set; }

      internal Event(string name = "") {
        this.Name = name;
        this.EventID = -1;
      }

      internal Event(BinaryReaderEx br) {
        long position = br.Position;
        long num1 = br.ReadInt64();
        this.EventID = br.ReadInt32();
        int num2 = (int) br.AssertUInt16((ushort) this.Type);
        int num3 = (int) br.ReadInt16();
        long num4 = br.ReadInt64();
        this.Name = br.GetUTF16(position + num1);
        br.Position = position + num4;
        this.ReadTypeData(br);
      }

      internal abstract void ReadTypeData(BinaryReaderEx br);

      internal override void Write(BinaryWriterEx bw, int index) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteInt32(this.EventID);
        bw.WriteUInt16((ushort) this.Type);
        bw.WriteInt16((short) index);
        bw.ReserveInt64("TypeDataOffset");
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(this.Name, true);
        bw.Pad(8);
        bw.FillInt64("TypeDataOffset", bw.Position - position);
        this.WriteTypeData(bw);
      }

      internal abstract void WriteTypeData(BinaryWriterEx bw);

      public override string ToString() {
        return string.Format("[ID {0}] {1} \"{2}\"",
                             (object) this.EventID,
                             (object) this.Type,
                             (object) this.Name);
      }

      public class Light : MSB2.Event {
        public override MSB2.EventType Type {
          get { return MSB2.EventType.Light; }
        }

        public short UnkT00 { get; set; }

        public float UnkT04 { get; set; }

        public float UnkT08 { get; set; }

        public Color ColorT0C { get; set; }

        public Color ColorT10 { get; set; }

        public float UnkT1C { get; set; }

        public float UnkT20 { get; set; }

        public Color ColorT24 { get; set; }

        public Color ColorT28 { get; set; }

        public Color ColorT34 { get; set; }

        public Color ColorT38 { get; set; }

        public Color ColorT3C { get; set; }

        public float UnkT40 { get; set; }

        public int UnkT44 { get; set; }

        public Light(string name = "")
            : base(name) {}

        internal Light(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt16();
          int num = (int) br.AssertInt16((short) -1);
          this.UnkT04 = br.ReadSingle();
          this.UnkT08 = br.ReadSingle();
          this.ColorT0C = br.ReadRGBA();
          this.ColorT10 = br.ReadRGBA();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.UnkT1C = br.ReadSingle();
          this.UnkT20 = br.ReadSingle();
          this.ColorT24 = br.ReadRGBA();
          this.ColorT28 = br.ReadRGBA();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.ColorT34 = br.ReadRGBA();
          this.ColorT38 = br.ReadRGBA();
          this.ColorT3C = br.ReadRGBA();
          this.UnkT40 = br.ReadSingle();
          this.UnkT44 = br.ReadInt32();
          br.AssertPattern(56, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt16(this.UnkT00);
          bw.WriteInt16((short) -1);
          bw.WriteSingle(this.UnkT04);
          bw.WriteSingle(this.UnkT08);
          bw.WriteRGBA(this.ColorT0C);
          bw.WriteRGBA(this.ColorT10);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteSingle(this.UnkT1C);
          bw.WriteSingle(this.UnkT20);
          bw.WriteRGBA(this.ColorT24);
          bw.WriteRGBA(this.ColorT28);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteRGBA(this.ColorT34);
          bw.WriteRGBA(this.ColorT38);
          bw.WriteRGBA(this.ColorT3C);
          bw.WriteSingle(this.UnkT40);
          bw.WriteInt32(this.UnkT44);
          bw.WritePattern(56, (byte) 0);
        }
      }

      public class Shadow : MSB2.Event {
        public override MSB2.EventType Type {
          get { return MSB2.EventType.Shadow; }
        }

        public int UnkT00 { get; set; }

        public float UnkT04 { get; set; }

        public float UnkT08 { get; set; }

        public float UnkT0C { get; set; }

        public int UnkT10 { get; set; }

        public Color ColorT14 { get; set; }

        public float UnkT18 { get; set; }

        public int UnkT1C { get; set; }

        public float UnkT20 { get; set; }

        public Color ColorT24 { get; set; }

        public Shadow(string name = "")
            : base(name) {}

        internal Shadow(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadSingle();
          this.UnkT08 = br.ReadSingle();
          this.UnkT0C = br.ReadSingle();
          this.UnkT10 = br.ReadInt32();
          this.ColorT14 = br.ReadRGBA();
          this.UnkT18 = br.ReadSingle();
          this.UnkT1C = br.ReadInt32();
          this.UnkT20 = br.ReadSingle();
          this.ColorT24 = br.ReadRGBA();
          br.AssertPattern(24, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteSingle(this.UnkT04);
          bw.WriteSingle(this.UnkT08);
          bw.WriteSingle(this.UnkT0C);
          bw.WriteInt32(this.UnkT10);
          bw.WriteRGBA(this.ColorT14);
          bw.WriteSingle(this.UnkT18);
          bw.WriteInt32(this.UnkT1C);
          bw.WriteSingle(this.UnkT20);
          bw.WriteRGBA(this.ColorT24);
          bw.WritePattern(24, (byte) 0);
        }
      }

      public class Fog : MSB2.Event {
        public override MSB2.EventType Type {
          get { return MSB2.EventType.Fog; }
        }

        public int UnkT00 { get; set; }

        public Color ColorT04 { get; set; }

        public float UnkT08 { get; set; }

        public float UnkT0C { get; set; }

        public float UnkT10 { get; set; }

        public int UnkT14 { get; set; }

        public Fog(string name = "")
            : base(name) {}

        internal Fog(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.ColorT04 = br.ReadRGBA();
          this.UnkT08 = br.ReadSingle();
          this.UnkT0C = br.ReadSingle();
          this.UnkT10 = br.ReadSingle();
          this.UnkT14 = br.ReadInt32();
          br.AssertPattern(16, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteRGBA(this.ColorT04);
          bw.WriteSingle(this.UnkT08);
          bw.WriteSingle(this.UnkT0C);
          bw.WriteSingle(this.UnkT10);
          bw.WriteInt32(this.UnkT14);
          bw.WritePattern(16, (byte) 0);
        }
      }

      public class BGColor : MSB2.Event {
        public override MSB2.EventType Type {
          get { return MSB2.EventType.BGColor; }
        }

        public Color Color { get; set; }

        public BGColor(string name = "")
            : base(name) {}

        internal BGColor(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.Color = br.ReadRGBA();
          br.AssertPattern(36, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteRGBA(this.Color);
          bw.WritePattern(36, (byte) 0);
        }
      }

      public class MapOffset : MSB2.Event {
        public override MSB2.EventType Type {
          get { return MSB2.EventType.MapOffset; }
        }

        public Vector3 Translation { get; set; }

        public MapOffset(string name = "")
            : base(name) {}

        internal MapOffset(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.Translation = br.ReadVector3();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteVector3(this.Translation);
          bw.WriteInt32(0);
        }
      }

      public class Warp : MSB2.Event {
        public override MSB2.EventType Type {
          get { return MSB2.EventType.Warp; }
        }

        public int UnkT00 { get; set; }

        public Vector3 Position { get; set; }

        public Warp(string name = "")
            : base(name) {}

        internal Warp(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.Position = br.ReadVector3();
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteVector3(this.Position);
        }
      }

      public class CheapMode : MSB2.Event {
        public override MSB2.EventType Type {
          get { return MSB2.EventType.CheapMode; }
        }

        public int UnkT00 { get; set; }

        public CheapMode(string name = "")
            : base(name) {}

        internal CheapMode(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          br.AssertPattern(12, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WritePattern(12, (byte) 0);
        }
      }
    }

    private class LayerParam : MSB2.Param<MSB2.Entry> {
      internal override string Name {
        get { return "LAYER_PARAM_ST"; }
      }

      internal override int Version {
        get { return 5; }
      }

      internal override MSB2.Entry ReadEntry(BinaryReaderEx br) {
        throw new NotSupportedException(
            "Layer param should always be empty in DS2.");
      }

      public override List<MSB2.Entry> GetEntries() {
        throw new NotSupportedException(
            "Layer param should always be empty in DS2.");
      }
    }

    private class MapstudioBoneName : MSB2.Param<MSB2.BoneName> {
      internal override string Name {
        get { return "MAPSTUDIO_BONE_NAME_STRING"; }
      }

      internal override int Version {
        get { return 0; }
      }

      public List<MSB2.BoneName> BoneNames { get; set; }

      public MapstudioBoneName() {
        this.BoneNames = new List<MSB2.BoneName>();
      }

      internal override MSB2.BoneName ReadEntry(BinaryReaderEx br) {
        MSB2.BoneName boneName = new MSB2.BoneName(br);
        this.BoneNames.Add(boneName);
        return boneName;
      }

      public override List<MSB2.BoneName> GetEntries() {
        return this.BoneNames;
      }
    }

    internal class BoneName : MSB2.NamedEntry {
      public BoneName(string name) {
        this.Name = name;
      }

      internal BoneName(BinaryReaderEx br) {
        this.Name = br.ReadUTF16();
      }

      internal override void Write(BinaryWriterEx bw, int index) {
        bw.WriteUTF16(MSB.ReambiguateName(this.Name), true);
      }
    }

    private class MapstudioPartsPose : MSB2.Param<MSB2.PartPose> {
      internal override string Name {
        get { return "MAPSTUDIO_PARTS_POSE_ST"; }
      }

      internal override int Version {
        get { return 0; }
      }

      public List<MSB2.PartPose> Poses { get; set; }

      public MapstudioPartsPose() {
        this.Poses = new List<MSB2.PartPose>();
      }

      internal override MSB2.PartPose ReadEntry(BinaryReaderEx br) {
        MSB2.PartPose partPose = new MSB2.PartPose(br);
        this.Poses.Add(partPose);
        return partPose;
      }

      public override List<MSB2.PartPose> GetEntries() {
        return this.Poses;
      }
    }

    public class PartPose : MSB2.Entry {
      private short PartIndex;

      public string PartName { get; set; }

      public List<MSB2.PartPose.Bone> Bones { get; set; }

      public PartPose(string partName = null) {
        this.PartName = partName;
        this.Bones = new List<MSB2.PartPose.Bone>();
      }

      internal PartPose(BinaryReaderEx br) {
        this.PartIndex = br.ReadInt16();
        short num = br.ReadInt16();
        br.AssertInt32(new int[1]);
        br.AssertInt64(16L);
        this.Bones = new List<MSB2.PartPose.Bone>((int) num);
        for (int index = 0; index < (int) num; ++index)
          this.Bones.Add(new MSB2.PartPose.Bone(br));
      }

      internal override void Write(BinaryWriterEx bw, int index) {
        bw.WriteInt16(this.PartIndex);
        bw.WriteInt16((short) this.Bones.Count);
        bw.WriteInt32(0);
        bw.WriteInt64(16L);
        foreach (MSB2.PartPose.Bone bone in this.Bones)
          bone.Write(bw);
      }

      internal void GetNames(MSB2.Entries entries) {
        this.PartName =
            MSB.FindName<MSB2.Part>(entries.Parts, (int) this.PartIndex);
        foreach (MSB2.PartPose.Bone bone in this.Bones)
          bone.GetNames(entries);
      }

      internal void GetIndices(MSB2.Lookups lookups, MSB2.Entries entries) {
        this.PartIndex = (short) MSB2.FindIndex(lookups.Parts, this.PartName);
        foreach (MSB2.PartPose.Bone bone in this.Bones)
          bone.GetIndices(lookups, entries);
      }

      public override string ToString() {
        return string.Format("{0} [{1} Bones]",
                             (object) this.PartName,
                             (object) this.Bones?.Count);
      }

      public class Bone {
        private int NameIndex;

        public string Name { get; set; }

        public Vector3 Translation { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public Bone(string name = "") {
          this.Name = name;
          this.Scale = Vector3.One;
        }

        internal Bone(BinaryReaderEx br) {
          this.NameIndex = br.ReadInt32();
          this.Translation = br.ReadVector3();
          this.Rotation = br.ReadVector3();
          this.Scale = br.ReadVector3();
        }

        internal void Write(BinaryWriterEx bw) {
          bw.WriteInt32(this.NameIndex);
          bw.WriteVector3(this.Translation);
          bw.WriteVector3(this.Rotation);
          bw.WriteVector3(this.Scale);
        }

        internal void GetNames(MSB2.Entries entries) {
          this.Name =
              MSB.FindName<MSB2.BoneName>(entries.BoneNames, this.NameIndex);
        }

        internal void GetIndices(MSB2.Lookups lookups, MSB2.Entries entries) {
          if (!lookups.BoneNames.ContainsKey(this.Name)) {
            lookups.BoneNames[this.Name] = entries.BoneNames.Count;
            entries.BoneNames.Add(new MSB2.BoneName(this.Name));
          }
          this.NameIndex = MSB2.FindIndex(lookups.BoneNames, this.Name);
        }

        public override string ToString() {
          return string.Format("{0} [Trans {1:F2} | Rot {2:F2} | Scale {3:F2}]",
                               (object) this.Name,
                               (object) this.Translation,
                               (object) this.Rotation,
                               (object) this.Scale);
        }
      }
    }

    internal enum ModelType : ushort {
      MapPiece = 0,
      Object = 1,
      Collision = 3,
      Navmesh = 4,
    }

    public class ModelParam : MSB2.Param<MSB2.Model>, IMsbParam<IMsbModel> {
      internal override string Name {
        get { return "MODEL_PARAM_ST"; }
      }

      internal override int Version {
        get { return 5; }
      }

      public List<MSB2.Model> MapPieces { get; set; }

      public List<MSB2.Model> Objects { get; set; }

      public List<MSB2.Model> Collisions { get; set; }

      public List<MSB2.Model> Navmeshes { get; set; }

      public ModelParam() {
        this.MapPieces = new List<MSB2.Model>();
        this.Objects = new List<MSB2.Model>();
        this.Collisions = new List<MSB2.Model>();
        this.Navmeshes = new List<MSB2.Model>();
      }

      internal override MSB2.Model ReadEntry(BinaryReaderEx br) {
        MSB2.Model model = new MSB2.Model(br);
        switch (model.Type) {
          case MSB2.ModelType.MapPiece:
            this.MapPieces.Add(model);
            return model;
          case MSB2.ModelType.Object:
            this.Objects.Add(model);
            return model;
          case MSB2.ModelType.Collision:
            this.Collisions.Add(model);
            return model;
          case MSB2.ModelType.Navmesh:
            this.Navmeshes.Add(model);
            return model;
          default:
            throw new NotImplementedException(
                string.Format("Unimplemented model type: {0}",
                              (object) model.Type));
        }
      }

      public override List<MSB2.Model> GetEntries() {
        return SFUtil.ConcatAll<MSB2.Model>(new IEnumerable<MSB2.Model>[4] {
            (IEnumerable<MSB2.Model>) this.MapPieces,
            (IEnumerable<MSB2.Model>) this.Objects,
            (IEnumerable<MSB2.Model>) this.Collisions,
            (IEnumerable<MSB2.Model>) this.Navmeshes
        });
      }

      IReadOnlyList<IMsbModel> IMsbParam<IMsbModel>.GetEntries() {
        return (IReadOnlyList<IMsbModel>) this.GetEntries();
      }

      internal void DiscriminateModels() {
        for (short index = 0; (int) index < this.MapPieces.Count; ++index)
          this.MapPieces[(int) index]
              .Discriminate(MSB2.ModelType.MapPiece, index);
        for (short index = 0; (int) index < this.Objects.Count; ++index)
          this.Objects[(int) index].Discriminate(MSB2.ModelType.Object, index);
        for (short index = 0; (int) index < this.Collisions.Count; ++index)
          this.Collisions[(int) index]
              .Discriminate(MSB2.ModelType.Collision, index);
        for (short index = 0; (int) index < this.Navmeshes.Count; ++index)
          this.Navmeshes[(int) index]
              .Discriminate(MSB2.ModelType.Navmesh, index);
      }
    }

    public class Model : MSB2.NamedEntry, IMsbModel, IMsbEntry {
      internal MSB2.ModelType Type;
      internal short Index;

      public Model(string name = "") {
        this.Name = name;
      }

      internal Model(BinaryReaderEx br) {
        long position = br.Position;
        long num1 = br.ReadInt64();
        this.Type = br.ReadEnum16<MSB2.ModelType>();
        int num2 = (int) br.ReadInt16();
        br.AssertInt32(new int[1]);
        long num3 = br.ReadInt64();
        br.AssertInt64(new long[1]);
        this.Name = br.GetUTF16(position + num1);
        if (this.Type != MSB2.ModelType.Object)
          return;
        br.Position = position + num3;
        br.AssertInt64(new long[1]);
      }

      internal override void Write(BinaryWriterEx bw, int index) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteUInt16((ushort) this.Type);
        bw.WriteInt16(this.Index);
        bw.WriteInt32(0);
        bw.ReserveInt64("TypeDataOffset");
        bw.WriteInt64(0L);
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(MSB.ReambiguateName(this.Name), true);
        bw.Pad(8);
        if (this.Type == MSB2.ModelType.Object) {
          bw.FillInt64("TypeDataOffset", bw.Position - position);
          bw.WriteInt64(0L);
        } else
          bw.FillInt64("TypeDataOffset", 0L);
      }

      internal void Discriminate(MSB2.ModelType type, short index) {
        this.Type = type;
        this.Index = index;
      }

      public override string ToString() {
        return this.Name ?? "";
      }
    }

    internal struct Entries {
      public List<MSB2.Model> Models;
      public List<MSB2.Event> Events;
      public List<MSB2.Region> Regions;
      public List<MSB2.Part> Parts;
      public List<MSB2.BoneName> BoneNames;
    }

    internal struct Lookups {
      public Dictionary<string, int> Models;
      public Dictionary<string, int> Parts;
      public Dictionary<string, int> Collisions;
      public Dictionary<string, int> BoneNames;
    }

    public abstract class Entry {
      internal abstract void Write(BinaryWriterEx bw, int index);
    }

    public abstract class NamedEntry : MSB2.Entry, IMsbEntry {
      public string Name { get; set; }
    }

    public abstract class Param<T> where T : MSB2.Entry {
      internal abstract string Name { get; }

      internal abstract int Version { get; }

      internal List<T> Read(BinaryReaderEx br) {
        br.AssertInt32(this.Version);
        int num1 = br.ReadInt32();
        long offset = br.ReadInt64();
        long[] numArray = br.ReadInt64s(num1 - 1);
        long num2 = br.ReadInt64();
        string utF16 = br.GetUTF16(offset);
        if (utF16 != this.Name)
          throw new InvalidDataException("Expected param \"" +
                                         this.Name +
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

      internal virtual void Write(BinaryWriterEx bw, List<T> entries) {
        bw.WriteInt32(this.Version);
        bw.WriteInt32(entries.Count + 1);
        bw.ReserveInt64("ParamNameOffset");
        for (int index = 0; index < entries.Count; ++index)
          bw.ReserveInt64(string.Format("EntryOffset{0}", (object) index));
        bw.ReserveInt64("NextParamOffset");
        bw.FillInt64("ParamNameOffset", bw.Position);
        bw.WriteUTF16(this.Name, true);
        bw.Pad(8);
        int index1 = 0;
        System.Type type = (System.Type) null;
        for (int index2 = 0; index2 < entries.Count; ++index2) {
          if (type != entries[index2].GetType()) {
            type = entries[index2].GetType();
            index1 = 0;
          }
          bw.FillInt64(string.Format("EntryOffset{0}", (object) index2),
                       bw.Position);
          entries[index2].Write(bw, index1);
          bw.Pad(8);
          ++index1;
        }
      }

      public abstract List<T> GetEntries();
    }

    public enum PartType : ushort {
      MapPiece = 0,
      Object = 1,
      Collision = 3,
      Navmesh = 4,
      ConnectCollision = 5,
    }

    public class PartsParam : MSB2.Param<MSB2.Part>, IMsbParam<IMsbPart> {
      internal override string Name {
        get { return "PARTS_PARAM_ST"; }
      }

      internal override int Version {
        get { return 5; }
      }

      public List<MSB2.Part.MapPiece> MapPieces { get; set; }

      public List<MSB2.Part.Object> Objects { get; set; }

      public List<MSB2.Part.Collision> Collisions { get; set; }

      public List<MSB2.Part.Navmesh> Navmeshes { get; set; }

      public List<MSB2.Part.ConnectCollision> ConnectCollisions { get; set; }

      public PartsParam() {
        this.MapPieces = new List<MSB2.Part.MapPiece>();
        this.Objects = new List<MSB2.Part.Object>();
        this.Collisions = new List<MSB2.Part.Collision>();
        this.Navmeshes = new List<MSB2.Part.Navmesh>();
        this.ConnectCollisions = new List<MSB2.Part.ConnectCollision>();
      }

      internal override MSB2.Part ReadEntry(BinaryReaderEx br) {
        MSB2.PartType enum16 = br.GetEnum16<MSB2.PartType>(br.Position + 8L);
        switch (enum16) {
          case MSB2.PartType.MapPiece:
            MSB2.Part.MapPiece mapPiece = new MSB2.Part.MapPiece(br);
            this.MapPieces.Add(mapPiece);
            return (MSB2.Part) mapPiece;
          case MSB2.PartType.Object:
            MSB2.Part.Object @object = new MSB2.Part.Object(br);
            this.Objects.Add(@object);
            return (MSB2.Part) @object;
          case MSB2.PartType.Collision:
            MSB2.Part.Collision collision = new MSB2.Part.Collision(br);
            this.Collisions.Add(collision);
            return (MSB2.Part) collision;
          case MSB2.PartType.Navmesh:
            MSB2.Part.Navmesh navmesh = new MSB2.Part.Navmesh(br);
            this.Navmeshes.Add(navmesh);
            return (MSB2.Part) navmesh;
          case MSB2.PartType.ConnectCollision:
            MSB2.Part.ConnectCollision connectCollision =
                new MSB2.Part.ConnectCollision(br);
            this.ConnectCollisions.Add(connectCollision);
            return (MSB2.Part) connectCollision;
          default:
            throw new NotImplementedException(
                string.Format("Unimplemented part type: {0}", (object) enum16));
        }
      }

      public override List<MSB2.Part> GetEntries() {
        return SFUtil.ConcatAll<MSB2.Part>(new IEnumerable<MSB2.Part>[5] {
            (IEnumerable<MSB2.Part>) this.MapPieces,
            (IEnumerable<MSB2.Part>) this.Objects,
            (IEnumerable<MSB2.Part>) this.Collisions,
            (IEnumerable<MSB2.Part>) this.Navmeshes,
            (IEnumerable<MSB2.Part>) this.ConnectCollisions
        });
      }

      IReadOnlyList<IMsbPart> IMsbParam<IMsbPart>.GetEntries() {
        return (IReadOnlyList<IMsbPart>) this.GetEntries();
      }
    }

    public abstract class Part : MSB2.NamedEntry, IMsbPart, IMsbEntry {
      private int ModelIndex;

      public abstract MSB2.PartType Type { get; }

      public string ModelName { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      public Vector3 Scale { get; set; }

      public uint[] DrawGroups { get; private set; }

      public int Unk44 { get; set; }

      public int Unk48 { get; set; }

      public int Unk4C { get; set; }

      public int Unk50 { get; set; }

      public uint[] DispGroups { get; private set; }

      public int Unk64 { get; set; }

      public int Unk68 { get; set; }

      public int Unk6C { get; set; }

      internal Part(string name = "") {
        this.Name = name;
        this.Scale = Vector3.One;
        this.DrawGroups = new uint[4] {
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue
        };
        this.DispGroups = new uint[4] {
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue,
            uint.MaxValue
        };
      }

      internal Part(BinaryReaderEx br) {
        long position = br.Position;
        long num1 = br.ReadInt64();
        int num2 = (int) br.AssertUInt16((ushort) this.Type);
        int num3 = (int) br.ReadInt16();
        this.ModelIndex = br.ReadInt32();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        this.DrawGroups = br.ReadUInt32s(4);
        this.Unk44 = br.ReadInt32();
        this.Unk48 = br.ReadInt32();
        this.Unk4C = br.ReadInt32();
        this.Unk50 = br.ReadInt32();
        this.DispGroups = br.ReadUInt32s(4);
        this.Unk64 = br.ReadInt32();
        this.Unk68 = br.ReadInt32();
        this.Unk6C = br.ReadInt32();
        long num4 = br.ReadInt64();
        br.AssertInt64(new long[1]);
        this.Name = br.GetUTF16(position + num1);
        br.Position = position + num4;
        this.ReadTypeData(br);
      }

      internal abstract void ReadTypeData(BinaryReaderEx br);

      internal override void Write(BinaryWriterEx bw, int index) {
        long position1 = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteUInt16((ushort) this.Type);
        bw.WriteInt16((short) index);
        bw.WriteInt32(this.ModelIndex);
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.WriteVector3(this.Scale);
        bw.WriteUInt32s((IList<uint>) this.DrawGroups);
        bw.WriteInt32(this.Unk44);
        bw.WriteInt32(this.Unk48);
        bw.WriteInt32(this.Unk4C);
        bw.WriteInt32(this.Unk50);
        bw.WriteUInt32s((IList<uint>) this.DispGroups);
        bw.WriteInt32(this.Unk64);
        bw.WriteInt32(this.Unk68);
        bw.WriteInt32(this.Unk6C);
        bw.ReserveInt64("TypeDataOffset");
        bw.WriteInt64(0L);
        long position2 = bw.Position;
        bw.FillInt64("NameOffset", position2 - position1);
        bw.WriteUTF16(MSB.ReambiguateName(this.Name), true);
        if (bw.Position - position2 < 32L)
          bw.Position += 32L - (bw.Position - position2);
        bw.Pad(8);
        bw.FillInt64("TypeDataOffset", bw.Position - position1);
        this.WriteTypeData(bw);
      }

      internal abstract void WriteTypeData(BinaryWriterEx bw);

      internal virtual void GetNames(MSB2 msb, MSB2.Entries entries) {
        this.ModelName =
            MSB.FindName<MSB2.Model>(entries.Models, this.ModelIndex);
      }

      internal virtual void GetIndices(MSB2.Lookups lookups) {
        this.ModelIndex = MSB2.FindIndex(lookups.Models, this.ModelName);
      }

      public override string ToString() {
        return string.Format("{0} \"{1}\"",
                             (object) this.Type,
                             (object) this.Name);
      }

      public class MapPiece : MSB2.Part {
        public override MSB2.PartType Type {
          get { return MSB2.PartType.MapPiece; }
        }

        public short UnkT00 { get; set; }

        public short UnkT02 { get; set; }

        public MapPiece(string name = "")
            : base(name) {}

        internal MapPiece(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt16();
          this.UnkT02 = br.ReadInt16();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt16(this.UnkT00);
          bw.WriteInt16(this.UnkT02);
          bw.WriteInt32(0);
        }
      }

      public class Object : MSB2.Part {
        public override MSB2.PartType Type {
          get { return MSB2.PartType.Object; }
        }

        public int MapObjectInstanceParamID { get; set; }

        public int UnkT04 { get; set; }

        public Object(string name = "")
            : base(name) {}

        internal Object(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.MapObjectInstanceParamID = br.ReadInt32();
          this.UnkT04 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.MapObjectInstanceParamID);
          bw.WriteInt32(this.UnkT04);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      public class Collision : MSB2.Part {
        public override MSB2.PartType Type {
          get { return MSB2.PartType.Collision; }
        }

        public int UnkT00 { get; set; }

        public int UnkT04 { get; set; }

        public int UnkT08 { get; set; }

        public int UnkT0C { get; set; }

        public short UnkT10 { get; set; }

        public byte UnkT12 { get; set; }

        public byte UnkT13 { get; set; }

        public int UnkT14 { get; set; }

        public int UnkT18 { get; set; }

        public int UnkT1C { get; set; }

        public int UnkT20 { get; set; }

        public short UnkT24 { get; set; }

        public short UnkT26 { get; set; }

        public int UnkT28 { get; set; }

        public short UnkT2C { get; set; }

        public short UnkT2E { get; set; }

        public int UnkT30 { get; set; }

        public short UnkT34 { get; set; }

        public short UnkT36 { get; set; }

        public int UnkT38 { get; set; }

        public int UnkT3C { get; set; }

        public int UnkT40 { get; set; }

        public int UnkT44 { get; set; }

        public Collision(string name = "")
            : base(name) {}

        internal Collision(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadInt32();
          this.UnkT08 = br.ReadInt32();
          this.UnkT0C = br.ReadInt32();
          this.UnkT10 = br.ReadInt16();
          this.UnkT12 = br.ReadByte();
          this.UnkT13 = br.ReadByte();
          this.UnkT14 = br.ReadInt32();
          this.UnkT18 = br.ReadInt32();
          this.UnkT1C = br.ReadInt32();
          this.UnkT20 = br.ReadInt32();
          this.UnkT24 = br.ReadInt16();
          this.UnkT26 = br.ReadInt16();
          this.UnkT28 = br.ReadInt32();
          this.UnkT2C = br.ReadInt16();
          this.UnkT2E = br.ReadInt16();
          this.UnkT30 = br.ReadInt32();
          this.UnkT34 = br.ReadInt16();
          this.UnkT36 = br.ReadInt16();
          this.UnkT38 = br.ReadInt32();
          this.UnkT3C = br.ReadInt32();
          this.UnkT40 = br.ReadInt32();
          this.UnkT44 = br.ReadInt32();
          br.AssertPattern(16, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(this.UnkT04);
          bw.WriteInt32(this.UnkT08);
          bw.WriteInt32(this.UnkT0C);
          bw.WriteInt16(this.UnkT10);
          bw.WriteByte(this.UnkT12);
          bw.WriteByte(this.UnkT13);
          bw.WriteInt32(this.UnkT14);
          bw.WriteInt32(this.UnkT18);
          bw.WriteInt32(this.UnkT1C);
          bw.WriteInt32(this.UnkT20);
          bw.WriteInt16(this.UnkT24);
          bw.WriteInt16(this.UnkT26);
          bw.WriteInt32(this.UnkT28);
          bw.WriteInt16(this.UnkT2C);
          bw.WriteInt16(this.UnkT2E);
          bw.WriteInt32(this.UnkT30);
          bw.WriteInt16(this.UnkT34);
          bw.WriteInt16(this.UnkT36);
          bw.WriteInt32(this.UnkT38);
          bw.WriteInt32(this.UnkT3C);
          bw.WriteInt32(this.UnkT40);
          bw.WriteInt32(this.UnkT44);
          bw.WritePattern(16, (byte) 0);
        }
      }

      public class Navmesh : MSB2.Part {
        public override MSB2.PartType Type {
          get { return MSB2.PartType.Navmesh; }
        }

        public int UnkT00 { get; set; }

        public int UnkT04 { get; set; }

        public int UnkT08 { get; set; }

        public int UnkT0C { get; set; }

        public Navmesh(string name = "")
            : base(name) {}

        internal Navmesh(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadInt32();
          this.UnkT08 = br.ReadInt32();
          this.UnkT0C = br.ReadInt32();
          br.AssertPattern(16, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(this.UnkT04);
          bw.WriteInt32(this.UnkT08);
          bw.WriteInt32(this.UnkT0C);
          bw.WritePattern(16, (byte) 0);
        }
      }

      public class ConnectCollision : MSB2.Part {
        private int CollisionIndex;

        public override MSB2.PartType Type {
          get { return MSB2.PartType.ConnectCollision; }
        }

        public string CollisionName { get; set; }

        public byte MapID1 { get; set; }

        public byte MapID2 { get; set; }

        public byte MapID3 { get; set; }

        public byte MapID4 { get; set; }

        public int UnkT08 { get; set; }

        public int UnkT0C { get; set; }

        public ConnectCollision(string name = "")
            : base(name) {}

        internal ConnectCollision(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.CollisionIndex = br.ReadInt32();
          this.MapID1 = br.ReadByte();
          this.MapID2 = br.ReadByte();
          this.MapID3 = br.ReadByte();
          this.MapID4 = br.ReadByte();
          this.UnkT08 = br.ReadInt32();
          this.UnkT0C = br.ReadInt32();
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.CollisionIndex);
          bw.WriteByte(this.MapID1);
          bw.WriteByte(this.MapID2);
          bw.WriteByte(this.MapID3);
          bw.WriteByte(this.MapID4);
          bw.WriteInt32(this.UnkT08);
          bw.WriteInt32(this.UnkT0C);
        }

        internal override void GetNames(MSB2 msb, MSB2.Entries entries) {
          base.GetNames(msb, entries);
          this.CollisionName =
              MSB.FindName<MSB2.Part.Collision>(msb.Parts.Collisions,
                                                this.CollisionIndex);
        }

        internal override void GetIndices(MSB2.Lookups lookups) {
          base.GetIndices(lookups);
          this.CollisionIndex =
              MSB2.FindIndex(lookups.Collisions, this.CollisionName);
        }
      }
    }

    public enum RegionType : byte {
      Region0 = 0,
      Light = 3,
      StartPoint = 5,
      Sound = 7,
      SFX = 9,
      Wind = 13,     // 0x0D
      EnvLight = 14, // 0x0E
      Fog = 15,      // 0x0F
    }

    public class PointParam : MSB2.Param<MSB2.Region>, IMsbParam<IMsbRegion> {
      internal override string Name {
        get { return "POINT_PARAM_ST"; }
      }

      internal override int Version {
        get { return 5; }
      }

      public List<MSB2.Region.Region0> Region0s { get; set; }

      public List<MSB2.Region.Light> Lights { get; set; }

      public List<MSB2.Region.StartPoint> StartPoints { get; set; }

      public List<MSB2.Region.Sound> Sounds { get; set; }

      public List<MSB2.Region.SFX> SFXs { get; set; }

      public List<MSB2.Region.Wind> Winds { get; set; }

      public List<MSB2.Region.EnvLight> EnvLights { get; set; }

      public List<MSB2.Region.Fog> Fogs { get; set; }

      public PointParam() {
        this.Region0s = new List<MSB2.Region.Region0>();
        this.Lights = new List<MSB2.Region.Light>();
        this.StartPoints = new List<MSB2.Region.StartPoint>();
        this.Sounds = new List<MSB2.Region.Sound>();
        this.SFXs = new List<MSB2.Region.SFX>();
        this.Winds = new List<MSB2.Region.Wind>();
        this.EnvLights = new List<MSB2.Region.EnvLight>();
        this.Fogs = new List<MSB2.Region.Fog>();
      }

      internal override MSB2.Region ReadEntry(BinaryReaderEx br) {
        MSB2.RegionType enum8 = br.GetEnum8<MSB2.RegionType>(br.Position + 10L);
        switch (enum8) {
          case MSB2.RegionType.Region0:
            MSB2.Region.Region0 region0 = new MSB2.Region.Region0(br);
            this.Region0s.Add(region0);
            return (MSB2.Region) region0;
          case MSB2.RegionType.Light:
            MSB2.Region.Light light = new MSB2.Region.Light(br);
            this.Lights.Add(light);
            return (MSB2.Region) light;
          case MSB2.RegionType.StartPoint:
            MSB2.Region.StartPoint startPoint = new MSB2.Region.StartPoint(br);
            this.StartPoints.Add(startPoint);
            return (MSB2.Region) startPoint;
          case MSB2.RegionType.Sound:
            MSB2.Region.Sound sound = new MSB2.Region.Sound(br);
            this.Sounds.Add(sound);
            return (MSB2.Region) sound;
          case MSB2.RegionType.SFX:
            MSB2.Region.SFX sfx = new MSB2.Region.SFX(br);
            this.SFXs.Add(sfx);
            return (MSB2.Region) sfx;
          case MSB2.RegionType.Wind:
            MSB2.Region.Wind wind = new MSB2.Region.Wind(br);
            this.Winds.Add(wind);
            return (MSB2.Region) wind;
          case MSB2.RegionType.EnvLight:
            MSB2.Region.EnvLight envLight = new MSB2.Region.EnvLight(br);
            this.EnvLights.Add(envLight);
            return (MSB2.Region) envLight;
          case MSB2.RegionType.Fog:
            MSB2.Region.Fog fog = new MSB2.Region.Fog(br);
            this.Fogs.Add(fog);
            return (MSB2.Region) fog;
          default:
            throw new NotImplementedException(
                string.Format("Unimplemented region type: {0}",
                              (object) enum8));
        }
      }

      public override List<MSB2.Region> GetEntries() {
        return SFUtil.ConcatAll<MSB2.Region>(new IEnumerable<MSB2.Region>[8] {
            (IEnumerable<MSB2.Region>) this.Region0s,
            (IEnumerable<MSB2.Region>) this.Lights,
            (IEnumerable<MSB2.Region>) this.StartPoints,
            (IEnumerable<MSB2.Region>) this.Sounds,
            (IEnumerable<MSB2.Region>) this.SFXs,
            (IEnumerable<MSB2.Region>) this.Winds,
            (IEnumerable<MSB2.Region>) this.EnvLights,
            (IEnumerable<MSB2.Region>) this.Fogs
        });
      }

      IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries() {
        return (IReadOnlyList<IMsbRegion>) this.GetEntries();
      }
    }

    public abstract class Region : MSB2.NamedEntry, IMsbRegion, IMsbEntry {
      public abstract MSB2.RegionType Type { get; }

      internal abstract bool HasTypeData { get; }

      public short Unk08 { get; set; }

      public MSB2.Shape Shape { get; set; }

      public short Unk0E { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      internal Region(string name = "") {
        this.Name = name;
        this.Shape = (MSB2.Shape) new MSB2.Shape.Point();
      }

      internal Region(BinaryReaderEx br) {
        long position = br.Position;
        long num1 = br.ReadInt64();
        this.Unk08 = br.ReadInt16();
        int num2 = (int) br.AssertByte((byte) this.Type);
        MSB2.ShapeType shapeType = br.ReadEnum8<MSB2.ShapeType>();
        int num3 = (int) br.ReadInt16();
        this.Unk0E = br.ReadInt16();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        long num4 = br.ReadInt64();
        long num5 = br.ReadInt64();
        br.AssertInt32(-1);
        br.AssertPattern(36, (byte) 0);
        long num6 = br.ReadInt64();
        long num7 = br.ReadInt64();
        br.AssertInt64(new long[1]);
        br.AssertInt64(new long[1]);
        this.Name = br.GetUTF16(position + num1);
        br.Position = position + num4;
        br.AssertInt32(new int[1]);
        br.Position = position + num5;
        br.AssertInt32(new int[1]);
        br.Position = position + num6;
        switch (shapeType) {
          case MSB2.ShapeType.Point:
            this.Shape = (MSB2.Shape) new MSB2.Shape.Point();
            break;
          case MSB2.ShapeType.Circle:
            this.Shape = (MSB2.Shape) new MSB2.Shape.Circle(br);
            break;
          case MSB2.ShapeType.Sphere:
            this.Shape = (MSB2.Shape) new MSB2.Shape.Sphere(br);
            break;
          case MSB2.ShapeType.Cylinder:
            this.Shape = (MSB2.Shape) new MSB2.Shape.Cylinder(br);
            break;
          case MSB2.ShapeType.Rect:
            this.Shape = (MSB2.Shape) new MSB2.Shape.Rect(br);
            break;
          case MSB2.ShapeType.Box:
            this.Shape = (MSB2.Shape) new MSB2.Shape.Box(br);
            break;
          default:
            throw new NotImplementedException(
                string.Format("Unimplemented shape type: {0}",
                              (object) shapeType));
        }
        if (!this.HasTypeData)
          return;
        br.Position = position + num7;
        this.ReadTypeData(br);
      }

      internal virtual void ReadTypeData(BinaryReaderEx br) {
        throw new InvalidOperationException(
            "Type data should not be read for regions with no type data.");
      }

      internal override void Write(BinaryWriterEx bw, int index) {
        long position = bw.Position;
        bw.ReserveInt64("NameOffset");
        bw.WriteInt16(this.Unk08);
        bw.WriteByte((byte) this.Type);
        bw.WriteByte((byte) this.Shape.Type);
        bw.WriteInt16((short) index);
        bw.WriteInt16(this.Unk0E);
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.ReserveInt64("UnkOffsetA");
        bw.ReserveInt64("UnkOffsetB");
        bw.WriteInt32(-1);
        bw.WritePattern(36, (byte) 0);
        bw.ReserveInt64("ShapeDataOffset");
        bw.ReserveInt64("TypeDataOffset");
        bw.WriteInt64(0L);
        bw.WriteInt64(0L);
        bw.FillInt64("NameOffset", bw.Position - position);
        bw.WriteUTF16(this.Name, true);
        bw.Pad(4);
        bw.FillInt64("UnkOffsetA", bw.Position - position);
        bw.WriteInt32(0);
        bw.FillInt64("UnkOffsetB", bw.Position - position);
        bw.WriteInt32(0);
        bw.Pad(8);
        if (this.Shape.HasShapeData) {
          bw.FillInt64("ShapeDataOffset", bw.Position - position);
          this.Shape.WriteShapeData(bw);
        } else
          bw.FillInt64("ShapeDataOffset", 0L);
        if (this.HasTypeData) {
          bw.FillInt64("TypeDataOffset", bw.Position - position);
          this.WriteTypeData(bw);
        } else
          bw.FillInt64("TypeDataOffset", 0L);
      }

      internal virtual void WriteTypeData(BinaryWriterEx bw) {
        throw new InvalidOperationException(
            "Type data should not be written for regions with no type data.");
      }

      public override string ToString() {
        return string.Format("{0} {1} \"{2}\"",
                             (object) this.Type,
                             (object) this.Shape.Type,
                             (object) this.Name);
      }

      public class Region0 : MSB2.Region {
        public override MSB2.RegionType Type {
          get { return MSB2.RegionType.Region0; }
        }

        internal override bool HasTypeData {
          get { return false; }
        }

        public Region0(string name = "")
            : base(name) {}

        internal Region0(BinaryReaderEx br)
            : base(br) {}
      }

      public class Light : MSB2.Region {
        public override MSB2.RegionType Type {
          get { return MSB2.RegionType.Light; }
        }

        internal override bool HasTypeData {
          get { return true; }
        }

        public int UnkT00 { get; set; }

        public Color ColorT04 { get; set; }

        public Color ColorT08 { get; set; }

        public float UnkT0C { get; set; }

        public Light(string name = "")
            : base(name) {}

        internal Light(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.ColorT04 = br.ReadRGBA();
          this.ColorT08 = br.ReadRGBA();
          this.UnkT0C = br.ReadSingle();
          br.AssertPattern(20, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteRGBA(this.ColorT04);
          bw.WriteRGBA(this.ColorT08);
          bw.WriteSingle(this.UnkT0C);
          bw.WritePattern(20, (byte) 0);
        }
      }

      public class StartPoint : MSB2.Region {
        public override MSB2.RegionType Type {
          get { return MSB2.RegionType.StartPoint; }
        }

        internal override bool HasTypeData {
          get { return false; }
        }

        public StartPoint(string name = "")
            : base(name) {}

        internal StartPoint(BinaryReaderEx br)
            : base(br) {}
      }

      public class Sound : MSB2.Region {
        public override MSB2.RegionType Type {
          get { return MSB2.RegionType.Sound; }
        }

        internal override bool HasTypeData {
          get { return true; }
        }

        public int UnkT00 { get; set; }

        public int SoundID { get; set; }

        public int UnkT08 { get; set; }

        public Sound(string name = "")
            : base(name) {}

        internal Sound(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.SoundID = br.ReadInt32();
          this.UnkT08 = br.ReadInt32();
          br.AssertPattern(20, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(this.SoundID);
          bw.WriteInt32(this.UnkT08);
          bw.WritePattern(20, (byte) 0);
        }
      }

      public class SFX : MSB2.Region {
        public override MSB2.RegionType Type {
          get { return MSB2.RegionType.SFX; }
        }

        internal override bool HasTypeData {
          get { return true; }
        }

        public int EffectID { get; set; }

        public int UnkT04 { get; set; }

        public SFX(string name = "")
            : base(name) {}

        internal SFX(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.EffectID = br.ReadInt32();
          this.UnkT04 = br.ReadInt32();
          br.AssertPattern(24, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.EffectID);
          bw.WriteInt32(this.UnkT04);
          bw.WritePattern(24, (byte) 0);
        }
      }

      public class Wind : MSB2.Region {
        public override MSB2.RegionType Type {
          get { return MSB2.RegionType.Wind; }
        }

        internal override bool HasTypeData {
          get { return true; }
        }

        public int UnkT00 { get; set; }

        public float UnkT04 { get; set; }

        public float UnkT08 { get; set; }

        public float UnkT0C { get; set; }

        public float UnkT10 { get; set; }

        public float UnkT14 { get; set; }

        public float UnkT18 { get; set; }

        public Wind(string name = "")
            : base(name) {}

        internal Wind(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadSingle();
          this.UnkT08 = br.ReadSingle();
          this.UnkT0C = br.ReadSingle();
          this.UnkT10 = br.ReadSingle();
          this.UnkT14 = br.ReadSingle();
          this.UnkT18 = br.ReadSingle();
          br.AssertInt32(new int[1]);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteSingle(this.UnkT04);
          bw.WriteSingle(this.UnkT08);
          bw.WriteSingle(this.UnkT0C);
          bw.WriteSingle(this.UnkT10);
          bw.WriteSingle(this.UnkT14);
          bw.WriteSingle(this.UnkT18);
          bw.WriteInt32(0);
        }
      }

      public class EnvLight : MSB2.Region {
        public override MSB2.RegionType Type {
          get { return MSB2.RegionType.EnvLight; }
        }

        internal override bool HasTypeData {
          get { return true; }
        }

        public int UnkT00 { get; set; }

        public float UnkT04 { get; set; }

        public float UnkT08 { get; set; }

        public EnvLight(string name = "")
            : base(name) {}

        internal EnvLight(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadSingle();
          this.UnkT08 = br.ReadSingle();
          br.AssertPattern(20, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteSingle(this.UnkT04);
          bw.WriteSingle(this.UnkT08);
          bw.WritePattern(20, (byte) 0);
        }
      }

      public class Fog : MSB2.Region {
        public override MSB2.RegionType Type {
          get { return MSB2.RegionType.Fog; }
        }

        internal override bool HasTypeData {
          get { return true; }
        }

        public int UnkT00 { get; set; }

        public int UnkT04 { get; set; }

        public Fog(string name = "")
            : base(name) {}

        internal Fog(BinaryReaderEx br)
            : base(br) {}

        internal override void ReadTypeData(BinaryReaderEx br) {
          this.UnkT00 = br.ReadInt32();
          this.UnkT04 = br.ReadInt32();
          br.AssertPattern(28, (byte) 0);
        }

        internal override void WriteTypeData(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkT00);
          bw.WriteInt32(this.UnkT04);
          bw.WritePattern(28, (byte) 0);
        }
      }
    }

    private class RouteParam : MSB2.Param<MSB2.Entry> {
      internal override string Name {
        get { return "ROUTE_PARAM_ST"; }
      }

      internal override int Version {
        get { return 5; }
      }

      internal override MSB2.Entry ReadEntry(BinaryReaderEx br) {
        throw new NotSupportedException(
            "Route param should always be empty in DS2.");
      }

      public override List<MSB2.Entry> GetEntries() {
        throw new NotSupportedException(
            "Route param should always be empty in DS2.");
      }
    }

    public enum ShapeType : byte {
      Point,
      Circle,
      Sphere,
      Cylinder,
      Rect,
      Box,
    }

    public abstract class Shape {
      public abstract MSB2.ShapeType Type { get; }

      internal abstract bool HasShapeData { get; }

      internal virtual void WriteShapeData(BinaryWriterEx bw) {
        throw new InvalidOperationException(
            "Shape data should not be written for shapes with no shape data.");
      }

      public class Point : MSB2.Shape {
        public override MSB2.ShapeType Type {
          get { return MSB2.ShapeType.Point; }
        }

        internal override bool HasShapeData {
          get { return false; }
        }
      }

      public class Circle : MSB2.Shape {
        public override MSB2.ShapeType Type {
          get { return MSB2.ShapeType.Circle; }
        }

        internal override bool HasShapeData {
          get { return true; }
        }

        public float Radius { get; set; }

        public Circle() {}

        internal Circle(BinaryReaderEx br) {
          this.Radius = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw) {
          bw.WriteSingle(this.Radius);
        }
      }

      public class Sphere : MSB2.Shape {
        public override MSB2.ShapeType Type {
          get { return MSB2.ShapeType.Sphere; }
        }

        internal override bool HasShapeData {
          get { return true; }
        }

        public float Radius { get; set; }

        public Sphere() {}

        internal Sphere(BinaryReaderEx br) {
          this.Radius = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw) {
          bw.WriteSingle(this.Radius);
        }
      }

      public class Cylinder : MSB2.Shape {
        public override MSB2.ShapeType Type {
          get { return MSB2.ShapeType.Cylinder; }
        }

        internal override bool HasShapeData {
          get { return true; }
        }

        public float Radius { get; set; }

        public float Height { get; set; }

        public Cylinder() {}

        internal Cylinder(BinaryReaderEx br) {
          this.Radius = br.ReadSingle();
          this.Height = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw) {
          bw.WriteSingle(this.Radius);
          bw.WriteSingle(this.Height);
        }
      }

      public class Rect : MSB2.Shape {
        public override MSB2.ShapeType Type {
          get { return MSB2.ShapeType.Rect; }
        }

        internal override bool HasShapeData {
          get { return true; }
        }

        public float Width { get; set; }

        public float Depth { get; set; }

        public Rect() {}

        internal Rect(BinaryReaderEx br) {
          this.Width = br.ReadSingle();
          this.Depth = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw) {
          bw.WriteSingle(this.Width);
          bw.WriteSingle(this.Depth);
        }
      }

      public class Box : MSB2.Shape {
        public override MSB2.ShapeType Type {
          get { return MSB2.ShapeType.Box; }
        }

        internal override bool HasShapeData {
          get { return true; }
        }

        public float Width { get; set; }

        public float Depth { get; set; }

        public float Height { get; set; }

        public Box() {}

        internal Box(BinaryReaderEx br) {
          this.Width = br.ReadSingle();
          this.Depth = br.ReadSingle();
          this.Height = br.ReadSingle();
        }

        internal override void WriteShapeData(BinaryWriterEx bw) {
          bw.WriteSingle(this.Width);
          bw.WriteSingle(this.Depth);
          bw.WriteSingle(this.Height);
        }
      }
    }
  }
}