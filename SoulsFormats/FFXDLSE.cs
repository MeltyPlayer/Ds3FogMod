// Decompiled with JetBrains decompiler
// Type: SoulsFormats.FFXDLSE
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SoulsFormats {
  [ComVisible(true)]
  public class FFXDLSE : SoulsFile<FFXDLSE> {
    private static XmlSerializer _ffxSerializer;
    private static XmlSerializer _stateSerializer;
    private static XmlSerializer _paramSerializer;

    public FFXDLSE.FXEffect Effect { get; set; }

    public FFXDLSE() {
      this.Effect = new FFXDLSE.FXEffect();
    }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "DLsE";
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertASCII("DLsE");
      int num1 = (int) br.AssertByte((byte) 1);
      int num2 = (int) br.AssertByte((byte) 3);
      int num3 = (int) br.AssertByte(new byte[1]);
      int num4 = (int) br.AssertByte(new byte[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      int num5 = (int) br.AssertByte(new byte[1]);
      br.AssertInt32(1);
      short num6 = br.ReadInt16();
      List<string> classNames = new List<string>((int) num6);
      for (int index = 0; index < (int) num6; ++index) {
        int length = br.ReadInt32();
        classNames.Add(br.ReadASCII(length));
      }
      this.Effect = new FFXDLSE.FXEffect(br, classNames);
    }

    protected override void Write(BinaryWriterEx bw) {
      List<string> classNames = new List<string>();
      this.Effect.AddClassNames(classNames);
      bw.BigEndian = false;
      bw.WriteASCII("DLsE", false);
      bw.WriteByte((byte) 1);
      bw.WriteByte((byte) 3);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(1);
      bw.WriteInt16((short) classNames.Count);
      foreach (string text in classNames) {
        bw.WriteInt32(text.Length);
        bw.WriteASCII(text, false);
      }
      this.Effect.Write(bw, classNames);
    }

    private static XmlSerializer MakeSerializers(int returnIndex) {
      XmlSerializer[] xmlSerializerArray = XmlSerializer.FromTypes(
          new System.Type[3] {
              typeof(FFXDLSE),
              typeof(FFXDLSE.State),
              typeof(FFXDLSE.Param)
          });
      FFXDLSE._ffxSerializer = xmlSerializerArray[0];
      FFXDLSE._stateSerializer = xmlSerializerArray[1];
      FFXDLSE._paramSerializer = xmlSerializerArray[2];
      return xmlSerializerArray[returnIndex];
    }

    private static XmlSerializer FFXSerializer {
      get { return FFXDLSE._ffxSerializer ?? FFXDLSE.MakeSerializers(0); }
    }

    private static XmlSerializer StateSerializer {
      get { return FFXDLSE._stateSerializer ?? FFXDLSE.MakeSerializers(1); }
    }

    private static XmlSerializer ParamSerializer {
      get { return FFXDLSE._paramSerializer ?? FFXDLSE.MakeSerializers(2); }
    }

    public static FFXDLSE XmlDeserialize(Stream stream) {
      return (FFXDLSE) FFXDLSE.FFXSerializer.Deserialize(stream);
    }

    public static FFXDLSE XmlDeserialize(TextReader textReader) {
      return (FFXDLSE) FFXDLSE.FFXSerializer.Deserialize(textReader);
    }

    public static FFXDLSE XmlDeserialize(XmlReader xmlReader) {
      return (FFXDLSE) FFXDLSE.FFXSerializer.Deserialize(xmlReader);
    }

    public void XmlSerialize(Stream stream) {
      FFXDLSE.FFXSerializer.Serialize(stream, (object) this);
    }

    public void XmlSerialize(TextWriter textWriter) {
      FFXDLSE.FFXSerializer.Serialize(textWriter, (object) this);
    }

    public void XmlSerialize(XmlWriter xmlWriter) {
      FFXDLSE.FFXSerializer.Serialize(xmlWriter, (object) this);
    }

    public class Action : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializableAction"; }
      }

      internal override int Version {
        get { return 1; }
      }

      [XmlAttribute] public int ID { get; set; }

      public FFXDLSE.ParamList ParamList { get; set; }

      public Action() {
        this.ParamList = new FFXDLSE.ParamList();
      }

      internal Action(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        this.ID = br.ReadInt32();
        this.ParamList = new FFXDLSE.ParamList(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.ParamList.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(this.ID);
        this.ParamList.Write(bw, classNames);
      }
    }

    [XmlInclude(typeof(FFXDLSE.EvaluatableConstant))]
    [XmlInclude(typeof(FFXDLSE.Evaluatable2))]
    [XmlInclude(typeof(FFXDLSE.Evaluatable3))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableCurrentTick))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableTotalTick))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableAnd))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableOr))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableGE))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableGT))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableLE))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableLT))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableEQ))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableNE))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableNot))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableChildExists))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableParentExists))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableDistanceFromCamera))]
    [XmlInclude(typeof(FFXDLSE.EvaluatableEmittersStopped))]
    public abstract class Evaluatable : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializableEvaluatable<dl_int32>"; }
      }

      internal override int Version {
        get { return 1; }
      }

      internal abstract int Opcode { get; }

      internal abstract int Type { get; }

      public Evaluatable() {}

      internal Evaluatable(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        br.AssertInt32(this.Opcode);
        br.AssertInt32(this.Type);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(this.Opcode);
        bw.WriteInt32(this.Type);
      }

      internal static FFXDLSE.Evaluatable Read(
          BinaryReaderEx br,
          List<string> classNames) {
        int int32 = br.GetInt32(br.Position + 10L);
        switch (int32) {
          case 1:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableConstant(
                br,
                classNames);
          case 2:
            return (FFXDLSE.Evaluatable) new FFXDLSE.Evaluatable2(
                br,
                classNames);
          case 3:
            return (FFXDLSE.Evaluatable) new FFXDLSE.Evaluatable3(
                br,
                classNames);
          case 4:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableCurrentTick(
                br,
                classNames);
          case 5:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableTotalTick(
                br,
                classNames);
          case 8:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableAnd(
                br,
                classNames);
          case 9:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableOr(
                br,
                classNames);
          case 10:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableGE(
                br,
                classNames);
          case 11:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableGT(
                br,
                classNames);
          case 12:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableLE(
                br,
                classNames);
          case 13:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableLT(
                br,
                classNames);
          case 14:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableEQ(
                br,
                classNames);
          case 15:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableNE(
                br,
                classNames);
          case 20:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableNot(
                br,
                classNames);
          case 21:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableChildExists(
                br,
                classNames);
          case 22:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableParentExists(
                br,
                classNames);
          case 23:
            return (FFXDLSE.Evaluatable)
                new FFXDLSE.EvaluatableDistanceFromCamera(br, classNames);
          case 24:
            return (FFXDLSE.Evaluatable) new FFXDLSE.EvaluatableEmittersStopped(
                br,
                classNames);
          default:
            throw new NotImplementedException(
                string.Format("Unimplemented evaluatable opcode: {0}",
                              (object) int32));
        }
      }
    }

    public abstract class EvaluatableUnary : FFXDLSE.Evaluatable {
      internal override int Type {
        get { return 1; }
      }

      public FFXDLSE.Evaluatable Operand { get; set; }

      public EvaluatableUnary() {}

      internal EvaluatableUnary(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Operand = FFXDLSE.Evaluatable.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.Operand.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        this.Operand.Write(bw, classNames);
      }
    }

    public abstract class EvaluatableBinary : FFXDLSE.Evaluatable {
      internal override int Type {
        get { return 1; }
      }

      public FFXDLSE.Evaluatable Left { get; set; }

      public FFXDLSE.Evaluatable Right { get; set; }

      public EvaluatableBinary() {}

      internal EvaluatableBinary(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Right = FFXDLSE.Evaluatable.Read(br, classNames);
        this.Left = FFXDLSE.Evaluatable.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.Left.AddClassNames(classNames);
        this.Right.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        this.Right.Write(bw, classNames);
        this.Left.Write(bw, classNames);
      }
    }

    public class EvaluatableConstant : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 1; }
      }

      internal override int Type {
        get { return 3; }
      }

      [XmlAttribute] public int Value { get; set; }

      public EvaluatableConstant() {}

      internal EvaluatableConstant(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Value = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Value);
      }

      public override string ToString() {
        return string.Format("{0}", (object) this.Value);
      }
    }

    public class Evaluatable2 : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 2; }
      }

      internal override int Type {
        get { return 3; }
      }

      [XmlAttribute] public int Unk00 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Evaluatable2() {}

      internal Evaluatable2(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk00 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(this.ArgIndex);
      }

      public override string ToString() {
        return string.Format("{{2: {0}, {1}}}",
                             (object) this.Unk00,
                             (object) this.ArgIndex);
      }
    }

    public class Evaluatable3 : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 3; }
      }

      internal override int Type {
        get { return 3; }
      }

      [XmlAttribute] public int Unk00 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Evaluatable3() {}

      internal Evaluatable3(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk00 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(this.ArgIndex);
      }

      public override string ToString() {
        return string.Format("{{3: {0}, {1}}}",
                             (object) this.Unk00,
                             (object) this.ArgIndex);
      }
    }

    public class EvaluatableCurrentTick : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 4; }
      }

      internal override int Type {
        get { return 3; }
      }

      public EvaluatableCurrentTick() {}

      internal EvaluatableCurrentTick(
          BinaryReaderEx br,
          List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return "CurrentTick";
      }
    }

    public class EvaluatableTotalTick : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 5; }
      }

      internal override int Type {
        get { return 3; }
      }

      public EvaluatableTotalTick() {}

      internal EvaluatableTotalTick(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return "TotalTick";
      }
    }

    public class EvaluatableAnd : FFXDLSE.EvaluatableBinary {
      internal override int Opcode {
        get { return 8; }
      }

      public EvaluatableAnd() {}

      internal EvaluatableAnd(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("({0}) && ({1})",
                             (object) this.Left,
                             (object) this.Right);
      }
    }

    public class EvaluatableOr : FFXDLSE.EvaluatableBinary {
      internal override int Opcode {
        get { return 9; }
      }

      public EvaluatableOr() {}

      internal EvaluatableOr(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("({0}) || ({1})",
                             (object) this.Left,
                             (object) this.Right);
      }
    }

    public class EvaluatableGE : FFXDLSE.EvaluatableBinary {
      internal override int Opcode {
        get { return 10; }
      }

      public EvaluatableGE() {}

      internal EvaluatableGE(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("({0}) >= ({1})",
                             (object) this.Left,
                             (object) this.Right);
      }
    }

    public class EvaluatableGT : FFXDLSE.EvaluatableBinary {
      internal override int Opcode {
        get { return 11; }
      }

      public EvaluatableGT() {}

      internal EvaluatableGT(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("({0}) > ({1})",
                             (object) this.Left,
                             (object) this.Right);
      }
    }

    public class EvaluatableLE : FFXDLSE.EvaluatableBinary {
      internal override int Opcode {
        get { return 12; }
      }

      public EvaluatableLE() {}

      internal EvaluatableLE(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("({0}) <= ({1})",
                             (object) this.Left,
                             (object) this.Right);
      }
    }

    public class EvaluatableLT : FFXDLSE.EvaluatableBinary {
      internal override int Opcode {
        get { return 13; }
      }

      public EvaluatableLT() {}

      internal EvaluatableLT(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("({0}) < ({1})",
                             (object) this.Left,
                             (object) this.Right);
      }
    }

    public class EvaluatableEQ : FFXDLSE.EvaluatableBinary {
      internal override int Opcode {
        get { return 14; }
      }

      public EvaluatableEQ() {}

      internal EvaluatableEQ(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("({0}) == ({1})",
                             (object) this.Left,
                             (object) this.Right);
      }
    }

    public class EvaluatableNE : FFXDLSE.EvaluatableBinary {
      internal override int Opcode {
        get { return 15; }
      }

      public EvaluatableNE() {}

      internal EvaluatableNE(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("({0}) != ({1})",
                             (object) this.Left,
                             (object) this.Right);
      }
    }

    public class EvaluatableNot : FFXDLSE.EvaluatableUnary {
      internal override int Opcode {
        get { return 20; }
      }

      public EvaluatableNot() {}

      internal EvaluatableNot(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return string.Format("!({0})", (object) this.Operand);
      }
    }

    public class EvaluatableChildExists : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 21; }
      }

      internal override int Type {
        get { return 3; }
      }

      public EvaluatableChildExists() {}

      internal EvaluatableChildExists(
          BinaryReaderEx br,
          List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return "ChildExists";
      }
    }

    public class EvaluatableParentExists : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 22; }
      }

      internal override int Type {
        get { return 3; }
      }

      public EvaluatableParentExists() {}

      internal EvaluatableParentExists(
          BinaryReaderEx br,
          List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return "ParentExists";
      }
    }

    public class EvaluatableDistanceFromCamera : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 23; }
      }

      internal override int Type {
        get { return 3; }
      }

      public EvaluatableDistanceFromCamera() {}

      internal EvaluatableDistanceFromCamera(
          BinaryReaderEx br,
          List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return "DistanceFromCamera";
      }
    }

    public class EvaluatableEmittersStopped : FFXDLSE.Evaluatable {
      internal override int Opcode {
        get { return 24; }
      }

      internal override int Type {
        get { return 3; }
      }

      public EvaluatableEmittersStopped() {}

      internal EvaluatableEmittersStopped(
          BinaryReaderEx br,
          List<string> classNames)
          : base(br, classNames) {}

      public override string ToString() {
        return "EmittersStopped";
      }
    }

    private static class DLVector {
      public static List<int> Read(BinaryReaderEx br, List<string> classNames) {
        int num =
            (int) br.AssertInt16(
                (short) (classNames.IndexOf(nameof(DLVector)) + 1));
        int count = br.ReadInt32();
        return new List<int>((IEnumerable<int>) br.ReadInt32s(count));
      }

      public static void AddClassNames(List<string> classNames) {
        if (classNames.Contains(nameof(DLVector)))
          return;
        classNames.Add(nameof(DLVector));
      }

      public static void Write(
          BinaryWriterEx bw,
          List<string> classNames,
          List<int> vector) {
        bw.WriteInt16((short) (classNames.IndexOf(nameof(DLVector)) + 1));
        bw.WriteInt32(vector.Count);
        bw.WriteInt32s((IList<int>) vector);
      }
    }

    public abstract class FXSerializable {
      internal abstract string ClassName { get; }

      internal abstract int Version { get; }

      internal FXSerializable() {}

      internal FXSerializable(BinaryReaderEx br, List<string> classNames) {
        long position = br.Position;
        int num1 =
            (int) br.AssertInt16(
                (short) (classNames.IndexOf(this.ClassName) + 1));
        br.AssertInt32(this.Version);
        int num2 = br.ReadInt32();
        this.Deserialize(br, classNames);
        if (br.Position != position + (long) num2)
          throw new InvalidDataException(
              "Failed to read all object data (or read too much of it).");
      }

      protected internal abstract void Deserialize(
          BinaryReaderEx br,
          List<string> classNames);

      internal virtual void AddClassNames(List<string> classNames) {
        if (classNames.Contains(this.ClassName))
          return;
        classNames.Add(this.ClassName);
      }

      internal void Write(BinaryWriterEx bw, List<string> classNames) {
        long position = bw.Position;
        bw.WriteInt16((short) (classNames.IndexOf(this.ClassName) + 1));
        bw.WriteInt32(this.Version);
        bw.ReserveInt32(string.Format("{0:X}Length", (object) position));
        this.Serialize(bw, classNames);
        bw.FillInt32(string.Format("{0:X}Length", (object) position),
                     (int) (bw.Position - position));
      }

      protected internal abstract void Serialize(
          BinaryWriterEx bw,
          List<string> classNames);
    }

    public class FXEffect : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializableEffect"; }
      }

      internal override int Version {
        get { return 5; }
      }

      [XmlAttribute] public int ID { get; set; }

      public FFXDLSE.ParamList ParamList1 { get; set; }

      public FFXDLSE.ParamList ParamList2 { get; set; }

      public FFXDLSE.StateMap StateMap { get; set; }

      public FFXDLSE.ResourceSet ResourceSet { get; set; }

      public FXEffect() {
        this.ParamList1 = new FFXDLSE.ParamList();
        this.ParamList2 = new FFXDLSE.ParamList();
        this.StateMap = new FFXDLSE.StateMap();
        this.ResourceSet = new FFXDLSE.ResourceSet();
      }

      internal FXEffect(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        br.AssertInt32(new int[1]);
        this.ID = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(2);
        int num1 = (int) br.AssertInt16(new short[1]);
        int num2 = (int) br.AssertInt16((short) 2);
        br.AssertInt32(new int[1]);
        this.ParamList1 = new FFXDLSE.ParamList(br, classNames);
        this.ParamList2 = new FFXDLSE.ParamList(br, classNames);
        this.StateMap = new FFXDLSE.StateMap(br, classNames);
        this.ResourceSet = new FFXDLSE.ResourceSet(br, classNames);
        int num3 = (int) br.AssertByte(new byte[1]);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.DLVector.AddClassNames(classNames);
        this.ParamList1.AddClassNames(classNames);
        this.ParamList2.AddClassNames(classNames);
        this.StateMap.AddClassNames(classNames);
        this.ResourceSet.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(0);
        bw.WriteInt32(this.ID);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(2);
        bw.WriteInt16((short) 0);
        bw.WriteInt16((short) 2);
        bw.WriteInt32(0);
        this.ParamList1.Write(bw, classNames);
        this.ParamList2.Write(bw, classNames);
        this.StateMap.Write(bw, classNames);
        this.ResourceSet.Write(bw, classNames);
        bw.WriteByte((byte) 0);
      }
    }

    [XmlInclude(typeof(FFXDLSE.Param1))]
    [XmlInclude(typeof(FFXDLSE.Param2))]
    [XmlInclude(typeof(FFXDLSE.Param5))]
    [XmlInclude(typeof(FFXDLSE.Param6))]
    [XmlInclude(typeof(FFXDLSE.Param7))]
    [XmlInclude(typeof(FFXDLSE.Param9))]
    [XmlInclude(typeof(FFXDLSE.Param11))]
    [XmlInclude(typeof(FFXDLSE.Param12))]
    [XmlInclude(typeof(FFXDLSE.Param13))]
    [XmlInclude(typeof(FFXDLSE.Param15))]
    [XmlInclude(typeof(FFXDLSE.Param17))]
    [XmlInclude(typeof(FFXDLSE.Param18))]
    [XmlInclude(typeof(FFXDLSE.Param19))]
    [XmlInclude(typeof(FFXDLSE.Param20))]
    [XmlInclude(typeof(FFXDLSE.Param21))]
    [XmlInclude(typeof(FFXDLSE.Param37))]
    [XmlInclude(typeof(FFXDLSE.Param38))]
    [XmlInclude(typeof(FFXDLSE.Param40))]
    [XmlInclude(typeof(FFXDLSE.Param41))]
    [XmlInclude(typeof(FFXDLSE.Param44))]
    [XmlInclude(typeof(FFXDLSE.Param45))]
    [XmlInclude(typeof(FFXDLSE.Param46))]
    [XmlInclude(typeof(FFXDLSE.Param47))]
    [XmlInclude(typeof(FFXDLSE.Param59))]
    [XmlInclude(typeof(FFXDLSE.Param60))]
    [XmlInclude(typeof(FFXDLSE.Param66))]
    [XmlInclude(typeof(FFXDLSE.Param68))]
    [XmlInclude(typeof(FFXDLSE.Param69))]
    [XmlInclude(typeof(FFXDLSE.Param70))]
    [XmlInclude(typeof(FFXDLSE.Param71))]
    [XmlInclude(typeof(FFXDLSE.Param79))]
    [XmlInclude(typeof(FFXDLSE.Param81))]
    [XmlInclude(typeof(FFXDLSE.Param82))]
    [XmlInclude(typeof(FFXDLSE.Param83))]
    [XmlInclude(typeof(FFXDLSE.Param84))]
    [XmlInclude(typeof(FFXDLSE.Param85))]
    [XmlInclude(typeof(FFXDLSE.Param87))]
    public abstract class Param : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializableParam"; }
      }

      internal override int Version {
        get { return 2; }
      }

      internal abstract int Type { get; }

      public Param() {}

      internal Param(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        br.AssertInt32(this.Type);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(this.Type);
      }

      internal static FFXDLSE.Param Read(
          BinaryReaderEx br,
          List<string> classNames) {
        int int32 = br.GetInt32(br.Position + 10L);
        switch (int32) {
          case 1:
            return (FFXDLSE.Param) new FFXDLSE.Param1(br, classNames);
          case 2:
            return (FFXDLSE.Param) new FFXDLSE.Param2(br, classNames);
          case 5:
            return (FFXDLSE.Param) new FFXDLSE.Param5(br, classNames);
          case 6:
            return (FFXDLSE.Param) new FFXDLSE.Param6(br, classNames);
          case 7:
            return (FFXDLSE.Param) new FFXDLSE.Param7(br, classNames);
          case 9:
            return (FFXDLSE.Param) new FFXDLSE.Param9(br, classNames);
          case 11:
            return (FFXDLSE.Param) new FFXDLSE.Param11(br, classNames);
          case 12:
            return (FFXDLSE.Param) new FFXDLSE.Param12(br, classNames);
          case 13:
            return (FFXDLSE.Param) new FFXDLSE.Param13(br, classNames);
          case 15:
            return (FFXDLSE.Param) new FFXDLSE.Param15(br, classNames);
          case 17:
            return (FFXDLSE.Param) new FFXDLSE.Param17(br, classNames);
          case 18:
            return (FFXDLSE.Param) new FFXDLSE.Param18(br, classNames);
          case 19:
            return (FFXDLSE.Param) new FFXDLSE.Param19(br, classNames);
          case 20:
            return (FFXDLSE.Param) new FFXDLSE.Param20(br, classNames);
          case 21:
            return (FFXDLSE.Param) new FFXDLSE.Param21(br, classNames);
          case 37:
            return (FFXDLSE.Param) new FFXDLSE.Param37(br, classNames);
          case 38:
            return (FFXDLSE.Param) new FFXDLSE.Param38(br, classNames);
          case 40:
            return (FFXDLSE.Param) new FFXDLSE.Param40(br, classNames);
          case 41:
            return (FFXDLSE.Param) new FFXDLSE.Param41(br, classNames);
          case 44:
            return (FFXDLSE.Param) new FFXDLSE.Param44(br, classNames);
          case 45:
            return (FFXDLSE.Param) new FFXDLSE.Param45(br, classNames);
          case 46:
            return (FFXDLSE.Param) new FFXDLSE.Param46(br, classNames);
          case 47:
            return (FFXDLSE.Param) new FFXDLSE.Param47(br, classNames);
          case 59:
            return (FFXDLSE.Param) new FFXDLSE.Param59(br, classNames);
          case 60:
            return (FFXDLSE.Param) new FFXDLSE.Param60(br, classNames);
          case 66:
            return (FFXDLSE.Param) new FFXDLSE.Param66(br, classNames);
          case 68:
            return (FFXDLSE.Param) new FFXDLSE.Param68(br, classNames);
          case 69:
            return (FFXDLSE.Param) new FFXDLSE.Param69(br, classNames);
          case 70:
            return (FFXDLSE.Param) new FFXDLSE.Param70(br, classNames);
          case 71:
            return (FFXDLSE.Param) new FFXDLSE.Param71(br, classNames);
          case 79:
            return (FFXDLSE.Param) new FFXDLSE.Param79(br, classNames);
          case 81:
            return (FFXDLSE.Param) new FFXDLSE.Param81(br, classNames);
          case 82:
            return (FFXDLSE.Param) new FFXDLSE.Param82(br, classNames);
          case 83:
            return (FFXDLSE.Param) new FFXDLSE.Param83(br, classNames);
          case 84:
            return (FFXDLSE.Param) new FFXDLSE.Param84(br, classNames);
          case 85:
            return (FFXDLSE.Param) new FFXDLSE.Param85(br, classNames);
          case 87:
            return (FFXDLSE.Param) new FFXDLSE.Param87(br, classNames);
          default:
            throw new NotImplementedException(
                string.Format("Unimplemented param type: {0}", (object) int32));
        }
      }
    }

    public class Param1 : FFXDLSE.Param {
      internal override int Type {
        get { return 1; }
      }

      [XmlAttribute] public int Int { get; set; }

      public Param1() {}

      internal Param1(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Int = FFXDLSE.PrimitiveInt.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.PrimitiveInt.AddClassName(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        FFXDLSE.PrimitiveInt.Write(bw, classNames, this.Int);
      }
    }

    public class Param2 : FFXDLSE.Param {
      internal override int Type {
        get { return 2; }
      }

      public List<int> Ints { get; set; }

      public Param2() {
        this.Ints = new List<int>();
      }

      internal Param2(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.Ints = new List<int>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Ints.Add(FFXDLSE.PrimitiveInt.Read(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.PrimitiveInt.AddClassName(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Ints.Count);
        foreach (int num in this.Ints)
          FFXDLSE.PrimitiveInt.Write(bw, classNames, num);
      }
    }

    public class Param5 : FFXDLSE.Param {
      internal override int Type {
        get { return 5; }
      }

      public List<FFXDLSE.TickInt> TickInts { get; set; }

      public Param5() {
        this.TickInts = new List<FFXDLSE.TickInt>();
      }

      internal Param5(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickInts = new List<FFXDLSE.TickInt>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickInts.Add(new FFXDLSE.TickInt(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickInt tickInt in this.TickInts)
          tickInt.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickInts.Count);
        foreach (FFXDLSE.TickInt tickInt in this.TickInts)
          tickInt.Write(bw, classNames);
      }
    }

    public class Param6 : FFXDLSE.Param {
      internal override int Type {
        get { return 6; }
      }

      public List<FFXDLSE.TickInt> TickInts { get; set; }

      public Param6() {
        this.TickInts = new List<FFXDLSE.TickInt>();
      }

      internal Param6(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickInts = new List<FFXDLSE.TickInt>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickInts.Add(new FFXDLSE.TickInt(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickInt tickInt in this.TickInts)
          tickInt.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickInts.Count);
        foreach (FFXDLSE.TickInt tickInt in this.TickInts)
          tickInt.Write(bw, classNames);
      }
    }

    public class Param7 : FFXDLSE.Param {
      internal override int Type {
        get { return 7; }
      }

      [XmlAttribute] public float Float { get; set; }

      public Param7() {}

      internal Param7(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Float = FFXDLSE.PrimitiveFloat.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.PrimitiveFloat.AddClassName(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        FFXDLSE.PrimitiveFloat.Write(bw, classNames, this.Float);
      }
    }

    public class Param9 : FFXDLSE.Param {
      internal override int Type {
        get { return 9; }
      }

      public List<FFXDLSE.TickFloat> TickFloats { get; set; }

      public Param9() {
        this.TickFloats = new List<FFXDLSE.TickFloat>();
      }

      internal Param9(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickFloats = new List<FFXDLSE.TickFloat>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickFloats.Add(new FFXDLSE.TickFloat(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickFloat tickFloat in this.TickFloats)
          tickFloat.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickFloats.Count);
        foreach (FFXDLSE.TickFloat tickFloat in this.TickFloats)
          tickFloat.Write(bw, classNames);
      }
    }

    public class Param11 : FFXDLSE.Param {
      internal override int Type {
        get { return 11; }
      }

      public List<FFXDLSE.TickFloat> TickFloats { get; set; }

      public Param11() {
        this.TickFloats = new List<FFXDLSE.TickFloat>();
      }

      internal Param11(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickFloats = new List<FFXDLSE.TickFloat>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickFloats.Add(new FFXDLSE.TickFloat(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickFloat tickFloat in this.TickFloats)
          tickFloat.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickFloats.Count);
        foreach (FFXDLSE.TickFloat tickFloat in this.TickFloats)
          tickFloat.Write(bw, classNames);
      }
    }

    public class Param12 : FFXDLSE.Param {
      internal override int Type {
        get { return 12; }
      }

      public List<FFXDLSE.TickFloat> TickFloats { get; set; }

      public Param12() {
        this.TickFloats = new List<FFXDLSE.TickFloat>();
      }

      internal Param12(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickFloats = new List<FFXDLSE.TickFloat>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickFloats.Add(new FFXDLSE.TickFloat(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickFloat tickFloat in this.TickFloats)
          tickFloat.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickFloats.Count);
        foreach (FFXDLSE.TickFloat tickFloat in this.TickFloats)
          tickFloat.Write(bw, classNames);
      }
    }

    public class Param13 : FFXDLSE.Param {
      internal override int Type {
        get { return 13; }
      }

      public List<FFXDLSE.TickFloat3> TickFloat3s { get; set; }

      public Param13() {
        this.TickFloat3s = new List<FFXDLSE.TickFloat3>();
      }

      internal Param13(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickFloat3s = new List<FFXDLSE.TickFloat3>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickFloat3s.Add(new FFXDLSE.TickFloat3(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickFloat3 tickFloat3 in this.TickFloat3s)
          tickFloat3.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickFloat3s.Count);
        foreach (FFXDLSE.TickFloat3 tickFloat3 in this.TickFloat3s)
          tickFloat3.Write(bw, classNames);
      }
    }

    public class Param15 : FFXDLSE.Param {
      internal override int Type {
        get { return 15; }
      }

      public FFXDLSE.PrimitiveColor Color { get; set; }

      public Param15() {
        this.Color = new FFXDLSE.PrimitiveColor();
      }

      internal Param15(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Color = new FFXDLSE.PrimitiveColor(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.Color.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        this.Color.Write(bw, classNames);
      }
    }

    public class Param17 : FFXDLSE.Param {
      internal override int Type {
        get { return 17; }
      }

      public List<FFXDLSE.TickColor> TickColors { get; set; }

      public Param17() {
        this.TickColors = new List<FFXDLSE.TickColor>();
      }

      internal Param17(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickColors = new List<FFXDLSE.TickColor>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickColors.Add(new FFXDLSE.TickColor(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickColor tickColor in this.TickColors)
          tickColor.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickColors.Count);
        foreach (FFXDLSE.TickColor tickColor in this.TickColors)
          tickColor.Write(bw, classNames);
      }
    }

    public class Param18 : FFXDLSE.Param {
      internal override int Type {
        get { return 18; }
      }

      public List<FFXDLSE.TickColor> TickColors { get; set; }

      public Param18() {
        this.TickColors = new List<FFXDLSE.TickColor>();
      }

      internal Param18(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickColors = new List<FFXDLSE.TickColor>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickColors.Add(new FFXDLSE.TickColor(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickColor tickColor in this.TickColors)
          tickColor.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickColors.Count);
        foreach (FFXDLSE.TickColor tickColor in this.TickColors)
          tickColor.Write(bw, classNames);
      }
    }

    public class Param19 : FFXDLSE.Param {
      internal override int Type {
        get { return 19; }
      }

      public List<FFXDLSE.TickColor> TickColors { get; set; }

      public Param19() {
        this.TickColors = new List<FFXDLSE.TickColor>();
      }

      internal Param19(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickColors = new List<FFXDLSE.TickColor>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickColors.Add(new FFXDLSE.TickColor(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickColor tickColor in this.TickColors)
          tickColor.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickColors.Count);
        foreach (FFXDLSE.TickColor tickColor in this.TickColors)
          tickColor.Write(bw, classNames);
      }
    }

    public class Param20 : FFXDLSE.Param {
      internal override int Type {
        get { return 20; }
      }

      public List<FFXDLSE.TickColor> TickColors { get; set; }

      public Param20() {
        this.TickColors = new List<FFXDLSE.TickColor>();
      }

      internal Param20(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickColors = new List<FFXDLSE.TickColor>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickColors.Add(new FFXDLSE.TickColor(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickColor tickColor in this.TickColors)
          tickColor.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickColors.Count);
        foreach (FFXDLSE.TickColor tickColor in this.TickColors)
          tickColor.Write(bw, classNames);
      }
    }

    public class Param21 : FFXDLSE.Param {
      internal override int Type {
        get { return 21; }
      }

      public List<FFXDLSE.TickColor3> TickColor3s { get; set; }

      public Param21() {
        this.TickColor3s = new List<FFXDLSE.TickColor3>();
      }

      internal Param21(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        int capacity = br.ReadInt32();
        this.TickColor3s = new List<FFXDLSE.TickColor3>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.TickColor3s.Add(new FFXDLSE.TickColor3(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.TickColor3 tickColor3 in this.TickColor3s)
          tickColor3.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TickColor3s.Count);
        foreach (FFXDLSE.TickColor3 tickColor3 in this.TickColor3s)
          tickColor3.Write(bw, classNames);
      }
    }

    public class Param37 : FFXDLSE.Param {
      internal override int Type {
        get { return 37; }
      }

      [XmlAttribute] public int EffectID { get; set; }

      public FFXDLSE.ParamList ParamList { get; set; }

      public Param37() {
        this.ParamList = new FFXDLSE.ParamList();
      }

      internal Param37(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.EffectID = br.ReadInt32();
        this.ParamList = new FFXDLSE.ParamList(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.ParamList.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.EffectID);
        this.ParamList.Write(bw, classNames);
      }
    }

    public class Param38 : FFXDLSE.Param {
      internal override int Type {
        get { return 38; }
      }

      [XmlAttribute] public int ActionID { get; set; }

      public FFXDLSE.ParamList ParamList { get; set; }

      public Param38() {
        this.ParamList = new FFXDLSE.ParamList();
      }

      internal Param38(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.ActionID = br.ReadInt32();
        this.ParamList = new FFXDLSE.ParamList(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.ParamList.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.ActionID);
        this.ParamList.Write(bw, classNames);
      }
    }

    public class Param40 : FFXDLSE.Param {
      internal override int Type {
        get { return 40; }
      }

      [XmlAttribute] public int TextureID { get; set; }

      public Param40() {}

      internal Param40(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.TextureID = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.TextureID);
      }
    }

    public class Param41 : FFXDLSE.Param {
      internal override int Type {
        get { return 41; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      public Param41() {}

      internal Param41(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
      }
    }

    public class Param44 : FFXDLSE.Param {
      internal override int Type {
        get { return 44; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param44() {}

      internal Param44(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class Param45 : FFXDLSE.Param {
      internal override int Type {
        get { return 45; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param45() {}

      internal Param45(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class Param46 : FFXDLSE.Param {
      internal override int Type {
        get { return 46; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param46() {}

      internal Param46(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class Param47 : FFXDLSE.Param {
      internal override int Type {
        get { return 47; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param47() {}

      internal Param47(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class Param59 : FFXDLSE.Param {
      internal override int Type {
        get { return 59; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param59() {}

      internal Param59(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class Param60 : FFXDLSE.Param {
      internal override int Type {
        get { return 60; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param60() {}

      internal Param60(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class Param66 : FFXDLSE.Param {
      internal override int Type {
        get { return 66; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param66() {}

      internal Param66(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class Param68 : FFXDLSE.Param {
      internal override int Type {
        get { return 68; }
      }

      [XmlAttribute] public int SoundID { get; set; }

      public Param68() {}

      internal Param68(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.SoundID = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.SoundID);
      }
    }

    public class Param69 : FFXDLSE.Param {
      internal override int Type {
        get { return 69; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      public Param69() {}

      internal Param69(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
      }
    }

    public class Param70 : FFXDLSE.Param {
      internal override int Type {
        get { return 70; }
      }

      [XmlAttribute] public float Tick { get; set; }

      public Param70() {}

      internal Param70(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Tick = FFXDLSE.PrimitiveTick.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.PrimitiveTick.AddClassName(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        FFXDLSE.PrimitiveTick.Write(bw, classNames, this.Tick);
      }
    }

    public class Param71 : FFXDLSE.Param {
      internal override int Type {
        get { return 71; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param71() {}

      internal Param71(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class Param79 : FFXDLSE.Param {
      internal override int Type {
        get { return 79; }
      }

      [XmlAttribute] public int Int1 { get; set; }

      [XmlAttribute] public int Int2 { get; set; }

      public Param79() {}

      internal Param79(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Int1 = FFXDLSE.PrimitiveInt.Read(br, classNames);
        this.Int2 = FFXDLSE.PrimitiveInt.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.PrimitiveInt.AddClassName(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        FFXDLSE.PrimitiveInt.Write(bw, classNames, this.Int1);
        FFXDLSE.PrimitiveInt.Write(bw, classNames, this.Int2);
      }
    }

    public class Param81 : FFXDLSE.Param {
      internal override int Type {
        get { return 81; }
      }

      [XmlAttribute] public float Float1 { get; set; }

      [XmlAttribute] public float Float2 { get; set; }

      public Param81() {}

      internal Param81(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Float1 = FFXDLSE.PrimitiveFloat.Read(br, classNames);
        this.Float2 = FFXDLSE.PrimitiveFloat.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.PrimitiveFloat.AddClassName(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        FFXDLSE.PrimitiveFloat.Write(bw, classNames, this.Float1);
        FFXDLSE.PrimitiveFloat.Write(bw, classNames, this.Float2);
      }
    }

    public class Param82 : FFXDLSE.Param {
      internal override int Type {
        get { return 82; }
      }

      public FFXDLSE.Param Param { get; set; }

      public float Float { get; set; }

      public Param82() {
        this.Param = (FFXDLSE.Param) new FFXDLSE.Param1();
      }

      internal Param82(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Param = FFXDLSE.Param.Read(br, classNames);
        this.Float = FFXDLSE.PrimitiveFloat.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.Param.AddClassNames(classNames);
        FFXDLSE.PrimitiveFloat.AddClassName(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        this.Param.Write(bw, classNames);
        FFXDLSE.PrimitiveFloat.Write(bw, classNames, this.Float);
      }
    }

    public class Param83 : FFXDLSE.Param {
      internal override int Type {
        get { return 83; }
      }

      public FFXDLSE.PrimitiveColor Color1 { get; set; }

      public FFXDLSE.PrimitiveColor Color2 { get; set; }

      public Param83() {
        this.Color1 = new FFXDLSE.PrimitiveColor();
        this.Color2 = new FFXDLSE.PrimitiveColor();
      }

      internal Param83(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Color1 = new FFXDLSE.PrimitiveColor(br, classNames);
        this.Color2 = new FFXDLSE.PrimitiveColor(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.Color1.AddClassNames(classNames);
        this.Color2.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        this.Color1.Write(bw, classNames);
        this.Color2.Write(bw, classNames);
      }
    }

    public class Param84 : FFXDLSE.Param {
      internal override int Type {
        get { return 84; }
      }

      public FFXDLSE.Param Param { get; set; }

      public FFXDLSE.PrimitiveColor Color { get; set; }

      public Param84() {
        this.Param = (FFXDLSE.Param) new FFXDLSE.Param1();
        this.Color = new FFXDLSE.PrimitiveColor();
      }

      internal Param84(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Param = FFXDLSE.Param.Read(br, classNames);
        this.Color = new FFXDLSE.PrimitiveColor(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.Param.AddClassNames(classNames);
        this.Color.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        this.Param.Write(bw, classNames);
        this.Color.Write(bw, classNames);
      }
    }

    public class Param85 : FFXDLSE.Param {
      internal override int Type {
        get { return 85; }
      }

      [XmlAttribute] public float Tick1 { get; set; }

      [XmlAttribute] public float Tick2 { get; set; }

      public Param85() {}

      internal Param85(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Tick1 = FFXDLSE.PrimitiveTick.Read(br, classNames);
        this.Tick2 = FFXDLSE.PrimitiveTick.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.PrimitiveTick.AddClassName(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        FFXDLSE.PrimitiveTick.Write(bw, classNames, this.Tick1);
        FFXDLSE.PrimitiveTick.Write(bw, classNames, this.Tick2);
      }
    }

    public class Param87 : FFXDLSE.Param {
      internal override int Type {
        get { return 87; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      [XmlAttribute] public int ArgIndex { get; set; }

      public Param87() {}

      internal Param87(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        base.Deserialize(br, classNames);
        this.Unk04 = br.ReadInt32();
        this.ArgIndex = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        base.Serialize(bw, classNames);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.ArgIndex);
      }
    }

    public class TickInt {
      [XmlAttribute] public float Tick { get; set; }

      [XmlAttribute] public int Int { get; set; }

      public TickInt() {}

      public TickInt(float tick, int primInt) {
        this.Tick = tick;
        this.Int = primInt;
      }

      internal TickInt(BinaryReaderEx br, List<string> classNames) {
        this.Tick = FFXDLSE.PrimitiveTick.Read(br, classNames);
        this.Int = FFXDLSE.PrimitiveInt.Read(br, classNames);
      }

      internal void AddClassNames(List<string> classNames) {
        FFXDLSE.PrimitiveTick.AddClassName(classNames);
        FFXDLSE.PrimitiveInt.AddClassName(classNames);
      }

      internal void Write(BinaryWriterEx bw, List<string> classNames) {
        FFXDLSE.PrimitiveTick.Write(bw, classNames, this.Tick);
        FFXDLSE.PrimitiveInt.Write(bw, classNames, this.Int);
      }
    }

    public class TickFloat {
      [XmlAttribute] public float Tick { get; set; }

      [XmlAttribute] public float Float { get; set; }

      public TickFloat() {}

      public TickFloat(float tick, float primFloat) {
        this.Tick = tick;
        this.Float = primFloat;
      }

      internal TickFloat(BinaryReaderEx br, List<string> classNames) {
        this.Tick = FFXDLSE.PrimitiveTick.Read(br, classNames);
        this.Float = FFXDLSE.PrimitiveFloat.Read(br, classNames);
      }

      internal void AddClassNames(List<string> classNames) {
        FFXDLSE.PrimitiveTick.AddClassName(classNames);
        FFXDLSE.PrimitiveFloat.AddClassName(classNames);
      }

      internal void Write(BinaryWriterEx bw, List<string> classNames) {
        FFXDLSE.PrimitiveTick.Write(bw, classNames, this.Tick);
        FFXDLSE.PrimitiveFloat.Write(bw, classNames, this.Float);
      }
    }

    public class TickFloat3 {
      [XmlAttribute] public float Tick { get; set; }

      public float Float1 { get; set; }

      public float Float2 { get; set; }

      public float Float3 { get; set; }

      public TickFloat3() {}

      public TickFloat3(float tick, float float1, float float2, float float3) {
        this.Tick = tick;
        this.Float1 = float1;
        this.Float2 = float2;
        this.Float3 = float3;
      }

      internal TickFloat3(BinaryReaderEx br, List<string> classNames) {
        this.Tick = FFXDLSE.PrimitiveTick.Read(br, classNames);
        this.Float1 = FFXDLSE.PrimitiveFloat.Read(br, classNames);
        this.Float2 = FFXDLSE.PrimitiveFloat.Read(br, classNames);
        this.Float3 = FFXDLSE.PrimitiveFloat.Read(br, classNames);
      }

      internal void AddClassNames(List<string> classNames) {
        FFXDLSE.PrimitiveTick.AddClassName(classNames);
        FFXDLSE.PrimitiveFloat.AddClassName(classNames);
      }

      internal void Write(BinaryWriterEx bw, List<string> classNames) {
        FFXDLSE.PrimitiveTick.Write(bw, classNames, this.Tick);
        FFXDLSE.PrimitiveFloat.Write(bw, classNames, this.Float1);
        FFXDLSE.PrimitiveFloat.Write(bw, classNames, this.Float2);
        FFXDLSE.PrimitiveFloat.Write(bw, classNames, this.Float3);
      }
    }

    public class TickColor {
      [XmlAttribute] public float Tick { get; set; }

      public FFXDLSE.PrimitiveColor Color { get; set; }

      public TickColor() {
        this.Color = new FFXDLSE.PrimitiveColor();
      }

      public TickColor(float tick, FFXDLSE.PrimitiveColor color) {
        this.Tick = tick;
        this.Color = color;
      }

      internal TickColor(BinaryReaderEx br, List<string> classNames) {
        this.Tick = FFXDLSE.PrimitiveTick.Read(br, classNames);
        this.Color = new FFXDLSE.PrimitiveColor(br, classNames);
      }

      internal void AddClassNames(List<string> classNames) {
        FFXDLSE.PrimitiveTick.AddClassName(classNames);
        this.Color.AddClassNames(classNames);
      }

      internal void Write(BinaryWriterEx bw, List<string> classNames) {
        FFXDLSE.PrimitiveTick.Write(bw, classNames, this.Tick);
        this.Color.Write(bw, classNames);
      }
    }

    public class TickColor3 {
      [XmlAttribute] public float Tick { get; set; }

      public FFXDLSE.PrimitiveColor Color1 { get; set; }

      public FFXDLSE.PrimitiveColor Color2 { get; set; }

      public FFXDLSE.PrimitiveColor Color3 { get; set; }

      public TickColor3() {
        this.Color1 = new FFXDLSE.PrimitiveColor();
        this.Color2 = new FFXDLSE.PrimitiveColor();
        this.Color3 = new FFXDLSE.PrimitiveColor();
      }

      public TickColor3(
          float tick,
          FFXDLSE.PrimitiveColor color1,
          FFXDLSE.PrimitiveColor color2,
          FFXDLSE.PrimitiveColor color3) {
        this.Tick = tick;
        this.Color1 = color1;
        this.Color2 = color2;
        this.Color3 = color3;
      }

      internal TickColor3(BinaryReaderEx br, List<string> classNames) {
        this.Tick = FFXDLSE.PrimitiveTick.Read(br, classNames);
        this.Color1 = new FFXDLSE.PrimitiveColor(br, classNames);
        this.Color2 = new FFXDLSE.PrimitiveColor(br, classNames);
        this.Color3 = new FFXDLSE.PrimitiveColor(br, classNames);
      }

      internal void AddClassNames(List<string> classNames) {
        FFXDLSE.PrimitiveTick.AddClassName(classNames);
        this.Color1.AddClassNames(classNames);
        this.Color2.AddClassNames(classNames);
        this.Color3.AddClassNames(classNames);
      }

      internal void Write(BinaryWriterEx bw, List<string> classNames) {
        FFXDLSE.PrimitiveTick.Write(bw, classNames, this.Tick);
        this.Color1.Write(bw, classNames);
        this.Color2.Write(bw, classNames);
        this.Color3.Write(bw, classNames);
      }
    }

    public class ParamList : FFXDLSE.FXSerializable, IXmlSerializable {
      internal override string ClassName {
        get { return "FXSerializableParamList"; }
      }

      internal override int Version {
        get { return 2; }
      }

      [XmlAttribute] public int Unk04 { get; set; }

      public List<FFXDLSE.Param> Params { get; set; }

      public ParamList() {
        this.Params = new List<FFXDLSE.Param>();
      }

      internal ParamList(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        int capacity = br.ReadInt32();
        this.Unk04 = br.ReadInt32();
        this.Params = new List<FFXDLSE.Param>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Params.Add(FFXDLSE.Param.Read(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.FXSerializable fxSerializable in this.Params)
          fxSerializable.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(this.Params.Count);
        bw.WriteInt32(this.Unk04);
        foreach (FFXDLSE.FXSerializable fxSerializable in this.Params)
          fxSerializable.Write(bw, classNames);
      }

      XmlSchema IXmlSerializable.GetSchema() {
        return (XmlSchema) null;
      }

      void IXmlSerializable.ReadXml(XmlReader reader) {
        int content = (int) reader.MoveToContent();
        int num = reader.IsEmptyElement ? 1 : 0;
        this.Unk04 = int.Parse(reader.GetAttribute("Unk04"));
        reader.ReadStartElement();
        if (num != 0)
          return;
        while (reader.IsStartElement("Param"))
          this.Params.Add(
              (FFXDLSE.Param) FFXDLSE.ParamSerializer.Deserialize(reader));
        reader.ReadEndElement();
      }

      void IXmlSerializable.WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("Unk04", this.Unk04.ToString());
        for (int index = 0; index < this.Params.Count; ++index)
          FFXDLSE.ParamSerializer.Serialize(writer,
                                            (object) this.Params[index]);
      }
    }

    public class PrimitiveInt : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializablePrimitive<dl_int32>"; }
      }

      internal override int Version {
        get { return 1; }
      }

      [XmlAttribute] public int Value { get; set; }

      public PrimitiveInt() {}

      public PrimitiveInt(int value) {
        this.Value = value;
      }

      internal PrimitiveInt(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        this.Value = br.ReadInt32();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(this.Value);
      }

      internal static int Read(BinaryReaderEx br, List<string> classNames) {
        return new FFXDLSE.PrimitiveInt(br, classNames).Value;
      }

      internal static void AddClassName(List<string> classNames) {
        new FFXDLSE.PrimitiveInt().AddClassNames(classNames);
      }

      internal static void Write(
          BinaryWriterEx bw,
          List<string> classNames,
          int value) {
        new FFXDLSE.PrimitiveInt(value).Write(bw, classNames);
      }
    }

    public class PrimitiveFloat : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializablePrimitive<dl_float32>"; }
      }

      internal override int Version {
        get { return 1; }
      }

      [XmlAttribute] public float Value { get; set; }

      public PrimitiveFloat() {}

      public PrimitiveFloat(float value) {
        this.Value = value;
      }

      internal PrimitiveFloat(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        this.Value = br.ReadSingle();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteSingle(this.Value);
      }

      internal static float Read(BinaryReaderEx br, List<string> classNames) {
        return new FFXDLSE.PrimitiveFloat(br, classNames).Value;
      }

      internal static void AddClassName(List<string> classNames) {
        new FFXDLSE.PrimitiveFloat().AddClassNames(classNames);
      }

      internal static void Write(
          BinaryWriterEx bw,
          List<string> classNames,
          float value) {
        new FFXDLSE.PrimitiveFloat(value).Write(bw, classNames);
      }
    }

    public class PrimitiveTick : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializablePrimitive<FXTick>"; }
      }

      internal override int Version {
        get { return 1; }
      }

      [XmlAttribute] public float Value { get; set; }

      public PrimitiveTick() {}

      public PrimitiveTick(float value) {
        this.Value = value;
      }

      internal PrimitiveTick(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        this.Value = br.ReadSingle();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteSingle(this.Value);
      }

      internal static float Read(BinaryReaderEx br, List<string> classNames) {
        return new FFXDLSE.PrimitiveTick(br, classNames).Value;
      }

      internal static void AddClassName(List<string> classNames) {
        new FFXDLSE.PrimitiveTick().AddClassNames(classNames);
      }

      internal static void Write(
          BinaryWriterEx bw,
          List<string> classNames,
          float value) {
        new FFXDLSE.PrimitiveTick(value).Write(bw, classNames);
      }
    }

    public class PrimitiveColor : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializablePrimitive<FXColorRGBA>"; }
      }

      internal override int Version {
        get { return 1; }
      }

      public float R { get; set; }

      public float G { get; set; }

      public float B { get; set; }

      public float A { get; set; }

      public PrimitiveColor() {}

      public PrimitiveColor(float r, float g, float b, float a) {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
      }

      internal PrimitiveColor(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        this.R = br.ReadSingle();
        this.G = br.ReadSingle();
        this.B = br.ReadSingle();
        this.A = br.ReadSingle();
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteSingle(this.R);
        bw.WriteSingle(this.G);
        bw.WriteSingle(this.B);
        bw.WriteSingle(this.A);
      }
    }

    public class ResourceSet : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXResourceSet"; }
      }

      internal override int Version {
        get { return 1; }
      }

      public List<int> Vector1 { get; set; }

      public List<int> Vector2 { get; set; }

      public List<int> Vector3 { get; set; }

      public List<int> Vector4 { get; set; }

      public List<int> Vector5 { get; set; }

      public ResourceSet() {
        this.Vector1 = new List<int>();
        this.Vector2 = new List<int>();
        this.Vector3 = new List<int>();
        this.Vector4 = new List<int>();
        this.Vector5 = new List<int>();
      }

      internal ResourceSet(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        this.Vector1 = FFXDLSE.DLVector.Read(br, classNames);
        this.Vector2 = FFXDLSE.DLVector.Read(br, classNames);
        this.Vector3 = FFXDLSE.DLVector.Read(br, classNames);
        this.Vector4 = FFXDLSE.DLVector.Read(br, classNames);
        this.Vector5 = FFXDLSE.DLVector.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        FFXDLSE.DLVector.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        FFXDLSE.DLVector.Write(bw, classNames, this.Vector1);
        FFXDLSE.DLVector.Write(bw, classNames, this.Vector2);
        FFXDLSE.DLVector.Write(bw, classNames, this.Vector3);
        FFXDLSE.DLVector.Write(bw, classNames, this.Vector4);
        FFXDLSE.DLVector.Write(bw, classNames, this.Vector5);
      }
    }

    public class State : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializableState"; }
      }

      internal override int Version {
        get { return 1; }
      }

      public List<FFXDLSE.Action> Actions { get; set; }

      public List<FFXDLSE.Trigger> Triggers { get; set; }

      public State() {
        this.Actions = new List<FFXDLSE.Action>();
        this.Triggers = new List<FFXDLSE.Trigger>();
      }

      internal State(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        int capacity1 = br.ReadInt32();
        int capacity2 = br.ReadInt32();
        this.Actions = new List<FFXDLSE.Action>(capacity1);
        for (int index = 0; index < capacity1; ++index)
          this.Actions.Add(new FFXDLSE.Action(br, classNames));
        this.Triggers = new List<FFXDLSE.Trigger>(capacity2);
        for (int index = 0; index < capacity2; ++index)
          this.Triggers.Add(new FFXDLSE.Trigger(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.FXSerializable action in this.Actions)
          action.AddClassNames(classNames);
        foreach (FFXDLSE.FXSerializable trigger in this.Triggers)
          trigger.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(this.Actions.Count);
        bw.WriteInt32(this.Triggers.Count);
        foreach (FFXDLSE.FXSerializable action in this.Actions)
          action.Write(bw, classNames);
        foreach (FFXDLSE.FXSerializable trigger in this.Triggers)
          trigger.Write(bw, classNames);
      }
    }

    public class StateMap : FFXDLSE.FXSerializable, IXmlSerializable {
      internal override string ClassName {
        get { return "FXSerializableStateMap"; }
      }

      internal override int Version {
        get { return 1; }
      }

      public List<FFXDLSE.State> States { get; set; }

      public StateMap() {
        this.States = new List<FFXDLSE.State>();
      }

      internal StateMap(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        int capacity = br.ReadInt32();
        this.States = new List<FFXDLSE.State>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.States.Add(new FFXDLSE.State(br, classNames));
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        foreach (FFXDLSE.FXSerializable state in this.States)
          state.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(this.States.Count);
        foreach (FFXDLSE.FXSerializable state in this.States)
          state.Write(bw, classNames);
      }

      XmlSchema IXmlSerializable.GetSchema() {
        return (XmlSchema) null;
      }

      void IXmlSerializable.ReadXml(XmlReader reader) {
        int content = (int) reader.MoveToContent();
        int num = reader.IsEmptyElement ? 1 : 0;
        reader.ReadStartElement();
        if (num != 0)
          return;
        while (reader.IsStartElement("State"))
          this.States.Add(
              (FFXDLSE.State) FFXDLSE.StateSerializer.Deserialize(reader));
        reader.ReadEndElement();
      }

      void IXmlSerializable.WriteXml(XmlWriter writer) {
        for (int index = 0; index < this.States.Count; ++index) {
          writer.WriteComment(string.Format(" State {0} ", (object) index));
          FFXDLSE.StateSerializer.Serialize(writer,
                                            (object) this.States[index]);
        }
      }
    }

    public class Trigger : FFXDLSE.FXSerializable {
      internal override string ClassName {
        get { return "FXSerializableTrigger"; }
      }

      internal override int Version {
        get { return 1; }
      }

      [XmlAttribute] public int StateIndex { get; set; }

      public FFXDLSE.Evaluatable Evaluator { get; set; }

      public Trigger() {}

      internal Trigger(BinaryReaderEx br, List<string> classNames)
          : base(br, classNames) {}

      protected internal override void Deserialize(
          BinaryReaderEx br,
          List<string> classNames) {
        this.StateIndex = br.ReadInt32();
        this.Evaluator = FFXDLSE.Evaluatable.Read(br, classNames);
      }

      internal override void AddClassNames(List<string> classNames) {
        base.AddClassNames(classNames);
        this.Evaluator.AddClassNames(classNames);
      }

      protected internal override void Serialize(
          BinaryWriterEx bw,
          List<string> classNames) {
        bw.WriteInt32(this.StateIndex);
        this.Evaluator.Write(bw, classNames);
      }
    }
  }
}