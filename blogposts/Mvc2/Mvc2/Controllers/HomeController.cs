using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc2.Controllers
{
    public class HomeController : Controller
    {
        public void Index()
        {
            string thiasd = this.ToString();
            RenderView("Index");
        }

        public void About()
        {
            RenderView("About");
        }
    }
}
