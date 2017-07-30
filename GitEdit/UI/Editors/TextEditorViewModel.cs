using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitEdit.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace GitEdit.UI.Editors
{
    public sealed class TextEditorViewModel
        : BindableBase
    {
        public event EventHandler<FileInfo> FileLoadRequested;
        public event EventHandler<FileInfo> FileSaveRequested;

        IDocument document;
        public IDocument Document
        {
            get { return document; }
            set
            {
                SetProperty(ref document, value);
                RaisePropertyChanged(nameof(Title));
            }
        }

        bool isModified;
        public bool IsModified
        {
            get { return isModified; }
            set
            {
                SetProperty(ref isModified, value);
                RaisePropertyChanged(nameof(Title));
            }
        }

        public string Title
        {
            get
            {
                var currentFileName = document?.FileName;
                var fileName =
                    string.IsNullOrEmpty(currentFileName)
                    ? "untitled"
                    : Path.GetFileName(currentFileName);
                var indicator =
                    IsModified ? " *" : "";
                return string.Format("{0}{1} | {2}", fileName, indicator, App.Name);
            }
        }

        IHighlightingDefinition syntaxHighlighting;
        public IHighlightingDefinition SyntaxHighlighting
        {
            get { return syntaxHighlighting; }
            set
            {
                SetProperty(ref syntaxHighlighting, value);
                RaisePropertyChanged(nameof(SyntaxName));
            }
        }

        public string SyntaxName =>
            SyntaxHighlighting?.Name ?? "Plain Text";

        Encoding encoding = new UTF8Encoding();
        public Encoding Encoding
        {
            get { return encoding; }
            set
            {
                SetProperty(ref encoding, value);
                RaisePropertyChanged(nameof(EncodingName));
            }
        }

        public string EncodingName =>
            Encoding?.EncodingName ?? "No Encoding";

        public void OnFileNameChanged()
        {
            RaisePropertyChanged(nameof(Title));
        }

        public void LoadFile(FileInfo file)
        {
            FileLoadRequested?.Invoke(this, file);
        }

        public void SaveFile(FileInfo file)
        {
            FileSaveRequested?.Invoke(this, file);
        }

        public TextEditorViewModel()
        {
            document = new TextDocument();
        }
    }
}
