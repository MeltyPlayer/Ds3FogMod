// Decompiled with JetBrains decompiler
// Type: FogMod.MainForm3
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

namespace FogMod
{
  public class MainForm3 : Form
  {
    private static string defaultDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\DARK SOULS III\\Game";
    private static string defaultPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\DARK SOULS III\\Game\\randomizer\\Data0.bdt";
    private RandomizerOptions options = new RandomizerOptions()
    {
      Game = GameSpec.FromGame.DS3
    };
    private bool working;
    private IContainer components;
    private GroupBox groupBox1;
    private GroupBox groupBox2;
    private Label bossL;
    private CheckBox boss;
    private Label label4;
    private CheckBox pvp;
    private Label label2;
    private CheckBox warp;
    private Label label1;
    private CheckBox scale;
    private CheckBox treeskip;
    private Label label8;
    private Label label6;
    private CheckBox lords;
    private CheckBox pacifist;
    private Label label7;
    private TextBox fixedseed;
    private Label label10;
    private Button randb;
    private Button button2;
    private TextBox exe;
    private Label errorL;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel statusL;
    private Label label11;
    private CheckBox unconnected;
    private Label label3;
    private Label label5;
    private GroupBox groupBox3;
    private Label label13;
    private RadioButton latewarp;
    private Label label12;
    private RadioButton instawarp;
    private Label label9;
    private RadioButton earlywarp;

    public MainForm3()
    {
      this.InitializeComponent();
      this.errorL.Text = "";
      string exe = Settings.Default.Exe;
      if (!string.IsNullOrWhiteSpace(exe) && exe.ToLowerInvariant().EndsWith("data0.bdt") && File.Exists(exe))
        this.exe.Text = exe;
      else if (File.Exists(MainForm3.defaultPath))
        this.exe.Text = MainForm3.defaultPath;
      this.options["dryrun"] = false;
      string options = Settings.Default.Options;
      if (string.IsNullOrWhiteSpace(options))
      {
        this.ReadControlFlags((Control) this);
      }
      else
      {
        List<string> list = ((IEnumerable<string>) options.Split(' ')).ToList<string>();
        this.SetControlFlags((Control) this, (ICollection<string>) list);
        foreach (string s in list)
        {
          uint result;
          if (uint.TryParse(s, out result))
          {
            if (result == 0U)
              break;
            this.fixedseed.Text = result.ToString();
            break;
          }
        }
      }
    }

    private void UpdateExePath()
    {
      if (!(this.exe.Text.Trim() == ""))
      {
        bool flag = true;
        try
        {
          if (!Directory.Exists(Path.GetDirectoryName(this.exe.Text)))
            flag = false;
          if (Path.GetFileName(this.exe.Text).ToLowerInvariant() != "data0.bdt")
            flag = false;
        }
        catch (ArgumentException ex)
        {
          flag = false;
        }
        if (!flag)
          return;
      }
      Settings.Default.Exe = this.exe.Text;
      Settings.Default.Save();
    }

    private void OpenExe(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Select Data0.bdt of other mod";
      openFileDialog.Filter = "Modded params|Data0.bdt|All files|*.*";
      openFileDialog.RestoreDirectory = true;
      try
      {
        if (Directory.Exists(this.exe.Text))
        {
          openFileDialog.InitialDirectory = this.exe.Text;
        }
        else
        {
          string directoryName = Path.GetDirectoryName(this.exe.Text);
          if (Directory.Exists(directoryName))
            openFileDialog.InitialDirectory = directoryName;
          else if (Directory.Exists(MainForm3.defaultDir))
            openFileDialog.InitialDirectory = MainForm3.defaultDir;
        }
      }
      catch (ArgumentException ex)
      {
      }
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      this.exe.Text = openFileDialog.FileName;
    }

    private void setStatus(string msg, bool error = false, bool success = false)
    {
      this.statusL.Text = msg;
      this.statusStrip1.BackColor = error ? Color.IndianRed : (success ? Color.PaleGreen : SystemColors.Control);
    }

    private async void Randomize(object sender, EventArgs e)
    {
      MainForm3 mainForm3 = this;
      if (mainForm3.working)
        return;
      mainForm3.ReadControlFlags((Control) mainForm3);
      string gameDir = (string) null;
      if (!string.IsNullOrWhiteSpace(mainForm3.exe.Text))
      {
        gameDir = Path.GetDirectoryName(mainForm3.exe.Text);
        if (!File.Exists(gameDir + "\\Data0.bdt"))
        {
          mainForm3.SetError("Error: Data0.bdt not found for the mod to merge. Leave it blank to use Fog Gate Randomizer by itself.");
          return;
        }
        if (new DirectoryInfo(gameDir).FullName == Directory.GetCurrentDirectory())
        {
          mainForm3.SetError("Error: Data0.bdt is not from a different mod! Leave it blank to use Fog Gate Randomizer by itself.");
          return;
        }
      }
      if (mainForm3.fixedseed.Text.Trim() != "")
      {
        uint result;
        if (uint.TryParse(mainForm3.fixedseed.Text.Trim(), out result))
        {
          mainForm3.options.Seed = (int) result;
        }
        else
        {
          mainForm3.SetError("Invalid fixed seed");
          return;
        }
      }
      else
        mainForm3.options.Seed = new Random().Next();
      mainForm3.fixedseed.Text = mainForm3.options.Seed.ToString();
      mainForm3.UpdateOptions((object) null, (EventArgs) null);
      mainForm3.SetError((string) null);
      mainForm3.working = true;
      string prevText = mainForm3.randb.Text;
      mainForm3.randb.Text = "Randomizing...";
      mainForm3.setStatus("Randomizing...", false, false);
      RandomizerOptions rand = mainForm3.options.Copy();
      mainForm3.randb.BackColor = Color.LightYellow;
      Randomizer randomizer = new Randomizer();
      await Task.Factory.StartNew((Action) (() =>
      {
        Directory.CreateDirectory("spoiler_logs");
        string path = string.Format("spoiler_logs\\{0}_log_{1}_{2}.txt", (object) DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss"), (object) rand.Seed, (object) rand.ConfigHash());
        TextWriter text = (TextWriter) File.CreateText(path);
        TextWriter newOut = Console.Out;
        Console.SetOut(text);
        try
        {
          ItemReader.Result result = randomizer.Randomize(rand, GameSpec.FromGame.DS3, gameDir, Directory.GetCurrentDirectory());
          this.setStatus("Done. Info in " + path + " | Restart your game!" + (result.Randomized ? " | Key item hash: " + result.ItemHash : ""), false, true);
        }
        catch (Exception ex)
        {
          Console.WriteLine((object) ex);
          this.SetError("Error encountered: " + ex.Message + "\r\n\r\nIt may work to try again with a different seed. " + (gameDir == null ? "" : "The merged mod might also not be compatible. ") + "See most recent file in spoiler_logs directory for the full error.");
          this.setStatus("Error! See error message in " + path, true, false);
        }
        finally
        {
          text.Close();
          Console.SetOut(newOut);
        }
      }));
      mainForm3.randb.Enabled = true;
      mainForm3.randb.Text = prevText;
      mainForm3.randb.BackColor = SystemColors.Control;
      mainForm3.working = false;
      mainForm3.UpdateExePath();
    }

    private void SetError(string text = null)
    {
      this.errorL.Text = text ?? "";
    }

    private void UpdateFile(object sender, EventArgs e)
    {
      this.UpdateExePath();
    }

    private void UpdateOptions(object sender, EventArgs e)
    {
      this.ReadControlFlags((Control) this);
      Settings.Default.Options = string.Join(" ", (IEnumerable<string>) this.options.GetEnabled()) + " " + (object) this.options.DisplaySeed;
      Settings.Default.Save();
    }

    private void ReadControlFlags(Control control)
    {
      switch (control)
      {
        case RadioButton radioButton:
          this.options[control.Name] = radioButton.Checked;
          break;
        case CheckBox checkBox:
          this.options[control.Name] = checkBox.Checked;
          break;
        default:
          IEnumerator enumerator = control.Controls.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
              this.ReadControlFlags((Control) enumerator.Current);
            break;
          }
          finally
          {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
      }
    }

    private void SetControlFlags(Control control, ICollection<string> set)
    {
      if (!control.Enabled)
        return;
      switch (control)
      {
        case RadioButton radioButton:
          this.options[control.Name] = radioButton.Checked = set.Contains(control.Name);
          break;
        case CheckBox checkBox:
          this.options[control.Name] = checkBox.Checked = set.Contains(control.Name);
          break;
        default:
          IEnumerator enumerator = control.Controls.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
              this.SetControlFlags((Control) enumerator.Current, set);
            break;
          }
          finally
          {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
      }
    }

    private void fixedseed_TextChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.fixedseed.Text))
        this.randb.Text = "Randomize!";
      else
        this.randb.Text = "Run with fixed seed";
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (MainForm3));
      this.groupBox1 = new GroupBox();
      this.label4 = new Label();
      this.pvp = new CheckBox();
      this.label2 = new Label();
      this.warp = new CheckBox();
      this.bossL = new Label();
      this.boss = new CheckBox();
      this.groupBox2 = new GroupBox();
      this.label11 = new Label();
      this.unconnected = new CheckBox();
      this.label1 = new Label();
      this.scale = new CheckBox();
      this.treeskip = new CheckBox();
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
      this.errorL = new Label();
      this.statusStrip1 = new StatusStrip();
      this.statusL = new ToolStripStatusLabel();
      this.label3 = new Label();
      this.label5 = new Label();
      this.groupBox3 = new GroupBox();
      this.label13 = new Label();
      this.earlywarp = new RadioButton();
      this.instawarp = new RadioButton();
      this.label9 = new Label();
      this.latewarp = new RadioButton();
      this.label12 = new Label();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      this.groupBox1.Controls.Add((Control) this.label4);
      this.groupBox1.Controls.Add((Control) this.pvp);
      this.groupBox1.Controls.Add((Control) this.label2);
      this.groupBox1.Controls.Add((Control) this.warp);
      this.groupBox1.Controls.Add((Control) this.bossL);
      this.groupBox1.Controls.Add((Control) this.boss);
      this.groupBox1.Controls.Add((Control) this.lords);
      this.groupBox1.Controls.Add((Control) this.label7);
      this.groupBox1.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.groupBox1.Location = new Point(16, 14);
      this.groupBox1.Margin = new Padding(4);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new Padding(4);
      this.groupBox1.Size = new Size(420, 181);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Randomized entrances";
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label4.Location = new Point(24, 117);
      this.label4.Name = "label4";
      this.label4.Size = new Size(265, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Enable and randomize fog gates separating PvP zones";
      this.pvp.AutoSize = true;
      this.pvp.Checked = true;
      this.pvp.CheckState = CheckState.Checked;
      this.pvp.Location = new Point(7, 96);
      this.pvp.Margin = new Padding(3, 2, 3, 2);
      this.pvp.Name = "pvp";
      this.pvp.Size = new Size(111, 20);
      this.pvp.TabIndex = 6;
      this.pvp.Text = "PvP fog gates";
      this.pvp.UseVisualStyleBackColor = true;
      this.pvp.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label2.Location = new Point(24, 81);
      this.label2.Name = "label2";
      this.label2.Size = new Size(208, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Randomize warp destinations, like to DLCs";
      this.warp.AutoSize = true;
      this.warp.Checked = true;
      this.warp.CheckState = CheckState.Checked;
      this.warp.Location = new Point(7, 58);
      this.warp.Margin = new Padding(3, 2, 3, 2);
      this.warp.Name = "warp";
      this.warp.Size = new Size(159, 20);
      this.warp.TabIndex = 4;
      this.warp.Text = "Warps between areas";
      this.warp.UseVisualStyleBackColor = true;
      this.warp.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.bossL.AutoSize = true;
      this.bossL.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.bossL.Location = new Point(24, 42);
      this.bossL.Name = "bossL";
      this.bossL.Size = new Size(199, 13);
      this.bossL.TabIndex = 3;
      this.bossL.Text = "Randomize fog gates to and from bosses";
      this.boss.AutoSize = true;
      this.boss.Checked = true;
      this.boss.CheckState = CheckState.Checked;
      this.boss.Location = new Point(7, 22);
      this.boss.Margin = new Padding(3, 2, 3, 2);
      this.boss.Name = "boss";
      this.boss.Size = new Size(117, 20);
      this.boss.TabIndex = 2;
      this.boss.Text = "Boss fog gates";
      this.boss.UseVisualStyleBackColor = true;
      this.boss.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.groupBox2.Controls.Add((Control) this.label11);
      this.groupBox2.Controls.Add((Control) this.unconnected);
      this.groupBox2.Controls.Add((Control) this.label1);
      this.groupBox2.Controls.Add((Control) this.scale);
      this.groupBox2.Controls.Add((Control) this.treeskip);
      this.groupBox2.Controls.Add((Control) this.label8);
      this.groupBox2.Controls.Add((Control) this.label6);
      this.groupBox2.Controls.Add((Control) this.pacifist);
      this.groupBox2.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.groupBox2.Location = new Point(444, 14);
      this.groupBox2.Margin = new Padding(4);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new Padding(4);
      this.groupBox2.Size = new Size(471, 181);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Misc options";
      this.label11.AutoSize = true;
      this.label11.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label11.Location = new Point(26, 154);
      this.label11.Name = "label11";
      this.label11.Size = new Size(361, 13);
      this.label11.TabIndex = 23;
      this.label11.Text = "Entering a fog gate you just exited can send you to a different fixed location";
      this.unconnected.AutoSize = true;
      this.unconnected.Location = new Point(9, 133);
      this.unconnected.Margin = new Padding(3, 2, 3, 2);
      this.unconnected.Name = "unconnected";
      this.unconnected.Size = new Size(169, 20);
      this.unconnected.TabIndex = 22;
      this.unconnected.Text = "Disconnected fog gates";
      this.unconnected.UseVisualStyleBackColor = true;
      this.unconnected.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label1.Location = new Point(26, 116);
      this.label1.Name = "label1";
      this.label1.Size = new Size(307, 13);
      this.label1.TabIndex = 19;
      this.label1.Text = "Logic assumes you can jump to Firelink Shrine roof from the tree";
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
      this.treeskip.AutoSize = true;
      this.treeskip.Location = new Point(9, 95);
      this.treeskip.Margin = new Padding(3, 2, 3, 2);
      this.treeskip.Name = "treeskip";
      this.treeskip.Size = new Size(84, 20);
      this.treeskip.TabIndex = 18;
      this.treeskip.Text = "Tree skip";
      this.treeskip.UseVisualStyleBackColor = true;
      this.treeskip.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label8.AutoSize = true;
      this.label8.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label8.Location = new Point(24, 43);
      this.label8.Name = "label8";
      this.label8.Size = new Size(371, 13);
      this.label8.TabIndex = 13;
      this.label8.Text = "Increase or decrease enemy health and damage based on distance from start";
      this.label6.AutoSize = true;
      this.label6.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label6.Location = new Point(25, 79);
      this.label6.Name = "label6";
      this.label6.Size = new Size(251, 13);
      this.label6.TabIndex = 17;
      this.label6.Text = "Allow escaping boss fights without defeating bosses";
      this.lords.AutoSize = true;
      this.lords.Checked = true;
      this.lords.CheckState = CheckState.Checked;
      this.lords.Location = new Point(7, 132);
      this.lords.Margin = new Padding(3, 2, 3, 2);
      this.lords.Name = "lords";
      this.lords.Size = new Size(179, 20);
      this.lords.TabIndex = 14;
      this.lords.Text = "Require Cinders of a Lord";
      this.lords.UseVisualStyleBackColor = true;
      this.lords.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.pacifist.AutoSize = true;
      this.pacifist.Location = new Point(7, 58);
      this.pacifist.Margin = new Padding(3, 2, 3, 2);
      this.pacifist.Name = "pacifist";
      this.pacifist.Size = new Size(108, 20);
      this.pacifist.TabIndex = 16;
      this.pacifist.Text = "Pacifist Mode";
      this.pacifist.UseVisualStyleBackColor = true;
      this.pacifist.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label7.AutoSize = true;
      this.label7.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label7.Location = new Point(25, 152);
      this.label7.Name = "label7";
      this.label7.Size = new Size(335, 13);
      this.label7.TabIndex = 15;
      this.label7.Text = "Access to Soul of Cinder via Firelink Shrine and Kiln is not randomized";
      this.fixedseed.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.fixedseed.Location = new Point(583, 391);
      this.fixedseed.Margin = new Padding(3, 2, 3, 2);
      this.fixedseed.Name = "fixedseed";
      this.fixedseed.Size = new Size(153, 22);
      this.fixedseed.TabIndex = 15;
      this.fixedseed.TextChanged += new EventHandler(this.fixedseed_TextChanged);
      this.label10.AutoSize = true;
      this.label10.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.label10.Location = new Point(534, 394);
      this.label10.Name = "label10";
      this.label10.Size = new Size(44, 16);
      this.label10.TabIndex = 3;
      this.label10.Text = "Seed:";
      this.randb.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.randb.ForeColor = SystemColors.ControlText;
      this.randb.Location = new Point(742, 389);
      this.randb.Margin = new Padding(3, 2, 3, 2);
      this.randb.Name = "randb";
      this.randb.Size = new Size(173, 27);
      this.randb.TabIndex = 16;
      this.randb.Text = "Randomize!";
      this.randb.UseVisualStyleBackColor = false;
      this.randb.Click += new EventHandler(this.Randomize);
      this.button2.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.button2.Location = new Point(742, 345);
      this.button2.Margin = new Padding(3, 2, 3, 2);
      this.button2.Name = "button2";
      this.button2.Size = new Size(173, 27);
      this.button2.TabIndex = 13;
      this.button2.Text = "Select other mod to merge";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.OpenExe);
      this.exe.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.exe.Location = new Point(12, 347);
      this.exe.Margin = new Padding(3, 2, 3, 2);
      this.exe.Name = "exe";
      this.exe.Size = new Size(724, 22);
      this.exe.TabIndex = 12;
      this.exe.TextChanged += new EventHandler(this.UpdateFile);
      this.errorL.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.errorL.ForeColor = Color.Crimson;
      this.errorL.Location = new Point(444, 211);
      this.errorL.Name = "errorL";
      this.errorL.Size = new Size(471, 128);
      this.errorL.TabIndex = 9;
      this.errorL.Text = componentResourceManager.GetString("errorL.Text");
      this.statusStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.statusL
      });
      this.statusStrip1.Location = new Point(0, 438);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new Size(932, 22);
      this.statusStrip1.TabIndex = 10;
      this.statusStrip1.Text = "statusStrip1";
      this.statusL.Name = "statusL";
      this.statusL.Size = new Size(131, 17);
      this.statusL.Text = "Created by thefifthmatt";
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label3.Location = new Point(741, 415);
      this.label3.Name = "label3";
      this.label3.Size = new Size(180, 13);
      this.label3.TabIndex = 24;
      this.label3.Text = "Leave seed blank for a random seed";
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label5.Location = new Point(740, 372);
      this.label5.Name = "label5";
      this.label5.Size = new Size(176, 13);
      this.label5.TabIndex = 25;
      this.label5.Text = "Leave blank to run this mod by itself";
      this.groupBox3.Controls.Add((Control) this.latewarp);
      this.groupBox3.Controls.Add((Control) this.instawarp);
      this.groupBox3.Controls.Add((Control) this.label9);
      this.groupBox3.Controls.Add((Control) this.earlywarp);
      this.groupBox3.Controls.Add((Control) this.label13);
      this.groupBox3.Controls.Add((Control) this.label12);
      this.groupBox3.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.groupBox3.Location = new Point(13, 203);
      this.groupBox3.Margin = new Padding(4);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Padding = new Padding(4);
      this.groupBox3.Size = new Size(423, 136);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Warping between bonfires";
      this.label13.AutoSize = true;
      this.label13.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label13.Location = new Point(24, 40);
      this.label13.Name = "label13";
      this.label13.Size = new Size(320, 13);
      this.label13.TabIndex = 13;
      this.label13.Text = "Firelink Shrine and Coiled Sword are routed in early. Balanced start";
      this.earlywarp.AutoSize = true;
      this.earlywarp.Checked = true;
      this.earlywarp.Location = new Point(9, 20);
      this.earlywarp.Name = "earlywarp";
      this.earlywarp.Size = new Size(198, 20);
      this.earlywarp.TabIndex = 14;
      this.earlywarp.TabStop = true;
      this.earlywarp.Text = "Coiled Sword available early";
      this.earlywarp.UseVisualStyleBackColor = true;
      this.earlywarp.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.instawarp.AutoSize = true;
      this.instawarp.Location = new Point(9, 92);
      this.instawarp.Name = "instawarp";
      this.instawarp.Size = new Size(180, 20);
      this.instawarp.TabIndex = 16;
      this.instawarp.Text = "Coiled Sword not required";
      this.instawarp.UseVisualStyleBackColor = true;
      this.instawarp.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label9.AutoSize = true;
      this.label9.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label9.Location = new Point(24, 112);
      this.label9.Name = "label9";
      this.label9.Size = new Size(388, 13);
      this.label9.TabIndex = 15;
      this.label9.Text = "Firelink Shrine, and warping between bonfires, is available immediately. Easy start";
      this.latewarp.AutoSize = true;
      this.latewarp.Location = new Point(7, 56);
      this.latewarp.Name = "latewarp";
      this.latewarp.Size = new Size(211, 20);
      this.latewarp.TabIndex = 18;
      this.latewarp.Text = "Coiled Sword can be anywhere";
      this.latewarp.UseVisualStyleBackColor = true;
      this.latewarp.CheckedChanged += new EventHandler(this.UpdateOptions);
      this.label12.AutoSize = true;
      this.label12.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.label12.Location = new Point(22, 76);
      this.label12.Name = "label12";
      this.label12.Size = new Size(388, 13);
      this.label12.TabIndex = 17;
      this.label12.Text = "Firelink is still early, but Coiled Sword is like Lordvessel in Dark Souls. Slower start";
      this.AutoScaleDimensions = new SizeF(8f, 16f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(932, 460);
      this.Controls.Add((Control) this.groupBox3);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.statusStrip1);
      this.Controls.Add((Control) this.exe);
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.randb);
      this.Controls.Add((Control) this.label10);
      this.Controls.Add((Control) this.fixedseed);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.groupBox1);
      this.Controls.Add((Control) this.errorL);
      this.Font = new Font("Microsoft Sans Serif", 9.75f);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Margin = new Padding(4);
      this.Name = nameof (MainForm3);
      this.Text = "DS3 Fog Gate Randomizer v0.1";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
