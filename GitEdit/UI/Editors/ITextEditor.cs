using System;
using System.IO;
using System.Text;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace GitEdit.UI.Editors
{
    public interface ITextEditor
        : ITextEditorComponent
    {
        event EventHandler FileNameChanged;

        void Clear();
        void SaveFile(FileInfo file);
        void LoadFile(FileInfo file);
    }
}
