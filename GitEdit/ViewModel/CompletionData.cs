using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace GitEdit.ViewModel
{
    public class CompletionData
        : ICompletionData
    {
        public object Content => Text;
        public object Description => null;
        public ImageSource Image => null;
        public double Priority => 0.0;
        public string Text { get; }

        public CompletionData(string text)
        {
            Text = text;
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}
