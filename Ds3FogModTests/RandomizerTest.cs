using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using FogMod.io;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FogMod.Properties;

using SoulsFormats;

namespace FogMod {
  [TestClass]
  public class RandomizerTest {
    private static readonly IDirectory TEMP_DIR =
        IoDirectory.GetCwd().GetSubdir("temp", true);

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

      var tempDir = RandomizerTest.TEMP_DIR;

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

      var directories = new[] {"event", "map", "msg", "script"};
      foreach (var directory in directories) {
        var goldenSubdir = goldenDirectory.GetSubdir(directory);
        var tempSubdir = tempDir.GetSubdir(directory);

        this.AssertDirectoriesEqual_(goldenSubdir, tempSubdir);
      }

      // this.AssertFilesBytesInDirs_(goldenDirectory, tempDir, "Data0.bdt");

      // Verifies spoiler logs.
      this.AssertSpoilerLogsEqual_(
          goldenDirectory.GetSubdir("spoiler_logs").GetFiles().First(),
          tempDir.GetSubdir("spoiler_logs").GetFiles().First());
    }

    private void AssertDirectoriesEqual_(
        IDirectory expectedDirectory,
        IDirectory actualDirectory) {
      foreach (var expectedSubdir in expectedDirectory.GetSubdirs()) {
        this.AssertDirectoriesEqual_(expectedSubdir,
                                     actualDirectory.GetSubdir(
                                         expectedSubdir.Name));
      }

      foreach (var expectedFile in expectedDirectory.GetFiles()) {
        this.AssertFilesBytes_(expectedFile,
                               actualDirectory.GetFile(expectedFile.Name));
      }
    }

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


    private void AssertFilesBytes_(IFile expected, IFile actual) {
      var expectedBytes = this.ReadFileBytes_(expected);
      var actualBytes = this.ReadFileBytes_(actual);

      var expectedLength = expectedBytes.Length;
      if (expectedLength != actualBytes.Length) {
        this.DecompressFiles_(expected, actual);
        Assert.AreEqual(expectedLength,
                        actualBytes.Length,
                        "Expected files to be the same length.");
        return;
      }

      var differences = 0;
      for (var i = 0; i < expectedLength; ++i) {
        if (expectedBytes[i] != actualBytes[i]) {
          ++differences;
        }
      }

      if (differences != 0) {
        this.DecompressFiles_(expected, actual);

        var changePlural = differences == 1 ? "change" : "changes";
        Assert.Fail(
            $"Expected {actual.FullName} to have same contents as " +
            $"{expected.FullName}, but found {differences} {changePlural} " +
            $"out of {expectedLength} bytes");
      }
    }

    private void AssertFilesText_(IFile expected, IFile actual) {
      var areDifferent = File.ReadAllText(expected.FullName) !=
                         File.ReadAllText(actual.FullName);

      if (areDifferent) {
        this.DecompressFiles_(expected, actual);
        Assert.Fail(
            $"Expected {actual.FullName} to have same contents as " +
            $"{expected.FullName}.");
      }
    }

    private void DecompressFiles_(IFile expected, IFile actual) {
      this.DecompressFile_(expected,
                           RandomizerTest.TEMP_DIR.FullName + "\\expected.txt");
      this.DecompressFile_(actual,
                           RandomizerTest.TEMP_DIR.FullName + "\\actual.txt");
    }

    private byte[] ReadFileBytes_(IFile file) {
      try {
        return DCX.Decompress(file.FullName);
      } catch {
        return File.ReadAllBytes(file.FullName);
      }
    }

    private void DecompressFile_(IFile input, string output) {
      /*using var inputStream = new DeflateStream(
          new FileStream(input.FullName, FileMode.Open),
          CompressionMode.Decompress);
      using var outputStream = new DeflateStream(
          new FileStream(output, FileMode.Create),
          CompressionMode.Compress);

      inputStream.CopyTo(outputStream);*/

      var bytes = this.ReadFileBytes_(input);
      using var outputStream = new FileStream(output, FileMode.Create);
      outputStream.Write(bytes, 0, bytes.Length);
    }

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