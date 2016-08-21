using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using GitEdit.Model;
using GitEdit.ViewModel;

namespace GitEdit.View
{
    public class MyTextEditor
        : TextEditor
        , ITextEditor
    {
        public MyTextEditor()
        {
            DependencyPropertyDescriptor
                .FromProperty(IsModifiedProperty, typeof(TextEditor))
                .AddValueChanged(this, (sender, e) => ModificationIndicatorChanged(this, EventArgs.Empty));
        }

        public event EventHandler ModificationIndicatorChanged;

        #region Syntax highlighting
        public static void RegisterSyntaxHighlightDefinition(string name, Stream stream, string[] extensions)
        {
            if (stream == null) { throw new InvalidOperationException("Embedded resource not found"); }

            using (var reader = new XmlTextReader(stream))
            {
                HighlightingManager.Instance.RegisterHighlighting(
                    name,
                    extensions,
                    HighlightingLoader.Load(reader, HighlightingManager.Instance)
                );
            }
        }

        /// <summary>
        /// Returns the most appropreate syntax definition for given file; or null.
        /// </summary>
        static IHighlightingDefinition TryDetectSyntaxHighlighting(FileInfo file)
        {
            switch (file.Name)
            {
                case "COMMIT_EDITMSG":
                    return HighlightingManager.Instance.GetDefinition(Constant.CommitMessageSyntaxName);
                default:
                    return HighlightingManager.Instance.GetDefinitionByExtension(file.Extension);
            }
        }
        #endregion

        public void LoadFile(FileInfo file)
        {
            base.Load(file.FullName);
            Document.FileName = file.FullName;

            var syntax = TryDetectSyntaxHighlighting(file);
            if (syntax != null) { SyntaxHighlighting = syntax; }
        }

        public void SaveFile(FileInfo file)
        {
            file.UpdateSafely(tempFile =>
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
