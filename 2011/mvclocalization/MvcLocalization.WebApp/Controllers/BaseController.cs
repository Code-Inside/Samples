using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MvcLocalization.WebApp.Infrastructure;

namespace MvcLocalization.WebApp.Controllers
{
    public class BaseController : Controller
    {
        protected override void ExecuteCore()
        {
            CurrentLanguageStore store = new CurrentLanguageStore();
            LanguageKey key = store.GetPreferredLanguage();
            CultureInfo language = new CultureInfo(key.ToString());

            Thread.CurrentThread.CurrentCulture = language;
            Thread.CurrentThread.CurrentUICulture = language;
            base.ExecuteCore();
        }

    }
}
