using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace GitEdit.View.Model
{
    public class Wait
        : IDisposable
    {
        public Wait(TimeSpan timeout, EventHandler<string> wakeUpEvent)
        {
            if (WaitFile.Exists)
            {
                Application.Current.Shutdown();
            }
            else
            {
                WakeUp += wakeUpEvent;

                // Watch wake file
                using (var stream = WaitFile.Create()) {}
                _fileWatcher = new FileSystemWatcher()
                {
                    Path = WaitFile.DirectoryName,
                    Filter = WaitFile.Name,
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };

                _fileWatcher.Changed += (sender, e) =>
                {
                    WakeUp(this, WaitFile.ReadText());
                    _fileWatcher.EnableRaisingEvents = false;
                };

                // Begin timeout timer
                if (timeout.TotalMilliseconds > 0)
                {
                    _timer = new DispatcherTimer()
                    {
                        Interval = timeout
                    };
                    _timer.Tick += (sender, e) =>
                    {
                        Application.Current.Shutdown();
                    };
                }
            }
        }

        public void Dispose()
        {
            WaitFile.DeleteIfExists();
        }

        public static FinishWatcher TryWakeUp(string text, Func<FileSystemEventHandler> f)
        {
            if (WaitFile.Exists)
            {
                var watcher = new FinishWatcher(f());
                WaitFile.WriteText(text);
                return watcher;
            }
            return null;
        }

        private static readonly FileInfo WaitFile = new FileInfo(".wait");
        private FileSystemWatcher _fileWatcher;
        private DispatcherTimer _timer;

        public event EventHandler<string> WakeUp;

        public class FinishWatcher
        {
            public FinishWatcher(FileSystemEventHandler onDeleted)
            {
                Debug.Assert(WaitFile.Exists);
                _fileWatcher = new FileSystemWatcher()
                {
                    Path = WaitFile.DirectoryName,
                    Filter = WaitFile.Name,
                    EnableRaisingEvents = true
                };
                _fileWatcher.Deleted += onDeleted;
            }

            private FileSystemWatcher _fileWatcher;
        }
    }
}
