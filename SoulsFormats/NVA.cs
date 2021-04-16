// Decompiled with JetBrains decompiler
// Type: SoulsFormats.NVA
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class NVA : SoulsFile<NVA> {
    public NVA.NVAVersion Version { get; set; }

    public NVA.NavmeshSection Navmeshes { get; set; }

    public NVA.Section1 Entries1 { get; set; }

    public NVA.Section2 Entries2 { get; set; }

    public NVA.ConnectorSection Connectors { get; set; }

    public NVA.Section7 Entries7 { get; set; }

    public NVA() {
      this.Version = NVA.NVAVersion.DarkSouls3;
      this.Navmeshes = new NVA.NavmeshSection(2);
      this.Entries1 = new NVA.Section1();
      this.Entries2 = new NVA.Section2();
      this.Connectors = new NVA.ConnectorSection();
      this.Entries7 = new NVA.Section7();
    }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "NVMA";
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertASCII("NVMA");
      this.Version = br.ReadEnum32<NVA.NVAVersion>();
      int num = (int) br.ReadUInt32();
      br.AssertInt32(this.Version == NVA.NVAVersion.OldBloodborne ? 8 : 9);
      this.Navmeshes = new NVA.NavmeshSection(br);
      this.Entries1 = new NVA.Section1(br);
      this.Entries2 = new NVA.Section2(br);
      NVA.Section3 section3 = new NVA.Section3(br);
      this.Connectors = new NVA.ConnectorSection(br);
      NVA.ConnectorPointSection points = new NVA.ConnectorPointSection(br);
      NVA.ConnectorConditionSection conds =
          new NVA.ConnectorConditionSection(br);
      this.Entries7 = new NVA.Section7(br);
      NVA.MapNodeSection entries8 = this.Version != NVA.NVAVersion.OldBloodborne
                                        ? new NVA.MapNodeSection(br)
                                        : new NVA.MapNodeSection(1);
      foreach (NVA.Navmesh navmesh in (List<NVA.Navmesh>) this.Navmeshes)
        navmesh.TakeMapNodes(entries8);
      foreach (NVA.Connector connector in (List<NVA.Connector>) this.Connectors)
        connector.TakePointsAndConds(points, conds);
    }

    protected override void Write(BinaryWriterEx bw) {
      NVA.ConnectorPointSection points = new NVA.ConnectorPointSection();
      NVA.ConnectorConditionSection conds = new NVA.ConnectorConditionSection();
      foreach (NVA.Connector connector in (List<NVA.Connector>) this.Connectors)
        connector.GivePointsAndConds(points, conds);
      NVA.MapNodeSection mapNodes =
          new NVA.MapNodeSection(this.Version == NVA.NVAVersion.Sekiro ? 2 : 1);
      foreach (NVA.Navmesh navmesh in (List<NVA.Navmesh>) this.Navmeshes)
        navmesh.GiveMapNodes(mapNodes);
      bw.BigEndian = false;
      bw.WriteASCII("NVMA", false);
      bw.WriteUInt32((uint) this.Version);
      bw.ReserveUInt32("FileSize");
      bw.WriteInt32(this.Version == NVA.NVAVersion.OldBloodborne ? 8 : 9);
      this.Navmeshes.Write(bw, 0);
      this.Entries1.Write(bw, 1);
      this.Entries2.Write(bw, 2);
      new NVA.Section3().Write(bw, 3);
      this.Connectors.Write(bw, 4);
      points.Write(bw, 5);
      conds.Write(bw, 6);
      this.Entries7.Write(bw, 7);
      if (this.Version != NVA.NVAVersion.OldBloodborne)
        mapNodes.Write(bw, 8);
      bw.FillUInt32("FileSize", (uint) bw.Position);
    }

    public enum NVAVersion : uint {
      OldBloodborne = 3,
      DarkSouls3 = 4,
      Sekiro = 5,
    }

    public abstract class Section<T> : List<T> {
      public int Version { get; set; }

      internal Section(int version) {
        this.Version = version;
      }

      internal Section(BinaryReaderEx br, int index, params int[] versions) {
        br.AssertInt32(index);
        this.Version = br.AssertInt32(versions);
        int num = br.ReadInt32();
        int count = br.ReadInt32();
        this.Capacity = count;
        long position = br.Position;
        this.ReadEntries(br, count);
        br.Position = position + (long) num;
      }

      internal abstract void ReadEntries(BinaryReaderEx br, int count);

      internal void Write(BinaryWriterEx bw, int index) {
        bw.WriteInt32(index);
        bw.WriteInt32(this.Version);
        bw.ReserveInt32("SectionLength");
        bw.WriteInt32(this.Count);
        long position = bw.Position;
        this.WriteEntries(bw);
        if (bw.Position % 16L != 0L)
          bw.WritePattern(16 - (int) bw.Position % 16, byte.MaxValue);
        bw.FillInt32("SectionLength", (int) (bw.Position - position));
      }

      internal abstract void WriteEntries(BinaryWriterEx bw);
    }

    public class NavmeshSection : NVA.Section<NVA.Navmesh> {
      public NavmeshSection(int version)
          : base(version) {}

      internal NavmeshSection(BinaryReaderEx br)
          : base(br, 0, 2, 3, 4) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.Navmesh(br, this.Version));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        for (int index = 0; index < this.Count; ++index)
          this[index].Write(bw, this.Version, index);
        for (int index = 0; index < this.Count; ++index)
          this[index].WriteNameRefs(bw, this.Version, index);
      }
    }

    public class Navmesh {
      private short MapNodesIndex;
      private short MapNodeCount;

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      public Vector3 Scale { get; set; }

      public int NameID { get; set; }

      public int ModelID { get; set; }

      public int Unk38 { get; set; }

      public int VertexCount { get; set; }

      public List<int> NameReferenceIDs { get; set; }

      public List<NVA.MapNode> MapNodes { get; set; }

      public bool Unk4C { get; set; }

      public Navmesh() {
        this.Scale = Vector3.One;
        this.NameReferenceIDs = new List<int>();
        this.MapNodes = new List<NVA.MapNode>();
      }

      internal Navmesh(BinaryReaderEx br, int version) {
        this.Position = br.ReadVector3();
        double num1 = (double) br.AssertSingle(1f);
        this.Rotation = br.ReadVector3();
        br.AssertInt32(new int[1]);
        this.Scale = br.ReadVector3();
        br.AssertInt32(new int[1]);
        this.NameID = br.ReadInt32();
        this.ModelID = br.ReadInt32();
        this.Unk38 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.VertexCount = br.ReadInt32();
        int count = br.ReadInt32();
        this.MapNodesIndex = br.ReadInt16();
        this.MapNodeCount = br.ReadInt16();
        this.Unk4C = br.AssertInt32(0, 1) == 1;
        if (version < 4) {
          if (count > 16)
            throw new InvalidDataException(
                "Name reference count should not exceed 16 in DS3/BB.");
          this.NameReferenceIDs =
              new List<int>((IEnumerable<int>) br.ReadInt32s(count));
          for (int index = 0; index < 16 - count; ++index)
            br.AssertInt32(-1);
        } else {
          int num2 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.NameReferenceIDs =
              new List<int>(
                  (IEnumerable<int>) br.GetInt32s((long) num2, count));
        }
      }

      internal void TakeMapNodes(NVA.MapNodeSection entries8) {
        this.MapNodes = new List<NVA.MapNode>((int) this.MapNodeCount);
        for (int index = 0; index < (int) this.MapNodeCount; ++index)
          this.MapNodes.Add(entries8[(int) this.MapNodesIndex + index]);
        this.MapNodeCount = (short) -1;
        foreach (NVA.MapNode mapNode in this.MapNodes) {
          if (mapNode.SiblingDistances.Count > this.MapNodes.Count)
            mapNode.SiblingDistances.RemoveRange(
                this.MapNodes.Count,
                mapNode.SiblingDistances.Count - this.MapNodes.Count);
        }
      }

      internal void Write(BinaryWriterEx bw, int version, int index) {
        bw.WriteVector3(this.Position);
        bw.WriteSingle(1f);
        bw.WriteVector3(this.Rotation);
        bw.WriteInt32(0);
        bw.WriteVector3(this.Scale);
        bw.WriteInt32(0);
        bw.WriteInt32(this.NameID);
        bw.WriteInt32(this.ModelID);
        bw.WriteInt32(this.Unk38);
        bw.WriteInt32(0);
        bw.WriteInt32(this.VertexCount);
        bw.WriteInt32(this.NameReferenceIDs.Count);
        bw.WriteInt16(this.MapNodesIndex);
        bw.WriteInt16((short) this.MapNodes.Count);
        bw.WriteInt32(this.Unk4C ? 1 : 0);
        if (version < 4) {
          if (this.NameReferenceIDs.Count > 16)
            throw new InvalidDataException(
                "Name reference count should not exceed 16 in DS3/BB.");
          bw.WriteInt32s((IList<int>) this.NameReferenceIDs);
          for (int index1 = 0;
               index1 < 16 - this.NameReferenceIDs.Count;
               ++index1)
            bw.WriteInt32(-1);
        } else {
          bw.ReserveInt32(string.Format("NameRefOffset{0}", (object) index));
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
        }
      }

      internal void WriteNameRefs(BinaryWriterEx bw, int version, int index) {
        if (version < 4)
          return;
        bw.FillInt32(string.Format("NameRefOffset{0}", (object) index),
                     (int) bw.Position);
        bw.WriteInt32s((IList<int>) this.NameReferenceIDs);
      }

      internal void GiveMapNodes(NVA.MapNodeSection mapNodes) {
        this.MapNodesIndex = (short) mapNodes.Count;
        mapNodes.AddRange((IEnumerable<NVA.MapNode>) this.MapNodes);
      }

      public override string ToString() {
        return string.Format("{0} {1} {2} [{3} References] [{4} MapNodes]",
                             (object) this.NameID,
                             (object) this.Position,
                             (object) this.Rotation,
                             (object) this.NameReferenceIDs.Count,
                             (object) this.MapNodes.Count);
      }
    }

    public class Section1 : NVA.Section<NVA.Entry1> {
      public Section1()
          : base(1) {}

      internal Section1(BinaryReaderEx br)
          : base(br, 1, 1) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.Entry1(br));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        foreach (NVA.Entry1 entry1 in (List<NVA.Entry1>) this)
          entry1.Write(bw);
      }
    }

    public class Entry1 {
      public int Unk00 { get; set; }

      public Entry1() {}

      internal Entry1(BinaryReaderEx br) {
        this.Unk00 = br.ReadInt32();
        br.AssertInt32(new int[1]);
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(0);
      }

      public override string ToString() {
        return string.Format("{0}", (object) this.Unk00);
      }
    }

    public class Section2 : NVA.Section<NVA.Entry2> {
      public Section2()
          : base(1) {}

      internal Section2(BinaryReaderEx br)
          : base(br, 2, 1) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.Entry2(br));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        foreach (NVA.Entry2 entry2 in (List<NVA.Entry2>) this)
          entry2.Write(bw);
      }
    }

    public class Entry2 {
      public int Unk00 { get; set; }

      public List<NVA.Entry2.Reference> References { get; set; }

      public int Unk08 { get; set; }

      public Entry2() {
        this.References = new List<NVA.Entry2.Reference>();
        this.Unk08 = -1;
      }

      internal Entry2(BinaryReaderEx br) {
        this.Unk00 = br.ReadInt32();
        int capacity = br.ReadInt32();
        this.Unk08 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        if (capacity > 64)
          throw new InvalidDataException(
              "Entry2 reference count should not exceed 64.");
        this.References = new List<NVA.Entry2.Reference>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.References.Add(new NVA.Entry2.Reference(br));
        for (int index = 0; index < 64 - capacity; ++index)
          br.AssertInt64(new long[1]);
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(this.References.Count);
        bw.WriteInt32(this.Unk08);
        bw.WriteInt32(0);
        if (this.References.Count > 64)
          throw new InvalidDataException(
              "Entry2 reference count should not exceed 64.");
        foreach (NVA.Entry2.Reference reference in this.References)
          reference.Write(bw);
        for (int index = 0; index < 64 - this.References.Count; ++index)
          bw.WriteInt64(0L);
      }

      public override string ToString() {
        return string.Format("{0} {1} [{2} References]",
                             (object) this.Unk00,
                             (object) this.Unk08,
                             (object) this.References.Count);
      }

      public class Reference {
        public int UnkIndex { get; set; }

        public int NameID { get; set; }

        public Reference() {}

        internal Reference(BinaryReaderEx br) {
          this.UnkIndex = br.ReadInt32();
          this.NameID = br.ReadInt32();
        }

        internal void Write(BinaryWriterEx bw) {
          bw.WriteInt32(this.UnkIndex);
          bw.WriteInt32(this.NameID);
        }

        public override string ToString() {
          return string.Format("{0} {1}",
                               (object) this.UnkIndex,
                               (object) this.NameID);
        }
      }
    }

    private class Section3 : NVA.Section<NVA.Entry3> {
      public Section3()
          : base(1) {}

      internal Section3(BinaryReaderEx br)
          : base(br, 3, 1) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.Entry3(br));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        foreach (NVA.Entry3 entry3 in (List<NVA.Entry3>) this)
          entry3.Write(bw);
      }
    }

    private class Entry3 {
      internal Entry3(BinaryReaderEx br) {
        throw new NotImplementedException(
            "Section3 is empty in all known NVAs.");
      }

      internal void Write(BinaryWriterEx bw) {
        throw new NotImplementedException(
            "Section3 is empty in all known NVAs.");
      }
    }

    public class ConnectorSection : NVA.Section<NVA.Connector> {
      public ConnectorSection()
          : base(1) {}

      internal ConnectorSection(BinaryReaderEx br)
          : base(br, 4, 1) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.Connector(br));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        foreach (NVA.Connector connector in (List<NVA.Connector>) this)
          connector.Write(bw);
      }
    }

    public class Connector {
      private int PointCount;
      private int ConditionCount;
      private int PointsIndex;
      private int ConditionsIndex;

      public int MainNameID { get; set; }

      public int TargetNameID { get; set; }

      public List<NVA.ConnectorPoint> Points { get; set; }

      public List<NVA.ConnectorCondition> Conditions { get; set; }

      public Connector() {
        this.Points = new List<NVA.ConnectorPoint>();
        this.Conditions = new List<NVA.ConnectorCondition>();
      }

      internal Connector(BinaryReaderEx br) {
        this.MainNameID = br.ReadInt32();
        this.TargetNameID = br.ReadInt32();
        this.PointCount = br.ReadInt32();
        this.ConditionCount = br.ReadInt32();
        this.PointsIndex = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.ConditionsIndex = br.ReadInt32();
        br.AssertInt32(new int[1]);
      }

      internal void TakePointsAndConds(
          NVA.ConnectorPointSection points,
          NVA.ConnectorConditionSection conds) {
        this.Points = new List<NVA.ConnectorPoint>(this.PointCount);
        for (int index = 0; index < this.PointCount; ++index)
          this.Points.Add(points[this.PointsIndex + index]);
        this.PointCount = -1;
        this.Conditions = new List<NVA.ConnectorCondition>(this.ConditionCount);
        for (int index = 0; index < this.ConditionCount; ++index)
          this.Conditions.Add(conds[this.ConditionsIndex + index]);
        this.ConditionCount = -1;
      }

      internal void GivePointsAndConds(
          NVA.ConnectorPointSection points,
          NVA.ConnectorConditionSection conds) {
        this.PointsIndex = points.Count;
        points.AddRange((IEnumerable<NVA.ConnectorPoint>) this.Points);
        this.ConditionsIndex = conds.Count;
        conds.AddRange((IEnumerable<NVA.ConnectorCondition>) this.Conditions);
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(this.MainNameID);
        bw.WriteInt32(this.TargetNameID);
        bw.WriteInt32(this.Points.Count);
        bw.WriteInt32(this.Conditions.Count);
        bw.WriteInt32(this.PointsIndex);
        bw.WriteInt32(0);
        bw.WriteInt32(this.ConditionsIndex);
        bw.WriteInt32(0);
      }

      public override string ToString() {
        return string.Format("{0} -> {1} [{2} Points][{3} Conditions]",
                             (object) this.MainNameID,
                             (object) this.TargetNameID,
                             (object) this.Points.Count,
                             (object) this.Conditions.Count);
      }
    }

    internal class ConnectorPointSection : NVA.Section<NVA.ConnectorPoint> {
      public ConnectorPointSection()
          : base(1) {}

      internal ConnectorPointSection(BinaryReaderEx br)
          : base(br, 5, 1) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.ConnectorPoint(br));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        foreach (NVA.ConnectorPoint connectorPoint in
            (List<NVA.ConnectorPoint>) this)
          connectorPoint.Write(bw);
      }
    }

    public class ConnectorPoint {
      public int Unk00 { get; set; }

      public int Unk04 { get; set; }

      public int Unk08 { get; set; }

      public int Unk0C { get; set; }

      public ConnectorPoint() {}

      internal ConnectorPoint(BinaryReaderEx br) {
        this.Unk00 = br.ReadInt32();
        this.Unk04 = br.ReadInt32();
        this.Unk08 = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.Unk08);
        bw.WriteInt32(this.Unk0C);
      }

      public override string ToString() {
        return string.Format("{0} {1} {2} {3}",
                             (object) this.Unk00,
                             (object) this.Unk04,
                             (object) this.Unk08,
                             (object) this.Unk0C);
      }
    }

    internal class
        ConnectorConditionSection : NVA.Section<NVA.ConnectorCondition> {
      public ConnectorConditionSection()
          : base(1) {}

      internal ConnectorConditionSection(BinaryReaderEx br)
          : base(br, 6, 1) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.ConnectorCondition(br));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        foreach (NVA.ConnectorCondition connectorCondition in
            (List<NVA.ConnectorCondition>) this)
          connectorCondition.Write(bw);
      }
    }

    public class ConnectorCondition {
      public int Condition1 { get; set; }

      public int Condition2 { get; set; }

      public ConnectorCondition() {}

      internal ConnectorCondition(BinaryReaderEx br) {
        this.Condition1 = br.ReadInt32();
        this.Condition2 = br.ReadInt32();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(this.Condition1);
        bw.WriteInt32(this.Condition2);
      }

      public override string ToString() {
        return string.Format("{0} {1}",
                             (object) this.Condition1,
                             (object) this.Condition2);
      }
    }

    public class Section7 : NVA.Section<NVA.Entry7> {
      public Section7()
          : base(1) {}

      internal Section7(BinaryReaderEx br)
          : base(br, 7, 1) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.Entry7(br));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        foreach (NVA.Entry7 entry7 in (List<NVA.Entry7>) this)
          entry7.Write(bw);
      }
    }

    public class Entry7 {
      public Vector3 Position { get; set; }

      public int NameID { get; set; }

      public int Unk18 { get; set; }

      public Entry7() {}

      internal Entry7(BinaryReaderEx br) {
        this.Position = br.ReadVector3();
        double num = (double) br.AssertSingle(1f);
        this.NameID = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Unk18 = br.ReadInt32();
        br.AssertInt32(new int[1]);
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteVector3(this.Position);
        bw.WriteSingle(1f);
        bw.WriteInt32(this.NameID);
        bw.WriteInt32(0);
        bw.WriteInt32(this.Unk18);
        bw.WriteInt32(0);
      }

      public override string ToString() {
        return string.Format("{0} {1} {2}",
                             (object) this.Position,
                             (object) this.NameID,
                             (object) this.Unk18);
      }
    }

    internal class MapNodeSection : NVA.Section<NVA.MapNode> {
      public MapNodeSection(int version)
          : base(version) {}

      internal MapNodeSection(BinaryReaderEx br)
          : base(br, 8, 1, 2) {}

      internal override void ReadEntries(BinaryReaderEx br, int count) {
        for (int index = 0; index < count; ++index)
          this.Add(new NVA.MapNode(br, this.Version));
      }

      internal override void WriteEntries(BinaryWriterEx bw) {
        for (int index = 0; index < this.Count; ++index)
          this[index].Write(bw, this.Version, index);
        for (int index = 0; index < this.Count; ++index)
          this[index].WriteSubIDs(bw, this.Version, index);
      }
    }

    public class MapNode {
      public Vector3 Position { get; set; }

      public short Section0Index { get; set; }

      public short MainID { get; set; }

      public List<float> SiblingDistances { get; set; }

      public int Unk14 { get; set; }

      public MapNode() {
        this.SiblingDistances = new List<float>();
      }

      internal MapNode(BinaryReaderEx br, int version) {
        this.Position = br.ReadVector3();
        this.Section0Index = br.ReadInt16();
        this.MainID = br.ReadInt16();
        if (version < 2) {
          this.SiblingDistances = new List<float>(
              ((IEnumerable<ushort>) br.ReadUInt16s(16)).Select<ushort, float>(
                  (Func<ushort, float>) (s => s != ushort.MaxValue
                                                  ? (float) s * 0.01f
                                                  : -1f)));
        } else {
          int count = br.ReadInt32();
          this.Unk14 = br.ReadInt32();
          int num = br.ReadInt32();
          br.AssertInt32(new int[1]);
          this.SiblingDistances = new List<float>(
              ((IEnumerable<ushort>) br.GetUInt16s((long) num, count))
              .Select<ushort, float>(
                  (Func<ushort, float>) (s => s != ushort.MaxValue
                                                  ? (float) s * 0.01f
                                                  : -1f)));
        }
      }

      internal void Write(BinaryWriterEx bw, int version, int index) {
        bw.WriteVector3(this.Position);
        bw.WriteInt16(this.Section0Index);
        bw.WriteInt16(this.MainID);
        if (version < 2) {
          if (this.SiblingDistances.Count > 16)
            throw new InvalidDataException(
                "MapNode distance count must not exceed 16 in DS3/BB.");
          foreach (float siblingDistance in this.SiblingDistances)
            bw.WriteUInt16((double) siblingDistance == -1.0
                               ? ushort.MaxValue
                               : (ushort) Math.Round(
                                   (double) siblingDistance * 100.0));
          for (int index1 = 0;
               index1 < 16 - this.SiblingDistances.Count;
               ++index1)
            bw.WriteUInt16(ushort.MaxValue);
        } else {
          bw.WriteInt32(this.SiblingDistances.Count);
          bw.WriteInt32(this.Unk14);
          bw.ReserveInt32(string.Format("SubIDsOffset{0}", (object) index));
          bw.WriteInt32(0);
        }
      }

      internal void WriteSubIDs(BinaryWriterEx bw, int version, int index) {
        if (version < 2)
          return;
        bw.FillInt32(string.Format("SubIDsOffset{0}", (object) index),
                     (int) bw.Position);
        foreach (float siblingDistance in this.SiblingDistances)
          bw.WriteUInt16((double) siblingDistance == -1.0
                             ? ushort.MaxValue
                             : (ushort) Math.Round(
                                 (double) siblingDistance * 100.0));
      }

      public override string ToString() {
        return string.Format("{0} {1} {2} [{3} SubIDs]",
                             (object) this.Position,
                             (object) this.Section0Index,
                             (object) this.MainID,
                             (object) this.SiblingDistances.Count);
      }
    }
  }
}