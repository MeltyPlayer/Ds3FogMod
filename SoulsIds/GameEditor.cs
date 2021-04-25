// Decompiled with JetBrains decompiler
// Type: SoulsIds.GameEditor
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsFormats;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SoulsIds {
  public class GameEditor {
    public readonly GameSpec Spec;

    public GameEditor(GameSpec.FromGame game) {
      this.Spec = GameSpec.ForGame(game);
    }

    public GameEditor(GameSpec spec) {
      this.Spec = spec;
    }

    public Dictionary<string, PARAM> LoadParams(
        Dictionary<string, PARAM.Layout> layouts = null,
        bool allowError = false) {
      if (this.Spec.ParamFile == null)
        throw new Exception("Param path unknown");
      return this.LoadParams(this.Spec.GameDir + "\\" + this.Spec.ParamFile,
                             layouts,
                             allowError);
    }

    public Dictionary<string, PARAM> LoadParams(
        string path,
        Dictionary<string, PARAM.Layout> layouts = null,
        bool allowError = false) {
      layouts ??= this.LoadLayouts();
      return this.LoadBnd<PARAM>(
          path,
          (data, paramPath) => {
            PARAM obj;
            try {
              obj = SoulsFile<PARAM>.Read(data);
            } catch (Exception ex) {
              if (!allowError)
                throw new Exception("Failed to load param " + paramPath + ": " + (object) ex);
              return null;
            }
            if (layouts == null)
              return obj;
            if (layouts.ContainsKey(obj.ParamType)
            ) {
              PARAM.Layout layout = layouts[obj.ParamType];
              if ((long) layout.Size == obj.DetectedSize) {
                obj.ApplyParamdef(
                    layout.ToParamdef(obj.ParamType, out List<PARAMTDF> _));
                return obj;
              }
            }
            return null;
          },
          (string) null);
    }

    public Dictionary<T, string> LoadNames<T>(
        string name,
        Func<string, T> key,
        bool allowMissing = false) {
      if (this.Spec.NameDir == null)
        throw new Exception("Name file dir not provided");
      Dictionary<T, string> dictionary = new Dictionary<T, string>();
      string path = this.Spec.NameDir + "\\" + name + ".txt";
      if (allowMissing && !File.Exists(path))
        return new Dictionary<T, string>();
      foreach (string readLine in File.ReadLines(path)) {
        int length = readLine.IndexOf(' ');
        if (length == -1)
          throw new Exception("Bad line " + readLine + " in " + path);
        string str1 = readLine.Substring(0, length);
        string str2 = readLine.Substring(length + 1);
        dictionary[key(str1)] = str2;
      }
      return dictionary;
    }

    private Dictionary<string, PARAM.Layout> layoutsCache_;
    public Dictionary<string, PARAM.Layout> LoadLayouts() {
      if (this.layoutsCache_ != null) {
        return this.layoutsCache_;
      }

      if (this.Spec.LayoutDir == null)
        throw new Exception("Layout dir not provided");
      Dictionary<string, PARAM.Layout> dictionary =
          new Dictionary<string, PARAM.Layout>();
      foreach (string file in Directory.GetFiles(this.Spec.LayoutDir, "*.xml")
      ) {
        string withoutExtension = Path.GetFileNameWithoutExtension(file);
        PARAM.Layout layout = PARAM.Layout.ReadXMLFile(file);
        dictionary[withoutExtension] = layout;
      }
      return this.layoutsCache_ = dictionary;
    }

    public Dictionary<string, T> Load<T>(
        string relDir,
        Func<string, T> reader,
        string ext = "*.dcx") {
      if (this.Spec.GameDir == null)
        throw new Exception("Base game dir not provided");
      Dictionary<string, T> dictionary = new Dictionary<string, T>();
      foreach (string file in Directory.GetFiles(
          this.Spec.GameDir + "\\" + relDir,
          ext)) {
        string index = GameEditor.BaseName(file);
        try {
          dictionary[index] = reader(file);
        } catch (Exception ex) {
          throw new Exception(string.Format("Failed to load {0}: {1}",
                                            (object) file,
                                            (object) ex));
        }
      }
      return dictionary;
    }

    public Dictionary<string, T> LoadBnd<T>(
        string path,
        Func<byte[], string, T> parser,
        string fileExt = null) {
      GameEditor.BaseName(path);
      Dictionary<string, T> dictionary = new Dictionary<string, T>();
      IBinder binder;
      try {
        binder = this.ReadBnd(path);
      } catch (Exception ex) {
        throw new Exception(string.Format("Failed to load {0}: {1}",
                                          (object) path,
                                          (object) ex));
      }

      foreach (var file in binder.Files) {
        if (fileExt == null || file.Name.EndsWith(fileExt)) {
          string index = GameEditor.BaseName(file.Name);
          try {
            T obj = parser(file.Bytes, index);
            if (obj != null)
              dictionary[index] = obj;
          } catch (Exception ex) {
            throw new Exception($"Failed to load {path}: {index}: {ex}");
          }
        }
      }

      return dictionary;
    }

    public Dictionary<string, Dictionary<string, T>> LoadBnds<T>(
        string relDir,
        Func<byte[], string, T> parser,
        string ext = "*bnd.dcx",
        string fileExt = null) {
      if (this.Spec.GameDir == null)
        throw new Exception("Base game dir not provided");
      Dictionary<string, Dictionary<string, T>> dictionary1 =
          new Dictionary<string, Dictionary<string, T>>();
      foreach (string file in Directory.GetFiles(
          this.Spec.GameDir + "\\" + relDir,
          ext)) {
        string index = GameEditor.BaseName(file);
        var dictionary2 = this.LoadBnd(file, parser, fileExt);
        if (dictionary2.Count > 0)
          dictionary1[index] = dictionary2;
      }
      return dictionary1;
    }

    private IBinder ReadBnd(string path) {
      try {
        if (SoulsFile<BND3>.Is(path))
          return (IBinder) SoulsFile<BND3>.Read(path);
        if (SoulsFile<BND4>.Is(path))
          return (IBinder) SoulsFile<BND4>.Read(path);
        if (this.Spec.Game == GameSpec.FromGame.DS3 &&
            path.EndsWith("Data0.bdt"))
          return (IBinder) SFUtil.DecryptDS3Regulation(path);
        throw new Exception(string.Format(
                                "Unrecognized bnd format for game {0}: {1}",
                                (object) this.Spec.Game,
                                (object) path));
      } catch (Exception ex) {
        throw new Exception(string.Format("Failed to load {0}: {1}",
                                          (object) path,
                                          (object) ex));
      }
    }

    public void OverrideBnd<T>(
        string path,
        string toDir,
        Dictionary<string, T> diffData,
        Func<T, byte[]> writer,
        string fileExt = null) {
      string fileName = Path.GetFileName(path);
      string outPath =
          GameEditor.AbsolutePath(this.Spec.GameDir, toDir + "\\" + fileName);
      this.OverrideBndRel<T>(path, outPath, diffData, writer, fileExt);
    }

    public void OverrideBndRel<T>(
        string path,
        string outPath,
        Dictionary<string, T> diffData,
        Func<T, byte[]> writer,
        string fileExt = null) {
      if (this.Spec.Dcx == DCX.Type.Unknown)
        throw new Exception("DCX encoding not provided");
      Path.GetFileName(path);
      IBinder binder = this.ReadBnd(path);

      foreach (BinderFile file in binder.Files) {
        if (fileExt == null || file.Name.EndsWith(fileExt)) {
          string key = GameEditor.BaseName(file.Name);

          diffData.TryGetValue(key, out var value);
          if (value != null) {
            try {
              file.Bytes = writer(value);
            } catch (Exception ex) {
              Console.WriteLine($"Failed to load {path}: {key}: {ex}");
            }
          }
        }
      }

      if (binder is BND4 bnd) {
        if (this.Spec.Game == GameSpec.FromGame.DS3 &&
            outPath.EndsWith("Data0.bdt")) {
          SFUtil.EncryptDS3Regulation(outPath, bnd);
          if (new FileInfo(outPath).Length > 1048588L) {
            File.Delete(outPath);
            throw new Exception(
                "You must set loadLooseParams=1 in modengine.ini. Otherwise Data0.bdt is too large and will permanently corrupt your save file.");
          }
        } else
          bnd.Write(outPath, this.Spec.Dcx);
      }
      if (!(binder is BND3 bnD3))
        return;
      bnD3.Write(outPath, this.Spec.Dcx);
    }

    public void OverrideBnds<T>(
        string fromDir,
        string toDir,
        Dictionary<string, Dictionary<string, T>> diffBnds,
        Func<T, byte[]> writer,
        string ext = "*bnd.dcx",
        string fileExt = null) {
      if (this.Spec.GameDir == null)
        throw new Exception("Base game dir not provided");

      var files = Directory.GetFiles(this.Spec.GameDir + "\\" + fromDir, ext);
      foreach(var file in files) {
        string key = GameEditor.BaseName(file);
        if (diffBnds.ContainsKey(key))
          this.OverrideBnd<T>(file, toDir, diffBnds[key], writer, fileExt);
      }
    }

    public static string BaseName(string path) {
      path = Path.GetFileName(path);
      return path.IndexOf('.') == -1
                 ? path
                 : path.Substring(0, path.IndexOf('.'));
    }

    public static string AbsolutePath(string basePath, string maybeRelPath) {
      return basePath == null
                 ? Path.GetFullPath(maybeRelPath)
                 : Path.GetFullPath(Path.Combine(basePath, maybeRelPath));
    }

    public static void CopyRow(PARAM.Row from, PARAM.Row to) {
      for (int index = 0; index < from.Cells.Count; ++index)
        to.Cells[index].Value = from.Cells[index].Value;
    }
  }
}