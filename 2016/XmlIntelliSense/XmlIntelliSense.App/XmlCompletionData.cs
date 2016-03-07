using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace XmlIntelliSense.App
{
    public class XmlCompletionData : ICompletionData
    {
        private readonly bool _isAttribute;

        public XmlCompletionData(string text, string description, bool isAttribute)
        {
            _isAttribute = isAttribute;
            this.Text = text;
            this.Description = description;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        public object Content
        {
            get { return this.Text + " (" + Description + ")"; }
        }

        public object Description { get; private set; }

        public double Priority { get; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            if (_isAttribute)
            {
                textArea.Document.Replace(completionSegment, this.Text + "=\"\"");
                textArea.Caret.Offset = textArea.Caret.Offset - 1;
            }
            else
            {
                string element = this.Text + "></" + this.Text + ">";
                textArea.Document.Replace(completionSegment, element);
                textArea.Caret.Offset = textArea.Caret.Offset - (1 + this.Text.Length + 2);

            }


        }
    }
}