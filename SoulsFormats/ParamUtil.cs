// Decompiled with JetBrains decompiler
// Type: SoulsFormats.ParamUtil
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;

namespace SoulsFormats
{
  internal static class ParamUtil
  {
    public static string GetDefaultFormat(PARAMDEF.DefType type)
    {
      switch (type)
      {
        case PARAMDEF.DefType.s8:
          return "%d";
        case PARAMDEF.DefType.u8:
          return "%d";
        case PARAMDEF.DefType.s16:
          return "%d";
        case PARAMDEF.DefType.u16:
          return "%d";
        case PARAMDEF.DefType.s32:
          return "%d";
        case PARAMDEF.DefType.u32:
          return "%d";
        case PARAMDEF.DefType.f32:
          return "%f";
        case PARAMDEF.DefType.dummy8:
          return "";
        case PARAMDEF.DefType.fixstr:
          return "%d";
        case PARAMDEF.DefType.fixstrW:
          return "%d";
        default:
          throw new NotImplementedException(string.Format("No default format specified for {0}.{1}", (object) "DefType", (object) type));
      }
    }

    public static float GetDefaultMinimum(PARAMDEF.DefType type)
    {
      switch (type)
      {
        case PARAMDEF.DefType.s8:
          return (float) sbyte.MinValue;
        case PARAMDEF.DefType.u8:
          return 0.0f;
        case PARAMDEF.DefType.s16:
          return (float) short.MinValue;
        case PARAMDEF.DefType.u16:
          return 0.0f;
        case PARAMDEF.DefType.s32:
          return (float) int.MinValue;
        case PARAMDEF.DefType.u32:
          return 0.0f;
        case PARAMDEF.DefType.f32:
          return float.MinValue;
        case PARAMDEF.DefType.dummy8:
          return 0.0f;
        case PARAMDEF.DefType.fixstr:
          return -1f;
        case PARAMDEF.DefType.fixstrW:
          return -1f;
        default:
          throw new NotImplementedException(string.Format("No default minimum specified for {0}.{1}", (object) "DefType", (object) type));
      }
    }

    public static float GetDefaultMaximum(PARAMDEF.DefType type)
    {
      switch (type)
      {
        case PARAMDEF.DefType.s8:
          return (float) sbyte.MaxValue;
        case PARAMDEF.DefType.u8:
          return (float) byte.MaxValue;
        case PARAMDEF.DefType.s16:
          return (float) short.MaxValue;
        case PARAMDEF.DefType.u16:
          return (float) ushort.MaxValue;
        case PARAMDEF.DefType.s32:
          return (float) int.MaxValue;
        case PARAMDEF.DefType.u32:
          return (float) uint.MaxValue;
        case PARAMDEF.DefType.f32:
          return float.MaxValue;
        case PARAMDEF.DefType.dummy8:
          return 0.0f;
        case PARAMDEF.DefType.fixstr:
          return 1E+09f;
        case PARAMDEF.DefType.fixstrW:
          return 1E+09f;
        default:
          throw new NotImplementedException(string.Format("No default maximum specified for {0}.{1}", (object) "DefType", (object) type));
      }
    }

    public static float GetDefaultIncrement(PARAMDEF.DefType type)
    {
      switch (type)
      {
        case PARAMDEF.DefType.s8:
          return 1f;
        case PARAMDEF.DefType.u8:
          return 1f;
        case PARAMDEF.DefType.s16:
          return 1f;
        case PARAMDEF.DefType.u16:
          return 1f;
        case PARAMDEF.DefType.s32:
          return 1f;
        case PARAMDEF.DefType.u32:
          return 1f;
        case PARAMDEF.DefType.f32:
          return 0.01f;
        case PARAMDEF.DefType.dummy8:
          return 0.0f;
        case PARAMDEF.DefType.fixstr:
          return 1f;
        case PARAMDEF.DefType.fixstrW:
          return 1f;
        default:
          throw new NotImplementedException(string.Format("No default increment specified for {0}.{1}", (object) "DefType", (object) type));
      }
    }

    public static PARAMDEF.EditFlags GetDefaultEditFlags(PARAMDEF.DefType type)
    {
      switch (type)
      {
        case PARAMDEF.DefType.s8:
          return PARAMDEF.EditFlags.Wrap;
        case PARAMDEF.DefType.u8:
          return PARAMDEF.EditFlags.Wrap;
        case PARAMDEF.DefType.s16:
          return PARAMDEF.EditFlags.Wrap;
        case PARAMDEF.DefType.u16:
          return PARAMDEF.EditFlags.Wrap;
        case PARAMDEF.DefType.s32:
          return PARAMDEF.EditFlags.Wrap;
        case PARAMDEF.DefType.u32:
          return PARAMDEF.EditFlags.Wrap;
        case PARAMDEF.DefType.f32:
          return PARAMDEF.EditFlags.Wrap;
        case PARAMDEF.DefType.dummy8:
          return PARAMDEF.EditFlags.None;
        case PARAMDEF.DefType.fixstr:
          return PARAMDEF.EditFlags.Wrap;
        case PARAMDEF.DefType.fixstrW:
          return PARAMDEF.EditFlags.Wrap;
        default:
          throw new NotImplementedException(string.Format("No default edit flags specified for {0}.{1}", (object) "DefType", (object) type));
      }
    }

    public static bool IsArrayType(PARAMDEF.DefType type)
    {
      switch (type)
      {
        case PARAMDEF.DefType.dummy8:
        case PARAMDEF.DefType.fixstr:
        case PARAMDEF.DefType.fixstrW:
          return true;
        default:
          return false;
      }
    }

    public static bool IsBitType(PARAMDEF.DefType type)
    {
      switch (type)
      {
        case PARAMDEF.DefType.u8:
        case PARAMDEF.DefType.u16:
        case PARAMDEF.DefType.u32:
        case PARAMDEF.DefType.dummy8:
          return true;
        default:
          return false;
      }
    }

    public static int GetValueSize(PARAMDEF.DefType type)
    {
      switch (type)
      {
        case PARAMDEF.DefType.s8:
          return 1;
        case PARAMDEF.DefType.u8:
          return 1;
        case PARAMDEF.DefType.s16:
          return 2;
        case PARAMDEF.DefType.u16:
          return 2;
        case PARAMDEF.DefType.s32:
          return 4;
        case PARAMDEF.DefType.u32:
          return 4;
        case PARAMDEF.DefType.f32:
          return 4;
        case PARAMDEF.DefType.dummy8:
          return 1;
        case PARAMDEF.DefType.fixstr:
          return 1;
        case PARAMDEF.DefType.fixstrW:
          return 2;
        default:
          throw new NotImplementedException(string.Format("No value size specified for {0}.{1}", (object) "DefType", (object) type));
      }
    }

    public static object CastDefaultValue(PARAMDEF.Field field)
    {
      switch (field.DisplayType)
      {
        case PARAMDEF.DefType.s8:
          return (object) (sbyte) field.Default;
        case PARAMDEF.DefType.u8:
          return (object) (byte) field.Default;
        case PARAMDEF.DefType.s16:
          return (object) (short) field.Default;
        case PARAMDEF.DefType.u16:
          return (object) (ushort) field.Default;
        case PARAMDEF.DefType.s32:
          return (object) (int) field.Default;
        case PARAMDEF.DefType.u32:
          return (object) (uint) field.Default;
        case PARAMDEF.DefType.f32:
          return (object) field.Default;
        case PARAMDEF.DefType.dummy8:
          return field.BitSize == -1 ? (object) new byte[field.ArrayLength] : (object) (byte) field.Default;
        case PARAMDEF.DefType.fixstr:
          return (object) "";
        case PARAMDEF.DefType.fixstrW:
          return (object) "";
        default:
          throw new NotImplementedException(string.Format("Default not implemented for type {0}", (object) field.DisplayType));
      }
    }

    public static int GetBitLimit(PARAMDEF.DefType type)
    {
      if (type == PARAMDEF.DefType.u8)
        return 8;
      if (type == PARAMDEF.DefType.u16)
        return 16;
      if (type == PARAMDEF.DefType.u32)
        return 32;
      throw new InvalidOperationException("Bit type may only be u8, u16, or u32.");
    }
  }
}
