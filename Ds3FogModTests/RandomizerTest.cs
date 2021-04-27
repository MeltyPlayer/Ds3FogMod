using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public async Task VerifyAgainstGoldens() {
      var testExeDirectory = IoDirectory.GetCwd();
      var testProjectDirectory = testExeDirectory.GetSubdir("../..");

      var testProjectGoldensDirectory = testProjectDirectory
          .GetSubdir("goldens");

      var goldenDirectories = testProjectGoldensDirectory.GetSubdirs();
      foreach (var goldenDirectory in goldenDirectories) {
        await this.VerifyAgainstGolden_(goldenDirectory);
      }
    }

    private async Task VerifyAgainstGolden_(IDirectory goldenDirectory) {
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
      if (tempDir.Name != "temp") {
        Assert.Fail("Got the wrong directory!!!");
        return;
      }

      var tempDirInfo = new DirectoryInfo(tempDir.FullName);
      tempDirInfo.Delete(true);
      tempDirInfo.Create();

      var gameDir = "M:\\Games\\Steam\\steamapps\\common\\DARK SOULS III\\Game";

      var spoilerLogs = tempDir.GetSubdir("spoiler_logs", true);
      string path = string.Format($"{spoilerLogs.FullName}\\temp.txt");

      Writers.SpoilerLogs = File.CreateText(path);
      await new Randomizer().Randomize(opt,
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

      this.AssertData0_(goldenDirectory, tempDir);

      // Verifies spoiler logs.
      this.AssertSpoilerLogsEqual_(
          goldenDirectory.GetSubdir("spoiler_logs").GetFiles().First(),
          tempDir.GetSubdir("spoiler_logs").GetFiles().First());
    }

    private void AssertData0_(IDirectory goldenDirectory, IDirectory tempDir) {
      var expectedData0 = SoulsFile<BND4>.Read(
          this.ReadFileBytes_(goldenDirectory.GetFile("Data0.bdt"), true));
      var actualData0 = SoulsFile<BND4>.Read(
          this.ReadFileBytes_(tempDir.GetFile("Data0.bdt"), true));

      Assert.AreEqual(expectedData0.BigEndian, actualData0.BigEndian);
      Assert.AreEqual(expectedData0.BitBigEndian, actualData0.BitBigEndian);
      Assert.AreEqual(expectedData0.Extended, actualData0.Extended);
      Assert.AreEqual(expectedData0.Format, actualData0.Format);
      Assert.AreEqual(expectedData0.Unicode, actualData0.Unicode);
      Assert.AreEqual(expectedData0.Unk04, actualData0.Unk04);
      Assert.AreEqual(expectedData0.Unk05, actualData0.Unk05);
      Assert.AreEqual(expectedData0.Version, actualData0.Version);

      var expectedFiles = expectedData0.Files;
      var actualFiles = actualData0.Files;

      var expectedCount = expectedFiles.Count;
      Assert.AreEqual(expectedCount, actualFiles.Count);
      for (var i = 0; i < expectedCount; ++i) {
        var expectedFile = expectedFiles[i];
        var actualFile = actualFiles[i];

        var expectedName = expectedFile.Name;
        this.AssertBytes_(expectedFile.Bytes, actualFile.Bytes, expectedName);
      }

      // this.AssertFilesBytesInDirs_(goldenDirectory, tempDir, "Data0.bdt", true);
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
        var fileName = expectedFile.Name;
        if (fileName.EndsWith(".fst")) {
          continue;
        }

        this.AssertFilesBytes_(expectedFile, actualDirectory.GetFile(fileName));
      }
    }

    private void AssertFilesBytesInDirs_(
        IDirectory expected,
        IDirectory actual,
        string name,
        bool decrypt = false)
      => this.AssertFilesBytes_(expected.GetFile(name),
                                actual.GetFile(name),
                                decrypt);

    private void AssertFilesTextInDirs_(
        IDirectory expected,
        IDirectory actual,
        string name)
      => this.AssertFilesText_(expected.GetFile(name), actual.GetFile(name));


    private void AssertFilesBytes_(
        IFile expected,
        IFile actual,
        bool decrypt = false) {
      var expectedBytes = this.ReadFileBytes_(expected, decrypt);
      var actualBytes = this.ReadFileBytes_(actual, decrypt);
      this.AssertBytes_(expectedBytes, actualBytes, expected.FullName);
    }

    private void AssertBytes_(byte[] expected, byte[] actual, string name) {
      var expectedLength = expected.Length;
      if (expectedLength != actual.Length) {
        this.SaveExpectedAndActual_(expected, actual);
        Assert.AreEqual(expectedLength,
                        actual.Length,
                        "Expected files to be the same length.");
        return;
      }

      var differences = 0;
      for (var i = 0; i < expectedLength; ++i) {
        if (expected[i] != actual[i]) {
          ++differences;
        }
      }

      if (differences != 0) {
        this.SaveExpectedAndActual_(expected, actual);
        var changePlural = differences == 1 ? "change" : "changes";
        Assert.Fail(
            $"Expected {name} to have same contents as {name}, but " +
            $"found {differences} {changePlural} out of {expectedLength} " +
            "bytes");
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

    private void DecompressFiles_(IFile expected, IFile actual)
      => this.SaveExpectedAndActual_(this.ReadFileBytes_(expected),
                                     this.ReadFileBytes_(actual));

    private void SaveExpectedAndActual_(byte[] expected, byte[] actual) {
      this.SaveFile_(expected,
                     RandomizerTest.TEMP_DIR.FullName + "\\expected.txt");
      this.SaveFile_(actual, RandomizerTest.TEMP_DIR.FullName + "\\actual.txt");
    }

    private byte[] ReadFileBytes_(IFile file, bool decrypt = false) {
      byte[] rawBytes;
      try {
        rawBytes = DCX.Decompress(file.FullName);
      } catch {
        rawBytes = File.ReadAllBytes(file.FullName);
      }

      if (decrypt) {
        return SFUtil.DecryptByteArray(SFUtil.ds3RegulationKey, rawBytes);
      }

      return rawBytes;
    }

    private void SaveFile_(byte[] bytes, string output) {
      /*using var inputStream = new DeflateStream(
          new FileStream(input.FullName, FileMode.Open),
          CompressionMode.Decompress);
      using var outputStream = new DeflateStream(
          new FileStream(output, FileMode.Create),
          CompressionMode.Compress);

      inputStream.CopyTo(outputStream);*/

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