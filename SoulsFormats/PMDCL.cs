// Decompiled with JetBrains decompiler
// Type: SoulsFormats.PMDCL
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class PMDCL : SoulsFile<PMDCL> {
    public List<PMDCL.Decal> Decals;

    public PMDCL() {
      this.Decals = new List<PMDCL.Decal>();
    }

    protected override void Read(BinaryReaderEx br) {
      br.BigEndian = false;
      long num = br.ReadInt64();
      br.AssertInt64(32L);
      br.AssertInt64(new long[1]);
      br.AssertInt64(new long[1]);
      this.Decals = new List<PMDCL.Decal>((int) num);
      for (int index = 0; (long) index < num; ++index) {
        long offset = br.ReadInt64();
        br.StepIn(offset);
        this.Decals.Add(new PMDCL.Decal(br));
        br.StepOut();
      }
    }

    protected override void Write(BinaryWriterEx bw) {
      bw.BigEndian = false;
      bw.WriteInt64((long) this.Decals.Count);
      bw.WriteInt64(32L);
      bw.WriteInt64(0L);
      bw.WriteInt64(0L);
      for (int index = 0; index < this.Decals.Count; ++index)
        bw.ReserveInt64(string.Format("Decal{0}", (object) index));
      bw.Pad(32);
      for (int index = 0; index < this.Decals.Count; ++index) {
        bw.FillInt64(string.Format("Decal{0}", (object) index), bw.Position);
        this.Decals[index].Write(bw);
      }
    }

    public class Decal {
      public Vector3 XAngles;
      public Vector3 YAngles;
      public Vector3 ZAngles;
      public Vector3 Position;
      public float Unk3C;
      public int DecalParamID;
      public short Size1;
      public short Size2;

      public Decal(int decalParamID, Vector3 position) {
        this.XAngles = Vector3.Zero;
        this.YAngles = Vector3.Zero;
        this.ZAngles = Vector3.Zero;
        this.Position = position;
        this.Unk3C = 1f;
        this.DecalParamID = decalParamID;
        this.Size1 = (short) 10;
        this.Size2 = (short) 10;
      }

      public Decal(PMDCL.Decal clone) {
        this.XAngles = clone.XAngles;
        this.YAngles = clone.YAngles;
        this.ZAngles = clone.ZAngles;
        this.Position = clone.Position;
        this.Unk3C = clone.Unk3C;
        this.DecalParamID = clone.DecalParamID;
        this.Size1 = clone.Size1;
        this.Size2 = clone.Size2;
      }

      internal Decal(BinaryReaderEx br) {
        this.XAngles = br.ReadVector3();
        br.AssertInt32(new int[1]);
        this.YAngles = br.ReadVector3();
        br.AssertInt32(new int[1]);
        this.ZAngles = br.ReadVector3();
        br.AssertInt32(new int[1]);
        this.Position = br.ReadVector3();
        this.Unk3C = br.ReadSingle();
        this.DecalParamID = br.ReadInt32();
        this.Size1 = br.ReadInt16();
        this.Size2 = br.ReadInt16();
        br.AssertInt64(new long[1]);
        br.AssertInt64(new long[1]);
        br.AssertInt64(new long[1]);
      }

      internal void Write(BinaryWriterEx bw) {
        bw.WriteVector3(this.XAngles);
        bw.WriteInt32(0);
        bw.WriteVector3(this.YAngles);
        bw.WriteInt32(0);
        bw.WriteVector3(this.ZAngles);
        bw.WriteInt32(0);
        bw.WriteVector3(this.Position);
        bw.WriteSingle(this.Unk3C);
        bw.WriteInt32(this.DecalParamID);
        bw.WriteInt16(this.Size1);
        bw.WriteInt16(this.Size2);
        bw.WriteInt64(0L);
        bw.WriteInt64(0L);
        bw.WriteInt64(0L);
      }
    }
  }
}