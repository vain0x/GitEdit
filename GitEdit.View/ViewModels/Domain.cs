using System;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace GitEdit.View
{
    public static class Constant
    {
        public static string AppName = "GitEdit";

        public static string CommitMessageSyntaxName = "CommitMessage";
        public static string IniSyntaxName = "Ini";

        public static Tuple<string, string[]>[] SyntaxDefinitions =
            new[]
            {
                Tuple.Create(CommitMessageSyntaxName, new string[] { }),
                Tuple.Create(IniSyntaxName, new string[] { ".ini", ".cfg", ".gitconfig" }),
            };
    }
}
