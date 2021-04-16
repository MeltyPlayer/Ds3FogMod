// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MSBD
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class MSBD : SoulsFile<MSBD> {
    public MSBD.ModelSection Models;
    public MSBD.PartsSection Parts;

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = true;
      MSBD.Entries entries = new MSBD.Entries();
      int num1 = (int) br.Position;
      do {
        br.Position = (long) num1;
        int unk1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int offsets = br.ReadInt32() - 1;
        string ascii = br.GetASCII((long) num2);
        if (!(ascii == "MODEL_PARAM_ST")) {
          if (ascii == "PARTS_PARAM_ST") {
            this.Parts = new MSBD.PartsSection(br, unk1);
            entries.Parts = this.Parts.Read(br, offsets);
          } else
            br.Skip(offsets * 4);
        } else {
          this.Models = new MSBD.ModelSection(br, unk1);
          entries.Models = this.Models.Read(br, offsets);
        }
        num1 = br.ReadInt32();
      } while (num1 != 0);
      MSB.DisambiguateNames<MSBD.Model>(entries.Models);
      MSB.DisambiguateNames<MSBD.Part>(entries.Parts);
      this.Parts.GetNames(this, entries);
    }

    internal struct Entries {
      public List<MSBD.Model> Models;
      public List<MSBD.Part> Parts;
    }

    public abstract class Section<T> {
      public int Unk1;

      internal abstract string Type { get; }

      internal Section(BinaryReaderEx br, int unk1) {
        this.Unk1 = unk1;
      }

      public abstract List<T> GetEntries();

      internal List<T> Read(BinaryReaderEx br, int offsets) {
        List<T> objList = new List<T>(offsets);
        for (int index = 0; index < offsets; ++index) {
          int num = br.ReadInt32();
          br.StepIn((long) num);
          objList.Add(this.ReadEntry(br));
          br.StepOut();
        }
        return objList;
      }

      internal abstract T ReadEntry(BinaryReaderEx br);

      internal void Write(BinaryWriterEx bw, List<T> entries) {
        bw.WriteInt32(this.Unk1);
        bw.ReserveInt32("TypeOffset");
        bw.WriteInt32(entries.Count + 1);
        for (int index = 0; index < entries.Count; ++index)
          bw.ReserveInt32(string.Format("Offset{0}", (object) index));
        bw.ReserveInt32("NextOffset");
        bw.FillInt32("TypeOffset", (int) bw.Position);
        bw.WriteASCII(this.Type, true);
        bw.Pad(4);
        this.WriteEntries(bw, entries);
      }

      internal abstract void WriteEntries(BinaryWriterEx bw, List<T> entries);

      public override string ToString() {
        return string.Format("{0}:{1}[{2}]",
                             (object) this.Type,
                             (object) this.Unk1,
                             (object) this.GetEntries().Count);
      }
    }

    public abstract class Entry : IMsbEntry {
      public abstract string Name { get; set; }
    }

    public class ModelSection : MSBD.Section<MSBD.Model> {
      public List<MSBD.Model> MapPieces;
      public List<MSBD.Model> Objects;
      public List<MSBD.Model> Enemies;
      public List<MSBD.Model> Items;
      public List<MSBD.Model> Players;
      public List<MSBD.Model> Collisions;
      public List<MSBD.Model> Navmeshes;
      public List<MSBD.Model> DummyObjects;
      public List<MSBD.Model> DummyEnemies;
      public List<MSBD.Model> Others;

      internal override string Type {
        get { return "MODEL_PARAM_ST"; }
      }

      internal ModelSection(BinaryReaderEx br, int unk1)
          : base(br, unk1) {
        this.MapPieces = new List<MSBD.Model>();
        this.Objects = new List<MSBD.Model>();
        this.Enemies = new List<MSBD.Model>();
        this.Items = new List<MSBD.Model>();
        this.Players = new List<MSBD.Model>();
        this.Collisions = new List<MSBD.Model>();
        this.Navmeshes = new List<MSBD.Model>();
        this.DummyObjects = new List<MSBD.Model>();
        this.DummyEnemies = new List<MSBD.Model>();
        this.Others = new List<MSBD.Model>();
      }

      public override List<MSBD.Model> GetEntries() {
        return SFUtil.ConcatAll<MSBD.Model>(new IEnumerable<MSBD.Model>[10] {
            (IEnumerable<MSBD.Model>) this.MapPieces,
            (IEnumerable<MSBD.Model>) this.Objects,
            (IEnumerable<MSBD.Model>) this.Enemies,
            (IEnumerable<MSBD.Model>) this.Items,
            (IEnumerable<MSBD.Model>) this.Players,
            (IEnumerable<MSBD.Model>) this.Collisions,
            (IEnumerable<MSBD.Model>) this.Navmeshes,
            (IEnumerable<MSBD.Model>) this.DummyObjects,
            (IEnumerable<MSBD.Model>) this.DummyEnemies,
            (IEnumerable<MSBD.Model>) this.Others
        });
      }

      internal override MSBD.Model ReadEntry(BinaryReaderEx br) {
        MSBD.ModelType enum32 = br.GetEnum32<MSBD.ModelType>(br.Position + 4L);
        switch (enum32) {
          case MSBD.ModelType.MapPiece:
            MSBD.Model model1 = new MSBD.Model(br);
            this.MapPieces.Add(model1);
            return model1;
          case MSBD.ModelType.Object:
            MSBD.Model model2 = new MSBD.Model(br);
            this.Objects.Add(model2);
            return model2;
          case MSBD.ModelType.Enemy:
            MSBD.Model model3 = new MSBD.Model(br);
            this.Enemies.Add(model3);
            return model3;
          case MSBD.ModelType.Item:
            MSBD.Model model4 = new MSBD.Model(br);
            this.Items.Add(model4);
            return model4;
          case MSBD.ModelType.Player:
            MSBD.Model model5 = new MSBD.Model(br);
            this.Players.Add(model5);
            return model5;
          case MSBD.ModelType.Collision:
            MSBD.Model model6 = new MSBD.Model(br);
            this.Collisions.Add(model6);
            return model6;
          case MSBD.ModelType.Navmesh:
            MSBD.Model model7 = new MSBD.Model(br);
            this.Navmeshes.Add(model7);
            return model7;
          case MSBD.ModelType.DummyObject:
            MSBD.Model model8 = new MSBD.Model(br);
            this.DummyObjects.Add(model8);
            return model8;
          case MSBD.ModelType.DummyEnemy:
            MSBD.Model model9 = new MSBD.Model(br);
            this.DummyEnemies.Add(model9);
            return model9;
          case MSBD.ModelType.Other:
            MSBD.Model model10 = new MSBD.Model(br);
            this.Others.Add(model10);
            return model10;
          default:
            throw new NotImplementedException(
                string.Format("Unsupported model type: {0}", (object) enum32));
        }
      }

      internal override void WriteEntries(
          BinaryWriterEx bw,
          List<MSBD.Model> entries) {
        throw new NotImplementedException();
      }
    }

    internal enum ModelType : uint {
      MapPiece = 0,
      Object = 1,
      Enemy = 2,
      Item = 3,
      Player = 4,
      Collision = 5,
      Navmesh = 6,
      DummyObject = 7,
      DummyEnemy = 8,
      Other = 65535, // 0x0000FFFF
    }

    public class Model : MSBD.Entry {
      internal MSBD.ModelType Type { get; private set; }

      public override string Name { get; set; }

      internal Model(BinaryReaderEx br) {
        long position = br.Position;
        int num = br.ReadInt32();
        this.Type = br.ReadEnum32<MSBD.ModelType>();
        this.Name = br.GetShiftJIS(position + (long) num);
      }

      public override string ToString() {
        return string.Format("{0} : {1}",
                             (object) this.Type,
                             (object) this.Name);
      }
    }

    public class PartsSection : MSBD.Section<MSBD.Part> {
      public List<MSBD.Part> MapPieces;
      public List<MSBD.Part> Objects;
      public List<MSBD.Part> Enemies;
      public List<MSBD.Part> Items;
      public List<MSBD.Part> Players;
      public List<MSBD.Part> Collisions;
      public List<MSBD.Part> Protobosses;
      public List<MSBD.Part> Navmeshes;
      public List<MSBD.Part> DummyObjects;
      public List<MSBD.Part> DummyEnemies;
      public List<MSBD.Part> ConnectCollisions;

      internal override string Type {
        get { return "PARTS_PARAM_ST"; }
      }

      internal PartsSection(BinaryReaderEx br, int unk1)
          : base(br, unk1) {
        this.MapPieces = new List<MSBD.Part>();
        this.Objects = new List<MSBD.Part>();
        this.Enemies = new List<MSBD.Part>();
        this.Items = new List<MSBD.Part>();
        this.Players = new List<MSBD.Part>();
        this.Collisions = new List<MSBD.Part>();
        this.Protobosses = new List<MSBD.Part>();
        this.Navmeshes = new List<MSBD.Part>();
        this.DummyObjects = new List<MSBD.Part>();
        this.DummyEnemies = new List<MSBD.Part>();
        this.ConnectCollisions = new List<MSBD.Part>();
      }

      public override List<MSBD.Part> GetEntries() {
        return SFUtil.ConcatAll<MSBD.Part>(new IEnumerable<MSBD.Part>[11] {
            (IEnumerable<MSBD.Part>) this.MapPieces,
            (IEnumerable<MSBD.Part>) this.Objects,
            (IEnumerable<MSBD.Part>) this.Enemies,
            (IEnumerable<MSBD.Part>) this.Items,
            (IEnumerable<MSBD.Part>) this.Players,
            (IEnumerable<MSBD.Part>) this.Collisions,
            (IEnumerable<MSBD.Part>) this.Protobosses,
            (IEnumerable<MSBD.Part>) this.Navmeshes,
            (IEnumerable<MSBD.Part>) this.DummyObjects,
            (IEnumerable<MSBD.Part>) this.DummyEnemies,
            (IEnumerable<MSBD.Part>) this.ConnectCollisions
        });
      }

      internal override MSBD.Part ReadEntry(BinaryReaderEx br) {
        MSBD.PartsType enum32 = br.GetEnum32<MSBD.PartsType>(br.Position + 4L);
        switch (enum32) {
          case MSBD.PartsType.MapPiece:
            MSBD.Part part1 = new MSBD.Part(br);
            this.MapPieces.Add(part1);
            return part1;
          case MSBD.PartsType.Object:
            MSBD.Part part2 = new MSBD.Part(br);
            this.Objects.Add(part2);
            return part2;
          case MSBD.PartsType.Enemy:
            MSBD.Part part3 = new MSBD.Part(br);
            this.Enemies.Add(part3);
            return part3;
          case MSBD.PartsType.Item:
            MSBD.Part part4 = new MSBD.Part(br);
            this.Items.Add(part4);
            return part4;
          case MSBD.PartsType.Player:
            MSBD.Part part5 = new MSBD.Part(br);
            this.Players.Add(part5);
            return part5;
          case MSBD.PartsType.Collision:
            MSBD.Part part6 = new MSBD.Part(br);
            this.Collisions.Add(part6);
            return part6;
          case MSBD.PartsType.Protoboss:
            MSBD.Part part7 = new MSBD.Part(br);
            this.Protobosses.Add(part7);
            return part7;
          case MSBD.PartsType.Navmesh:
            MSBD.Part part8 = new MSBD.Part(br);
            this.Navmeshes.Add(part8);
            return part8;
          case MSBD.PartsType.DummyObject:
            MSBD.Part part9 = new MSBD.Part(br);
            this.DummyObjects.Add(part9);
            return part9;
          case MSBD.PartsType.DummyEnemy:
            MSBD.Part part10 = new MSBD.Part(br);
            this.DummyEnemies.Add(part10);
            return part10;
          case MSBD.PartsType.ConnectCollision:
            MSBD.Part part11 = new MSBD.Part(br);
            this.ConnectCollisions.Add(part11);
            return part11;
          default:
            throw new NotImplementedException(
                string.Format("Unsupported part type: {0}", (object) enum32));
        }
      }

      internal override void WriteEntries(
          BinaryWriterEx bw,
          List<MSBD.Part> entries) {
        throw new NotImplementedException();
      }

      internal void GetNames(MSBD msb, MSBD.Entries entries) {
        foreach (MSBD.Part part in entries.Parts)
          part.GetNames(msb, entries);
      }

      internal void GetIndices(MSBD msb, MSBD.Entries entries) {
        foreach (MSBD.Part part in entries.Parts)
          part.GetIndices(msb, entries);
      }
    }

    internal enum PartsType : uint {
      MapPiece,
      Object,
      Enemy,
      Item,
      Player,
      Collision,
      NPCWander,
      Protoboss,
      Navmesh,
      DummyObject,
      DummyEnemy,
      ConnectCollision,
    }

    public class Part : MSBD.Entry {
      private int modelIndex;
      public string ModelName;
      public Vector3 Position;
      public Vector3 Rotation;
      public Vector3 Scale;

      internal MSBD.PartsType Type { get; private set; }

      public override string Name { get; set; }

      internal Part(BinaryReaderEx br) {
        long position = br.Position;
        int num = br.ReadInt32();
        this.Type = br.ReadEnum32<MSBD.PartsType>();
        br.ReadInt32();
        this.modelIndex = br.ReadInt32();
        br.ReadInt32();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        this.Name = br.GetShiftJIS(position + (long) num);
      }

      internal virtual void GetNames(MSBD msb, MSBD.Entries entries) {
        this.ModelName =
            MSB.FindName<MSBD.Model>(entries.Models, this.modelIndex);
      }

      internal virtual void GetIndices(MSBD msb, MSBD.Entries entries) {
        this.modelIndex =
            MSB.FindIndex<MSBD.Model>(entries.Models, this.ModelName);
      }

      public override string ToString() {
        return string.Format("{0} : {1}",
                             (object) this.Type,
                             (object) this.Name);
      }
    }
  }
}