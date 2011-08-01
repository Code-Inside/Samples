using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadRss
{
    public class RssItem
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public DateTime PublishedOn { get; set; }
    }
}
