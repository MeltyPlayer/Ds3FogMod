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

using SoulsIds;

using FogMod.util.time;

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

      var modDirectory = IoDirectory.ModDirectory;
      var fogdistDirectory = modDirectory.GetSubdir("fogdist");
      var layoutsDirectory = fogdistDirectory.GetSubdir("Layouts");
      var namesDirectory = fogdistDirectory.GetSubdir("Names");

      var editor = new GameEditor(GameSpec.FromGame.DS3);
      editor.Spec.GameDir = fogdistDirectory.FullName;
      editor.Spec.LayoutDir = layoutsDirectory.FullName;
      editor.Spec.NameDir = namesDirectory.FullName;

      await new Randomizer().Randomize(opt,
                                       SoulsIds.GameSpec.FromGame.DS3,
                                       editor,
                                       opt["mergemods"]
                                           ? gameDir + "\\randomizer"
                                           : (string) null,
                                       tempDir.FullName);
      Writers.SpoilerLogs.Close();

      var stopwatch = new Stopwatch();
      stopwatch.Start();

      var directories = new[] {"event", "map", "msg", "script"};
      foreach (var directory in directories) {
        var goldenSubdir = goldenDirectory.GetSubdir(directory);
        var tempSubdir = tempDir.GetSubdir(directory);

        this.AssertDirectoriesEqual_(goldenSubdir, tempSubdir);
      }
      stopwatch.ResetAndPrint("Verifying output files");

      this.AssertData0_(goldenDirectory, tempDir, editor);
      stopwatch.ResetAndPrint("Verifying Data0.bdt");

      // Verifies spoiler logs.
      this.AssertSpoilerLogsEqual_(
          goldenDirectory.GetSubdir("spoiler_logs").GetFiles().First(),
          tempDir.GetSubdir("spoiler_logs").GetFiles().First());
      stopwatch.ResetAndPrint("Verifying spoiler logs");
    }

    private void AssertData0_(
        IDirectory goldenDirectory,
        IDirectory tempDir,
        GameEditor editor) {
      var expectedFile = goldenDirectory.GetFile("Data0.bdt");
      var actualFile = tempDir.GetFile("Data0.bdt");

      var expectedData0 = SoulsFile<BND4>.Read(
          this.ReadFileBytes_(expectedFile, true));
      var actualData0 = SoulsFile<BND4>.Read(
          this.ReadFileBytes_(actualFile, true));

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

      var expectedParams = editor.LoadParams(expectedFile.FullName, null, true);
      var actualParams = editor.LoadParams(actualFile.FullName, null, true);

      Assert.AreEqual(expectedParams.Count, actualParams.Count);
      foreach (var expectedParam in expectedParams) {
        var success =
            actualParams.TryGetValue(expectedParam.Key, out var actualParam);

        Assert.IsTrue(success, $"Expected to find {expectedParam} in actual.");
        this.AssertParamsEqual_(expectedParam.Value,
                                actualParam,
                                expectedParam.Key);
      }
    }

    private void AssertParamsEqual_(PARAM expected, PARAM actual, string key) {
      var differenceText = $"Difference for {key}";

      var expectedParamdef = expected.AppliedParamdef;
      var actualParamdef = actual.AppliedParamdef;
      Assert.AreEqual(expectedParamdef.BigEndian,
                      actualParamdef.BigEndian,
                      differenceText);
      Assert.AreEqual(expectedParamdef.ParamType,
                      actualParamdef.ParamType,
                      differenceText);
      Assert.AreEqual(expectedParamdef.Unicode,
                      actualParamdef.Unicode,
                      differenceText);
      Assert.AreEqual(expectedParamdef.Unk06,
                      actualParamdef.Unk06,
                      differenceText);
      Assert.AreEqual(expectedParamdef.Version,
                      actualParamdef.Version,
                      differenceText);

      var expectedFields = expectedParamdef.Fields;
      var actualFields = actualParamdef.Fields;
      Assert.AreEqual(expectedFields.Count, actualFields.Count, differenceText);
      for (var i = 0; i < expectedFields.Count; ++i) {
        var expectedField = expectedFields[i];
        var actualField = actualFields[i];

        Assert.AreEqual(expectedField.DisplayType,
                        actualField.DisplayType,
                        differenceText);
        Assert.AreEqual(expectedField.Default,
                        actualField.Default,
                        differenceText);
        Assert.AreEqual(expectedField.BitSize,
                        actualField.BitSize,
                        differenceText);
        Assert.AreEqual(expectedField.ArrayLength,
                        actualField.ArrayLength,
                        differenceText);
        Assert.AreEqual(expectedField.Description,
                        actualField.Description,
                        differenceText);
        Assert.AreEqual(expectedField.DisplayFormat,
                        actualField.DisplayFormat,
                        differenceText);
        Assert.AreEqual(expectedField.DisplayName,
                        actualField.DisplayName,
                        differenceText);
        Assert.AreEqual(expectedField.Increment,
                        actualField.Increment,
                        differenceText);
        Assert.AreEqual(expectedField.InternalName,
                        actualField.InternalName,
                        differenceText);
        Assert.AreEqual(expectedField.InternalType,
                        actualField.InternalType,
                        differenceText);
        Assert.AreEqual(expectedField.Maximum,
                        actualField.Maximum,
                        differenceText);
        Assert.AreEqual(expectedField.Minimum,
                        actualField.Minimum,
                        differenceText);
        Assert.AreEqual(expectedField.SortID,
                        actualField.SortID,
                        differenceText);
        Assert.AreEqual(expectedField.EditFlags,
                        actualField.EditFlags,
                        differenceText);
      }

      Assert.AreEqual(expected.BigEndian, actual.BigEndian, differenceText);
      Assert.AreEqual(expected.DetectedSize,
                      actual.DetectedSize,
                      differenceText);
      Assert.AreEqual(expected.Format2D, actual.Format2D, differenceText);
      Assert.AreEqual(expected.Format2E, actual.Format2E, differenceText);
      Assert.AreEqual(expected.Format2F, actual.Format2F, differenceText);
      Assert.AreEqual(expected.ParamType, actual.ParamType, differenceText);
      Assert.AreEqual(expected.Unk06, actual.Unk06, differenceText);
      Assert.AreEqual(expected.Unk08, actual.Unk08, differenceText);

      var expectedRows = expected.Rows;
      var actualRows = actual.Rows;
      Assert.AreEqual(expectedRows.Count, actualRows.Count, differenceText);
      for (var r = 0; r < expectedRows.Count; ++r) {
        var expectedRow = expectedRows[r];
        var actualRow = actualRows[r];

        Assert.AreEqual(expectedRow.Name, actualRow.Name, differenceText);
        Assert.AreEqual(expectedRow.ID, actualRow.ID, differenceText);

        var expectedCells = expectedRow.Cells;
        var actualCells = actualRow.Cells;
        Assert.AreEqual(expectedCells.Count, actualCells.Count, differenceText);
        for (var c = 0; c < expectedCells.Count; ++c) {
          var expectedCell = expectedCells[c];
          var actualCell = actualCells[c];

          var expectedDef = expectedCell.Def;
          var actualDef = actualCell.Def;
          // TODO: Check all these fields?

          var expectedValue = expectedCell.Value;
          var actualValue = actualCell.Value;

          var displayType = expectedDef.DisplayType;
          if (displayType == PARAMDEF.DefType.dummy8) {
            AssertBytes_(expectedValue as byte[],
                         actualValue as byte[],
                         expectedDef.DisplayName);
          } else if (displayType == PARAMDEF.DefType.f32) {
            var expectedFloat = (float) expectedValue;
            var actualFloat = (float) actualValue;
            Assert.AreEqual(expectedFloat,
                            actualFloat,
                            .0001,
                            differenceText);
          } else {
            Assert.AreEqual(expectedValue, actualValue, differenceText);
          }
        }
      }
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
        rawBytes = file.SkimAllBytes();
      }

      if (decrypt) {
        return SFUtil.DecryptByteArray(SFUtil.ds3RegulationKey, rawBytes);
      }

      return rawBytes;
    }

    private void SaveFile_(byte[] bytes, string output) {
      using var outputStream = new FileStream(output, FileMode.Create);
      outputStream.Write(bytes, 0, bytes.Length);
    }

    private void AssertSpoilerLogsEqual_(IFile expected, IFile actual) {
      Func<IFile, string> readFromSpoilerLogs = f
          => File.ReadAllLines(f.FullName)
                 .Where(l => !l.StartsWith("Writing ") &&
                             !l.StartsWith("Found extra file ") &&
                             !l.StartsWith("No extra files found"))
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