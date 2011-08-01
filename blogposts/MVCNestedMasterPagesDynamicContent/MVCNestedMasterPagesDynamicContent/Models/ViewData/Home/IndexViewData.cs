using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCNestedMasterPagesDynamicContent.Models.ViewData.Home
{
    public class IndexViewData : ViewDataBase
    {
        public IndexViewData(SiteMasterViewData siteMaster)
        {
            base.SiteMasterViewData = siteMaster;
        }

        public string Text { get; set; }
    }
}
