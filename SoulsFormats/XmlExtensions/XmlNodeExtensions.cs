// Decompiled with JetBrains decompiler
// Type: SoulsFormats.XmlExtensions.XmlNodeExtensions
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Xml;

namespace SoulsFormats.XmlExtensions {
  internal static class XmlNodeExtensions {
    public static bool ReadBoolean(this XmlNode node, string xpath) {
      return bool.Parse(node.SelectSingleNode(xpath).InnerText);
    }

    public static bool ReadBooleanOrDefault(
        this XmlNode node,
        string xpath,
        bool def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? bool.Parse(xmlNode.InnerText) : def;
    }

    public static sbyte ReadSByte(this XmlNode node, string xpath) {
      return sbyte.Parse(node.SelectSingleNode(xpath).InnerText);
    }

    public static sbyte ReadSByteOrDefault(
        this XmlNode node,
        string xpath,
        sbyte def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? sbyte.Parse(xmlNode.InnerText) : def;
    }

    public static byte ReadByte(this XmlNode node, string xpath) {
      return byte.Parse(node.SelectSingleNode(xpath).InnerText);
    }

    public static byte ReadByteOrDefault(
        this XmlNode node,
        string xpath,
        byte def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? byte.Parse(xmlNode.InnerText) : def;
    }

    public static short ReadInt16(this XmlNode node, string xpath) {
      return short.Parse(node.SelectSingleNode(xpath).InnerText);
    }

    public static short ReadInt16OrDefault(
        this XmlNode node,
        string xpath,
        short def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? short.Parse(xmlNode.InnerText) : def;
    }

    public static ushort ReadUInt16(this XmlNode node, string xpath) {
      return ushort.Parse(node.SelectSingleNode(xpath).InnerText);
    }

    public static ushort ReadUInt16OrDefault(
        this XmlNode node,
        string xpath,
        ushort def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? ushort.Parse(xmlNode.InnerText) : def;
    }

    public static int ReadInt32(this XmlNode node, string xpath) {
      return int.Parse(node.SelectSingleNode(xpath).InnerText);
    }

    public static int ReadInt32OrDefault(
        this XmlNode node,
        string xpath,
        int def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? int.Parse(xmlNode.InnerText) : def;
    }

    public static uint ReadUInt32(this XmlNode node, string xpath) {
      return uint.Parse(node.SelectSingleNode(xpath).InnerText);
    }

    public static uint ReadUInt32OrDefault(
        this XmlNode node,
        string xpath,
        uint def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? uint.Parse(xmlNode.InnerText) : def;
    }

    public static float ReadSingle(this XmlNode node, string xpath) {
      return float.Parse(node.SelectSingleNode(xpath).InnerText);
    }

    public static float ReadSingle(
        this XmlNode node,
        string xpath,
        IFormatProvider provider) {
      return float.Parse(node.SelectSingleNode(xpath).InnerText, provider);
    }

    public static float ReadSingleOrDefault(
        this XmlNode node,
        string xpath,
        float def,
        IFormatProvider provider) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? float.Parse(xmlNode.InnerText, provider) : def;
    }

    public static float ReadSingleOrDefault(
        this XmlNode node,
        string xpath,
        float def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? float.Parse(xmlNode.InnerText) : def;
    }

    public static string ReadString(this XmlNode node, string xpath) {
      return node.SelectSingleNode(xpath).InnerText;
    }

    public static string ReadStringOrDefault(
        this XmlNode node,
        string xpath,
        string def) {
      XmlNode xmlNode = node.SelectSingleNode(xpath);
      return xmlNode != null ? xmlNode.InnerText : def;
    }
  }
}