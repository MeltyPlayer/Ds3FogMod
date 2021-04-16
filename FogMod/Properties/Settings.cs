// Decompiled with JetBrains decompiler
// Type: FogMod.Properties.Settings
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FogMod.Properties {
  [CompilerGenerated]
  [GeneratedCode(
      "Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator",
      "15.9.0.0")]
  public sealed class Settings : ApplicationSettingsBase {
    private static Settings defaultInstance =
        (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default {
      get { return Settings.defaultInstance; }
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string Options {
      get { return (string) this[nameof(Options)]; }
      set { this[nameof(Options)] = (object) value; }
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string Exe {
      get { return (string) this[nameof(Exe)]; }
      set { this[nameof(Exe)] = (object) value; }
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string Language {
      get { return (string) this[nameof(Language)]; }
      set { this[nameof(Language)] = (object) value; }
    }
  }
}