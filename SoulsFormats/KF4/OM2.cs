// Decompiled with JetBrains decompiler
// Type: SoulsFormats.KF4.OM2
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats.KF4 {
  [ComVisible(true)]
  public class OM2 : SoulsFile<OM2> {
    public OM2.Struct1[] Struct1s { get; private set; }

    public List<OM2.Struct2> Struct2s { get; set; }

    protected override void Read(BinaryReaderEx br) {
      br.ReadInt32();
      short num1 = br.ReadInt16();
      int num2 = (int) br.ReadInt16();
      int num3 = (int) br.ReadInt16();
      int num4 = (int) br.ReadInt16();
      br.AssertInt32(new int[1]);
      this.Struct1s = new OM2.Struct1[32];
      for (int index = 0; index < 32; ++index)
        this.Struct1s[index] = new OM2.Struct1(br);
      this.Struct2s = new List<OM2.Struct2>((int) num1);
      for (int index = 0; index < (int) num1; ++index)
        this.Struct2s.Add(new OM2.Struct2(br));
    }

    public class Struct1 {
      public float Unk00 { get; set; }

      public float Unk04 { get; set; }

      public float Unk08 { get; set; }

      public byte Unk0C { get; set; }

      internal Struct1(BinaryReaderEx br) {
        this.Unk00 = br.ReadSingle();
        this.Unk04 = br.ReadSingle();
        this.Unk08 = br.ReadSingle();
        this.Unk0C = br.ReadByte();
        int num1 = (int) br.AssertByte(new byte[1]);
        int num2 = (int) br.AssertByte(new byte[1]);
        int num3 = (int) br.AssertByte(new byte[1]);
      }
    }

    public class Struct2 {
      public List<OM2.Mesh> Meshes { get; set; }

      public byte Unk05 { get; set; }

      public byte Struct2Index { get; set; }

      internal Struct2(BinaryReaderEx br) {
        int num1 = br.ReadInt32();
        short num2 = br.ReadInt16();
        this.Unk05 = br.ReadByte();
        this.Struct2Index = br.ReadByte();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.StepIn((long) num1);
        this.Meshes = new List<OM2.Mesh>((int) num2);
        for (int index = 0; index < (int) num2; ++index)
          this.Meshes.Add(new OM2.Mesh(br));
        br.StepOut();
      }
    }

    public class Mesh {
      public List<OM2.Vertex> Vertices { get; set; }

      internal Mesh(BinaryReaderEx br) {
        br.Skip(160);
        byte num = br.ReadByte();
        br.Skip(15);
        this.Vertices = new List<OM2.Vertex>((int) num);
        for (int index = 0; index < (int) num; ++index)
          this.Vertices.Add(new OM2.Vertex(br));
        br.Skip(16);
      }
    }

    public class Vertex {
      public Vector3 Position { get; set; }

      public float Unk0C { get; set; }

      public Vector3 Normal { get; set; }

      public int Unk1C { get; set; }

      public Vector3 Unk20 { get; set; }

      public int Unk2C { get; set; }

      public Vector4 Unk30 { get; set; }

      internal Vertex(BinaryReaderEx br) {
        this.Position = br.ReadVector3();
        this.Unk0C = br.ReadSingle();
        this.Normal = br.ReadVector3();
        this.Unk1C = br.ReadInt32();
        this.Unk20 = br.ReadVector3();
        this.Unk2C = br.ReadInt32();
        this.Unk30 = br.ReadVector4();
      }
    }
  }
}