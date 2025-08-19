using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace CommandScheduler
{
    public class SchedulerApplicationContext : ApplicationContext
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private NotifyIcon _notifyIcon;
        private OptionsForm _optionsForm;
        private Settings _settings;
        private List<Timer> _timers = new List<Timer>();

        public SchedulerApplicationContext()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = CreateGearIcon(),
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Options", ShowOptions),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };

            LoadSettingsAndStartTimers();
        }

        private Icon CreateGearIcon()
        {
            var bitmap = new Bitmap(32, 32);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw gear body
                graphics.FillEllipse(Brushes.Gray, 6, 6, 20, 20);
                graphics.FillEllipse(Brushes.White, 10, 10, 12, 12);

                // Draw gear teeth
                for (int i = 0; i < 8; i++)
                {
                    var angle = i * 45;
                    var x1 = 16 + (float)(10 * Math.Cos(angle * Math.PI / 180));
                    var y1 = 16 + (float)(10 * Math.Sin(angle * Math.PI / 180));
                    var x2 = 16 + (float)(14 * Math.Cos(angle * Math.PI / 180));
                    var y2 = 16 + (float)(14 * Math.Sin(angle * Math.PI / 180));
                    graphics.DrawLine(new Pen(Brushes.Gray, 4), x1, y1, x2, y2);
                }
            }

            IntPtr hicon = bitmap.GetHicon();
            Icon icon = (Icon)Icon.FromHandle(hicon).Clone();
            DestroyIcon(hicon);
            return icon;
        }

        private void LoadSettingsAndStartTimers()
        {
            _settings = Settings.Load();
            StopTimers();
            foreach (var command in _settings.Commands)
            {
                if (command.IsEnabled && command.TimePeriod.TotalMilliseconds > 0)
                {
                    var timer = new Timer(command.TimePeriod.TotalMilliseconds);
                    timer.Elapsed += (sender, args) => ExecuteCommand(command);
                    timer.Start();
                    _timers.Add(timer);
                }
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
