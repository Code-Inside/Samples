using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Html2Canvas.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult Upload()
        {
            string fullString = this.Request.Form["img"];
            var base64 = fullString.Substring(fullString.IndexOf(",") + 1);
            byte[] b;
            b = Convert.FromBase64String(base64);

            MemoryStream ms = new System.IO.MemoryStream(b);

            Image img = System.Drawing.Image.FromStream(ms);

            // Since this is a console app, save file so I can see if it works.

            img.Save(Path.Combine(
              AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString() + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);

            return RedirectToAction("Index");
        }
    }
}
