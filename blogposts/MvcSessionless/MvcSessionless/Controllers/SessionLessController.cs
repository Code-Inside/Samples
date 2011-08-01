using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace MvcSessionless.Controllers
{
    [ControllerSessionState(SessionStateBehavior.Disabled)]
    public class SessionLessController : Controller
    {

        //
        // GET: /SessionLess/

        public ActionResult Index()
        {
            Thread.Sleep(1000);
            ViewModel.Session = this.ControllerContext.HttpContext.Session;
            return View(ViewModel);
        }

    }
}
