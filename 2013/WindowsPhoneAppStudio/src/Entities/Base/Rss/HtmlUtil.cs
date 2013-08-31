using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WPAppStudio.Entities.Base
{
    public class HtmlUtil
    {
        /// <summary>
        /// Clean html tags from string
        /// </summary>
        /// <param name="html">Html to be cleaned</param>
        /// <returns></returns>
        public static string CleanHtml(string html)
        {
            string result = null;
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                doc.DocumentNode.Descendants()
                    .Where(n => n.Name == "script" || n.Name == "style" || n.Name == "#comment")
                    .ToList()
                    .ForEach(n => n.Remove());

                result = doc.DocumentNode.InnerText;
                result = result.Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error cleaning html: {0}. Error: {1}", html, ex);
            }

            return result ?? html;
        }

        /// <summary>
        /// Truncates a string containing HTML to a number of text characters, keeping whole words.
        /// The result contains HTML and any tags left open are closed.
        /// </summary>
        /// <param name="html">Html to be truncated</param>
        /// <param name="maxCharacters">Maximum number of characters</param>
        /// <param name="trailingText">Text to append to truncated text</param>
        /// <returns>Truncated string</returns>
        public static string TruncateHtml(string html, int maxCharacters, string trailingText)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            // find the spot to truncate
            // count the text characters and ignore tags
            var textCount = 0;
            var charCount = 0;
            var ignore = false;
            foreach (char c in html)
            {
                charCount++;
                if (c == '<')
                    ignore = true;
                else if (!ignore)
                    textCount++;

                if (c == '>')
                    ignore = false;

                // stop once we hit the limit
                if (textCount >= maxCharacters)
                    break;
            }

            // Truncate the html and keep whole words only
            var trunc = new StringBuilder(TruncateWords(html, charCount));

            // keep track of open tags and close any tags left open
            var tags = new Stack<string>();
            var matches = Regex.Matches(trunc.ToString(),
                @"<((?<tag>[^\s/>]+)|/(?<closeTag>[^\s>]+)).*?(?<selfClose>/)?\s*>",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var tag = match.Groups["tag"].Value;
                    var closeTag = match.Groups["closeTag"].Value;

                    // push to stack if open tag and ignore it if it is self-closing, i.e. <br />
                    if (!string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(match.Groups["selfClose"].Value))
                        tags.Push(tag);

                    // pop from stack if close tag
                    else if (!string.IsNullOrEmpty(closeTag))
                    {
                        // pop the tag to close it.. find the matching opening tag
                        // ignore any unclosed tags
                        while (tags.Pop() != closeTag && tags.Count > 0)
                        { }
                    }
                }
            }

            if (html.Length > charCount)
                // add the trailing text
                trunc.Append(trailingText);

            // pop the rest off the stack to close remainder of tags
            while (tags.Count > 0)
            {
                trunc.Append("</");
                trunc.Append(tags.Pop());
                trunc.Append('>');
            }

            return trunc.ToString();
        }

        /// <summary>
        /// Truncates text and discars any partial words left at the end
        /// </summary>
        /// <param name="text">String to be truncated</param>
        /// <param name="maxCharacters">Maximum number of characters</param>
        /// <param name="trailingText">String to append to truncated text</param>
        /// <returns>Truncated string</returns>
        public static string TruncateWords(string text, int maxCharacters, string trailingText=null)
        {
            if (string.IsNullOrEmpty(text) || maxCharacters <= 0 || text.Length <= maxCharacters)
                return text;

            // trunctate the text, then remove the partial word at the end
            return Regex.Replace(Truncate(text, maxCharacters),
                @"\s+[^\s]+$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled) + trailingText;
        }

        /// <summary>
        /// Truncates text to a number of characters and adds trailing text, i.e. elipses, to the end
        /// </summary>
        /// <param name="text">String to be truncated</param>
        /// <param name="maxCharacters">Maximum number of characters</param>
        /// <param name="trailingText">String to append to truncated text</param>
        /// <returns>Truncated string</returns>
        public static string Truncate(string text, int maxCharacters, string trailingText = null)
        {
            if (string.IsNullOrEmpty(text) || maxCharacters <= 0 || text.Length <= maxCharacters)
                return text;
            else
                return text.Substring(0, maxCharacters) + trailingText;
        }
    }
}

