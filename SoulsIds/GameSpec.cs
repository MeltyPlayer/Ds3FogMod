// Decompiled with JetBrains decompiler
// Type: SoulsIds.GameSpec
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsFormats;

using System.Collections.Generic;

namespace SoulsIds {
  public class GameSpec {
    private static readonly Dictionary<GameSpec.FromGame, GameSpec> Games =
        new Dictionary<GameSpec.FromGame, GameSpec>() {
            [GameSpec.FromGame.UNKNOWN] = new GameSpec(),
            [GameSpec.FromGame.DS1] = new GameSpec() {
                Dcx = DCX.Type.None,
                GameDir =
                    "C:\\Program Files (x86)\\Steam\\SteamApps\\common\\Dark Souls Prepare to Die Edition\\DATA",
                EsdDir = "script\\talk",
                MsgDir = "msg\\ENGLISH",
                MsbDir = "map\\MapStudio",
                ParamFile = "param\\GameParam\\GameParam.parambnd",
                NameDir = "dist\\DS1R\\Names",
                LayoutDir = "dist\\DS1\\Layouts"
            },
            [GameSpec.FromGame.DS1R] = new GameSpec() {
                Dcx = DCX.Type.DCX_DFLT_10000_24_9,
                GameDir =
                    "C:\\Program Files (x86)\\Steam\\steamapps\\common\\DARK SOULS REMASTERED",
                EsdDir = "script\\talk",
                MsgDir = "msg\\ENGLISH",
                MsbDir = "map\\MapStudio",
                ParamFile = "param\\GameParam\\GameParam.parambnd.dcx",
                NameDir = "dist\\DS1R\\Names",
                LayoutDir = "dist\\DS1R\\Layouts"
            },
            [GameSpec.FromGame.DS2] = new GameSpec() {
                Dcx = DCX.Type.DCX_DFLT_10000_24_9,
                GameDir =
                    "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Dark Souls II\\Game",
                EsdDir = "ezstate",
                MsgDir = "menu\\text\\english",
                MsbDir = "map",
                ParamFile = "gameparam_dlc2.parambnd.dcx",
                NameDir = "dist\\DS2S\\Names",
                LayoutDir = "dist\\DS2S\\Layouts"
            },
            [GameSpec.FromGame.DS2S] = new GameSpec() {
                Dcx = DCX.Type.DCX_DFLT_10000_24_9,
                GameDir =
                    "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Dark Souls II Scholar of the First Sin\\Game",
                EsdDir = "ezstate",
                MsgDir = "menu\\text\\english",
                MsbDir = "map",
                ParamFile = "gameparam_dlc2.parambnd.dcx",
                NameDir = "dist\\DS2S\\Names",
                LayoutDir = "dist\\DS2S\\Layouts"
            },
            [GameSpec.FromGame.BB] = new GameSpec() {
                Dcx = DCX.Type.DCX_DFLT_10000_44_9
            },
            [GameSpec.FromGame.DS3] = new GameSpec() {
                Dcx = DCX.Type.DCX_DFLT_10000_44_9,
                GameDir =
                    "C:\\Program Files (x86)\\Steam\\steamapps\\common\\DARK SOULS III\\Game",
                EsdDir = "script\\talk",
                MsgDir = "msg\\engus",
                MsbDir = "map\\mapstudio",
                ParamFile = "Data0.bdt",
                NameDir = "dist\\DS3\\Names",
                LayoutDir = "dist\\DS3\\Layouts"
            },
            [GameSpec.FromGame.SDT] = new GameSpec() {
                Dcx = DCX.Type.DCX_KRAK,
                GameDir =
                    "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Sekiro",
                EsdDir = "script\\talk",
                MsgDir = "msg\\engus",
                MsbDir = "map\\mapstudio",
                ParamFile = "param\\GameParam\\GameParam.parambnd.dcx",
                NameDir = "dist\\SDT\\Names",
                LayoutDir = "dist\\SDT\\Layouts"
            }
        };

    public GameSpec.FromGame Game { get; set; }

    public string GameDir { get; set; }

    public string EsdDir { get; set; }

    public string MsgDir { get; set; }

    public string MsbDir { get; set; }

    public string ParamFile { get; set; }

    public DCX.Type Dcx { get; set; }

    public string NameDir { get; set; }

    public string LayoutDir { get; set; }

    public GameSpec Clone() {
      return (GameSpec) this.MemberwiseClone();
    }

    public static GameSpec ForGame(GameSpec.FromGame game) {
      GameSpec gameSpec1;
      GameSpec gameSpec2 = GameSpec.Games.TryGetValue(game, out gameSpec1)
                               ? gameSpec1.Clone()
                               : new GameSpec();
      gameSpec2.Game = game;
      return gameSpec2;
    }

    public enum FromGame {
      UNKNOWN,
      DS1,
      DS1R,
      DS2,
      DS2S,
      BB,
      DS3,
      SDT,
    }
  }
}