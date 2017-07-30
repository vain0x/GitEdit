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
    public sealed class MainWindowViewModel
        : BindableBase
    {
        public ICommand CompleteCommand { get; }
        public ICommand AbortCommand { get; }

        IMainWindow View { get; }

        public TextEditorViewModel Editor { get; } =
            new TextEditorViewModel();

        public string FontFamily =>
            Settings.Default.FontFamily;

        public int FontSize =>
            Settings.Default.FontSize;

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

        public bool TrySave()
        {
            var currentFileName = Editor.Document?.FileName;
            var fileInfoOrNull =
                string.IsNullOrEmpty(currentFileName)
                ? View.GetSaveFileOrNull()
                : new FileInfo(currentFileName);
            if (fileInfoOrNull == null) return false;

            Editor.SaveFile(fileInfoOrNull);
            return true;
        }

        public void SaveQuit(EncodingType encodingType)
        {
            SetEncoding(encodingType);
            if (!TrySave()) return;
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
        }
    }

    public interface IMainWindow
    {
        FileInfo GetSaveFileOrNull();
        void Quit();
    }
}
