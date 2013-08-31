using System;
using Microsoft.Phone.Tasks;

namespace WPAppStudio.Services
{
    public class WebBrowserService
    {
        /// <summary>
        /// Open the WebBrowser.
        /// </summary>
        /// <param name="url">url</param>
        public void OpenBrowser(string url)
        {
            var webBrowserTask = new WebBrowserTask { Uri = new Uri(url, UriKind.Absolute) };

            webBrowserTask.Show();
        }
    }
}