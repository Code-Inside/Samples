using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace XmlAttributes
{
    public class BlogComment
    {
       
        public string _name = string.Empty;
        public string _content = string.Empty;
        public List<BlogComment> _comments = new List<BlogComment>();

        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
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

        public BlogComment()
        { }
        public BlogComment(string name, string content)
        {
            _name = name;
            _content = content;
        }
    }
}
