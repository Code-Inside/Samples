using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XmlIntelliSense.App.XSemmel
{
    public static class XParser
    {

        /// <summary>
        /// Liefert den Namen des Elements beim offset
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string GetElementAtCursor(string xml, int offset)
        {
            if (offset == xml.Length)
            {
                offset--;
            }
            int startIdx = xml.LastIndexOf('<', offset);
            if (startIdx < 0) return null;

            if (startIdx < xml.Length && xml[startIdx + 1] == '/')
            {
                startIdx = startIdx + 1;
            }

            int endIdx1 = xml.IndexOf(' ', startIdx);
            if (endIdx1 == -1 /*|| endIdx1 > offset*/) endIdx1 = int.MaxValue;

            int endIdx2 = xml.IndexOf('>', startIdx);
            if (endIdx2 == -1 /*|| endIdx2 > offset*/)
            {
                endIdx2 = int.MaxValue;
            }
            else
            {
                if (endIdx2 < xml.Length && xml[endIdx2 - 1] == '/')
                {
                    endIdx2 = endIdx2 - 1;
                }
            }

            int endIdx = Math.Min(endIdx1, endIdx2);
            if (endIdx2 > 0 && endIdx2 < int.MaxValue && endIdx > startIdx)
            {
                return xml.Substring(startIdx + 1, endIdx - startIdx - 1);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Liefert true falls das Element beim offset ein schließendes Element ist,
        /// also &lt;/x&gt; oder &lt;x/&gt;
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="offset"></param>
        /// <param name="elementName">optional, elementName = GetElementAtCursor(xml, offset)</param>
        /// <returns></returns>
        public static bool IsClosingElement(string xml, int offset, string elementName = null)
        {
            if (elementName == null)
            {
                elementName = GetElementAtCursor(xml, offset);
            }
            else
            {
                Debug.Assert(GetElementAtCursor(xml, offset) == elementName);
            }

            if (offset >= xml.Length || offset < 0)
            {
                return false;
            }
            int idxOpen = xml.LastIndexOf('<', offset);
            if (idxOpen < 0)
            {
                return false;
            }

            int idxClose = xml.LastIndexOf('>', offset);
            if (idxClose > 0)
            {
                if (idxClose > idxOpen && idxClose < offset - 1)
                {
                    return false;
                }
            }

            string prefix = xml.Substring(idxOpen, offset - idxOpen);
            if (prefix.Contains("/"))
            {
                return true;
            }


            return false;
        }

        public static string GetElementAtCursorFuzzy(string xml, int offset)
        {
            int idx = xml.LastIndexOf('<', offset);
            if (idx == -1) return null;

            string s = xml.Substring(idx + 1, offset - idx - 1);
            s = s.TrimStart();

            int idxEnd = s.IndexOf(' ');
            if (idxEnd > 0)
            {
                s = s.Substring(0, idxEnd);
            }

            return s;
        }

        /// <summary>
        /// Group0 = Whole match
        /// Group1 = Element name
        /// GroupX = Attributes
        /// </summary>
        //private const string regexOpeningElement = @"<[ ]*(\w*)([ ]*\w*=""\w*"")*[ ]*>";
        private const string regexOpeningElement = @"<[ ]*([\w:-]*)([ \r\n\t]*\w*=""[^""]*"")*[ ]*>";

        private static readonly string regexClosingElement = string.Format(regexClosingNamedElement, @"\w:");
        private const string regexClosingNamedElement = @"<[ ]*/[ ]*({0})[ ]*>";


        /// <summary>
        /// Returns the parent element
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="offset"></param>
        /// <returns>null if there is a syntax error, "" if there is no parent</returns>
        public static string GetParentElementAtCursor(string xml, int offset)
        {
            int i = offset;
            if (i > 0 && xml[i - 1] == '<')
            {
                i--;
            }

            while (i > 0)
            {
                Regex r = new Regex(regexOpeningElement, RegexOptions.RightToLeft);
                Match m = r.Match(xml, i);
                if (m.Success)
                {
                    int idx = m.Index;
                    string elementName = m.Groups[1].Value;

                    string regexClosing = string.Format(regexClosingNamedElement, elementName);
                    Regex closing = new Regex(regexClosing);
                    Match m2 = closing.Match(xml, idx);
                    if (m2.Success)
                    {
                        if (m2.Index >= offset)
                        {
                            return elementName;
                        }
                        i = idx;
                        //continue
                    }
                    else
                    {
                        return elementName;
                    }
                }
                else
                {
                    return null;
                }
            }

            return "";
        }

        public static bool IsInsideAttributeValue(string xml, int offset)
        {
            if (IsInsideElementDeclaration(xml, offset))
            {
                int idxOpen = xml.LastIndexOf('<', offset - 1);

                int apostophFound = 0;
                for (int i = idxOpen; i < offset; i++)
                {
                    if (xml[i] == '"') apostophFound++;
                }

                if (apostophFound % 2 == 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool IsInsideAttributeKey(string xml, int offset)
        {
            if (IsInsideElementDeclaration(xml, offset))
            {
                int idxOpen = xml.LastIndexOf('<', offset - 1);

                int apostophFound = 0;
                int spaceFound = 0;
                for (int i = idxOpen; i < offset; i++)
                {
                    if (xml[i] == '"') apostophFound++;
                    if (xml[i] == ' ') spaceFound++;
                }

                if (apostophFound % 2 == 0 && spaceFound > 0)
                {
                    if (xml[offset] == '"')
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            return false;
        }


        public static bool IsInsideEmptyElement(string xml, int offset)
        {
            if (IsInsideElementDeclaration(xml, offset))
            {
                int idxClose = xml.IndexOf('>', offset);
                int idxEmptyClose = xml.IndexOf("/>", offset, StringComparison.InvariantCulture);
                if (idxClose == -1 && idxEmptyClose >= 0) return true;
                if (idxEmptyClose == -1 && idxClose >= 0) return false;
                return idxEmptyClose < idxClose;
            }
            return false;
        }

        public static bool IsInsideElementDeclaration(string xml, int offset)
        {
            if (offset <= 0) return false;
            int idxOpen = xml.LastIndexOf('<', offset - 1);
            int idxClose;
            if (offset >= xml.Length)
            {
                idxClose = -1;
            }
            else
            {
                idxClose = xml.IndexOf('>', offset);
            }

            if (idxOpen == -1 && idxClose == -1) return false;
            if (idxOpen >= 0 && idxClose == -1) return true;

            string subString = xml.Substring(idxOpen + 1, idxClose - idxOpen - 1);
            if (subString.Contains(">"))
            {
                return false;
            }
            if (idxClose > idxOpen) return true;
            return false;
        }

        public static bool IsInsideComment(string text, int offset)
        {
            int lastOpening = text.LastIndexOf("<!--", offset, StringComparison.InvariantCulture);
            int lastClosing = text.LastIndexOf("-->", offset, StringComparison.InvariantCulture);

            if (lastOpening == -1 || lastClosing > lastOpening)
            {
                return false;
            }
            return true;
        }

        //        public static bool IsInsideElementDeclaration(string xml, int offset)
        //        {
        //            string s = GetElementAtCursor(xml, offset);
        //            if (s == null)
        //            {
        //                return false;
        //            }
        //            return true;
        //        }


        public static string Trim(string s)
        {
            if (s == null)
            {
                return null;
            }

            {
                int idxOpen2 = -1;
                int idxOpen = s.IndexOf('<');
                if (idxOpen >= 0)
                {
                    idxOpen2 = s.IndexOf('<', idxOpen + 1);
                }
                int idxClose = s.IndexOf('>');

                if (idxOpen2 < 0 || idxOpen2 > idxClose)
                {
                    if (idxOpen < 0)
                    {
                        throw new Exception("No XML found");
                    }

                    s = s.Substring(idxOpen);
                }
            }

            {
                int idxClose2 = -1;
                int idxClose = s.LastIndexOf('>');
                if (idxClose > 0)
                {
                    idxClose2 = s.LastIndexOf('>', idxClose - 1);
                }
                int idxOpen = s.LastIndexOf('<');

                if (idxOpen < 0)
                {
                    throw new Exception("No XML found");
                }

                if (idxClose >= 0 && (idxClose2 < 0 || idxClose2 < idxOpen))
                {
                    s = s.Substring(0, idxClose + 1);
                }
            }

            return s;
        }

    }
}
