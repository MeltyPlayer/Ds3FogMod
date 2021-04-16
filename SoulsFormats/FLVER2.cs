// Decompiled with JetBrains decompiler
// Type: SoulsFormats.FLVER2
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
  public class FLVER2 : SoulsFile<FLVER2>, IFlver {
    public FLVER2.FLVERHeader Header { get; set; }

    public List<FLVER.Dummy> Dummies { get; set; }

    IReadOnlyList<FLVER.Dummy> IFlver.Dummies {
      get { return (IReadOnlyList<FLVER.Dummy>) this.Dummies; }
    }

    public List<FLVER2.Material> Materials { get; set; }

    IReadOnlyList<IFlverMaterial> IFlver.Materials {
      get { return (IReadOnlyList<IFlverMaterial>) this.Materials; }
    }

    public List<FLVER2.GXList> GXLists { get; set; }

    public List<FLVER.Bone> Bones { get; set; }

    IReadOnlyList<FLVER.Bone> IFlver.Bones {
      get { return (IReadOnlyList<FLVER.Bone>) this.Bones; }
    }

    public List<FLVER2.Mesh> Meshes { get; set; }

    IReadOnlyList<IFlverMesh> IFlver.Meshes {
      get { return (IReadOnlyList<IFlverMesh>) this.Meshes; }
    }

    public List<FLVER2.BufferLayout> BufferLayouts { get; set; }

    public FLVER2.SekiroUnkStruct SekiroUnk { get; set; }

    public FLVER2() {
      this.Header = new FLVER2.FLVERHeader();
      this.Dummies = new List<FLVER.Dummy>();
      this.Materials = new List<FLVER2.Material>();
      this.GXLists = new List<FLVER2.GXList>();
      this.Bones = new List<FLVER.Bone>();
      this.Meshes = new List<FLVER2.Mesh>();
      this.BufferLayouts = new List<FLVER2.BufferLayout>();
    }

    protected override bool Is(BinaryReaderEx br) {
      if (br.Length < 12L)
        return false;
      string ascii1 = br.GetASCII(0L, 6);
      string ascii2 = br.GetASCII(6L, 2);
      br.BigEndian = ascii2 == "B\0";
      int int32 = br.GetInt32(8L);
      return ascii1 == "FLVER\0" && int32 >= 131072;
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      this.Header = new FLVER2.FLVERHeader();
      br.AssertASCII("FLVER\0");
      this.Header.BigEndian =
          (br.AssertASCII("L\0", "B\0") == "B\0" ? 1 : 0) != 0;
      br.BigEndian = this.Header.BigEndian;
      this.Header.Version = br.AssertInt32(131077,
                                           131081,
                                           131084,
                                           131085,
                                           131086,
                                           131087,
                                           131088,
                                           131091,
                                           131092,
                                           131094,
                                           131098);
      int dataOffset = br.ReadInt32();
      br.ReadInt32();
      int capacity1 = br.ReadInt32();
      int capacity2 = br.ReadInt32();
      int capacity3 = br.ReadInt32();
      int capacity4 = br.ReadInt32();
      int capacity5 = br.ReadInt32();
      this.Header.BoundingBoxMin = br.ReadVector3();
      this.Header.BoundingBoxMax = br.ReadVector3();
      br.ReadInt32();
      br.ReadInt32();
      int headerIndexSize = (int) br.AssertByte((byte) 0, (byte) 16, (byte) 32);
      this.Header.Unicode = br.ReadBoolean();
      this.Header.Unk4A = br.ReadBoolean();
      int num1 = (int) br.AssertByte(new byte[1]);
      this.Header.Unk4C = br.ReadInt32();
      int capacity6 = br.ReadInt32();
      int capacity7 = br.ReadInt32();
      int capacity8 = br.ReadInt32();
      this.Header.Unk5C = br.ReadByte();
      this.Header.Unk5D = br.ReadByte();
      int num2 = (int) br.AssertByte(new byte[1]);
      int num3 = (int) br.AssertByte(new byte[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      this.Header.Unk68 = br.AssertInt32(0, 1, 2, 3, 4);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      this.Dummies = new List<FLVER.Dummy>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Dummies.Add(new FLVER.Dummy(br, this.Header.Version));
      this.Materials = new List<FLVER2.Material>(capacity2);
      Dictionary<int, int> gxListIndices = new Dictionary<int, int>();
      this.GXLists = new List<FLVER2.GXList>();
      for (int index = 0; index < capacity2; ++index)
        this.Materials.Add(
            new FLVER2.Material(br,
                                this.Header,
                                this.GXLists,
                                gxListIndices));
      this.Bones = new List<FLVER.Bone>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.Bones.Add(new FLVER.Bone(br, this.Header.Unicode));
      this.Meshes = new List<FLVER2.Mesh>(capacity4);
      for (int index = 0; index < capacity4; ++index)
        this.Meshes.Add(new FLVER2.Mesh(br, this.Header));
      List<FLVER2.FaceSet> items1 = new List<FLVER2.FaceSet>(capacity6);
      for (int index = 0; index < capacity6; ++index)
        items1.Add(
            new FLVER2.FaceSet(br, this.Header, headerIndexSize, dataOffset));
      List<FLVER2.VertexBuffer> items2 =
          new List<FLVER2.VertexBuffer>(capacity5);
      for (int index = 0; index < capacity5; ++index)
        items2.Add(new FLVER2.VertexBuffer(br));
      this.BufferLayouts = new List<FLVER2.BufferLayout>(capacity7);
      for (int index = 0; index < capacity7; ++index)
        this.BufferLayouts.Add(new FLVER2.BufferLayout(br));
      List<FLVER2.Texture> items3 = new List<FLVER2.Texture>(capacity8);
      for (int index = 0; index < capacity8; ++index)
        items3.Add(new FLVER2.Texture(br, this.Header));
      if (this.Header.Version >= 131098)
        this.SekiroUnk = new FLVER2.SekiroUnkStruct(br);
      Dictionary<int, FLVER2.Texture> textureDict =
          SFUtil.Dictionize<FLVER2.Texture>(items3);
      foreach (FLVER2.Material material in this.Materials)
        material.TakeTextures(textureDict);
      if (textureDict.Count != 0)
        throw new NotSupportedException("Orphaned textures found.");
      Dictionary<int, FLVER2.FaceSet> faceSetDict =
          SFUtil.Dictionize<FLVER2.FaceSet>(items1);
      Dictionary<int, FLVER2.VertexBuffer> vertexBufferDict =
          SFUtil.Dictionize<FLVER2.VertexBuffer>(items2);
      foreach (FLVER2.Mesh mesh in this.Meshes) {
        mesh.TakeFaceSets(faceSetDict);
        mesh.TakeVertexBuffers(vertexBufferDict, this.BufferLayouts);
        mesh.ReadVertices(br, dataOffset, this.BufferLayouts, this.Header);
      }
      if (faceSetDict.Count != 0)
        throw new NotSupportedException("Orphaned face sets found.");
      if (vertexBufferDict.Count != 0)
        throw new NotSupportedException("Orphaned vertex buffers found.");
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = this.Header.BigEndian;
      bw.WriteASCII("FLVER\0", false);
      bw.WriteASCII(this.Header.BigEndian ? "B\0" : "L\0", false);
      bw.WriteInt32(this.Header.Version);
      bw.ReserveInt32("DataOffset");
      bw.ReserveInt32("DataSize");
      bw.WriteInt32(this.Dummies.Count);
      bw.WriteInt32(this.Materials.Count);
      bw.WriteInt32(this.Bones.Count);
      bw.WriteInt32(this.Meshes.Count);
      bw.WriteInt32(
          this.Meshes.Sum<FLVER2.Mesh>(
              (Func<FLVER2.Mesh, int>) (m => m.VertexBuffers.Count)));
      bw.WriteVector3(this.Header.BoundingBoxMin);
      bw.WriteVector3(this.Header.BoundingBoxMax);
      int trueFaceCount = 0;
      int totalFaceCount = 0;
      foreach (FLVER2.Mesh mesh in this.Meshes) {
        bool allowPrimitiveRestarts =
            mesh.Vertices.Count < (int) ushort.MaxValue;
        foreach (FLVER2.FaceSet faceSet in mesh.FaceSets)
          faceSet.AddFaceCounts(allowPrimitiveRestarts,
                                ref trueFaceCount,
                                ref totalFaceCount);
      }
      bw.WriteInt32(trueFaceCount);
      bw.WriteInt32(totalFaceCount);
      byte num1 = 0;
      if (this.Header.Version < 131091) {
        num1 = (byte) 16;
        foreach (FLVER2.Mesh mesh in this.Meshes) {
          foreach (FLVER2.FaceSet faceSet in mesh.FaceSets)
            num1 = (byte) Math.Max((int) num1, faceSet.GetVertexIndexSize());
        }
      }
      bw.WriteByte(num1);
      bw.WriteBoolean(this.Header.Unicode);
      bw.WriteBoolean(this.Header.Unk4A);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(this.Header.Unk4C);
      bw.WriteInt32(
          this.Meshes.Sum<FLVER2.Mesh>(
              (Func<FLVER2.Mesh, int>) (m => m.FaceSets.Count)));
      bw.WriteInt32(this.BufferLayouts.Count);
      bw.WriteInt32(this.Materials.Sum<FLVER2.Material>(
                        (Func<FLVER2.Material, int>)
                        (m => m.Textures.Count)));
      bw.WriteByte(this.Header.Unk5C);
      bw.WriteByte(this.Header.Unk5D);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(this.Header.Unk68);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      foreach (FLVER.Dummy dummy in this.Dummies)
        dummy.Write(bw, this.Header.Version);
      for (int index = 0; index < this.Materials.Count; ++index)
        this.Materials[index].Write(bw, index);
      for (int index = 0; index < this.Bones.Count; ++index)
        this.Bones[index].Write(bw, index);
      for (int index = 0; index < this.Meshes.Count; ++index)
        this.Meshes[index].Write(bw, index);
      int num2 = 0;
      foreach (FLVER2.Mesh mesh in this.Meshes) {
        for (int index = 0; index < mesh.FaceSets.Count; ++index) {
          int indexSize = (int) num1;
          if (indexSize == 0)
            indexSize = mesh.FaceSets[index].GetVertexIndexSize();
          mesh.FaceSets[index].Write(bw, this.Header, indexSize, num2 + index);
        }
        num2 += mesh.FaceSets.Count;
      }
      int num3 = 0;
      foreach (FLVER2.Mesh mesh in this.Meshes) {
        for (int bufferIndex = 0;
             bufferIndex < mesh.VertexBuffers.Count;
             ++bufferIndex)
          mesh.VertexBuffers[bufferIndex]
              .Write(bw,
                     this.Header,
                     num3 + bufferIndex,
                     bufferIndex,
                     this.BufferLayouts,
                     mesh.Vertices.Count);
        num3 += mesh.VertexBuffers.Count;
      }
      for (int index = 0; index < this.BufferLayouts.Count; ++index)
        this.BufferLayouts[index].Write(bw, index);
      int textureIndex = 0;
      for (int index = 0; index < this.Materials.Count; ++index) {
        this.Materials[index].WriteTextures(bw, index, textureIndex);
        textureIndex += this.Materials[index].Textures.Count;
      }
      if (this.Header.Version >= 131098)
        this.SekiroUnk.Write(bw);
      bw.Pad(16);
      for (int index = 0; index < this.BufferLayouts.Count; ++index)
        this.BufferLayouts[index].WriteMembers(bw, index);
      bw.Pad(16);
      for (int index = 0; index < this.Meshes.Count; ++index)
        this.Meshes[index].WriteBoundingBox(bw, index, this.Header);
      bw.Pad(16);
      int position1 = (int) bw.Position;
      for (int index = 0; index < this.Meshes.Count; ++index)
        this.Meshes[index].WriteBoneIndices(bw, index, position1);
      bw.Pad(16);
      int num4 = 0;
      for (int index1 = 0; index1 < this.Meshes.Count; ++index1) {
        bw.FillInt32(string.Format("MeshFaceSetIndices{0}", (object) index1),
                     (int) bw.Position);
        for (int index2 = 0;
             index2 < this.Meshes[index1].FaceSets.Count;
             ++index2)
          bw.WriteInt32(num4 + index2);
        num4 += this.Meshes[index1].FaceSets.Count;
      }
      bw.Pad(16);
      int num5 = 0;
      for (int index1 = 0; index1 < this.Meshes.Count; ++index1) {
        bw.FillInt32(
            string.Format("MeshVertexBufferIndices{0}", (object) index1),
            (int) bw.Position);
        for (int index2 = 0;
             index2 < this.Meshes[index1].VertexBuffers.Count;
             ++index2)
          bw.WriteInt32(num5 + index2);
        num5 += this.Meshes[index1].VertexBuffers.Count;
      }
      bw.Pad(16);
      List<int> gxOffsets = new List<int>();
      foreach (FLVER2.GXList gxList in this.GXLists) {
        gxOffsets.Add((int) bw.Position);
        BinaryWriterEx bw1 = bw;
        FLVER2.FLVERHeader header = this.Header;
        gxList.Write(bw1, header);
      }
      for (int index = 0; index < this.Materials.Count; ++index)
        this.Materials[index].FillGXOffset(bw, index, gxOffsets);
      bw.Pad(16);
      int num6 = 0;
      for (int index1 = 0; index1 < this.Materials.Count; ++index1) {
        FLVER2.Material material = this.Materials[index1];
        material.WriteStrings(bw, this.Header, index1);
        for (int index2 = 0; index2 < material.Textures.Count; ++index2)
          material.Textures[index2]
                  .WriteStrings(bw, this.Header, num6 + index2);
        num6 += material.Textures.Count;
      }
      bw.Pad(16);
      for (int index = 0; index < this.Bones.Count; ++index)
        this.Bones[index].WriteStrings(bw, this.Header.Unicode, index);
      int align = this.Header.Version <= 131086 ? 32 : 16;
      bw.Pad(align);
      if (this.Header.Version == 131087 || this.Header.Version == 131088)
        bw.Pad(32);
      int position2 = (int) bw.Position;
      bw.FillInt32("DataOffset", position2);
      int num7 = 0;
      int num8 = 0;
      for (int index1 = 0; index1 < this.Meshes.Count; ++index1) {
        FLVER2.Mesh mesh = this.Meshes[index1];
        for (int index2 = 0; index2 < mesh.FaceSets.Count; ++index2) {
          int indexSize = (int) num1;
          if (indexSize == 0)
            indexSize = mesh.FaceSets[index2].GetVertexIndexSize();
          bw.Pad(align);
          mesh.FaceSets[index2]
              .WriteVertices(bw, indexSize, num7 + index2, position2);
        }
        num7 += mesh.FaceSets.Count;
        foreach (FLVER.Vertex vertex in mesh.Vertices)
          vertex.PrepareWrite();
        for (int index2 = 0; index2 < mesh.VertexBuffers.Count; ++index2) {
          bw.Pad(align);
          mesh.VertexBuffers[index2]
              .WriteBuffer(bw,
                           num8 + index2,
                           this.BufferLayouts,
                           mesh.Vertices,
                           position2,
                           this.Header);
        }
        foreach (FLVER.Vertex vertex in mesh.Vertices)
          vertex.FinishWrite();
        num8 += mesh.VertexBuffers.Count;
      }
      bw.Pad(align);
      bw.FillInt32("DataSize", (int) bw.Position - position2);
      if (this.Header.Version != 131087 && this.Header.Version != 131088)
        return;
      bw.Pad(32);
    }

    public class BufferLayout : List<FLVER.LayoutMember> {
      public int Size {
        get {
          return this.Sum<FLVER.LayoutMember>(
              (Func<FLVER.LayoutMember, int>) (member => member.Size));
        }
      }

      public BufferLayout() {}

      internal BufferLayout(BinaryReaderEx br) {
        int num1 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        int num2 = br.ReadInt32();
        br.StepIn((long) num2);
        int structOffset = 0;
        this.Capacity = num1;
        for (int index = 0; index < num1; ++index) {
          FLVER.LayoutMember layoutMember =
              new FLVER.LayoutMember(br, structOffset);
          structOffset += layoutMember.Size;
          this.Add(layoutMember);
        }
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw, int index) {
        bw.WriteInt32(this.Count);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.ReserveInt32(string.Format("VertexStructLayout{0}", (object) index));
      }

      internal void WriteMembers(BinaryWriterEx bw, int index) {
        bw.FillInt32(string.Format("VertexStructLayout{0}", (object) index),
                     (int) bw.Position);
        int structOffset = 0;
        foreach (FLVER.LayoutMember layoutMember in
            (List<FLVER.LayoutMember>) this) {
          layoutMember.Write(bw, structOffset);
          structOffset += layoutMember.Size;
        }
      }
    }

    public class FaceSet {
      public FLVER2.FaceSet.FSFlags Flags { get; set; }

      public bool TriangleStrip { get; set; }

      public bool CullBackfaces { get; set; }

      public short Unk06 { get; set; }

      public List<int> Indices { get; set; }

      public FaceSet() {
        this.Flags = FLVER2.FaceSet.FSFlags.None;
        this.TriangleStrip = false;
        this.CullBackfaces = true;
        this.Indices = new List<int>();
      }

      public FaceSet(
          FLVER2.FaceSet.FSFlags flags,
          bool triangleStrip,
          bool cullBackfaces,
          short unk06,
          List<int> indices) {
        this.Flags = flags;
        this.TriangleStrip = triangleStrip;
        this.CullBackfaces = cullBackfaces;
        this.Unk06 = unk06;
        this.Indices = indices;
      }

      internal FaceSet(
          BinaryReaderEx br,
          FLVER2.FLVERHeader header,
          int headerIndexSize,
          int dataOffset) {
        this.Flags = (FLVER2.FaceSet.FSFlags) br.ReadUInt32();
        this.TriangleStrip = br.ReadBoolean();
        this.CullBackfaces = br.ReadBoolean();
        this.Unk06 = br.ReadInt16();
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int num3 = 0;
        if (header.Version > 131077) {
          br.ReadInt32();
          br.AssertInt32(new int[1]);
          num3 = br.AssertInt32(0, 16, 32);
          br.AssertInt32(new int[1]);
        }
        if (num3 == 0)
          num3 = headerIndexSize;
        if (num3 == 16) {
          this.Indices = new List<int>(num1);
          foreach (int uint16 in br.GetUInt16s((long) (dataOffset + num2), num1)
          )
            this.Indices.Add(uint16);
        } else {
          if (num3 != 32)
            throw new NotImplementedException(
                string.Format("Unsupported index size: {0}", (object) num3));
          this.Indices =
              new List<int>(
                  (IEnumerable<int>) br.GetInt32s(
                      (long) (dataOffset + num2),
                      num1));
        }
      }

      internal void Write(
          BinaryWriterEx bw,
          FLVER2.FLVERHeader header,
          int indexSize,
          int index) {
        bw.WriteUInt32((uint) this.Flags);
        bw.WriteBoolean(this.TriangleStrip);
        bw.WriteBoolean(this.CullBackfaces);
        bw.WriteInt16(this.Unk06);
        bw.WriteInt32(this.Indices.Count);
        bw.ReserveInt32(string.Format("FaceSetVertices{0}", (object) index));
        if (header.Version <= 131077)
          return;
        bw.WriteInt32(this.Indices.Count * (indexSize / 8));
        bw.WriteInt32(0);
        bw.WriteInt32(header.Version >= 131091 ? indexSize : 0);
        bw.WriteInt32(0);
      }

      internal void WriteVertices(
          BinaryWriterEx bw,
          int indexSize,
          int index,
          int dataStart) {
        bw.FillInt32(string.Format("FaceSetVertices{0}", (object) index),
                     (int) bw.Position - dataStart);
        switch (indexSize) {
          case 16:
            using (List<int>.Enumerator enumerator =
                this.Indices.GetEnumerator()) {
              while (enumerator.MoveNext()) {
                int current = enumerator.Current;
                bw.WriteUInt16((ushort) current);
              }
              break;
            }
          case 32:
            bw.WriteInt32s((IList<int>) this.Indices);
            break;
        }
      }

      internal int GetVertexIndexSize() {
        foreach (int index in this.Indices) {
          if (index > 65536)
            return 32;
        }
        return 16;
      }

      internal void AddFaceCounts(
          bool allowPrimitiveRestarts,
          ref int trueFaceCount,
          ref int totalFaceCount) {
        if (this.TriangleStrip) {
          for (int index1 = 0; index1 < this.Indices.Count - 2; ++index1) {
            int index2 = this.Indices[index1];
            int index3 = this.Indices[index1 + 1];
            int index4 = this.Indices[index1 + 2];
            if (!allowPrimitiveRestarts ||
                index2 != (int) ushort.MaxValue &&
                index3 != (int) ushort.MaxValue &&
                index4 != (int) ushort.MaxValue) {
              ++totalFaceCount;
              if ((this.Flags & FLVER2.FaceSet.FSFlags.MotionBlur) ==
                  FLVER2.FaceSet.FSFlags.None &&
                  index2 != index3 &&
                  (index3 != index4 && index4 != index2))
                ++trueFaceCount;
            }
          }
        } else {
          totalFaceCount += this.Indices.Count / 3;
          trueFaceCount += this.Indices.Count / 3;
        }
      }

      public List<int> Triangulate(
          bool allowPrimitiveRestarts,
          bool includeDegenerateFaces = false) {
        if (!this.TriangleStrip)
          return new List<int>((IEnumerable<int>) this.Indices);
        List<int> intList = new List<int>();
        bool flag = false;
        for (int index1 = 0; index1 < this.Indices.Count - 2; ++index1) {
          int index2 = this.Indices[index1];
          int index3 = this.Indices[index1 + 1];
          int index4 = this.Indices[index1 + 2];
          if (allowPrimitiveRestarts &&
              (index2 == (int) ushort.MaxValue ||
               index3 == (int) ushort.MaxValue ||
               index4 == (int) ushort.MaxValue)) {
            flag = false;
          } else {
            if (includeDegenerateFaces ||
                index2 != index3 && index3 != index4 && index4 != index2) {
              if (flag) {
                intList.Add(index4);
                intList.Add(index3);
                intList.Add(index2);
              } else {
                intList.Add(index2);
                intList.Add(index3);
                intList.Add(index4);
              }
            }
            flag = !flag;
          }
        }
        return intList;
      }

      [Flags]
      public enum FSFlags : uint {
        None = 0,
        LodLevel1 = 16777216,    // 0x01000000
        LodLevel2 = 33554432,    // 0x02000000
        MotionBlur = 2147483648, // 0x80000000
      }
    }

    public class FLVERHeader {
      public bool BigEndian { get; set; }

      public int Version { get; set; }

      public Vector3 BoundingBoxMin { get; set; }

      public Vector3 BoundingBoxMax { get; set; }

      public bool Unicode { get; set; }

      public bool Unk4A { get; set; }

      public int Unk4C { get; set; }

      public byte Unk5C { get; set; }

      public byte Unk5D { get; set; }

      public int Unk68 { get; set; }

      public FLVERHeader() {
        this.BigEndian = false;
        this.Version = 131092;
        this.Unicode = true;
      }
    }

    public class GXList : List<FLVER2.GXItem> {
      public int TerminatorLength { get; set; }

      public GXList() {}

      internal GXList(BinaryReaderEx br, FLVER2.FLVERHeader header) {
        while (br.GetInt32(br.Position) != int.MaxValue)
          this.Add(new FLVER2.GXItem(br, header));
        br.AssertInt32(int.MaxValue);
        br.AssertInt32(100);
        this.TerminatorLength = br.ReadInt32() - 12;
        br.AssertPattern(this.TerminatorLength, (byte) 0);
      }

      internal void Write(BinaryWriterEx bw, FLVER2.FLVERHeader header) {
        foreach (FLVER2.GXItem gxItem in (List<FLVER2.GXItem>) this)
          gxItem.Write(bw, header);
        bw.WriteInt32(int.MaxValue);
        bw.WriteInt32(100);
        bw.WriteInt32(this.TerminatorLength + 12);
        bw.WritePattern(this.TerminatorLength, (byte) 0);
      }
    }

    public class GXItem {
      public string ID { get; set; }

      public int Unk04 { get; set; }

      public byte[] Data { get; set; }

      public GXItem() {
        this.ID = "0";
        this.Unk04 = 100;
        this.Data = new byte[0];
      }

      public GXItem(string id, int unk04, byte[] data) {
        this.ID = id;
        this.Unk04 = unk04;
        this.Data = data;
      }

      internal GXItem(BinaryReaderEx br, FLVER2.FLVERHeader header) {
        this.ID = header.Version > 131088
                      ? br.ReadFixStr(4)
                      : br.ReadInt32().ToString();
        this.Unk04 = br.ReadInt32();
        int num = br.ReadInt32();
        this.Data = br.ReadBytes(num - 12);
      }

      internal void Write(BinaryWriterEx bw, FLVER2.FLVERHeader header) {
        if (header.Version <= 131088) {
          int result;
          if (!int.TryParse(this.ID, out result))
            throw new FormatException(
                "For Dark Souls 2, GX IDs must be convertible to int.");
          bw.WriteInt32(result);
        } else
          bw.WriteFixStr(this.ID, 4, (byte) 0);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.Data.Length + 12);
        bw.WriteBytes(this.Data);
      }
    }

    public class Material : IFlverMaterial {
      private int textureIndex;
      private int textureCount;

      public string Name { get; set; }

      public string MTD { get; set; }

      public int Flags { get; set; }

      public List<FLVER2.Texture> Textures { get; set; }

      IReadOnlyList<IFlverTexture> IFlverMaterial.Textures {
        get { return (IReadOnlyList<IFlverTexture>) this.Textures; }
      }

      public int GXIndex { get; set; }

      public int Unk18 { get; set; }

      public Material() {
        this.Name = "";
        this.MTD = "";
        this.Textures = new List<FLVER2.Texture>();
        this.GXIndex = -1;
      }

      public Material(string name, string mtd, int flags) {
        this.Name = name;
        this.MTD = mtd;
        this.Flags = flags;
        this.Textures = new List<FLVER2.Texture>();
        this.GXIndex = -1;
        this.Unk18 = 0;
      }

      internal Material(
          BinaryReaderEx br,
          FLVER2.FLVERHeader header,
          List<FLVER2.GXList> gxLists,
          Dictionary<int, int> gxListIndices) {
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        this.textureCount = br.ReadInt32();
        this.textureIndex = br.ReadInt32();
        this.Flags = br.ReadInt32();
        int key = br.ReadInt32();
        this.Unk18 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        if (header.Unicode) {
          this.Name = br.GetUTF16((long) num1);
          this.MTD = br.GetUTF16((long) num2);
        } else {
          this.Name = br.GetShiftJIS((long) num1);
          this.MTD = br.GetShiftJIS((long) num2);
        }
        if (key == 0) {
          this.GXIndex = -1;
        } else {
          if (!gxListIndices.ContainsKey(key)) {
            br.StepIn((long) key);
            gxListIndices[key] = gxLists.Count;
            gxLists.Add(new FLVER2.GXList(br, header));
            br.StepOut();
          }
          this.GXIndex = gxListIndices[key];
        }
      }

      internal void TakeTextures(Dictionary<int, FLVER2.Texture> textureDict) {
        this.Textures = new List<FLVER2.Texture>(this.textureCount);
        for (int textureIndex = this.textureIndex;
             textureIndex < this.textureIndex + this.textureCount;
             ++textureIndex) {
          if (!textureDict.ContainsKey(textureIndex))
            throw new NotSupportedException(
                "Texture not found or already taken: " + (object) textureIndex);
          this.Textures.Add(textureDict[textureIndex]);
          textureDict.Remove(textureIndex);
        }
        this.textureIndex = -1;
        this.textureCount = -1;
      }

      internal void Write(BinaryWriterEx bw, int index) {
        bw.ReserveInt32(string.Format("MaterialName{0}", (object) index));
        bw.ReserveInt32(string.Format("MaterialMTD{0}", (object) index));
        bw.WriteInt32(this.Textures.Count);
        bw.ReserveInt32(string.Format("TextureIndex{0}", (object) index));
        bw.WriteInt32(this.Flags);
        bw.ReserveInt32(string.Format("GXOffset{0}", (object) index));
        bw.WriteInt32(this.Unk18);
        bw.WriteInt32(0);
      }

      internal void FillGXOffset(
          BinaryWriterEx bw,
          int index,
          List<int> gxOffsets) {
        if (this.GXIndex == -1)
          bw.FillInt32(string.Format("GXOffset{0}", (object) index), 0);
        else
          bw.FillInt32(string.Format("GXOffset{0}", (object) index),
                       gxOffsets[this.GXIndex]);
      }

      internal void WriteTextures(
          BinaryWriterEx bw,
          int index,
          int textureIndex) {
        bw.FillInt32(string.Format("TextureIndex{0}", (object) index),
                     textureIndex);
        for (int index1 = 0; index1 < this.Textures.Count; ++index1)
          this.Textures[index1].Write(bw, textureIndex + index1);
      }

      internal void WriteStrings(
          BinaryWriterEx bw,
          FLVER2.FLVERHeader header,
          int index) {
        bw.FillInt32(string.Format("MaterialName{0}", (object) index),
                     (int) bw.Position);
        if (header.Unicode)
          bw.WriteUTF16(this.Name, true);
        else
          bw.WriteShiftJIS(this.Name, true);
        bw.FillInt32(string.Format("MaterialMTD{0}", (object) index),
                     (int) bw.Position);
        if (header.Unicode)
          bw.WriteUTF16(this.MTD, true);
        else
          bw.WriteShiftJIS(this.MTD, true);
      }

      public override string ToString() {
        return this.Name + " | " + this.MTD;
      }
    }

    public class Mesh : IFlverMesh {
      private int[] faceSetIndices;
      private int[] vertexBufferIndices;

      public byte Dynamic { get; set; }

      public int MaterialIndex { get; set; }

      public int DefaultBoneIndex { get; set; }

      public List<int> BoneIndices { get; set; }

      public List<FLVER2.FaceSet> FaceSets { get; set; }

      public List<FLVER2.VertexBuffer> VertexBuffers { get; set; }

      public List<FLVER.Vertex> Vertices { get; set; }

      IReadOnlyList<FLVER.Vertex> IFlverMesh.Vertices {
        get { return (IReadOnlyList<FLVER.Vertex>) this.Vertices; }
      }

      public FLVER2.Mesh.BoundingBoxes BoundingBox { get; set; }

      public Mesh() {
        this.DefaultBoneIndex = -1;
        this.BoneIndices = new List<int>();
        this.FaceSets = new List<FLVER2.FaceSet>();
        this.VertexBuffers = new List<FLVER2.VertexBuffer>();
        this.Vertices = new List<FLVER.Vertex>();
      }

      internal Mesh(BinaryReaderEx br, FLVER2.FLVERHeader header) {
        this.Dynamic = br.AssertByte((byte) 0, (byte) 1);
        int num1 = (int) br.AssertByte(new byte[1]);
        int num2 = (int) br.AssertByte(new byte[1]);
        int num3 = (int) br.AssertByte(new byte[1]);
        this.MaterialIndex = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.DefaultBoneIndex = br.ReadInt32();
        int count1 = br.ReadInt32();
        int num4 = br.ReadInt32();
        int num5 = br.ReadInt32();
        int count2 = br.ReadInt32();
        int num6 = br.ReadInt32();
        int count3 = br.AssertInt32(1, 2, 3);
        int num7 = br.ReadInt32();
        if (num4 != 0) {
          br.StepIn((long) num4);
          this.BoundingBox = new FLVER2.Mesh.BoundingBoxes(br, header);
          br.StepOut();
        }
        this.BoneIndices =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num5, count1));
        this.faceSetIndices = br.GetInt32s((long) num6, count2);
        this.vertexBufferIndices = br.GetInt32s((long) num7, count3);
      }

      internal void TakeFaceSets(Dictionary<int, FLVER2.FaceSet> faceSetDict) {
        this.FaceSets = new List<FLVER2.FaceSet>(this.faceSetIndices.Length);
        foreach (int faceSetIndex in this.faceSetIndices) {
          if (!faceSetDict.ContainsKey(faceSetIndex))
            throw new NotSupportedException(
                "Face set not found or already taken: " +
                (object) faceSetIndex);
          this.FaceSets.Add(faceSetDict[faceSetIndex]);
          faceSetDict.Remove(faceSetIndex);
        }
        this.faceSetIndices = (int[]) null;
      }

      internal void TakeVertexBuffers(
          Dictionary<int, FLVER2.VertexBuffer> vertexBufferDict,
          List<FLVER2.BufferLayout> layouts) {
        this.VertexBuffers =
            new List<FLVER2.VertexBuffer>(this.vertexBufferIndices.Length);
        foreach (int vertexBufferIndex in this.vertexBufferIndices) {
          if (!vertexBufferDict.ContainsKey(vertexBufferIndex))
            throw new NotSupportedException(
                "Vertex buffer not found or already taken: " +
                (object) vertexBufferIndex);
          this.VertexBuffers.Add(vertexBufferDict[vertexBufferIndex]);
          vertexBufferDict.Remove(vertexBufferIndex);
        }
        this.vertexBufferIndices = (int[]) null;
        List<FLVER.LayoutSemantic> layoutSemanticList =
            new List<FLVER.LayoutSemantic>();
        foreach (FLVER2.VertexBuffer vertexBuffer in this.VertexBuffers) {
          foreach (FLVER.LayoutMember layoutMember in (List<FLVER.LayoutMember>)
              layouts[vertexBuffer.LayoutIndex]) {
            if (layoutMember.Semantic != FLVER.LayoutSemantic.UV &&
                layoutMember.Semantic != FLVER.LayoutSemantic.Tangent &&
                (layoutMember.Semantic != FLVER.LayoutSemantic.VertexColor &&
                 layoutMember.Semantic != FLVER.LayoutSemantic.Position) &&
                layoutMember.Semantic != FLVER.LayoutSemantic.Normal) {
              if (layoutSemanticList.Contains(layoutMember.Semantic))
                throw new NotImplementedException("Unexpected semantic list.");
              layoutSemanticList.Add(layoutMember.Semantic);
            }
          }
        }
        for (int index = 0; index < this.VertexBuffers.Count; ++index) {
          if (this.VertexBuffers[index].BufferIndex != index)
            throw new FormatException("Unexpected vertex buffer index.");
        }
      }

      internal void ReadVertices(
          BinaryReaderEx br,
          int dataOffset,
          List<FLVER2.BufferLayout> layouts,
          FLVER2.FLVERHeader header) {
        IEnumerable<FLVER.LayoutMember> source =
            layouts.SelectMany<FLVER2.BufferLayout, FLVER.LayoutMember>(
                (Func<FLVER2.BufferLayout, IEnumerable<FLVER.LayoutMember>>)
                (l => (IEnumerable<FLVER.LayoutMember>) l));
        int uvCapacity = source
                         .Where<FLVER.LayoutMember>(
                             (Func<FLVER.LayoutMember, bool>)
                             (m => m.Semantic == FLVER.LayoutSemantic.UV))
                         .Count<FLVER.LayoutMember>();
        int tangentCapacity = source
                              .Where<FLVER.LayoutMember>(
                                  (Func<FLVER.LayoutMember, bool>)
                                  (m => m.Semantic ==
                                        FLVER.LayoutSemantic.Tangent))
                              .Count<FLVER.LayoutMember>();
        int colorCapacity = source
                            .Where<FLVER.LayoutMember>(
                                (Func<FLVER.LayoutMember, bool>)
                                (m => m.Semantic ==
                                      FLVER.LayoutSemantic.VertexColor))
                            .Count<FLVER.LayoutMember>();
        int vertexCount = this.VertexBuffers[0].VertexCount;
        this.Vertices = new List<FLVER.Vertex>(vertexCount);
        for (int index = 0; index < vertexCount; ++index)
          this.Vertices.Add(
              new FLVER.Vertex(uvCapacity,
                               tangentCapacity,
                               colorCapacity));
        foreach (FLVER2.VertexBuffer vertexBuffer in this.VertexBuffers)
          vertexBuffer.ReadBuffer(br,
                                  layouts,
                                  this.Vertices,
                                  dataOffset,
                                  header);
      }

      internal void Write(BinaryWriterEx bw, int index) {
        bw.WriteByte(this.Dynamic);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 0);
        bw.WriteInt32(this.MaterialIndex);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(this.DefaultBoneIndex);
        bw.WriteInt32(this.BoneIndices.Count);
        bw.ReserveInt32(string.Format("MeshBoundingBox{0}", (object) index));
        bw.ReserveInt32(string.Format("MeshBoneIndices{0}", (object) index));
        bw.WriteInt32(this.FaceSets.Count);
        bw.ReserveInt32(string.Format("MeshFaceSetIndices{0}", (object) index));
        bw.WriteInt32(this.VertexBuffers.Count);
        bw.ReserveInt32(
            string.Format("MeshVertexBufferIndices{0}", (object) index));
      }

      internal void WriteBoundingBox(
          BinaryWriterEx bw,
          int index,
          FLVER2.FLVERHeader header) {
        if (this.BoundingBox == null) {
          bw.FillInt32(string.Format("MeshBoundingBox{0}", (object) index), 0);
        } else {
          bw.FillInt32(string.Format("MeshBoundingBox{0}", (object) index),
                       (int) bw.Position);
          this.BoundingBox.Write(bw, header);
        }
      }

      internal void WriteBoneIndices(
          BinaryWriterEx bw,
          int index,
          int boneIndicesStart) {
        if (this.BoneIndices.Count == 0) {
          bw.FillInt32(string.Format("MeshBoneIndices{0}", (object) index),
                       boneIndicesStart);
        } else {
          bw.FillInt32(string.Format("MeshBoneIndices{0}", (object) index),
                       (int) bw.Position);
          bw.WriteInt32s((IList<int>) this.BoneIndices);
        }
      }

      public List<FLVER.Vertex[]> GetFaces(
          FLVER2.FaceSet.FSFlags fsFlags = FLVER2.FaceSet.FSFlags.None) {
        if (this.FaceSets.Count == 0)
          return new List<FLVER.Vertex[]>();
        List<int> intList =
            (this.FaceSets.Find(
                 (Predicate<FLVER2.FaceSet>) (fs => fs.Flags ==
                                                    fsFlags)) ??
             this.FaceSets[0]).Triangulate(
                this.Vertices.Count < (int) ushort.MaxValue,
                false);
        List<FLVER.Vertex[]> vertexArrayList =
            new List<FLVER.Vertex[]>(intList.Count);
        for (int index1 = 0; index1 < intList.Count - 2; index1 += 3) {
          int index2 = intList[index1];
          int index3 = intList[index1 + 1];
          int index4 = intList[index1 + 2];
          vertexArrayList.Add(new FLVER.Vertex[3] {
              this.Vertices[index2],
              this.Vertices[index3],
              this.Vertices[index4]
          });
        }
        return vertexArrayList;
      }

      public class BoundingBoxes {
        public Vector3 Min { get; set; }

        public Vector3 Max { get; set; }

        public Vector3 Unk { get; set; }

        public BoundingBoxes() {
          this.Min = new Vector3(float.MinValue);
          this.Max = new Vector3(float.MaxValue);
        }

        internal BoundingBoxes(BinaryReaderEx br, FLVER2.FLVERHeader header) {
          this.Min = br.ReadVector3();
          this.Max = br.ReadVector3();
          if (header.Version < 131098)
            return;
          this.Unk = br.ReadVector3();
        }

        internal void Write(BinaryWriterEx bw, FLVER2.FLVERHeader header) {
          bw.WriteVector3(this.Min);
          bw.WriteVector3(this.Max);
          if (header.Version < 131098)
            return;
          bw.WriteVector3(this.Unk);
        }
      }
    }

    public class SekiroUnkStruct {
      public List<FLVER2.SekiroUnkStruct.Member> Members1 { get; set; }

      public List<FLVER2.SekiroUnkStruct.Member> Members2 { get; set; }

      public SekiroUnkStruct() {
        this.Members1 = new List<FLVER2.SekiroUnkStruct.Member>();
        this.Members2 = new List<FLVER2.SekiroUnkStruct.Member>();
      }

      internal SekiroUnkStruct(BinaryReaderEx br) {
        short num1 = br.ReadInt16();
        short num2 = br.ReadInt16();
        uint num3 = br.ReadUInt32();
        uint num4 = br.ReadUInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.StepIn((long) num3);
        this.Members1 = new List<FLVER2.SekiroUnkStruct.Member>((int) num1);
        for (int index = 0; index < (int) num1; ++index)
          this.Members1.Add(new FLVER2.SekiroUnkStruct.Member(br));
        br.StepOut();
        br.StepIn((long) num4);
        this.Members2 = new List<FLVER2.SekiroUnkStruct.Member>((int) num2);
        for (int index = 0; index < (int) num2; ++index)
          this.Members2.Add(new FLVER2.SekiroUnkStruct.Member(br));
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt16((short) this.Members1.Count);
        bw.WriteInt16((short) this.Members2.Count);
        bw.ReserveUInt32("SekiroUnkOffset1");
        bw.ReserveUInt32("SekiroUnkOffset2");
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.FillUInt32("SekiroUnkOffset1", (uint) bw.Position);
        foreach (FLVER2.SekiroUnkStruct.Member member in this.Members1)
          member.Write(bw);
        bw.FillUInt32("SekiroUnkOffset2", (uint) bw.Position);
        foreach (FLVER2.SekiroUnkStruct.Member member in this.Members2)
          member.Write(bw);
      }

      public class Member {
        public short[] Unk00 { get; private set; }

        public int Index { get; set; }

        public Member() {
          this.Unk00 = new short[4];
        }

        internal Member(BinaryReaderEx br) {
          this.Unk00 = br.ReadInt16s(4);
          this.Index = br.ReadInt32();
          br.AssertInt32(new int[1]);
        }

        internal void Write(BinaryWriterEx bw) {
          bw.WriteInt16s((IList<short>) this.Unk00);
          bw.WriteInt32(this.Index);
          bw.WriteInt32(0);
        }
      }
    }

    public class Texture : IFlverTexture {
      public string Type { get; set; }

      public string Path { get; set; }

      public Vector2 Scale { get; set; }

      public byte Unk10 { get; set; }

      public bool Unk11 { get; set; }

      public float Unk14 { get; set; }

      public float Unk18 { get; set; }

      public float Unk1C { get; set; }

      public Texture() {
        this.Type = "";
        this.Path = "";
        this.Scale = Vector2.One;
      }

      public Texture(
          string type,
          string path,
          Vector2 scale,
          byte unk10,
          bool unk11,
          int unk14,
          int unk18,
          float unk1C) {
        this.Type = type;
        this.Path = path;
        this.Scale = scale;
        this.Unk10 = unk10;
        this.Unk11 = unk11;
        this.Unk14 = (float) unk14;
        this.Unk18 = (float) unk18;
        this.Unk1C = unk1C;
      }

      internal Texture(BinaryReaderEx br, FLVER2.FLVERHeader header) {
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        this.Scale = br.ReadVector2();
        this.Unk10 = br.AssertByte((byte) 0, (byte) 1, (byte) 2);
        this.Unk11 = br.ReadBoolean();
        int num3 = (int) br.AssertByte(new byte[1]);
        int num4 = (int) br.AssertByte(new byte[1]);
        this.Unk14 = br.ReadSingle();
        this.Unk18 = br.ReadSingle();
        this.Unk1C = br.ReadSingle();
        if (header.Unicode) {
          this.Type = br.GetUTF16((long) num2);
          this.Path = br.GetUTF16((long) num1);
        } else {
          this.Type = br.GetShiftJIS((long) num2);
          this.Path = br.GetShiftJIS((long) num1);
        }
      }

      internal void Write(BinaryWriterEx bw, int index) {
        bw.ReserveInt32(string.Format("TexturePath{0}", (object) index));
        bw.ReserveInt32(string.Format("TextureType{0}", (object) index));
        bw.WriteVector2(this.Scale);
        bw.WriteByte(this.Unk10);
        bw.WriteBoolean(this.Unk11);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 0);
        bw.WriteSingle(this.Unk14);
        bw.WriteSingle(this.Unk18);
        bw.WriteSingle(this.Unk1C);
      }

      internal void WriteStrings(
          BinaryWriterEx bw,
          FLVER2.FLVERHeader header,
          int index) {
        bw.FillInt32(string.Format("TexturePath{0}", (object) index),
                     (int) bw.Position);
        if (header.Unicode)
          bw.WriteUTF16(this.Path, true);
        else
          bw.WriteShiftJIS(this.Path, true);
        bw.FillInt32(string.Format("TextureType{0}", (object) index),
                     (int) bw.Position);
        if (header.Unicode)
          bw.WriteUTF16(this.Type, true);
        else
          bw.WriteShiftJIS(this.Type, true);
      }

      public override string ToString() {
        return this.Type + " = " + this.Path;
      }
    }

    public class VertexBuffer {
      internal int VertexSize;
      internal int BufferIndex;
      internal int VertexCount;
      internal int BufferOffset;

      public int LayoutIndex { get; set; }

      public VertexBuffer(int layoutIndex) {
        this.LayoutIndex = layoutIndex;
      }

      internal VertexBuffer(BinaryReaderEx br) {
        this.BufferIndex = br.ReadInt32();
        this.LayoutIndex = br.ReadInt32();
        this.VertexSize = br.ReadInt32();
        this.VertexCount = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.ReadInt32();
        this.BufferOffset = br.ReadInt32();
      }

      internal void ReadBuffer(
          BinaryReaderEx br,
          List<FLVER2.BufferLayout> layouts,
          List<FLVER.Vertex> vertices,
          int dataOffset,
          FLVER2.FLVERHeader header) {
        FLVER2.BufferLayout layout = layouts[this.LayoutIndex];
        if (this.VertexSize != layout.Size)
          throw new InvalidDataException(
              "Mismatched vertex buffer and buffer layout sizes.");
        br.StepIn((long) (dataOffset + this.BufferOffset));
        float uvFactor = 1024f;
        if (header.Version >= 131087)
          uvFactor = 2048f;
        for (int index = 0; index < vertices.Count; ++index)
          vertices[index].Read(br, (List<FLVER.LayoutMember>) layout, uvFactor);
        br.StepOut();
        this.VertexSize = -1;
        this.BufferIndex = -1;
        this.VertexCount = -1;
        this.BufferOffset = -1;
      }

      internal void Write(
          BinaryWriterEx bw,
          FLVER2.FLVERHeader header,
          int index,
          int bufferIndex,
          List<FLVER2.BufferLayout> layouts,
          int vertexCount) {
        FLVER2.BufferLayout layout = layouts[this.LayoutIndex];
        bw.WriteInt32(bufferIndex);
        bw.WriteInt32(this.LayoutIndex);
        bw.WriteInt32(layout.Size);
        bw.WriteInt32(vertexCount);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(header.Version > 131077 ? layout.Size * vertexCount : 0);
        bw.ReserveInt32(string.Format("VertexBufferOffset{0}", (object) index));
      }

      internal void WriteBuffer(
          BinaryWriterEx bw,
          int index,
          List<FLVER2.BufferLayout> layouts,
          List<FLVER.Vertex> Vertices,
          int dataStart,
          FLVER2.FLVERHeader header) {
        FLVER2.BufferLayout layout = layouts[this.LayoutIndex];
        bw.FillInt32(string.Format("VertexBufferOffset{0}", (object) index),
                     (int) bw.Position - dataStart);
        float uvFactor = 1024f;
        if (header.Version >= 131087)
          uvFactor = 2048f;
        foreach (FLVER.Vertex vertex in Vertices)
          vertex.Write(bw, (List<FLVER.LayoutMember>) layout, uvFactor);
      }
    }
  }
}