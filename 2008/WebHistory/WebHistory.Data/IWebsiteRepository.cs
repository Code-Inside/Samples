using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebHistory.Model;

namespace WebHistory.Data
{
    public interface IWebsiteRepository
    {
        Website GetWebsite(string url);
    }
}
