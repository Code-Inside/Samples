using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace XmlAttributes
{
    [XmlRootAttribute("BlogEntry")]
    public class BlogEntry
    {
        private string _title = string.Empty;
        private string _content = string.Empty;
        private List<BlogComment> _comments = new List<BlogComment>();

        [XmlElement("title")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [XmlElement("content")]     
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        [XmlElement("comments")]
        public List<BlogComment> Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        public BlogEntry()
        { }
    }
}
