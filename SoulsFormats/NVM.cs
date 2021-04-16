// Decompiled with JetBrains decompiler
// Type: SoulsFormats.NVM
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class NVM : SoulsFile<NVM> {
    public bool BigEndian;
    public List<Vector3> Vertices;
    public List<NVM.Triangle> Triangles;
    public NVM.Box RootBox;
    public List<NVM.Entity> Entities;

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      this.BigEndian = br.AssertInt32(1, 16777216) != 1;
      br.BigEndian = this.BigEndian;
      int capacity1 = br.ReadInt32();
      br.AssertInt32(128);
      int capacity2 = br.ReadInt32();
      br.AssertInt32(128 + capacity1 * 12);
      int num1 = br.ReadInt32();
      br.AssertInt32(new int[1]);
      int capacity3 = br.ReadInt32();
      int num2 = br.ReadInt32();
      for (int index = 0; index < 23; ++index)
        br.AssertInt32(new int[1]);
      this.Vertices = new List<Vector3>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Vertices.Add(br.ReadVector3());
      this.Triangles = new List<NVM.Triangle>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Triangles.Add(new NVM.Triangle(br));
      br.Position = (long) num1;
      this.RootBox = new NVM.Box(br);
      br.Position = (long) num2;
      this.Entities = new List<NVM.Entity>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.Entities.Add(new NVM.Entity(br));
    }

    protected override void Write(BinaryWriterEx bw) {
      // ISSUE: reference to a compiler-generated field
      bw.BigEndian = this.BigEndian;
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(1);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.Vertices.Count);
      // ISSUE: reference to a compiler-generated field
      bw.ReserveInt32("VertexOffset");
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.Triangles.Count);
      // ISSUE: reference to a compiler-generated field
      bw.ReserveInt32("TriangleOffset");
      // ISSUE: reference to a compiler-generated field
      bw.ReserveInt32("RootBoxOffset");
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(0);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.Entities.Count);
      // ISSUE: reference to a compiler-generated field
      bw.ReserveInt32("EntityOffset");
      for (int index = 0; index < 23; ++index) {
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt32(0);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.FillInt32("VertexOffset",
                   (int) bw.Position);
      foreach (Vector3 vertex in this.Vertices) {
        // ISSUE: reference to a compiler-generated field
        bw.WriteVector3(vertex);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.FillInt32("TriangleOffset",
                   (int) bw.Position);
      foreach (NVM.Triangle triangle in this.Triangles) {
        // ISSUE: reference to a compiler-generated field
        triangle.Write(bw);
      }
      // ISSUE: reference to a compiler-generated field
      var boxTriangleIndexOffsets = new Queue<int>();


      void WriteBoxTriangleIndices(NVM.Box box) {
        if (box == null)
          return;
        WriteBoxTriangleIndices(box.ChildBox1);
        WriteBoxTriangleIndices(box.ChildBox2);
        WriteBoxTriangleIndices(box.ChildBox3);
        WriteBoxTriangleIndices(box.ChildBox4);
        if (box.TriangleIndices.Count == 0) {
          // ISSUE: reference to a compiler-generated field
          boxTriangleIndexOffsets.Enqueue(0);
        } else {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          boxTriangleIndexOffsets.Enqueue((int) bw.Position);
          // ISSUE: reference to a compiler-generated field
          bw.WriteInt32s((IList<int>) box.TriangleIndices);
        }
      }

      WriteBoxTriangleIndices(this.RootBox);

      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      int num = this.RootBox.Write(bw,
                                   boxTriangleIndexOffsets);
      // ISSUE: reference to a compiler-generated field
      bw.FillInt32("RootBoxOffset", num);
      List<int> intList = new List<int>();
      foreach (NVM.Entity entity in this.Entities) {
        // ISSUE: reference to a compiler-generated field
        intList.Add((int) bw.Position);
        // ISSUE: reference to a compiler-generated field
        bw.WriteInt32s((IList<int>) entity.TriangleIndices);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.FillInt32("EntityOffset",
                   (int) bw.Position);
      for (int index = 0; index < this.Entities.Count; ++index) {
        // ISSUE: reference to a compiler-generated field
        this.Entities[index].Write(bw, intList[index]);
      }
    }

    public class Triangle {
      public int VertexIndex1;
      public int VertexIndex2;
      public int VertexIndex3;
      public int EdgeIndex1;
      public int EdgeIndex2;
      public int EdgeIndex3;
      public int ObstacleCount;
      public NVM.TriangleFlags Flags;

      internal Triangle(BinaryReaderEx br) {
        this.VertexIndex1 = br.ReadInt32();
        this.VertexIndex2 = br.ReadInt32();
        this.VertexIndex3 = br.ReadInt32();
        this.EdgeIndex1 = br.ReadInt32();
        this.EdgeIndex2 = br.ReadInt32();
        this.EdgeIndex3 = br.ReadInt32();
        int num = br.ReadInt32();
        this.ObstacleCount = num >> 2 & 16383;
        this.Flags = (NVM.TriangleFlags) (num >> 16);
        if ((num & 3) != 0)
          throw new FormatException(
              "Lower 2 bits of obstacle count are expected to be 0, but weren't.");
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(this.VertexIndex1);
        bw.WriteInt32(this.VertexIndex2);
        bw.WriteInt32(this.VertexIndex3);
        bw.WriteInt32(this.EdgeIndex1);
        bw.WriteInt32(this.EdgeIndex2);
        bw.WriteInt32(this.EdgeIndex3);
        int num = this.ObstacleCount << 2 | (int) this.Flags << 16;
        bw.WriteInt32(num);
      }

      public override string ToString() {
        return string.Format("[{0}, {1}, {2}] [{3}, {4}, {5}] {6} {7}",
                             (object) this.VertexIndex1,
                             (object) this.VertexIndex2,
                             (object) this.VertexIndex3,
                             (object) this.EdgeIndex1,
                             (object) this.EdgeIndex2,
                             (object) this.EdgeIndex3,
                             (object) this.Flags,
                             (object) this.ObstacleCount);
      }
    }

    [Flags]
    public enum TriangleFlags {
      NONE = 0,
      INSIDE_WALL = 1,
      BLOCK_GATE = 2,
      CLOSED_DOOR = 4,
      DOOR = 8,
      HOLE = 16,            // 0x00000010
      LADDER = 32,          // 0x00000020
      LARGE_SPACE = 64,     // 0x00000040
      EDGE = 128,           // 0x00000080
      EVENT = 256,          // 0x00000100
      LANDING_POINT = 512,  // 0x00000200
      FLOOR_TO_WALL = 1024, // 0x00000400
      DEGENERATE = 2048,    // 0x00000800
      WALL = 4096,          // 0x00001000
      BLOCK = 8192,         // 0x00002000
      GATE = 16384,         // 0x00004000
      DISABLE = 32768,      // 0x00008000
    }

    public class Box {
      public Vector3 Corner1;
      public Vector3 Corner2;
      public List<int> TriangleIndices;
      public NVM.Box ChildBox1;
      public NVM.Box ChildBox2;
      public NVM.Box ChildBox3;
      public NVM.Box ChildBox4;

      internal Box(BinaryReaderEx br) {
        // ISSUE: reference to a compiler-generated field
        this.Corner1 = br.ReadVector3();
        // ISSUE: reference to a compiler-generated field
        int count = br.ReadInt32();
        // ISSUE: reference to a compiler-generated field
        this.Corner2 = br.ReadVector3();
        // ISSUE: reference to a compiler-generated field
        int num = br.ReadInt32();
        // ISSUE: reference to a compiler-generated field
        int boxOffset1 = br.ReadInt32();
        // ISSUE: reference to a compiler-generated field
        int boxOffset2 = br.ReadInt32();
        // ISSUE: reference to a compiler-generated field
        int boxOffset3 = br.ReadInt32();
        // ISSUE: reference to a compiler-generated field
        int boxOffset4 = br.ReadInt32();
        // ISSUE: reference to a compiler-generated field
        br.AssertInt32(new int[1]);
        // ISSUE: reference to a compiler-generated field
        br.AssertInt32(new int[1]);
        // ISSUE: reference to a compiler-generated field
        br.AssertInt32(new int[1]);
        // ISSUE: reference to a compiler-generated field
        br.AssertInt32(new int[1]);
        // ISSUE: reference to a compiler-generated field
        this.TriangleIndices = count != 0
                                   ? new List<int>(
                                       (IEnumerable<int>)
                                       br.GetInt32s(
                                           (long) num,
                                           count))
                                   : new List<int>();

        NVM.Box ReadBox(int boxOffset) {
          if (boxOffset == 0)
            return (NVM.Box) null;
          // ISSUE: reference to a compiler-generated field
          br.Position = (long) boxOffset;
          // ISSUE: reference to a compiler-generated field
          return new NVM.Box(br);
        }

        this.ChildBox1 = ReadBox(boxOffset1);
        this.ChildBox2 = ReadBox(boxOffset2);
        this.ChildBox3 = ReadBox(boxOffset3);
        this.ChildBox4 = ReadBox(boxOffset4);
      }

      internal int Write(BinaryWriterEx bw, Queue<int> triangleIndexOffsets) {
        NVM.Box childBox1 = this.ChildBox1;
        int num1 = childBox1 != null
                       ? childBox1.Write(bw, triangleIndexOffsets)
                       : 0;
        NVM.Box childBox2 = this.ChildBox2;
        int num2 = childBox2 != null
                       ? childBox2.Write(bw, triangleIndexOffsets)
                       : 0;
        NVM.Box childBox3 = this.ChildBox3;
        int num3 = childBox3 != null
                       ? childBox3.Write(bw, triangleIndexOffsets)
                       : 0;
        NVM.Box childBox4 = this.ChildBox4;
        int num4 = childBox4 != null
                       ? childBox4.Write(bw, triangleIndexOffsets)
                       : 0;
        int position = (int) bw.Position;
        bw.WriteVector3(this.Corner1);
        bw.WriteInt32(this.TriangleIndices.Count);
        bw.WriteVector3(this.Corner2);
        bw.WriteInt32(triangleIndexOffsets.Dequeue());
        bw.WriteInt32(num1);
        bw.WriteInt32(num2);
        bw.WriteInt32(num3);
        bw.WriteInt32(num4);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        return position;
      }
    }

    public class Entity {
      public int EntityID;
      public List<int> TriangleIndices;

      internal Entity(BinaryReaderEx br) {
        this.EntityID = br.ReadInt32();
        int num = br.ReadInt32();
        int count = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.TriangleIndices =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num, count));
      }

      internal void Write(BinaryWriterEx bw, int indexOffset) {
        bw.WriteInt32(this.EntityID);
        bw.WriteInt32(indexOffset);
        bw.WriteInt32(this.TriangleIndices.Count);
        bw.WriteInt32(0);
      }
    }
  }
}