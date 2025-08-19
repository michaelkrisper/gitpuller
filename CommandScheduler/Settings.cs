using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CommandScheduler
{
    public class Settings
    {
        private static readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CommandScheduler", "settings.xml");

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

            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                using (var reader = new StreamReader(_filePath))
                {
                    return (Settings)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                // If deserialization fails, return new settings
                return new Settings();
            }
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializer = new XmlSerializer(typeof(Settings));
            using (var writer = new StreamWriter(_filePath))
            {
                serializer.Serialize(writer, this);
            }
        }
    }
}
