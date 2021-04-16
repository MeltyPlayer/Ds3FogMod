// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BinaryReaderEx
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace SoulsFormats {
  [ComVisible(true)]
  public class BinaryReaderEx {
    private BinaryReader br;
    private Stack<long> steps;

    public bool BigEndian { get; set; }

    public bool VarintLong { get; set; }

    public int VarintSize {
      get { return !this.VarintLong ? 4 : 8; }
    }

    public Stream Stream { get; private set; }

    public long Position {
      get { return this.Stream.Position; }
      set { this.Stream.Position = value; }
    }

    public long Length {
      get { return this.Stream.Length; }
    }

    public BinaryReaderEx(bool bigEndian, byte[] input)
        : this(bigEndian, (Stream) new MemoryStream(input)) {}

    public BinaryReaderEx(bool bigEndian, Stream stream) {
      this.BigEndian = bigEndian;
      this.steps = new Stack<long>();
      this.Stream = stream;
      this.br = new BinaryReader(stream);
    }

    private byte[] ReadReversedBytes(int length) {
      byte[] numArray = this.ReadBytes(length);
      Array.Reverse((Array) numArray);
      return numArray;
    }

    private T GetValue<T>(Func<T> readValue, long offset) {
      this.StepIn(offset);
      T obj = readValue();
      this.StepOut();
      return obj;
    }

    private T[] GetValues<T>(
        Func<int, T[]> readValues,
        long offset,
        int count) {
      this.StepIn(offset);
      T[] objArray = readValues(count);
      this.StepOut();
      return objArray;
    }

    private T AssertValue<T>(
        T value,
        string typeName,
        string valueFormat,
        T[] options) where T : IEquatable<T> {
      foreach (T option in options) {
        if (value.Equals(option))
          return value;
      }
      string str1 = string.Format(valueFormat, (object) value);
      string str2 = string.Join(", ",
                                ((IEnumerable<T>) options).Select<T, string>(
                                    (Func<T, string>) (o => string.Format(
                                                            valueFormat,
                                                            (object) o))));
      throw new InvalidDataException(string.Format(
                                         "Read {0}: {1} | Expected: {2} | Ending position: 0x{3:X}",
                                         (object) typeName,
                                         (object) str1,
                                         (object) str2,
                                         (object) this.Position));
    }

    public void StepIn(long offset) {
      this.steps.Push(this.Stream.Position);
      this.Stream.Position = offset;
    }

    public void StepOut() {
      if (this.steps.Count == 0)
        throw new InvalidOperationException(
            "Reader is already stepped all the way out.");
      this.Stream.Position = this.steps.Pop();
    }

    public void Pad(int align) {
      if (this.Stream.Position % (long) align <= 0L)
        return;
      this.Stream.Position +=
          (long) align - this.Stream.Position % (long) align;
    }

    public void Skip(int count) {
      this.Stream.Position += (long) count;
    }

    public bool ReadBoolean() {
      byte num = this.br.ReadByte();
      switch (num) {
        case 0:
          return false;
        case 1:
          return true;
        default:
          throw new InvalidDataException(
              string.Format(
                  "ReadBoolean encountered non-boolean value: 0x{0:X2}",
                  (object) num));
      }
    }

    public bool[] ReadBooleans(int count) {
      bool[] flagArray = new bool[count];
      for (int index = 0; index < count; ++index)
        flagArray[index] = this.ReadBoolean();
      return flagArray;
    }

    public bool GetBoolean(long offset) {
      return this.GetValue<bool>(new Func<bool>(this.ReadBoolean), offset);
    }

    public bool[] GetBooleans(long offset, int count) {
      return this.GetValues<bool>(new Func<int, bool[]>(this.ReadBooleans),
                                  offset,
                                  count);
    }

    public bool AssertBoolean(bool option) {
      return this.AssertValue<bool>((this.ReadBoolean() ? 1 : 0) != 0,
                                    "Boolean",
                                    "{0}",
                                    new bool[1] {
                                        option
                                    });
    }

    public sbyte ReadSByte() {
      return this.br.ReadSByte();
    }

    public sbyte[] ReadSBytes(int count) {
      sbyte[] numArray = new sbyte[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadSByte();
      return numArray;
    }

    public sbyte GetSByte(long offset) {
      return this.GetValue<sbyte>(new Func<sbyte>(this.ReadSByte), offset);
    }

    public sbyte[] GetSBytes(long offset, int count) {
      return this.GetValues<sbyte>(new Func<int, sbyte[]>(this.ReadSBytes),
                                   offset,
                                   count);
    }

    public sbyte AssertSByte(params sbyte[] options) {
      return this.AssertValue<sbyte>(this.ReadSByte(),
                                     "SByte",
                                     "0x{0:X}",
                                     options);
    }

    public byte ReadByte() {
      return this.br.ReadByte();
    }

    public byte[] ReadBytes(int count) {
      byte[] numArray = this.br.ReadBytes(count);
      if (numArray.Length == count)
        return numArray;
      throw new EndOfStreamException(
          "Remaining size of stream was smaller than requested number of bytes.");
    }

    public byte GetByte(long offset) {
      return this.GetValue<byte>(new Func<byte>(this.ReadByte), offset);
    }

    public byte[] GetBytes(long offset, int count) {
      this.StepIn(offset);
      byte[] numArray = this.ReadBytes(count);
      this.StepOut();
      return numArray;
    }

    public byte AssertByte(params byte[] options) {
      return this.AssertValue<byte>(this.ReadByte(),
                                    "Byte",
                                    "0x{0:X}",
                                    options);
    }

    public short ReadInt16() {
      return this.BigEndian
                 ? BitConverter.ToInt16(this.ReadReversedBytes(2), 0)
                 : this.br.ReadInt16();
    }

    public short[] ReadInt16s(int count) {
      short[] numArray = new short[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadInt16();
      return numArray;
    }

    public short GetInt16(long offset) {
      return this.GetValue<short>(new Func<short>(this.ReadInt16), offset);
    }

    public short[] GetInt16s(long offset, int count) {
      return this.GetValues<short>(new Func<int, short[]>(this.ReadInt16s),
                                   offset,
                                   count);
    }

    public short AssertInt16(params short[] options) {
      return this.AssertValue<short>(this.ReadInt16(),
                                     "Int16",
                                     "0x{0:X}",
                                     options);
    }

    public ushort ReadUInt16() {
      return this.BigEndian
                 ? BitConverter.ToUInt16(this.ReadReversedBytes(2), 0)
                 : this.br.ReadUInt16();
    }

    public ushort[] ReadUInt16s(int count) {
      ushort[] numArray = new ushort[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadUInt16();
      return numArray;
    }

    public ushort GetUInt16(long offset) {
      return this.GetValue<ushort>(new Func<ushort>(this.ReadUInt16), offset);
    }

    public ushort[] GetUInt16s(long offset, int count) {
      return this.GetValues<ushort>(new Func<int, ushort[]>(this.ReadUInt16s),
                                    offset,
                                    count);
    }

    public ushort AssertUInt16(params ushort[] options) {
      return this.AssertValue<ushort>(this.ReadUInt16(),
                                      "UInt16",
                                      "0x{0:X}",
                                      options);
    }

    public int ReadInt32() {
      return this.BigEndian
                 ? BitConverter.ToInt32(this.ReadReversedBytes(4), 0)
                 : this.br.ReadInt32();
    }

    public int[] ReadInt32s(int count) {
      int[] numArray = new int[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadInt32();
      return numArray;
    }

    public int GetInt32(long offset) {
      return this.GetValue<int>(new Func<int>(this.ReadInt32), offset);
    }

    public int[] GetInt32s(long offset, int count) {
      return this.GetValues<int>(new Func<int, int[]>(this.ReadInt32s),
                                 offset,
                                 count);
    }

    public int AssertInt32(params int[] options) {
      return this.AssertValue<int>(this.ReadInt32(),
                                   "Int32",
                                   "0x{0:X}",
                                   options);
    }

    public uint ReadUInt32() {
      return this.BigEndian
                 ? BitConverter.ToUInt32(this.ReadReversedBytes(4), 0)
                 : this.br.ReadUInt32();
    }

    public uint[] ReadUInt32s(int count) {
      uint[] numArray = new uint[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadUInt32();
      return numArray;
    }

    public uint GetUInt32(long offset) {
      return this.GetValue<uint>(new Func<uint>(this.ReadUInt32), offset);
    }

    public uint[] GetUInt32s(long offset, int count) {
      return this.GetValues<uint>(new Func<int, uint[]>(this.ReadUInt32s),
                                  offset,
                                  count);
    }

    public uint AssertUInt32(params uint[] options) {
      return this.AssertValue<uint>(this.ReadUInt32(),
                                    "UInt32",
                                    "0x{0:X}",
                                    options);
    }

    public long ReadInt64() {
      return this.BigEndian
                 ? BitConverter.ToInt64(this.ReadReversedBytes(8), 0)
                 : this.br.ReadInt64();
    }

    public long[] ReadInt64s(int count) {
      long[] numArray = new long[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadInt64();
      return numArray;
    }

    public long GetInt64(long offset) {
      return this.GetValue<long>(new Func<long>(this.ReadInt64), offset);
    }

    public long[] GetInt64s(long offset, int count) {
      return this.GetValues<long>(new Func<int, long[]>(this.ReadInt64s),
                                  offset,
                                  count);
    }

    public long AssertInt64(params long[] options) {
      return this.AssertValue<long>(this.ReadInt64(),
                                    "Int64",
                                    "0x{0:X}",
                                    options);
    }

    public ulong ReadUInt64() {
      return this.BigEndian
                 ? BitConverter.ToUInt64(this.ReadReversedBytes(8), 0)
                 : this.br.ReadUInt64();
    }

    public ulong[] ReadUInt64s(int count) {
      ulong[] numArray = new ulong[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadUInt64();
      return numArray;
    }

    public ulong GetUInt64(long offset) {
      return this.GetValue<ulong>(new Func<ulong>(this.ReadUInt64), offset);
    }

    public ulong[] GetUInt64s(long offset, int count) {
      return this.GetValues<ulong>(new Func<int, ulong[]>(this.ReadUInt64s),
                                   offset,
                                   count);
    }

    public ulong AssertUInt64(params ulong[] options) {
      return this.AssertValue<ulong>(this.ReadUInt64(),
                                     "UInt64",
                                     "0x{0:X}",
                                     options);
    }

    public long ReadVarint() {
      return this.VarintLong ? this.ReadInt64() : (long) this.ReadInt32();
    }

    public long[] ReadVarints(int count) {
      long[] numArray = new long[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = !this.VarintLong
                              ? (long) this.ReadInt32()
                              : this.ReadInt64();
      return numArray;
    }

    public long GetVarint(long offset) {
      return this.VarintLong
                 ? this.GetInt64(offset)
                 : (long) this.GetInt32(offset);
    }

    public long[] GetVarints(long offset, int count) {
      return this.GetValues<long>(new Func<int, long[]>(this.ReadVarints),
                                  offset,
                                  count);
    }

    public long AssertVarint(params long[] options) {
      return this.AssertValue<long>(this.ReadVarint(),
                                    this.VarintLong ? "Varint64" : "Varint32",
                                    "0x{0:X}",
                                    options);
    }

    public float ReadSingle() {
      return this.BigEndian
                 ? BitConverter.ToSingle(this.ReadReversedBytes(4), 0)
                 : this.br.ReadSingle();
    }

    public float[] ReadSingles(int count) {
      float[] numArray = new float[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadSingle();
      return numArray;
    }

    public float GetSingle(long offset) {
      return this.GetValue<float>(new Func<float>(this.ReadSingle), offset);
    }

    public float[] GetSingles(long offset, int count) {
      return this.GetValues<float>(new Func<int, float[]>(this.ReadSingles),
                                   offset,
                                   count);
    }

    public float AssertSingle(params float[] options) {
      return this.AssertValue<float>(this.ReadSingle(),
                                     "Single",
                                     "{0}",
                                     options);
    }

    public double ReadDouble() {
      return this.BigEndian
                 ? BitConverter.ToDouble(this.ReadReversedBytes(8), 0)
                 : this.br.ReadDouble();
    }

    public double[] ReadDoubles(int count) {
      double[] numArray = new double[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = this.ReadDouble();
      return numArray;
    }

    public double GetDouble(long offset) {
      return this.GetValue<double>(new Func<double>(this.ReadDouble), offset);
    }

    public double[] GetDoubles(long offset, int count) {
      return this.GetValues<double>(new Func<int, double[]>(this.ReadDoubles),
                                    offset,
                                    count);
    }

    public double AssertDouble(params double[] options) {
      return this.AssertValue<double>(this.ReadDouble(),
                                      "Double",
                                      "{0}",
                                      options);
    }

    private TEnum ReadEnum<TEnum, TValue>(
        Func<TValue> readValue,
        string valueFormat) {
      TValue obj = readValue();
      if (!System.Enum.IsDefined(typeof(TEnum), (object) obj))
        throw new InvalidDataException(
            string.Format("Read Byte not present in enum: {0}",
                          (object) string.Format(valueFormat, (object) obj)));
      return (TEnum) (object) obj;
    }

    public TEnum ReadEnum8<TEnum>() where TEnum : System.Enum {
      return this.ReadEnum<TEnum, byte>(new Func<byte>(this.ReadByte),
                                        "0x{0:X}");
    }

    public TEnum GetEnum8<TEnum>(long position) where TEnum : System.Enum {
      this.StepIn(position);
      TEnum @enum = this.ReadEnum8<TEnum>();
      this.StepOut();
      return @enum;
    }

    public TEnum ReadEnum16<TEnum>() where TEnum : System.Enum {
      return this.ReadEnum<TEnum, ushort>(new Func<ushort>(this.ReadUInt16),
                                          "0x{0:X}");
    }

    public TEnum GetEnum16<TEnum>(long position) where TEnum : System.Enum {
      this.StepIn(position);
      TEnum @enum = this.ReadEnum16<TEnum>();
      this.StepOut();
      return @enum;
    }

    public TEnum ReadEnum32<TEnum>() where TEnum : System.Enum {
      return this.ReadEnum<TEnum, uint>(new Func<uint>(this.ReadUInt32),
                                        "0x{0:X}");
    }

    public TEnum GetEnum32<TEnum>(long position) where TEnum : System.Enum {
      this.StepIn(position);
      TEnum @enum = this.ReadEnum32<TEnum>();
      this.StepOut();
      return @enum;
    }

    public TEnum ReadEnum64<TEnum>() where TEnum : System.Enum {
      return this.ReadEnum<TEnum, ulong>(new Func<ulong>(this.ReadUInt64),
                                         "0x{0:X}");
    }

    public TEnum GetEnum64<TEnum>(long position) where TEnum : System.Enum {
      this.StepIn(position);
      TEnum @enum = this.ReadEnum64<TEnum>();
      this.StepOut();
      return @enum;
    }

    private string ReadChars(Encoding encoding, int length) {
      byte[] bytes = this.ReadBytes(length);
      return encoding.GetString(bytes);
    }

    private string ReadCharsTerminated(Encoding encoding) {
      List<byte> byteList = new List<byte>();
      for (byte index = this.ReadByte();
           index != (byte) 0;
           index = this.ReadByte())
        byteList.Add(index);
      return encoding.GetString(byteList.ToArray());
    }

    public string ReadASCII() {
      return this.ReadCharsTerminated(SFEncoding.ASCII);
    }

    public string ReadASCII(int length) {
      return this.ReadChars(SFEncoding.ASCII, length);
    }

    public string GetASCII(long offset) {
      this.StepIn(offset);
      string str = this.ReadASCII();
      this.StepOut();
      return str;
    }

    public string GetASCII(long offset, int length) {
      this.StepIn(offset);
      string str = this.ReadASCII(length);
      this.StepOut();
      return str;
    }

    public string AssertASCII(params string[] values) {
      string str1 = this.ReadASCII(values[0].Length);
      bool flag = false;
      foreach (string str2 in values) {
        if (str1 == str2)
          flag = true;
      }
      if (!flag)
        throw new InvalidDataException(
            string.Format("Read ASCII: {0} | Expected ASCII: {1}",
                          (object) str1,
                          (object) string.Join(", ", values)));
      return str1;
    }

    public string ReadShiftJIS() {
      return this.ReadCharsTerminated(SFEncoding.ShiftJIS);
    }

    public string ReadShiftJIS(int length) {
      return this.ReadChars(SFEncoding.ShiftJIS, length);
    }

    public string GetShiftJIS(long offset) {
      this.StepIn(offset);
      string str = this.ReadShiftJIS();
      this.StepOut();
      return str;
    }

    public string GetShiftJIS(long offset, int length) {
      this.StepIn(offset);
      string str = this.ReadShiftJIS(length);
      this.StepOut();
      return str;
    }

    public string ReadUTF16() {
      List<byte> byteList = new List<byte>();
      for (byte[] numArray = this.ReadBytes(2);
           numArray[0] != (byte) 0 || numArray[1] != (byte) 0;
           numArray = this.ReadBytes(2)) {
        byteList.Add(numArray[0]);
        byteList.Add(numArray[1]);
      }
      return this.BigEndian
                 ? SFEncoding.UTF16BE.GetString(byteList.ToArray())
                 : SFEncoding.UTF16.GetString(byteList.ToArray());
    }

    public string GetUTF16(long offset) {
      this.StepIn(offset);
      string str = this.ReadUTF16();
      this.StepOut();
      return str;
    }

    public string ReadFixStr(int size) {
      byte[] bytes = this.ReadBytes(size);
      int count = 0;
      while (count < size && bytes[count] != (byte) 0)
        ++count;
      return SFEncoding.ShiftJIS.GetString(bytes, 0, count);
    }

    public string ReadFixStrW(int size) {
      byte[] bytes = this.ReadBytes(size);
      int count;
      for (count = 0; count < size; count += 2) {
        if (count == size - 1)
          --count;
        else if (bytes[count] == (byte) 0 && bytes[count + 1] == (byte) 0)
          break;
      }
      return this.BigEndian
                 ? SFEncoding.UTF16BE.GetString(bytes, 0, count)
                 : SFEncoding.UTF16.GetString(bytes, 0, count);
    }

    public Vector2 ReadVector2() {
      return new Vector2(this.ReadSingle(), this.ReadSingle());
    }

    public Vector3 ReadVector3() {
      double num1 = (double) this.ReadSingle();
      float num2 = this.ReadSingle();
      float num3 = this.ReadSingle();
      double num4 = (double) num2;
      double num5 = (double) num3;
      return new Vector3((float) num1, (float) num4, (float) num5);
    }

    public Vector4 ReadVector4() {
      double num1 = (double) this.ReadSingle();
      float num2 = this.ReadSingle();
      float num3 = this.ReadSingle();
      float num4 = this.ReadSingle();
      double num5 = (double) num2;
      double num6 = (double) num3;
      double num7 = (double) num4;
      return new Vector4((float) num1,
                         (float) num5,
                         (float) num6,
                         (float) num7);
    }

    public void AssertPattern(int length, byte pattern) {
      byte[] numArray = this.ReadBytes(length);
      for (int index = 0; index < length; ++index) {
        if ((int) numArray[index] != (int) pattern)
          throw new InvalidDataException(
              string.Format("Expected {0} 0x{1:X2}, got {2:X2} at position {3}",
                            (object) length,
                            (object) pattern,
                            (object) numArray[index],
                            (object) index));
      }
    }

    public Color ReadARGB() {
      int alpha = (int) this.br.ReadByte();
      byte num1 = this.br.ReadByte();
      byte num2 = this.br.ReadByte();
      byte num3 = this.br.ReadByte();
      int red = (int) num1;
      int green = (int) num2;
      int blue = (int) num3;
      return Color.FromArgb(alpha, red, green, blue);
    }

    public Color ReadABGR() {
      int alpha = (int) this.br.ReadByte();
      byte num1 = this.br.ReadByte();
      byte num2 = this.br.ReadByte();
      int red = (int) this.br.ReadByte();
      int green = (int) num2;
      int blue = (int) num1;
      return Color.FromArgb(alpha, red, green, blue);
    }

    public Color ReadRGBA() {
      return Color.FromArgb((int) this.br.ReadByte(),
                            (int) this.br.ReadByte(),
                            (int) this.br.ReadByte(),
                            (int) this.br.ReadByte());
    }

    public Color ReadBGRA() {
      return Color.FromArgb((int) this.br.ReadByte(),
                            (int) this.br.ReadByte(),
                            (int) this.br.ReadByte(),
                            (int) this.br.ReadByte());
    }
  }
}