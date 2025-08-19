using System;
using System.Xml.Serialization;

namespace CommandScheduler
{
    public class CommandConfig
    {
        [XmlIgnore]
        public TimeSpan TimePeriod { get; set; }

        // Surrogate property for XML serialization
        public string TimePeriodString
        {
            get { return TimePeriod.ToString("c"); }
            set { TimePeriod = TimeSpan.Parse(value); }
        }

        public string WorkingDirectory { get; set; }
        public string Command { get; set; }
        public bool IsEnabled { get; set; }

        [XmlIgnore]
        public int FailureCount { get; set; }
    }
}
