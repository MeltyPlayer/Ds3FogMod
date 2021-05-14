// Decompiled with JetBrains decompiler
// Type: FogMod.Randomizer
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsIds;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FogMod.io;
using FogMod.util.time;

using YamlDotNet.Serialization;

namespace FogMod {
  public class Randomizer {
    public async Task<ItemReader.Result> Randomize(
        RandomizerOptions opt,
        GameSpec.FromGame game,
        GameEditor editor,
        string gameDir,
        string outDir) {
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      Writers.SpoilerLogs.WriteLine(string.Format("Seed: {0}. Options: {1}",
                                                  (object) opt.DisplaySeed,
                                                  (object) string.Join(
                                                      " ",
                                                      (IEnumerable<string>) opt
                                                          .GetEnabled())));

      var fogdistDirectory = new IoDirectory(editor.Spec.GameDir);
      var fogTxt = fogdistDirectory.GetFile("fog.txt");

      IDeserializer deserializer = new DeserializerBuilder().Build();

      AnnotationData ann;
      using (StreamReader streamReader = File.OpenText(fogTxt.FullName))
        ann = deserializer.Deserialize<AnnotationData>(
            (TextReader) streamReader);
      ann.SetGame(game);
      stopwatch.ResetAndPrint("Set up annotations");

      Events events = null;
      if (game == GameSpec.FromGame.DS3) {
        var locations = fogdistDirectory.GetFile("locations.txt");
        using var streamReader = File.OpenText(locations.FullName);
        ann.Locations = deserializer.Deserialize<AnnotationData.FogLocations>(
            streamReader);

        var ds3Common = fogdistDirectory.GetFile("Base\\ds3-common.emedf.json");
        events = new Events(ds3Common.FullName);
      }
      stopwatch.ResetAndPrint("Read fog gates & events");

      Graph g = new Graph();
      g.Construct(opt, ann);
      stopwatch.ResetAndPrint("Construct graph");

      ItemReader.Result items =
          await new ItemReader().FindItems(opt,
                                           ann,
                                           g,
                                           events,
                                           gameDir,
                                           editor);
      Writers.SpoilerLogs.WriteLine(items.Randomized
                                        ? "Key item hash: " + items.ItemHash
                                        : "No key items randomized");
      Writers.SpoilerLogs.WriteLine();
      stopwatch.ResetAndPrint("Find items");

      GraphConnector.Randomize(opt, g, ann);
      stopwatch.ResetAndPrint("Randomizer");

      if (opt["bonedryrun"])
        goto DoneRandomizing;
      Writers.SpoilerLogs.WriteLine();

      Console.WriteLine();
      Console.WriteLine("Saving game files...");
      if (game == GameSpec.FromGame.DS3) {
        EventConfig eventConfig;
        using (StreamReader streamReader = File.OpenText("fogdist\\events.txt"))
          eventConfig =
              deserializer.Deserialize<EventConfig>((TextReader) streamReader);
        if (opt["eventsyaml"] || opt["events"]) {
          await new GenerateConfig().WriteEventConfig(editor, ann, events, opt);
          goto DoneRandomizing;
        }
        await new GameDataWriter3().WriteAsync(opt,
                                               ann,
                                               g,
                                               gameDir,
                                               outDir,
                                               events,
                                               eventConfig,
                                               editor);
      } else {
        if (opt["dryrun"]) {
          Console.WriteLine("Success (dry run)");
          goto DoneRandomizing;
        }
        await new GameDataWriter().WriteAsync(opt, ann, g, gameDir, game);
      }
      goto DoneRandomizing;

      DoneRandomizing:
      stopwatch.ResetAndPrint("Saving");
      return items;
    }
  }
}