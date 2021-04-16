// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MCG
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class MCG : SoulsFile<MCG>
  {
    public bool BigEndian { get; set; }

    public int Unk04 { get; set; }

    public List<MCG.Node> Nodes { get; set; }

    public List<MCG.Edge> Edges { get; set; }

    public int Unk18 { get; set; }

    public int Unk1C { get; set; }

    public MCG()
    {
      this.Nodes = new List<MCG.Node>();
      this.Edges = new List<MCG.Edge>();
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = true;
      this.BigEndian = br.AssertInt32(1, 16777216) == 1;
      br.BigEndian = this.BigEndian;
      this.Unk04 = br.ReadInt32();
      int capacity1 = br.ReadInt32();
      int num1 = br.ReadInt32();
      int capacity2 = br.ReadInt32();
      int num2 = br.ReadInt32();
      this.Unk18 = br.ReadInt32();
      this.Unk1C = br.ReadInt32();
      br.Position = (long) num1;
      this.Nodes = new List<MCG.Node>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Nodes.Add(new MCG.Node(br));
      br.Position = (long) num2;
      this.Edges = new List<MCG.Edge>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Edges.Add(new MCG.Edge(br));
    }

    public override bool Validate(out Exception ex)
    {
      if (!SoulsFile<MCG>.ValidateNull((object) this.Nodes, "Nodes may not be null.", out ex) || !SoulsFile<MCG>.ValidateNull((object) this.Edges, "Edges may not be null.", out ex))
        return false;
      for (int index1 = 0; index1 < this.Nodes.Count; ++index1)
      {
        MCG.Node node = this.Nodes[index1];
        if (!SoulsFile<MCG>.ValidateNull((object) node, string.Format("{0}[{1}]: {2} may not be null.", (object) "Nodes", (object) index1, (object) "Node"), out ex) || !SoulsFile<MCG>.ValidateNull((object) node.ConnectedNodeIndices, string.Format("{0}[{1}]: {2} may not be null.", (object) "Nodes", (object) index1, (object) "ConnectedNodeIndices"), out ex) || !SoulsFile<MCG>.ValidateNull((object) node.ConnectedEdgeIndices, string.Format("{0}[{1}]: {2} may not be null.", (object) "Nodes", (object) index1, (object) "ConnectedEdgeIndices"), out ex))
          return false;
        if (node.ConnectedNodeIndices.Count != node.ConnectedEdgeIndices.Count)
        {
          ex = (Exception) new InvalidDataException(string.Format("{0}[{1}]: {2} count must equal {3} count.", (object) "Nodes", (object) index1, (object) "ConnectedNodeIndices", (object) "ConnectedEdgeIndices"));
          return false;
        }
        for (int index2 = 0; index2 < node.ConnectedNodeIndices.Count; ++index2)
        {
          int connectedNodeIndex = node.ConnectedNodeIndices[index2];
          int connectedEdgeIndex = node.ConnectedEdgeIndices[index2];
          if (SoulsFile<MCG>.ValidateIndex((long) this.Nodes.Count, (long) connectedNodeIndex, string.Format("{0}[{1}].{2}[{3}]: Index out of range: {4}", (object) "Nodes", (object) index1, (object) "ConnectedNodeIndices", (object) index2, (object) connectedNodeIndex), out ex))
          {
            if (SoulsFile<MCG>.ValidateIndex((long) this.Edges.Count, (long) connectedEdgeIndex, string.Format("{0}[{1}].{2}[{3}]: Index out of range: {4}", (object) "Nodes", (object) index1, (object) "ConnectedEdgeIndices", (object) index2, (object) connectedEdgeIndex), out ex))
              continue;
          }
          return false;
        }
      }
      for (int index = 0; index < this.Edges.Count; ++index)
      {
        MCG.Edge edge = this.Edges[index];
        if (SoulsFile<MCG>.ValidateNull((object) edge, string.Format("{0}[{1}]: {2} may not be null.", (object) "Edges", (object) index, (object) "Edge"), out ex) && SoulsFile<MCG>.ValidateNull((object) edge.UnkIndicesA, string.Format("{0}[{1}]: {2} may not be null.", (object) "Edges", (object) index, (object) "UnkIndicesA"), out ex) && SoulsFile<MCG>.ValidateNull((object) edge.UnkIndicesB, string.Format("{0}[{1}]: {2} may not be null.", (object) "Edges", (object) index, (object) "UnkIndicesB"), out ex))
        {
          if (SoulsFile<MCG>.ValidateIndex((long) this.Nodes.Count, (long) edge.NodeIndexA, string.Format("{0}[{1}].{2}: Index out of range: {3}", (object) "Edges", (object) index, (object) "NodeIndexA", (object) edge.NodeIndexA), out ex))
          {
            if (SoulsFile<MCG>.ValidateIndex((long) this.Nodes.Count, (long) edge.NodeIndexB, string.Format("{0}[{1}].{2}: Index out of range: {3}", (object) "Edges", (object) index, (object) "NodeIndexB", (object) edge.NodeIndexB), out ex))
              continue;
          }
        }
        return false;
      }
      ex = (Exception) null;
      return true;
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bw.BigEndian = this.BigEndian;
      bw.WriteInt32(1);
      bw.WriteInt32(this.Unk04);
      bw.WriteInt32(this.Nodes.Count);
      bw.ReserveInt32("NodesOffset");
      bw.WriteInt32(this.Edges.Count);
      bw.ReserveInt32("EdgesOffset");
      bw.WriteInt32(this.Unk18);
      bw.WriteInt32(this.Unk1C);
      long[] numArray1 = new long[this.Edges.Count];
      long[] numArray2 = new long[this.Edges.Count];
      for (int index = 0; index < this.Edges.Count; ++index)
      {
        numArray1[index] = bw.Position;
        bw.WriteInt32s((IList<int>) this.Edges[index].UnkIndicesA);
        numArray2[index] = bw.Position;
        bw.WriteInt32s((IList<int>) this.Edges[index].UnkIndicesB);
      }
      long[] numArray3 = new long[this.Nodes.Count];
      long[] numArray4 = new long[this.Nodes.Count];
      for (int index = 0; index < this.Nodes.Count; ++index)
      {
        MCG.Node node = this.Nodes[index];
        numArray3[index] = node.ConnectedNodeIndices.Count == 0 ? 0L : bw.Position;
        bw.WriteInt32s((IList<int>) node.ConnectedNodeIndices);
        numArray4[index] = node.ConnectedEdgeIndices.Count == 0 ? 0L : bw.Position;
        bw.WriteInt32s((IList<int>) node.ConnectedEdgeIndices);
      }
      bw.FillInt32("EdgesOffset", (int) bw.Position);
      for (int index = 0; index < this.Edges.Count; ++index)
        this.Edges[index].Write(bw, numArray1[index], numArray2[index]);
      bw.FillInt32("NodesOffset", (int) bw.Position);
      for (int index = 0; index < this.Nodes.Count; ++index)
        this.Nodes[index].Write(bw, numArray3[index], numArray4[index]);
    }

    public class Node
    {
      public Vector3 Position { get; set; }

      public List<int> ConnectedNodeIndices { get; set; }

      public List<int> ConnectedEdgeIndices { get; set; }

      public int Unk18 { get; set; }

      public int Unk1C { get; set; }

      public Node()
      {
        this.ConnectedNodeIndices = new List<int>();
        this.ConnectedEdgeIndices = new List<int>();
        this.Unk18 = -1;
      }

      internal Node(BinaryReaderEx br)
      {
        int count = br.ReadInt32();
        this.Position = br.ReadVector3();
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        this.Unk18 = br.ReadInt32();
        this.Unk1C = br.ReadInt32();
        this.ConnectedNodeIndices = new List<int>((IEnumerable<int>) br.GetInt32s((long) num1, count));
        this.ConnectedEdgeIndices = new List<int>((IEnumerable<int>) br.GetInt32s((long) num2, count));
      }

      internal void Write(BinaryWriterEx bw, long nodeIndicesOffset, long edgeIndicesOffset)
      {
        bw.WriteInt32(this.ConnectedNodeIndices.Count);
        bw.WriteVector3(this.Position);
        bw.WriteInt32((int) nodeIndicesOffset);
        bw.WriteInt32((int) edgeIndicesOffset);
        bw.WriteInt32(this.Unk18);
        bw.WriteInt32(this.Unk1C);
      }
    }

    public class Edge
    {
      public int NodeIndexA { get; set; }

      public List<int> UnkIndicesA { get; set; }

      public int NodeIndexB { get; set; }

      public List<int> UnkIndicesB { get; set; }

      public int MCPRoomIndex { get; set; }

      public uint MapID { get; set; }

      public float Unk20 { get; set; }

      public Edge()
      {
        this.UnkIndicesA = new List<int>();
        this.UnkIndicesB = new List<int>();
      }

      internal Edge(BinaryReaderEx br)
      {
        this.NodeIndexA = br.ReadInt32();
        int count1 = br.ReadInt32();
        int num1 = br.ReadInt32();
        this.NodeIndexB = br.ReadInt32();
        int count2 = br.ReadInt32();
        int num2 = br.ReadInt32();
        this.MCPRoomIndex = br.ReadInt32();
        this.MapID = br.ReadUInt32();
        this.Unk20 = br.ReadSingle();
        this.UnkIndicesA = new List<int>((IEnumerable<int>) br.GetInt32s((long) num1, count1));
        this.UnkIndicesB = new List<int>((IEnumerable<int>) br.GetInt32s((long) num2, count2));
      }

      internal void Write(BinaryWriterEx bw, long indicesOffsetA, long indicesOffsetB)
      {
        bw.WriteInt32(this.NodeIndexA);
        bw.WriteInt32(this.UnkIndicesA.Count);
        bw.WriteInt32((int) indicesOffsetA);
        bw.WriteInt32(this.NodeIndexB);
        bw.WriteInt32(this.UnkIndicesB.Count);
        bw.WriteInt32((int) indicesOffsetB);
        bw.WriteInt32(this.MCPRoomIndex);
        bw.WriteUInt32(this.MapID);
        bw.WriteSingle(this.Unk20);
      }
    }
  }
}
