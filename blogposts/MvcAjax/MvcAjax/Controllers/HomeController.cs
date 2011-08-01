using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAjax.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActionLink()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult jQuery()
        {
            return View();
        }
    }
}
