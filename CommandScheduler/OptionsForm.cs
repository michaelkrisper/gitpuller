using System;
using System.Windows.Forms;

namespace CommandScheduler
{
    public partial class OptionsForm : Form
    {
        private Settings _settings;

        public OptionsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            _settings = Settings.Load();
            dataGridView1.Rows.Clear();
            foreach (var command in _settings.Commands)
            {
                dataGridView1.Rows.Add(command.TimePeriod.ToString(), command.WorkingDirectory, command.Command);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    dataGridView1.Rows.Remove(row);
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            _settings.Commands.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                var timePeriodCell = row.Cells[0].Value;
                var workingDirectoryCell = row.Cells[1].Value;
                var commandCell = row.Cells[2].Value;

                if (timePeriodCell == null || workingDirectoryCell == null || commandCell == null)
                {
                    MessageBox.Show("Please fill all cells.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (TimeSpan.TryParse(timePeriodCell.ToString(), out var timePeriod))
                {
                    _settings.Commands.Add(new CommandConfig
                    {
                        TimePeriod = timePeriod,
                        WorkingDirectory = workingDirectoryCell.ToString(),
                        Command = commandCell.ToString()
                    });
                }
                else
                {
                    MessageBox.Show("Invalid TimeSpan format. Please use hh:mm:ss.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            _settings.Save();
            Close();
        }
    }
}
