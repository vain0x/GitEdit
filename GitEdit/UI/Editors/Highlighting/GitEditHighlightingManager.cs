using System;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace GitEdit.UI.Editors
{
    public class GitEditHighlightingManager
    {
        #region SyntaxDefinitions
        public const string CommitMessageSyntaxName = "CommitMessage";
        public const string GitIgnoreSyntaxName = "GitIgnore";
        public const string IniSyntaxName = "Ini";

        public static Tuple<string, string[]>[] SyntaxDefinitions =>
            new[]
            {
                Tuple.Create(CommitMessageSyntaxName, new string[] { }),
                Tuple.Create(GitIgnoreSyntaxName, new[] { ".gitignore" }),
                Tuple.Create(IniSyntaxName, new[] { ".ini", ".cfg", ".gitconfig" }),
            };
        #endregion

        /// <summary>
        /// Returns the most appropreate syntax definition for given file; or null.
        /// </summary>
        public IHighlightingDefinition TryDetectSyntaxHighlighting(FileInfo file)
        {
            switch (file.Name)
            {
                case "COMMIT_EDITMSG":
                case "MERGE_MSG":
                    return HighlightingManager.Instance.GetDefinition(CommitMessageSyntaxName);
                case "config":
                    return HighlightingManager.Instance.GetDefinition(IniSyntaxName);
                case "exclude":
                    return HighlightingManager.Instance.GetDefinition(GitIgnoreSyntaxName);
                default:
                    return HighlightingManager.Instance.GetDefinitionByExtension(file.Extension);
            }
        }

        IHighlightingDefinition LoadEmbeddedDefinition(string path)
        {
            using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Embedded resource not found");
                }
                using (var reader = new XmlTextReader(stream))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        public void RegisterSyntaxHighlightings()
        {
            foreach (var def in SyntaxDefinitions)
            {
                var name = def.Item1;
                var path = string.Format("GitEdit.Resources.SyntaxHighlighting.{0}.xshd", name);

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
