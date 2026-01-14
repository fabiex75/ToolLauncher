using EnvironmentLauncher.Models;
using EnvironmentLauncher.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvironmentLauncher.Forms
{
    public partial class ToolsForm : Form
    {
        private readonly ConfigurationService _service;
        private DataGridView grid = new();
        private Button btnSave = new();

        public ToolsForm(ConfigurationService service)
        {
            _service = service;

            Text = "Tool";
            Width = 700;
            Height = 400;

            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = false;

            // columns: Usa, Nome, Eseguibile, Argomenti, WorkingDir, UsaEnv
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Nome",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ExecutablePath",
                HeaderText = "Eseguibile",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Arguments",
                HeaderText = "Argomenti",
                Width = 150
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "WorkingDirectory",
                HeaderText = "Working Dir",
                Width = 150
            });

            grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "UseEnvironment",
                HeaderText = "Usa Env",
                Width = 60
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ForceNewInstanceArguments",
                HeaderText = "Forza args (new inst.)",
                Width = 200
            });

            grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "KillExistingInstances",
                HeaderText = "Uccidi istanze",
                Width = 80
            });

            btnSave.Text = "Salva";
            btnSave.Dock = DockStyle.Bottom;

            Controls.Add(grid);
            Controls.Add(btnSave);

            Load += (_, _) =>
            {
                var cfg = _service.Load();
                var list = cfg.Tools ?? new List<ToolConfig>();
                grid.DataSource = new BindingSource { DataSource = list };
            };

            btnSave.Click += Save;
        }

        private void Save(object? sender, EventArgs e)
        {
            var config = _service.Load();
            var data = ((BindingSource)grid.DataSource).DataSource as List<ToolConfig> ?? new List<ToolConfig>();
            config.Tools = data;

            _service.Save(config);
            Close();
        }
    }

}
