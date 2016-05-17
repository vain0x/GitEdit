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

        public static string[] SyntaxNames =
            new string[]
            {
                CommitMessageSyntaxName,
            };
    }
}
