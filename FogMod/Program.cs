// Decompiled with JetBrains decompiler
// Type: FogMod.Program
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsIds;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FogMod {
  public static class Program {
    private const int ATTACH_PARENT_PROCESS = -1;

    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);

    [STAThread]
    private static async Task Main(string[] args) {
      if (args.Length != 0 &&
          !((IEnumerable<string>) args).Contains<string>("/gui")) {
        Program.AttachConsole(-1);
        RandomizerOptions opt = new RandomizerOptions() {
            Seed = new Random().Next()
        };
        foreach (string s in args) {
          uint result;
          if (uint.TryParse(s, out result))
            opt.Seed = (int) result;
          else
            opt[s] = true;
        }
        GameSpec.FromGame fromGame =
            ((IEnumerable<string>) args).Contains<string>("ptde")
                ? GameSpec.FromGame.DS1
                : GameSpec.FromGame.DS1R;
        GameSpec.FromGame game = GameSpec.FromGame.DS3;
        opt.Game = game;
        string gameDir = GameSpec.ForGame(game).GameDir;

        var editor = new GameEditor(game);
        editor.Spec.GameDir = @"fogdist";
        editor.Spec.LayoutDir = @"fogdist\Layouts";
        editor.Spec.NameDir = @"fogdist\Names";

        if (game == GameSpec.FromGame.DS3)
          await new Randomizer().Randomize(opt,
                                     game,
                                     editor,
                                     opt["mergemods"]
                                         ? gameDir + "\\randomizer"
                                         : (string) null,
                                     gameDir + "\\fog");
        else
          await new Randomizer().Randomize(opt, game, editor, gameDir, gameDir);
      } else {
        CommandLineFlags.Populate(args);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run((Form) new MainForm3());
      }
    }
  }
}