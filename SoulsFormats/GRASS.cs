// Decompiled with JetBrains decompiler
// Type: SoulsFormats.GRASS
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class GRASS : SoulsFile<GRASS> {
    public List<GRASS.Volume> BoundingVolumeHierarchy { get; set; }

    public List<GRASS.Vertex> Vertices { get; set; }

    public List<GRASS.Face> Faces { get; set; }

    public GRASS() {
      this.BoundingVolumeHierarchy = new List<GRASS.Volume>();
      this.Vertices = new List<GRASS.Vertex>();
      this.Faces = new List<GRASS.Face>();
    }

    protected override bool Is(BinaryReaderEx br) {
      if (br.Length < 40L)
        return false;
      int int32_1 = br.GetInt32(0L);
      int int32_2 = br.GetInt32(4L);
      int int32_3 = br.GetInt32(8L);
      int int32_4 = br.GetInt32(16L);
      int int32_5 = br.GetInt32(24L);
      int int32_6 = br.GetInt32(32L);
      return int32_1 == 1 &&
             int32_2 == 40 &&
             (int32_3 == 20 && int32_4 == 36) &&
             int32_5 == 24 &&
             int32_6 == 24;
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertInt32(1);
      br.AssertInt32(40);
      br.AssertInt32(20);
      int capacity1 = br.ReadInt32();
      br.AssertInt32(36);
      int capacity2 = br.ReadInt32();
      br.AssertInt32(24);
      int capacity3 = br.ReadInt32();
      br.AssertInt32(24);
      br.AssertInt32(capacity1);
      this.BoundingVolumeHierarchy = new List<GRASS.Volume>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.BoundingVolumeHierarchy.Add(new GRASS.Volume(br));
      this.Vertices = new List<GRASS.Vertex>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Vertices.Add(new GRASS.Vertex(br));
      this.Faces = new List<GRASS.Face>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.Faces.Add(new GRASS.Face(br));
      for (int index = 0; index < capacity1; ++index)
        this.BoundingVolumeHierarchy[index].BoundingBox =
            new GRASS.BoundingBox(br);
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = false;
      bw.WriteInt32(1);
      bw.WriteInt32(40);
      bw.WriteInt32(20);
      bw.WriteInt32(this.BoundingVolumeHierarchy.Count);
      bw.WriteInt32(36);
      bw.WriteInt32(this.Vertices.Count);
      bw.WriteInt32(24);
      bw.WriteInt32(this.Faces.Count);
      bw.WriteInt32(24);
      bw.WriteInt32(this.BoundingVolumeHierarchy.Count);
      foreach (GRASS.Volume volume in this.BoundingVolumeHierarchy)
        volume.Write(bw);
      foreach (GRASS.Vertex vertex in this.Vertices)
        vertex.Write(bw);
      foreach (GRASS.Face face in this.Faces)
        face.Write(bw);
      foreach (GRASS.Volume volume in this.BoundingVolumeHierarchy)
        volume.BoundingBox.Write(bw);
    }

    public class Volume {
      public int StartChildIndex { get; set; }

      public int EndChildIndex { get; set; }

      public int StartFaceIndex { get; set; }

      public int EndFaceIndex { get; set; }

      public int Unk10 { get; set; }

      public GRASS.BoundingBox BoundingBox { get; set; }

      public Volume() {}

      internal Volume(BinaryReaderEx br) {
        this.StartChildIndex = br.ReadInt32();
        this.EndChildIndex = br.ReadInt32();
        this.StartFaceIndex = br.ReadInt32();
        this.EndFaceIndex = br.ReadInt32();
        this.Unk10 = br.ReadInt32();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(this.StartChildIndex);
        bw.WriteInt32(this.EndChildIndex);
        bw.WriteInt32(this.StartFaceIndex);
        bw.WriteInt32(this.EndFaceIndex);
        bw.WriteInt32(this.Unk10);
      }
    }

    public class Vertex {
      public Vector3 Position { get; set; }

      public float[] GrassDensities { get; private set; }

      public Vertex() {
        this.GrassDensities = new float[6];
      }

      internal Vertex(BinaryReaderEx br) {
        this.Position = br.ReadVector3();
        this.GrassDensities = br.ReadSingles(6);
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteVector3(this.Position);
        bw.WriteSingles((IList<float>) this.GrassDensities);
      }
    }

    public class Face {
      public Vector3 Unk00 { get; set; }

      public int VertexIndexA { get; set; }

      public int VertexIndexB { get; set; }

      public int VertexIndexC { get; set; }

      public Face() {}

      internal Face(BinaryReaderEx br) {
        this.Unk00 = br.ReadVector3();
        this.VertexIndexA = br.ReadInt32();
        this.VertexIndexB = br.ReadInt32();
        this.VertexIndexC = br.ReadInt32();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteVector3(this.Unk00);
        bw.WriteInt32(this.VertexIndexA);
        bw.WriteInt32(this.VertexIndexB);
        bw.WriteInt32(this.VertexIndexC);
      }
    }

    public struct BoundingBox {
      public Vector3 Min { get; set; }

      public Vector3 Max { get; set; }

      public BoundingBox(Vector3 min, Vector3 max) {
        this.Min = min;
        this.Max = max;
      }

      internal BoundingBox(BinaryReaderEx br) {
        this.Min = br.ReadVector3();
        this.Max = br.ReadVector3();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteVector3(this.Min);
        bw.WriteVector3(this.Max);
      }

      public override string ToString() {
        return string.Format("{0:F3} - {1:F3}",
                             (object) this.Min,
                             (object) this.Max);
      }
    }
  }
}