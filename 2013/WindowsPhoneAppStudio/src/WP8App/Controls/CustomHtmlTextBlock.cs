using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WPAppStudio.Helpers;

namespace WPAppStudio.Controls
{
    public class CustomHtmlTextBlock : Control
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof (string),
                typeof (CustomHtmlTextBlock),
                new PropertyMetadata("CustomHtmlTextBlock", OnTextPropertyChanged));

        private TextBlock _measureBlock;
        private StackPanel _stackPanel;

        public CustomHtmlTextBlock()
        {
            // Get the style from generic.xaml
            DefaultStyleKey = typeof (CustomHtmlTextBlock);
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (CustomHtmlTextBlock) d;
            var value = (string) e.NewValue;
            source.ParseText(value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _stackPanel = GetTemplateChild("StackPanel") as StackPanel;
            ParseText(Text);
        }
        
        private void ParseTextHide(string value)
        {
            if (_stackPanel == null)
            {
                return;
            }
            // Clear previous TextBlocks
            _stackPanel.Children.Clear();
            // Calculate max char count
            int maxTexCount = GetMaxTextSize();

            if (maxTexCount == 0)
                maxTexCount = value.Length;

            if (value.Length < maxTexCount)
            {
                TextBlock textBlock = GetTextBlock();
                textBlock.Text = value;
                _stackPanel.Children.Add(textBlock);
            }
            else
            {
                int n = value.Length/maxTexCount;
                int start = 0;

                // Add textblocks
                for (int i = 0; i < n; i++)
                {
                    TextBlock textBlock = GetTextBlock();
                    textBlock.Text = value.Substring(start, maxTexCount);
                    _stackPanel.Children.Add(textBlock);
                    start = maxTexCount;
                }

                // Pickup the leftover text
                if (value.Length%maxTexCount > 0)
                {
                    TextBlock textBlock = GetTextBlock();
                    textBlock.Text = value.Substring(maxTexCount*n, value.Length - maxTexCount*n);
                    _stackPanel.Children.Add(textBlock);
                }
            }
        }

        private void ParseText(string value)
        {
            var reader = new StringReader(value);

            if (_stackPanel == null)
            {
                return;
            }
            // Clear previous TextBlocks
            _stackPanel.Children.Clear();
            // Calculate max char count
            int maxTexCount = GetMaxTextSize();

            if (maxTexCount == 0)
                maxTexCount = value.Length;

            if (value.Length < maxTexCount)
            {
                TextBlock textBlock = GetTextBlock();
                textBlock.Text = value;
                _stackPanel.Children.Add(textBlock);
            }
            else
            {
                while (reader.Peek() > 0)
                {
                    string line = reader.ReadLine();
                    ParseLine(line);
                }
            }
        }

        private void ParseLine(string line)
        {
            int lineCount = 0;
            int maxLineCount = GetMaxLineCount();
            string tempLine = line;
            var sbLine = new StringBuilder();

            while (lineCount < maxLineCount)
            {
                int charactersFitted = MeasureString(tempLine, (int) Width);
                string leftSide = tempLine.Substring(0, charactersFitted);
                sbLine.Append(leftSide);
                tempLine = tempLine.Substring(charactersFitted, tempLine.Length - (charactersFitted));
                lineCount++;
            }

            TextBlock textBlock = GetTextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Text = sbLine.ToString();

            var wb = new WebBrowser {Height = 10*(textBlock.ActualWidth/320)};
            wb.NavigateToString(WebBrowserHelper.WrapHtml(line, Application.Current.RootVisual.RenderSize.Width));

            _stackPanel.Children.Add(wb);

            if (tempLine.Length > 0)
                ParseLine(tempLine);
        }

        private int MeasureString(string text, int desWidth)
        {
            int charactersFitted;

            var sb = new StringBuilder();

            //get original size
            Size size = MeasureString(text);

            if (size.Width > desWidth)
            {
                string[] words = text.Split(' ');
                sb.Append(words[0]);

                for (int i = 1; i < words.Length; i++)
                {
                    sb.Append(" " + words[i]);
                    int nWidth = (int) MeasureString(sb.ToString()).Width;

                    if (nWidth > desWidth)
                    {
                        sb.Remove(sb.Length - words[i].Length, words[i].Length);
                        break;
                    }
                }

                charactersFitted = sb.Length;
            }
            else
            {
                charactersFitted = text.Length;
            }

            return charactersFitted;
        }

        private Size MeasureString(string text)
        {
            if (_measureBlock == null)
            {
                _measureBlock = GetTextBlock();
            }

            _measureBlock.Text = text;
            return new Size(_measureBlock.ActualWidth, _measureBlock.ActualHeight);
        }

        private int GetMaxTextSize()
        {
            // Get average char size
            Size size = MeasureText(" ");
            // Get number of char that fit in the line
            var charLineCount = (int) (Width/size.Width);
            // Get line count
            var lineCount = (int) (2048/size.Height);

            return charLineCount*lineCount/2;
        }

        private int GetMaxLineCount()
        {
            Size size = MeasureText(" ");
            // Get line count
            int lineCount = (int) (2048/size.Height) - 5;

            return lineCount;
        }

        private TextBlock GetTextBlock()
        {
            var textBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = FontSize,
                    FontFamily = FontFamily,
                    FontStyle = FontStyle,
                    FontWeight = FontWeight,
                    Foreground = Foreground,
                    Margin = new Thickness(0, 0, MeasureText(" ").Width, 0)
                };
            return textBlock;
        }

        private Size MeasureText(string value)
        {
            TextBlock textBlock = GetTextBlockOnly();
            textBlock.Text = value;
            return new Size(textBlock.ActualWidth, textBlock.ActualHeight);
        }

        private TextBlock GetTextBlockOnly()
        {
            var textBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = FontSize,
                    FontFamily = FontFamily,
                    FontWeight = FontWeight
                };
            return textBlock;
        }
    }
}