// Decompiled with JetBrains decompiler
// Type: SoulsFormats.FLVER
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public static class FLVER
  {
    public class Bone
    {
      public string Name { get; set; }

      public short ParentIndex { get; set; }

      public short ChildIndex { get; set; }

      public short NextSiblingIndex { get; set; }

      public short PreviousSiblingIndex { get; set; }

      public Vector3 Translation { get; set; }

      public Vector3 Rotation { get; set; }

      public Vector3 Scale { get; set; }

      public Vector3 BoundingBoxMin { get; set; }

      public Vector3 BoundingBoxMax { get; set; }

      public int Unk3C { get; set; }

      public Bone()
      {
        this.Name = "";
        this.ParentIndex = (short) -1;
        this.ChildIndex = (short) -1;
        this.NextSiblingIndex = (short) -1;
        this.PreviousSiblingIndex = (short) -1;
        this.Scale = Vector3.One;
      }

      public Matrix4x4 ComputeLocalTransform()
      {
        return Matrix4x4.CreateScale(this.Scale) * Matrix4x4.CreateRotationX(this.Rotation.X) * Matrix4x4.CreateRotationZ(this.Rotation.Z) * Matrix4x4.CreateRotationY(this.Rotation.Y) * Matrix4x4.CreateTranslation(this.Translation);
      }

      public override string ToString()
      {
        return this.Name;
      }

      internal Bone(BinaryReaderEx br, bool unicode)
      {
        this.Translation = br.ReadVector3();
        int num = br.ReadInt32();
        this.Rotation = br.ReadVector3();
        this.ParentIndex = br.ReadInt16();
        this.ChildIndex = br.ReadInt16();
        this.Scale = br.ReadVector3();
        this.NextSiblingIndex = br.ReadInt16();
        this.PreviousSiblingIndex = br.ReadInt16();
        this.BoundingBoxMin = br.ReadVector3();
        this.Unk3C = br.ReadInt32();
        this.BoundingBoxMax = br.ReadVector3();
        br.AssertPattern(52, (byte) 0);
        if (unicode)
          this.Name = br.GetUTF16((long) num);
        else
          this.Name = br.GetShiftJIS((long) num);
      }

      internal void Write(BinaryWriterEx bw, int index)
      {
        bw.WriteVector3(this.Translation);
        bw.ReserveInt32(string.Format("BoneNameOffset{0}", (object) index));
        bw.WriteVector3(this.Rotation);
        bw.WriteInt16(this.ParentIndex);
        bw.WriteInt16(this.ChildIndex);
        bw.WriteVector3(this.Scale);
        bw.WriteInt16(this.NextSiblingIndex);
        bw.WriteInt16(this.PreviousSiblingIndex);
        bw.WriteVector3(this.BoundingBoxMin);
        bw.WriteInt32(this.Unk3C);
        bw.WriteVector3(this.BoundingBoxMax);
        bw.WritePattern(52, (byte) 0);
      }

      internal void WriteStrings(BinaryWriterEx bw, bool unicode, int index)
      {
        bw.FillInt32(string.Format("BoneNameOffset{0}", (object) index), (int) bw.Position);
        if (unicode)
          bw.WriteUTF16(this.Name, true);
        else
          bw.WriteShiftJIS(this.Name, true);
      }
    }

    public class Dummy
    {
      public Vector3 Position { get; set; }

      public Vector3 Forward { get; set; }

      public Vector3 Upward { get; set; }

      public short ReferenceID { get; set; }

      public short ParentBoneIndex { get; set; }

      public short AttachBoneIndex { get; set; }

      public Color Color { get; set; }

      public bool Flag1 { get; set; }

      public bool UseUpwardVector { get; set; }

      public int Unk30 { get; set; }

      public int Unk34 { get; set; }

      public Dummy()
      {
        this.ParentBoneIndex = (short) -1;
        this.AttachBoneIndex = (short) -1;
      }

      public override string ToString()
      {
        return string.Format("{0}", (object) this.ReferenceID);
      }

      internal Dummy(BinaryReaderEx br, int version)
      {
        this.Position = br.ReadVector3();
        this.Color = version != 131088 ? br.ReadARGB() : br.ReadBGRA();
        this.Forward = br.ReadVector3();
        this.ReferenceID = br.ReadInt16();
        this.ParentBoneIndex = br.ReadInt16();
        this.Upward = br.ReadVector3();
        this.AttachBoneIndex = br.ReadInt16();
        this.Flag1 = br.ReadBoolean();
        this.UseUpwardVector = br.ReadBoolean();
        this.Unk30 = br.ReadInt32();
        this.Unk34 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
      }

      internal void Write(BinaryWriterEx bw, int version)
      {
        bw.WriteVector3(this.Position);
        if (version == 131088)
          bw.WriteBGRA(this.Color);
        else
          bw.WriteARGB(this.Color);
        bw.WriteVector3(this.Forward);
        bw.WriteInt16(this.ReferenceID);
        bw.WriteInt16(this.ParentBoneIndex);
        bw.WriteVector3(this.Upward);
        bw.WriteInt16(this.AttachBoneIndex);
        bw.WriteBoolean(this.Flag1);
        bw.WriteBoolean(this.UseUpwardVector);
        bw.WriteInt32(this.Unk30);
        bw.WriteInt32(this.Unk34);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
      }
    }

    public class LayoutMember
    {
      public int Unk00 { get; set; }

      public FLVER.LayoutType Type { get; set; }

      public FLVER.LayoutSemantic Semantic { get; set; }

      public int Index { get; set; }

      public int Size
      {
        get
        {
          switch (this.Type)
          {
            case FLVER.LayoutType.Float2:
            case FLVER.LayoutType.UVPair:
            case FLVER.LayoutType.ShortBoneIndices:
            case FLVER.LayoutType.Short4toFloat4A:
            case FLVER.LayoutType.Short4toFloat4B:
              return 8;
            case FLVER.LayoutType.Float3:
              return 12;
            case FLVER.LayoutType.Float4:
              return 16;
            case FLVER.LayoutType.Byte4A:
            case FLVER.LayoutType.Byte4B:
            case FLVER.LayoutType.Short2toFloat2:
            case FLVER.LayoutType.Byte4C:
            case FLVER.LayoutType.UV:
            case FLVER.LayoutType.Byte4E:
              return 4;
            default:
              throw new NotImplementedException(string.Format("No size defined for buffer layout type: {0}", (object) this.Type));
          }
        }
      }

      public LayoutMember(
        FLVER.LayoutType type,
        FLVER.LayoutSemantic semantic,
        int index = 0,
        int unk00 = 0)
      {
        this.Unk00 = unk00;
        this.Type = type;
        this.Semantic = semantic;
        this.Index = index;
      }

      internal LayoutMember(BinaryReaderEx br, int structOffset)
      {
        this.Unk00 = br.ReadInt32();
        br.AssertInt32(structOffset);
        this.Type = br.ReadEnum32<FLVER.LayoutType>();
        this.Semantic = br.ReadEnum32<FLVER.LayoutSemantic>();
        this.Index = br.ReadInt32();
      }

      internal void Write(BinaryWriterEx bw, int structOffset)
      {
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(structOffset);
        bw.WriteUInt32((uint) this.Type);
        bw.WriteUInt32((uint) this.Semantic);
        bw.WriteInt32(this.Index);
      }

      public override string ToString()
      {
        return string.Format("{0}: {1}", (object) this.Type, (object) this.Semantic);
      }
    }

    public enum LayoutType : uint
    {
      Float2 = 1,
      Float3 = 2,
      Float4 = 3,
      Byte4A = 16, // 0x00000010
      Byte4B = 17, // 0x00000011
      Short2toFloat2 = 18, // 0x00000012
      Byte4C = 19, // 0x00000013
      UV = 21, // 0x00000015
      UVPair = 22, // 0x00000016
      ShortBoneIndices = 24, // 0x00000018
      Short4toFloat4A = 26, // 0x0000001A
      Short4toFloat4B = 46, // 0x0000002E
      Byte4E = 47, // 0x0000002F
    }

    public enum LayoutSemantic : uint
    {
      Position = 0,
      BoneWeights = 1,
      BoneIndices = 2,
      Normal = 3,
      UV = 5,
      Tangent = 6,
      Bitangent = 7,
      VertexColor = 10, // 0x0000000A
    }

    public class Vertex
    {
      public Vector3 Position;
      public FLVER.VertexBoneWeights BoneWeights;
      public FLVER.VertexBoneIndices BoneIndices;
      public Vector3 Normal;
      public int NormalW;
      public List<Vector3> UVs;
      public List<Vector4> Tangents;
      public Vector4 Bitangent;
      public List<FLVER.VertexColor> Colors;
      private Queue<Vector3> uvQueue;
      private Queue<Vector4> tangentQueue;
      private Queue<FLVER.VertexColor> colorQueue;

      public Vertex(int uvCapacity = 0, int tangentCapacity = 0, int colorCapacity = 0)
      {
        this.UVs = new List<Vector3>(uvCapacity);
        this.Tangents = new List<Vector4>(tangentCapacity);
        this.Colors = new List<FLVER.VertexColor>(colorCapacity);
      }

      public Vertex(FLVER.Vertex clone)
      {
        this.Position = clone.Position;
        this.BoneWeights = clone.BoneWeights;
        this.BoneIndices = clone.BoneIndices;
        this.Normal = clone.Normal;
        this.UVs = new List<Vector3>((IEnumerable<Vector3>) clone.UVs);
        this.Tangents = new List<Vector4>((IEnumerable<Vector4>) clone.Tangents);
        this.Bitangent = clone.Bitangent;
        this.Colors = new List<FLVER.VertexColor>((IEnumerable<FLVER.VertexColor>) clone.Colors);
      }

      internal void PrepareWrite()
      {
        this.uvQueue = new Queue<Vector3>((IEnumerable<Vector3>) this.UVs);
        this.tangentQueue = new Queue<Vector4>((IEnumerable<Vector4>) this.Tangents);
        this.colorQueue = new Queue<FLVER.VertexColor>((IEnumerable<FLVER.VertexColor>) this.Colors);
      }

      internal void FinishWrite()
      {
        this.uvQueue = (Queue<Vector3>) null;
        this.tangentQueue = (Queue<Vector4>) null;
        this.colorQueue = (Queue<FLVER.VertexColor>) null;
      }

      internal void Read(BinaryReaderEx br, List<FLVER.LayoutMember> layout, float uvFactor)
      {
        foreach (FLVER.LayoutMember layoutMember in layout)
        {
          if (layoutMember.Semantic == FLVER.LayoutSemantic.Position)
          {
            if (layoutMember.Type == FLVER.LayoutType.Float3)
            {
              this.Position = br.ReadVector3();
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Float4)
                throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              this.Position = br.ReadVector3();
              double num = (double) br.AssertSingle(new float[1]);
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.BoneWeights)
          {
            if (layoutMember.Type == FLVER.LayoutType.Byte4A)
            {
              for (int index = 0; index < 4; ++index)
                this.BoneWeights[index] = (float) br.ReadSByte() / (float) sbyte.MaxValue;
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
            {
              for (int index = 0; index < 4; ++index)
                this.BoneWeights[index] = (float) br.ReadByte() / (float) byte.MaxValue;
            }
            else if (layoutMember.Type == FLVER.LayoutType.UVPair)
            {
              for (int index = 0; index < 4; ++index)
                this.BoneWeights[index] = (float) br.ReadInt16() / (float) short.MaxValue;
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Short4toFloat4A)
                throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              for (int index = 0; index < 4; ++index)
                this.BoneWeights[index] = (float) br.ReadInt16() / (float) short.MaxValue;
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.BoneIndices)
          {
            if (layoutMember.Type == FLVER.LayoutType.Byte4B)
            {
              for (int index = 0; index < 4; ++index)
                this.BoneIndices[index] = (int) br.ReadByte();
            }
            else if (layoutMember.Type == FLVER.LayoutType.ShortBoneIndices)
            {
              for (int index = 0; index < 4; ++index)
                this.BoneIndices[index] = (int) br.ReadUInt16();
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4E)
                throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              for (int index = 0; index < 4; ++index)
                this.BoneIndices[index] = (int) br.ReadByte();
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.Normal)
          {
            if (layoutMember.Type == FLVER.LayoutType.Float3)
              this.Normal = br.ReadVector3();
            else if (layoutMember.Type == FLVER.LayoutType.Float4)
            {
              this.Normal = br.ReadVector3();
              float num = br.ReadSingle();
              this.NormalW = (int) num;
              if ((double) num != (double) this.NormalW)
                throw new InvalidDataException(string.Format("Float4 Normal W was not a whole number: {0}", (object) num));
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4A)
            {
              this.Normal = FLVER.Vertex.ReadByteNormXYZ(br);
              this.NormalW = (int) br.ReadByte();
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4B)
            {
              this.Normal = FLVER.Vertex.ReadByteNormXYZ(br);
              this.NormalW = (int) br.ReadByte();
            }
            else if (layoutMember.Type == FLVER.LayoutType.Short2toFloat2)
            {
              this.NormalW = (int) br.ReadByte();
              this.Normal = FLVER.Vertex.ReadSByteNormZYX(br);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
            {
              this.Normal = FLVER.Vertex.ReadByteNormXYZ(br);
              this.NormalW = (int) br.ReadByte();
            }
            else if (layoutMember.Type == FLVER.LayoutType.Short4toFloat4A)
            {
              this.Normal = FLVER.Vertex.ReadShortNormXYZ(br);
              this.NormalW = (int) br.ReadInt16();
            }
            else if (layoutMember.Type == FLVER.LayoutType.Short4toFloat4B)
            {
              this.Normal = FLVER.Vertex.ReadUShortNormXYZ(br);
              this.NormalW = (int) br.ReadInt16();
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4E)
                throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              this.Normal = FLVER.Vertex.ReadByteNormXYZ(br);
              this.NormalW = (int) br.ReadByte();
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.UV)
          {
            if (layoutMember.Type == FLVER.LayoutType.Float2)
              this.UVs.Add(new Vector3(br.ReadVector2(), 0.0f));
            else if (layoutMember.Type == FLVER.LayoutType.Float3)
              this.UVs.Add(br.ReadVector3());
            else if (layoutMember.Type == FLVER.LayoutType.Float4)
            {
              this.UVs.Add(new Vector3(br.ReadVector2(), 0.0f));
              this.UVs.Add(new Vector3(br.ReadVector2(), 0.0f));
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4A)
              this.UVs.Add(new Vector3((float) br.ReadInt16(), (float) br.ReadInt16(), 0.0f) / uvFactor);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4B)
              this.UVs.Add(new Vector3((float) br.ReadInt16(), (float) br.ReadInt16(), 0.0f) / uvFactor);
            else if (layoutMember.Type == FLVER.LayoutType.Short2toFloat2)
              this.UVs.Add(new Vector3((float) br.ReadInt16(), (float) br.ReadInt16(), 0.0f) / uvFactor);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
              this.UVs.Add(new Vector3((float) br.ReadInt16(), (float) br.ReadInt16(), 0.0f) / uvFactor);
            else if (layoutMember.Type == FLVER.LayoutType.UV)
              this.UVs.Add(new Vector3((float) br.ReadInt16(), (float) br.ReadInt16(), 0.0f) / uvFactor);
            else if (layoutMember.Type == FLVER.LayoutType.UVPair)
            {
              this.UVs.Add(new Vector3((float) br.ReadInt16(), (float) br.ReadInt16(), 0.0f) / uvFactor);
              this.UVs.Add(new Vector3((float) br.ReadInt16(), (float) br.ReadInt16(), 0.0f) / uvFactor);
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Short4toFloat4B)
                throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              this.UVs.Add(new Vector3((float) br.ReadInt16(), (float) br.ReadInt16(), (float) br.ReadInt16()) / uvFactor);
              int num = (int) br.AssertInt16(new short[1]);
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.Tangent)
          {
            if (layoutMember.Type == FLVER.LayoutType.Float4)
              this.Tangents.Add(br.ReadVector4());
            else if (layoutMember.Type == FLVER.LayoutType.Byte4A)
              this.Tangents.Add(FLVER.Vertex.ReadByteNormXYZW(br));
            else if (layoutMember.Type == FLVER.LayoutType.Byte4B)
              this.Tangents.Add(FLVER.Vertex.ReadByteNormXYZW(br));
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
              this.Tangents.Add(FLVER.Vertex.ReadByteNormXYZW(br));
            else if (layoutMember.Type == FLVER.LayoutType.Short4toFloat4A)
            {
              this.Tangents.Add(FLVER.Vertex.ReadShortNormXYZW(br));
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4E)
                throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              this.Tangents.Add(FLVER.Vertex.ReadByteNormXYZW(br));
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.Bitangent)
          {
            if (layoutMember.Type == FLVER.LayoutType.Byte4A)
              this.Bitangent = FLVER.Vertex.ReadByteNormXYZW(br);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4B)
              this.Bitangent = FLVER.Vertex.ReadByteNormXYZW(br);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
            {
              this.Bitangent = FLVER.Vertex.ReadByteNormXYZW(br);
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4E)
                throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              this.Bitangent = FLVER.Vertex.ReadByteNormXYZW(br);
            }
          }
          else
          {
            if (layoutMember.Semantic != FLVER.LayoutSemantic.VertexColor)
              throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
            if (layoutMember.Type == FLVER.LayoutType.Float4)
              this.Colors.Add(FLVER.VertexColor.ReadFloatRGBA(br));
            else if (layoutMember.Type == FLVER.LayoutType.Byte4A)
            {
              this.Colors.Add(FLVER.VertexColor.ReadByteRGBA(br));
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4C)
                throw new NotImplementedException(string.Format("Read not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              this.Colors.Add(FLVER.VertexColor.ReadByteRGBA(br));
            }
          }
        }
      }

      private static float ReadByteNorm(BinaryReaderEx br)
      {
        return (float) ((int) br.ReadByte() - (int) sbyte.MaxValue) / (float) sbyte.MaxValue;
      }

      private static Vector3 ReadByteNormXYZ(BinaryReaderEx br)
      {
        return new Vector3(FLVER.Vertex.ReadByteNorm(br), FLVER.Vertex.ReadByteNorm(br), FLVER.Vertex.ReadByteNorm(br));
      }

      private static Vector4 ReadByteNormXYZW(BinaryReaderEx br)
      {
        return new Vector4(FLVER.Vertex.ReadByteNorm(br), FLVER.Vertex.ReadByteNorm(br), FLVER.Vertex.ReadByteNorm(br), FLVER.Vertex.ReadByteNorm(br));
      }

      private static float ReadSByteNorm(BinaryReaderEx br)
      {
        return (float) br.ReadSByte() / (float) sbyte.MaxValue;
      }

      private static Vector3 ReadSByteNormZYX(BinaryReaderEx br)
      {
        float z = FLVER.Vertex.ReadSByteNorm(br);
        float y = FLVER.Vertex.ReadSByteNorm(br);
        return new Vector3(FLVER.Vertex.ReadSByteNorm(br), y, z);
      }

      private static float ReadShortNorm(BinaryReaderEx br)
      {
        return (float) br.ReadInt16() / (float) short.MaxValue;
      }

      private static Vector3 ReadShortNormXYZ(BinaryReaderEx br)
      {
        return new Vector3(FLVER.Vertex.ReadShortNorm(br), FLVER.Vertex.ReadShortNorm(br), FLVER.Vertex.ReadShortNorm(br));
      }

      private static Vector4 ReadShortNormXYZW(BinaryReaderEx br)
      {
        return new Vector4(FLVER.Vertex.ReadShortNorm(br), FLVER.Vertex.ReadShortNorm(br), FLVER.Vertex.ReadShortNorm(br), FLVER.Vertex.ReadShortNorm(br));
      }

      private static float ReadUShortNorm(BinaryReaderEx br)
      {
        return (float) ((int) br.ReadUInt16() - (int) short.MaxValue) / (float) short.MaxValue;
      }

      private static Vector3 ReadUShortNormXYZ(BinaryReaderEx br)
      {
        return new Vector3(FLVER.Vertex.ReadUShortNorm(br), FLVER.Vertex.ReadUShortNorm(br), FLVER.Vertex.ReadUShortNorm(br));
      }

      internal void Write(BinaryWriterEx bw, List<FLVER.LayoutMember> layout, float uvFactor)
      {
        foreach (FLVER.LayoutMember layoutMember in layout)
        {
          if (layoutMember.Semantic == FLVER.LayoutSemantic.Position)
          {
            if (layoutMember.Type == FLVER.LayoutType.Float3)
            {
              bw.WriteVector3(this.Position);
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Float4)
                throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              bw.WriteVector3(this.Position);
              bw.WriteSingle(0.0f);
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.BoneWeights)
          {
            if (layoutMember.Type == FLVER.LayoutType.Byte4A)
            {
              for (int index = 0; index < 4; ++index)
                bw.WriteSByte((sbyte) Math.Round((double) this.BoneWeights[index] * (double) sbyte.MaxValue));
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
            {
              for (int index = 0; index < 4; ++index)
                bw.WriteByte((byte) Math.Round((double) this.BoneWeights[index] * (double) byte.MaxValue));
            }
            else if (layoutMember.Type == FLVER.LayoutType.UVPair)
            {
              for (int index = 0; index < 4; ++index)
                bw.WriteInt16((short) Math.Round((double) this.BoneWeights[index] * (double) short.MaxValue));
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Short4toFloat4A)
                throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              for (int index = 0; index < 4; ++index)
                bw.WriteInt16((short) Math.Round((double) this.BoneWeights[index] * (double) short.MaxValue));
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.BoneIndices)
          {
            if (layoutMember.Type == FLVER.LayoutType.Byte4B)
            {
              for (int index = 0; index < 4; ++index)
                bw.WriteByte((byte) this.BoneIndices[index]);
            }
            else if (layoutMember.Type == FLVER.LayoutType.ShortBoneIndices)
            {
              for (int index = 0; index < 4; ++index)
                bw.WriteUInt16((ushort) this.BoneIndices[index]);
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4E)
                throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              for (int index = 0; index < 4; ++index)
                bw.WriteByte((byte) this.BoneIndices[index]);
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.Normal)
          {
            if (layoutMember.Type == FLVER.LayoutType.Float3)
              bw.WriteVector3(this.Normal);
            else if (layoutMember.Type == FLVER.LayoutType.Float4)
            {
              bw.WriteVector3(this.Normal);
              bw.WriteSingle((float) this.NormalW);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4A)
            {
              FLVER.Vertex.WriteByteNormXYZ(bw, this.Normal);
              bw.WriteByte((byte) this.NormalW);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4B)
            {
              FLVER.Vertex.WriteByteNormXYZ(bw, this.Normal);
              bw.WriteByte((byte) this.NormalW);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Short2toFloat2)
            {
              bw.WriteByte((byte) this.NormalW);
              FLVER.Vertex.WriteSByteNormZYX(bw, this.Normal);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
            {
              FLVER.Vertex.WriteByteNormXYZ(bw, this.Normal);
              bw.WriteByte((byte) this.NormalW);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Short4toFloat4A)
            {
              FLVER.Vertex.WriteShortNormXYZ(bw, this.Normal);
              bw.WriteInt16((short) this.NormalW);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Short4toFloat4B)
            {
              FLVER.Vertex.WriteUShortNormXYZ(bw, this.Normal);
              bw.WriteInt16((short) this.NormalW);
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4E)
                throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              FLVER.Vertex.WriteByteNormXYZ(bw, this.Normal);
              bw.WriteByte((byte) this.NormalW);
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.UV)
          {
            Vector3 vector = this.uvQueue.Dequeue() * uvFactor;
            if (layoutMember.Type == FLVER.LayoutType.Float2)
            {
              bw.WriteSingle(vector.X);
              bw.WriteSingle(vector.Y);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Float3)
              bw.WriteVector3(vector);
            else if (layoutMember.Type == FLVER.LayoutType.Float4)
            {
              bw.WriteSingle(vector.X);
              bw.WriteSingle(vector.Y);
              Vector3 vector3 = this.uvQueue.Dequeue() * uvFactor;
              bw.WriteSingle(vector3.X);
              bw.WriteSingle(vector3.Y);
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4A)
            {
              bw.WriteInt16((short) Math.Round((double) vector.X));
              bw.WriteInt16((short) Math.Round((double) vector.Y));
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4B)
            {
              bw.WriteInt16((short) Math.Round((double) vector.X));
              bw.WriteInt16((short) Math.Round((double) vector.Y));
            }
            else if (layoutMember.Type == FLVER.LayoutType.Short2toFloat2)
            {
              bw.WriteInt16((short) Math.Round((double) vector.X));
              bw.WriteInt16((short) Math.Round((double) vector.Y));
            }
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
            {
              bw.WriteInt16((short) Math.Round((double) vector.X));
              bw.WriteInt16((short) Math.Round((double) vector.Y));
            }
            else if (layoutMember.Type == FLVER.LayoutType.UV)
            {
              bw.WriteInt16((short) Math.Round((double) vector.X));
              bw.WriteInt16((short) Math.Round((double) vector.Y));
            }
            else if (layoutMember.Type == FLVER.LayoutType.UVPair)
            {
              bw.WriteInt16((short) Math.Round((double) vector.X));
              bw.WriteInt16((short) Math.Round((double) vector.Y));
              Vector3 vector3 = this.uvQueue.Dequeue() * uvFactor;
              bw.WriteInt16((short) Math.Round((double) vector3.X));
              bw.WriteInt16((short) Math.Round((double) vector3.Y));
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Short4toFloat4B)
                throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              bw.WriteInt16((short) Math.Round((double) vector.X));
              bw.WriteInt16((short) Math.Round((double) vector.Y));
              bw.WriteInt16((short) Math.Round((double) vector.Z));
              bw.WriteInt16((short) 0);
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.Tangent)
          {
            Vector4 vector = this.tangentQueue.Dequeue();
            if (layoutMember.Type == FLVER.LayoutType.Float4)
              bw.WriteVector4(vector);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4A)
              FLVER.Vertex.WriteByteNormXYZW(bw, vector);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4B)
              FLVER.Vertex.WriteByteNormXYZW(bw, vector);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
              FLVER.Vertex.WriteByteNormXYZW(bw, vector);
            else if (layoutMember.Type == FLVER.LayoutType.Short4toFloat4A)
            {
              FLVER.Vertex.WriteShortNormXYZW(bw, vector);
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4E)
                throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              FLVER.Vertex.WriteByteNormXYZW(bw, vector);
            }
          }
          else if (layoutMember.Semantic == FLVER.LayoutSemantic.Bitangent)
          {
            if (layoutMember.Type == FLVER.LayoutType.Byte4A)
              FLVER.Vertex.WriteByteNormXYZW(bw, this.Bitangent);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4B)
              FLVER.Vertex.WriteByteNormXYZW(bw, this.Bitangent);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4C)
            {
              FLVER.Vertex.WriteByteNormXYZW(bw, this.Bitangent);
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4E)
                throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              FLVER.Vertex.WriteByteNormXYZW(bw, this.Bitangent);
            }
          }
          else
          {
            if (layoutMember.Semantic != FLVER.LayoutSemantic.VertexColor)
              throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
            FLVER.VertexColor vertexColor = this.colorQueue.Dequeue();
            if (layoutMember.Type == FLVER.LayoutType.Float4)
              vertexColor.WriteFloatRGBA(bw);
            else if (layoutMember.Type == FLVER.LayoutType.Byte4A)
            {
              vertexColor.WriteByteRGBA(bw);
            }
            else
            {
              if (layoutMember.Type != FLVER.LayoutType.Byte4C)
                throw new NotImplementedException(string.Format("Write not implemented for {0} {1}.", (object) layoutMember.Type, (object) layoutMember.Semantic));
              vertexColor.WriteByteRGBA(bw);
            }
          }
        }
      }

      private static void WriteByteNorm(BinaryWriterEx bw, float value)
      {
        bw.WriteByte((byte) Math.Round((double) value * (double) sbyte.MaxValue + (double) sbyte.MaxValue));
      }

      private static void WriteByteNormXYZ(BinaryWriterEx bw, Vector3 value)
      {
        FLVER.Vertex.WriteByteNorm(bw, value.X);
        FLVER.Vertex.WriteByteNorm(bw, value.Y);
        FLVER.Vertex.WriteByteNorm(bw, value.Z);
      }

      private static void WriteByteNormXYZW(BinaryWriterEx bw, Vector4 value)
      {
        FLVER.Vertex.WriteByteNorm(bw, value.X);
        FLVER.Vertex.WriteByteNorm(bw, value.Y);
        FLVER.Vertex.WriteByteNorm(bw, value.Z);
        FLVER.Vertex.WriteByteNorm(bw, value.W);
      }

      private static void WriteSByteNorm(BinaryWriterEx bw, float value)
      {
        bw.WriteSByte((sbyte) Math.Round((double) value * (double) sbyte.MaxValue));
      }

      private static void WriteSByteNormZYX(BinaryWriterEx bw, Vector3 value)
      {
        FLVER.Vertex.WriteSByteNorm(bw, value.Z);
        FLVER.Vertex.WriteSByteNorm(bw, value.Y);
        FLVER.Vertex.WriteSByteNorm(bw, value.X);
      }

      private static void WriteShortNorm(BinaryWriterEx bw, float value)
      {
        bw.WriteInt16((short) Math.Round((double) value * (double) short.MaxValue));
      }

      private static void WriteShortNormXYZ(BinaryWriterEx bw, Vector3 value)
      {
        FLVER.Vertex.WriteShortNorm(bw, value.X);
        FLVER.Vertex.WriteShortNorm(bw, value.Y);
        FLVER.Vertex.WriteShortNorm(bw, value.Z);
      }

      private static void WriteShortNormXYZW(BinaryWriterEx bw, Vector4 value)
      {
        FLVER.Vertex.WriteShortNorm(bw, value.X);
        FLVER.Vertex.WriteShortNorm(bw, value.Y);
        FLVER.Vertex.WriteShortNorm(bw, value.Z);
        FLVER.Vertex.WriteShortNorm(bw, value.W);
      }

      private static void WriteUShortNorm(BinaryWriterEx bw, float value)
      {
        bw.WriteUInt16((ushort) Math.Round((double) value * (double) short.MaxValue + (double) short.MaxValue));
      }

      private static void WriteUShortNormXYZ(BinaryWriterEx bw, Vector3 value)
      {
        FLVER.Vertex.WriteUShortNorm(bw, value.X);
        FLVER.Vertex.WriteUShortNorm(bw, value.Y);
        FLVER.Vertex.WriteUShortNorm(bw, value.Z);
      }
    }

    public struct VertexBoneIndices
    {
      private int A;
      private int B;
      private int C;
      private int D;

      public int Length
      {
        get
        {
          return 4;
        }
      }

      public int this[int i]
      {
        get
        {
          switch (i)
          {
            case 0:
              return this.A;
            case 1:
              return this.B;
            case 2:
              return this.C;
            case 3:
              return this.D;
            default:
              throw new IndexOutOfRangeException(string.Format("Index ({0}) was out of range. Must be non-negative and less than 4.", (object) i));
          }
        }
        set
        {
          switch (i)
          {
            case 0:
              this.A = value;
              break;
            case 1:
              this.B = value;
              break;
            case 2:
              this.C = value;
              break;
            case 3:
              this.D = value;
              break;
            default:
              throw new IndexOutOfRangeException(string.Format("Index ({0}) was out of range. Must be non-negative and less than 4.", (object) i));
          }
        }
      }
    }

    public struct VertexBoneWeights
    {
      private float A;
      private float B;
      private float C;
      private float D;

      public int Length
      {
        get
        {
          return 4;
        }
      }

      public float this[int i]
      {
        get
        {
          switch (i)
          {
            case 0:
              return this.A;
            case 1:
              return this.B;
            case 2:
              return this.C;
            case 3:
              return this.D;
            default:
              throw new IndexOutOfRangeException(string.Format("Index ({0}) was out of range. Must be non-negative and less than 4.", (object) i));
          }
        }
        set
        {
          switch (i)
          {
            case 0:
              this.A = value;
              break;
            case 1:
              this.B = value;
              break;
            case 2:
              this.C = value;
              break;
            case 3:
              this.D = value;
              break;
            default:
              throw new IndexOutOfRangeException(string.Format("Index ({0}) was out of range. Must be non-negative and less than 4.", (object) i));
          }
        }
      }
    }

    public struct VertexColor
    {
      public float A;
      public float R;
      public float G;
      public float B;

      public VertexColor(float a, float r, float g, float b)
      {
        this.A = a;
        this.R = r;
        this.G = g;
        this.B = b;
      }

      public VertexColor(byte a, byte r, byte g, byte b)
      {
        this.A = (float) a / (float) byte.MaxValue;
        this.R = (float) r / (float) byte.MaxValue;
        this.G = (float) g / (float) byte.MaxValue;
        this.B = (float) b / (float) byte.MaxValue;
      }

      internal static FLVER.VertexColor ReadFloatRGBA(BinaryReaderEx br)
      {
        float r = br.ReadSingle();
        float g = br.ReadSingle();
        float b = br.ReadSingle();
        return new FLVER.VertexColor(br.ReadSingle(), r, g, b);
      }

      internal static FLVER.VertexColor ReadByteARGB(BinaryReaderEx br)
      {
        int num1 = (int) br.ReadByte();
        byte num2 = br.ReadByte();
        byte num3 = br.ReadByte();
        byte num4 = br.ReadByte();
        int num5 = (int) num2;
        int num6 = (int) num3;
        int num7 = (int) num4;
        return new FLVER.VertexColor((byte) num1, (byte) num5, (byte) num6, (byte) num7);
      }

      internal static FLVER.VertexColor ReadByteRGBA(BinaryReaderEx br)
      {
        byte r = br.ReadByte();
        byte g = br.ReadByte();
        byte b = br.ReadByte();
        return new FLVER.VertexColor(br.ReadByte(), r, g, b);
      }

      internal void WriteFloatRGBA(BinaryWriterEx bw)
      {
        bw.WriteSingle(this.R);
        bw.WriteSingle(this.G);
        bw.WriteSingle(this.B);
        bw.WriteSingle(this.A);
      }

      internal void WriteByteARGB(BinaryWriterEx bw)
      {
        bw.WriteByte((byte) Math.Round((double) this.A * (double) byte.MaxValue));
        bw.WriteByte((byte) Math.Round((double) this.R * (double) byte.MaxValue));
        bw.WriteByte((byte) Math.Round((double) this.G * (double) byte.MaxValue));
        bw.WriteByte((byte) Math.Round((double) this.B * (double) byte.MaxValue));
      }

      internal void WriteByteRGBA(BinaryWriterEx bw)
      {
        bw.WriteByte((byte) Math.Round((double) this.R * (double) byte.MaxValue));
        bw.WriteByte((byte) Math.Round((double) this.G * (double) byte.MaxValue));
        bw.WriteByte((byte) Math.Round((double) this.B * (double) byte.MaxValue));
        bw.WriteByte((byte) Math.Round((double) this.A * (double) byte.MaxValue));
      }
    }
  }
}
