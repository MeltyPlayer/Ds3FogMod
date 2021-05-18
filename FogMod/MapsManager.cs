using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FogMod.io;
using FogMod.util.time;

using SoulsFormats;

using SoulsIds;

namespace FogMod {
  public class MapsManager {
    private static Task<Dictionary<string, MSB3>> cache_;

    public static async Task<Dictionary<string, MSB3>> Get(
        string gameDir,
        AnnotationData ann,
        GameEditor editor) {
      if (MapsManager.cache_ != null) {
        return await MapsManager.cache_;
      }

      return await (MapsManager.cache_ = Task.Run(() => {
                       var stopwatch = new Stopwatch();
                       stopwatch.Start();

                       var fogdistDirectory =
                           new IoDirectory(editor.Spec.GameDir);
                       var baseDirectory = fogdistDirectory.GetSubdir("Base");

                       // Overrides where only one copy is needed
                       var msbFiles = baseDirectory.GetFiles("*.msb.dcx");
                       var maps = new Dictionary<string, MSB3>();
                       foreach (var basePath in msbFiles) {
                         string path = basePath.FullName;
                         string name = GameEditor.BaseName(path);
                         if (!ann.Specs.ContainsKey(name)) {
                           continue;
                         }

                         string altPath =
                             $@"{gameDir}\map\mapstudio\{name}.msb.dcx";
                         if (gameDir != null && File.Exists(altPath)) {
                           Console.WriteLine($"Using override {altPath}");
                           path = altPath;
                         }

                         // TODO: Slow
                         maps[name] = MSB3.Read(path);
                       }
                       stopwatch.ResetAndPrint("  Loading maps");

                       return maps;
                     }));
    }
  }
}