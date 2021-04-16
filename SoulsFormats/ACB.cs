// Decompiled with JetBrains decompiler
// Type: SoulsFormats.ACB
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class ACB : SoulsFile<ACB> {
    public bool BigEndian { get; set; }

    public List<ACB.Asset> Assets { get; set; }

    protected override bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "ACB\0";
    }

    protected override void Read(BinaryReaderEx br) {
      this.BigEndian = (long) br.GetUInt32(12L) > br.Length;
      br.BigEndian = this.BigEndian;
      br.AssertASCII("ACB\0");
      int num1 = (int) br.AssertByte((byte) 2);
      int num2 = (int) br.AssertByte((byte) 1);
      int num3 = (int) br.AssertByte(new byte[1]);
      int num4 = (int) br.AssertByte(new byte[1]);
      int num5 = br.ReadInt32();
      br.ReadInt32();
      this.Assets = new List<ACB.Asset>(num5);
      foreach (int readInt32 in br.ReadInt32s(num5)) {
        br.Position = (long) readInt32;
        ACB.AssetType enum16 = br.GetEnum16<ACB.AssetType>(br.Position + 8L);
        switch (enum16) {
          case ACB.AssetType.PWV:
            this.Assets.Add((ACB.Asset) new ACB.Asset.PWV(br));
            break;
          case ACB.AssetType.General:
            this.Assets.Add((ACB.Asset) new ACB.Asset.General(br));
            break;
          case ACB.AssetType.Model:
            this.Assets.Add((ACB.Asset) new ACB.Asset.Model(br));
            break;
          case ACB.AssetType.Texture:
            this.Assets.Add((ACB.Asset) new ACB.Asset.Texture(br));
            break;
          case ACB.AssetType.GITexture:
            this.Assets.Add((ACB.Asset) new ACB.Asset.GITexture(br));
            break;
          case ACB.AssetType.Motion:
            this.Assets.Add((ACB.Asset) new ACB.Asset.Motion(br));
            break;
          default:
            throw new NotImplementedException(
                string.Format("Unsupported asset type: {0}", (object) enum16));
        }
      }
    }

    protected override void Write(BinaryWriterEx bw) {
      List<int> offsetIndex = new List<int>();
      SortedDictionary<int, List<int>> membersOffsetIndex =
          new SortedDictionary<int, List<int>>();
      bw.BigEndian = this.BigEndian;
      bw.WriteASCII("ACB\0", false);
      bw.WriteByte((byte) 2);
      bw.WriteByte((byte) 1);
      bw.WriteByte((byte) 0);
      bw.WriteByte((byte) 0);
      bw.WriteInt32(this.Assets.Count);
      bw.ReserveInt32("OffsetIndexOffset");
      for (int index = 0; index < this.Assets.Count; ++index) {
        offsetIndex.Add((int) bw.Position);
        bw.ReserveInt32(string.Format("AssetOffset{0}", (object) index));
      }
      for (int index = 0; index < this.Assets.Count; ++index) {
        bw.FillInt32(string.Format("AssetOffset{0}", (object) index),
                     (int) bw.Position);
        this.Assets[index].Write(bw, index, offsetIndex, membersOffsetIndex);
      }
      for (int index = 0; index < this.Assets.Count; ++index) {
        if (this.Assets[index] is ACB.Asset.Model asset)
          asset.WriteMembers(bw, index, offsetIndex, membersOffsetIndex);
      }
      for (int index = 0; index < this.Assets.Count; ++index)
        this.Assets[index].WritePaths(bw, index);
      for (int entryIndex = 0; entryIndex < this.Assets.Count; ++entryIndex) {
        if (this.Assets[entryIndex] is ACB.Asset.Model asset &&
            asset.Members != null) {
          for (int memberIndex = 0;
               memberIndex < asset.Members.Count;
               ++memberIndex)
            asset.Members[memberIndex].WriteText(bw, entryIndex, memberIndex);
        }
      }
      bw.Pad(4);
      bw.FillInt32("OffsetIndexOffset", (int) bw.Position);
      bw.WriteInt32s((IList<int>) offsetIndex);
      foreach (List<int> intList in membersOffsetIndex.Values)
        bw.WriteInt32s((IList<int>) intList);
    }

    public enum AssetType : ushort {
      PWV,
      General,
      Model,
      Texture,
      GITexture,
      Motion,
    }

    public abstract class Asset {
      public abstract ACB.AssetType Type { get; }

      public string AbsolutePath { get; set; }

      public string RelativePath { get; set; }

      internal Asset() {
        this.AbsolutePath = "";
        this.RelativePath = "";
      }

      internal Asset(BinaryReaderEx br) {
        int num1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int num3 = (int) br.AssertUInt16((ushort) this.Type);
        this.AbsolutePath = br.GetUTF16((long) num1);
        this.RelativePath = br.GetUTF16((long) num2);
      }

      internal virtual void Write(
          BinaryWriterEx bw,
          int index,
          List<int> offsetIndex,
          SortedDictionary<int, List<int>> membersOffsetIndex) {
        offsetIndex.Add((int) bw.Position);
        bw.ReserveInt32(string.Format("AbsolutePathOffset{0}", (object) index));
        offsetIndex.Add((int) bw.Position);
        bw.ReserveInt32(string.Format("RelativePathOffset{0}", (object) index));
        bw.WriteUInt16((ushort) this.Type);
      }

      internal void WritePaths(BinaryWriterEx bw, int index) {
        bw.FillInt32(string.Format("AbsolutePathOffset{0}", (object) index),
                     (int) bw.Position);
        bw.WriteUTF16(this.AbsolutePath, true);
        bw.FillInt32(string.Format("RelativePathOffset{0}", (object) index),
                     (int) bw.Position);
        bw.WriteUTF16(this.RelativePath, true);
      }

      public override string ToString() {
        return string.Format("{0}: {1} | {2}",
                             (object) this.Type,
                             (object) this.RelativePath,
                             (object) this.AbsolutePath);
      }

      public class PWV : ACB.Asset {
        public override ACB.AssetType Type {
          get { return ACB.AssetType.PWV; }
        }

        public PWV() {}

        internal PWV(BinaryReaderEx br)
            : base(br) {
          int num = (int) br.AssertInt16(new short[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(
            BinaryWriterEx bw,
            int index,
            List<int> offsetIndex,
            SortedDictionary<int, List<int>> membersOffsetIndex) {
          base.Write(bw, index, offsetIndex, membersOffsetIndex);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(0);
        }
      }

      public class General : ACB.Asset {
        public override ACB.AssetType Type {
          get { return ACB.AssetType.General; }
        }

        public General() {}

        internal General(BinaryReaderEx br)
            : base(br) {
          int num = (int) br.AssertInt16(new short[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(
            BinaryWriterEx bw,
            int index,
            List<int> offsetIndex,
            SortedDictionary<int, List<int>> membersOffsetIndex) {
          base.Write(bw, index, offsetIndex, membersOffsetIndex);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(0);
        }
      }

      public class Model : ACB.Asset {
        public override ACB.AssetType Type {
          get { return ACB.AssetType.Model; }
        }

        public short Unk0A { get; set; }

        public ACB.Asset.Model.MemberList Members { get; set; }

        public int Unk10 { get; set; }

        public short Unk1C { get; set; }

        public bool Reflectible { get; set; }

        public bool Unk1F { get; set; }

        public int Unk20 { get; set; }

        public byte Unk24 { get; set; }

        public bool DisableShadowSource { get; set; }

        public bool DisableShadowTarget { get; set; }

        public bool Unk27 { get; set; }

        public float Unk28 { get; set; }

        public bool Unk2C { get; set; }

        public bool FixToCamera { get; set; }

        public bool Unk2E { get; set; }

        public short Unk30 { get; set; }

        public short Unk32 { get; set; }

        public byte Unk34 { get; set; }

        public bool Unk35 { get; set; }

        public bool Unk36 { get; set; }

        public bool Unk37 { get; set; }

        public Model() {
          this.Reflectible = true;
        }

        internal Model(BinaryReaderEx br)
            : base(br) {
          this.Unk0A = br.ReadInt16();
          int num1 = br.ReadInt32();
          this.Unk10 = br.ReadInt32();
          br.AssertInt32(new int[1]);
          br.AssertInt32(new int[1]);
          this.Unk1C = br.ReadInt16();
          this.Reflectible = br.ReadBoolean();
          this.Unk1F = br.ReadBoolean();
          this.Unk20 = br.ReadInt32();
          this.Unk24 = br.ReadByte();
          this.DisableShadowSource = br.ReadBoolean();
          this.DisableShadowTarget = br.ReadBoolean();
          this.Unk27 = br.ReadBoolean();
          this.Unk28 = br.ReadSingle();
          this.Unk2C = br.ReadBoolean();
          this.FixToCamera = br.ReadBoolean();
          this.Unk2E = br.ReadBoolean();
          int num2 = (int) br.AssertByte(new byte[1]);
          this.Unk30 = br.ReadInt16();
          this.Unk32 = br.ReadInt16();
          this.Unk34 = br.ReadByte();
          this.Unk35 = br.ReadBoolean();
          this.Unk36 = br.ReadBoolean();
          this.Unk37 = br.ReadBoolean();
          br.AssertPattern(24, (byte) 0);
          if (num1 == 0)
            return;
          br.Position = (long) num1;
          this.Members = new ACB.Asset.Model.MemberList(br);
        }

        internal override void Write(
            BinaryWriterEx bw,
            int index,
            List<int> offsetIndex,
            SortedDictionary<int, List<int>> membersOffsetIndex) {
          base.Write(bw, index, offsetIndex, membersOffsetIndex);
          bw.WriteInt16(this.Unk0A);
          membersOffsetIndex[index] = new List<int>();
          if (this.Members != null)
            membersOffsetIndex[index].Add((int) bw.Position);
          bw.ReserveInt32(string.Format("MembersOffset{0}", (object) index));
          bw.WriteInt32(this.Unk10);
          bw.WriteInt32(0);
          bw.WriteInt32(0);
          bw.WriteInt16(this.Unk1C);
          bw.WriteBoolean(this.Reflectible);
          bw.WriteBoolean(this.Unk1F);
          bw.WriteInt32(this.Unk20);
          bw.WriteByte(this.Unk24);
          bw.WriteBoolean(this.DisableShadowSource);
          bw.WriteBoolean(this.DisableShadowTarget);
          bw.WriteBoolean(this.Unk27);
          bw.WriteSingle(this.Unk28);
          bw.WriteBoolean(this.Unk2C);
          bw.WriteBoolean(this.FixToCamera);
          bw.WriteBoolean(this.Unk2E);
          bw.WriteByte((byte) 0);
          bw.WriteInt16(this.Unk30);
          bw.WriteInt16(this.Unk32);
          bw.WriteByte(this.Unk34);
          bw.WriteBoolean(this.Unk35);
          bw.WriteBoolean(this.Unk36);
          bw.WriteBoolean(this.Unk37);
          bw.WritePattern(24, (byte) 0);
        }

        internal void WriteMembers(
            BinaryWriterEx bw,
            int index,
            List<int> offsetIndex,
            SortedDictionary<int, List<int>> membersOffsetIndex) {
          if (this.Members == null) {
            bw.FillInt32(string.Format("MembersOffset{0}", (object) index), 0);
          } else {
            bw.FillInt32(string.Format("MembersOffset{0}", (object) index),
                         (int) bw.Position);
            this.Members.Write(bw, index, offsetIndex, membersOffsetIndex);
          }
        }

        public class MemberList : List<ACB.Asset.Model.Member> {
          public short Unk00 { get; set; }

          public MemberList() {}

          public MemberList(int capacity)
              : base(capacity) {}

          public MemberList(IEnumerable<ACB.Asset.Model.Member> collection)
              : base(collection) {}

          internal MemberList(BinaryReaderEx br) {
            this.Unk00 = br.ReadInt16();
            short num1 = br.ReadInt16();
            int num2 = br.ReadInt32();
            br.StepIn((long) num2);
            this.Capacity = (int) num1;
            int[] numArray = br.ReadInt32s((int) num1);
            for (int index = 0; index < (int) num1; ++index) {
              br.Position = (long) numArray[index];
              this.Add(new ACB.Asset.Model.Member(br));
            }
            br.StepOut();
          }

          internal void Write(
              BinaryWriterEx bw,
              int index,
              List<int> offsetIndex,
              SortedDictionary<int, List<int>> membersOffsetIndex) {
            bw.WriteInt16(this.Unk00);
            bw.WriteInt16((short) this.Count);
            membersOffsetIndex[index].Add((int) bw.Position);
            bw.ReserveInt32(
                string.Format("MemberOffsetsOffset{0}", (object) index));
            bw.FillInt32(
                string.Format("MemberOffsetsOffset{0}", (object) index),
                (int) bw.Position);
            for (int index1 = 0; index1 < this.Count; ++index1) {
              membersOffsetIndex[index].Add((int) bw.Position);
              bw.ReserveInt32(string.Format("MemberOffset{0}:{1}",
                                            (object) index,
                                            (object) index1));
            }
            for (int memberIndex = 0; memberIndex < this.Count; ++memberIndex) {
              bw.FillInt32(
                  string.Format("MemberOffset{0}:{1}",
                                (object) index,
                                (object) memberIndex),
                  (int) bw.Position);
              this[memberIndex].Write(bw, index, memberIndex, offsetIndex);
            }
          }
        }

        public class Member {
          public string Text { get; set; }

          public int Unk04 { get; set; }

          public Member() {
            this.Text = "";
          }

          internal Member(BinaryReaderEx br) {
            int num = br.ReadInt32();
            this.Unk04 = br.ReadInt32();
            this.Text = br.GetUTF16((long) num);
          }

          internal void Write(
              BinaryWriterEx bw,
              int entryIndex,
              int memberIndex,
              List<int> offsetIndex) {
            offsetIndex.Add((int) bw.Position);
            bw.ReserveInt32(string.Format("MemberTextOffset{0}:{1}",
                                          (object) entryIndex,
                                          (object) memberIndex));
            bw.WriteInt32(this.Unk04);
          }

          internal void WriteText(
              BinaryWriterEx bw,
              int entryIndex,
              int memberIndex) {
            bw.FillInt32(string.Format("MemberTextOffset{0}:{1}",
                                       (object) entryIndex,
                                       (object) memberIndex),
                         (int) bw.Position);
            bw.WriteUTF16(this.Text, true);
          }
        }
      }

      public class Texture : ACB.Asset {
        public override ACB.AssetType Type {
          get { return ACB.AssetType.Texture; }
        }

        public Texture() {}

        internal Texture(BinaryReaderEx br)
            : base(br) {
          int num = (int) br.AssertInt16(new short[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(
            BinaryWriterEx bw,
            int index,
            List<int> offsetIndex,
            SortedDictionary<int, List<int>> membersOffsetIndex) {
          base.Write(bw, index, offsetIndex, membersOffsetIndex);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(0);
        }
      }

      public class GITexture : ACB.Asset {
        public override ACB.AssetType Type {
          get { return ACB.AssetType.GITexture; }
        }

        public int Unk10 { get; set; }

        public int Unk14 { get; set; }

        public GITexture() {}

        internal GITexture(BinaryReaderEx br)
            : base(br) {
          int num = (int) br.AssertInt16(new short[1]);
          br.AssertInt32(new int[1]);
          this.Unk10 = br.ReadInt32();
        }

        internal override void Write(
            BinaryWriterEx bw,
            int index,
            List<int> offsetIndex,
            SortedDictionary<int, List<int>> membersOffsetIndex) {
          base.Write(bw, index, offsetIndex, membersOffsetIndex);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(0);
          bw.WriteInt32(this.Unk10);
        }
      }

      public class Motion : ACB.Asset {
        public override ACB.AssetType Type {
          get { return ACB.AssetType.Motion; }
        }

        public Motion() {}

        internal Motion(BinaryReaderEx br)
            : base(br) {
          int num = (int) br.AssertInt16(new short[1]);
          br.AssertInt32(new int[1]);
        }

        internal override void Write(
            BinaryWriterEx bw,
            int index,
            List<int> offsetIndex,
            SortedDictionary<int, List<int>> membersOffsetIndex) {
          base.Write(bw, index, offsetIndex, membersOffsetIndex);
          bw.WriteInt16((short) 0);
          bw.WriteInt32(0);
        }
      }
    }
  }
}