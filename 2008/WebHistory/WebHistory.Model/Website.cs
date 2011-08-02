using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebHistory.Model
{
    public class Website
    {
        public string Url { get; set; }
        public List<ArchiveWebsite> ArchiveWebsites { get; set; }

        public Website()
        {
            this.ArchiveWebsites = new List<ArchiveWebsite>();
        }
    }
}
