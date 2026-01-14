using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentLauncher.Models
{
    public class EnvironmentVariable
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        // Whether this variable should be applied when launching a tool
        public bool Enabled { get; set; } = true;
        // Scope: e.g. "User", "Machine", "Process" - used for informational purposes
        public string Scope { get; set; } = "User";
    }
}