using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace CommandScheduler
{
    public class SchedulerApplicationContext : ApplicationContext
    {
        private NotifyIcon _notifyIcon;
        private OptionsForm _optionsForm;
        private Settings _settings;
        private List<Timer> _timers = new List<Timer>();

        public SchedulerApplicationContext()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Options", ShowOptions),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };

            LoadSettingsAndStartTimers();
        }

        private void LoadSettingsAndStartTimers()
        {
            _settings = Settings.Load();
            StopTimers();
            foreach (var command in _settings.Commands)
            {
                var timer = new Timer(command.TimePeriod.TotalMilliseconds);
                timer.Elapsed += (sender, args) => ExecuteCommand(command);
                timer.Start();
                _timers.Add(timer);
            }
        }

        private void ExecuteCommand(CommandConfig command)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command.Command}",
                        WorkingDirectory = command.WorkingDirectory,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                };
                process.Start();
            }
            catch (Exception ex)
            {
                // Log the exception
            }
        }

        private void StopTimers()
        {
            foreach (var timer in _timers)
            {
                timer.Stop();
                timer.Dispose();
            }
            _timers.Clear();
        }

        private void ShowOptions(object sender, EventArgs e)
        {
            if (_optionsForm == null || _optionsForm.IsDisposed)
            {
                _optionsForm = new OptionsForm();
                _optionsForm.FormClosed += (o, args) => LoadSettingsAndStartTimers();
            }
            _optionsForm.Show();
        }

        private void Exit(object sender, EventArgs e)
        {
            StopTimers();
            _notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
