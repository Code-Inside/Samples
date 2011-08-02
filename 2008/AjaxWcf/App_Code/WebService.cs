using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;

namespace WebServiceNamespace
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {

        public WebService()
        {

        }

        [WebMethod]
        public DateTime GetDateTime()
        {
            return DateTime.Now;
        }

        [WebMethod]
        public ComplexType GetComplexOne()
        {
            return ComplexType.GetOne();
        }

        [WebMethod]
        public List<ComplexType> GetComplexList()
        {
            return ComplexType.GetList();
        }
    }
}