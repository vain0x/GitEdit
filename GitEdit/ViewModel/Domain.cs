using System;

namespace GitEdit.ViewModel
{
    public static class Constant
    {
        public static string AppName => "GitEdit";

        public static string CommitMessageSyntaxName => "CommitMessage";
        public static string GitIgnoreSyntaxName => "GitIgnore";
        public static string IniSyntaxName => "Ini";

        public static Tuple<string, string[]>[] SyntaxDefinitions =>
            new[]
            {
                Tuple.Create(CommitMessageSyntaxName, new string[] { }),
                Tuple.Create(GitIgnoreSyntaxName, new string[] { ".gitignore" }),
                Tuple.Create(IniSyntaxName, new string[] { ".ini", ".cfg", ".gitconfig" }),
            };
    }
}
