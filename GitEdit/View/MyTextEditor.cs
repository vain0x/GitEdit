using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using ICSharpCode.AvalonEdit;
using GitEdit.Model;
using GitEdit.ViewModel;
using GitEdit.View.Editor;

namespace GitEdit.View
{
    public class MyTextEditor
        : TextEditor
        , ITextEditor
    {
        GitEditHighlightingManager HighlightingManager { get; }
        CodeCompletion CodeCompletion { get; }

        public event EventHandler ModificationIndicatorChanged;
        public event EventHandler SyntaxHighlightingChanged;
        public event EventHandler EncodingChanged;
        public event EventHandler<FileInfo> FileLoaded;

        public void ListenPropertyChanged(DependencyProperty dp, Action<EventArgs> raise)
        {
            DependencyPropertyDescriptor
                .FromProperty(dp, typeof(TextEditor))
                .AddValueChanged(this, (sender, e) => raise(e));
        }

        public MyTextEditor()
        {
            HighlightingManager = new GitEditHighlightingManager();
            CodeCompletion = new CodeCompletion(this);
            Encoding = new UTF8Encoding();

            ListenPropertyChanged(
                IsModifiedProperty,
                e => ModificationIndicatorChanged?.Invoke(this, e)
            );
            ListenPropertyChanged(
                SyntaxHighlightingProperty,
                e => SyntaxHighlightingChanged?.Invoke(this, e)
            );
            ListenPropertyChanged(
                EncodingProperty,
                e => EncodingChanged?.Invoke(this, e)
            );

            FileLoaded += (sender, file) =>
            {
                var syntax = HighlightingManager.TryDetectSyntaxHighlighting(file);
                if (syntax != null) { SyntaxHighlighting = syntax; }

                CodeCompletion.RecollectCompletionWords();
            };
        }

        bool ITextEditor.IsOriginal =>
            Document.UndoStack.IsOriginalFile;

        public void LoadFile(FileInfo file)
        {
            Load(file.FullName);
            Document.FileName = file.FullName;
            FileLoaded?.Invoke(this, file);
        }

        public void SaveFile(FileInfo file)
        {
            file.UpdateSafely(tempFile =>
            {
                using (var stream = tempFile.OpenWrite())
                {
                    Save(stream);
                }
            });
            Document.FileName = file.FullName;
        }
    }
}
