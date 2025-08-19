using System;

namespace CommandScheduler
{
    public class CommandConfig
    {
        public TimeSpan TimePeriod { get; set; }
        public string WorkingDirectory { get; set; }
        public string Command { get; set; }
        public bool IsEnabled { get; set; }
    }
}
