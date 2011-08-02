using System.Collections.Generic;
using System.Web;

namespace MvcLocalization.WebApp.Infrastructure
{
    public class CurrentLanguageStore
    {
        /// <summary>
        /// Cookie Repository for getting and setting cookie values.
        /// </summary>
        private ICookieRepository _cookieRep;

        /// <summary>
        /// HttpContext for accessing the HttpRequests UserLanguages
        /// </summary>
        private HttpContextBase _context;

        public CurrentLanguageStore()
        {
            this._cookieRep = new HttpCookieRepository();
            this._context = new HttpContextWrapper(HttpContext.Current);
        }

        /// <summary>
        /// Default ctor for a new instance.
        /// </summary>
        /// <param name="baseContext">HttpBaseContext for accessing the HttpRequest.</param>
        /// <param name="cookieRepository">Implementation of ICookieRepository.</param>
        public CurrentLanguageStore(HttpContextBase baseContext, ICookieRepository cookieRepository)
        {
            _cookieRep = cookieRepository;
            _context = baseContext;
        }

        /// <summary>
        /// Gets the preferred language.
        /// </summary>
        /// <returns></returns>
        public LanguageKey GetPreferredLanguage()
        {
            string[] browserLanguages = this._context.Request.UserLanguages;

            if (this._cookieRep.HasElement(CookieKey.UserLanguage))
            {
                string cookieResult = this._cookieRep.GetElement(CookieKey.UserLanguage);

                if (string.IsNullOrWhiteSpace(cookieResult))
                    return LanguageKey.En;

                if (cookieResult.ToLower() == LanguageKey.De.ToString().ToLower())
                    return LanguageKey.De;

                return LanguageKey.En;
            }

            if (browserLanguages == null)
                return LanguageKey.En;

            foreach (var language in browserLanguages)
            {
                if (language.StartsWith(LanguageKey.De.ToString().ToLower()))
                    return LanguageKey.De;
                else if (language.StartsWith(LanguageKey.En.ToString().ToLower()))
                    return LanguageKey.En;
            }

            return LanguageKey.En;

        }

        public void SetPreferredLanguage(LanguageKey key)
        {
            if (this._cookieRep.HasElement(CookieKey.UserLanguage))
            {
                this._cookieRep.UpdateElement(CookieKey.UserLanguage, key.ToString());
            }
            else
            {
                this._cookieRep.AddElement(CookieKey.UserLanguage, key.ToString());
            }
        }
    }
}