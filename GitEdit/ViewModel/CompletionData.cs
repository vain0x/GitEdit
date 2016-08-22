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
        public object Content
        {
            get { return Text; }
            set { throw new NotSupportedException(); }
        }

        public object Description { get; set; }
        public ImageSource Image { get; set; }
        public double Priority { get; set; }
        public string Text { get; set; }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}
