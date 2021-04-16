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
    private static DirectoryInfo TEMP_DIRECTORY_ =
        new DirectoryInfo(Directory.GetCurrentDirectory());

    [TestMethod]
    public void VerifyAgainstGoldens() {
      var testExeDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
      var testProjectDirectory = testExeDirectory.Parent.Parent;

      var testProjectGoldensDirectory = testProjectDirectory
                                        .GetDirectories()
                                        .First(d => d.Name == "goldens");

      foreach (var goldenDirectory in
          testProjectGoldensDirectory.GetDirectories()) {
        this.VerifyAgainstGolden_(goldenDirectory);
      }
    }

    private void VerifyAgainstGolden_(DirectoryInfo goldenDirectory) {
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

      var tempDir = TEMP_DIRECTORY_.CreateSubdirectory("temp");

      var gameDir = "M:\\Games\\Steam\\steamapps\\common\\DARK SOULS III\\Game";

      var spoilerLogs = tempDir.CreateSubdirectory("spoiler_logs");
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
      AssertSpoilerLogsEqual_(
          GetSubdir_(goldenDirectory, "spoiler_logs").GetFiles().First(),
          GetSubdir_(tempDir, "spoiler_logs").GetFiles().First());

      //tempDir.Delete();
    }

    private DirectoryInfo GetSubdir_(DirectoryInfo dir, string name)
      => dir.GetDirectories().First(d => d.Name == name);

    private FileInfo GetFile_(DirectoryInfo dir, string name)
      => dir.GetFiles().First(f => f.Name == name);

    /*private void AssertDirectoriesEqual() {

    }*/

    private void AssertFilesBytesInDirs_(
        DirectoryInfo expected,
        DirectoryInfo actual,
        string name)
      => AssertFilesBytes_(GetFile_(expected, name), GetFile_(actual, name));

    private void AssertFilesTextInDirs_(
        DirectoryInfo expected,
        DirectoryInfo actual,
        string name)
      => AssertFilesText_(GetFile_(expected, name), GetFile_(actual, name));


    private void AssertFilesBytes_(FileInfo expected, FileInfo actual)
      => Assert.AreEqual(File.ReadAllBytes(expected.FullName),
                         File.ReadAllBytes(actual.FullName),
                         $"Expected {actual} to have same contents as {expected}.");

    private void AssertFilesText_(FileInfo expected, FileInfo actual)
      => Assert.IsTrue(File.ReadAllText(expected.FullName) ==
                       File.ReadAllText(actual.FullName),
                       $"Expected {actual} to have same contents as {expected}.");

    private void AssertSpoilerLogsEqual_(FileInfo expected, FileInfo actual) {
      Func<FileInfo, string> readFromSpoilerLogs = f
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