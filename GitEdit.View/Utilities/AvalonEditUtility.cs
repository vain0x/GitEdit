using System;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace GitEdit.View
{
    public static class AvalonEditUtility
    {
        public static void RegisterSyntaxHighlightDefinition(string name, string[] extensions)
        {
            var path = string.Format("GitEdit.View.Resources.SyntaxHighlighting.{0}.xshd", name);
            using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream(path))
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
        }

        /// <summary>
        /// Returns the most appropreate syntax definition for given file; or null.
        /// </summary>
        public static IHighlightingDefinition TryDetectSyntaxHighlighting(FileInfo file)
        {
            switch (file.Name)
            {
                case "COMMIT_EDITMSG":
                    return HighlightingManager.Instance.GetDefinition(Constant.CommitMessageSyntaxName);
                default:
                    return HighlightingManager.Instance.GetDefinitionByExtension(file.Extension);
            }
        }
    }
}
