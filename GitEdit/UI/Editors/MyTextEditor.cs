using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace GitEdit.UI.Editors
{
    public sealed class MyTextEditor
        : TextEditor
        , ITextEditor
    {
        public event EventHandler FileNameChanged;

        GitEditHighlightingManager HighlightingManager { get; }
        CodeCompletion CodeCompletion { get; }

        void OnFileLoaded(FileInfo file)
        {
            var syntax = HighlightingManager.TryDetectSyntaxHighlighting(file);
            if (syntax != null) { SyntaxHighlighting = syntax; }

            CodeCompletion.RecollectCompletionWords();
        }

        public void LoadFile(FileInfo file)
        {
            Load(file.FullName);

            Document.FileName = file.FullName;
            FileNameChanged?.Invoke(this, EventArgs.Empty);

            OnFileLoaded(file);
        }

        public void SaveFile(FileInfo file)
        {
            using (var stream = file.OpenWrite())
            {
                stream.SetLength(0);
                Save(stream);
            }

            Document.FileName = file.FullName;
            FileNameChanged?.Invoke(this, EventArgs.Empty);
        }

        public MyTextEditor()
        {
            HighlightingManager = new GitEditHighlightingManager();
            CodeCompletion = new CodeCompletion(this);
            Encoding = new UTF8Encoding();
        }
    }
}
