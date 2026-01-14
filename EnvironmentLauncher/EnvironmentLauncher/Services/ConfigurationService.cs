using EnvironmentLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnvironmentLauncher.Services
{
    public class ConfigurationService
    {
        private const string ConfigFile = "config.json";

        public AppConfig Load()
        {
            if (!File.Exists(ConfigFile))
                return new AppConfig();

            var json = File.ReadAllText(ConfigFile);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }

        public void Save(AppConfig config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(ConfigFile, json);
        }
    }

    public class AppConfig
    {
        public List<EnvironmentVariable> EnvironmentVariables { get; set; } = new();
        public List<ToolConfig> Tools { get; set; } = new();
    }
}
