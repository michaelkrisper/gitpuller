using System.Text;
using System.Windows.Forms;

namespace CommandScheduler
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public void SetLogs(System.Collections.Generic.IEnumerable<LogEntry> logs)
        {
            var sb = new StringBuilder();
            foreach (var log in logs)
            {
                sb.AppendLine(log.ToString());
            }
            textBoxLogs.Text = sb.ToString();
        }
    }
}
