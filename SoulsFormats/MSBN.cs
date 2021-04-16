// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MSBN
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
  [ComVisible(true)]
  public class MSBN : SoulsFile<MSBN>
  {
    public MSBN.ModelSection Models;
    public MSBN.PartsSection Parts;

    protected override void Read(BinaryReaderEx br)
    {
      br.BigEndian = false;
      MSBN.Entries entries = new MSBN.Entries();
      int num1 = (int) br.Position;
      do
      {
        br.Position = (long) num1;
        int unk1 = br.ReadInt32();
        int num2 = br.ReadInt32();
        int offsets = br.ReadInt32() - 1;
        string ascii = br.GetASCII((long) num2);
        if (!(ascii == "MODEL_PARAM_ST"))
        {
          if (ascii == "PARTS_PARAM_ST")
          {
            this.Parts = new MSBN.PartsSection(br, unk1);
            entries.Parts = this.Parts.Read(br, offsets);
          }
          else
            br.Skip(offsets * 4);
        }
        else
        {
          this.Models = new MSBN.ModelSection(br, unk1);
          entries.Models = this.Models.Read(br, offsets);
        }
        num1 = br.ReadInt32();
      }
      while (num1 != 0);
      MSB.DisambiguateNames<MSBN.Model>(entries.Models);
      MSB.DisambiguateNames<MSBN.Part>(entries.Parts);
      this.Parts.GetNames(this, entries);
    }

    internal struct Entries
    {
      public List<MSBN.Model> Models;
      public List<MSBN.Part> Parts;
    }

    public abstract class Section<T>
    {
      public int Unk1;

      internal abstract string Type { get; }

      internal Section(BinaryReaderEx br, int unk1)
      {
        this.Unk1 = unk1;
      }

      public abstract List<T> GetEntries();

      internal List<T> Read(BinaryReaderEx br, int offsets)
      {
        List<T> objList = new List<T>(offsets);
        for (int index = 0; index < offsets; ++index)
        {
          int num = br.ReadInt32();
          br.StepIn((long) num);
          objList.Add(this.ReadEntry(br));
          br.StepOut();
        }
        return objList;
      }

      internal abstract T ReadEntry(BinaryReaderEx br);

      internal void Write(BinaryWriterEx bw, List<T> entries)
      {
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

      public override string ToString()
      {
        return string.Format("{0}:{1}[{2}]", (object) this.Type, (object) this.Unk1, (object) this.GetEntries().Count);
      }
    }

    public abstract class Entry : IMsbEntry
    {
      public abstract string Name { get; set; }
    }

    public class ModelSection : MSBN.Section<MSBN.Model>
    {
      public List<MSBN.Model> MapPieces;
      public List<MSBN.Model> Objects;
      public List<MSBN.Model> Enemies;
      public List<MSBN.Model> Items;
      public List<MSBN.Model> Players;
      public List<MSBN.Model> Collisions;
      public List<MSBN.Model> Navmeshes;
      public List<MSBN.Model> DummyObjects;
      public List<MSBN.Model> DummyEnemies;
      public List<MSBN.Model> Others;

      internal override string Type
      {
        get
        {
          return "MODEL_PARAM_ST";
        }
      }

      internal ModelSection(BinaryReaderEx br, int unk1)
        : base(br, unk1)
      {
        this.MapPieces = new List<MSBN.Model>();
        this.Objects = new List<MSBN.Model>();
        this.Enemies = new List<MSBN.Model>();
        this.Items = new List<MSBN.Model>();
        this.Players = new List<MSBN.Model>();
        this.Collisions = new List<MSBN.Model>();
        this.Navmeshes = new List<MSBN.Model>();
        this.DummyObjects = new List<MSBN.Model>();
        this.DummyEnemies = new List<MSBN.Model>();
        this.Others = new List<MSBN.Model>();
      }

      public override List<MSBN.Model> GetEntries()
      {
        return SFUtil.ConcatAll<MSBN.Model>(new IEnumerable<MSBN.Model>[10]
        {
          (IEnumerable<MSBN.Model>) this.MapPieces,
          (IEnumerable<MSBN.Model>) this.Objects,
          (IEnumerable<MSBN.Model>) this.Enemies,
          (IEnumerable<MSBN.Model>) this.Items,
          (IEnumerable<MSBN.Model>) this.Players,
          (IEnumerable<MSBN.Model>) this.Collisions,
          (IEnumerable<MSBN.Model>) this.Navmeshes,
          (IEnumerable<MSBN.Model>) this.DummyObjects,
          (IEnumerable<MSBN.Model>) this.DummyEnemies,
          (IEnumerable<MSBN.Model>) this.Others
        });
      }

      internal override MSBN.Model ReadEntry(BinaryReaderEx br)
      {
        MSBN.ModelType enum32 = br.GetEnum32<MSBN.ModelType>(br.Position + 4L);
        switch (enum32)
        {
          case MSBN.ModelType.Collision:
            MSBN.Model model1 = new MSBN.Model(br);
            this.Collisions.Add(model1);
            return model1;
          case MSBN.ModelType.MapPiece:
            MSBN.Model model2 = new MSBN.Model(br);
            this.MapPieces.Add(model2);
            return model2;
          case MSBN.ModelType.Object:
            MSBN.Model model3 = new MSBN.Model(br);
            this.Objects.Add(model3);
            return model3;
          case MSBN.ModelType.Enemy:
            MSBN.Model model4 = new MSBN.Model(br);
            this.Enemies.Add(model4);
            return model4;
          case MSBN.ModelType.Item:
            MSBN.Model model5 = new MSBN.Model(br);
            this.Items.Add(model5);
            return model5;
          case MSBN.ModelType.Player:
            MSBN.Model model6 = new MSBN.Model(br);
            this.Players.Add(model6);
            return model6;
          case MSBN.ModelType.Navmesh:
            MSBN.Model model7 = new MSBN.Model(br);
            this.Navmeshes.Add(model7);
            return model7;
          case MSBN.ModelType.DummyObject:
            MSBN.Model model8 = new MSBN.Model(br);
            this.DummyObjects.Add(model8);
            return model8;
          case MSBN.ModelType.DummyEnemy:
            MSBN.Model model9 = new MSBN.Model(br);
            this.DummyEnemies.Add(model9);
            return model9;
          case MSBN.ModelType.Other:
            MSBN.Model model10 = new MSBN.Model(br);
            this.Others.Add(model10);
            return model10;
          default:
            throw new NotImplementedException(string.Format("Unsupported model type: {0}", (object) enum32));
        }
      }

      internal override void WriteEntries(BinaryWriterEx bw, List<MSBN.Model> entries)
      {
        throw new NotImplementedException();
      }
    }

    internal enum ModelType : uint
    {
      Collision = 0,
      MapPiece = 1,
      Object = 2,
      Enemy = 3,
      Item = 4,
      Player = 5,
      Navmesh = 6,
      DummyObject = 7,
      DummyEnemy = 8,
      Other = 4294967295, // 0xFFFFFFFF
    }

    public class Model : MSBN.Entry
    {
      internal MSBN.ModelType Type { get; private set; }

      public override string Name { get; set; }

      internal Model(BinaryReaderEx br)
      {
        long position = br.Position;
        int num = br.ReadInt32();
        this.Type = br.ReadEnum32<MSBN.ModelType>();
        this.Name = br.GetShiftJIS(position + (long) num);
      }

      public override string ToString()
      {
        return string.Format("{0} : {1}", (object) this.Type, (object) this.Name);
      }
    }

    public class PartsSection : MSBN.Section<MSBN.Part>
    {
      public List<MSBN.Part> MapPieces;
      public List<MSBN.Part> Objects;
      public List<MSBN.Part> Enemies;
      public List<MSBN.Part> Items;
      public List<MSBN.Part> Players;
      public List<MSBN.Part> Collisions;
      public List<MSBN.Part> Protobosses;
      public List<MSBN.Part> Navmeshes;
      public List<MSBN.Part> DummyObjects;
      public List<MSBN.Part> DummyEnemies;
      public List<MSBN.Part> ConnectCollisions;

      internal override string Type
      {
        get
        {
          return "PARTS_PARAM_ST";
        }
      }

      internal PartsSection(BinaryReaderEx br, int unk1)
        : base(br, unk1)
      {
        this.MapPieces = new List<MSBN.Part>();
        this.Objects = new List<MSBN.Part>();
        this.Enemies = new List<MSBN.Part>();
        this.Items = new List<MSBN.Part>();
        this.Players = new List<MSBN.Part>();
        this.Collisions = new List<MSBN.Part>();
        this.Protobosses = new List<MSBN.Part>();
        this.Navmeshes = new List<MSBN.Part>();
        this.DummyObjects = new List<MSBN.Part>();
        this.DummyEnemies = new List<MSBN.Part>();
        this.ConnectCollisions = new List<MSBN.Part>();
      }

      public override List<MSBN.Part> GetEntries()
      {
        return SFUtil.ConcatAll<MSBN.Part>(new IEnumerable<MSBN.Part>[11]
        {
          (IEnumerable<MSBN.Part>) this.MapPieces,
          (IEnumerable<MSBN.Part>) this.Objects,
          (IEnumerable<MSBN.Part>) this.Enemies,
          (IEnumerable<MSBN.Part>) this.Items,
          (IEnumerable<MSBN.Part>) this.Players,
          (IEnumerable<MSBN.Part>) this.Collisions,
          (IEnumerable<MSBN.Part>) this.Protobosses,
          (IEnumerable<MSBN.Part>) this.Navmeshes,
          (IEnumerable<MSBN.Part>) this.DummyObjects,
          (IEnumerable<MSBN.Part>) this.DummyEnemies,
          (IEnumerable<MSBN.Part>) this.ConnectCollisions
        });
      }

      internal override MSBN.Part ReadEntry(BinaryReaderEx br)
      {
        MSBN.PartsType enum32 = br.GetEnum32<MSBN.PartsType>(br.Position + 4L);
        switch (enum32)
        {
          case MSBN.PartsType.Collision:
            MSBN.Part part1 = new MSBN.Part(br);
            this.Collisions.Add(part1);
            return part1;
          case MSBN.PartsType.MapPiece:
            MSBN.Part part2 = new MSBN.Part(br);
            this.MapPieces.Add(part2);
            return part2;
          case MSBN.PartsType.Object:
            MSBN.Part part3 = new MSBN.Part(br);
            this.Objects.Add(part3);
            return part3;
          case MSBN.PartsType.Enemy:
            MSBN.Part part4 = new MSBN.Part(br);
            this.Enemies.Add(part4);
            return part4;
          case MSBN.PartsType.Item:
            MSBN.Part part5 = new MSBN.Part(br);
            this.Items.Add(part5);
            return part5;
          case MSBN.PartsType.Player:
            MSBN.Part part6 = new MSBN.Part(br);
            this.Players.Add(part6);
            return part6;
          case MSBN.PartsType.Protoboss:
            MSBN.Part part7 = new MSBN.Part(br);
            this.Protobosses.Add(part7);
            return part7;
          case MSBN.PartsType.Navmesh:
            MSBN.Part part8 = new MSBN.Part(br);
            this.Navmeshes.Add(part8);
            return part8;
          case MSBN.PartsType.DummyObject:
            MSBN.Part part9 = new MSBN.Part(br);
            this.DummyObjects.Add(part9);
            return part9;
          case MSBN.PartsType.DummyEnemy:
            MSBN.Part part10 = new MSBN.Part(br);
            this.DummyEnemies.Add(part10);
            return part10;
          case MSBN.PartsType.ConnectCollision:
            MSBN.Part part11 = new MSBN.Part(br);
            this.ConnectCollisions.Add(part11);
            return part11;
          default:
            throw new NotImplementedException(string.Format("Unsupported part type: {0}", (object) enum32));
        }
      }

      internal override void WriteEntries(BinaryWriterEx bw, List<MSBN.Part> entries)
      {
        throw new NotImplementedException();
      }

      internal void GetNames(MSBN msb, MSBN.Entries entries)
      {
        foreach (MSBN.Part part in entries.Parts)
          part.GetNames(msb, entries);
      }

      internal void GetIndices(MSBN msb, MSBN.Entries entries)
      {
        foreach (MSBN.Part part in entries.Parts)
          part.GetIndices(msb, entries);
      }
    }

    internal enum PartsType : uint
    {
      Collision,
      MapPiece,
      Object,
      Enemy,
      Item,
      Player,
      NPCWander,
      Protoboss,
      Navmesh,
      DummyObject,
      DummyEnemy,
      ConnectCollision,
    }

    public class Part : MSBN.Entry
    {
      private int modelIndex;
      public string ModelName;
      public Vector3 Position;
      public Vector3 Rotation;
      public Vector3 Scale;

      internal MSBN.PartsType Type { get; private set; }

      public override string Name { get; set; }

      internal Part(BinaryReaderEx br)
      {
        long position = br.Position;
        int num = br.ReadInt32();
        this.Type = br.ReadEnum32<MSBN.PartsType>();
        br.ReadInt32();
        this.modelIndex = br.ReadInt32();
        br.ReadInt32();
        this.Position = br.ReadVector3();
        this.Rotation = br.ReadVector3();
        this.Scale = br.ReadVector3();
        this.Name = br.GetShiftJIS(position + (long) num);
      }

      internal virtual void GetNames(MSBN msb, MSBN.Entries entries)
      {
        this.ModelName = MSB.FindName<MSBN.Model>(entries.Models, this.modelIndex);
      }

      internal virtual void GetIndices(MSBN msb, MSBN.Entries entries)
      {
        this.modelIndex = MSB.FindIndex<MSBN.Model>(entries.Models, this.ModelName);
      }

      public override string ToString()
      {
        return string.Format("{0} : {1}", (object) this.Type, (object) this.Name);
      }
    }
  }
}
