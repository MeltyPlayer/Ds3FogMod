// Decompiled with JetBrains decompiler
// Type: FogMod.Properties.Resources
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace FogMod.Properties {
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder",
                 "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources() {}

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager {
      get {
        if (FogMod.Properties.Resources.resourceMan == null)
          FogMod.Properties.Resources.resourceMan = new ResourceManager(
              "FogMod.Properties.Resources",
              typeof(FogMod.Properties.Resources).Assembly);
        return FogMod.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture {
      get { return FogMod.Properties.Resources.resourceCulture; }
      set { FogMod.Properties.Resources.resourceCulture = value; }
    }
  }
}