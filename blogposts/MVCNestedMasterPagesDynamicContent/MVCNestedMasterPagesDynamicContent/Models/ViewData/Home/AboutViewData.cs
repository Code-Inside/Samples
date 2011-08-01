using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCNestedMasterPagesDynamicContent.Models.ViewData.Home
{
    public class AboutViewData : ViewDataBase
    {
        public AboutViewData(SiteMasterViewData siteMaster)
        {
            base.SiteMasterViewData = siteMaster;
        }

        public string Text { get; set; }
    }
}
