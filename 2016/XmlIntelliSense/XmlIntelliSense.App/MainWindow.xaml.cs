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
            textEditor.TextArea.Caret.PositionChanged += IntelliSense;
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
