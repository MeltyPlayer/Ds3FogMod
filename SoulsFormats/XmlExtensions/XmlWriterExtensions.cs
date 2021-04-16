// Decompiled with JetBrains decompiler
// Type: SoulsFormats.XmlExtensions.XmlWriterExtensions
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Xml;

namespace SoulsFormats.XmlExtensions {
  internal static class XmlWriterExtensions {
    public static void WriteDefaultElement<T>(
        this XmlWriter xw,
        string localName,
        T value,
        T defaultValue)
        where T : IEquatable<T> {
      if (object.Equals((object) value, (object) defaultValue))
        return;
      xw.WriteElementString(localName, value.ToString());
    }

    public static void WriteDefaultElement(
        this XmlWriter xw,
        string localName,
        float value,
        float defaultValue,
        IFormatProvider provider) {
      if ((double) value == (double) defaultValue)
        return;
      xw.WriteElementString(localName, value.ToString(provider));
    }

    public static void WriteDefaultElement(
        this XmlWriter xw,
        string localName,
        float value,
        float defaultValue,
        string format,
        IFormatProvider provider) {
      if ((double) value == (double) defaultValue)
        return;
      xw.WriteElementString(localName, value.ToString(format, provider));
    }
  }
}