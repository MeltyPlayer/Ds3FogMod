// Decompiled with JetBrains decompiler
// Type: SoulsFormats.Other.MDL
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
  public class MDL : SoulsFile<MDL>
  {
    public int Unk0C;
    public int Unk10;
    public int Unk14;
    public List<MDL.Bone> Meshes;
    public ushort[] Indices;
    public List<MDL.Vertex> VerticesA;
    public List<MDL.Vertex> VerticesB;
    public List<MDL.Vertex> VerticesC;
    public List<MDL.VertexD> VerticesD;
    public List<MDL.Struct7> Struct7s;
    public List<MDL.Material> Materials;
    public List<string> Textures;

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(4L, 4) == "MDL ";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      br.ReadInt32();
      br.AssertASCII("MDL ");
      int num1 = (int) br.AssertInt16((short) 1);
      int num2 = (int) br.AssertInt16((short) 1);
      this.Unk0C = br.ReadInt32();
      this.Unk10 = br.ReadInt32();
      this.Unk14 = br.ReadInt32();
      int num3 = br.ReadInt32();
      int count = br.ReadInt32();
      int capacity1 = br.ReadInt32();
      int capacity2 = br.ReadInt32();
      int capacity3 = br.ReadInt32();
      int capacity4 = br.ReadInt32();
      int capacity5 = br.ReadInt32();
      int capacity6 = br.ReadInt32();
      int capacity7 = br.ReadInt32();
      int num4 = br.ReadInt32();
      int num5 = br.ReadInt32();
      int num6 = br.ReadInt32();
      int num7 = br.ReadInt32();
      int num8 = br.ReadInt32();
      int num9 = br.ReadInt32();
      int num10 = br.ReadInt32();
      int num11 = br.ReadInt32();
      int num12 = br.ReadInt32();
      br.Position = (long) num4;
      this.Meshes = new List<MDL.Bone>();
      for (int index = 0; index < num3; ++index)
        this.Meshes.Add(new MDL.Bone(br));
      this.Indices = br.GetUInt16s((long) num5, count);
      br.Position = (long) num6;
      this.VerticesA = new List<MDL.Vertex>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.VerticesA.Add(new MDL.Vertex(br, MDL.VertexFormat.A));
      br.Position = (long) num7;
      this.VerticesB = new List<MDL.Vertex>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.VerticesB.Add(new MDL.Vertex(br, MDL.VertexFormat.B));
      br.Position = (long) num8;
      this.VerticesC = new List<MDL.Vertex>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.VerticesC.Add(new MDL.Vertex(br, MDL.VertexFormat.C));
      br.Position = (long) num9;
      this.VerticesD = new List<MDL.VertexD>(capacity4);
      for (int index = 0; index < capacity4; ++index)
        this.VerticesD.Add(new MDL.VertexD(br));
      br.Position = (long) num10;
      this.Struct7s = new List<MDL.Struct7>(capacity5);
      for (int index = 0; index < capacity5; ++index)
        this.Struct7s.Add(new MDL.Struct7(br));
      br.Position = (long) num11;
      this.Materials = new List<MDL.Material>(capacity6);
      for (int index = 0; index < capacity6; ++index)
        this.Materials.Add(new MDL.Material(br));
      br.Position = (long) num12;
      this.Textures = new List<string>(capacity7);
      for (int index = 0; index < capacity7; ++index)
        this.Textures.Add(br.ReadShiftJIS());
    }

    private static Vector3 Read11_11_10Vector3(BinaryReaderEx br)
    {
      int num = br.ReadInt32();
      return new Vector3((float) (num << 21 >> 21) / 1023f, (float) (num << 10 >> 21) / 1023f, (float) (num >> 22) / 511f);
    }

    public List<MDL.Vertex[]> GetFaces(MDL.Faceset faceset, List<MDL.Vertex> vertices)
    {
      List<ushort> ushortList = this.Triangulate(faceset, vertices);
      List<MDL.Vertex[]> vertexArrayList = new List<MDL.Vertex[]>();
      for (int index = 0; index < ushortList.Count; index += 3)
        vertexArrayList.Add(new MDL.Vertex[3]
        {
          vertices[(int) ushortList[index]],
          vertices[(int) ushortList[index + 1]],
          vertices[(int) ushortList[index + 2]]
        });
      return vertexArrayList;
    }

    public List<ushort> Triangulate(MDL.Faceset faceset, List<MDL.Vertex> vertices)
    {
      bool flag = false;
      List<ushort> ushortList = new List<ushort>();
      for (int startIndex = faceset.StartIndex; startIndex < faceset.StartIndex + faceset.IndexCount - 2; ++startIndex)
      {
        ushort index1 = this.Indices[startIndex];
        ushort index2 = this.Indices[startIndex + 1];
        ushort index3 = this.Indices[startIndex + 2];
        if (index1 == ushort.MaxValue || index2 == ushort.MaxValue || index3 == ushort.MaxValue)
        {
          flag = false;
        }
        else
        {
          if ((int) index1 != (int) index2 && (int) index1 != (int) index3 && (int) index2 != (int) index3)
          {
            MDL.Vertex vertex1 = vertices[(int) index1];
            MDL.Vertex vertex2 = vertices[(int) index2];
            MDL.Vertex vertex3 = vertices[(int) index3];
            Vector3 vector2 = Vector3.Normalize((vertex1.Normal + vertex2.Normal + vertex3.Normal) / 3f);
            Vector3 vector1 = Vector3.Normalize(Vector3.Cross(vertex2.Position - vertex1.Position, vertex3.Position - vertex1.Position));
            flag = (double) Vector3.Dot(vector1, vector2) / ((double) vector1.Length() * (double) vector2.Length()) <= 0.0;
            if (!flag)
            {
              ushortList.Add(index1);
              ushortList.Add(index2);
              ushortList.Add(index3);
            }
            else
            {
              ushortList.Add(index3);
              ushortList.Add(index2);
              ushortList.Add(index1);
            }
          }
          flag = !flag;
        }
      }
      return ushortList;
    }

    public class Bone
    {
      public Vector3 Translation;
      public Vector3 Rotation;
      public Vector3 Scale;
      public int ParentIndex;
      public int ChildIndex;
      public int NextSiblingIndex;
      public int PreviousSiblingIndex;
      public List<MDL.Faceset> FacesetsA;
      public List<MDL.Faceset> FacesetsB;
      public List<MDL.FacesetC> FacesetsC;
      public List<MDL.FacesetC> FacesetsD;
      public int Unk54;
      public Vector3 BoundingBoxMin;
      public Vector3 BoundingBoxMax;
      public short[] Unk70;

      internal Bone(BinaryReaderEx br)
      {
        this.Translation = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        this.ParentIndex = br.ReadInt32();
        this.ChildIndex = br.ReadInt32();
        this.NextSiblingIndex = br.ReadInt32();
        this.PreviousSiblingIndex = br.ReadInt32();
        int capacity1 = br.ReadInt32();
        int capacity2 = br.ReadInt32();
        int capacity3 = br.ReadInt32();
        int capacity4 = br.ReadInt32();
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int num3 = br.ReadInt32();
        int num4 = br.ReadInt32();
        this.Unk54 = br.ReadInt32();
        this.BoundingBoxMin = br.ReadVector3();
        this.BoundingBoxMax = br.ReadVector3();
        this.Unk70 = br.ReadInt16s(10);
        br.AssertPattern(12, (byte) 0);
        br.StepIn((long) num1);
        this.FacesetsA = new List<MDL.Faceset>(capacity1);
        for (int index = 0; index < capacity1; ++index)
          this.FacesetsA.Add(new MDL.Faceset(br));
        br.StepOut();
        br.StepIn((long) num2);
        this.FacesetsB = new List<MDL.Faceset>(capacity2);
        for (int index = 0; index < capacity2; ++index)
          this.FacesetsB.Add(new MDL.Faceset(br));
        br.StepOut();
        br.StepIn((long) num3);
        this.FacesetsC = new List<MDL.FacesetC>(capacity3);
        for (int index = 0; index < capacity3; ++index)
          this.FacesetsC.Add(new MDL.FacesetC(br));
        br.StepOut();
        br.StepIn((long) num4);
        this.FacesetsD = new List<MDL.FacesetC>(capacity4);
        for (int index = 0; index < capacity4; ++index)
          this.FacesetsD.Add(new MDL.FacesetC(br));
        br.StepOut();
      }
    }

    public class Faceset
    {
      public byte MaterialIndex { get; set; }

      public byte Unk01 { get; set; }

      public short VertexCount { get; set; }

      public int IndexCount { get; set; }

      public int StartVertex { get; set; }

      public int StartIndex { get; set; }

      internal Faceset(BinaryReaderEx br)
      {
        this.MaterialIndex = br.ReadByte();
        this.Unk01 = br.ReadByte();
        this.VertexCount = br.ReadInt16();
        this.IndexCount = br.ReadInt32();
        this.StartVertex = br.ReadInt32();
        this.StartIndex = br.ReadInt32();
      }
    }

    public class FacesetC
    {
      public List<MDL.Faceset> Facesets;
      public byte IndexCount;
      public byte Unk03;
      public short[] Indices;

      internal FacesetC(BinaryReaderEx br)
      {
        short num1 = br.ReadInt16();
        this.IndexCount = br.ReadByte();
        this.Unk03 = br.ReadByte();
        int num2 = br.ReadInt32();
        this.Indices = br.ReadInt16s(8);
        br.StepIn((long) num2);
        this.Facesets = new List<MDL.Faceset>((int) num1);
        for (int index = 0; index < (int) num1; ++index)
          this.Facesets.Add(new MDL.Faceset(br));
        br.StepOut();
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
      public Color Color;
      public Vector2[] UVs;
      public short UnkShortA;
      public short UnkShortB;
      public float UnkFloatA;
      public float UnkFloatB;

      public virtual Vector3 Position { get; set; }

      public virtual Vector3 Normal { get; set; }

      public virtual Vector3 Tangent { get; set; }

      public virtual Vector3 Bitangent { get; set; }

      public Vertex()
      {
        this.UVs = new Vector2[4];
      }

      internal Vertex(BinaryReaderEx br, MDL.VertexFormat format)
      {
        this.Position = br.ReadVector3();
        this.Normal = MDL.Read11_11_10Vector3(br);
        this.Tangent = MDL.Read11_11_10Vector3(br);
        this.Bitangent = MDL.Read11_11_10Vector3(br);
        this.Color = br.ReadRGBA();
        this.UVs = new Vector2[4];
        for (int index = 0; index < 4; ++index)
          this.UVs[index] = br.ReadVector2();
        if (format >= MDL.VertexFormat.B)
        {
          this.UnkShortA = br.ReadInt16();
          this.UnkShortB = br.ReadInt16();
        }
        if (format < MDL.VertexFormat.C)
          return;
        this.UnkFloatA = br.ReadSingle();
        this.UnkFloatB = br.ReadSingle();
      }
    }

    public class VertexD : MDL.Vertex
    {
      public Vector3[] Positions;
      public Vector3[] Normals;
      public Vector3[] Tangents;
      public Vector3[] Bitangents;

      public override Vector3 Position
      {
        get
        {
          return this.Positions[0];
        }
        set
        {
          this.Positions[0] = value;
        }
      }

      public override Vector3 Normal
      {
        get
        {
          return this.Normals[0];
        }
        set
        {
          this.Normals[0] = value;
        }
      }

      public override Vector3 Tangent
      {
        get
        {
          return this.Tangents[0];
        }
        set
        {
          this.Tangents[0] = value;
        }
      }

      public override Vector3 Bitangent
      {
        get
        {
          return this.Bitangents[0];
        }
        set
        {
          this.Bitangents[0] = value;
        }
      }

      internal VertexD(BinaryReaderEx br)
      {
        this.Positions = new Vector3[16];
        for (int index = 0; index < 16; ++index)
          this.Positions[index] = br.ReadVector3();
        this.Normals = new Vector3[16];
        for (int index = 0; index < 16; ++index)
          this.Normals[index] = MDL.Read11_11_10Vector3(br);
        this.Tangents = new Vector3[16];
        for (int index = 0; index < 16; ++index)
          this.Tangents[index] = MDL.Read11_11_10Vector3(br);
        this.Bitangents = new Vector3[16];
        for (int index = 0; index < 16; ++index)
          this.Bitangents[index] = MDL.Read11_11_10Vector3(br);
        this.Color = br.ReadRGBA();
        this.UVs = new Vector2[4];
        for (int index = 0; index < 4; ++index)
          this.UVs[index] = br.ReadVector2();
        this.UnkShortA = br.ReadInt16();
        this.UnkShortB = br.ReadInt16();
        this.UnkFloatA = br.ReadSingle();
        this.UnkFloatB = br.ReadSingle();
      }
    }

    public class Struct7
    {
      public float Unk00;
      public float Unk04;
      public float Unk08;
      public float Unk0C;
      public float Unk10;
      public float Unk14;
      public int Unk18;
      public int Unk1C;

      internal Struct7(BinaryReaderEx br)
      {
        this.Unk00 = br.ReadSingle();
        this.Unk04 = br.ReadSingle();
        this.Unk08 = br.ReadSingle();
        this.Unk0C = br.ReadSingle();
        this.Unk10 = br.ReadSingle();
        this.Unk14 = br.ReadSingle();
        this.Unk18 = br.ReadInt32();
        this.Unk1C = br.ReadInt32();
      }
    }

    public class Material
    {
      public int Unk04;
      public int Unk08;
      public int Unk0C;
      public int TextureIndex;
      public int Unk14;
      public int Unk18;
      public int Unk1C;
      public float Unk20;
      public float Unk24;
      public float Unk28;
      public float Unk2C;
      public float Unk30;
      public float Unk34;
      public float Unk38;
      public float Unk3C;
      public float Unk40;
      public float Unk44;
      public float Unk48;
      public float Unk4C;
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
        this.TextureIndex = br.ReadInt32();
        this.Unk14 = br.ReadInt32();
        this.Unk18 = br.ReadInt32();
        this.Unk1C = br.ReadInt32();
        this.Unk20 = br.ReadSingle();
        this.Unk24 = br.ReadSingle();
        this.Unk28 = br.ReadSingle();
        this.Unk2C = br.ReadSingle();
        this.Unk30 = br.ReadSingle();
        this.Unk34 = br.ReadSingle();
        this.Unk38 = br.ReadSingle();
        this.Unk3C = br.ReadSingle();
        this.Unk40 = br.ReadSingle();
        this.Unk44 = br.ReadSingle();
        this.Unk48 = br.ReadSingle();
        this.Unk4C = br.ReadSingle();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Unk60 = br.ReadSingle();
        this.Unk64 = br.ReadSingle();
        this.Unk68 = br.ReadSingle();
        this.Unk6C = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
      }
    }
  }
}
