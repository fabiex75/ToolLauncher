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
            // If there are existing running processes for this executable, we can either
            // kill them (if requested) or try to append special arguments to force
            // a new instance. Otherwise we start normally. This is necessary because
            // many GUI apps are single-instance and will route new launches to the
            // existing process (which won't inherit the environment of the new start).
            var startArgs = tool.Arguments ?? string.Empty;

            try
            {
                var exeName = System.IO.Path.GetFileNameWithoutExtension(tool.ExecutablePath ?? string.Empty);
                if (!string.IsNullOrEmpty(exeName))
                {
                    var running = System.Diagnostics.Process.GetProcessesByName(exeName);
                    if (running != null && running.Length > 0)
                    {
                        if (tool.KillExistingInstances)
                        {
                            foreach (var p in running)
                            {
                                try { p.Kill(); p.WaitForExit(3000); } catch { }
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(tool.ForceNewInstanceArguments))
                        {
                            // append special args to force a new instance (app-specific)
                            startArgs = (startArgs + " " + tool.ForceNewInstanceArguments).Trim();
                        }
                    }
                }
            }
            catch { }

            var startInfo = new ProcessStartInfo
            {
                FileName = tool.ExecutablePath,
                Arguments = startArgs,
                UseShellExecute = false
            };

            if (!string.IsNullOrWhiteSpace(tool.WorkingDirectory))
                startInfo.WorkingDirectory = tool.WorkingDirectory;

            // Always apply provided environment variables to the process (temporary, per-process)
            if (envVars != null)
            {
                // If PATH is present in envVars we want to merge/prepend it with the existing PATH
                var pathEntry = envVars.FirstOrDefault(e => e.Enabled && string.Equals(e.Name, "PATH", StringComparison.OrdinalIgnoreCase));
                string existingPath = string.Empty;
                try
                {
                    existingPath = startInfo.Environment.ContainsKey("PATH") ? startInfo.Environment["PATH"] ?? string.Empty : Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                }
                catch { existingPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty; }

                if (pathEntry != null)
                {
                    // Prepend the PATH value from configuration to preserve system/user entries
                    var toAdd = pathEntry.Value ?? string.Empty;
                    var mergedParts = new List<string>();
                    if (!string.IsNullOrWhiteSpace(toAdd))
                        mergedParts.AddRange(toAdd.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
                    if (!string.IsNullOrWhiteSpace(existingPath))
                        mergedParts.AddRange(existingPath.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));

                    // remove duplicates while preserving order
                    var finalPath = string.Join(";", mergedParts.Where(p => !string.IsNullOrEmpty(p)).Distinct(StringComparer.OrdinalIgnoreCase));
                    startInfo.Environment["PATH"] = finalPath;
                }

                foreach (var env in envVars.Where(e => e.Enabled))
                {
                    if (string.Equals(env.Name, "PATH", StringComparison.OrdinalIgnoreCase))
                        continue; // already handled

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
