using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentLauncher.Models
{
    public class ToolConfig
    {
        public string Name { get; set; } = string.Empty;
        public string ExecutablePath { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        // Optional working directory for the process
        public string WorkingDirectory { get; set; } = string.Empty;
        // If true, the configured environment variables will be applied when launching
        public bool UseEnvironment { get; set; } = true;
    }

}
