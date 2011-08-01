using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace MvcAjax.Controllers
{
    public class AjaxController : Controller
    {

        public ActionResult ItemData()
        {
            return View();
        }

        public ActionResult ItemEdit(string input)
        {
            ViewData["Item"] = input;
            return View();
        }

    }
}
