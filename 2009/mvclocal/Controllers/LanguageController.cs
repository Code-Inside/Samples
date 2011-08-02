using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MVCLocalize.Code;

namespace MVCLocalize.Controllers
{
    [LocalizedFilter]
    public class LanguageController : Controller
    {

        public ActionResult Switch(string language)
        {
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            LanguageHelper.SetLanguage(language);

            if(this.ControllerContext.HttpContext.Request.UrlReferrer != null)
            {
                return Redirect(this.ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
