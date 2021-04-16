// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BTL
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class BTL : SoulsFile<BTL> {
    public int Version { get; set; }

    public bool LongOffsets { get; set; }

    public List<BTL.Light> Lights { get; set; }

    public BTL() {
      this.Version = 16;
      this.LongOffsets = true;
      this.Lights = new List<BTL.Light>();
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertInt32(2);
      this.Version = br.AssertInt32(1, 2, 5, 6, 16);
      int capacity = br.ReadInt32();
      int count = br.ReadInt32();
      br.AssertInt32(new int[1]);
      this.LongOffsets = br.AssertInt32(192, 200, 232) != 192;
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      long position = br.Position;
      br.Skip(count);
      this.Lights = new List<BTL.Light>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Lights.Add(
            new BTL.Light(br, position, this.Version, this.LongOffsets));
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = false;
      bw.WriteInt32(2);
      bw.WriteInt32(this.Version);
      bw.WriteInt32(this.Lights.Count);
      bw.ReserveInt32("NamesLength");
      bw.WriteInt32(0);
      bw.WriteInt32(this.Version == 16 ? 232 : (this.LongOffsets ? 200 : 192));
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      long position = bw.Position;
      List<long> longList = new List<long>(this.Lights.Count);
      foreach (BTL.Light light in this.Lights) {
        long num = bw.Position - position;
        longList.Add(num);
        bw.WriteUTF16(light.Name, true);
        if (num % 16L != 0L)
          bw.WritePattern((int) (16L - num % 16L), (byte) 0);
      }
      bw.FillInt32("NamesLength", (int) (bw.Position - position));
      for (int index = 0; index < this.Lights.Count; ++index)
        this.Lights[index]
            .Write(bw, longList[index], this.Version, this.LongOffsets);
    }

    public enum LightType : uint {
      Point,
      Spot,
      Directional,
    }

    public class Light {
      public uint Unk00 { get; set; }

      public uint Unk04 { get; set; }

      public uint Unk08 { get; set; }

      public uint Unk0C { get; set; }

      public string Name { get; set; }

      public BTL.LightType Type { get; set; }

      public bool Unk1C { get; set; }

      [SupportsAlpha(false)] public Color DiffuseColor { get; set; }

      public float DiffusePower { get; set; }

      [SupportsAlpha(false)] public Color SpecularColor { get; set; }

      public bool CastShadows { get; set; }

      public float SpecularPower { get; set; }

      public float ConeAngle { get; set; }

      public float Unk30 { get; set; }

      public float Unk34 { get; set; }

      public Vector3 Position { get; set; }

      public Vector3 Rotation { get; set; }

      public int Unk50 { get; set; }

      public float Unk54 { get; set; }

      public float Radius { get; set; }

      public int Unk5C { get; set; }

      public byte[] Unk64 { get; set; }

      public float Unk68 { get; set; }

      [SupportsAlpha(true)] public Color ShadowColor { get; set; }

      public float Unk70 { get; set; }

      public float FlickerIntervalMin { get; set; }

      public float FlickerIntervalMax { get; set; }

      public float FlickerBrightnessMult { get; set; }

      public int Unk80 { get; set; }

      public byte[] Unk84 { get; set; }

      public float Unk88 { get; set; }

      public float Unk90 { get; set; }

      public float Unk98 { get; set; }

      public float NearClip { get; set; }

      public byte[] UnkA0 { get; set; }

      public float Sharpness { get; set; }

      public float UnkAC { get; set; }

      public float Width { get; set; }

      public float UnkBC { get; set; }

      public byte[] UnkC0 { get; set; }

      public float UnkC4 { get; set; }

      public float UnkC8 { get; set; }

      public float UnkCC { get; set; }

      public float UnkD0 { get; set; }

      public float UnkD4 { get; set; }

      public float UnkD8 { get; set; }

      public int UnkDC { get; set; }

      public float UnkE0 { get; set; }

      public Light() {
        this.Name = "";
        this.Unk1C = true;
        this.DiffuseColor = Color.White;
        this.DiffusePower = 1f;
        this.SpecularColor = Color.White;
        this.SpecularPower = 1f;
        this.Unk50 = 4;
        this.Radius = 10f;
        this.Unk5C = -1;
        this.Unk64 = new byte[4] {
            (byte) 0,
            (byte) 0,
            (byte) 0,
            (byte) 1
        };
        this.ShadowColor = Color.FromArgb(100, 0, 0, 0);
        this.FlickerBrightnessMult = 1f;
        this.Unk80 = -1;
        this.Unk84 = new byte[4];
        this.Unk98 = 1f;
        this.NearClip = 1f;
        this.UnkA0 = new byte[4] {
            (byte) 1,
            (byte) 0,
            (byte) 2,
            (byte) 1
        };
        this.Sharpness = 1f;
        this.UnkC0 = new byte[4];
      }

      public BTL.Light Clone() {
        BTL.Light light = (BTL.Light) this.MemberwiseClone();
        light.Unk64 = (byte[]) this.Unk64.Clone();
        light.Unk84 = (byte[]) this.Unk84.Clone();
        light.UnkA0 = (byte[]) this.UnkA0.Clone();
        light.UnkC0 = (byte[]) this.UnkC0.Clone();
        return light;
      }

      internal Light(
          BinaryReaderEx br,
          long namesStart,
          int version,
          bool longOffsets) {
        this.Unk00 = br.ReadUInt32();
        this.Unk04 = br.ReadUInt32();
        this.Unk08 = br.ReadUInt32();
        this.Unk0C = br.ReadUInt32();
        long num = !longOffsets ? (long) br.ReadInt32() : br.ReadInt64();
        this.Name = br.GetUTF16(namesStart + num);
        this.Type = br.ReadEnum32<BTL.LightType>();
        this.Unk1C = br.ReadBoolean();
        byte[] numArray1 = br.ReadBytes(3);
        this.DiffuseColor = Color.FromArgb((int) byte.MaxValue,
                                           (int) numArray1[0],
                                           (int) numArray1[1],
                                           (int) numArray1[2]);
        this.DiffusePower = br.ReadSingle();
        byte[] numArray2 = br.ReadBytes(3);
        this.SpecularColor = Color.FromArgb((int) byte.MaxValue,
                                            (int) numArray2[0],
                                            (int) numArray2[1],
                                            (int) numArray2[2]);
        this.CastShadows = br.ReadBoolean();
        this.SpecularPower = br.ReadSingle();
        this.ConeAngle = br.ReadSingle();
        this.Unk30 = br.ReadSingle();
        this.Unk34 = br.ReadSingle();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Unk50 = br.ReadInt32();
        this.Unk54 = br.ReadSingle();
        this.Radius = br.ReadSingle();
        this.Unk5C = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Unk64 = br.ReadBytes(4);
        this.Unk68 = br.ReadSingle();
        byte[] numArray3 = br.ReadBytes(4);
        this.ShadowColor = Color.FromArgb((int) numArray3[3],
                                          (int) numArray3[0],
                                          (int) numArray3[1],
                                          (int) numArray3[2]);
        this.Unk70 = br.ReadSingle();
        this.FlickerIntervalMin = br.ReadSingle();
        this.FlickerIntervalMax = br.ReadSingle();
        this.FlickerBrightnessMult = br.ReadSingle();
        this.Unk80 = br.ReadInt32();
        this.Unk84 = br.ReadBytes(4);
        this.Unk88 = br.ReadSingle();
        br.AssertInt32(new int[1]);
        this.Unk90 = br.ReadSingle();
        br.AssertInt32(new int[1]);
        this.Unk98 = br.ReadSingle();
        this.NearClip = br.ReadSingle();
        if (version == 2 && !longOffsets) {
          br.AssertPattern(36, (byte) 0);
          this.UnkA0 = new byte[4];
          this.UnkC0 = new byte[4];
        } else {
          this.UnkA0 = br.ReadBytes(4);
          this.Sharpness = br.ReadSingle();
          br.AssertInt32(new int[1]);
          this.UnkAC = br.ReadSingle();
          br.AssertInt64(new long[1]);
          this.Width = br.ReadSingle();
          this.UnkBC = br.ReadSingle();
          this.UnkC0 = br.ReadBytes(4);
          this.UnkC4 = br.ReadSingle();
        }
        if (version < 16)
          return;
        this.UnkC8 = br.ReadSingle();
        this.UnkCC = br.ReadSingle();
        this.UnkD0 = br.ReadSingle();
        this.UnkD4 = br.ReadSingle();
        this.UnkD8 = br.ReadSingle();
        this.UnkDC = br.ReadInt32();
        this.UnkE0 = br.ReadSingle();
        br.AssertInt32(new int[1]);
      }

      internal void Write(
          BinaryWriterEx bw,
          long nameOffset,
          int version,
          bool longOffsets) {
        bw.WriteUInt32(this.Unk00);
        bw.WriteUInt32(this.Unk04);
        bw.WriteUInt32(this.Unk08);
        bw.WriteUInt32(this.Unk0C);
        if (longOffsets)
          bw.WriteInt64(nameOffset);
        else
          bw.WriteInt32((int) nameOffset);
        bw.WriteUInt32((uint) this.Type);
        bw.WriteBoolean(this.Unk1C);
        bw.WriteByte(this.DiffuseColor.R);
        bw.WriteByte(this.DiffuseColor.G);
        bw.WriteByte(this.DiffuseColor.B);
        bw.WriteSingle(this.DiffusePower);
        bw.WriteByte(this.SpecularColor.R);
        bw.WriteByte(this.SpecularColor.G);
        bw.WriteByte(this.SpecularColor.B);
        bw.WriteBoolean(this.CastShadows);
        bw.WriteSingle(this.SpecularPower);
        bw.WriteSingle(this.ConeAngle);
        bw.WriteSingle(this.Unk30);
        bw.WriteSingle(this.Unk34);
        bw.WriteVector3(this.Position);
        bw.WriteVector3(this.Rotation);
        bw.WriteInt32(this.Unk50);
        bw.WriteSingle(this.Unk54);
        bw.WriteSingle(this.Radius);
        bw.WriteInt32(this.Unk5C);
        bw.WriteInt32(0);
        bw.WriteBytes(this.Unk64);
        bw.WriteSingle(this.Unk68);
        bw.WriteByte(this.ShadowColor.R);
        bw.WriteByte(this.ShadowColor.G);
        bw.WriteByte(this.ShadowColor.B);
        bw.WriteByte(this.ShadowColor.A);
        bw.WriteSingle(this.Unk70);
        bw.WriteSingle(this.FlickerIntervalMin);
        bw.WriteSingle(this.FlickerIntervalMax);
        bw.WriteSingle(this.FlickerBrightnessMult);
        bw.WriteInt32(this.Unk80);
        bw.WriteBytes(this.Unk84);
        bw.WriteSingle(this.Unk88);
        bw.WriteInt32(0);
        bw.WriteSingle(this.Unk90);
        bw.WriteInt32(0);
        bw.WriteSingle(this.Unk98);
        bw.WriteSingle(this.NearClip);
        bw.WriteBytes(this.UnkA0);
        bw.WriteSingle(this.Sharpness);
        bw.WriteInt32(0);
        bw.WriteSingle(this.UnkAC);
        if (longOffsets)
          bw.WriteInt64(0L);
        else
          bw.WriteInt32(0);
        bw.WriteSingle(this.Width);
        bw.WriteSingle(this.UnkBC);
        bw.WriteBytes(this.UnkC0);
        bw.WriteSingle(this.UnkC4);
        if (version < 16)
          return;
        bw.WriteSingle(this.UnkC8);
        bw.WriteSingle(this.UnkCC);
        bw.WriteSingle(this.UnkD0);
        bw.WriteSingle(this.UnkD4);
        bw.WriteSingle(this.UnkD8);
        bw.WriteInt32(this.UnkDC);
        bw.WriteSingle(this.UnkE0);
        bw.WriteInt32(0);
      }

      public override string ToString() {
        return this.Name;
      }
    }
  }
}