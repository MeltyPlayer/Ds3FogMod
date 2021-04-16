// Decompiled with JetBrains decompiler
// Type: SoulsFormats.EDGE
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class EDGE : SoulsFile<EDGE> {
    public int ID { get; set; }

    public List<EDGE.Edge> Edges { get; set; }

    public EDGE() {
      this.Edges = new List<EDGE.Edge>();
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertInt32(4);
      int capacity = br.ReadInt32();
      this.ID = br.ReadInt32();
      br.AssertInt32(new int[1]);
      this.Edges = new List<EDGE.Edge>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Edges.Add(new EDGE.Edge(br));
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = false;
      bw.WriteInt32(4);
      bw.WriteInt32(this.Edges.Count);
      bw.WriteInt32(this.ID);
      bw.WriteInt32(0);
      foreach (EDGE.Edge edge in this.Edges)
        edge.Write(bw);
    }

    public enum EdgeType : byte {
      Grapple = 1,
      Hang = 2,
      Hug = 3,
    }

    public class Edge {
      public Vector3 V1 { get; set; }

      public Vector3 V2 { get; set; }

      public Vector3 V3 { get; set; }

      public float Unk2C { get; set; }

      public int Unk30 { get; set; }

      public EDGE.EdgeType Type { get; set; }

      public byte VariationID { get; set; }

      public byte Unk36 { get; set; }

      public Edge() {
        this.Type = EDGE.EdgeType.Grapple;
      }

      public EDGE.Edge Clone() {
        return (EDGE.Edge) this.MemberwiseClone();
      }

      internal Edge(BinaryReaderEx br) {
        this.V1 = br.ReadVector3();
        double num1 = (double) br.AssertSingle(1f);
        this.V2 = br.ReadVector3();
        double num2 = (double) br.AssertSingle(1f);
        this.V3 = br.ReadVector3();
        this.Unk2C = br.ReadSingle();
        this.Unk30 = br.ReadInt32();
        this.Type = br.ReadEnum8<EDGE.EdgeType>();
        this.VariationID = br.ReadByte();
        this.Unk36 = br.ReadByte();
        int num3 = (int) br.AssertByte(new byte[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteVector3(this.V1);
        bw.WriteSingle(1f);
        bw.WriteVector3(this.V2);
        bw.WriteSingle(1f);
        bw.WriteVector3(this.V3);
        bw.WriteSingle(this.Unk2C);
        bw.WriteInt32(this.Unk30);
        bw.WriteByte((byte) this.Type);
        bw.WriteByte(this.VariationID);
        bw.WriteByte(this.Unk36);
        bw.WriteByte((byte) 0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
      }

      public override string ToString() {
        return this.Type == EDGE.EdgeType.Grapple
                   ? string.Format("{0} Var:{1} {2} {3} {4} {5} {6}",
                                   (object) this.Type,
                                   (object) this.VariationID,
                                   (object) this.Unk30,
                                   (object) this.Unk36,
                                   (object) this.V1,
                                   (object) this.V2,
                                   (object) this.V3)
                   : string.Format("{0} Var:{1} {2} {3} {4} {5}",
                                   (object) this.Type,
                                   (object) this.VariationID,
                                   (object) this.Unk30,
                                   (object) this.Unk36,
                                   (object) this.V1,
                                   (object) this.V2);
      }
    }
  }
}