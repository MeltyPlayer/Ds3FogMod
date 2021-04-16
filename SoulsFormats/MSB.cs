// Decompiled with JetBrains decompiler
// Type: SoulsFormats.MSB
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SoulsFormats
{
  internal static class MSB
  {
    public static void DisambiguateNames<T>(List<T> entries) where T : IMsbEntry
    {
      bool flag;
      do
      {
        flag = false;
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        foreach (T entry in entries)
        {
          IMsbEntry msbEntry = (IMsbEntry) entry;
          string name = msbEntry.Name;
          if (!dictionary.ContainsKey(name))
          {
            dictionary[name] = 1;
          }
          else
          {
            flag = true;
            dictionary[name]++;
            msbEntry.Name = string.Format("{0} {{{1}}}", (object) name, (object) dictionary[name]);
          }
        }
      }
      while (flag);
    }

    public static string ReambiguateName(string name)
    {
      return Regex.Replace(name, " \\{\\d+\\}", "");
    }

    public static string FindName<T>(List<T> list, int index) where T : IMsbEntry
    {
      return index == -1 ? (string) null : list[index].Name;
    }

    public static string[] FindNames<T>(List<T> list, int[] indices) where T : IMsbEntry
    {
      string[] strArray = new string[indices.Length];
      for (int index = 0; index < indices.Length; ++index)
        strArray[index] = MSB.FindName<T>(list, indices[index]);
      return strArray;
    }

    public static int FindIndex<T>(List<T> list, string name) where T : IMsbEntry
    {
      if (name == null)
        return -1;
      int index = list.FindIndex((Predicate<T>) (entry => entry.Name == name));
      if (index != -1)
        return index;
      throw new KeyNotFoundException("Name not found: " + name);
    }

    public static int[] FindIndices<T>(List<T> list, string[] names) where T : IMsbEntry
    {
      int[] numArray = new int[names.Length];
      for (int index = 0; index < names.Length; ++index)
        numArray[index] = MSB.FindIndex<T>(list, names[index]);
      return numArray;
    }
  }
}
