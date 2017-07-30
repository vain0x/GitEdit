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
        public event EventHandler QuitRequested;

        public ICommand CompleteCommand { get; }
        public ICommand AbortCommand { get; }

        ISaveFileChooser SaveFileChooser { get; }

        public TextEditorViewModel Editor { get; } =
            new TextEditorViewModel();

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
                case EncodingType.Current:
                default:
                    break;
            }
        }

        public bool TrySave()
        {
            var currentFileName = Editor.Document?.FileName;
            var fileInfoOrNull =
                string.IsNullOrEmpty(currentFileName)
                ? SaveFileChooser.GetSaveFileOrNull()
                : new FileInfo(currentFileName);
            if (fileInfoOrNull == null) return false;

            Editor.SaveFile(fileInfoOrNull);
            return true;
        }

        public void SaveQuit(EncodingType encodingType)
        {
            SetEncoding(encodingType);
            if (!TrySave()) return;
            QuitRequested?.Invoke(this, EventArgs.Empty);
        }

        public void ClearQuit()
        {
            Editor.Document.Text = "";
            SaveQuit(EncodingType.Current);
        }

        public MainWindowViewModel(ISaveFileChooser saveFileChooser)
        {
            SaveFileChooser = saveFileChooser;

            CompleteCommand = new DelegateCommand<EncodingType>(SaveQuit);
            AbortCommand = new DelegateCommand<object>(_ => ClearQuit());
        }
    }
}
