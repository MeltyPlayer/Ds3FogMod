// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Other.MDL0
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats.Other
{
  [ComVisible(true)]
  public class MDL0 : SoulsFile<MDL0>
  {
    public int Unk04;
    public int Unk08;
    public List<MDL0.Bone> Bones;
    public List<ushort> Indices;
    public List<MDL0.Vertex> VerticesA;
    public List<MDL0.Vertex> VerticesB;
    public List<MDL0.Vertex> VerticesC;
    public List<MDL0.Struct6> Struct6s;
    public List<MDL0.Material> Materials;
    public List<string> Textures;

    protected override void Read(BinaryReaderEx br)
    {
      br.ReadInt32();
      this.Unk04 = br.ReadInt32();
      this.Unk08 = br.ReadInt32();
      br.ReadInt32();
      int capacity1 = br.ReadInt32();
      int count = br.ReadInt32();
      int capacity2 = br.ReadInt32();
      int capacity3 = br.ReadInt32();
      int capacity4 = br.ReadInt32();
      int capacity5 = br.ReadInt32();
      int capacity6 = br.ReadInt32();
      int capacity7 = br.ReadInt32();
      int num1 = br.ReadInt32();
      int num2 = br.ReadInt32();
      int num3 = br.ReadInt32();
      int num4 = br.ReadInt32();
      int num5 = br.ReadInt32();
      int num6 = br.ReadInt32();
      int num7 = br.ReadInt32();
      int num8 = br.ReadInt32();
      br.Position = (long) num1;
      this.Bones = new List<MDL0.Bone>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Bones.Add(new MDL0.Bone(br));
      br.Position = (long) num2;
      this.Indices = new List<ushort>((IEnumerable<ushort>) br.ReadUInt16s(count));
      br.Position = (long) num3;
      this.VerticesA = new List<MDL0.Vertex>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.VerticesA.Add(new MDL0.Vertex(br, MDL0.VertexFormat.A));
      br.Position = (long) num4;
      this.VerticesB = new List<MDL0.Vertex>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.VerticesB.Add(new MDL0.Vertex(br, MDL0.VertexFormat.B));
      br.Position = (long) num5;
      this.VerticesC = new List<MDL0.Vertex>(capacity4);
      for (int index = 0; index < capacity4; ++index)
        this.VerticesC.Add(new MDL0.Vertex(br, MDL0.VertexFormat.C));
      br.Position = (long) num6;
      this.Struct6s = new List<MDL0.Struct6>(capacity5);
      for (int index = 0; index < capacity5; ++index)
        this.Struct6s.Add(new MDL0.Struct6(br));
      br.Position = (long) num7;
      this.Materials = new List<MDL0.Material>(capacity6);
      for (int index = 0; index < capacity6; ++index)
        this.Materials.Add(new MDL0.Material(br));
      br.Position = (long) num8;
      this.Textures = new List<string>(capacity7);
      for (int index = 0; index < capacity7; ++index)
        this.Textures.Add(br.ReadShiftJIS());
    }

    public List<int> Triangulate(MDL0.Mesh mesh, List<MDL0.Vertex> vertices)
    {
      List<int> intList = new List<int>();
      bool flag = false;
      for (int startIndex = mesh.StartIndex; startIndex < mesh.StartIndex + mesh.IndexCount - 2; ++startIndex)
      {
        ushort index1 = this.Indices[startIndex];
        ushort index2 = this.Indices[startIndex + 1];
        ushort index3 = this.Indices[startIndex + 2];
        if ((int) index1 != (int) index2 && (int) index1 != (int) index3 && (int) index2 != (int) index3)
        {
          MDL0.Vertex vertex1 = vertices[(int) index1 - mesh.StartVertex];
          MDL0.Vertex vertex2 = vertices[(int) index2 - mesh.StartVertex];
          MDL0.Vertex vertex3 = vertices[(int) index3 - mesh.StartVertex];
          Vector3 vector2 = Vector3.Normalize((vertex1.Normal + vertex2.Normal + vertex3.Normal) / 3f);
          Vector3 vector1 = Vector3.Normalize(Vector3.Cross(vertex2.Position - vertex1.Position, vertex3.Position - vertex1.Position));
          flag = (double) Vector3.Dot(vector1, vector2) / ((double) vector1.Length() * (double) vector2.Length()) < 0.0;
          if (!flag)
          {
            intList.Add((int) index1);
            intList.Add((int) index2);
            intList.Add((int) index3);
          }
          else
          {
            intList.Add((int) index3);
            intList.Add((int) index2);
            intList.Add((int) index1);
          }
        }
        flag = !flag;
      }
      return intList;
    }

    public class Bone
    {
      public Vector3 Translation;
      public Vector3 Rotation;
      public Vector3 Scale;
      public int ParentIndex;
      public int ChildIndex;
      public int NextSiblingIndex;
      public int PrevSiblingIndex;
      public List<MDL0.Mesh> MeshesA;
      public List<MDL0.Mesh> MeshesB;
      public List<MDL0.MeshGroup> MeshesC;
      public int Unk4C;

      internal Bone(BinaryReaderEx br)
      {
        this.Translation = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        this.ParentIndex = br.ReadInt32();
        this.ChildIndex = br.ReadInt32();
        this.NextSiblingIndex = br.ReadInt32();
        this.PrevSiblingIndex = br.ReadInt32();
        int capacity1 = br.ReadInt32();
        int capacity2 = br.ReadInt32();
        int capacity3 = br.ReadInt32();
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int num3 = br.ReadInt32();
        this.Unk4C = br.ReadInt32();
        br.StepIn((long) num1);
        this.MeshesA = new List<MDL0.Mesh>(capacity1);
        for (int index = 0; index < capacity1; ++index)
          this.MeshesA.Add(new MDL0.Mesh(br));
        br.StepOut();
        br.StepIn((long) num2);
        this.MeshesB = new List<MDL0.Mesh>(capacity2);
        for (int index = 0; index < capacity2; ++index)
          this.MeshesB.Add(new MDL0.Mesh(br));
        br.StepOut();
        br.StepIn((long) num3);
        this.MeshesC = new List<MDL0.MeshGroup>(capacity3);
        for (int index = 0; index < capacity3; ++index)
          this.MeshesC.Add(new MDL0.MeshGroup(br));
        br.StepOut();
      }
    }

    public class MeshGroup
    {
      public List<MDL0.Mesh> Meshes;
      public byte Unk02;
      public byte Unk03;
      public short[] BoneIndices;

      internal MeshGroup(BinaryReaderEx br)
      {
        short num1 = br.ReadInt16();
        this.Unk02 = br.ReadByte();
        this.Unk03 = br.ReadByte();
        this.BoneIndices = br.ReadInt16s(4);
        int num2 = br.ReadInt32();
        br.StepIn((long) num2);
        this.Meshes = new List<MDL0.Mesh>((int) num1);
        for (int index = 0; index < (int) num1; ++index)
          this.Meshes.Add(new MDL0.Mesh(br));
        br.StepOut();
      }
    }

    public class Mesh
    {
      public byte MaterialIndex;
      public byte Unk01;
      public short VertexCount;
      public int IndexCount;
      public int StartVertex;
      public int StartIndex;

      internal Mesh(BinaryReaderEx br)
      {
        this.MaterialIndex = br.ReadByte();
        this.Unk01 = br.AssertByte((byte) 0, (byte) 1, (byte) 2);
        this.VertexCount = br.ReadInt16();
        this.IndexCount = br.ReadInt32();
        this.StartVertex = br.ReadInt32();
        this.StartIndex = br.ReadInt32();
      }
    }

    public enum VertexFormat
    {
      A,
      B,
      C,
    }

    public class Vertex
    {
      public Vector3 Position;
      public Vector3 Normal;
      public Color Color;
      public Vector2[] UVs;
      public short UnkShortA;
      public short UnkShortB;
      public float UnkFloatA;
      public float UnkFloatB;

      public Vertex(Vector3 position, Vector3 normal)
      {
        this.Position = position;
        this.Normal = normal;
        this.UVs = new Vector2[2];
      }

      internal Vertex(BinaryReaderEx br, MDL0.VertexFormat format)
      {
        this.Position = br.ReadVector3();
        this.Normal = MDL0.Vertex.Read11_11_10Vector3(br);
        this.Color = br.ReadRGBA();
        this.UVs = new Vector2[2];
        for (int index = 0; index < 2; ++index)
          this.UVs[index] = br.ReadVector2();
        if (format >= MDL0.VertexFormat.B)
        {
          this.UnkShortA = br.ReadInt16();
          this.UnkShortB = br.ReadInt16();
        }
        if (format < MDL0.VertexFormat.C)
          return;
        this.UnkFloatA = br.ReadSingle();
        this.UnkFloatB = br.ReadSingle();
      }

      private static Vector3 Read11_11_10Vector3(BinaryReaderEx br)
      {
        int num = br.ReadInt32();
        return new Vector3((float) (num << 21 >> 21) / 1023f, (float) (num << 10 >> 21) / 1023f, (float) (num >> 22) / 511f);
      }
    }

    public class Struct6
    {
      public Vector3 Position;
      public Vector3 Rotation;
      public int BoneIndex;

      internal Struct6(BinaryReaderEx br)
      {
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.BoneIndex = br.ReadInt32();
        br.AssertInt32(new int[1]);
      }
    }

    public class Material
    {
      public int Unk04;
      public int Unk08;
      public int Unk0C;
      public int DiffuseMapIndex;
      public int ReflectionMaskIndex;
      public int ReflectionMapIndex;
      public Vector4 Unk20;
      public Vector4 Unk30;
      public Vector4 Unk40;
      public float Unk60;
      public float Unk64;
      public float Unk68;
      public int Unk6C;

      internal Material(BinaryReaderEx br)
      {
        br.AssertInt32(new int[1]);
        this.Unk04 = br.ReadInt32();
        this.Unk08 = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        this.DiffuseMapIndex = br.ReadInt32();
        this.ReflectionMaskIndex = br.ReadInt32();
        this.ReflectionMapIndex = br.ReadInt32();
        br.AssertInt32(-1);
        this.Unk20 = br.ReadVector4();
        this.Unk30 = br.ReadVector4();
        this.Unk40 = br.ReadVector4();
        br.AssertPattern(16, (byte) 0);
        this.Unk60 = br.ReadSingle();
        this.Unk64 = br.ReadSingle();
        this.Unk68 = br.ReadSingle();
        this.Unk6C = br.ReadInt32();
      }
    }
  }
}
