using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mvc;

/// <summary>
/// Summary description for SearchController
/// </summary>
public class SearchController : Controller
{

    [ControllerAction]
    public void Index()
    {
        RenderView("Index");
    }

    [ControllerAction]
	public void Results(string query)
	{
        BookCollectionManager man = new BookCollectionManager();
        BookCollection data = man.GetBookCollection(query, 1);
        ViewData["BookCollection"] = data;
        RenderView("Results");
	}
}
