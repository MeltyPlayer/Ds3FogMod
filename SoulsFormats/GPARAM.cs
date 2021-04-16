// Decompiled with JetBrains decompiler
// Type: SoulsFormats.GPARAM
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class GPARAM : SoulsFile<GPARAM> {
    public GPARAM.GPGame Game;
    public bool Unk0D;
    public int Unk14;
    public float Unk50;
    public List<GPARAM.Group> Groups;
    public byte[] UnkBlock2;
    public List<GPARAM.Unk3> Unk3s;

    public GPARAM() {
      this.Game = GPARAM.GPGame.Sekiro;
      this.Groups = new List<GPARAM.Group>();
      this.UnkBlock2 = new byte[0];
      this.Unk3s = new List<GPARAM.Unk3>();
    }

    protected override bool Is(BinaryReaderEx br) {
      if (br.Length < 4L)
        return false;
      string ascii = br.GetASCII(0L, 4);
      return ascii == "filt" || ascii == "f\0i\0";
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      if (br.AssertASCII("filt", "f\0i\0") == "f\0i\0")
        br.AssertASCII("l\0t\0");
      this.Game = br.ReadEnum32<GPARAM.GPGame>();
      int num1 = (int) br.AssertByte(new byte[1]);
      this.Unk0D = br.ReadBoolean();
      int num2 = (int) br.AssertInt16(new short[1]);
      int num3 = br.ReadInt32();
      this.Unk14 = br.ReadInt32();
      br.AssertInt32(64, 80, 84);
      GPARAM.Offsets offsets = new GPARAM.Offsets();
      offsets.GroupHeaders = br.ReadInt32();
      offsets.ParamHeaderOffsets = br.ReadInt32();
      offsets.ParamHeaders = br.ReadInt32();
      offsets.Values = br.ReadInt32();
      offsets.ValueIDs = br.ReadInt32();
      offsets.Unk2 = br.ReadInt32();
      int capacity = br.ReadInt32();
      offsets.Unk3 = br.ReadInt32();
      offsets.Unk3ValueIDs = br.ReadInt32();
      br.AssertInt32(new int[1]);
      if (this.Game == GPARAM.GPGame.DarkSouls3 ||
          this.Game == GPARAM.GPGame.Sekiro) {
        offsets.CommentOffsetsOffsets = br.ReadInt32();
        offsets.CommentOffsets = br.ReadInt32();
        offsets.Comments = br.ReadInt32();
      }
      if (this.Game == GPARAM.GPGame.Sekiro)
        this.Unk50 = br.ReadSingle();
      this.Groups = new List<GPARAM.Group>(num3);
      for (int index = 0; index < num3; ++index)
        this.Groups.Add(new GPARAM.Group(br, this.Game, index, offsets));
      this.UnkBlock2 =
          br.GetBytes((long) offsets.Unk2, offsets.Unk3 - offsets.Unk2);
      br.Position = (long) offsets.Unk3;
      this.Unk3s = new List<GPARAM.Unk3>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Unk3s.Add(new GPARAM.Unk3(br, this.Game, offsets));
      if (this.Game != GPARAM.GPGame.DarkSouls3 &&
          this.Game != GPARAM.GPGame.Sekiro)
        return;
      int[] int32s = br.GetInt32s((long) offsets.CommentOffsetsOffsets, num3);
      int num4 = offsets.Comments - offsets.CommentOffsets;
      for (int index1 = 0; index1 < num3; ++index1) {
        int num5 = index1 != num3 - 1
                       ? (int32s[index1 + 1] - int32s[index1]) / 4
                       : (num4 - int32s[index1]) / 4;
        br.Position = (long) (offsets.CommentOffsets + int32s[index1]);
        for (int index2 = 0; index2 < num5; ++index2) {
          int num6 = br.ReadInt32();
          string utF16 = br.GetUTF16((long) (offsets.Comments + num6));
          this.Groups[index1].Comments.Add(utF16);
        }
      }
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = false;
      if (this.Game == GPARAM.GPGame.DarkSouls2)
        bw.WriteASCII("filt", false);
      else
        bw.WriteUTF16("filt", false);
      bw.WriteUInt32((uint) this.Game);
      bw.WriteByte((byte) 0);
      bw.WriteBoolean(this.Unk0D);
      bw.WriteInt16((short) 0);
      bw.WriteInt32(this.Groups.Count);
      bw.WriteInt32(this.Unk14);
      bw.ReserveInt32("HeaderSize");
      bw.ReserveInt32("GroupHeadersOffset");
      bw.ReserveInt32("ParamHeaderOffsetsOffset");
      bw.ReserveInt32("ParamHeadersOffset");
      bw.ReserveInt32("ValuesOffset");
      bw.ReserveInt32("ValueIDsOffset");
      bw.ReserveInt32("UnkOffset2");
      bw.WriteInt32(this.Unk3s.Count);
      bw.ReserveInt32("UnkOffset3");
      bw.ReserveInt32("Unk3ValuesOffset");
      bw.WriteInt32(0);
      if (this.Game == GPARAM.GPGame.DarkSouls3 ||
          this.Game == GPARAM.GPGame.Sekiro) {
        bw.ReserveInt32("CommentOffsetsOffsetsOffset");
        bw.ReserveInt32("CommentOffsetsOffset");
        bw.ReserveInt32("CommentsOffset");
      }
      if (this.Game == GPARAM.GPGame.Sekiro)
        bw.WriteSingle(this.Unk50);
      bw.FillInt32("HeaderSize", (int) bw.Position);
      for (int groupIndex = 0; groupIndex < this.Groups.Count; ++groupIndex)
        this.Groups[groupIndex].WriteHeaderOffset(bw, groupIndex);
      int position1 = (int) bw.Position;
      bw.FillInt32("GroupHeadersOffset", position1);
      for (int groupIndex = 0; groupIndex < this.Groups.Count; ++groupIndex)
        this.Groups[groupIndex]
            .WriteHeader(bw, this.Game, groupIndex, position1);
      int position2 = (int) bw.Position;
      bw.FillInt32("ParamHeaderOffsetsOffset", position2);
      for (int groupIndex = 0; groupIndex < this.Groups.Count; ++groupIndex)
        this.Groups[groupIndex]
            .WriteParamHeaderOffsets(bw, groupIndex, position2);
      int position3 = (int) bw.Position;
      bw.FillInt32("ParamHeadersOffset", position3);
      for (int groupindex = 0; groupindex < this.Groups.Count; ++groupindex)
        this.Groups[groupindex]
            .WriteParamHeaders(bw, this.Game, groupindex, position3);
      int position4 = (int) bw.Position;
      bw.FillInt32("ValuesOffset", position4);
      for (int groupindex = 0; groupindex < this.Groups.Count; ++groupindex)
        this.Groups[groupindex].WriteValues(bw, groupindex, position4);
      int position5 = (int) bw.Position;
      bw.FillInt32("ValueIDsOffset", (int) bw.Position);
      for (int groupIndex = 0; groupIndex < this.Groups.Count; ++groupIndex)
        this.Groups[groupIndex]
            .WriteValueIDs(bw, this.Game, groupIndex, position5);
      bw.FillInt32("UnkOffset2", (int) bw.Position);
      bw.WriteBytes(this.UnkBlock2);
      bw.FillInt32("UnkOffset3", (int) bw.Position);
      for (int index = 0; index < this.Unk3s.Count; ++index)
        this.Unk3s[index].WriteHeader(bw, this.Game, index);
      int position6 = (int) bw.Position;
      bw.FillInt32("Unk3ValuesOffset", position6);
      for (int index = 0; index < this.Unk3s.Count; ++index)
        this.Unk3s[index].WriteValues(bw, this.Game, index, position6);
      if (this.Game != GPARAM.GPGame.DarkSouls3 &&
          this.Game != GPARAM.GPGame.Sekiro)
        return;
      bw.FillInt32("CommentOffsetsOffsetsOffset", (int) bw.Position);
      for (int index = 0; index < this.Groups.Count; ++index)
        this.Groups[index].WriteCommentOffsetsOffset(bw, index);
      int position7 = (int) bw.Position;
      bw.FillInt32("CommentOffsetsOffset", position7);
      for (int index = 0; index < this.Groups.Count; ++index)
        this.Groups[index].WriteCommentOffsets(bw, index, position7);
      int position8 = (int) bw.Position;
      bw.FillInt32("CommentsOffset", position8);
      for (int index = 0; index < this.Groups.Count; ++index)
        this.Groups[index].WriteComments(bw, index, position8);
    }

    public GPARAM.Group this[string name1] {
      get {
        return this.Groups.Find(
            (Predicate<GPARAM.Group>)
            (group => group.Name1 == name1));
      }
    }

    public enum GPGame : uint {
      DarkSouls2 = 2,
      DarkSouls3 = 3,
      Sekiro = 5,
    }

    internal struct Offsets {
      public int GroupHeaders;
      public int ParamHeaderOffsets;
      public int ParamHeaders;
      public int Values;
      public int ValueIDs;
      public int Unk2;
      public int Unk3;
      public int Unk3ValueIDs;
      public int CommentOffsetsOffsets;
      public int CommentOffsets;
      public int Comments;
    }

    public class Group {
      public string Name1;
      public string Name2;
      public List<GPARAM.Param> Params;
      public List<string> Comments;

      public Group(string name1, string name2) {
        this.Name1 = name1;
        this.Name2 = name2;
        this.Params = new List<GPARAM.Param>();
        this.Comments = new List<string>();
      }

      internal Group(
          BinaryReaderEx br,
          GPARAM.GPGame game,
          int index,
          GPARAM.Offsets offsets) {
        int num1 = br.ReadInt32();
        br.StepIn((long) (offsets.GroupHeaders + num1));
        int capacity = br.ReadInt32();
        int num2 = br.ReadInt32();
        if (game == GPARAM.GPGame.DarkSouls2) {
          this.Name1 = br.ReadShiftJIS();
        } else {
          this.Name1 = br.ReadUTF16();
          this.Name2 = br.ReadUTF16();
        }
        br.StepIn((long) (offsets.ParamHeaderOffsets + num2));
        this.Params = new List<GPARAM.Param>(capacity);
        for (int index1 = 0; index1 < capacity; ++index1)
          this.Params.Add(new GPARAM.Param(br, game, offsets));
        br.StepOut();
        br.StepOut();
        this.Comments = new List<string>();
      }

      internal void WriteHeaderOffset(BinaryWriterEx bw, int groupIndex) {
        bw.ReserveInt32(
            string.Format("GroupHeaderOffset{0}", (object) groupIndex));
      }

      internal void WriteHeader(
          BinaryWriterEx bw,
          GPARAM.GPGame game,
          int groupIndex,
          int groupHeadersOffset) {
        bw.FillInt32(string.Format("GroupHeaderOffset{0}", (object) groupIndex),
                     (int) bw.Position - groupHeadersOffset);
        bw.WriteInt32(this.Params.Count);
        bw.ReserveInt32(string.Format("ParamHeaderOffsetsOffset{0}",
                                      (object) groupIndex));
        if (game == GPARAM.GPGame.DarkSouls2) {
          bw.WriteShiftJIS(this.Name1, true);
        } else {
          bw.WriteUTF16(this.Name1, true);
          bw.WriteUTF16(this.Name2, true);
        }
        bw.Pad(4);
      }

      internal void WriteParamHeaderOffsets(
          BinaryWriterEx bw,
          int groupIndex,
          int paramHeaderOffsetsOffset) {
        bw.FillInt32(
            string.Format("ParamHeaderOffsetsOffset{0}", (object) groupIndex),
            (int) bw.Position - paramHeaderOffsetsOffset);
        for (int paramIndex = 0; paramIndex < this.Params.Count; ++paramIndex)
          this.Params[paramIndex]
              .WriteParamHeaderOffset(bw, groupIndex, paramIndex);
      }

      internal void WriteParamHeaders(
          BinaryWriterEx bw,
          GPARAM.GPGame game,
          int groupindex,
          int paramHeadersOffset) {
        for (int paramIndex = 0; paramIndex < this.Params.Count; ++paramIndex)
          this.Params[paramIndex]
              .WriteParamHeader(bw,
                                game,
                                groupindex,
                                paramIndex,
                                paramHeadersOffset);
      }

      internal void WriteValues(
          BinaryWriterEx bw,
          int groupindex,
          int valuesOffset) {
        for (int paramIndex = 0; paramIndex < this.Params.Count; ++paramIndex)
          this.Params[paramIndex]
              .WriteValues(bw, groupindex, paramIndex, valuesOffset);
      }

      internal void WriteValueIDs(
          BinaryWriterEx bw,
          GPARAM.GPGame game,
          int groupIndex,
          int valueIDsOffset) {
        for (int paramIndex = 0; paramIndex < this.Params.Count; ++paramIndex)
          this.Params[paramIndex]
              .WriteValueIDs(bw, game, groupIndex, paramIndex, valueIDsOffset);
      }

      internal void WriteCommentOffsetsOffset(BinaryWriterEx bw, int index) {
        bw.ReserveInt32(
            string.Format("CommentOffsetsOffset{0}", (object) index));
      }

      internal void WriteCommentOffsets(
          BinaryWriterEx bw,
          int index,
          int commentOffsetsOffset) {
        bw.FillInt32(string.Format("CommentOffsetsOffset{0}", (object) index),
                     (int) bw.Position - commentOffsetsOffset);
        for (int index1 = 0; index1 < this.Comments.Count; ++index1)
          bw.ReserveInt32(string.Format("CommentOffset{0}:{1}",
                                        (object) index,
                                        (object) index1));
      }

      internal void WriteComments(
          BinaryWriterEx bw,
          int index,
          int commentsOffset) {
        for (int index1 = 0; index1 < this.Comments.Count; ++index1) {
          bw.FillInt32(
              string.Format("CommentOffset{0}:{1}",
                            (object) index,
                            (object) index1),
              (int) bw.Position - commentsOffset);
          bw.WriteUTF16(this.Comments[index1], true);
          bw.Pad(4);
        }
      }

      public GPARAM.Param this[string name1] {
        get {
          return this.Params.Find(
              (Predicate<GPARAM.Param>) (param => param.Name1 ==
                                                  name1));
        }
      }

      public override string ToString() {
        return this.Name2 == null
                   ? this.Name1
                   : this.Name1 + " | " + this.Name2;
      }
    }

    public enum ParamType : byte {
      Byte = 1,
      Short = 2,
      IntA = 3,
      BoolA = 5,
      IntB = 7,
      Float = 9,
      BoolB = 11,  // 0x0B
      Float2 = 12, // 0x0C
      Float3 = 13, // 0x0D
      Float4 = 14, // 0x0E
      Byte4 = 15,  // 0x0F
    }

    public class Param {
      public string Name1;
      public string Name2;
      public GPARAM.ParamType Type;
      public List<object> Values;
      public List<int> ValueIDs;
      public List<float> UnkFloats;

      public Param(string name1, string name2, GPARAM.ParamType type) {
        this.Name1 = name1;
        this.Name2 = name2;
        this.Type = type;
        this.Values = new List<object>();
        this.ValueIDs = new List<int>();
        this.UnkFloats = (List<float>) null;
      }

      internal Param(
          BinaryReaderEx br,
          GPARAM.GPGame game,
          GPARAM.Offsets offsets) {
        int num1 = br.ReadInt32();
        br.StepIn((long) (offsets.ParamHeaders + num1));
        int num2 = br.ReadInt32();
        int num3 = br.ReadInt32();
        this.Type = br.ReadEnum8<GPARAM.ParamType>();
        byte num4 = br.ReadByte();
        int num5 = (int) br.AssertByte(new byte[1]);
        int num6 = (int) br.AssertByte(new byte[1]);
        if (this.Type == GPARAM.ParamType.Byte && num4 > (byte) 1)
          throw new Exception("Notify TKGP so he can look into this, please.");
        if (game == GPARAM.GPGame.DarkSouls2) {
          this.Name1 = br.ReadShiftJIS();
        } else {
          this.Name1 = br.ReadUTF16();
          this.Name2 = br.ReadUTF16();
        }
        br.StepIn((long) (offsets.Values + num2));
        this.Values = new List<object>((int) num4);
        for (int index = 0; index < (int) num4; ++index) {
          switch (this.Type) {
            case GPARAM.ParamType.Byte:
              this.Values.Add((object) br.ReadByte());
              break;
            case GPARAM.ParamType.Short:
              this.Values.Add((object) br.ReadInt16());
              break;
            case GPARAM.ParamType.IntA:
              this.Values.Add((object) br.ReadInt32());
              break;
            case GPARAM.ParamType.BoolA:
              this.Values.Add((object) br.ReadBoolean());
              break;
            case GPARAM.ParamType.IntB:
              this.Values.Add((object) br.ReadInt32());
              break;
            case GPARAM.ParamType.Float:
              this.Values.Add((object) br.ReadSingle());
              break;
            case GPARAM.ParamType.BoolB:
              this.Values.Add((object) br.ReadBoolean());
              break;
            case GPARAM.ParamType.Float2:
              this.Values.Add((object) br.ReadVector2());
              br.AssertInt32(new int[1]);
              br.AssertInt32(new int[1]);
              break;
            case GPARAM.ParamType.Float3:
              this.Values.Add((object) br.ReadVector3());
              br.AssertInt32(new int[1]);
              break;
            case GPARAM.ParamType.Float4:
              this.Values.Add((object) br.ReadVector4());
              break;
            case GPARAM.ParamType.Byte4:
              this.Values.Add((object) br.ReadBytes(4));
              break;
          }
        }
        br.StepOut();
        br.StepIn((long) (offsets.ValueIDs + num3));
        this.ValueIDs = new List<int>((int) num4);
        this.UnkFloats = game != GPARAM.GPGame.Sekiro
                             ? (List<float>) null
                             : new List<float>((int) num4);
        for (int index = 0; index < (int) num4; ++index) {
          this.ValueIDs.Add(br.ReadInt32());
          if (game == GPARAM.GPGame.Sekiro)
            this.UnkFloats.Add(br.ReadSingle());
        }
        br.StepOut();
        br.StepOut();
      }

      internal void WriteParamHeaderOffset(
          BinaryWriterEx bw,
          int groupIndex,
          int paramIndex) {
        bw.ReserveInt32(string.Format("ParamHeaderOffset{0}:{1}",
                                      (object) groupIndex,
                                      (object) paramIndex));
      }

      internal void WriteParamHeader(
          BinaryWriterEx bw,
          GPARAM.GPGame game,
          int groupIndex,
          int paramIndex,
          int paramHeadersOffset) {
        bw.FillInt32(
            string.Format("ParamHeaderOffset{0}:{1}",
                          (object) groupIndex,
                          (object) paramIndex),
            (int) bw.Position - paramHeadersOffset);
        bw.ReserveInt32(string.Format("ValuesOffset{0}:{1}",
                                      (object) groupIndex,
                                      (object) paramIndex));
        bw.ReserveInt32(string.Format("ValueIDsOffset{0}:{1}",
                                      (object) groupIndex,
                                      (object) paramIndex));
        bw.WriteByte((byte) this.Type);
        bw.WriteByte((byte) this.Values.Count);
        bw.WriteByte((byte) 0);
        bw.WriteByte((byte) 0);
        if (game == GPARAM.GPGame.DarkSouls2) {
          bw.WriteShiftJIS(this.Name1, true);
        } else {
          bw.WriteUTF16(this.Name1, true);
          bw.WriteUTF16(this.Name2, true);
        }
        bw.Pad(4);
      }

      internal void WriteValues(
          BinaryWriterEx bw,
          int groupIndex,
          int paramIndex,
          int valuesOffset) {
        bw.FillInt32(
            string.Format("ValuesOffset{0}:{1}",
                          (object) groupIndex,
                          (object) paramIndex),
            (int) bw.Position - valuesOffset);
        for (int index = 0; index < this.Values.Count; ++index) {
          object obj = this.Values[index];
          switch (this.Type) {
            case GPARAM.ParamType.Byte:
              bw.WriteInt32((int) (byte) obj);
              break;
            case GPARAM.ParamType.Short:
              bw.WriteInt16((short) obj);
              break;
            case GPARAM.ParamType.IntA:
              bw.WriteInt32((int) obj);
              break;
            case GPARAM.ParamType.BoolA:
              bw.WriteBoolean((bool) obj);
              break;
            case GPARAM.ParamType.IntB:
              bw.WriteInt32((int) obj);
              break;
            case GPARAM.ParamType.Float:
              bw.WriteSingle((float) obj);
              break;
            case GPARAM.ParamType.BoolB:
              bw.WriteBoolean((bool) obj);
              break;
            case GPARAM.ParamType.Float2:
              bw.WriteVector2((Vector2) obj);
              bw.WriteInt32(0);
              bw.WriteInt32(0);
              break;
            case GPARAM.ParamType.Float3:
              bw.WriteVector3((Vector3) obj);
              bw.WriteInt32(0);
              break;
            case GPARAM.ParamType.Float4:
              bw.WriteVector4((Vector4) obj);
              break;
            case GPARAM.ParamType.Byte4:
              bw.WriteBytes((byte[]) obj);
              break;
          }
        }
        bw.Pad(4);
      }

      internal void WriteValueIDs(
          BinaryWriterEx bw,
          GPARAM.GPGame game,
          int groupIndex,
          int paramIndex,
          int valueIDsOffset) {
        bw.FillInt32(
            string.Format("ValueIDsOffset{0}:{1}",
                          (object) groupIndex,
                          (object) paramIndex),
            (int) bw.Position - valueIDsOffset);
        for (int index = 0; index < this.ValueIDs.Count; ++index) {
          bw.WriteInt32(this.ValueIDs[index]);
          if (game == GPARAM.GPGame.Sekiro)
            bw.WriteSingle(this.UnkFloats[index]);
        }
      }

      public object this[int index] {
        get { return this.Values[index]; }
        set { this.Values[index] = value; }
      }

      public override string ToString() {
        return this.Name2 == null
                   ? this.Name1
                   : this.Name1 + " | " + this.Name2;
      }
    }

    public class Unk3 {
      public int GroupIndex;
      public List<int> ValueIDs;
      public int Unk0C;

      public Unk3(int groupIndex) {
        this.GroupIndex = groupIndex;
        this.ValueIDs = new List<int>();
      }

      internal Unk3(
          BinaryReaderEx br,
          GPARAM.GPGame game,
          GPARAM.Offsets offsets) {
        this.GroupIndex = br.ReadInt32();
        int count = br.ReadInt32();
        uint num = br.ReadUInt32();
        if (game == GPARAM.GPGame.Sekiro)
          this.Unk0C = br.ReadInt32();
        this.ValueIDs =
            new List<int>((IEnumerable<int>) br.GetInt32s(
                              (long) offsets.Unk3ValueIDs + (long) num,
                              count));
      }

      internal void WriteHeader(
          BinaryWriterEx bw,
          GPARAM.GPGame game,
          int index) {
        bw.WriteInt32(this.GroupIndex);
        bw.WriteInt32(this.ValueIDs.Count);
        bw.ReserveInt32(string.Format("Unk3ValueIDsOffset{0}", (object) index));
        if (game != GPARAM.GPGame.Sekiro)
          return;
        bw.WriteInt32(this.Unk0C);
      }

      internal void WriteValues(
          BinaryWriterEx bw,
          GPARAM.GPGame game,
          int index,
          int unk3ValueIDsOffset) {
        if (this.ValueIDs.Count == 0) {
          bw.FillInt32(string.Format("Unk3ValueIDsOffset{0}", (object) index),
                       0);
        } else {
          bw.FillInt32(string.Format("Unk3ValueIDsOffset{0}", (object) index),
                       (int) bw.Position - unk3ValueIDsOffset);
          bw.WriteInt32s((IList<int>) this.ValueIDs);
        }
      }
    }
  }
}