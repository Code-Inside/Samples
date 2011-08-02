using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcDropDownListHelper.Models;

namespace MvcDropDownListHelper.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new FooViewModel());
        }

        [HttpPost]
        public ActionResult Index(FooViewModel model)
        {
            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
