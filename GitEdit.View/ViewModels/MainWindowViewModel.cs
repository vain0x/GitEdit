using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
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
                var file = new FileInfo(args[1]);
                if (file.Exists) { OpenFile(file); }
            }
        }

        private IMainWindow _view;

        private MyTextEditor Editor
        {
            get { return _view.Editor; }
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

        public void OpenFile(FileInfo file)
        {
            Editor.LoadFile(file);
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
    }

    public interface IMainWindow
    {
        MyTextEditor Editor { get; }
        void Save();
        void Quit();
    }
}
