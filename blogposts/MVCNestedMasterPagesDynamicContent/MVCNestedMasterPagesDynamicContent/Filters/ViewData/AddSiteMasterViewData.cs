using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCNestedMasterPagesDynamicContent.Models.ViewData;
using System.Web.Mvc;

namespace MVCNestedMasterPagesDynamicContent.Filters.ViewData
{
    public class AddSiteMasterViewData : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewDataBase data = new ViewDataBase();
            data.SiteMasterViewData = new SiteMasterViewData();
            data.SiteMasterViewData.Title = "Master Dynamisch @ " + DateTime.Now.ToShortDateString();

            // remove existing viewdata
            filterContext.Controller.TempData.Remove("ViewData");

            filterContext.Controller.TempData.Add("ViewData", data);
        }
    }
}
