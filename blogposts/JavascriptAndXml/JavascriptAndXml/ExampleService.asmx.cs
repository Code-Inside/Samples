using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Script.Services;
using System.Xml;
using System.IO;


namespace JavascriptAndXml
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für ExampleService
    /// </summary>
    [ScriptService]
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class ExampleService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat=ResponseFormat.Xml)]
        public XmlDocument LoadExample(int id)
        {
            string filename = "Example_" + id.ToString() + ".xml";
            string path = Path.Combine(this.Context.Request.PhysicalApplicationPath, filename);

            XmlDocument ExampleDocument = new XmlDocument();
            ExampleDocument.Load(path);

            return ExampleDocument;
        }
    }
}
