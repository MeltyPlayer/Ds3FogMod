using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FogMod.util.math {
  public class Vector3d {
    public Vector3d(Vector3 vector) {
      this.X = vector.X;
      this.Y = vector.Y;
      this.Z = vector.Z;
    }

    public Vector3d(double x, double y, double z) {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Vector3 ToVector3()
      => new Vector3(this.Convert(this.X), this.Convert(this.Y), this.Convert(this.Z));

    private static readonly Dictionary<float, bool> GARBOS = new Dictionary<float, bool>();

    static Vector3d() {
      GARBOS[27.61277f] = true;
      GARBOS[399.8125f] = true;
      GARBOS[61.42861f] = false;
    }

    private float Convert(double value) {
      float fValue = (float) value;

      foreach (var garbo in Vector3d.GARBOS) {
        var garboValue = garbo.Key;

        if (Math.Abs(value - garboValue) < .0001) {
          return garbo.Value ? this.NextAfter(fValue, float.MaxValue) :
            this.NextAfter(fValue, float.MinValue);
        }
      }

      return fValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct FloatIntUnion {
      [FieldOffset(0)]
      public int i;
      [FieldOffset(0)]
      public float f;
    }

    //  Returns the next float after x in the direction of y.
    float NextAfter(float x, float y) {
      if (float.IsNaN(x) || float.IsNaN(y)) return x + y;
      if (x == y) return y;  // nextafter(0, -0) = -0

      FloatIntUnion u;
      u.i = 0; u.f = x;  // shut up the compiler

      if (x == 0) {
        u.i = 1;
        return y > 0 ? u.f : -u.f;
      }

      if ((x > 0) == (y > x))
        u.i++;
      else
        u.i--;
      return u.f;
    }
  }
}
