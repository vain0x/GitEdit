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
        void RegisterSyntaxHighlightDefinition(string name, Stream stream, string[] extensions)
        {
            if (stream == null)
            {
                throw new InvalidOperationException("Embedded resource not found");
            }

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

        public void RegisterSyntaxHighlightings()
        {
            foreach (var def in Constant.SyntaxDefinitions)
            {
                var name = def.Item1;
                var path = string.Format("GitEdit.Resource.SyntaxHighlighting.{0}.xshd", name);

                using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream(path))
                {
                    RegisterSyntaxHighlightDefinition(name, stream, def.Item2);
                }
            }
        }

        public GitEditHighlightingManager()
        {
            RegisterSyntaxHighlightings();
        }
    }
}
