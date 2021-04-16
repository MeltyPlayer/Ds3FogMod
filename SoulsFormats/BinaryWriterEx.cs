// Decompiled with JetBrains decompiler
// Type: SoulsFormats.BinaryWriterEx
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace SoulsFormats {
  [ComVisible(true)]
  public class BinaryWriterEx {
    private BinaryWriter bw;
    private Stack<long> steps;
    private Dictionary<string, long> reservations;

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

    public BinaryWriterEx(bool bigEndian)
        : this(bigEndian, (Stream) new MemoryStream()) {}

    public BinaryWriterEx(bool bigEndian, Stream stream) {
      this.BigEndian = bigEndian;
      this.steps = new Stack<long>();
      this.reservations = new Dictionary<string, long>();
      this.Stream = stream;
      this.bw = new BinaryWriter(stream);
    }

    private void WriteReversedBytes(byte[] bytes) {
      Array.Reverse((Array) bytes);
      this.bw.Write(bytes);
    }

    private void Reserve(string name, string typeName, int length) {
      name = name + ":" + typeName;
      if (this.reservations.ContainsKey(name))
        throw new ArgumentException("Key already reserved: " + name);
      this.reservations[name] = this.Stream.Position;
      for (int index = 0; index < length; ++index)
        this.WriteByte((byte) 254);
    }

    private long Fill(string name, string typeName) {
      name = name + ":" + typeName;
      long num;
      if (!this.reservations.TryGetValue(name, out num))
        throw new ArgumentException("Key is not reserved: " + name);
      this.reservations.Remove(name);
      return num;
    }

    public void Finish() {
      if (this.reservations.Count > 0)
        throw new InvalidOperationException(
            "Not all reservations filled: " +
            string.Join(", ", (IEnumerable<string>) this.reservations.Keys));
      this.bw.Close();
    }

    public byte[] FinishBytes() {
      byte[] array = ((MemoryStream) this.Stream).ToArray();
      this.Finish();
      return array;
    }

    public void StepIn(long offset) {
      this.steps.Push(this.Stream.Position);
      this.Stream.Position = offset;
    }

    public void StepOut() {
      if (this.steps.Count == 0)
        throw new InvalidOperationException(
            "Writer is already stepped all the way out.");
      this.Stream.Position = this.steps.Pop();
    }

    public void Pad(int align) {
      while (this.Stream.Position % (long) align > 0L)
        this.WriteByte((byte) 0);
    }

    public void WriteBoolean(bool value) {
      this.bw.Write(value);
    }

    public void WriteBooleans(IList<bool> values) {
      foreach (bool flag in (IEnumerable<bool>) values)
        this.WriteBoolean(flag);
    }

    public void ReserveBoolean(string name) {
      this.Reserve(name, "Boolean", 1);
    }

    public void FillBoolean(string name, bool value) {
      this.StepIn(this.Fill(name, "Boolean"));
      this.WriteBoolean(value);
      this.StepOut();
    }

    public void WriteSByte(sbyte value) {
      this.bw.Write(value);
    }

    public void WriteSBytes(IList<sbyte> values) {
      foreach (sbyte num in (IEnumerable<sbyte>) values)
        this.WriteSByte(num);
    }

    public void ReserveSByte(string name) {
      this.Reserve(name, "SByte", 1);
    }

    public void FillSByte(string name, sbyte value) {
      this.StepIn(this.Fill(name, "SByte"));
      this.WriteSByte(value);
      this.StepOut();
    }

    public void WriteByte(byte value) {
      this.bw.Write(value);
    }

    public void WriteBytes(byte[] bytes) {
      this.bw.Write(bytes);
    }

    public void WriteBytes(IList<byte> values) {
      foreach (byte num in (IEnumerable<byte>) values)
        this.WriteByte(num);
    }

    public void ReserveByte(string name) {
      this.Reserve(name, "Byte", 1);
    }

    public void FillByte(string name, byte value) {
      this.StepIn(this.Fill(name, "Byte"));
      this.WriteByte(value);
      this.StepOut();
    }

    public void WriteInt16(short value) {
      if (this.BigEndian)
        this.WriteReversedBytes(BitConverter.GetBytes(value));
      else
        this.bw.Write(value);
    }

    public void WriteInt16s(IList<short> values) {
      foreach (short num in (IEnumerable<short>) values)
        this.WriteInt16(num);
    }

    public void ReserveInt16(string name) {
      this.Reserve(name, "Int16", 2);
    }

    public void FillInt16(string name, short value) {
      this.StepIn(this.Fill(name, "Int16"));
      this.WriteInt16(value);
      this.StepOut();
    }

    public void WriteUInt16(ushort value) {
      if (this.BigEndian)
        this.WriteReversedBytes(BitConverter.GetBytes(value));
      else
        this.bw.Write(value);
    }

    public void WriteUInt16s(IList<ushort> values) {
      foreach (ushort num in (IEnumerable<ushort>) values)
        this.WriteUInt16(num);
    }

    public void ReserveUInt16(string name) {
      this.Reserve(name, "UInt16", 2);
    }

    public void FillUInt16(string name, ushort value) {
      this.StepIn(this.Fill(name, "UInt16"));
      this.WriteUInt16(value);
      this.StepOut();
    }

    public void WriteInt32(int value) {
      if (this.BigEndian)
        this.WriteReversedBytes(BitConverter.GetBytes(value));
      else
        this.bw.Write(value);
    }

    public void WriteInt32s(IList<int> values) {
      foreach (int num in (IEnumerable<int>) values)
        this.WriteInt32(num);
    }

    public void ReserveInt32(string name) {
      this.Reserve(name, "Int32", 4);
    }

    public void FillInt32(string name, int value) {
      this.StepIn(this.Fill(name, "Int32"));
      this.WriteInt32(value);
      this.StepOut();
    }

    public void WriteUInt32(uint value) {
      if (this.BigEndian)
        this.WriteReversedBytes(BitConverter.GetBytes(value));
      else
        this.bw.Write(value);
    }

    public void WriteUInt32s(IList<uint> values) {
      foreach (uint num in (IEnumerable<uint>) values)
        this.WriteUInt32(num);
    }

    public void ReserveUInt32(string name) {
      this.Reserve(name, "UInt32", 4);
    }

    public void FillUInt32(string name, uint value) {
      this.StepIn(this.Fill(name, "UInt32"));
      this.WriteUInt32(value);
      this.StepOut();
    }

    public void WriteInt64(long value) {
      if (this.BigEndian)
        this.WriteReversedBytes(BitConverter.GetBytes(value));
      else
        this.bw.Write(value);
    }

    public void WriteInt64s(IList<long> values) {
      foreach (long num in (IEnumerable<long>) values)
        this.WriteInt64(num);
    }

    public void ReserveInt64(string name) {
      this.Reserve(name, "Int64", 8);
    }

    public void FillInt64(string name, long value) {
      this.StepIn(this.Fill(name, "Int64"));
      this.WriteInt64(value);
      this.StepOut();
    }

    public void WriteUInt64(ulong value) {
      if (this.BigEndian)
        this.WriteReversedBytes(BitConverter.GetBytes(value));
      else
        this.bw.Write(value);
    }

    public void WriteUInt64s(IList<ulong> values) {
      foreach (ulong num in (IEnumerable<ulong>) values)
        this.WriteUInt64(num);
    }

    public void ReserveUInt64(string name) {
      this.Reserve(name, "UInt64", 8);
    }

    public void FillUInt64(string name, ulong value) {
      this.StepIn(this.Fill(name, "UInt64"));
      this.WriteUInt64(value);
      this.StepOut();
    }

    public void WriteVarint(long value) {
      if (this.VarintLong)
        this.WriteInt64(value);
      else
        this.WriteInt32((int) value);
    }

    public void WriteVarints(IList<long> values) {
      foreach (long num in (IEnumerable<long>) values) {
        if (this.VarintLong)
          this.WriteInt64(num);
        else
          this.WriteInt32((int) num);
      }
    }

    public void ReserveVarint(string name) {
      if (this.VarintLong)
        this.Reserve(name, "Varint64", 8);
      else
        this.Reserve(name, "Varint32", 4);
    }

    public void FillVarint(string name, long value) {
      if (this.VarintLong) {
        this.StepIn(this.Fill(name, "Varint64"));
        this.WriteInt64(value);
        this.StepOut();
      } else {
        this.StepIn(this.Fill(name, "Varint32"));
        this.WriteInt32((int) value);
        this.StepOut();
      }
    }

    public void WriteSingle(float value) {
      if (this.BigEndian)
        this.WriteReversedBytes(BitConverter.GetBytes(value));
      else
        this.bw.Write(value);
    }

    public void WriteSingles(IList<float> values) {
      foreach (float num in (IEnumerable<float>) values)
        this.WriteSingle(num);
    }

    public void ReserveSingle(string name) {
      this.Reserve(name, "Single", 4);
    }

    public void FillSingle(string name, float value) {
      this.StepIn(this.Fill(name, "Single"));
      this.WriteSingle(value);
      this.StepOut();
    }

    public void WriteDouble(double value) {
      if (this.BigEndian)
        this.WriteReversedBytes(BitConverter.GetBytes(value));
      else
        this.bw.Write(value);
    }

    public void WriteDoubles(IList<double> values) {
      foreach (double num in (IEnumerable<double>) values)
        this.WriteDouble(num);
    }

    public void ReserveDouble(string name) {
      this.Reserve(name, "Double", 8);
    }

    public void FillDouble(string name, double value) {
      this.StepIn(this.Fill(name, "Double"));
      this.WriteDouble(value);
      this.StepOut();
    }

    private void WriteChars(string text, Encoding encoding, bool terminate) {
      if (terminate)
        text += "\0";
      this.bw.Write(encoding.GetBytes(text));
    }

    public void WriteASCII(string text, bool terminate = false) {
      this.WriteChars(text, SFEncoding.ASCII, terminate);
    }

    public void WriteShiftJIS(string text, bool terminate = false) {
      this.WriteChars(text, SFEncoding.ShiftJIS, terminate);
    }

    public void WriteUTF16(string text, bool terminate = false) {
      if (this.BigEndian)
        this.WriteChars(text, SFEncoding.UTF16BE, terminate);
      else
        this.WriteChars(text, SFEncoding.UTF16, terminate);
    }

    public void WriteFixStr(string text, int size, byte padding = 0) {
      byte[] buffer = new byte[size];
      for (int index = 0; index < size; ++index)
        buffer[index] = padding;
      byte[] bytes = SFEncoding.ShiftJIS.GetBytes(text + "\0");
      Array.Copy((Array) bytes, (Array) buffer, Math.Min(size, bytes.Length));
      this.bw.Write(buffer);
    }

    public void WriteFixStrW(string text, int size, byte padding = 0) {
      byte[] buffer = new byte[size];
      for (int index = 0; index < size; ++index)
        buffer[index] = padding;
      byte[] numArray = !this.BigEndian
                            ? SFEncoding.UTF16.GetBytes(text + "\0")
                            : SFEncoding.UTF16BE.GetBytes(text + "\0");
      Array.Copy((Array) numArray,
                 (Array) buffer,
                 Math.Min(size, numArray.Length));
      this.bw.Write(buffer);
    }

    public void WriteVector2(Vector2 vector) {
      this.WriteSingle(vector.X);
      this.WriteSingle(vector.Y);
    }

    public void WriteVector3(Vector3 vector) {
      this.WriteSingle(vector.X);
      this.WriteSingle(vector.Y);
      this.WriteSingle(vector.Z);
    }

    public void WriteVector4(Vector4 vector) {
      this.WriteSingle(vector.X);
      this.WriteSingle(vector.Y);
      this.WriteSingle(vector.Z);
      this.WriteSingle(vector.W);
    }

    public void WritePattern(int length, byte pattern) {
      byte[] bytes = new byte[length];
      if (pattern != (byte) 0) {
        for (int index = 0; index < length; ++index)
          bytes[index] = pattern;
      }
      this.WriteBytes(bytes);
    }

    public void WriteARGB(Color color) {
      this.bw.Write(color.A);
      this.bw.Write(color.R);
      this.bw.Write(color.G);
      this.bw.Write(color.B);
    }

    public void WriteABGR(Color color) {
      this.bw.Write(color.A);
      this.bw.Write(color.B);
      this.bw.Write(color.G);
      this.bw.Write(color.R);
    }

    public void WriteRGBA(Color color) {
      this.bw.Write(color.R);
      this.bw.Write(color.G);
      this.bw.Write(color.B);
      this.bw.Write(color.A);
    }

    public void WriteBGRA(Color color) {
      this.bw.Write(color.B);
      this.bw.Write(color.G);
      this.bw.Write(color.R);
      this.bw.Write(color.A);
    }
  }
}