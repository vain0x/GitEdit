using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;
using GitEdit.Model;
using GitEdit.ViewModel;

namespace GitEdit.View
{
    public class MyTextEditor
        : TextEditor
        , ITextEditor
    {
        public MyTextEditor()
            : base()
        {
            DependencyPropertyDescriptor
                .FromProperty(IsModifiedProperty, typeof(TextEditor))
                .AddValueChanged(this, (sender, e) => ModificationIndicatorChanged(this, EventArgs.Empty));
        }

        public event EventHandler ModificationIndicatorChanged;

        public void LoadFile(FileInfo file)
        {
            base.Load(file.FullName);
            Document.FileName = file.FullName;

            var syntax = AvalonEditUtility.TryDetectSyntaxHighlighting(file);
            if (syntax != null) { SyntaxHighlighting = syntax; }
        }

        public void SaveFile(FileInfo file)
        {
            file.OverwriteSafely(tempFile =>
            {
                using (var stream = tempFile.OpenWrite())
                {
                    base.Save(stream);
                }
            });
            Document.FileName = file.FullName;
        }
    }
}
