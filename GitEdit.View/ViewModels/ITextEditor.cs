using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace GitEdit.View.ViewModel
{
    public interface ITextEditor
        : ITextEditorComponent
    {
        event EventHandler ModificationIndicatorChanged;

        IHighlightingDefinition SyntaxHighlighting { get; }
        Encoding Encoding { get; }

        void SaveFile(FileInfo file);
        void LoadFile(FileInfo file);
    }
}
