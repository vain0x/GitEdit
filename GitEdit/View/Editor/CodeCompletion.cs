using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
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

        #region Document
        bool IsIdentifierChar(char @char)
        {
            return @char == '_' || char.IsLetterOrDigit(@char);
        }

        TextSegment WordSegmentUnderCaret()
        {
            var textArea = Editor.TextArea;
            var document = Editor.Document;

            var caretOffset = textArea.Caret.Offset;
            var location = document.GetLocation(caretOffset);
            var lineOffset = document.GetOffset(location.Line, 0);
            var line = document.GetLineByNumber(location.Line);
            var lineText = document.GetText(lineOffset, line.TotalLength);

            var wordOffsetBegin = caretOffset - lineOffset;
            while (0 < wordOffsetBegin && IsIdentifierChar(lineText[wordOffsetBegin - 1]))
            {
                wordOffsetBegin--;
            }

            var wordOffsetEnd = caretOffset - lineOffset;
            while (wordOffsetEnd < lineText.Length && IsIdentifierChar(lineText[wordOffsetEnd]))
            {
                wordOffsetEnd++;
            }

            return
                new TextSegment()
                {
                    StartOffset = lineOffset + wordOffsetBegin,
                    EndOffset = lineOffset + wordOffsetEnd
                };
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

            var segment = WordSegmentUnderCaret();
            var word = Editor.Document.GetText(segment);
            completionWindow.StartOffset = segment.StartOffset;
            completionWindow.EndOffset = segment.EndOffset;
            completionWindow.CompletionList.SelectItem(word);

            if (completionWindow.CompletionList.ListBox.Items.Count == 1)
            {
                completionWindow.CompletionList.RequestInsertion(EventArgs.Empty);
                completionWindow.Close();
                return;
            }

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
