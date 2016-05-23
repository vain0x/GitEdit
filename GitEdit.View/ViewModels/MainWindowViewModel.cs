using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace GitEdit.View.Model
{
    public class MainWindowViewModel
        : ViewModelBase
    {
        public MainWindowViewModel(IMainWindow view)
        {
            _view = view;

            Editor.ModificationIndicatorChanged +=
                (sender, e) => NotifyPropertyChanged("Title");
            Editor.Document.FileNameChanged +=
                (sender, e) => NotifyPropertyChanged("Title");

            // Open the given file
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (args[1] == "--wait")
                {
                    Wait();
                }
                else
                {
                    var path = args[1];
                    if (! TryWakeUp(path))
                    {
                        var file = new FileInfo(path);
                        if (file.Exists) { OpenFile(file); }
                    }
                }
            }
        }

        private IMainWindow _view;

        private MyTextEditor Editor
        {
            get { return _view.Editor; }
        }

        private Rect _rect = Properties.Settings.Default.MainWindowRect;
        public Rect Rect
        {
            get { return _rect; }
            set { SetProperty(ref _rect, value); }
        }

        public string FontFamily
        {
            get { return Properties.Settings.Default.FontFamily; }
        }

        public int FontSize
        {
            get { return Properties.Settings.Default.FontSize; }
        }

        public string Title
        {
            get
            {
                var doc = Editor.Document;
                if (doc == null) return Constant.AppName;
                var fileName =
                    string.IsNullOrEmpty(doc.FileName)
                    ? "untitled"
                    : Path.GetFileName(doc.FileName);
                var indicator =
                    _view.Editor.IsModified ? " *" : "";
                return string.Format("{0}{1} | {2}", fileName, indicator, Constant.AppName);
            }
        }

        public string SyntaxName
        {
            get { return Editor.SyntaxHighlighting.RefBind(x => x.Name); }
        }

        public string EncodingName
        {
            get { return Editor.Encoding.RefBind(x => x.EncodingName); }
        }

        private void UpdateStatusBar()
        {
            foreach (var name in new string[] { "SyntaxName", "EncodingName" })
            {
                NotifyPropertyChanged(name);
            }
        }

        public void OpenFile(FileInfo file)
        {
            Editor.LoadFile(file);
            UpdateStatusBar();
        }

        public void SaveFile(FileInfo file)
        {
            Editor.SaveFile(file);
        }

        private RelayCommand _saveQuitCommand;
        public ICommand SaveQuitCommand
        {
            get { return _saveQuitCommand ?? (_saveQuitCommand = new RelayCommand(_ => SaveQuit())); }
        }

        public void SaveQuit()
        {
            _view.Save();
            _view.Quit();
        }

        private RelayCommand _clearQuitCommand;
        public ICommand ClearQuitCommand
        {
            get { return _clearQuitCommand ?? (_clearQuitCommand = new RelayCommand(_ => ClearQuit())); }
        }

        public void ClearQuit()
        {
            Editor.Document.Text = "";
            SaveQuit();
        }

        public void OnClosed()
        {
            if (_wait == null)
            {
                App.Start("--wait");
            }
            else
            {
                _wait.Dispose();
            }
        }

        /// <summary>
        /// 待機モードを開始する。
        /// </summary>
        private void Wait()
        {
            _wait = new Wait(
                timeout: new TimeSpan(1, 0, 0),
                wakeUpEvent: (sender, text) =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        OpenFile(new FileInfo(text));
                        _view.Show();
                    });
                });
            _view.Hide();
        }

        /// <summary>
        /// 待機中のプロセスがあれば、それを起こす。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool TryWakeUp(string path)
        {
            _waitWatcher = Model.Wait.TryWakeUp(
                path,
                () => (sender, e) => Application.Current.Shutdown()
                );
            if (_waitWatcher == null)
            {
                return false;
            }
            else
            {
                _view.Hide();
                return true;
            }
        }

        private Wait _wait;
        private Wait.FinishWatcher _waitWatcher;
    }

    public interface IMainWindow
    {
        MyTextEditor Editor { get; }
        Dispatcher Dispatcher { get; }
        void Show();
        void Hide();
        void Save();
        void Quit();
    }
}
