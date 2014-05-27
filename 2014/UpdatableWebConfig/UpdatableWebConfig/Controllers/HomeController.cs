using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpdatableWebConfig.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var config = CodeInsideConfig.GetConfig();

            ViewBag.Url = config.WebUrl;
            ViewBag.Id = config.Id;

            ViewBag.NumberOfAuthors = config.Authors.Count;

            return View();
        }

        public ActionResult About()
        {
            var config = CodeInsideConfig.GetWritableBaseConfig();

            var writableConfigSection = config.GetSection("codeInsideConfig") as CodeInsideConfig;
            writableConfigSection.Id = Guid.NewGuid();
            writableConfigSection.Authors.Add(new CodeInsideConfigAuthor() { Name = "Hello World!" + Guid.NewGuid() });

            try
            {
                config.Save();
                return RedirectToAction("Index", "Home");
            }
            catch (ConfigurationErrorsException exc)
            {
                throw;
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}