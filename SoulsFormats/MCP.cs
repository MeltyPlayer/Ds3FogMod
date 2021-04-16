// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MCP
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class MCP : SoulsFile<MCP> {
    public bool BigEndian { get; set; }

    public int Unk04 { get; set; }

    public List<MCP.Room> Rooms { get; set; }

    public MCP() {
      this.Rooms = new List<MCP.Room>();
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = true;
      this.BigEndian = br.AssertInt32(2, 33554432) == 2;
      br.BigEndian = this.BigEndian;
      this.Unk04 = br.ReadInt32();
      int capacity = br.ReadInt32();
      int num = br.ReadInt32();
      br.Position = (long) num;
      this.Rooms = new List<MCP.Room>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Rooms.Add(new MCP.Room(br));
    }

    public override bool Validate(out Exception ex) {
      if (!SoulsFile<MCP>.ValidateNull((object) this.Rooms,
                                       "Rooms may not be null.",
                                       out ex))
        return false;
      for (int index1 = 0; index1 < this.Rooms.Count; ++index1) {
        MCP.Room room = this.Rooms[index1];
        if (!SoulsFile<MCP>.ValidateNull((object) room,
                                         string.Format(
                                             "{0}[{1}]: {2} may not be null.",
                                             (object) "Rooms",
                                             (object) index1,
                                             (object) "Room"),
                                         out ex) ||
            !SoulsFile<MCP>.ValidateNull((object) room.ConnectedRoomIndices,
                                         string.Format(
                                             "{0}[{1}]: {2} may not be null.",
                                             (object) "Rooms",
                                             (object) index1,
                                             (object) "ConnectedRoomIndices"),
                                         out ex))
          return false;
        for (int index2 = 0;
             index2 < room.ConnectedRoomIndices.Count;
             ++index2) {
          int connectedRoomIndex = room.ConnectedRoomIndices[index2];
          if (!SoulsFile<MCP>.ValidateIndex((long) this.Rooms.Count,
                                            (long) connectedRoomIndex,
                                            string.Format(
                                                "{0}[{1}].{2}[{3}]: Index out of range: {4}",
                                                (object) "Rooms",
                                                (object) index1,
                                                (object) "ConnectedRoomIndices",
                                                (object) index2,
                                                (object) connectedRoomIndex),
                                            out ex))
            return false;
        }
      }
      ex = (Exception) null;
      return true;
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = this.BigEndian;
      bw.WriteInt32(2);
      bw.WriteInt32(this.Unk04);
      bw.WriteInt32(this.Rooms.Count);
      bw.ReserveInt32("RoomsOffset");
      long[] numArray = new long[this.Rooms.Count];
      for (int index = 0; index < this.Rooms.Count; ++index) {
        numArray[index] = bw.Position;
        bw.WriteInt32s((IList<int>) this.Rooms[index].ConnectedRoomIndices);
      }
      bw.FillInt32("RoomsOffset", (int) bw.Position);
      for (int index = 0; index < this.Rooms.Count; ++index)
        this.Rooms[index].Write(bw, numArray[index]);
    }

    public class Room {
      public uint MapID { get; set; }

      public int LocalIndex { get; set; }

      public Vector3 BoundingBoxMin { get; set; }

      public Vector3 BoundingBoxMax { get; set; }

      public List<int> ConnectedRoomIndices { get; set; }

      public Room() {
        this.ConnectedRoomIndices = new List<int>();
      }

      internal Room(BinaryReaderEx br) {
        this.MapID = br.ReadUInt32();
        this.LocalIndex = br.ReadInt32();
        int count = br.ReadInt32();
        int num = br.ReadInt32();
        this.BoundingBoxMin = br.ReadVector3();
        this.BoundingBoxMax = br.ReadVector3();
        this.ConnectedRoomIndices =
            new List<int>((IEnumerable<int>) br.GetInt32s((long) num, count));
      }

      internal void Write(BinaryWriterEx bw, long indicesOffset) {
        bw.WriteUInt32(this.MapID);
        bw.WriteInt32(this.LocalIndex);
        bw.WriteInt32(this.ConnectedRoomIndices.Count);
        bw.WriteInt32((int) indicesOffset);
        bw.WriteVector3(this.BoundingBoxMin);
        bw.WriteVector3(this.BoundingBoxMax);
      }
    }
  }
}