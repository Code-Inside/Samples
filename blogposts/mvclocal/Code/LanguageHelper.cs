using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace MVCLocalize.Code
{
    public static class LanguageHelper
    {
        private const string LanguageKey = "language";

        public static void SetLanguage(string language)
        {
            if (HasElement<string>(LanguageKey))
            {
                HttpContext.Current.Session[LanguageKey] = language;
            }
            else
            {
                AddElement<string>(LanguageKey, language);
            }
        }

        public static string GetLanguage()
        {
            if (HasElement<string>(LanguageKey))
            {
                return (string)HttpContext.Current.Session[LanguageKey];
            }
            else
            {
                string language = English;

                if (HttpContext.Current.Request.UserLanguages != null)
                {
                    language = HttpContext.Current.Request.UserLanguages.First();
                }

                if (language.ToLower().StartsWith(German))
                {
                    return German;
                }
                else
                {
                    return English;
                }
            }
        }

        public static string German
        {
            get
            {
                return "de";
            }
        }

        public static string English
        {
            get
            {
                return "en";
            }
        }

        /// <summary>
        /// Adds an element
        /// </summary>
        /// <param name="user"></param>
        private static void AddElement<T>(string key, T data)
        {
            HttpContext.Current.Session[key] = data;
        }

        /// <summary>
        /// Gets an element
        /// </summary>
        /// <param name="user"></param>
        private static bool HasElement<T>(string key)
        {
            return HttpContext.Current.Session[key] is T;
        }
    }
}
