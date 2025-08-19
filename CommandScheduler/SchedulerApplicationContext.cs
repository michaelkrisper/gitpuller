using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
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
        private LogForm _logForm;
        private Settings _settings;
        private List<Timer> _timers = new List<Timer>();
        private static List<LogEntry> _logs = new List<LogEntry>();

        public SchedulerApplicationContext()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = CreateHourglassIcon(),
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Options", ShowOptions),
                    new MenuItem("Log", ShowLog),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };

            LoadSettingsAndStartTimers();
        }

        private Icon CreateHourglassIcon()
        {
            var bitmap = new Bitmap(32, 32);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var pen = new Pen(Color.Black, 2);

                // Hourglass frame
                graphics.DrawLine(pen, 8, 4, 24, 4);
                graphics.DrawLine(pen, 8, 28, 24, 28);
                graphics.DrawLine(pen, 8, 4, 8, 28);
                graphics.DrawLine(pen, 24, 4, 24, 28);

                // Hourglass shape
                Point[] topGlass = { new Point(10, 6), new Point(22, 6), new Point(16, 15) };
                Point[] bottomGlass = { new Point(10, 26), new Point(22, 26), new Point(16, 17) };
                graphics.FillPolygon(Brushes.LightBlue, topGlass);
                graphics.FillPolygon(Brushes.LightSkyBlue, bottomGlass);
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
            var stopwatch = new Stopwatch();
            var output = new StringBuilder();
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
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    }
                };

                process.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
                process.ErrorDataReceived += (sender, args) => output.AppendLine(args.Data);

                stopwatch.Start();
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                output.AppendLine("Exception: " + ex.Message);
            }
            finally
            {
                _logs.Add(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Command = command.Command,
                    Output = output.ToString(),
                    Duration = stopwatch.Elapsed
                });
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

        private void ShowLog(object sender, EventArgs e)
        {
            if (_logForm == null || _logForm.IsDisposed)
            {
                _logForm = new LogForm();
            }
            _logForm.SetLogs(_logs);
            _logForm.Show();
        }

        private void Exit(object sender, EventArgs e)
        {
            StopTimers();
            _notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
