using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using GitEdit.ViewModel;

namespace GitEdit.View.Editor
{
    public class CodeCompletion
    {
        TextEditor Editor { get; }

        CompletionWindow CurrentCompletionWindowOrNull { get; set; }

        #region Collect completion words
        List<CompletionData> CompletionItems { get; } =
            new List<CompletionData>();

        IEnumerable<string> EnumerateCompletionWords(string text)
        {
            var regex = new Regex(@"[a-zA-Z_]\w{4,}");
            var index = 0;
            while (true)
            {
                var match = regex.Match(text, index);
                if (!match.Success) break;
                yield return match.Value;
                index = match.Index + match.Length;
            }
        }

        void CollectCompletionWords()
        {
            var items =
                EnumerateCompletionWords(Editor.Text)
                .Distinct()
                .Select(word => new CompletionData() { Text = word });

            CompletionItems.AddRange(items);
        }

        public void RecollectCompletionWords()
        {
            CompletionItems.Clear();
            CollectCompletionWords();
        }
        #endregion

        #region Completion items
        void AddCompletionItemsToList()
        {
            var completionWindow = CurrentCompletionWindowOrNull;
            if (completionWindow == null) return;
            foreach (var item in CompletionItems)
            {
                completionWindow.CompletionList.CompletionData.Add(item);
            }
        }

        void InitializeCompletionWindow()
        {
            if (CurrentCompletionWindowOrNull != null) return;
            var completionWindow = new CompletionWindow(Editor.TextArea);
            CurrentCompletionWindowOrNull = completionWindow;

            completionWindow.Closed += (sender, e) =>
            {
                CurrentCompletionWindowOrNull = null;
            };

            AddCompletionItemsToList();
        }

        void OpenCompletionWindow()
        {
            if (CurrentCompletionWindowOrNull != null) return;
            InitializeCompletionWindow();
            CurrentCompletionWindowOrNull.Show();
        }
        #endregion

        public CodeCompletion(TextEditor editor)
        {
            Editor = editor;

            Editor.KeyDown += (sender, e) =>
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.Space)
                {
                    OpenCompletionWindow();
                    e.Handled = true;
                }
            };
        }
    }
}
