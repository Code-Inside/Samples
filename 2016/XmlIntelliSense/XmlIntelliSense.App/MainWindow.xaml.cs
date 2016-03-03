using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XmlIntelliSense.App.SharpDevelopXmlEditor;

namespace XmlIntelliSense.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            textEditor.TextChanged += IntelliSense;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextArea.Caret.PositionChanged += IntelliSense;
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            try
            {
                switch (e.Text)
                {
                    case ">":
                    {
                        //auto-insert closing element
                        int offset = textEditor.CaretOffset;
                        var test =  XmlParser.GetActiveElementStartPath(textEditor.Text, offset - 1);
                        var s = test.Elements.GetLast().Name;
                        if (!string.IsNullOrWhiteSpace(s) && "!--" != s)
                        {
                            //if (!XParser.IsClosingElement(_editor.Text, offset - 1, s))
                            //{
                                string endElement = "</" + s + ">";
                                var rightOfCursor =
                                    textEditor.Text.Substring(offset,
                                        Math.Max(0, Math.Min(endElement.Length + 50, textEditor.Text.Length) - offset - 1))
                                        .TrimStart();
                                if (!rightOfCursor.StartsWith(endElement))
                                {
                                    textEditor.TextArea.Document.Insert(offset, endElement);
                                    textEditor.CaretOffset = offset;
                                }
                            //}
                        }
                        break;
                    }
                    case "/":
                        {
                            //insert name of closing element
                            int offset = textEditor.CaretOffset;
                            if (offset > 1 && textEditor.Text[offset - 2] == '<')
                            {
                                //expand to closing tag
                                string s = XmlParser.GetActiveElementStartPath(textEditor.Text, offset - 1).Elements.GetLast().Name;
                                //if (!string.IsNullOrEmpty(s))
                                //{
                                //    showCompletion(new List<ICompletionData>
                                //    {
                                //        new MyCompletionData(s + ">")
                                //    });
                                //    return true;
                                //}
                            }
                            if (textEditor.Text.Length > offset + 2 && textEditor.Text[offset] == '>')
                            {
                                //remove closing tag if exist
                                string s = XmlParser.GetActiveElementStartPath(textEditor.Text, offset - 1).Elements.GetLast().Name;
                                if (!string.IsNullOrWhiteSpace(s))
                                {
                                    //search closing end tag. Element must be empty (whitespace allowed)  
                                    //"<hallo>  </hallo>" --> enter '/' --> "<hallo/>  "
                                    string expectedEndTag = "</" + s + ">";
                                    for (int i = offset + 1; i < textEditor.Text.Length - expectedEndTag.Length + 1; i++)
                                    {
                                        if (!char.IsWhiteSpace(textEditor.Text[i]))
                                        {
                                            if (textEditor.Text.Substring(i, expectedEndTag.Length) == expectedEndTag)
                                            {
                                                //remove already existing endTag
                                                textEditor.Document.Remove(i, expectedEndTag.Length);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception exc)
            {
                
            }

            if (e.Text.Length > 0)
            {
                char c = e.Text[0];
                if (!(char.IsLetterOrDigit(c) || char.IsPunctuation(c)))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    //_completionWindow.CompletionList.RequestInsertion(e);
                    e.Handled = true;
                }
            }
        }

        private void IntelliSense(object sender, EventArgs eventArgs)
        {
            var GetActiveElementStartPath = XmlParser.GetActiveElementStartPath(textEditor.Text, textEditor.TextArea.Caret.Offset);
            var GetActiveElementStartPathAtIndex = XmlParser.GetActiveElementStartPathAtIndex(textEditor.Text, textEditor.TextArea.Caret.Offset);
            var GetAttributeName = XmlParser.GetAttributeName(textEditor.Text, textEditor.TextArea.Caret.Offset);
            var GetAttributeNameAtIndex = XmlParser.GetAttributeNameAtIndex(textEditor.Text, textEditor.TextArea.Caret.Offset);
            var GetParentElementPath = XmlParser.GetParentElementPath(textEditor.Text);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("GetActiveElementStartPath: " + GetActiveElementStartPath.ToString());
            builder.AppendLine("GetActiveElementStartPathAtIndex: " + GetActiveElementStartPathAtIndex.ToString());
            builder.AppendLine("GetAttributeName: " + GetAttributeName.ToString());
            builder.AppendLine("GetAttributeNameAtIndex: " + GetAttributeNameAtIndex.ToString());
            builder.AppendLine("GetParentElementPath: " + GetParentElementPath.ToString());

            this.CurrentPath.Text = builder.ToString();
        }
    }
}
