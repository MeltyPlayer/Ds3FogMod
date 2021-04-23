using System;
using System.IO;
using System.Linq;
using System.Text;

using FogMod.io;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FogMod.Properties;

namespace FogMod {
  [TestClass]
  public class RandomizerTest {
    [TestMethod]
    public void VerifyAgainstGoldens() {
      var testExeDirectory = IoDirectory.GetCwd();
      var testProjectDirectory = testExeDirectory.GetSubdir("../..");

      var testProjectGoldensDirectory = testProjectDirectory
          .GetSubdir("goldens");

      var goldenDirectories = testProjectGoldensDirectory.GetSubdirs();
      foreach (var goldenDirectory in goldenDirectories) {
        this.VerifyAgainstGolden_(goldenDirectory);
      }
    }

    private void VerifyAgainstGolden_(IDirectory goldenDirectory) {
      var opt = new RandomizerOptions {
          Game = SoulsIds.GameSpec.FromGame.DS3
      };

      // Randomize boss fog gates
      opt["boss"] = true;
      // Randomize pvp fog gates
      opt["pvp"] = true;
      // Require cinders of a lord
      opt["lords"] = true;

      // Scale enemies & bosses
      opt["scale"] = true;
      // Allow escaping w/o fighting bosses
      opt["pacifist"] = true;
      // Tree skip
      opt["treeskip"] = true;

      opt["earlywarp"] = true;

      // Use fixed seed
      //opt["fixedseed"] = true;
      opt.Seed = int.Parse(goldenDirectory.Name);

      var tempDir = IoDirectory.GetCwd().GetSubdir("temp", true);

      var gameDir = "M:\\Games\\Steam\\steamapps\\common\\DARK SOULS III\\Game";

      var spoilerLogs = tempDir.GetSubdir("spoiler_logs", true);
      string path = string.Format($"{spoilerLogs.FullName}\\temp.txt");

      Writers.SpoilerLogs = File.CreateText(path);
      new Randomizer().Randomize(opt,
                                 SoulsIds.GameSpec.FromGame.DS3,
                                 opt["mergemods"]
                                     ? gameDir + "\\randomizer"
                                     : (string) null,
                                 tempDir.FullName);
      Writers.SpoilerLogs.Close();

      /*var directories = new[] {"event", "map", "msg", "script"};
      foreach (var directory in directories) {
        var goldenSubdir = GetSubdir_(goldenDirectory, directory);
        var tempSubdir = GetSubdir_(tempDir, directory);

        foreach (var file in goldenSubdir.GetFiles()) {
          AssertFilesTextInDirs_(goldenSubdir, tempSubdir, file.Name);
        }
      }*/

      //AssertFilesEqualInDirs_(goldenDirectory, tempDir, "Data0.bdt");

      // Verifies spoiler logs.
      this.AssertSpoilerLogsEqual_(
          goldenDirectory.GetSubdir("spoiler_logs").GetFiles().First(),
          tempDir.GetSubdir("spoiler_logs").GetFiles().First());

      //tempDir.Delete();
    }

    /*private void AssertDirectoriesEqual() {

    }*/

    private void AssertFilesBytesInDirs_(
        IDirectory expected,
        IDirectory actual,
        string name)
      => this.AssertFilesBytes_(expected.GetFile(name), actual.GetFile(name));

    private void AssertFilesTextInDirs_(
        IDirectory expected,
        IDirectory actual,
        string name)
      => this.AssertFilesText_(expected.GetFile(name), actual.GetFile(name));


    private void AssertFilesBytes_(IFile expected, IFile actual)
      => Assert.AreEqual(File.ReadAllBytes(expected.FullName),
                         File.ReadAllBytes(actual.FullName),
                         $"Expected {actual} to have same contents as {expected}.");

    private void AssertFilesText_(IFile expected, IFile actual)
      => Assert.IsTrue(File.ReadAllText(expected.FullName) ==
                       File.ReadAllText(actual.FullName),
                       $"Expected {actual} to have same contents as {expected}.");

    private void AssertSpoilerLogsEqual_(IFile expected, IFile actual) {
      Func<IFile, string> readFromSpoilerLogs = f
          => File.ReadAllLines(f.FullName)
                 .Where(l => !l.StartsWith("Writing "))
                 .Aggregate(new StringBuilder(),
                            (current, next)
                                => current
                                   .Append(current.Length == 0 ? "" : ", ")
                                   .Append(next))
                 .ToString();
      Assert.AreEqual(
          readFromSpoilerLogs(expected),
          readFromSpoilerLogs(actual));
    }
  }
}