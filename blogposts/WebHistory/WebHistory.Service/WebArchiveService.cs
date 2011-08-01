using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebHistory.Model;
using WebHistory.Data;

namespace WebHistory.Service
{
    public class WebArchiveService : IArchiveService
    {
        private IWebsiteRepository _webRep;

        public WebArchiveService(IWebsiteRepository rep)
        {
            this._webRep = rep;
        }

        public WebArchiveService()
        {
            this._webRep = new ArchiveWebsiteRepository();
        }

        public Website Load(string url)
        {
            if (url.StartsWith("http://"))
            {
                return this._webRep.GetWebsite(url);
            }
            else
            {
                throw new ArgumentException("URL must start with http://...");
            }
        }

    }
}
