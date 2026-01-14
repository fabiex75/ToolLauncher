using System;
using System.Windows.Forms;

namespace EnvironmentLauncher.Forms
{
    [Obsolete("EnvVarsForm is deprecated — environment variables are managed in MainForm.")]
    public partial class EnvVarsForm : Form
    {
        public EnvVarsForm()
        {
            Text = "Deprecated";
            Width = 300;
            Height = 100;
            var l = new Label { Text = "Questa form è deprecata. Usa la MainForm.", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
            Controls.Add(l);
        }
    }
}
