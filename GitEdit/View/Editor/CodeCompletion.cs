using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using GitEdit.ViewModel;

namespace GitEdit.View.Editor
{
    public class CodeCompletion
    {
        TextEditor Editor { get; }

        #region Collect completion words
        List<CompletionData> CompletionItems { get; } =
            new List<CompletionData>();

        static Regex CompletionWordRegex { get; } =
            new Regex(@"[a-zA-Z_]\w{4,}");

        void CollectCompletionWords(string text)
        {
            var items =
                CompletionWordRegex.Matches(text)
                .Cast<Match>()
                .Select(m => m.Value)
                .Distinct()
                .Select(word => new CompletionData() { Text = word });

            CompletionItems.AddRange(items);
        }

        public void RecollectCompletionWords()
        {
            var text = Editor.Text;
            Task.Run(() =>
            {
                CompletionItems.Clear();
                CollectCompletionWords(text);
            });
        }
        #endregion

        #region Completion window
        CompletionWindow CurrentCompletionWindowOrNull { get; set; }

        void AddCompletionItemsTo(CompletionWindow completionWindow)
        {
            foreach (var item in CompletionItems)
            {
                completionWindow.CompletionList.CompletionData.Add(item);
            }
        }

        void OnCompletionWindowClosed(object sender, EventArgs e)
        {
            var completionWindow = CurrentCompletionWindowOrNull;
            if (completionWindow == null) return;
            CurrentCompletionWindowOrNull = null;
            completionWindow.Closed -= OnCompletionWindowClosed;
        }

        void OpenCompletionWindow()
        {
            if (CurrentCompletionWindowOrNull != null) return;
            var completionWindow = new CompletionWindow(Editor.TextArea);
            CurrentCompletionWindowOrNull = completionWindow;
            completionWindow.Closed += OnCompletionWindowClosed;
            AddCompletionItemsTo(completionWindow);
            completionWindow.Show();
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
