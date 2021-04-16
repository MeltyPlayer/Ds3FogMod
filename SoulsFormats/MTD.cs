// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MTD
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class MTD : SoulsFile<MTD>
  {
    public string ShaderPath { get; set; }

    public string Description { get; set; }

    public List<MTD.Param> Params { get; set; }

    public List<MTD.Texture> Textures { get; set; }

    public MTD()
    {
      this.ShaderPath = "Unknown.spx";
      this.Description = "";
      this.Params = new List<MTD.Param>();
      this.Textures = new List<MTD.Texture>();
    }

    protected override bool Is(BinaryReaderEx br)
    {
      return br.Length >= 48L && br.GetASCII(44L, 4) == "MTD ";
    }

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      MTD.Block.Read(br, new int?(0), new int?(3), new byte?((byte) 1));
      MTD.Block.Read(br, new int?(1), new int?(2), new byte?((byte) 176));
      MTD.AssertMarkedString(br, (byte) 52, "MTD ");
      br.AssertInt32(1000);
      int num1 = (int) MTD.AssertMarker(br, (byte) 1);
      MTD.Block.Read(br, new int?(2), new int?(4), new byte?((byte) 163));
      this.ShaderPath = MTD.ReadMarkedString(br, (byte) 163);
      this.Description = MTD.ReadMarkedString(br, (byte) 3);
      br.AssertInt32(1);
      MTD.Block.Read(br, new int?(3), new int?(4), new byte?((byte) 163));
      br.AssertInt32(new int[1]);
      int num2 = (int) MTD.AssertMarker(br, (byte) 3);
      int capacity1 = br.ReadInt32();
      this.Params = new List<MTD.Param>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.Params.Add(new MTD.Param(br));
      int num3 = (int) MTD.AssertMarker(br, (byte) 3);
      int capacity2 = br.ReadInt32();
      this.Textures = new List<MTD.Texture>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.Textures.Add(new MTD.Texture(br));
      int num4 = (int) MTD.AssertMarker(br, (byte) 4);
      br.AssertInt32(new int[1]);
      int num5 = (int) MTD.AssertMarker(br, (byte) 4);
      br.AssertInt32(new int[1]);
      int num6 = (int) MTD.AssertMarker(br, (byte) 4);
      br.AssertInt32(new int[1]);
    }

    protected override void Write(BinaryWriterEx bw)
    {
      bw.BigEndian = false;
      MTD.Block block1 = MTD.Block.Write(bw, 0, 3, (byte) 1);
      MTD.Block block2 = MTD.Block.Write(bw, 1, 2, (byte) 176);
      MTD.WriteMarkedString(bw, (byte) 52, "MTD ");
      bw.WriteInt32(1000);
      BinaryWriterEx bw1 = bw;
      block2.Finish(bw1);
      MTD.WriteMarker(bw, (byte) 1);
      MTD.Block block3 = MTD.Block.Write(bw, 2, 4, (byte) 163);
      MTD.WriteMarkedString(bw, (byte) 163, this.ShaderPath);
      MTD.WriteMarkedString(bw, (byte) 3, this.Description);
      bw.WriteInt32(1);
      MTD.Block block4 = MTD.Block.Write(bw, 3, 4, (byte) 163);
      bw.WriteInt32(0);
      MTD.WriteMarker(bw, (byte) 3);
      bw.WriteInt32(this.Params.Count);
      foreach (MTD.Param obj in this.Params)
        obj.Write(bw);
      MTD.WriteMarker(bw, (byte) 3);
      bw.WriteInt32(this.Textures.Count);
      foreach (MTD.Texture texture in this.Textures)
        texture.Write(bw);
      MTD.WriteMarker(bw, (byte) 4);
      bw.WriteInt32(0);
      block4.Finish(bw);
      MTD.WriteMarker(bw, (byte) 4);
      bw.WriteInt32(0);
      block3.Finish(bw);
      MTD.WriteMarker(bw, (byte) 4);
      bw.WriteInt32(0);
      block1.Finish(bw);
    }

    private static byte ReadMarker(BinaryReaderEx br)
    {
      int num = (int) br.ReadByte();
      br.Pad(4);
      return (byte) num;
    }

    private static byte AssertMarker(BinaryReaderEx br, byte marker)
    {
      int num = (int) br.AssertByte(marker);
      br.Pad(4);
      return marker;
    }

    private static void WriteMarker(BinaryWriterEx bw, byte marker)
    {
      bw.WriteByte(marker);
      bw.Pad(4);
    }

    private static string ReadMarkedString(BinaryReaderEx br, byte marker)
    {
      int length = br.ReadInt32();
      string str = br.ReadShiftJIS(length);
      int num = (int) MTD.AssertMarker(br, marker);
      return str;
    }

    private static string AssertMarkedString(BinaryReaderEx br, byte marker, string assert)
    {
      string str = MTD.ReadMarkedString(br, marker);
      if (str != assert)
        throw new InvalidDataException(string.Format("Read marked string: {0} | Expected: {1} | Ending position: 0x{2:X}", (object) str, (object) assert, (object) br.Position));
      return str;
    }

    private static void WriteMarkedString(BinaryWriterEx bw, byte marker, string str)
    {
      byte[] bytes = SFEncoding.ShiftJIS.GetBytes(str);
      bw.WriteInt32(bytes.Length);
      bw.WriteBytes(bytes);
      MTD.WriteMarker(bw, marker);
    }

    public class Param
    {
      public string Name { get; set; }

      public MTD.ParamType Type { get; }

      public object Value { get; set; }

      public Param(string name, MTD.ParamType type, object value = null)
      {
        this.Name = name;
        this.Type = type;
        this.Value = value;
        if (this.Value != null)
          return;
        switch (type)
        {
          case MTD.ParamType.Bool:
            this.Value = (object) false;
            break;
          case MTD.ParamType.Int:
            this.Value = (object) 0;
            break;
          case MTD.ParamType.Int2:
            this.Value = (object) new int[2];
            break;
          case MTD.ParamType.Float:
            this.Value = (object) 0.0f;
            break;
          case MTD.ParamType.Float2:
            this.Value = (object) new float[2];
            break;
          case MTD.ParamType.Float3:
            this.Value = (object) new float[3];
            break;
          case MTD.ParamType.Float4:
            this.Value = (object) new float[4];
            break;
        }
      }

      internal Param(BinaryReaderEx br)
      {
        MTD.Block.Read(br, new int?(4), new int?(4), new byte?((byte) 163));
        this.Name = MTD.ReadMarkedString(br, (byte) 163);
        this.Type = (MTD.ParamType) System.Enum.Parse(typeof (MTD.ParamType), MTD.ReadMarkedString(br, (byte) 4), true);
        br.AssertInt32(1);
        MTD.Block.Read(br, new int?(), new int?(1), new byte?());
        br.ReadInt32();
        if (this.Type == MTD.ParamType.Int)
          this.Value = (object) br.ReadInt32();
        else if (this.Type == MTD.ParamType.Int2)
          this.Value = (object) br.ReadInt32s(2);
        else if (this.Type == MTD.ParamType.Bool)
          this.Value = (object) br.ReadBoolean();
        else if (this.Type == MTD.ParamType.Float)
          this.Value = (object) br.ReadSingle();
        else if (this.Type == MTD.ParamType.Float2)
          this.Value = (object) br.ReadSingles(2);
        else if (this.Type == MTD.ParamType.Float3)
          this.Value = (object) br.ReadSingles(3);
        else if (this.Type == MTD.ParamType.Float4)
          this.Value = (object) br.ReadSingles(4);
        int num = (int) MTD.AssertMarker(br, (byte) 4);
        br.AssertInt32(new int[1]);
      }

      internal void Write(BinaryWriterEx bw)
      {
        MTD.Block block1 = MTD.Block.Write(bw, 4, 4, (byte) 163);
        MTD.WriteMarkedString(bw, (byte) 163, this.Name);
        MTD.WriteMarkedString(bw, (byte) 4, this.Type.ToString().ToLower());
        bw.WriteInt32(1);
        int type = -1;
        byte marker = byte.MaxValue;
        if (this.Type == MTD.ParamType.Bool)
        {
          type = 4096;
          marker = (byte) 192;
        }
        else if (this.Type == MTD.ParamType.Int || this.Type == MTD.ParamType.Int2)
        {
          type = 4097;
          marker = (byte) 197;
        }
        else if (this.Type == MTD.ParamType.Float || this.Type == MTD.ParamType.Float2 || (this.Type == MTD.ParamType.Float3 || this.Type == MTD.ParamType.Float4))
        {
          type = 4098;
          marker = (byte) 202;
        }
        MTD.Block block2 = MTD.Block.Write(bw, type, 1, marker);
        int num = -1;
        if (this.Type == MTD.ParamType.Bool || this.Type == MTD.ParamType.Int || this.Type == MTD.ParamType.Float)
          num = 1;
        else if (this.Type == MTD.ParamType.Int2 || this.Type == MTD.ParamType.Float2)
          num = 2;
        else if (this.Type == MTD.ParamType.Float3)
          num = 3;
        else if (this.Type == MTD.ParamType.Float4)
          num = 4;
        bw.WriteInt32(num);
        if (this.Type == MTD.ParamType.Int)
          bw.WriteInt32((int) this.Value);
        else if (this.Type == MTD.ParamType.Int2)
          bw.WriteInt32s((IList<int>) (int[]) this.Value);
        else if (this.Type == MTD.ParamType.Bool)
          bw.WriteBoolean((bool) this.Value);
        else if (this.Type == MTD.ParamType.Float)
          bw.WriteSingle((float) this.Value);
        else if (this.Type == MTD.ParamType.Float2)
          bw.WriteSingles((IList<float>) (float[]) this.Value);
        else if (this.Type == MTD.ParamType.Float3)
          bw.WriteSingles((IList<float>) (float[]) this.Value);
        else if (this.Type == MTD.ParamType.Float4)
          bw.WriteSingles((IList<float>) (float[]) this.Value);
        BinaryWriterEx bw1 = bw;
        block2.Finish(bw1);
        MTD.WriteMarker(bw, (byte) 4);
        bw.WriteInt32(0);
        BinaryWriterEx bw2 = bw;
        block1.Finish(bw2);
      }

      public override string ToString()
      {
        if (this.Type == MTD.ParamType.Float2 || this.Type == MTD.ParamType.Float3 || this.Type == MTD.ParamType.Float4)
          return this.Name + " = {" + string.Join<float>(", ", (IEnumerable<float>) (float[]) this.Value) + "}";
        return this.Type == MTD.ParamType.Int2 ? this.Name + " = {" + string.Join<int>(", ", (IEnumerable<int>) (int[]) this.Value) + "}" : string.Format("{0} = {1}", (object) this.Name, this.Value);
      }
    }

    public enum ParamType
    {
      Bool,
      Int,
      Int2,
      Float,
      Float2,
      Float3,
      Float4,
    }

    public class Texture
    {
      public string Type { get; set; }

      public bool Extended { get; set; }

      public int UVNumber { get; set; }

      public int ShaderDataIndex { get; set; }

      public string Path { get; set; }

      public List<float> UnkFloats { get; set; }

      public Texture()
      {
        this.Type = "g_DiffuseTexture";
        this.Path = "";
        this.UnkFloats = new List<float>();
      }

      internal Texture(BinaryReaderEx br)
      {
        MTD.Block block = MTD.Block.Read(br, new int?(8192), new int?(), new byte?((byte) 163));
        if (block.Version == 3)
        {
          this.Extended = false;
        }
        else
        {
          if (block.Version != 5)
            throw new InvalidDataException(string.Format("Texture block version is expected to be 3 or 5, but it was {0}.", (object) block.Version));
          this.Extended = true;
        }
        this.Type = MTD.ReadMarkedString(br, (byte) 53);
        this.UVNumber = br.ReadInt32();
        int num = (int) MTD.AssertMarker(br, (byte) 53);
        this.ShaderDataIndex = br.ReadInt32();
        if (this.Extended)
        {
          br.AssertInt32(163);
          this.Path = MTD.ReadMarkedString(br, (byte) 186);
          int count = br.ReadInt32();
          this.UnkFloats = new List<float>((IEnumerable<float>) br.ReadSingles(count));
        }
        else
        {
          this.Path = "";
          this.UnkFloats = new List<float>();
        }
      }

      internal void Write(BinaryWriterEx bw)
      {
        MTD.Block block = MTD.Block.Write(bw, 8192, this.Extended ? 5 : 3, (byte) 163);
        MTD.WriteMarkedString(bw, (byte) 53, this.Type);
        bw.WriteInt32(this.UVNumber);
        MTD.WriteMarker(bw, (byte) 53);
        bw.WriteInt32(this.ShaderDataIndex);
        if (this.Extended)
        {
          bw.WriteInt32(163);
          MTD.WriteMarkedString(bw, (byte) 186, this.Path);
          bw.WriteInt32(this.UnkFloats.Count);
          bw.WriteSingles((IList<float>) this.UnkFloats);
        }
        BinaryWriterEx bw1 = bw;
        block.Finish(bw1);
      }

      public override string ToString()
      {
        return this.Type;
      }
    }

    public enum BlendMode
    {
      Normal = 0,
      TexEdge = 1,
      Blend = 2,
      Water = 3,
      Add = 4,
      Sub = 5,
      Mul = 6,
      AddMul = 7,
      SubMul = 8,
      WaterWave = 9,
      LSNormal = 32, // 0x00000020
      LSTexEdge = 33, // 0x00000021
      LSBlend = 34, // 0x00000022
      LSWater = 35, // 0x00000023
      LSAdd = 36, // 0x00000024
      LSSub = 37, // 0x00000025
      LSMul = 38, // 0x00000026
      LSAddMul = 39, // 0x00000027
      LSSubMul = 40, // 0x00000028
      LSWaterWave = 41, // 0x00000029
    }

    public enum LightingType
    {
      None = 0,
      HemDirDifSpcx3 = 1,
      HemEnvDifSpc = 3,
    }

    private class Block
    {
      public long Start;
      public uint Length;
      public int Type;
      public int Version;
      public byte Marker;

      private Block(long start, uint length, int type, int version, byte marker)
      {
        this.Start = start;
        this.Length = length;
        this.Type = type;
        this.Version = version;
        this.Marker = marker;
      }

      public static MTD.Block Read(
        BinaryReaderEx br,
        int? assertType,
        int? assertVersion,
        byte? assertMarker)
      {
        br.AssertInt32(new int[1]);
        uint length = br.ReadUInt32();
        long position = br.Position;
        int num1;
        if (!assertType.HasValue)
          num1 = br.ReadInt32();
        else
          num1 = br.AssertInt32(assertType.Value);
        int type = num1;
        int num2;
        if (!assertVersion.HasValue)
          num2 = br.ReadInt32();
        else
          num2 = br.AssertInt32(assertVersion.Value);
        int version = num2;
        byte marker = assertMarker.HasValue ? MTD.AssertMarker(br, assertMarker.Value) : MTD.ReadMarker(br);
        return new MTD.Block(position, length, type, version, marker);
      }

      public static MTD.Block Write(BinaryWriterEx bw, int type, int version, byte marker)
      {
        bw.WriteInt32(0);
        long start = bw.Position + 4L;
        bw.ReserveUInt32(string.Format("Block{0:X}", (object) start));
        bw.WriteInt32(type);
        bw.WriteInt32(version);
        MTD.WriteMarker(bw, marker);
        return new MTD.Block(start, 0U, type, version, marker);
      }

      public void Finish(BinaryWriterEx bw)
      {
        this.Length = (uint) (bw.Position - this.Start);
        bw.FillUInt32(string.Format("Block{0:X}", (object) this.Start), this.Length);
      }
    }
  }
}
