// Decompiled with JetBrains decompiler
// Type: SoulsFormats.FLVER0
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
  public class FLVER0 : SoulsFile<FLVER0>, IFlver {
    public bool BigEndian { get; set; }

    public int Version { get; set; }

    public Vector3 BoundingBoxMin { get; set; }

    public Vector3 BoundingBoxMax { get; set; }

    public byte VertexIndexSize { get; set; }

    public bool Unicode { get; set; }

    public byte Unk4A { get; set; }

    public byte Unk4B { get; set; }

    public int Unk4C { get; set; }

    public int Unk5C { get; set; }

    public List<FLVER.Dummy> Dummies { get; set; }

    IReadOnlyList<FLVER.Dummy> IFlver.Dummies {
      get { return (IReadOnlyList<FLVER.Dummy>) this.Dummies; }
    }

    public List<FLVER0.Material> Materials { get; set; }

    IReadOnlyList<IFlverMaterial> IFlver.Materials {
      get { return (IReadOnlyList<IFlverMaterial>) this.Materials; }
    }

    public List<FLVER.Bone> Bones { get; set; }

    IReadOnlyList<FLVER.Bone> IFlver.Bones {
      get { return (IReadOnlyList<FLVER.Bone>) this.Bones; }
    }

    public List<FLVER0.Mesh> Meshes { get; set; }

    IReadOnlyList<IFlverMesh> IFlver.Meshes {
      get { return (IReadOnlyList<IFlverMesh>) this.Meshes; }
    }

    protected override bool Is(BinaryReaderEx br) {
      if (br.Length < 12L)
        return false;
      string str1 = br.ReadASCII(6);
      string str2 = br.ReadASCII(2);
      if (str2 == "L\0")
        br.BigEndian = false;
      else if (str2 == "B\0")
        br.BigEndian = true;
      int num = br.ReadInt32();
      return str1 == "FLVER\0" && num >= 0 && num < 131072;
    }

    protected override void Read(BinaryReaderEx br) {
      br.AssertASCII("FLVER\0");
      this.BigEndian = br.AssertASCII("L\0", "B\0") == "B\0";
      br.BigEndian = this.BigEndian;
      this.Version = br.AssertInt32(14, 15, 16, 18, 19, 20, 21, 65538, 65539);
      int dataOffset = br.ReadInt32();
      br.ReadInt32();
      int capacity1 = br.ReadInt32();
      int capacity2 = br.ReadInt32();
      int capacity3 = br.ReadInt32();
      int capacity4 = br.ReadInt32();
      br.ReadInt32();
      this.BoundingBoxMin = br.ReadVector3();
      this.BoundingBoxMax = br.ReadVector3();
      br.ReadInt32();
      br.ReadInt32();
      this.VertexIndexSize = br.AssertByte((byte) 16, (byte) 32);
      this.Unicode = br.ReadBoolean();
      this.Unk4A = br.ReadByte();
      this.Unk4B = br.ReadByte();
      this.Unk4C = br.ReadInt32();
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      this.Unk5C = (int) br.ReadByte();
      int num1 = (int) br.AssertByte(new byte[1]);
      int num2 = (int) br.AssertByte(new byte[1]);
      int num3 = (int) br.AssertByte(new byte[1]);
      br.AssertPattern(32, (byte) 0);
      this.Dummies = new List<FLVER.Dummy>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Dummies.Add(new FLVER.Dummy(br, this.Version));
      this.Materials = new List<FLVER0.Material>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Materials.Add(new FLVER0.Material(br, this));
      this.Bones = new List<FLVER.Bone>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.Bones.Add(new FLVER.Bone(br, this.Unicode));
      this.Meshes = new List<FLVER0.Mesh>(capacity4);
      for (int index = 0; index < capacity4; ++index)
        this.Meshes.Add(new FLVER0.Mesh(br, this, dataOffset));
    }

    public class BufferLayout : List<FLVER.LayoutMember> {
      public int Size {
        get {
          return this.Sum<FLVER.LayoutMember>(
              (Func<FLVER.LayoutMember, int>) (member => member.Size));
        }
      }

      internal BufferLayout(BinaryReaderEx br) {
        short num1 = br.ReadInt16();
        short num2 = br.ReadInt16();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        int structOffset = 0;
        this.Capacity = (int) num1;
        for (int index = 0; index < (int) num1; ++index) {
          FLVER.LayoutMember layoutMember =
              new FLVER.LayoutMember(br, structOffset);
          structOffset += layoutMember.Size;
          this.Add(layoutMember);
        }
        if (this.Size != (int) num2)
          throw new InvalidDataException("Mismatched buffer layout size.");
      }
    }

    public class Material : IFlverMaterial {
      public string Name { get; set; }

      public string MTD { get; set; }

      public List<FLVER0.Texture> Textures { get; set; }

      IReadOnlyList<IFlverTexture> IFlverMaterial.Textures {
        get { return (IReadOnlyList<IFlverTexture>) this.Textures; }
      }

      public List<FLVER0.BufferLayout> Layouts { get; set; }

      internal Material(BinaryReaderEx br, FLVER0 flv) {
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int num3 = br.ReadInt32();
        int num4 = br.ReadInt32();
        br.ReadInt32();
        int num5 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Name = flv.Unicode
                        ? br.GetUTF16((long) num1)
                        : br.GetShiftJIS((long) num1);
        this.MTD = flv.Unicode
                       ? br.GetUTF16((long) num2)
                       : br.GetShiftJIS((long) num2);
        br.StepIn((long) num3);
        byte num6 = br.ReadByte();
        int num7 = (int) br.AssertByte(new byte[1]);
        int num8 = (int) br.AssertByte(new byte[1]);
        int num9 = (int) br.AssertByte(new byte[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Textures = new List<FLVER0.Texture>((int) num6);
        for (int index = 0; index < (int) num6; ++index)
          this.Textures.Add(new FLVER0.Texture(br, flv));
        br.StepOut();
        if (num5 != 0) {
          br.StepIn((long) num5);
          int capacity = br.ReadInt32();
          br.AssertInt32((int) br.Position + 12);
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.Layouts = new List<FLVER0.BufferLayout>(capacity);
          for (int index = 0; index < capacity; ++index) {
            int num10 = br.ReadInt32();
            br.StepIn((long) num10);
            this.Layouts.Add(new FLVER0.BufferLayout(br));
            br.StepOut();
          }
          br.StepOut();
        } else {
          this.Layouts = new List<FLVER0.BufferLayout>(1);
          br.StepIn((long) num4);
          this.Layouts.Add(new FLVER0.BufferLayout(br));
          br.StepOut();
        }
      }
    }

    public class Mesh : IFlverMesh {
      public byte Dynamic { get; set; }

      public byte MaterialIndex { get; set; }

      int IFlverMesh.MaterialIndex {
        get { return (int) this.MaterialIndex; }
      }

      public bool Unk02 { get; set; }

      public byte Unk03 { get; set; }

      public short DefaultBoneIndex { get; set; }

      public short[] BoneIndices { get; private set; }

      public short Unk46 { get; set; }

      public List<int> VertexIndices { get; set; }

      public List<FLVER.Vertex> Vertices { get; set; }

      IReadOnlyList<FLVER.Vertex> IFlverMesh.Vertices {
        get { return (IReadOnlyList<FLVER.Vertex>) this.Vertices; }
      }

      public int LayoutIndex { get; set; }

      internal Mesh(BinaryReaderEx br, FLVER0 flv, int dataOffset) {
        this.Dynamic = br.ReadByte();
        this.MaterialIndex = br.ReadByte();
        this.Unk02 = br.ReadBoolean();
        this.Unk03 = br.ReadByte();
        int count = br.ReadInt32();
        int capacity = br.ReadInt32();
        this.DefaultBoneIndex = br.ReadInt16();
        this.BoneIndices = br.ReadInt16s(28);
        this.Unk46 = br.ReadInt16();
        br.ReadInt32();
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int num3 = br.ReadInt32();
        int num4 = br.ReadInt32();
        int num5 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        if (flv.VertexIndexSize == (byte) 16) {
          this.VertexIndices = new List<int>(capacity);
          foreach (int uint16 in br.GetUInt16s((long) (dataOffset + num1),
                                               count))
            this.VertexIndices.Add(uint16);
        } else if (flv.VertexIndexSize == (byte) 32)
          this.VertexIndices = new List<int>(
              (IEnumerable<int>) br.GetInt32s((long) (dataOffset + num1),
                                              count));
        FLVER0.VertexBuffer vertexBuffer;
        if (num4 == 0) {
          vertexBuffer = new FLVER0.VertexBuffer() {
              BufferLength = num2,
              BufferOffset = num3,
              LayoutIndex = 0
          };
        } else {
          br.StepIn((long) num4);
          List<FLVER0.VertexBuffer> vertexBufferList =
              FLVER0.VertexBuffer.ReadVertexBuffers(br);
          if (vertexBufferList.Count == 0)
            throw new NotSupportedException(
                "First vertex buffer list is expected to contain at least 1 buffer.");
          for (int index = 1; index < vertexBufferList.Count; ++index) {
            if (vertexBufferList[index].BufferLength != 0)
              throw new NotSupportedException(
                  "Vertex buffers after the first one in the first buffer list are expected to be empty.");
          }
          vertexBuffer = vertexBufferList[0];
          br.StepOut();
        }
        if (num5 != 0) {
          br.StepIn((long) num5);
          if (FLVER0.VertexBuffer.ReadVertexBuffers(br).Count != 0)
            throw new NotSupportedException(
                "Second vertex buffer list is expected to contain exactly 0 buffers.");
          br.StepOut();
        }
        br.StepIn((long) (dataOffset + vertexBuffer.BufferOffset));
        this.LayoutIndex = vertexBuffer.LayoutIndex;
        FLVER0.BufferLayout layout = flv
                                     .Materials[(int) this.MaterialIndex]
                                     .Layouts[this.LayoutIndex];
        float uvFactor = 1024f;
        if (!br.BigEndian)
          uvFactor = 2048f;
        this.Vertices = new List<FLVER.Vertex>(capacity);
        for (int index = 0; index < capacity; ++index) {
          FLVER.Vertex vertex = new FLVER.Vertex(0, 0, 0);
          vertex.Read(br, (List<FLVER.LayoutMember>) layout, uvFactor);
          this.Vertices.Add(vertex);
        }
        br.StepOut();
      }

      public List<FLVER.Vertex[]> GetFaces(int version) {
        List<int> intList = this.Triangulate(version);
        List<FLVER.Vertex[]> vertexArrayList = new List<FLVER.Vertex[]>();
        for (int index = 0; index < intList.Count; index += 3)
          vertexArrayList.Add(new FLVER.Vertex[3] {
              this.Vertices[intList[index]],
              this.Vertices[intList[index + 1]],
              this.Vertices[intList[index + 2]]
          });
        return vertexArrayList;
      }

      public List<int> Triangulate(int version) {
        List<int> intList = new List<int>();
        if (version >= 21 && this.Unk03 == (byte) 0) {
          intList = new List<int>((IEnumerable<int>) this.VertexIndices);
        } else {
          bool flag1 = false;
          bool flag2 = false;
          for (int index = 0; index < this.VertexIndices.Count - 2; ++index) {
            int vertexIndex1 = this.VertexIndices[index];
            int vertexIndex2 = this.VertexIndices[index + 1];
            int vertexIndex3 = this.VertexIndices[index + 2];
            if (vertexIndex1 == (int) ushort.MaxValue ||
                vertexIndex2 == (int) ushort.MaxValue ||
                vertexIndex3 == (int) ushort.MaxValue) {
              flag1 = true;
            } else {
              if (vertexIndex1 != vertexIndex2 &&
                  vertexIndex1 != vertexIndex3 &&
                  vertexIndex2 != vertexIndex3) {
                if (flag1) {
                  FLVER.Vertex vertex1 = this.Vertices[vertexIndex1];
                  FLVER.Vertex vertex2 = this.Vertices[vertexIndex2];
                  FLVER.Vertex vertex3 = this.Vertices[vertexIndex3];
                  Vector3 vector2 = Vector3.Normalize(
                      (vertex1.Normal + vertex2.Normal + vertex3.Normal) / 3f);
                  Vector3 vector1 = Vector3.Normalize(
                      Vector3.Cross(vertex2.Position - vertex1.Position,
                                    vertex3.Position - vertex1.Position));
                  flag2 = (double) Vector3.Dot(vector1, vector2) /
                          ((double) vector1.Length() *
                           (double) vector2.Length()) >=
                          0.0;
                  flag1 = false;
                }
                if (!flag2) {
                  intList.Add(vertexIndex1);
                  intList.Add(vertexIndex2);
                  intList.Add(vertexIndex3);
                } else {
                  intList.Add(vertexIndex3);
                  intList.Add(vertexIndex2);
                  intList.Add(vertexIndex1);
                }
              }
              flag2 = !flag2;
            }
          }
        }
        return intList;
      }
    }

    public class Texture : IFlverTexture {
      public string Type { get; set; }

      public string Path { get; set; }

      internal Texture(BinaryReaderEx br, FLVER0 flv) {
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Path = flv.Unicode
                        ? br.GetUTF16((long) num1)
                        : br.GetShiftJIS((long) num1);
        if (num2 > 0)
          this.Type = flv.Unicode
                          ? br.GetUTF16((long) num2)
                          : br.GetShiftJIS((long) num2);
        else
          this.Type = (string) null;
      }
    }

    private class VertexBuffer {
      public int LayoutIndex;
      public int BufferLength;
      public int BufferOffset;

      public VertexBuffer() {}

      internal VertexBuffer(BinaryReaderEx br) {
        this.LayoutIndex = br.ReadInt32();
        this.BufferLength = br.ReadInt32();
        this.BufferOffset = br.ReadInt32();
        br.AssertInt32(new int[1]);
      }

      internal static List<FLVER0.VertexBuffer> ReadVertexBuffers(
          BinaryReaderEx br) {
        int capacity = br.ReadInt32();
        int num = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        List<FLVER0.VertexBuffer> vertexBufferList =
            new List<FLVER0.VertexBuffer>(capacity);
        br.StepIn((long) num);
        for (int index = 0; index < capacity; ++index)
          vertexBufferList.Add(new FLVER0.VertexBuffer(br));
        br.StepOut();
        return vertexBufferList;
      }
    }
  }
}