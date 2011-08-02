using System;
using System.Linq;
using System.Web;

namespace MvcLocalization.WebApp.Infrastructure
{
    /// <summary>
    /// ICookieRepository Implementation based on HttpCookies
    /// </summary>
    public class HttpCookieRepository : ICookieRepository
    {
        /// <summary>
        /// Cookiename
        /// </summary>
        public static readonly string Name = "TestApp";

        /// <summary>
        /// HttpContext
        /// </summary>
        private HttpContextBase _httpContext;

        /// <summary>
        /// Default Ctor
        /// </summary>
        public HttpCookieRepository()
        {
            this._httpContext = new HttpContextWrapper(HttpContext.Current);
        }

        /// <summary>
        /// Default Ctor
        /// </summary>
        public HttpCookieRepository(HttpContextBase context)
        {
            this._httpContext = context;
        }

        /// <summary>
        /// Adds an element to the cookie. To add several values in an row all elements will be stored in the tmp cookie object at first.
        /// To avoid "duplicating" the cookie, we remove it from the request. Now we append the cookie (containing the old and the new value) to the 
        /// request again.
        /// Because we don´t know when the last element is added, we swap the whole request cookiecollection at the end to the response. 
        /// With this way we can add several items and the other cookievalues in the request don´t change.
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="key">CookieKey for Store</param>
        /// <param name="data">Data to store</param>
        public void AddElement(CookieKey key, string data)
        {
            HttpCookie cookieInRequest = this.IsCookieInRequest();
            if (cookieInRequest == null)
            {
                cookieInRequest = new HttpCookie(HttpCookieRepository.Name);
                this.SetDefaultValues(cookieInRequest);
            }
            cookieInRequest.Values.Add(key.ToString(), data);
            this._httpContext.Request.Cookies.Remove(HttpCookieRepository.Name);
            this._httpContext.Request.Cookies.Add(cookieInRequest);
            this.TransferCookieFromRequestToResponse();
        }

        /// <summary>
        /// Get a specific element of the cookie based on the type and key
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="key">Specific Cookie</param>
        /// <returns>The element from the cookie</returns>
        public string GetElement(CookieKey key)
        {
            HttpCookie cookie = this._httpContext.Request.Cookies.Get(HttpCookieRepository.Name);
            if (cookie == null)
                return null;

            return cookie.Values.Get(key.ToString());
        }

        /// <summary>
        /// Checks if a element is in the cookie based on the key and type
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="key">CookieKey for lookup</param>
        /// <returns>
        /// true if element is found and has the correct type, false if the key doesn´t exists or it has the wrong type
        /// </returns>
        public bool HasElement(CookieKey key)
        {
            if (this._httpContext.Request.Cookies.AllKeys.Contains(HttpCookieRepository.Name) == false) return false;
            string value = this._httpContext.Request.Cookies.Get(HttpCookieRepository.Name).Values.Get(key.ToString());
            if (value == null) return false;
            return true;
        }

        /// <summary>
        /// Deletes a element in the cookie based on the key
        /// </summary>
        /// <param name="key">CookieKey for lookup</param>
        public void DeleteElement(CookieKey key)
        {
            this.TransferCookieFromRequestToResponse();
            if (this._httpContext.Response.Cookies.Get(HttpCookieRepository.Name).Values[key.ToString()] != null)
                this._httpContext.Response.Cookies.Get(HttpCookieRepository.Name).Values.Remove(key.ToString());
        }

        /// <summary>
        /// Updates and element
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="key">CookieKey for lookup</param>
        /// <param name="data">New data for storing</param>
        /// <exception cref="InvalidOperationException">Exception is thrown if element doesn´t exists</exception>
        public void UpdateElement(CookieKey key, string data)
        {
            this.TransferCookieFromRequestToResponse();
            this._httpContext.Response.Cookies.Get(HttpCookieRepository.Name).Values[key.ToString()] = data;
        }

        /// <summary>
        /// Search for a cookie in the request, if it´s in the request return it
        /// if it is not in the request - return null
        /// </summary>
        private HttpCookie IsCookieInRequest()
        {
            return this._httpContext.Request.Cookies.Get(HttpCookieRepository.Name);
        }

        /// <summary>
        /// Transfer the cookie from Request to Response for updating the cookie.
        /// </summary>
        private void TransferCookieFromRequestToResponse()
        {
            HttpCookie cookie = this._httpContext.Request.Cookies.Get(HttpCookieRepository.Name);
            if (cookie != null)
            {
                this.SetDefaultValues(cookie);
                this._httpContext.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Cookie Default Settings (Expires/HttpOnly)
        /// </summary>
        /// <param name="cookie"></param>
        private void SetDefaultValues(HttpCookie cookie)
        {
            cookie.Expires = DateTime.Now.AddYears(2);
            cookie.HttpOnly = true;
        }
    }
}