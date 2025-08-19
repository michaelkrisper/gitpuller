using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CommandScheduler
{
    public class Settings
    {
        private static readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CommandScheduler", "settings.json");

        public List<CommandConfig> Commands { get; set; }

        public Settings()
        {
            Commands = new List<CommandConfig>();
        }

        public static Settings Load()
        {
            if (!File.Exists(_filePath))
            {
                return new Settings();
            }

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<Settings>(json);
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
