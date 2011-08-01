using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MVCNestedMasterPagesDynamicContent.Models.ViewData;
using MVCNestedMasterPagesDynamicContent.Models.ViewData.Home;
using MVCNestedMasterPagesDynamicContent.Filters.ViewData;

namespace MVCNestedMasterPagesDynamicContent.Controllers
{
    [HandleError]
    [AddSiteMasterViewData]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewDataBase masterData = (ViewDataBase)this.ControllerContext.Controller.TempData["ViewData"];

            IndexViewData viewData = new IndexViewData(masterData.SiteMasterViewData);
            viewData.Text = "Welcome to ASP.NET MVC!";

            return View(viewData);
        }

        public ActionResult About()
        {
            ViewDataBase masterData = (ViewDataBase)this.ControllerContext.Controller.TempData["ViewData"];

            AboutViewData viewData = new AboutViewData(masterData.SiteMasterViewData);
            viewData.Text = "About Page";

            return View(viewData);
        }
    }
}
