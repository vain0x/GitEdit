using System;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using GitEdit.ViewModel;

namespace GitEdit.View
{
    public class GitEditHighlightingManager
    {
        /// <summary>
        /// Returns the most appropreate syntax definition for given file; or null.
        /// </summary>
        public IHighlightingDefinition TryDetectSyntaxHighlighting(FileInfo file)
        {
            switch (file.Name)
            {
                case "COMMIT_EDITMSG":
                    return HighlightingManager.Instance.GetDefinition(Constant.CommitMessageSyntaxName);
                default:
                    return HighlightingManager.Instance.GetDefinitionByExtension(file.Extension);
            }
        }

        IHighlightingDefinition LoadEmbeddedDefinition(string path)
        {
            using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream(path))
            using (var reader = new XmlTextReader(stream))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Embedded resource not found");
                }
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        public void RegisterSyntaxHighlightings()
        {
            foreach (var def in Constant.SyntaxDefinitions)
            {
                var name = def.Item1;
                var path = string.Format("GitEdit.Resource.SyntaxHighlighting.{0}.xshd", name);

                HighlightingManager.Instance.RegisterHighlighting(
                    name,
                    def.Item2,
                    () => LoadEmbeddedDefinition(path)
                );
            }
        }

        public GitEditHighlightingManager()
        {
            RegisterSyntaxHighlightings();
        }
    }
}
