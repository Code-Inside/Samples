using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using MVCLocalize.Code;//for Controller.Resource extension

namespace MVCLocalize.Controllers
{
	[HandleError]
    [LocalizedFilter]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
		    
			//fron Controller we can only acces global resources, since we dont have a path to a View...
            ViewData["Message"] = "From Controller -> " + Resources.Test.Foobar;

			return View();
		}

		public ActionResult About()
		{
            ViewData["Title"] = LanguageHelper.GetLanguage(); ;

			return View();
		}
	}
}
