using EnvironmentLauncher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentLauncher.Services
{
    public class ToolLauncherService
    {
        public void Launch(ToolConfig tool, List<EnvironmentVariable> envVars)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = tool.ExecutablePath,
                Arguments = tool.Arguments,
                UseShellExecute = false
            };

            if (!string.IsNullOrWhiteSpace(tool.WorkingDirectory))
                startInfo.WorkingDirectory = tool.WorkingDirectory;

            // Always apply provided environment variables to the process (temporary, per-process)
            if (envVars != null)
            {
                foreach (var env in envVars.Where(e => e.Enabled))
                {
                    startInfo.Environment[env.Name] = env.Value;
                }
            }

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                // Best-effort: surface the error to the caller via exception
                throw new InvalidOperationException($"Impossibile avviare '{tool?.Name}': {ex.Message}", ex);
            }
        }
    }
}
