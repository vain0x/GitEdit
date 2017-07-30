using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GitEdit.Mvvm;
using GitEdit.Properties;
using GitEdit.UI.Editors;

namespace GitEdit.UI
{
    public class MainWindowViewModel
        : ViewModelBase
    {
        public ICommand CompleteCommand { get; }
        public ICommand AbortCommand { get; }

        IMainWindow View { get; }

        ITextEditor Editor =>
            View.Editor;

        Rect _rect = Settings.Default.MainWindowRect;
        public Rect Rect
        {
            get { return _rect; }
            set { SetProperty(ref _rect, value); }
        }

        public string FontFamily =>
            Settings.Default.FontFamily;

        public int FontSize =>
            Settings.Default.FontSize;

        public string Title
        {
            get
            {
                var currentFileName = Editor.Document?.FileName;
                var fileName =
                    string.IsNullOrEmpty(currentFileName)
                    ? "untitled"
                    : Path.GetFileName(currentFileName);
                var indicator =
                    Editor.IsOriginal ? "" : " *";
                return string.Format("{0}{1} | {2}", fileName, indicator, App.Name);
            }
        }

        public string SyntaxName =>
            Editor.SyntaxHighlighting
            .ApplyTo(h => h == null ? "Plain text" : h.Name);

        public string EncodingName =>
            Editor.Encoding
            .ApplyTo(e => e == null ? "No encoding" : e.EncodingName);

        public void OpenFile(FileInfo file)
        {
            Editor.LoadFile(file);
        }

        void SetEncoding(EncodingType encodingType)
        {
            switch (encodingType)
            {
                case EncodingType.Utf8:
                    Editor.Encoding = new UTF8Encoding();
                    break;
                case EncodingType.Default:
                default:
                    break;
            }
        }

        public Result Save()
        {
            var currentFileName = Editor.Document?.FileName;
            var fileInfoOrNull =
                string.IsNullOrEmpty(currentFileName)
                ? View.GetSaveFileOrNull()
                : new FileInfo(currentFileName);
            if (fileInfoOrNull == null) return Result.Failure;

            Editor.SaveFile(fileInfoOrNull);
            return Result.Success;
        }

        public void SaveQuit(EncodingType encodingType)
        {
            SetEncoding(encodingType);
            if (Save() != Result.Success) return;
            View.Quit();
        }

        public void ClearQuit()
        {
            Editor.Document.Text = "";
            SaveQuit(EncodingType.Default);
        }

        public MainWindowViewModel(IMainWindow view)
        {
            View = view;
            CompleteCommand =
                new DelegateCommand<string>(parameter =>
                    SaveQuit(
                        (EncodingType)Enum.Parse(typeof(EncodingType), parameter)
                    ));
            AbortCommand = new DelegateCommand<object>(_ => ClearQuit());

            Editor.ModificationIndicatorChanged +=
                (sender, e) => NotifyPropertyChanged(nameof(Title));
            Editor.Document.FileNameChanged +=
                (sender, e) => NotifyPropertyChanged(nameof(Title));
            Editor.EncodingChanged +=
                (sender, e) => NotifyPropertyChanged(nameof(EncodingName));
            Editor.SyntaxHighlightingChanged +=
                (sender, e) => NotifyPropertyChanged(nameof(SyntaxName));

            // Open the given file
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var file = new FileInfo(args[1]);
                if (file.Exists) { OpenFile(file); }
            }
        }
    }

    public interface IMainWindow
    {
        ITextEditor Editor { get; }
        FileInfo GetSaveFileOrNull();
        void Quit();
    }
}
