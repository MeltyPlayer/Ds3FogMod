using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FogMod.io;

using SoulsFormats;

using SoulsIds;

namespace FogMod {
  public static class ParamsManager {
    private static Task<Dictionary<string, PARAM>> cache_;

    public static void ClearCache() => ParamsManager.cache_ = null;

    public static async Task<Dictionary<string, PARAM>> Get(
        string gameDir,
        Events events,
        GameEditor editor
    ) {
      if (ParamsManager.cache_ != null) {
        return await ParamsManager.cache_;
      }

      return await (ParamsManager.cache_ = Task.Run(async () => {
                       var fogdistDirectory =
                           new IoDirectory(editor.Spec.GameDir);
                       string path = fogdistDirectory.GetFile("Base\\Data0.bdt")
                           .FullName;
                       string altPath = $@"{gameDir}\Data0.bdt";
                       if (gameDir != null && File.Exists(altPath)) {
                         Console.WriteLine($"Using override {altPath}");
                         path = altPath;
                       }

                       // TODO: Slow
                       var layouts = await editor.LoadLayouts();
                       return await editor.LoadParams(path, layouts, true);
                     }));
    }
  }
}