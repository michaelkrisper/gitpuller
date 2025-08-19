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
                dataGridView1.Rows.Add(command.IsEnabled, command.TimePeriod.ToString(), command.WorkingDirectory, command.Command);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click is on the delete button column and it's not the header row
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["Delete"].Index)
            {
                // Check if the row is not the new row
                if (!dataGridView1.Rows[e.RowIndex].IsNewRow)
                {
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["IsEnabled"].Value = true;
            e.Row.Cells["TimePeriod"].Value = "00:00:00";
            e.Row.Cells["WorkingDirectory"].Value = "";
            e.Row.Cells["Command"].Value = "";
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["WorkingDirectory"].Index)
            {
                using (var folderBrowserDialog = new FolderBrowserDialog())
                {
                    // Set the initial directory to the current value of the cell, if it's a valid directory
                    string currentPath = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                    if (!string.IsNullOrEmpty(currentPath) && System.IO.Directory.Exists(currentPath))
                    {
                        folderBrowserDialog.SelectedPath = currentPath;
                    }

                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = folderBrowserDialog.SelectedPath;
                    }
                }
            }
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settings.Commands.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                var isEnabledCell = row.Cells["IsEnabled"].Value;
                var timePeriodCell = row.Cells["TimePeriod"].Value;
                var workingDirectoryCell = row.Cells["WorkingDirectory"].Value;
                var commandCell = row.Cells["Command"].Value;

                if (timePeriodCell == null || workingDirectoryCell == null || commandCell == null)
                {
                    MessageBox.Show("Please fill all cells.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true; // Prevent form from closing
                    return;
                }

                if (TimeSpan.TryParse(timePeriodCell.ToString(), out var timePeriod))
                {
                    _settings.Commands.Add(new CommandConfig
                    {
                        IsEnabled = Convert.ToBoolean(isEnabledCell),
                        TimePeriod = timePeriod,
                        WorkingDirectory = workingDirectoryCell.ToString(),
                        Command = commandCell.ToString()
                    });
                }
                else
                {
                    MessageBox.Show("Invalid TimeSpan format. Please use hh:mm:ss.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true; // Prevent form from closing
                    return;
                }
            }
            _settings.Save();
        }
    }
}
