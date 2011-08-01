using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace MvcSessionless.Controllers
{
    [ControllerSessionState(SessionStateBehavior.Default)]
    public class SessionController : Controller
    {
        //
        // GET: /Session/

        public ActionResult Index()
        {
            Thread.Sleep(1000);
            ViewModel.Session = this.ControllerContext.HttpContext.Session.SessionID;
            return View(ViewModel);
        }

    }
}
