using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OfficeAppWithDotNetCore.Web.Areas.OfficeAddin.Controllers
{
    [Area("OfficeAddin")]
    public class HomeController : Controller
    {
        public IActionResult TaskPane()
        {
            return View();
        }

        public IActionResult FunctionFile()
        {
            return View();
        }
    }
}