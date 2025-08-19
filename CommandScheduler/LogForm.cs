using System;
using System.Text;
using System.Windows.Forms;

namespace CommandScheduler
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
            Load += LogForm_Load;
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            SchedulerApplicationContext.OnLogEntryAdded += OnLogEntryAdded;
            FormClosed += (s, ev) => SchedulerApplicationContext.OnLogEntryAdded -= OnLogEntryAdded;
        }

        private void OnLogEntryAdded(LogEntry logEntry)
        {
            if (textBoxLogs.InvokeRequired)
            {
                textBoxLogs.Invoke(new Action(() => OnLogEntryAdded(logEntry)));
                return;
            }
            textBoxLogs.AppendText(logEntry.ToString());
        }

        public void SetLogs(System.Collections.Generic.IEnumerable<LogEntry> logs)
        {
            if (logs == null) return;
            var sb = new StringBuilder();
            foreach (var log in logs)
            {
                sb.Append(log.ToString());
            }
            textBoxLogs.Text = sb.ToString();
            textBoxLogs.SelectionStart = textBoxLogs.Text.Length;
            textBoxLogs.ScrollToCaret();
        }
    }
}
