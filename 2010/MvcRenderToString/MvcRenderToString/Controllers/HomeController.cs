using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcRenderToString.Results;
using MvcRenderToString.Helper;
using MvcRenderToString.Models;
namespace MvcRenderToString.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ExcelResult Excel()
        {
            List<ExcelData> foobars = new List<ExcelData>();
            foobars.Add(new ExcelData() { Foobar = "HelloWorld!"});
            foobars.Add(new ExcelData() { Foobar = "HelloWorld!" });
            foobars.Add(new ExcelData() { Foobar = "HelloWorld!" });
            foobars.Add(new ExcelData() { Foobar = "HelloWorld!" });

            string content = this.RenderViewToString("Excel", foobars);
            return new ExcelResult("Foobar.xls", content);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
