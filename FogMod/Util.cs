// Decompiled with JetBrains decompiler
// Type: FogMod.Util
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Reflection;

namespace FogMod {
  public class Util {
    public static void AddMulti<K, V>(
        IDictionary<K, List<V>> dict,
        K key,
        V value) {
      if (!dict.ContainsKey(key))
        dict[key] = new List<V>();
      dict[key].Add(value);
    }

    public static void AddMulti<K, V>(
        IDictionary<K, List<V>> dict,
        K key,
        IEnumerable<V> values) {
      if (!dict.ContainsKey(key))
        dict[key] = new List<V>();
      dict[key].AddRange(values);
    }

    public static void AddMulti<K, V>(
        IDictionary<K, HashSet<V>> dict,
        K key,
        V value) {
      if (!dict.ContainsKey(key))
        dict[key] = new HashSet<V>();
      dict[key].Add(value);
    }

    public static void AddMulti<K, V>(
        IDictionary<K, HashSet<V>> dict,
        K key,
        IEnumerable<V> values) {
      if (!dict.ContainsKey(key))
        dict[key] = new HashSet<V>();
      dict[key].UnionWith(values);
    }

    public static void AddMulti<K, V>(
        IDictionary<K, SortedSet<V>> dict,
        K key,
        V value) {
      if (!dict.ContainsKey(key))
        dict[key] = new SortedSet<V>();
      dict[key].Add(value);
    }

    public static void Shuffle<T>(Random random, IList<T> list) {
      for (int minValue = 0; minValue < list.Count - 1; ++minValue) {
        int index = random.Next(minValue, list.Count);
        T obj = list[minValue];
        list[minValue] = list[index];
        list[index] = obj;
      }
    }

    public static void CopyAll<T>(T source, T target) {
      Type type = typeof(T);
      foreach (PropertyInfo property1 in type.GetProperties()) {
        PropertyInfo property2 = type.GetProperty(property1.Name);
        if (property1.CanWrite)
          property2.SetValue((object) target,
                             property1.GetValue(
                                 (object) source,
                                 (object[]) null),
                             (object[]) null);
        else if (property1.PropertyType.IsArray) {
          Array sourceArray = (Array) property1.GetValue((object) source);
          Array.Copy(sourceArray,
                     (Array) property2.GetValue((object) target),
                     sourceArray.Length);
        }
      }
    }

    public static int SearchBytes(byte[] array, byte[] candidate) {
      for (int position = 0; position < array.Length; ++position) {
        if (Util.IsMatch(array, position, candidate))
          return position;
      }
      return -1;
    }

    private static bool IsMatch(byte[] array, int position, byte[] candidate) {
      if (candidate.Length > array.Length - position)
        return false;
      for (int index = 0; index < candidate.Length; ++index) {
        if ((int) array[position + index] != (int) candidate[index])
          return false;
      }
      return true;
    }
  }
}