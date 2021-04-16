// Decompiled with JetBrains decompiler
// Type: FogMod.Randomizer
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsIds;

using System;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.Serialization;

namespace FogMod {
  public class Randomizer {
    public ItemReader.Result Randomize(
        RandomizerOptions opt,
        GameSpec.FromGame game,
        string gameDir,
        string outDir) {
      Console.WriteLine(string.Format("Seed: {0}. Options: {1}",
                                      (object) opt.DisplaySeed,
                                      (object) string.Join(
                                          " ",
                                          (IEnumerable<string>) opt
                                              .GetEnabled())));
      string path = game == GameSpec.FromGame.DS3
                        ? "fogdist\\fog.txt"
                        : "dist\\fog.txt";
      IDeserializer deserializer = new DeserializerBuilder().Build();
      AnnotationData ann;
      using (StreamReader streamReader = File.OpenText(path))
        ann = deserializer.Deserialize<AnnotationData>(
            (TextReader) streamReader);
      ann.SetGame(game);
      Events events = (Events) null;
      if (game == GameSpec.FromGame.DS3) {
        using (StreamReader streamReader =
            File.OpenText("fogdist\\locations.txt"))
          ann.Locations =
              deserializer.Deserialize<AnnotationData.FogLocations>(
                  (TextReader) streamReader);
        events = new Events("fogdist\\Base\\ds3-common.emedf.json");
      }
      Graph g = new Graph();
      g.Construct(opt, ann);
      ItemReader.Result items =
          new ItemReader().FindItems(opt, ann, g, events, gameDir, game);
      Console.WriteLine(items.Randomized
                            ? "Key item hash: " + items.ItemHash
                            : "No key items randomized");
      Console.WriteLine();
      
      GraphConnector.Randomize(opt, g, ann);

      if (opt["bonedryrun"])
        return items;
      Console.WriteLine();
      if (game == GameSpec.FromGame.DS3) {
        EventConfig eventConfig;
        using (StreamReader streamReader = File.OpenText("fogdist\\events.txt"))
          eventConfig =
              deserializer.Deserialize<EventConfig>((TextReader) streamReader);
        if (opt["eventsyaml"] || opt["events"]) {
          new GenerateConfig().WriteEventConfig(ann, events, opt);
          return items;
        }
        new GameDataWriter3().Write(opt,
                                    ann,
                                    g,
                                    gameDir,
                                    outDir,
                                    events,
                                    eventConfig);
      } else {
        if (opt["dryrun"]) {
          Console.WriteLine("Success (dry run)");
          return items;
        }
        new GameDataWriter().Write(opt, ann, g, gameDir, game);
      }
      return items;
    }
  }
}