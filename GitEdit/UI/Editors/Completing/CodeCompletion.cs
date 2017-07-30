using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;

namespace GitEdit.UI.Editors
{
    public sealed class CodeCompletion
    {
        TextEditor Editor { get; }

        #region Collect completion words
        Task<CompletionData[]> completionItemsTask =
            Task.FromResult(new CompletionData[0]);

        static Regex CompletionWordRegex { get; } =
            new Regex(@"[a-zA-Z_-][\w-]{4,}");

        CompletionData[] CollectCompletionWords(string text)
        {
            return
                CompletionWordRegex.Matches(text)
                .Cast<Match>()
                .Select(m => m.Value)
                .Distinct()
                .Select(word => new CompletionData(word))
                .ToArray();
        }

        public void RecollectCompletionWords()
        {
            var text = Editor.Text;
            completionItemsTask = Task.Run(() => CollectCompletionWords(text));
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
            foreach (var item in completionItemsTask.Result)
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

        void ShowCompletionWindow(CompletionWindow completionWindow)
        {
            if (CurrentCompletionWindowOrNull != null) return;
            CurrentCompletionWindowOrNull = completionWindow;
            completionWindow.Closed += OnCompletionWindowClosed;
            completionWindow.Show();
        }

        CompletionWindow TryNewCompletionWindow()
        {
            if (CurrentCompletionWindowOrNull != null) return null;

            var completionWindow = new CompletionWindow(Editor.TextArea);
            AddCompletionItemsTo(completionWindow);

            var segment = WordSegmentUnderCaret();
            var word = Editor.Document.GetText(segment);
            completionWindow.StartOffset = segment.StartOffset;
            completionWindow.EndOffset = segment.EndOffset;
            completionWindow.CompletionList.SelectItem(word);

            return completionWindow;
        }

        void ShowSuggestions()
        {
            var completionWindow = TryNewCompletionWindow();
            if (completionWindow == null) return;

            var suggestionCount = completionWindow.CompletionList.ListBox.Items.Count;
            if (suggestionCount > 0)
            {
                ShowCompletionWindow(completionWindow);
            }
            else
            {
                completionWindow.Close();
            }
        }

        void TryComplete()
        {
            var completionWindow = TryNewCompletionWindow();
            if (completionWindow == null) return;

            var suggestionCount = completionWindow.CompletionList.ListBox.Items.Count;
            if (suggestionCount == 1)
            {
                completionWindow.CompletionList.RequestInsertion(EventArgs.Empty);
                completionWindow.Close();
            }
            else
            {
                ShowCompletionWindow(completionWindow);
            }
        }
        #endregion

        #region Editor evet handlers
        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.Space)
            {
                TryComplete();
                e.Handled = true;
            }
        }

        void OnDocumentUpdateFinished(object sender, EventArgs e)
        {
            var wordSegment = WordSegmentUnderCaret();
            if (wordSegment.Length == 0)
            {
                CurrentCompletionWindowOrNull?.Close();
            }
            else if (wordSegment.Length == 3)
            {
                ShowSuggestions();
            }
        }

        void AttachEvents()
        {
            Editor.KeyDown += OnKeyDown;

            Editor.DocumentChanged += (sender, e) =>
            {
                var document = Editor.Document;
                if (document != null)
                {
                    document.UpdateFinished += OnDocumentUpdateFinished;
                }
            };
        }
        #endregion

        public CodeCompletion(TextEditor editor)
        {
            Editor = editor;

            AttachEvents();
        }
    }
}
