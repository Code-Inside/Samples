using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using ICSharpCode.AvalonEdit.CodeCompletion;
using XmlIntelliSense.App.SharpDevelopXmlEditor;
using XmlIntelliSense.App.XSemmel;

namespace XmlIntelliSense.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public XDocument LastValidXDocument { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            textEditor.Text = File.ReadAllText("Sample.xml");
            XSD.Text = File.ReadAllText("Schema.xsd");


            textEditor.TextChanged += IntelliSense;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextArea.Caret.PositionChanged += IntelliSense;
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            var _editor = textEditor;

            try
            {
                var lastXDoc = XDocument.Parse(_editor.Text);
                LastValidXDocument = lastXDoc;
            }
            catch (Exception exc)
            {
                // can happen...
            }

            try
            {

                switch (e.Text)
                {
                    case ">":
                        {
                            //auto-insert closing element
                            int offset = _editor.CaretOffset;
                            string s = XParser.GetElementAtCursor(_editor.Text, offset - 1);
                            if (!string.IsNullOrWhiteSpace(s) && "!--" != s)
                            {
                                if (!XParser.IsClosingElement(_editor.Text, offset - 1, s))
                                {
                                    string endElement = "</" + s + ">";
                                    var rightOfCursor = _editor.Text.Substring(offset, Math.Max(0, Math.Min(endElement.Length + 50, _editor.Text.Length) - offset - 1)).TrimStart();
                                    if (!rightOfCursor.StartsWith(endElement))
                                    {
                                        _editor.TextArea.Document.Insert(offset, endElement);
                                        _editor.CaretOffset = offset;
                                    }
                                }
                            }
                            break;
                        }
                    case "/":
                        {
                            //insert name of closing element
                            int offset = _editor.CaretOffset;
                            if (offset > 1 && _editor.Text[offset - 2] == '<')
                            {
                                //expand to closing tag
                                string s = XParser.GetParentElementAtCursor(_editor.Text, offset - 1);
                                if (!string.IsNullOrEmpty(s))
                                {
                                    //   showCompletion(new List<ICompletionData>
                                    //   {
                                    //       new MyCompletionData(s + ">")
                                    //});
                                    //  return true;
                                }
                            }
                            if (_editor.Text.Length > offset + 2 && _editor.Text[offset] == '>')
                            {
                                //remove closing tag if exist
                                string s = XParser.GetElementAtCursor(_editor.Text, offset - 1);
                                if (!string.IsNullOrWhiteSpace(s))
                                {
                                    //search closing end tag. Element must be empty (whitespace allowed)  
                                    //"<hallo>  </hallo>" --> enter '/' --> "<hallo/>  "
                                    string expectedEndTag = "</" + s + ">";
                                    for (int i = offset + 1; i < _editor.Text.Length - expectedEndTag.Length + 1; i++)
                                    {
                                        if (!char.IsWhiteSpace(_editor.Text[i]))
                                        {
                                            if (_editor.Text.Substring(i, expectedEndTag.Length) == expectedEndTag)
                                            {
                                                //remove already existing endTag
                                                _editor.Document.Remove(i, expectedEndTag.Length);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "<":
                        var test = XmlParser.GetParentElementPath(_editor.Text);
                        var autocompletelist = ProvidePossibleElementsAutocomplete(test);

                        var completionWindow = new CompletionWindow(textEditor.TextArea);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        foreach (var autocompleteelement in autocompletelist)
                        {
                            data.Add(new XmlElementCompletionData(autocompleteelement.Item1, autocompleteelement.Item2));
                        }
                        completionWindow.Show();
                        completionWindow.Closed += delegate
                        {
                            completionWindow = null;
                        };

                        break;

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

        public List<Tuple<string, string>> ProvidePossibleElementsAutocomplete(XmlElementPath path)
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(this.XSD.Text)));
            LastValidXDocument.Validate(schemas, (o, e) =>
            {
                this.CurrentPath.Text = e.Message.ToString();
            }, true);

            StringBuilder xpath = new StringBuilder();
            foreach (var element in path.Elements)
            {
                xpath.Append("//" + element.Name);
            }

            var parentElement = LastValidXDocument.XPathSelectElement(xpath.ToString());

            if (parentElement != null)
            {
                var element = parentElement.GetSchemaInfo();

                // Get the complex type of the Customer element.
                XmlSchemaComplexType complexType = element.SchemaType as XmlSchemaComplexType;

                // Get the sequence particle of the complex type.
                XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;


                // Iterate over each XmlSchemaElement in the Items collection.
                foreach (XmlSchemaObject childElement in sequence.Items)
                {
                    var asElement = (XmlSchemaElement)childElement;

                    string description;

                    if (asElement.ElementSchemaType.Datatype == null)
                    {
                        description = asElement.ElementSchemaType.TypeCode.ToString();
                    }
                    else
                    {
                        description = asElement.ElementSchemaType.TypeCode.ToString();
                    }

                    result.Add(new Tuple<string,string>(asElement.Name, description));
                }

            }
            return result;
        }

        private void IntelliSense(object sender, EventArgs eventArgs)
        {
            //var GetActiveElementStartPath = XmlParser.GetActiveElementStartPath(textEditor.Text, textEditor.TextArea.Caret.Offset);
            //var GetActiveElementStartPathAtIndex = XmlParser.GetActiveElementStartPathAtIndex(textEditor.Text, textEditor.TextArea.Caret.Offset);
            //var GetAttributeName = XmlParser.GetAttributeName(textEditor.Text, textEditor.TextArea.Caret.Offset);
            //var GetAttributeNameAtIndex = XmlParser.GetAttributeNameAtIndex(textEditor.Text, textEditor.TextArea.Caret.Offset);
            //var GetParentElementPath = XmlParser.GetParentElementPath(textEditor.Text);

            //StringBuilder builder = new StringBuilder();
            //builder.AppendLine("GetActiveElementStartPath: " + GetActiveElementStartPath.ToString());
            //builder.AppendLine("GetActiveElementStartPathAtIndex: " + GetActiveElementStartPathAtIndex.ToString());
            //builder.AppendLine("GetAttributeName: " + GetAttributeName.ToString());
            //builder.AppendLine("GetAttributeNameAtIndex: " + GetAttributeNameAtIndex.ToString());
            //builder.AppendLine("GetParentElementPath: " + GetParentElementPath.ToString());

            //this.CurrentPath.Text = builder.ToString();
        }
    }
}
