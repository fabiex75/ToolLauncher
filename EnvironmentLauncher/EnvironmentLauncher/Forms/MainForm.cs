using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvironmentLauncher.Forms;
using EnvironmentLauncher.Models;
using EnvironmentLauncher.Services;

namespace EnvironmentLauncher
{
    public class MainForm : Form
    {
        private readonly ConfigurationService _configService = new();
        private readonly ToolLauncherService _launcher = new();

        private ListBox lstTools = new();
        private Button btnTools = new();
        private Button btnLaunch = new();

        private DataGridView gridEnv = new();
        private Button btnSaveEnv = new();
        private Button btnResetEnv = new();
        private BindingList<EnvironmentVariable> _envList = new();
        private BindingList<ToolConfig> _toolsList = new();

        private DataGridView gridTools;
        private Button btnSaveTools;

        // Layout
        private SplitContainer split = new();
        private TabControl tabControl = new();
        private TabPage tabEnv = new();
        private TabPage tabTools = new();
        private TabPage tabVersions = new();

        // Versions UI
        private ComboBox cmbJavaVersions = new();
        private Button btnSetJavaHome = new();
        private ComboBox cmbMavenVersions = new();
        private Button btnSetMavenHome = new();
        private ComboBox cmbNodeVersions = new();
        private Button btnSetNodeHome = new();

        public MainForm()
        {
            Text = "Environment Tool Launcher";
            Width = 900;
            Height = 600;

            // Left: tools list; Right: tabs (Env / Tools / Versions)
            split.Dock = DockStyle.Fill;
            split.SplitterDistance = 320;
            split.FixedPanel = FixedPanel.Panel1;

            lstTools.Dock = DockStyle.Fill;
            split.Panel1.Controls.Add(lstTools);

            // Tab control on the right
            tabControl.Dock = DockStyle.Fill;
            tabEnv.Text = "Environment";
            tabTools.Text = "Tools";
            tabVersions.Text = "Versions";

            // environment grid in tabEnv (editable temporary) with its own buttons
            gridEnv.Dock = DockStyle.Fill;
            gridEnv.AllowUserToAddRows = true;
            gridEnv.AllowUserToDeleteRows = true;
            gridEnv.EditMode = DataGridViewEditMode.EditOnEnter;

            var envPanelMain = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            envPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            envPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            var envButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            envButtons.Controls.Add(btnSaveEnv);
            envButtons.Controls.Add(btnResetEnv);
            envPanelMain.Controls.Add(gridEnv, 0, 0);
            envPanelMain.Controls.Add(envButtons, 0, 1);
            tabEnv.Controls.Add(envPanelMain);

            // tools tab: embedded grid and save button
            gridTools = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false };
            gridTools.AllowUserToAddRows = true;
            gridTools.AllowUserToDeleteRows = true;
            gridTools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Nome", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            gridTools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ExecutablePath", HeaderText = "Eseguibile", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            gridTools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Arguments", HeaderText = "Argomenti", Width = 150 });
            gridTools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "WorkingDirectory", HeaderText = "Working Dir", Width = 150 });
            gridTools.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "UseEnvironment", HeaderText = "Usa Env", Width = 60 });

            btnSaveTools = new Button { Text = "Salva Tool", AutoSize = true };
            var toolsPanelMain = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            toolsPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            toolsPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            var toolsButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            toolsButtons.Controls.Add(btnSaveTools);
            toolsPanelMain.Controls.Add(gridTools, 0, 0);
            toolsPanelMain.Controls.Add(toolsButtons, 0, 1);
            tabTools.Controls.Add(toolsPanelMain);

            // versions tab: Java, Maven, Node controls
            var grpJava = new GroupBox { Text = "Java (JDK)", Dock = DockStyle.Top, Height = 90 };
            cmbJavaVersions.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbJavaVersions.Width = 500;
            cmbJavaVersions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnSetJavaHome.Text = "Imposta JAVA_HOME";
            btnSetJavaHome.AutoSize = true;
            grpJava.Controls.Add(cmbJavaVersions);
            grpJava.Controls.Add(btnSetJavaHome);
            cmbJavaVersions.Location = new System.Drawing.Point(8, 22);
            btnSetJavaHome.Location = new System.Drawing.Point(8, 52);
            tabVersions.Controls.Add(grpJava);

            var grpMaven = new GroupBox { Text = "Maven", Dock = DockStyle.Top, Height = 90 };
            cmbMavenVersions.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMavenVersions.Width = 500;
            cmbMavenVersions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnSetMavenHome.Text = "Imposta MAVEN_HOME";
            btnSetMavenHome.AutoSize = true;
            grpMaven.Controls.Add(cmbMavenVersions);
            grpMaven.Controls.Add(btnSetMavenHome);
            cmbMavenVersions.Location = new System.Drawing.Point(8, 22);
            btnSetMavenHome.Location = new System.Drawing.Point(8, 52);
            tabVersions.Controls.Add(grpMaven);

            var grpNode = new GroupBox { Text = "Node.js", Dock = DockStyle.Top, Height = 90 };
            cmbNodeVersions.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNodeVersions.Width = 500;
            cmbNodeVersions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnSetNodeHome.Text = "Imposta NODE_HOME";
            btnSetNodeHome.AutoSize = true;
            grpNode.Controls.Add(cmbNodeVersions);
            grpNode.Controls.Add(btnSetNodeHome);
            cmbNodeVersions.Location = new System.Drawing.Point(8, 22);
            btnSetNodeHome.Location = new System.Drawing.Point(8, 52);
            tabVersions.Controls.Add(grpNode);

            tabControl.TabPages.AddRange(new TabPage[] { tabEnv, tabTools, tabVersions });
            split.Panel2.Controls.Add(tabControl);

            // bottom panel with actions
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };

            btnLaunch.Text = "Avvia";
            btnSaveEnv.Text = "Salva Variabili";
            btnResetEnv.Text = "Reset valori salvati";
            btnLaunch.AutoSize = true;
            btnSaveEnv.AutoSize = true;
            btnResetEnv.AutoSize = true;

            panel.Controls.Add(btnLaunch);

            Controls.Add(split);
            Controls.Add(panel);

            InitializeEnvGridColumns();

            Load += (_, _) => { LoadTools(); LoadEnv(); PopulateJavaVersions(); PopulateMavenVersions(); PopulateNodeVersions(); };

            btnSaveTools.Click += (_, _) => SaveTools();
            btnLaunch.Click += LaunchSelected;
            btnSaveEnv.Click += (_, _) => SaveEnv();
            btnResetEnv.Click += (_, _) => ResetEnvToSaved();
            btnSetJavaHome.Click += (_, _) => ApplySelectedJavaHome();
            btnSetMavenHome.Click += (_, _) => ApplySelectedMavenHome();
            btnSetNodeHome.Click += (_, _) => ApplySelectedNodeHome();
        }

        private void InitializeEnvGridColumns()
        {
            gridEnv.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "Enabled",
                HeaderText = "Usa",
                Width = 40
            });

            gridEnv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Nome",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            gridEnv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Value",
                HeaderText = "Valore",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            gridEnv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Scope",
                HeaderText = "Ambito",
                Width = 100,
                ReadOnly = true
            });
        }

        private void LoadEnv()
        {
            var config = _configService.Load();

            var dict = new Dictionary<string, EnvironmentVariable>(StringComparer.OrdinalIgnoreCase);
            foreach (var v in config.EnvironmentVariables ?? new List<EnvironmentVariable>())
            {
                if (v == null) continue;
                if (string.IsNullOrEmpty(v.Name)) continue;
                dict[v.Name] = v;
            }

            // merge with current user environment without overwriting saved values
            try
            {
                var userEnv = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);
                foreach (DictionaryEntry de in userEnv)
                {
                    var name = de.Key as string;
                    var value = de.Value as string ?? string.Empty;
                    if (string.IsNullOrEmpty(name))
                        continue;

                    if (!dict.ContainsKey(name))
                        dict[name] = new EnvironmentVariable { Name = name, Value = value, Enabled = true, Scope = "User" };
                }
            }
            catch { }

            _envList = new BindingList<EnvironmentVariable>(dict.Values.OrderBy(v => v.Name).ToList());
            gridEnv.DataSource = _envList;
        }

        private void SaveEnv()
        {
            var config = _configService.Load();
            config.EnvironmentVariables = _envList.ToList();
            _configService.Save(config);
            var result = MessageBox.Show("Vuoi applicare le variabili abilitate come variabili utente (persistenti)?\nSì = Applica alle variabili utente, No = Solo salva nella configurazione, Annulla = Annulla operazione.",
                "Applica variabili utente?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                // revert save? keep saved config but do nothing
                return;
            }

            if (result == DialogResult.Yes)
            {
                var applied = 0;
                foreach (var ev in _envList.Where(e => e.Enabled))
                {
                    try
                    {
                        Environment.SetEnvironmentVariable(ev.Name, ev.Value, EnvironmentVariableTarget.User);
                        applied++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Impossibile impostare la variabile '{ev.Name}': {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                MessageBox.Show($"{applied} variabili applicate all'utente. Potrebbe essere necessario effettuare il logout/login per renderle effettive nelle nuove sessioni.", "Completato", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Variabili salvate nella configurazione.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetEnvToSaved()
        {
            // Re-read current user environment and merge with saved configuration,
            // then refresh the grid so the user sees the current defaults.
            LoadEnv();
            MessageBox.Show("Ambiente ricaricato dai valori correnti dell'utente.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadTools()
        {
            lstTools.Items.Clear();
            var config = _configService.Load();

            EnsureDefaultTools(config);

            _toolsList = new BindingList<ToolConfig>(config.Tools ?? new List<ToolConfig>());
            gridTools.DataSource = _toolsList;

            lstTools.DisplayMember = "Name";
            foreach (var t in _toolsList)
                lstTools.Items.Add(t);
        }

        private void SaveTools()
        {
            var config = _configService.Load();
            config.Tools = _toolsList.ToList();
            _configService.Save(config);
            // refresh left list
            LoadTools();
            MessageBox.Show("Tool salvati.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EnsureDefaultTools(AppConfig config)
        {
            // if tools list is empty or missing common tools, try to detect and add them
            var known = new List<(string name, string[] possiblePaths, string exe)>
            {
                ("Visual Studio Code", new [] {"C:\\Program Files\\Microsoft VS Code\\Code.exe","C:\\Users\\%USERNAME%\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe"}, "Code.exe"),
                ("Visual Studio", new [] {"C:\\Program Files\\Microsoft Visual Studio"}, "devenv.exe"),
                ("Notepad++", new [] {"C:\\Program Files\\Notepad++\\notepad++.exe"}, "notepad++.exe"),
            };

            foreach (var k in known)
            {
                bool exists = config.Tools.Any(t => t.Name.Contains(k.name, StringComparison.OrdinalIgnoreCase));
                if (exists) continue;

                string? found = null;
                foreach (var p in k.possiblePaths)
                {
                    var path = p.Replace("%USERNAME%", Environment.UserName);
                    if (File.Exists(path)) { found = path; break; }

                    // if it's a folder root to search
                    if (Directory.Exists(path) && !p.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var exe = Directory.EnumerateFiles(path, k.exe, SearchOption.AllDirectories).FirstOrDefault();
                            if (!string.IsNullOrEmpty(exe)) { found = exe; break; }
                        }
                        catch { }
                    }
                }

                if (!string.IsNullOrEmpty(found))
                {
                    config.Tools.Add(new ToolConfig { Name = k.name, ExecutablePath = found, Arguments = "", UseEnvironment = true });
                }
            }

            // detect multi-version tools (java, node, maven)
            DetectMultipleVersions(config, "Java", new [] { "C:\\Program Files\\Java", "C:\\Program Files (x86)\\Java" }, "java.exe", "bin\\java.exe");
            DetectMultipleVersions(config, "Node.js", new [] { "C:\\Program Files\\nodejs" }, "node.exe", "node.exe");
            DetectMultipleVersions(config, "Maven", new [] { "C:\\Program Files\\Apache Software Foundation","C:\\Program Files" }, "mvn.cmd", "bin\\mvn.cmd");

            // persist if we added defaults
            _configService.Save(config);
        }

        private void DetectMultipleVersions(AppConfig config, string displayName, string[] roots, string exeName, string relativeExe)
        {
            try
            {
                var found = new List<string>();
                foreach (var root in roots)
                {
                    if (!Directory.Exists(root)) continue;

                    foreach (var dir in Directory.EnumerateDirectories(root))
                    {
                        var candidate = Path.Combine(dir, relativeExe);
                        if (File.Exists(candidate))
                            found.Add(candidate);
                        else
                        {
                            // try deeper search for exeName
                            try
                            {
                                var exe = Directory.EnumerateFiles(dir, exeName, SearchOption.AllDirectories).FirstOrDefault();
                                if (!string.IsNullOrEmpty(exe)) found.Add(exe);
                            }
                            catch { }
                        }
                    }
                }

                // add entries for each found version if not present
                foreach (var f in found)
                {
                    if (config.Tools.Any(t => string.Equals(t.ExecutablePath, f, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    var name = displayName + " (" + Path.GetFileName(Path.GetDirectoryName(f)) + ")";
                    config.Tools.Add(new ToolConfig { Name = name, ExecutablePath = f, Arguments = "", UseEnvironment = true });
                }
            }
            catch { }
        }

        private void PopulateJavaVersions()
        {
            cmbJavaVersions.Items.Clear();
            var found = new List<string>();

            // search common program files locations (already scanned) and user locations
            var user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var candidates = new List<string>
            {
                Path.Combine(user, ".jdks"),
                Path.Combine(user, "jdk"),
                Path.Combine(user, "AppData", "Local", "Programs"),
                "C:\\Program Files\\Java",
                "C:\\Program Files (x86)\\Java"
            };

            foreach (var c in candidates)
            {
                try
                {
                    if (!Directory.Exists(c)) continue;

                    // search top-level directories for bin\java.exe
                    foreach (var d in Directory.EnumerateDirectories(c))
                    {
                        var javaExe = Path.Combine(d, "bin", "java.exe");
                        if (File.Exists(javaExe))
                        {
                            found.Add(d);
                            continue;
                        }

                        // try to find deeper
                        try
                        {
                            var exe = Directory.EnumerateFiles(d, "java.exe", SearchOption.AllDirectories).FirstOrDefault();
                            if (!string.IsNullOrEmpty(exe))
                            {
                                string? root = Path.GetDirectoryName(Path.GetDirectoryName(exe));
                                if (!string.IsNullOrEmpty(root)) found.Add(root);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // make unique and add
            foreach (var v in found.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                cmbJavaVersions.Items.Add(v);
            }

            if (cmbJavaVersions.Items.Count > 0)
                cmbJavaVersions.SelectedIndex = 0;
        }

        private void ApplySelectedJavaHome()
        {
            if (cmbJavaVersions.SelectedItem is not string root)
            {
                MessageBox.Show("Seleziona una versione JDK.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // set or update JAVA_HOME in the env list (use root path)
            var existing = _envList.FirstOrDefault(e => string.Equals(e.Name, "JAVA_HOME", StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Value = root;
                existing.Enabled = true;
            }
            else
            {
                _envList.Add(new EnvironmentVariable { Name = "JAVA_HOME", Value = root, Enabled = true, Scope = "User" });
            }

            // refresh grid
            gridEnv.Refresh();
            // update PATH to include bin if present
            var binJava = Path.Combine(root, "bin");
            EnsurePathContains(binJava);

            MessageBox.Show($"JAVA_HOME impostato su: {root}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PopulateMavenVersions()
        {
            cmbMavenVersions.Items.Clear();
            var found = new List<string>();
            var candidates = new List<string>
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Apache Software Foundation"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Maven"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Maven")
            };

            foreach (var c in candidates)
            {
                try
                {
                    if (!Directory.Exists(c)) continue;
                    foreach (var d in Directory.EnumerateDirectories(c))
                    {
                        var mvn = Path.Combine(d, "bin", "mvn.cmd");
                        if (File.Exists(mvn)) found.Add(d);
                    }
                }
                catch { }
            }

            foreach (var v in found.Distinct(StringComparer.OrdinalIgnoreCase))
                cmbMavenVersions.Items.Add(v);

            if (cmbMavenVersions.Items.Count > 0)
                cmbMavenVersions.SelectedIndex = 0;
        }

        private void ApplySelectedMavenHome()
        {
            if (cmbMavenVersions.SelectedItem is not string root)
            {
                MessageBox.Show("Seleziona una versione di Maven.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var existing = _envList.FirstOrDefault(e => string.Equals(e.Name, "MAVEN_HOME", StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Value = root;
                existing.Enabled = true;
            }
            else
            {
                _envList.Add(new EnvironmentVariable { Name = "MAVEN_HOME", Value = root, Enabled = true, Scope = "User" });
            }

            var bin = Path.Combine(root, "bin");
            EnsurePathContains(bin);

            gridEnv.Refresh();
            MessageBox.Show($"MAVEN_HOME impostato su: {root}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PopulateNodeVersions()
        {
            cmbNodeVersions.Items.Clear();
            var found = new List<string>();
            var candidates = new List<string>
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "nodejs")
            };

            foreach (var c in candidates)
            {
                try
                {
                    if (!Directory.Exists(c)) continue;
                    if (File.Exists(Path.Combine(c, "node.exe"))) found.Add(c);
                }
                catch { }
            }

            foreach (var v in found.Distinct(StringComparer.OrdinalIgnoreCase))
                cmbNodeVersions.Items.Add(v);

            if (cmbNodeVersions.Items.Count > 0)
                cmbNodeVersions.SelectedIndex = 0;
        }

        private void ApplySelectedNodeHome()
        {
            if (cmbNodeVersions.SelectedItem is not string root)
            {
                MessageBox.Show("Seleziona una versione di Node.js.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var existing = _envList.FirstOrDefault(e => string.Equals(e.Name, "NODE_HOME", StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Value = root;
                existing.Enabled = true;
            }
            else
            {
                _envList.Add(new EnvironmentVariable { Name = "NODE_HOME", Value = root, Enabled = true, Scope = "User" });
            }

            EnsurePathContains(root);

            gridEnv.Refresh();
            MessageBox.Show($"NODE_HOME impostato su: {root}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EnsurePathContains(string pathToAdd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pathToAdd)) return;
                pathToAdd = Path.GetFullPath(pathToAdd);
                if (!Directory.Exists(pathToAdd)) return;

                var current = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
                var parts = current.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
                if (parts.Any(p => string.Equals(p, pathToAdd, StringComparison.OrdinalIgnoreCase)))
                    return;

                var updated = pathToAdd + ";" + current;
                Environment.SetEnvironmentVariable("PATH", updated, EnvironmentVariableTarget.User);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossibile aggiornare PATH: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LaunchSelected(object? sender, EventArgs e)
        {
            if (lstTools.SelectedItem is not ToolConfig tool)
                return;

            // use current (possibly temporary) env list from the grid for launching
            _launcher.Launch(tool, _envList.ToList());
        }
    }

}
