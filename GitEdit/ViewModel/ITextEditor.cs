using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace GitEdit.ViewModel
{
    public interface ITextEditor
        : ITextEditorComponent
    {
        /// <summary>
        /// Gets whether the document has no modification.
        /// </summary>
        bool IsOriginal { get; }

        event EventHandler ModificationIndicatorChanged;
        event EventHandler SyntaxHighlightingChanged;
        event EventHandler EncodingChanged;

        IHighlightingDefinition SyntaxHighlighting { get; }
        Encoding Encoding { get; }

        void SaveFile(FileInfo file);
        void LoadFile(FileInfo file);
    }
}
