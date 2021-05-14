// Decompiled with JetBrains decompiler
// Type: FogMod.MainForm
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using FogMod.Properties;

using SoulsIds;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FogMod {
  public class MainForm : Form {
    private static string defaultPath =
        "C:\\Program Files (x86)\\Steam\\steamapps\\common\\DARK SOULS REMASTERED\\DarkSoulsRemastered.exe";

    private static string defaultPath2 =
        "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Dark Souls Prepare to Die Edition\\DATA\\DARKSOULS.exe";

    private static List<string> defaultLang = new List<string>() {
        "ENGLISH"
    };

    private static List<string> ptdeLang = new List<string>() {
        "ENGLISH",
        "FRENCH",
        "GERMAN",
        "ITALIAN",
        "JAPANESE",
        "KOREAN",
        "POLISH",
        "RUSSIAN",
        "SPANISH",
        "TCHINESE"
    };

    private static List<string> ds1rLang = new List<string>() {
        "ENGLISH",
        "FRENCH",
        "GERMAN",
        "ITALIAN",
        "JAPANESE",
        "KOREAN",
        "NSPANISH",
        "POLISH",
        "PORTUGUESE",
        "RUSSIAN",
        "SCHINESE",
        "SPANISH",
        "TCHINESE"
    };

    private RandomizerOptions options = new RandomizerOptions();
    private GameSpec.FromGame game;
    private string languageToSet;
    private bool working;
    private IContainer components;
    private GroupBox groupBox1;
    private GroupBox groupBox2;
    private CheckBox world;
    private Label bossL;
    private CheckBox boss;
    private Label worldL;
    private Label label3;
    private CheckBox minor;
    private Label label4;
    private CheckBox major;
    private CheckBox lordvessel;
    private Label label2;
    private CheckBox warp;
    private Label label5;
    private Label label1;
    private CheckBox scale;
    private CheckBox hard;
    private Label label8;
    private Label label6;
    private CheckBox lords;
    private CheckBox pacifist;
    private Label label7;
    private Label label9;
    private CheckBox bboc;
    private TextBox fixedseed;
    private Label label10;
    private Button randb;
    private Button button2;
    private TextBox exe;
    private Button restoreButton;
    private Label restoreL;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel statusL;
    private TextBox randomizeL;
    private Label label11;
    private CheckBox unconnected;
    private Label label12;
    private CheckBox start;
    private ComboBox language;
    private Label label13;
    private Label label14;

    public MainForm() {
      if (!string.IsNullOrWhiteSpace(Settings.Default.Language))
        this.languageToSet = Settings.Default.Language;
      this.InitializeComponent();
      string exe = Settings.Default.Exe;
      if (!string.IsNullOrWhiteSpace(exe))
        this.exe.Text = exe;
      else if (File.Exists(MainForm.defaultPath))
        this.exe.Text = MainForm.defaultPath;
      else if (File.Exists(MainForm.defaultPath2))
        this.exe.Text = MainForm.defaultPath2;
      this.options["dryrun"] = false;
      string options = Settings.Default.Options;
      if (string.IsNullOrWhiteSpace(options))
        this.ReadControlFlags((Control) this);
      else
        this.SetControlFlags((Control) this,
                             (ICollection<string>) options.Split(' '));
    }

    private void UpdateExePath() {
      bool flag = true;
      string path1 = (string) null;
      try {
        path1 = Path.GetDirectoryName(this.exe.Text);
        if (this.exe.Text.Trim() == "" || !Directory.Exists(path1))
          flag = false;
        string lower = Path.GetFileName(this.exe.Text).ToLower();
        if (lower == "darksoulsremastered.exe")
          this.game = GameSpec.FromGame.DS1R;
        else if (lower == "darksouls.exe")
          this.game = GameSpec.FromGame.DS1;
        else
          flag = false;
      } catch (ArgumentException ex) {
        flag = false;
      }
      if (!flag) {
        this.game = GameSpec.FromGame.UNKNOWN;
        this.restoreButton.Enabled = false;
        this.restoreL.Text = "";
        this.language.DataSource = (object) MainForm.defaultLang;
        this.language.Enabled = false;
      } else {
        Settings.Default.Exe = this.exe.Text;
        Settings.Default.Save();
        List<string> stringList = this.game == GameSpec.FromGame.DS1R
                                      ? MainForm.ds1rLang
                                      : MainForm.ptdeLang;
        this.language.DataSource = (object) stringList;
        this.language.Enabled = true;
        if (this.languageToSet != null &&
            stringList.Contains(this.languageToSet)) {
          this.language.SelectedIndex = stringList.IndexOf(this.languageToSet);
          this.languageToSet = (string) null;
        }
        List<string> allBaseFiles = GameDataWriter.GetAllBaseFiles(this.game);
        if (allBaseFiles.Count == 0) {
          this.randb.Enabled = false;
          this.setStatus(string.Format(
                             "Error: FogMod dist\\{0} subdirectory is missing",
                             (object) this.game),
                         true);
        }
        List<string> source = new List<string>();
        foreach (string str in allBaseFiles) {
          string path2 = path1 + "\\" + str + ".bak";
          if (File.Exists(path2))
            source.Add(File.GetLastWriteTime(path2)
                           .ToString("yyyy-MM-dd HH:mm:ss"));
        }
        if (source.Count == 0) {
          this.restoreL.Text = "Backups will be created with randomization";
          this.restoreButton.Enabled = false;
        } else {
          this.restoreL.Text =
              (allBaseFiles.Count == source.Count
                   ? "Backups"
                   : "Partial backups") +
              " from " +
              source.Max<string>();
          this.restoreButton.Enabled = true;
        }
      }
    }

    private void OpenExe(object sender, EventArgs e) {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Select Dark Souls install location";
      openFileDialog.Filter =
          "Dark Souls exe|DarkSoulsRemastered.exe;DARKSOULS.exe";
      openFileDialog.RestoreDirectory = true;
      try {
        if (Directory.Exists(this.exe.Text)) {
          openFileDialog.InitialDirectory = this.exe.Text;
        } else {
          string directoryName = Path.GetDirectoryName(this.exe.Text);
          if (Directory.Exists(directoryName))
            openFileDialog.InitialDirectory = directoryName;
        }
      } catch (ArgumentException ex) {}
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      this.exe.Text = openFileDialog.FileName;
    }

    private void setStatus(string msg, bool error = false) {
      this.statusL.Text = msg;
      this.statusStrip1.BackColor =
          error ? Color.IndianRed : SystemColors.Control;
    }

    private async void Randomize(object sender, EventArgs e) {
      MainForm mainForm = this;
      if (mainForm.working)
        return;
      mainForm.ReadControlFlags((Control) mainForm);
      RandomizerOptions rand = mainForm.options.Copy();
      rand.Language = (string) mainForm.language.SelectedValue ?? "ENGLISH";
      if (!File.Exists(mainForm.exe.Text) ||
          mainForm.game == GameSpec.FromGame.UNKNOWN) {
        mainForm.setStatus("Game exe not set", true);
      } else {
        string gameDir = Path.GetDirectoryName(mainForm.exe.Text);
        if (!File.Exists(gameDir + "\\map\\MapStudio\\m10_02_00_00.msb"))
          mainForm.setStatus("Did not find unpacked installation at game path",
                             true);
        else if (rand["start"] && !rand["boss"] && !rand["world"]) {
          mainForm.setStatus(
              "Cannot start outside of Asylum if no Asylum fog gates are randomized",
              true);
        } else {
          if (mainForm.fixedseed.Text.Trim() != "") {
            uint result;
            if (uint.TryParse(mainForm.fixedseed.Text.Trim(), out result)) {
              rand.Seed = (int) result;
            } else {
              mainForm.setStatus("Invalid fixed seed", true);
              return;
            }
          } else
            rand.Seed = new Random().Next();
          mainForm.working = true;
          mainForm.randomizeL.Text =
              string.Format("Seed: {0}", (object) rand.Seed);
          mainForm.randb.Text = "Randomizing...";
          mainForm.setStatus("Randomizing...", false);
          mainForm.randb.BackColor = Color.LightYellow;
          Randomizer randomizer = new Randomizer();

          var editor = new GameEditor(this.game);
          editor.Spec.GameDir = @"dist";
          editor.Spec.LayoutDir = @"dist\Layouts";
          editor.Spec.NameDir = @"dist\Names";

          await Task.Factory.StartNew((Action) (async () => {
                                                   Directory.CreateDirectory(
                                                       "runs");
                                                   string path = string.Format(
                                                       "runs\\{0}_log_{1}_{2}.txt",
                                                       (object) DateTime
                                                                .Now.ToString(
                                                                    "yyyy-MM-dd_HH.mm.ss"),
                                                       (object) rand.Seed,
                                                       (object) rand
                                                           .ConfigHash());
                                                   TextWriter text =
                                                       (TextWriter) File
                                                           .CreateText(path);
                                                   TextWriter newOut =
                                                       Console.Out;
                                                   Console.SetOut(text);
                                                   try {
                                                     ItemReader.Result result =
                                                         await randomizer.Randomize(
                                                             rand,
                                                             this.game,
                                                             editor,
                                                             gameDir,
                                                             gameDir);
                                                     this.setStatus(
                                                         "Done. Info in " +
                                                         path +
                                                         (result.Randomized
                                                              ? " | Key item hash: " +
                                                                result.ItemHash
                                                              : ""),
                                                         false);
                                                   } catch (Exception ex) {
                                                     Console.WriteLine(
                                                         (object) ex);
                                                     this.setStatus(
                                                         "Error! See error message in " +
                                                         path,
                                                         true);
                                                   } finally {
                                                     text.Close();
                                                     Console.SetOut(newOut);
                                                   }
                                                 }));
          mainForm.randb.Enabled = true;
          mainForm.randb.Text = "Randomize!";
          mainForm.randb.BackColor = SystemColors.Control;
          mainForm.working = false;
          mainForm.UpdateExePath();
        }
      }
    }

    private void UpdateFile(object sender, EventArgs e) {
      this.UpdateExePath();
    }

    private void UpdateOptions(object sender, EventArgs e) {
      this.ReadControlFlags((Control) this);
      Settings.Default.Options =
          string.Join(" ", (IEnumerable<string>) this.options.GetEnabled());
      Settings.Default.Save();
    }

    private void UpdateLanguage(object sender, EventArgs e) {
      Settings.Default.Language = (string) this.language.SelectedValue;
      Settings.Default.Save();
    }

    private void Restore(object sender, EventArgs e) {
      if (this.working)
        return;
      string directoryName = Path.GetDirectoryName(this.exe.Text);
      if (this.exe.Text.Trim() == "" ||
          !Directory.Exists(directoryName) ||
          this.game == GameSpec.FromGame.UNKNOWN)
        return;
      List<string> allBaseFiles = GameDataWriter.GetAllBaseFiles(this.game);
      List<string> stringList = new List<string>();
      foreach (string str1 in allBaseFiles) {
        string str2 = directoryName + "\\" + str1;
        if (File.Exists(str2 + ".bak"))
          stringList.Add(str2);
      }
      string str3 = this.game == GameSpec.FromGame.DS1R
                        ? "\n\nTo completely ensure restoration of vanilla files, also use Properties -> Local Files -> Verify Integrity Of Game Files in Steam."
                        : "";
      if (MessageBox.Show(
              string.Join("\n", (IEnumerable<string>) stringList) + str3,
              "Restore these files?",
              MessageBoxButtons.YesNo) !=
          DialogResult.Yes)
        return;
      foreach (string str1 in allBaseFiles) {
        string str2 = directoryName + "\\" + str1;
        string str4 = str2 + ".bak";
        if (File.Exists(str4)) {
          if (File.Exists(str2))
            File.Delete(str2);
          File.Move(str4, str2);
        }
      }
      this.UpdateExePath();
    }

    private void ReadControlFlags(Control control) {
      switch (control) {
        case RadioButton radioButton:
          this.options[control.Name] = radioButton.Checked;
          break;
        case CheckBox checkBox:
          this.options[control.Name] = checkBox.Checked;
          break;
        default:
          IEnumerator enumerator = control.Controls.GetEnumerator();
          try {
            while (enumerator.MoveNext())
              this.ReadControlFlags((Control) enumerator.Current);
            break;
          } finally {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
      }
    }

    private void SetControlFlags(Control control, ICollection<string> set) {
      switch (control) {
        case RadioButton radioButton:
          this.options[control.Name] =
              radioButton.Checked = set.Contains(control.Name);
          break;
        case CheckBox checkBox:
          this.options[control.Name] =
              checkBox.Checked = set.Contains(control.Name);
          break;
        default:
          IEnumerator enumerator = control.Controls.GetEnumerator();
          try {
            while (enumerator.MoveNext())
              this.SetControlFlags((Control) enumerator.Current, set);
            break;
          } finally {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
      }
    }

    protected override void Dispose(bool disposing) {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent() {
      ComponentResourceManager componentResourceManager =
          new ComponentResourceManager(typeof(MainForm));
      this.groupBox1 = new GroupBox();
      this.label3 = new Label();
      this.minor = new CheckBox();
      this.label4 = new Label();
      this.major = new CheckBox();
      this.label5 = new Label();
      this.lordvessel = new CheckBox();
      this.label2 = new Label();
      this.warp = new CheckBox();
      this.bossL = new Label();
      this.boss = new CheckBox();
      this.worldL = new Label();
      this.world = new CheckBox();
      this.groupBox2 = new GroupBox();
      this.label12 = new Label();
      this.start = new CheckBox();
      this.label11 = new Label();
      this.unconnected = new CheckBox();
      this.label9 = new Label();
      this.bboc = new CheckBox();
      this.label1 = new Label();
      this.scale = new CheckBox();
      this.hard = new CheckBox();
      this.label8 = new Label();
      this.label6 = new Label();
      this.lords = new CheckBox();
      this.pacifist = new CheckBox();
      this.label7 = new Label();
      this.fixedseed = new TextBox();
      this.label10 = new Label();
      this.randb = new Button();
      this.button2 = new Button();
      this.exe = new TextBox();
      this.restoreButton = new Button();
      this.restoreL = new Label();
      this.statusStrip1 = new StatusStrip();
      this.statusL = new ToolStripStatusLabel();
      this.randomizeL = new TextBox();
      this.language = new ComboBox();
      this.label13 = new Label();
      this.label14 = new Label();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      this.groupBox1.Controls.Add((Control) this.label3);
      this.groupBox1.Controls.Add((Control) this.minor);
      this.groupBox1.Controls.Add((Control) this.label4);
      this.groupBox1.Controls.Add((Control) this.major);
      this.groupBox1.Controls.Add((Control) this.label5);
      this.groupBox1.Controls.Add((Control) this.lordvessel);
      this.groupBox1.Controls.Add((Control) this.label2);
      this.groupBox1.Controls.Add((Control) this.warp);
      this.groupBox1.Controls.Add((Control) this.bossL);
      this.groupBox1.Controls.Add((Control) this.boss);
      this.groupBox1.Controls.Add((Control) this.worldL);
      this.groupBox1.Controls.Add((Control) this.world);
      this.groupBox1.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.groupBox1.Location = new Point(16, 14);
      this.groupBox1.Margin = new Padding(4);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new Padding(4);
      this.groupBox1.Size = new Size(451, (int) byte.MaxValue);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Randomized entrances";
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label3.Location = new Point(24, 194);
      this.label3.Name = "label3";
      this.label3.Size = new Size(367, 13);
      this.label3.TabIndex = 9;
      this.label3.Text =
          "Enable and randomize invasion fog gates usually separating off smaller areas";
      this.minor.AutoSize = true;
      this.minor.Location = new Point(7, 173);
      this.minor.Margin = new Padding(3, 2, 3, 2);
      this.minor.Name = "minor";
      this.minor.Size = new Size(147, 20);
      this.minor.TabIndex = 8;
      this.minor.Text = "Minor PvP fog gates";
      this.minor.UseVisualStyleBackColor = true;
      this.minor.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label4.Location = new Point(24, 157);
      this.label4.Name = "label4";
      this.label4.Size = new Size(310, 13);
      this.label4.TabIndex = 7;
      this.label4.Text =
          "Enable and randomize invasion fog gates separating major areas";
      this.major.AutoSize = true;
      this.major.Location = new Point(7, 136);
      this.major.Margin = new Padding(3, 2, 3, 2);
      this.major.Name = "major";
      this.major.Size = new Size(148, 20);
      this.major.TabIndex = 6;
      this.major.Text = "Major PvP fog gates";
      this.major.UseVisualStyleBackColor = true;
      this.major.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label5.Location = new Point(25, 232);
      this.label5.Name = "label5";
      this.label5.Size = new Size(328, 13);
      this.label5.TabIndex = 11;
      this.label5.Text =
          "Randomize golden fog gates, in which case they are never dispelled";
      this.lordvessel.AutoSize = true;
      this.lordvessel.Location = new Point(8, 209);
      this.lordvessel.Margin = new Padding(3, 2, 3, 2);
      this.lordvessel.Name = "lordvessel";
      this.lordvessel.Size = new Size(131, 20);
      this.lordvessel.TabIndex = 10;
      this.lordvessel.Text = "Lordvessel gates";
      this.lordvessel.UseVisualStyleBackColor = true;
      this.lordvessel.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label2.Location = new Point(24, 121);
      this.label2.Name = "label2";
      this.label2.Size = new Size(274, 13);
      this.label2.TabIndex = 5;
      this.label2.Text =
          "Randomize warp destinations, like to/from Painted World";
      this.warp.AutoSize = true;
      this.warp.Checked = true;
      this.warp.CheckState = CheckState.Checked;
      this.warp.Location = new Point(7, 98);
      this.warp.Margin = new Padding(3, 2, 3, 2);
      this.warp.Name = "warp";
      this.warp.Size = new Size(159, 20);
      this.warp.TabIndex = 4;
      this.warp.Text = "Warps between areas";
      this.warp.UseVisualStyleBackColor = true;
      this.warp.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.bossL.AutoSize = true;
      this.bossL.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.bossL.Location = new Point(24, 82);
      this.bossL.Name = "bossL";
      this.bossL.Size = new Size(199, 13);
      this.bossL.TabIndex = 3;
      this.bossL.Text = "Randomize fog gates to and from bosses";
      this.boss.AutoSize = true;
      this.boss.Checked = true;
      this.boss.CheckState = CheckState.Checked;
      this.boss.Location = new Point(7, 62);
      this.boss.Margin = new Padding(3, 2, 3, 2);
      this.boss.Name = "boss";
      this.boss.Size = new Size(117, 20);
      this.boss.TabIndex = 2;
      this.boss.Text = "Boss fog gates";
      this.boss.UseVisualStyleBackColor = true;
      this.boss.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.worldL.AutoSize = true;
      this.worldL.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.worldL.Location = new Point(25, 44);
      this.worldL.Name = "worldL";
      this.worldL.Size = new Size(149, 13);
      this.worldL.TabIndex = 1;
      this.worldL.Text = "Randomize two-way fog gates";
      this.world.AutoSize = true;
      this.world.Checked = true;
      this.world.CheckState = CheckState.Checked;
      this.world.Location = new Point(8, 23);
      this.world.Margin = new Padding(3, 2, 3, 2);
      this.world.Name = "world";
      this.world.Size = new Size(227, 20);
      this.world.TabIndex = 0;
      this.world.Text = "Traversable fog gates (non-boss)";
      this.world.UseVisualStyleBackColor = true;
      this.world.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.groupBox2.Controls.Add((Control) this.label12);
      this.groupBox2.Controls.Add((Control) this.start);
      this.groupBox2.Controls.Add((Control) this.label11);
      this.groupBox2.Controls.Add((Control) this.unconnected);
      this.groupBox2.Controls.Add((Control) this.label9);
      this.groupBox2.Controls.Add((Control) this.bboc);
      this.groupBox2.Controls.Add((Control) this.label1);
      this.groupBox2.Controls.Add((Control) this.scale);
      this.groupBox2.Controls.Add((Control) this.hard);
      this.groupBox2.Controls.Add((Control) this.label8);
      this.groupBox2.Controls.Add((Control) this.label6);
      this.groupBox2.Controls.Add((Control) this.lords);
      this.groupBox2.Controls.Add((Control) this.pacifist);
      this.groupBox2.Controls.Add((Control) this.label7);
      this.groupBox2.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.groupBox2.Location = new Point(475, 14);
      this.groupBox2.Margin = new Padding(4);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new Padding(4);
      this.groupBox2.Size = new Size(451, 296);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Options";
      this.label12.AutoSize = true;
      this.label12.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label12.Location = new Point(24, 265);
      this.label12.Name = "label12";
      this.label12.Size = new Size(335, 13);
      this.label12.TabIndex = 25;
      this.label12.Text =
          "Immediately warp away from Asylum, returning later through a fog gate";
      this.start.AutoSize = true;
      this.start.Location = new Point(7, 244);
      this.start.Margin = new Padding(3, 2, 3, 2);
      this.start.Name = "start";
      this.start.Size = new Size(215, 20);
      this.start.TabIndex = 24;
      this.start.Text = "Random start outside of Asylum";
      this.start.UseVisualStyleBackColor = true;
      this.label11.AutoSize = true;
      this.label11.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label11.Location = new Point(24, 229);
      this.label11.Name = "label11";
      this.label11.Size = new Size(365, 13);
      this.label11.TabIndex = 23;
      this.label11.Text =
          "If enabled, entering a fog gate you just exited can send you somewhere else";
      this.unconnected.AutoSize = true;
      this.unconnected.Location = new Point(7, 208);
      this.unconnected.Margin = new Padding(3, 2, 3, 2);
      this.unconnected.Name = "unconnected";
      this.unconnected.Size = new Size(169, 20);
      this.unconnected.TabIndex = 22;
      this.unconnected.Text = "Disconnected fog gates";
      this.unconnected.UseVisualStyleBackColor = true;
      this.label9.AutoSize = true;
      this.label9.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label9.Location = new Point(24, 193);
      this.label9.Name = "label9";
      this.label9.Size = new Size(280, 13);
      this.label9.TabIndex = 21;
      this.label9.Text =
          "BoC floor no longer crumbles. Not related to randomization";
      this.bboc.AutoSize = true;
      this.bboc.Location = new Point(7, 172);
      this.bboc.Margin = new Padding(3, 2, 3, 2);
      this.bboc.Name = "bboc";
      this.bboc.Size = new Size(155, 20);
      this.bboc.TabIndex = 20;
      this.bboc.Text = "No-Fall Bed of Chaos";
      this.bboc.UseVisualStyleBackColor = true;
      this.bboc.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label1.Location = new Point(24, 156);
      this.label1.Name = "label1";
      this.label1.Size = new Size(357, 13);
      this.label1.TabIndex = 19;
      this.label1.Text =
          "Various glitches may be required, similar to Race Mode+ in item randomizer";
      this.scale.AutoSize = true;
      this.scale.Checked = true;
      this.scale.CheckState = CheckState.Checked;
      this.scale.Location = new Point(7, 22);
      this.scale.Margin = new Padding(3, 2, 3, 2);
      this.scale.Name = "scale";
      this.scale.Size = new Size(191, 20);
      this.scale.TabIndex = 12;
      this.scale.Text = "Scale enemies and bosses";
      this.scale.UseVisualStyleBackColor = true;
      this.scale.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.hard.AutoSize = true;
      this.hard.Location = new Point(7, 135);
      this.hard.Margin = new Padding(3, 2, 3, 2);
      this.hard.Name = "hard";
      this.hard.Size = new Size(108, 20);
      this.hard.TabIndex = 18;
      this.hard.Text = "Glitched logic";
      this.hard.UseVisualStyleBackColor = true;
      this.hard.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label8.AutoSize = true;
      this.label8.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label8.Location = new Point(24, 43);
      this.label8.Name = "label8";
      this.label8.Size = new Size(371, 13);
      this.label8.TabIndex = 13;
      this.label8.Text =
          "Increase or decrease enemy health and damage based on distance from start";
      this.label6.AutoSize = true;
      this.label6.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label6.Location = new Point(23, 119);
      this.label6.Name = "label6";
      this.label6.Size = new Size(251, 13);
      this.label6.TabIndex = 17;
      this.label6.Text = "Allow escaping boss fights without defeating bosses";
      this.lords.AutoSize = true;
      this.lords.Checked = true;
      this.lords.CheckState = CheckState.Checked;
      this.lords.Location = new Point(5, 62);
      this.lords.Margin = new Padding(3, 2, 3, 2);
      this.lords.Name = "lords";
      this.lords.Size = new Size(142, 20);
      this.lords.TabIndex = 14;
      this.lords.Text = "Require Lord Souls";
      this.lords.UseVisualStyleBackColor = true;
      this.lords.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.pacifist.AutoSize = true;
      this.pacifist.Location = new Point(5, 98);
      this.pacifist.Margin = new Padding(3, 2, 3, 2);
      this.pacifist.Name = "pacifist";
      this.pacifist.Size = new Size(108, 20);
      this.pacifist.TabIndex = 16;
      this.pacifist.Text = "Pacifist Mode";
      this.pacifist.UseVisualStyleBackColor = true;
      this.pacifist.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label7.AutoSize = true;
      this.label7.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label7.Location = new Point(23, 82);
      this.label7.Name = "label7";
      this.label7.Size = new Size(225, 13);
      this.label7.TabIndex = 15;
      this.label7.Text = "Require opening the kiln door to access Gwyn";
      this.fixedseed.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.fixedseed.Location = new Point(161, 379);
      this.fixedseed.Margin = new Padding(3, 2, 3, 2);
      this.fixedseed.Name = "fixedseed";
      this.fixedseed.Size = new Size(153, 22);
      this.fixedseed.TabIndex = 5;
      this.label10.AutoSize = true;
      this.label10.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.label10.Location = new Point(18, 382);
      this.label10.Name = "label10";
      this.label10.Size = new Size(137, 16);
      this.label10.TabIndex = 3;
      this.label10.Text = "Fixed seed (optional):";
      this.randb.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.randb.ForeColor = SystemColors.ControlText;
      this.randb.Location = new Point(794, 379);
      this.randb.Margin = new Padding(3, 2, 3, 2);
      this.randb.Name = "randb";
      this.randb.Size = new Size(121, 27);
      this.randb.TabIndex = 6;
      this.randb.Text = "Randomize!";
      this.randb.UseVisualStyleBackColor = false;
      this.randb.Click += new EventHandler(this.Randomize);
      this.button2.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.button2.Location = new Point(794, 318);
      this.button2.Margin = new Padding(3, 2, 3, 2);
      this.button2.Name = "button2";
      this.button2.Size = new Size(121, 27);
      this.button2.TabIndex = 3;
      this.button2.Text = "Select game exe";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.OpenExe);
      this.exe.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.exe.Location = new Point(16, 320);
      this.exe.Margin = new Padding(3, 2, 3, 2);
      this.exe.Name = "exe";
      this.exe.Size = new Size(770, 22);
      this.exe.TabIndex = 2;
      this.exe.TextChanged += new EventHandler(this.UpdateFile);
      this.restoreButton.Enabled = false;
      this.restoreButton.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.restoreButton.Location = new Point(794, 349);
      this.restoreButton.Margin = new Padding(3, 2, 3, 2);
      this.restoreButton.Name = "restoreButton";
      this.restoreButton.Size = new Size(121, 27);
      this.restoreButton.TabIndex = 4;
      this.restoreButton.Text = "Restore backups";
      this.restoreButton.UseVisualStyleBackColor = true;
      this.restoreButton.Click += new EventHandler(this.Restore);
      this.restoreL.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.restoreL.ForeColor = SystemColors.ControlDarkDark;
      this.restoreL.Location = new Point(320, 347);
      this.restoreL.Name = "restoreL";
      this.restoreL.Size = new Size(466, 27);
      this.restoreL.TabIndex = 9;
      this.restoreL.TextAlign = ContentAlignment.MiddleRight;
      this.statusStrip1.Items.AddRange(new ToolStripItem[1] {
          (ToolStripItem) this.statusL
      });
      this.statusStrip1.Location = new Point(0, 428);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new Size(932, 22);
      this.statusStrip1.TabIndex = 10;
      this.statusStrip1.Text = "statusStrip1";
      this.statusL.Name = "statusL";
      this.statusL.Size = new Size(131, 17);
      this.statusL.Text = "Created by thefifthmatt";
      this.randomizeL.BackColor = SystemColors.Control;
      this.randomizeL.BorderStyle = BorderStyle.None;
      this.randomizeL.Font = new Font("Microsoft Sans Serif", 9f);
      this.randomizeL.Location = new Point(796, 410);
      this.randomizeL.Name = "randomizeL";
      this.randomizeL.ReadOnly = true;
      this.randomizeL.Size = new Size(119, 14);
      this.randomizeL.TabIndex = 11;
      this.randomizeL.TextAlign = HorizontalAlignment.Center;
      this.language.DropDownStyle = ComboBoxStyle.DropDownList;
      this.language.Enabled = false;
      this.language.FormattingEnabled = true;
      this.language.Location = new Point(161, 349);
      this.language.Name = "language";
      this.language.Size = new Size(153, 24);
      this.language.TabIndex = 12;
      this.language.SelectedIndexChanged +=
          new EventHandler(this.UpdateLanguage);
      this.label13.AutoSize = true;
      this.label13.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.label13.Location = new Point(19, 354);
      this.label13.Name = "label13";
      this.label13.Size = new Size(108, 16);
      this.label13.TabIndex = 13;
      this.label13.Text = "Game language:";
      this.label14.AutoSize = true;
      this.label14.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.label14.Location = new Point(19, 275);
      this.label14.Name = "label14";
      this.label14.Size = new Size(382, 32);
      this.label14.TabIndex = 14;
      this.label14.Text =
          "Runs usually take 4-8 hours to complete depending on options. \r\nSee documentation to learn more!";
      this.AutoScaleDimensions = new SizeF(8f, 16f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(932, 450);
      this.Controls.Add((Control) this.label14);
      this.Controls.Add((Control) this.label13);
      this.Controls.Add((Control) this.language);
      this.Controls.Add((Control) this.randomizeL);
      this.Controls.Add((Control) this.statusStrip1);
      this.Controls.Add((Control) this.restoreL);
      this.Controls.Add((Control) this.restoreButton);
      this.Controls.Add((Control) this.exe);
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.randb);
      this.Controls.Add((Control) this.label10);
      this.Controls.Add((Control) this.fixedseed);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.groupBox1);
      this.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Margin = new Padding(4);
      this.Name = nameof(MainForm);
      this.Text = "DS1 Fog Gate Randomizer v0.3";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}