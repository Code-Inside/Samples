using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcLocalization.WebApp.Infrastructure;

namespace MvcLocalization.WebApp.Controllers
{
    public class LanguageController : Controller
    {
        private CurrentLanguageStore _languageStore;

        public LanguageController()
        {
            this._languageStore = new CurrentLanguageStore();
        }

        public RedirectToRouteResult SwitchLanguage(LanguageKey key)
        {
            this._languageStore.SetPreferredLanguage(key);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LanguageBox()
        {
            LanguageKey languageNow = this._languageStore.GetPreferredLanguage();

            if (languageNow == LanguageKey.De) ViewBag.AvailableLanguage = LanguageKey.En;
            else ViewBag.AvailableLanguage = LanguageKey.De;

            return View();
        }

    }
}
