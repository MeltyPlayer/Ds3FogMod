// Decompiled with JetBrains decompiler
// Type: FogMod.EventConfig
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System.Collections.Generic;

namespace FogMod {
  public class EventConfig {
    public List<EventConfig.NewEvent> NewEvents { get; set; }

    public List<EventConfig.EventSpec> Events { get; set; }

    public class NewEvent {
      public int ID { get; set; }

      public string Name { get; set; }

      public string Comment { get; set; }

      public List<string> Commands { get; set; }
    }

    public class EventSpec : SoulsIds.Events.AbstractEventSpec {
      public List<EventConfig.EventTemplate> Template { get; set; }
    }

    public class EventTemplate {
      public string Fog { get; set; }

      public string FogSfx { get; set; }

      public string Warp { get; set; }

      public string Sfx { get; set; }

      public string SetFlag { get; set; }

      public string SetFlagIf { get; set; }

      public string SetFlagArea { get; set; }

      public string WarpReplace { get; set; }

      public int RepeatWarpObject { get; set; }

      public int RepeatWarpFlag { get; set; }

      public int CopyTo { get; set; }

      public string Remove { get; set; }

      public List<SoulsIds.Events.EventAddCommand> Add { get; set; }

      public string Replace { get; set; }
    }
  }
}