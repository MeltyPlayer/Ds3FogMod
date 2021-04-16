// Decompiled with JetBrains decompiler
// Type: SoulsFormats.FXR3
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace SoulsFormats {
  [ComVisible(true)]
  public class FXR3 : SoulsFile<FXR3> {
    public FXR3.FXRVersion Version { get; set; }

    public int ID { get; set; }

    public FXR3.Section1 Section1Tree { get; set; }

    public FXR3.Section4 Section4Tree { get; set; }

    public List<int> Section12s { get; set; }

    public List<int> Section13s { get; set; }

    public FXR3() {
      this.Version = FXR3.FXRVersion.DarkSouls3;
      this.Section1Tree = new FXR3.Section1();
      this.Section4Tree = new FXR3.Section4();
      this.Section12s = new List<int>();
      this.Section13s = new List<int>();
    }

    protected override bool Is(BinaryReaderEx br) {
      if (br.Length < 8L)
        return false;
      string ascii = br.GetASCII(0L, 4);
      short int16 = br.GetInt16(6L);
      if (!(ascii == "FXR\0"))
        return false;
      return int16 == (short) 4 || int16 == (short) 5;
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      br.AssertASCII("FXR\0");
      int num1 = (int) br.AssertInt16(new short[1]);
      this.Version = br.ReadEnum16<FXR3.FXRVersion>();
      br.AssertInt32(1);
      this.ID = br.ReadInt32();
      int num2 = br.ReadInt32();
      br.AssertInt32(1);
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      int num3 = br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.ReadInt32();
      br.AssertInt32(1);
      br.AssertInt32(new int[1]);
      if (this.Version == FXR3.FXRVersion.Sekiro) {
        int num4 = br.ReadInt32();
        int count1 = br.ReadInt32();
        int num5 = br.ReadInt32();
        int count2 = br.ReadInt32();
        br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Section12s =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num4, count1));
        this.Section13s =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num5, count2));
      } else {
        this.Section12s = new List<int>();
        this.Section13s = new List<int>();
      }
      br.Position = (long) num2;
      this.Section1Tree = new FXR3.Section1(br);
      br.Position = (long) num3;
      this.Section4Tree = new FXR3.Section4(br);
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.WriteASCII("FXR\0", false);
      bw.WriteInt16((short) 0);
      bw.WriteUInt16((ushort) this.Version);
      bw.WriteInt32(1);
      bw.WriteInt32(this.ID);
      bw.ReserveInt32("Section1Offset");
      bw.WriteInt32(1);
      bw.ReserveInt32("Section2Offset");
      bw.WriteInt32(this.Section1Tree.Section2s.Count);
      bw.ReserveInt32("Section3Offset");
      bw.ReserveInt32("Section3Count");
      bw.ReserveInt32("Section4Offset");
      bw.ReserveInt32("Section4Count");
      bw.ReserveInt32("Section5Offset");
      bw.ReserveInt32("Section5Count");
      bw.ReserveInt32("Section6Offset");
      bw.ReserveInt32("Section6Count");
      bw.ReserveInt32("Section7Offset");
      bw.ReserveInt32("Section7Count");
      bw.ReserveInt32("Section8Offset");
      bw.ReserveInt32("Section8Count");
      bw.ReserveInt32("Section9Offset");
      bw.ReserveInt32("Section9Count");
      bw.ReserveInt32("Section10Offset");
      bw.ReserveInt32("Section10Count");
      bw.ReserveInt32("Section11Offset");
      bw.ReserveInt32("Section11Count");
      bw.WriteInt32(1);
      bw.WriteInt32(0);
      if (this.Version == FXR3.FXRVersion.Sekiro) {
        bw.ReserveInt32("Section12Offset");
        bw.WriteInt32(this.Section12s.Count);
        bw.ReserveInt32("Section13Offset");
        bw.WriteInt32(this.Section13s.Count);
        bw.ReserveInt32("Section14Offset");
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
      }
      bw.FillInt32("Section1Offset", (int) bw.Position);
      this.Section1Tree.Write(bw);
      bw.Pad(16);
      bw.FillInt32("Section2Offset", (int) bw.Position);
      this.Section1Tree.WriteSection2s(bw);
      bw.Pad(16);
      bw.FillInt32("Section3Offset", (int) bw.Position);
      List<FXR3.Section2> section2s = this.Section1Tree.Section2s;
      List<FXR3.Section3> section3s = new List<FXR3.Section3>();
      for (int index = 0; index < section2s.Count; ++index)
        section2s[index].WriteSection3s(bw, index, section3s);
      bw.FillInt32("Section3Count", section3s.Count);
      bw.Pad(16);
      bw.FillInt32("Section4Offset", (int) bw.Position);
      List<FXR3.Section4> section4s = new List<FXR3.Section4>();
      this.Section4Tree.Write(bw, section4s);
      this.Section4Tree.WriteSection4s(bw, section4s);
      bw.FillInt32("Section4Count", section4s.Count);
      bw.Pad(16);
      bw.FillInt32("Section5Offset", (int) bw.Position);
      int section5Count1 = 0;
      for (int index = 0; index < section4s.Count; ++index)
        section4s[index].WriteSection5s(bw, index, ref section5Count1);
      bw.FillInt32("Section5Count", section5Count1);
      bw.Pad(16);
      bw.FillInt32("Section6Offset", (int) bw.Position);
      int section5Count2 = 0;
      List<FXR3.FFXDrawEntityHost> section6s =
          new List<FXR3.FFXDrawEntityHost>();
      for (int index = 0; index < section4s.Count; ++index)
        section4s[index]
            .WriteSection6s(bw, index, ref section5Count2, section6s);
      bw.FillInt32("Section6Count", section6s.Count);
      bw.Pad(16);
      bw.FillInt32("Section7Offset", (int) bw.Position);
      List<FXR3.FFXProperty> section7s = new List<FXR3.FFXProperty>();
      for (int index = 0; index < section6s.Count; ++index)
        section6s[index].WriteSection7s(bw, index, section7s);
      bw.FillInt32("Section7Count", section7s.Count);
      bw.Pad(16);
      bw.FillInt32("Section8Offset", (int) bw.Position);
      List<FXR3.Section8> section8s = new List<FXR3.Section8>();
      for (int index = 0; index < section7s.Count; ++index)
        section7s[index].WriteSection8s(bw, index, section8s);
      bw.FillInt32("Section8Count", section8s.Count);
      bw.Pad(16);
      bw.FillInt32("Section9Offset", (int) bw.Position);
      List<FXR3.Section9> section9s = new List<FXR3.Section9>();
      for (int index = 0; index < section8s.Count; ++index)
        section8s[index].WriteSection9s(bw, index, section9s);
      bw.FillInt32("Section9Count", section9s.Count);
      bw.Pad(16);
      bw.FillInt32("Section10Offset", (int) bw.Position);
      List<FXR3.Section10> section10s = new List<FXR3.Section10>();
      for (int index = 0; index < section6s.Count; ++index)
        section6s[index].WriteSection10s(bw, index, section10s);
      bw.FillInt32("Section10Count", section10s.Count);
      bw.Pad(16);
      bw.FillInt32("Section11Offset", (int) bw.Position);
      int section11Count = 0;
      for (int index = 0; index < section3s.Count; ++index)
        section3s[index].WriteSection11s(bw, index, ref section11Count);
      for (int index = 0; index < section6s.Count; ++index)
        section6s[index].WriteSection11s(bw, index, ref section11Count);
      for (int index = 0; index < section7s.Count; ++index)
        section7s[index].WriteSection11s(bw, index, ref section11Count);
      for (int index = 0; index < section8s.Count; ++index)
        section8s[index].WriteSection11s(bw, index, ref section11Count);
      for (int index = 0; index < section9s.Count; ++index)
        section9s[index].WriteSection11s(bw, index, ref section11Count);
      for (int index = 0; index < section10s.Count; ++index)
        section10s[index].WriteSection11s(bw, index, ref section11Count);
      bw.FillInt32("Section11Count", section11Count);
      bw.Pad(16);
      if (this.Version != FXR3.FXRVersion.Sekiro)
        return;
      bw.FillInt32("Section12Offset", (int) bw.Position);
      bw.WriteInt32s((IList<int>) this.Section12s);
      bw.Pad(16);
      bw.FillInt32("Section13Offset", (int) bw.Position);
      bw.WriteInt32s((IList<int>) this.Section13s);
      bw.Pad(16);
      bw.FillInt32("Section14Offset", (int) bw.Position);
    }

    public enum FXRVersion : ushort {
      DarkSouls3 = 4,
      Sekiro = 5,
    }

    public class Section1 {
      public List<FXR3.Section2> Section2s { get; set; }

      public Section1() {
        this.Section2s = new List<FXR3.Section2>();
      }

      internal Section1(BinaryReaderEx br) {
        br.AssertInt32(new int[1]);
        int capacity = br.ReadInt32();
        int num = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.StepIn((long) num);
        this.Section2s = new List<FXR3.Section2>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Section2s.Add(new FXR3.Section2(br));
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteInt32(0);
        bw.WriteInt32(this.Section2s.Count);
        bw.ReserveInt32("Section1Section2sOffset");
        bw.WriteInt32(0);
      }

      internal void WriteSection2s(BinaryWriterEx bw) {
        bw.FillInt32("Section1Section2sOffset", (int) bw.Position);
        for (int index = 0; index < this.Section2s.Count; ++index)
          this.Section2s[index].Write(bw, index);
      }
    }

    public class Section2 {
      public List<FXR3.Section3> Section3s { get; set; }

      public Section2() {
        this.Section3s = new List<FXR3.Section3>();
      }

      internal Section2(BinaryReaderEx br) {
        br.AssertInt32(new int[1]);
        int capacity = br.ReadInt32();
        int num = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.StepIn((long) num);
        this.Section3s = new List<FXR3.Section3>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Section3s.Add(new FXR3.Section3(br));
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw, int index) {
        bw.WriteInt32(0);
        bw.WriteInt32(this.Section3s.Count);
        bw.ReserveInt32(
            string.Format("Section2Section3sOffset[{0}]", (object) index));
        bw.WriteInt32(0);
      }

      internal void WriteSection3s(
          BinaryWriterEx bw,
          int index,
          List<FXR3.Section3> section3s) {
        bw.FillInt32(
            string.Format("Section2Section3sOffset[{0}]", (object) index),
            (int) bw.Position);
        foreach (FXR3.Section3 section3 in this.Section3s)
          section3.Write(bw, section3s);
      }
    }

    public class Section3 {
      public int Unk08 { get; set; }

      public int Unk10 { get; set; }

      public int Unk38 { get; set; }

      public int Section11Data1 { get; set; }

      public int Section11Data2 { get; set; }

      public Section3() {}

      internal Section3(BinaryReaderEx br) {
        int num1 = (int) br.AssertInt16((short) 11);
        int num2 = (int) br.AssertByte(new byte[1]);
        int num3 = (int) br.AssertByte((byte) 1);
        br.AssertInt32(new int[1]);
        this.Unk08 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Unk10 = br.AssertInt32(16842748, 16842749);
        br.AssertInt32(new int[1]);
        br.AssertInt32(1);
        br.AssertInt32(new int[1]);
        int num4 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Unk38 = br.AssertInt32(16842748, 16842749);
        br.AssertInt32(new int[1]);
        br.AssertInt32(1);
        br.AssertInt32(new int[1]);
        int num5 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Section11Data1 = br.GetInt32((long) num4);
        this.Section11Data2 = br.GetInt32((long) num5);
      }

      internal void Write(BinaryWriterEx bw, List<FXR3.Section3> section3s) {
        int count = section3s.Count;
        bw.WriteInt16((short) 11);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 1);
        bw.WriteInt32(0);
        bw.WriteInt32(this.Unk08);
        bw.WriteInt32(0);
        bw.WriteInt32(this.Unk10);
        bw.WriteInt32(0);
        bw.WriteInt32(1);
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section3Section11Offset1[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(this.Unk38);
        bw.WriteInt32(0);
        bw.WriteInt32(1);
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section3Section11Offset2[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        section3s.Add(this);
      }

      internal void WriteSection11s(
          BinaryWriterEx bw,
          int index,
          ref int section11Count) {
        bw.FillInt32(
            string.Format("Section3Section11Offset1[{0}]", (object) index),
            (int) bw.Position);
        bw.WriteInt32(this.Section11Data1);
        bw.FillInt32(
            string.Format("Section3Section11Offset2[{0}]", (object) index),
            (int) bw.Position);
        bw.WriteInt32(this.Section11Data2);
        section11Count += 2;
      }
    }

    public class Section4 {
      [XmlAttribute] public short Unk00 { get; set; }

      public List<FXR3.Section4> Section4s { get; set; }

      public List<FXR3.Section5> Section5s { get; set; }

      public List<FXR3.FFXDrawEntityHost> Section6s { get; set; }

      public Section4() {
        this.Section4s = new List<FXR3.Section4>();
        this.Section5s = new List<FXR3.Section5>();
        this.Section6s = new List<FXR3.FFXDrawEntityHost>();
      }

      internal Section4(BinaryReaderEx br) {
        this.Unk00 = br.ReadInt16();
        int num1 = (int) br.AssertByte(new byte[1]);
        int num2 = (int) br.AssertByte((byte) 1);
        br.AssertInt32(new int[1]);
        int capacity1 = br.ReadInt32();
        int capacity2 = br.ReadInt32();
        int capacity3 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num3 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num4 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num5 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.StepIn((long) num5);
        this.Section4s = new List<FXR3.Section4>(capacity3);
        for (int index = 0; index < capacity3; ++index)
          this.Section4s.Add(new FXR3.Section4(br));
        br.StepOut();
        br.StepIn((long) num3);
        this.Section5s = new List<FXR3.Section5>(capacity1);
        for (int index = 0; index < capacity1; ++index)
          this.Section5s.Add(new FXR3.Section5(br));
        br.StepOut();
        br.StepIn((long) num4);
        this.Section6s = new List<FXR3.FFXDrawEntityHost>(capacity2);
        for (int index = 0; index < capacity2; ++index)
          this.Section6s.Add(new FXR3.FFXDrawEntityHost(br));
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw, List<FXR3.Section4> section4s) {
        int count = section4s.Count;
        bw.WriteInt16(this.Unk00);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 1);
        bw.WriteInt32(0);
        bw.WriteInt32(this.Section5s.Count);
        bw.WriteInt32(this.Section6s.Count);
        bw.WriteInt32(this.Section4s.Count);
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section4Section5sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section4Section6sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section4Section4sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        section4s.Add(this);
      }

      internal void WriteSection4s(
          BinaryWriterEx bw,
          List<FXR3.Section4> section4s) {
        int num = section4s.IndexOf(this);
        if (this.Section4s.Count == 0) {
          bw.FillInt32(
              string.Format("Section4Section4sOffset[{0}]", (object) num),
              0);
        } else {
          bw.FillInt32(
              string.Format("Section4Section4sOffset[{0}]", (object) num),
              (int) bw.Position);
          foreach (FXR3.Section4 section4 in this.Section4s)
            section4.Write(bw, section4s);
          foreach (FXR3.Section4 section4 in this.Section4s)
            section4.WriteSection4s(bw, section4s);
        }
      }

      internal void WriteSection5s(
          BinaryWriterEx bw,
          int index,
          ref int section5Count) {
        if (this.Section5s.Count == 0) {
          bw.FillInt32(
              string.Format("Section4Section5sOffset[{0}]", (object) index),
              0);
        } else {
          bw.FillInt32(
              string.Format("Section4Section5sOffset[{0}]", (object) index),
              (int) bw.Position);
          for (int index1 = 0; index1 < this.Section5s.Count; ++index1)
            this.Section5s[index1].Write(bw, section5Count + index1);
          section5Count += this.Section5s.Count;
        }
      }

      internal void WriteSection6s(
          BinaryWriterEx bw,
          int index,
          ref int section5Count,
          List<FXR3.FFXDrawEntityHost> section6s) {
        bw.FillInt32(
            string.Format("Section4Section6sOffset[{0}]", (object) index),
            (int) bw.Position);
        foreach (FXR3.FFXDrawEntityHost section6 in this.Section6s)
          section6.Write(bw, section6s);
        for (int index1 = 0; index1 < this.Section5s.Count; ++index1)
          this.Section5s[index1]
              .WriteSection6s(bw, section5Count + index1, section6s);
        section5Count += this.Section5s.Count;
      }
    }

    public class Section5 {
      [XmlAttribute] public short Unk00 { get; set; }

      public List<FXR3.FFXDrawEntityHost> Section6s { get; set; }

      public Section5() {
        this.Section6s = new List<FXR3.FFXDrawEntityHost>();
      }

      internal Section5(BinaryReaderEx br) {
        this.Unk00 = br.ReadInt16();
        int num1 = (int) br.AssertByte(new byte[1]);
        int num2 = (int) br.AssertByte((byte) 1);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        int capacity = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        int num3 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.StepIn((long) num3);
        this.Section6s = new List<FXR3.FFXDrawEntityHost>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Section6s.Add(new FXR3.FFXDrawEntityHost(br));
        br.StepOut();
      }

      internal void Write(BinaryWriterEx bw, int index) {
        bw.WriteInt16(this.Unk00);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 1);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(this.Section6s.Count);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section5Section6sOffset[{0}]", (object) index));
        bw.WriteInt32(0);
      }

      internal void WriteSection6s(
          BinaryWriterEx bw,
          int index,
          List<FXR3.FFXDrawEntityHost> section6s) {
        bw.FillInt32(
            string.Format("Section5Section6sOffset[{0}]", (object) index),
            (int) bw.Position);
        foreach (FXR3.FFXDrawEntityHost section6 in this.Section6s)
          section6.Write(bw, section6s);
      }
    }

    public class FFXDrawEntityHost {
      [XmlAttribute] public short Unk00 { get; set; }

      public bool Unk02 { get; set; }

      public bool Unk03 { get; set; }

      public int Unk04 { get; set; }

      public List<FXR3.FFXProperty> Properties1 { get; set; }

      public List<FXR3.FFXProperty> Properties2 { get; set; }

      public List<FXR3.Section10> Section10s { get; set; }

      public List<int> Section11s1 { get; set; }

      public List<int> Section11s2 { get; set; }

      public FFXDrawEntityHost() {
        this.Properties1 = new List<FXR3.FFXProperty>();
        this.Properties2 = new List<FXR3.FFXProperty>();
        this.Section10s = new List<FXR3.Section10>();
        this.Section11s1 = new List<int>();
        this.Section11s2 = new List<int>();
      }

      internal FFXDrawEntityHost(BinaryReaderEx br) {
        this.Unk00 = br.ReadInt16();
        this.Unk02 = br.ReadBoolean();
        this.Unk03 = br.ReadBoolean();
        this.Unk04 = br.ReadInt32();
        int count1 = br.ReadInt32();
        int capacity1 = br.ReadInt32();
        int capacity2 = br.ReadInt32();
        int count2 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int capacity3 = br.ReadInt32();
        int num1 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num2 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num3 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        br.StepIn((long) num3);
        this.Properties1 = new List<FXR3.FFXProperty>(capacity2);
        for (int index = 0; index < capacity2; ++index)
          this.Properties1.Add(new FXR3.FFXProperty(br));
        this.Properties2 = new List<FXR3.FFXProperty>(capacity3);
        for (int index = 0; index < capacity3; ++index)
          this.Properties2.Add(new FXR3.FFXProperty(br));
        br.StepOut();
        br.StepIn((long) num2);
        this.Section10s = new List<FXR3.Section10>(capacity1);
        for (int index = 0; index < capacity1; ++index)
          this.Section10s.Add(new FXR3.Section10(br));
        br.StepOut();
        br.StepIn((long) num1);
        this.Section11s1 =
            new List<int>((IEnumerable<int>) br.ReadInt32s(count1));
        this.Section11s2 =
            new List<int>((IEnumerable<int>) br.ReadInt32s(count2));
        br.StepOut();
      }

      internal void Write(
          BinaryWriterEx bw,
          List<FXR3.FFXDrawEntityHost> section6s) {
        int count = section6s.Count;
        bw.WriteInt16(this.Unk00);
        bw.WriteBoolean(this.Unk02);
        bw.WriteBoolean(this.Unk03);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.Section11s1.Count);
        bw.WriteInt32(this.Section10s.Count);
        bw.WriteInt32(this.Properties1.Count);
        bw.WriteInt32(this.Section11s2.Count);
        bw.WriteInt32(0);
        bw.WriteInt32(this.Properties2.Count);
        bw.ReserveInt32(
            string.Format("Section6Section11sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section6Section10sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section6Section7sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
        section6s.Add(this);
      }

      internal void WriteSection7s(
          BinaryWriterEx bw,
          int index,
          List<FXR3.FFXProperty> section7s) {
        bw.FillInt32(
            string.Format("Section6Section7sOffset[{0}]", (object) index),
            (int) bw.Position);
        foreach (FXR3.FFXProperty ffxProperty in this.Properties1)
          ffxProperty.Write(bw, section7s);
        foreach (FXR3.FFXProperty ffxProperty in this.Properties2)
          ffxProperty.Write(bw, section7s);
      }

      internal void WriteSection10s(
          BinaryWriterEx bw,
          int index,
          List<FXR3.Section10> section10s) {
        bw.FillInt32(
            string.Format("Section6Section10sOffset[{0}]", (object) index),
            (int) bw.Position);
        foreach (FXR3.Section10 section10 in this.Section10s)
          section10.Write(bw, section10s);
      }

      internal void WriteSection11s(
          BinaryWriterEx bw,
          int index,
          ref int section11Count) {
        if (this.Section11s1.Count == 0 && this.Section11s2.Count == 0) {
          bw.FillInt32(
              string.Format("Section6Section11sOffset[{0}]", (object) index),
              0);
        } else {
          bw.FillInt32(
              string.Format("Section6Section11sOffset[{0}]", (object) index),
              (int) bw.Position);
          bw.WriteInt32s((IList<int>) this.Section11s1);
          bw.WriteInt32s((IList<int>) this.Section11s2);
          section11Count += this.Section11s1.Count + this.Section11s2.Count;
        }
      }
    }

    public class FFXProperty {
      [XmlAttribute] public short Unk00 { get; set; }

      public int Unk04 { get; set; }

      public List<FXR3.Section8> Section8s { get; set; }

      public List<int> Section11s { get; set; }

      public FFXProperty() {
        this.Section8s = new List<FXR3.Section8>();
        this.Section11s = new List<int>();
      }

      internal FFXProperty(BinaryReaderEx br) {
        this.Unk00 = br.ReadInt16();
        int num1 = (int) br.AssertByte(new byte[1]);
        int num2 = (int) br.AssertByte((byte) 1);
        this.Unk04 = br.ReadInt32();
        int count = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num3 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num4 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int capacity = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.StepIn((long) num4);
        this.Section8s = new List<FXR3.Section8>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Section8s.Add(new FXR3.Section8(br));
        br.StepOut();
        this.Section11s =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num3, count));
      }

      internal void Write(BinaryWriterEx bw, List<FXR3.FFXProperty> section7s) {
        int count = section7s.Count;
        bw.WriteInt16(this.Unk00);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 1);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.Section11s.Count);
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section7Section11sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section7Section8sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.WriteInt32(this.Section8s.Count);
        bw.WriteInt32(0);
        section7s.Add(this);
      }

      internal void WriteSection8s(
          BinaryWriterEx bw,
          int index,
          List<FXR3.Section8> section8s) {
        bw.FillInt32(
            string.Format("Section7Section8sOffset[{0}]", (object) index),
            (int) bw.Position);
        foreach (FXR3.Section8 section8 in this.Section8s)
          section8.Write(bw, section8s);
      }

      internal void WriteSection11s(
          BinaryWriterEx bw,
          int index,
          ref int section11Count) {
        if (this.Section11s.Count == 0) {
          bw.FillInt32(
              string.Format("Section7Section11sOffset[{0}]", (object) index),
              0);
        } else {
          bw.FillInt32(
              string.Format("Section7Section11sOffset[{0}]", (object) index),
              (int) bw.Position);
          bw.WriteInt32s((IList<int>) this.Section11s);
          section11Count += this.Section11s.Count;
        }
      }
    }

    public class Section8 {
      [XmlAttribute] public short Unk00 { get; set; }

      public int Unk04 { get; set; }

      public List<FXR3.Section9> Section9s { get; set; }

      public List<int> Section11s { get; set; }

      public Section8() {
        this.Section9s = new List<FXR3.Section9>();
        this.Section11s = new List<int>();
      }

      internal Section8(BinaryReaderEx br) {
        this.Unk00 = br.ReadInt16();
        int num1 = (int) br.AssertByte(new byte[1]);
        int num2 = (int) br.AssertByte((byte) 1);
        this.Unk04 = br.ReadInt32();
        int count = br.ReadInt32();
        int capacity = br.ReadInt32();
        int num3 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num4 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.StepIn((long) num4);
        this.Section9s = new List<FXR3.Section9>(capacity);
        for (int index = 0; index < capacity; ++index)
          this.Section9s.Add(new FXR3.Section9(br));
        br.StepOut();
        this.Section11s =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num3, count));
      }

      internal void Write(BinaryWriterEx bw, List<FXR3.Section8> section8s) {
        int count = section8s.Count;
        bw.WriteInt16(this.Unk00);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 1);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.Section11s.Count);
        bw.WriteInt32(this.Section9s.Count);
        bw.ReserveInt32(
            string.Format("Section8Section11sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section8Section9sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        section8s.Add(this);
      }

      internal void WriteSection9s(
          BinaryWriterEx bw,
          int index,
          List<FXR3.Section9> section9s) {
        bw.FillInt32(
            string.Format("Section8Section9sOffset[{0}]", (object) index),
            (int) bw.Position);
        foreach (FXR3.Section9 section9 in this.Section9s)
          section9.Write(bw, section9s);
      }

      internal void WriteSection11s(
          BinaryWriterEx bw,
          int index,
          ref int section11Count) {
        bw.FillInt32(
            string.Format("Section8Section11sOffset[{0}]", (object) index),
            (int) bw.Position);
        bw.WriteInt32s((IList<int>) this.Section11s);
        section11Count += this.Section11s.Count;
      }
    }

    public class Section9 {
      public int Unk04 { get; set; }

      public List<int> Section11s { get; set; }

      public Section9() {
        this.Section11s = new List<int>();
      }

      internal Section9(BinaryReaderEx br) {
        int num1 = (int) br.AssertInt16((short) 48);
        int num2 = (int) br.AssertByte(new byte[1]);
        int num3 = (int) br.AssertByte((byte) 1);
        this.Unk04 = br.ReadInt32();
        int count = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int num4 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Section11s =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num4, count));
      }

      internal void Write(BinaryWriterEx bw, List<FXR3.Section9> section9s) {
        int count = section9s.Count;
        bw.WriteInt16((short) 48);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 1);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.Section11s.Count);
        bw.WriteInt32(0);
        bw.ReserveInt32(
            string.Format("Section9Section11sOffset[{0}]", (object) count));
        bw.WriteInt32(0);
        section9s.Add(this);
      }

      internal void WriteSection11s(
          BinaryWriterEx bw,
          int index,
          ref int section11Count) {
        bw.FillInt32(
            string.Format("Section9Section11sOffset[{0}]", (object) index),
            (int) bw.Position);
        bw.WriteInt32s((IList<int>) this.Section11s);
        section11Count += this.Section11s.Count;
      }
    }

    public class Section10 {
      public List<int> Section11s { get; set; }

      public Section10() {
        this.Section11s = new List<int>();
      }

      internal Section10(BinaryReaderEx br) {
        int num = br.ReadInt32();
        br.AssertInt32(new int[1]);
        int count = br.ReadInt32();
        br.AssertInt32(new int[1]);
        this.Section11s =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num, count));
      }

      internal void Write(BinaryWriterEx bw, List<FXR3.Section10> section10s) {
        int count = section10s.Count;
        bw.ReserveInt32(string.Format("Section10Section11sOffset[{0}]",
                                      (object) count));
        bw.WriteInt32(0);
        bw.WriteInt32(this.Section11s.Count);
        bw.WriteInt32(0);
        section10s.Add(this);
      }

      internal void WriteSection11s(
          BinaryWriterEx bw,
          int index,
          ref int section11Count) {
        bw.FillInt32(
            string.Format("Section10Section11sOffset[{0}]", (object) index),
            (int) bw.Position);
        bw.WriteInt32s((IList<int>) this.Section11s);
        section11Count += this.Section11s.Count;
      }
    }
  }
}