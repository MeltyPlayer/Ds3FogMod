// Decompiled with JetBrains decompiler
// Type: SoulsFormats.KF1.MDO
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats.KF1
{
  [ComVisible(true)]
  public class MDO : SoulsFile<MDO>
  {
    public List<string> Textures;
    public List<MDO.Unk1> Unk1s;
    public List<MDO.Mesh> Meshes;

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      int capacity1 = br.ReadInt32();
      this.Textures = new List<string>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Textures.Add(br.ReadShiftJIS());
      br.Pad(4);
      int capacity2 = br.ReadInt32();
      this.Unk1s = new List<MDO.Unk1>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Unk1s.Add(new MDO.Unk1(br));
      for (int index = 0; index < 12; ++index)
        br.AssertInt32(new int[1]);
      int capacity3 = br.ReadInt32();
      this.Meshes = new List<MDO.Mesh>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.Meshes.Add(new MDO.Mesh(br));
    }

    public class Unk1
    {
      public float Unk00;
      public float Unk04;
      public float Unk08;
      public float Unk0C;
      public float Unk10;
      public float Unk14;
      public float Unk18;

      internal Unk1(BinaryReaderEx br)
      {
        this.Unk00 = br.ReadSingle();
        this.Unk04 = br.ReadSingle();
        this.Unk08 = br.ReadSingle();
        this.Unk0C = br.ReadSingle();
        this.Unk10 = br.ReadSingle();
        this.Unk14 = br.ReadSingle();
        this.Unk18 = br.ReadSingle();
        br.AssertInt32(new int[1]);
      }
    }

    public class Mesh
    {
      public int Unk00;
      public short TextureIndex;
      public short Unk06;
      public ushort[] Indices;
      public List<MDO.Vertex> Vertices;

      internal Mesh(BinaryReaderEx br)
      {
        this.Unk00 = br.ReadInt32();
        this.TextureIndex = br.ReadInt16();
        this.Unk06 = br.ReadInt16();
        ushort num1 = br.ReadUInt16();
        ushort num2 = br.ReadUInt16();
        uint num3 = br.ReadUInt32();
        uint num4 = br.ReadUInt32();
        this.Indices = br.GetUInt16s((long) num3, (int) num1);
        br.StepIn((long) num4);
        this.Vertices = new List<MDO.Vertex>((int) num2);
        for (int index = 0; index < (int) num2; ++index)
          this.Vertices.Add(new MDO.Vertex(br));
        br.StepOut();
      }

      public List<MDO.Vertex[]> GetFaces()
      {
        List<MDO.Vertex[]> vertexArrayList = new List<MDO.Vertex[]>();
        for (int index = 0; index < this.Indices.Length; index += 3)
          vertexArrayList.Add(new MDO.Vertex[3]
          {
            this.Vertices[(int) this.Indices[index]],
            this.Vertices[(int) this.Indices[index + 1]],
            this.Vertices[(int) this.Indices[index + 2]]
          });
        return vertexArrayList;
      }
    }

    public class Vertex
    {
      public Vector3 Position;
      public Vector3 Normal;
      public Vector2 UV;

      internal Vertex(BinaryReaderEx br)
      {
        this.Position = br.ReadVector3();
        this.Normal = br.ReadVector3();
        this.UV = br.ReadVector2();
      }
    }
  }
}
