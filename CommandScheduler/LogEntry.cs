using System;

namespace CommandScheduler
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Command { get; set; }
        public string Output { get; set; }
        public TimeSpan Duration { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] - Command: {Command} - Duration: {Duration.TotalMilliseconds}ms\nOutput:\n{Output}\n";
        }
    }
}
