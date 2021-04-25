using System;
using System.Collections.Generic;
using System.IO;

using SoulsFormats;

using SoulsIds;

namespace FogMod {
  public static class ParamsManager {
    private static Dictionary<string, PARAM> cache_;

    public static Dictionary<string, PARAM> Get(
        string gameDir,
        Events events,
        GameEditor editor
    ) {
      if (ParamsManager.cache_ != null) {
        return ParamsManager.cache_;
      }

      string path = @"fogdist\Base\Data0.bdt";
      string altPath = $@"{gameDir}\Data0.bdt";
      if (gameDir != null && File.Exists(altPath)) {
        Console.WriteLine($"Using override {altPath}");
        path = altPath;
      }

      // TODO: Slow
      var layouts = editor.LoadLayouts();
      return ParamsManager.cache_ = editor.LoadParams(path, layouts, true);
    }
  }
}