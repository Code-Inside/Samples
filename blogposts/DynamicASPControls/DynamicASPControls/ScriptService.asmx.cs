using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;

namespace DynamicASPControls
{
    /// <summary>
    /// Summary description for ScriptService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ScriptService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public string GetControlHtml(string controlLocation)
        {
            Page page = new Page();
            UserControl userControl = (UserControl)page.LoadControl(controlLocation);
            userControl.EnableViewState = false;
            HtmlForm form = new HtmlForm();
            form.Controls.Add(userControl);
            page.Controls.Add(form);
            StringWriter textWriter = new StringWriter();
            HttpContext.Current.Server.Execute(page, textWriter, false);
            return CleanHtml(textWriter.ToString());
        }

        /// <summary>
        /// Removes Form tags
        /// </summary>
        private string CleanHtml(string html)
        {
            return Regex.Replace(html, @"<[/]?(form)[^>]*?>", "", RegexOptions.IgnoreCase);
        }
    }
}
