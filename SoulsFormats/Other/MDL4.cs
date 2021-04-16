// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Other.MDL4
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats.Other {
  [ComVisible(true)]
  public class MDL4 : SoulsFile<MDL4> {
    public int Version;
    public int Unk20;
    public Vector3 BoundingBoxMin;
    public Vector3 BoundingBoxMax;
    public int TrueFaceCount;
    public int TotalFaceCount;
    public List<MDL4.Dummy> Dummies;
    public List<MDL4.Material> Materials;
    public List<MDL4.Bone> Bones;
    public List<MDL4.Mesh> Meshes;

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == nameof(MDL4);
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = true;
      br.AssertASCII(nameof(MDL4));
      this.Version = br.AssertInt32(262145, 262146);
      int dataStart = br.ReadInt32();
      br.ReadInt32();
      int capacity1 = br.ReadInt32();
      int capacity2 = br.ReadInt32();
      int capacity3 = br.ReadInt32();
      int capacity4 = br.ReadInt32();
      this.Unk20 = br.ReadInt32();
      this.BoundingBoxMin = br.ReadVector3();
      this.BoundingBoxMax = br.ReadVector3();
      this.TrueFaceCount = br.ReadInt32();
      this.TotalFaceCount = br.ReadInt32();
      br.AssertPattern(60, (byte) 0);
      this.Dummies = new List<MDL4.Dummy>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Dummies.Add(new MDL4.Dummy(br));
      this.Materials = new List<MDL4.Material>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Materials.Add(new MDL4.Material(br));
      this.Bones = new List<MDL4.Bone>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.Bones.Add(new MDL4.Bone(br));
      this.Meshes = new List<MDL4.Mesh>(capacity4);
      for (int index = 0; index < capacity4; ++index)
        this.Meshes.Add(new MDL4.Mesh(br, dataStart, this.Version));
    }

    public class Dummy {
      public Vector3 Forward;
      public Vector3 Upward;
      public Color Color;
      public short ID;
      public short Unk1E;
      public short Unk20;
      public short Unk22;

      internal Dummy(BinaryReaderEx br) {
        this.Forward = br.ReadVector3();
        this.Upward = br.ReadVector3();
        this.Color = br.ReadRGBA();
        this.ID = br.ReadInt16();
        this.Unk1E = br.ReadInt16();
        this.Unk20 = br.ReadInt16();
        this.Unk22 = br.ReadInt16();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
      }
    }

    public class Material {
      public string Name;
      public string Shader;
      public byte Unk3C;
      public byte Unk3D;
      public byte Unk3E;
      public List<MDL4.Material.Param> Params;

      internal Material(BinaryReaderEx br) {
        this.Name = br.ReadFixStr(31);
        this.Shader = br.ReadFixStr(29);
        this.Unk3C = br.ReadByte();
        this.Unk3D = br.ReadByte();
        this.Unk3E = br.ReadByte();
        byte num = br.ReadByte();
        long position = br.Position;
        this.Params = new List<MDL4.Material.Param>((int) num);
        for (int index = 0; index < (int) num; ++index)
          this.Params.Add(new MDL4.Material.Param(br));
        br.Position = position + 2048L;
      }

      public class Param {
        public MDL4.Material.ParamType Type;
        public string Name;
        public object Value;

        internal Param(BinaryReaderEx br) {
          long position = br.Position;
          this.Type = br.ReadEnum8<MDL4.Material.ParamType>();
          this.Name = br.ReadFixStr(31);
          switch (this.Type) {
            case MDL4.Material.ParamType.Int:
              this.Value = (object) br.ReadInt32();
              break;
            case MDL4.Material.ParamType.Float:
              this.Value = (object) br.ReadSingle();
              break;
            case MDL4.Material.ParamType.Float4:
              this.Value = (object) br.ReadSingles(4);
              break;
            case MDL4.Material.ParamType.String:
              this.Value = (object) br.ReadShiftJIS();
              break;
            default:
              throw new NotImplementedException(
                  "Unknown param type: " + (object) this.Type);
          }
          br.Position = position + 64L;
        }
      }

      public enum ParamType : byte {
        Int = 0,
        Float = 1,
        Float4 = 4,
        String = 5,
      }
    }

    public class Bone {
      public string Name;
      public Vector3 Translation;
      public Vector3 Rotation;
      public Vector3 Scale;
      public Vector3 BoundingBoxMin;
      public Vector3 BoundingBoxMax;
      public short ParentIndex;
      public short ChildIndex;
      public short NextSiblingIndex;
      public short PreviousSiblingIndex;
      public short[] UnkIndices;

      internal Bone(BinaryReaderEx br) {
        this.Name = br.ReadFixStr(32);
        this.Translation = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        this.BoundingBoxMin = br.ReadVector3();
        this.BoundingBoxMax = br.ReadVector3();
        this.ParentIndex = br.ReadInt16();
        this.ChildIndex = br.ReadInt16();
        this.NextSiblingIndex = br.ReadInt16();
        this.PreviousSiblingIndex = br.ReadInt16();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.UnkIndices = br.ReadInt16s(16);
      }
    }

    public class Mesh {
      public byte VertexFormat;
      public byte MaterialIndex;
      public bool Unk02;
      public bool Unk03;
      public short Unk08;
      public short[] BoneIndices;
      public ushort[] VertexIndices;
      public List<MDL4.Vertex> Vertices;
      public byte[][] UnkBlocks;

      internal Mesh(BinaryReaderEx br, int dataStart, int version) {
        this.VertexFormat = br.AssertByte((byte) 0, (byte) 1, (byte) 2);
        this.MaterialIndex = br.ReadByte();
        this.Unk02 = br.ReadBoolean();
        this.Unk03 = br.ReadBoolean();
        ushort num1 = br.ReadUInt16();
        this.Unk08 = br.ReadInt16();
        this.BoneIndices = br.ReadInt16s(28);
        br.ReadInt32();
        int num2 = br.ReadInt32();
        int num3 = br.ReadInt32();
        int num4 = br.ReadInt32();
        if (this.VertexFormat == (byte) 2) {
          this.UnkBlocks = new byte[16][];
          for (int index = 0; index < 16; ++index) {
            int count = br.ReadInt32();
            int num5 = br.ReadInt32();
            this.UnkBlocks[index] =
                br.GetBytes((long) (dataStart + num5), count);
          }
        }
        this.VertexIndices =
            br.GetUInt16s((long) (dataStart + num2), (int) num1);
        br.StepIn((long) (dataStart + num4));
        int num6 = 0;
        switch (version) {
          case 262145:
            if (this.VertexFormat == (byte) 0) {
              num6 = 64;
              break;
            }
            if (this.VertexFormat == (byte) 1) {
              num6 = 84;
              break;
            }
            if (this.VertexFormat == (byte) 2) {
              num6 = 60;
              break;
            }
            break;
          case 262146:
            if (this.VertexFormat == (byte) 0) {
              num6 = 40;
              break;
            }
            break;
        }
        int capacity = num3 / num6;
        this.Vertices = new List<MDL4.Vertex>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Vertices.Add(new MDL4.Vertex(br, version, this.VertexFormat));
        br.StepOut();
      }

      public List<MDL4.Vertex[]> GetFaces() {
        ushort[] triangleList = this.ToTriangleList();
        List<MDL4.Vertex[]> vertexArrayList = new List<MDL4.Vertex[]>();
        for (int index = 0; index < triangleList.Length; index += 3)
          vertexArrayList.Add(new MDL4.Vertex[3] {
              this.Vertices[(int) triangleList[index]],
              this.Vertices[(int) triangleList[index + 1]],
              this.Vertices[(int) triangleList[index + 2]]
          });
        return vertexArrayList;
      }

      public ushort[] ToTriangleList() {
        List<ushort> ushortList = new List<ushort>();
        bool flag = false;
        for (int index = 0; index < this.VertexIndices.Length - 2; ++index) {
          ushort vertexIndex1 = this.VertexIndices[index];
          ushort vertexIndex2 = this.VertexIndices[index + 1];
          ushort vertexIndex3 = this.VertexIndices[index + 2];
          if (vertexIndex1 == ushort.MaxValue ||
              vertexIndex2 == ushort.MaxValue ||
              vertexIndex3 == ushort.MaxValue) {
            flag = false;
          } else {
            if ((int) vertexIndex1 != (int) vertexIndex2 &&
                (int) vertexIndex1 != (int) vertexIndex3 &&
                (int) vertexIndex2 != (int) vertexIndex3) {
              if (!flag) {
                ushortList.Add(vertexIndex1);
                ushortList.Add(vertexIndex2);
                ushortList.Add(vertexIndex3);
              } else {
                ushortList.Add(vertexIndex3);
                ushortList.Add(vertexIndex2);
                ushortList.Add(vertexIndex1);
              }
            }
            flag = !flag;
          }
        }
        return ushortList.ToArray();
      }
    }

    public class Vertex {
      public Vector3 Position;
      public Vector4 Normal;
      public Vector4 Tangent;
      public Vector4 Bitangent;
      public byte[] Color;
      public List<Vector2> UVs;
      public short[] BoneIndices;
      public float[] BoneWeights;
      public int Unk3C;

      internal Vertex(BinaryReaderEx br, int version, byte format) {
        this.UVs = new List<Vector2>();
        switch (version) {
          case 262145:
            switch (format) {
              case 0:
                this.Position = br.ReadVector3();
                this.Normal = MDL4.Vertex.Read10BitVector4(br);
                this.Tangent = MDL4.Vertex.Read10BitVector4(br);
                this.Bitangent = MDL4.Vertex.Read10BitVector4(br);
                this.Color = br.ReadBytes(4);
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.Unk3C = br.ReadInt32();
                return;
              case 1:
                this.Position = br.ReadVector3();
                this.Normal = MDL4.Vertex.Read10BitVector4(br);
                this.Tangent = MDL4.Vertex.Read10BitVector4(br);
                this.Bitangent = MDL4.Vertex.Read10BitVector4(br);
                this.Color = br.ReadBytes(4);
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.BoneIndices = br.ReadInt16s(4);
                this.BoneWeights = br.ReadSingles(4);
                return;
              case 2:
                this.Color = br.ReadBytes(4);
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.UVs.Add(br.ReadVector2());
                this.BoneIndices = br.ReadInt16s(4);
                this.BoneWeights = br.ReadSingles(4);
                return;
              default:
                return;
            }
          case 262146:
            if (format != (byte) 0)
              break;
            this.Position = br.ReadVector3();
            this.Normal = MDL4.Vertex.ReadSByteVector4(br);
            this.Tangent = MDL4.Vertex.ReadSByteVector4(br);
            this.Color = br.ReadBytes(4);
            this.UVs.Add(MDL4.Vertex.ReadShortUV(br));
            this.UVs.Add(MDL4.Vertex.ReadShortUV(br));
            this.UVs.Add(MDL4.Vertex.ReadShortUV(br));
            this.UVs.Add(MDL4.Vertex.ReadShortUV(br));
            break;
        }
      }

      private static Vector4 ReadByteVector4(BinaryReaderEx br) {
        byte num1 = br.ReadByte();
        byte num2 = br.ReadByte();
        byte num3 = br.ReadByte();
        return new Vector4(
            (float) ((int) br.ReadByte() - (int) sbyte.MaxValue) /
            (float) sbyte.MaxValue,
            (float) ((int) num3 - (int) sbyte.MaxValue) /
            (float) sbyte.MaxValue,
            (float) ((int) num2 - (int) sbyte.MaxValue) /
            (float) sbyte.MaxValue,
            (float) ((int) num1 - (int) sbyte.MaxValue) /
            (float) sbyte.MaxValue);
      }

      private static Vector4 ReadSByteVector4(BinaryReaderEx br) {
        sbyte num1 = br.ReadSByte();
        sbyte num2 = br.ReadSByte();
        sbyte num3 = br.ReadSByte();
        return new Vector4((float) br.ReadSByte() / (float) sbyte.MaxValue,
                           (float) num3 / (float) sbyte.MaxValue,
                           (float) num2 / (float) sbyte.MaxValue,
                           (float) num1 / (float) sbyte.MaxValue);
      }

      private static Vector2 ReadShortUV(BinaryReaderEx br) {
        return new Vector2((float) br.ReadInt16() / 2048f,
                           (float) br.ReadInt16() / 2048f);
      }

      private static Vector4 Read10BitVector4(BinaryReaderEx br) {
        int num = br.ReadInt32();
        return new Vector4((float) (num << 22 >> 22) / 511f,
                           (float) (num << 12 >> 22) / 511f,
                           (float) (num << 2 >> 22) / 511f,
                           (float) (num >> 30));
      }
    }
  }
}