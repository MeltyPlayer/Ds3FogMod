// Decompiled with JetBrains decompiler
// Type: SoulsFormats.NGP
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class NGP : SoulsFile<NGP>
  {
    public bool BigEndian { get; set; }

    public NGP.NGPVersion Version { get; set; }

    public int Unk1C { get; set; }

    public List<NGP.StructA> StructAs { get; set; }

    public List<NGP.StructB> StructBs { get; set; }

    public List<int> StructCs { get; set; }

    public List<short> StructDs { get; set; }

    public List<NGP.Mesh> Meshes { get; set; }

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "NVG2";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      this.BigEndian = br.GetInt16(4L) == (short) 256;
      br.BigEndian = this.BigEndian;
      br.AssertASCII("NVG2");
      this.Version = br.ReadEnum16<NGP.NGPVersion>();
      int num1 = (int) br.AssertInt16(new short[1]);
      int num2 = br.ReadInt32();
      int capacity1 = br.ReadInt32();
      int capacity2 = br.ReadInt32();
      int count1 = br.ReadInt32();
      int count2 = br.ReadInt32();
      this.Unk1C = br.ReadInt32();
      br.VarintLong = this.Version == NGP.NGPVersion.Scholar;
      long num3 = br.ReadVarint();
      long num4 = br.ReadVarint();
      long num5 = br.ReadVarint();
      long num6 = br.ReadVarint();
      long[] numArray = br.ReadVarints(num2);
      br.Position = num3;
      this.StructAs = new List<NGP.StructA>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.StructAs.Add(new NGP.StructA(br));
      br.Position = num4;
      this.StructBs = new List<NGP.StructB>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.StructBs.Add(new NGP.StructB(br));
      br.Position = num5;
      this.StructCs = new List<int>((IEnumerable<int>) br.ReadInt32s(count1));
      br.Position = num6;
      this.StructDs = new List<short>((IEnumerable<short>) br.ReadInt16s(count2));
      this.Meshes = new List<NGP.Mesh>(num2);
      for (int index = 0; index < num2; ++index)
      {
        br.Position = numArray[index];
        this.Meshes.Add(new NGP.Mesh(br, this.Version));
      }
    }

    protected override void Write(BinaryWriterEx bw)
    {
      void writeMeshes() {
        for (int index = 0; index < this.Meshes.Count; ++index) {
          bw.Pad(bw.VarintSize);
          bw.FillVarint(string.Format("MeshOffset{0}", (object)index), bw.Position);
          // ISSUE: reference to a compiler-generated field
          this.Meshes[index].Write(bw, this.Version);
        }
      }

      // ISSUE: reference to a compiler-generated field
      bw.BigEndian = this.BigEndian;
      // ISSUE: reference to a compiler-generated field
      bw.VarintLong = this.Version == NGP.NGPVersion.Scholar;
      // ISSUE: reference to a compiler-generated field
      bw.WriteASCII("NVG2", false);
      // ISSUE: reference to a compiler-generated field
      bw.WriteUInt16((ushort) this.Version);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt16((short) 0);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.Meshes.Count);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.StructAs.Count);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.StructBs.Count);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.StructCs.Count);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.StructDs.Count);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32(this.Unk1C);
      // ISSUE: reference to a compiler-generated field
      bw.ReserveVarint("OffsetA");
      // ISSUE: reference to a compiler-generated field
      bw.ReserveVarint("OffsetB");
      // ISSUE: reference to a compiler-generated field
      bw.ReserveVarint("OffsetC");
      // ISSUE: reference to a compiler-generated field
      bw.ReserveVarint("OffsetD");
      for (int index = 0; index < this.Meshes.Count; ++index)
      {
        // ISSUE: reference to a compiler-generated field
        bw.ReserveVarint(string.Format("MeshOffset{0}", (object) index));
      }
      if (this.Version == NGP.NGPVersion.Vanilla)
        writeMeshes();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.Pad(bw.VarintSize);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.FillVarint("OffsetA", bw.Position);
      foreach (NGP.StructA structA in this.StructAs)
      {
        // ISSUE: reference to a compiler-generated field
        structA.Write(bw);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.Pad(bw.VarintSize);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.FillVarint("OffsetB", bw.Position);
      foreach (NGP.StructB structB in this.StructBs)
      {
        // ISSUE: reference to a compiler-generated field
        structB.Write(bw);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.Pad(bw.VarintSize);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.FillVarint("OffsetC", bw.Position);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt32s((IList<int>) this.StructCs);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.Pad(bw.VarintSize);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bw.FillVarint("OffsetD", bw.Position);
      // ISSUE: reference to a compiler-generated field
      bw.WriteInt16s((IList<short>) this.StructDs);
      if (this.Version != NGP.NGPVersion.Scholar)
        return;
      writeMeshes();
    }

    public enum NGPVersion : ushort
    {
      Vanilla = 1,
      Scholar = 2,
    }

    public class StructA
    {
      public Vector3 Unk00 { get; set; }

      public float Unk0C { get; set; }

      public int Unk10 { get; set; }

      public short Unk14 { get; set; }

      public short Unk16 { get; set; }

      public short Unk18 { get; set; }

      public short Unk1A { get; set; }

      public short Unk1C { get; set; }

      public short Unk1E { get; set; }

      public short Unk20 { get; set; }

      public short Unk22 { get; set; }

      public StructA()
      {
      }

      internal StructA(BinaryReaderEx br)
      {
        this.Unk00 = br.ReadVector3();
        this.Unk0C = br.ReadSingle();
        this.Unk10 = br.ReadInt32();
        this.Unk14 = br.ReadInt16();
        this.Unk16 = br.ReadInt16();
        this.Unk18 = br.ReadInt16();
        this.Unk1A = br.ReadInt16();
        this.Unk1C = br.ReadInt16();
        this.Unk1E = br.ReadInt16();
        this.Unk20 = br.ReadInt16();
        this.Unk22 = br.ReadInt16();
      }

      internal void Write(BinaryWriterEx bw)
      {
        bw.WriteVector3(this.Unk00);
        bw.WriteSingle(this.Unk0C);
        bw.WriteInt32(this.Unk10);
        bw.WriteInt16(this.Unk14);
        bw.WriteInt16(this.Unk16);
        bw.WriteInt16(this.Unk18);
        bw.WriteInt16(this.Unk1A);
        bw.WriteInt16(this.Unk1C);
        bw.WriteInt16(this.Unk1E);
        bw.WriteInt16(this.Unk20);
        bw.WriteInt16(this.Unk22);
      }
    }

    public class StructB
    {
      public int Unk00 { get; set; }

      public int Unk04 { get; set; }

      public int Unk08 { get; set; }

      public StructB()
      {
      }

      internal StructB(BinaryReaderEx br)
      {
        this.Unk00 = br.ReadInt32();
        this.Unk04 = br.ReadInt32();
        this.Unk08 = br.ReadInt32();
      }

      internal void Write(BinaryWriterEx bw)
      {
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.Unk08);
      }
    }

    public class Mesh
    {
      public int Unk00 { get; set; }

      public int Unk08 { get; set; }

      public Vector3 BoundingBoxMin { get; set; }

      public Vector3 BoundingBoxMax { get; set; }

      public short Unk30 { get; set; }

      public short Unk32 { get; set; }

      public List<Vector3> Vertices { get; set; }

      public List<int> Struct2s { get; set; }

      public List<NGP.Face> Faces { get; set; }

      public List<NGP.Struct4> Struct4s { get; set; }

      public NGP.Struct5 Struct5Root { get; set; }

      public Mesh()
      {
        this.Vertices = new List<Vector3>();
        this.Struct2s = new List<int>();
        this.Faces = new List<NGP.Face>();
        this.Struct4s = new List<NGP.Struct4>();
        this.Struct5Root = new NGP.Struct5();
      }

      internal Mesh(BinaryReaderEx br, NGP.NGPVersion version)
      {
        this.Unk00 = br.ReadInt32();
        br.ReadInt32();
        this.Unk08 = br.ReadInt32();
        if (version == NGP.NGPVersion.Scholar)
          br.AssertInt32(new int[1]);
        this.BoundingBoxMin = br.ReadVector3();
        this.BoundingBoxMax = br.ReadVector3();
        int capacity = br.ReadInt32();
        short num1 = br.ReadInt16();
        short num2 = br.ReadInt16();
        this.Unk30 = br.ReadInt16();
        this.Unk32 = br.ReadInt16();
        int num3 = (int) br.AssertByte((byte) 1);
        int num4 = (int) br.AssertByte(new byte[1]);
        int num5 = (int) br.AssertByte(new byte[1]);
        int num6 = (int) br.AssertByte(new byte[1]);
        if (version == NGP.NGPVersion.Scholar)
          br.AssertInt64(new long[1]);
        long num7 = br.ReadVarint();
        long num8 = br.ReadVarint();
        long num9 = br.ReadVarint();
        long num10 = br.ReadVarint();
        long rootOffset = br.ReadVarint();
        long faceIndexOffset = br.ReadVarint();
        br.Position = num7;
        this.Vertices = new List<Vector3>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Vertices.Add(br.ReadVector3());
        br.Position = num8;
        this.Struct2s = new List<int>((IEnumerable<int>) br.ReadInt32s((int) num1));
        br.Position = num9;
        this.Faces = new List<NGP.Face>((int) num1);
        for (int index = 0; index < (int) num1; ++index)
          this.Faces.Add(new NGP.Face(br));
        br.Position = num10;
        this.Struct4s = new List<NGP.Struct4>((int) num2);
        for (int index = 0; index < (int) num2; ++index)
          this.Struct4s.Add(new NGP.Struct4(br));
        br.Position = rootOffset;
        this.Struct5Root = new NGP.Struct5(br, rootOffset, faceIndexOffset);
      }

      internal void Write(BinaryWriterEx bw, NGP.NGPVersion version)
      {
        long position = bw.Position;
        bw.WriteInt32(this.Unk00);
        bw.ReserveInt32("MeshLength");
        bw.WriteInt32(this.Unk08);
        if (version == NGP.NGPVersion.Scholar)
          bw.WriteInt32(0);
        bw.WriteVector3(this.BoundingBoxMin);
        bw.WriteVector3(this.BoundingBoxMax);
        bw.WriteInt32(this.Vertices.Count);
        bw.WriteInt16((short) this.Faces.Count);
        bw.WriteInt16((short) this.Struct4s.Count);
        bw.WriteInt16(this.Unk30);
        bw.WriteInt16(this.Unk32);
        bw.WriteByte((byte) 1);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 0);
        if (version == NGP.NGPVersion.Scholar)
          bw.WriteInt64(0L);
        bw.ReserveVarint("VerticesOffset");
        bw.ReserveVarint("Struct2sOffset");
        bw.ReserveVarint("FacesOffset");
        bw.ReserveVarint("Struct4sOffset");
        bw.ReserveVarint("Struct5sOffset");
        bw.ReserveVarint("Struct6sOffset");
        bw.FillVarint("VerticesOffset", bw.Position);
        foreach (Vector3 vertex in this.Vertices)
          bw.WriteVector3(vertex);
        bw.Pad(bw.VarintSize);
        bw.FillVarint("Struct2sOffset", bw.Position);
        foreach (int struct2 in this.Struct2s)
          bw.WriteInt32(struct2);
        bw.Pad(bw.VarintSize);
        bw.FillVarint("FacesOffset", bw.Position);
        foreach (NGP.Face face in this.Faces)
          face.Write(bw);
        bw.Pad(bw.VarintSize);
        bw.FillVarint("Struct4sOffset", bw.Position);
        foreach (NGP.Struct4 struct4 in this.Struct4s)
          struct4.Write(bw);
        bw.Pad(bw.VarintSize);
        bw.FillVarint("Struct5sOffset", bw.Position);
        short index1 = 0;
        this.Struct5Root.Write(bw, ref index1);
        bw.Pad(bw.VarintSize);
        bw.FillVarint("Struct6sOffset", bw.Position);
        short index2 = 0;
        int faceIndexIndex = 0;
        this.Struct5Root.WriteFaceIndices(bw, ref index2, ref faceIndexIndex);
        bw.Pad(bw.VarintSize);
        bw.FillInt32("MeshLength", (int) (bw.Position - position));
      }
    }

    public class Face
    {
      public short V1 { get; set; }

      public short V2 { get; set; }

      public short V3 { get; set; }

      public short Unk06 { get; set; }

      public short Unk08 { get; set; }

      public short Unk0A { get; set; }

      public Face()
      {
      }

      internal Face(BinaryReaderEx br)
      {
        this.V1 = br.ReadInt16();
        this.V2 = br.ReadInt16();
        this.V3 = br.ReadInt16();
        this.Unk06 = br.ReadInt16();
        this.Unk08 = br.ReadInt16();
        this.Unk0A = br.ReadInt16();
      }

      internal void Write(BinaryWriterEx bw)
      {
        bw.WriteInt16(this.V1);
        bw.WriteInt16(this.V2);
        bw.WriteInt16(this.V3);
        bw.WriteInt16(this.Unk06);
        bw.WriteInt16(this.Unk08);
        bw.WriteInt16(this.Unk0A);
      }
    }

    public class Struct4
    {
      public short Unk00 { get; set; }

      public short Unk02 { get; set; }

      public short Unk04 { get; set; }

      public short Unk06 { get; set; }

      public short Unk08 { get; set; }

      public short Unk0A { get; set; }

      public short Unk0C { get; set; }

      public short Unk0E { get; set; }

      public Struct4()
      {
      }

      internal Struct4(BinaryReaderEx br)
      {
        this.Unk00 = br.ReadInt16();
        this.Unk02 = br.ReadInt16();
        this.Unk04 = br.ReadInt16();
        this.Unk06 = br.ReadInt16();
        this.Unk08 = br.ReadInt16();
        this.Unk0A = br.ReadInt16();
        this.Unk0C = br.ReadInt16();
        this.Unk0E = br.ReadInt16();
      }

      internal void Write(BinaryWriterEx bw)
      {
        bw.WriteInt16(this.Unk00);
        bw.WriteInt16(this.Unk02);
        bw.WriteInt16(this.Unk04);
        bw.WriteInt16(this.Unk06);
        bw.WriteInt16(this.Unk08);
        bw.WriteInt16(this.Unk0A);
        bw.WriteInt16(this.Unk0C);
        bw.WriteInt16(this.Unk0E);
      }
    }

    public class Struct5
    {
      public float Unk00 { get; set; }

      public NGP.Struct5 Left { get; set; }

      public NGP.Struct5 Right { get; set; }

      public List<short> FaceIndices { get; set; }

      public Struct5()
      {
      }

      internal Struct5(BinaryReaderEx br, long rootOffset, long faceIndexOffset)
      {
        this.Unk00 = br.ReadSingle();
        short num1 = br.ReadInt16();
        short num2 = br.ReadInt16();
        short num3 = br.ReadInt16();
        short num4 = br.ReadInt16();
        if (num1 != (short) -1)
        {
          br.Position = rootOffset + (long) ((int) num1 * 12);
          this.Left = new NGP.Struct5(br, rootOffset, faceIndexOffset);
        }
        if (num2 != (short) -1)
        {
          br.Position = rootOffset + (long) ((int) num2 * 12);
          this.Right = new NGP.Struct5(br, rootOffset, faceIndexOffset);
        }
        if (num3 <= (short) 0)
          return;
        br.Position = faceIndexOffset + (long) ((int) num4 * 2);
        this.FaceIndices = new List<short>((IEnumerable<short>) br.ReadInt16s((int) num3));
      }

      internal void Write(BinaryWriterEx bw, ref short index)
      {
        short num = index;
        bw.WriteSingle(this.Unk00);
        bw.ReserveInt16(string.Format("LeftIndex{0}", (object) num));
        bw.ReserveInt16(string.Format("RightIndex{0}", (object) num));
        bw.ReserveInt16(string.Format("FaceIndexCount{0}", (object) num));
        bw.ReserveInt16(string.Format("FaceIndexIndex{0}", (object) num));
        if (this.Left == null)
        {
          bw.FillInt16(string.Format("LeftIndex{0}", (object) num), (short) -1);
        }
        else
        {
          ++index;
          bw.FillInt16(string.Format("LeftIndex{0}", (object) num), index);
          this.Left.Write(bw, ref index);
        }
        if (this.Right == null)
        {
          bw.FillInt16(string.Format("RightIndex{0}", (object) num), (short) -1);
        }
        else
        {
          ++index;
          bw.FillInt16(string.Format("RightIndex{0}", (object) num), index);
          this.Right.Write(bw, ref index);
        }
      }

      internal void WriteFaceIndices(BinaryWriterEx bw, ref short index, ref int faceIndexIndex)
      {
        short num = index;
        if (this.FaceIndices == null)
        {
          bw.FillInt16(string.Format("FaceIndexCount{0}", (object) num), (short) 0);
          bw.FillInt16(string.Format("FaceIndexIndex{0}", (object) num), (short) 0);
        }
        else
        {
          bw.FillInt16(string.Format("FaceIndexCount{0}", (object) num), (short) this.FaceIndices.Count);
          bw.FillInt16(string.Format("FaceIndexIndex{0}", (object) num), (short) faceIndexIndex);
          bw.WriteInt16s((IList<short>) this.FaceIndices);
          faceIndexIndex += this.FaceIndices.Count;
        }
        if (this.Left != null)
        {
          ++index;
          this.Left.WriteFaceIndices(bw, ref index, ref faceIndexIndex);
        }
        if (this.Right == null)
          return;
        ++index;
        this.Right.WriteFaceIndices(bw, ref index, ref faceIndexIndex);
      }
    }
  }
}
