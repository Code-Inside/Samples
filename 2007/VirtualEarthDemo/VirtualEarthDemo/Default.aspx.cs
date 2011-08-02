using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace VirtualEarthDemo
{
    public partial class _Default : System.Web.UI.Page
    {
        string mapHeight = "400px";
        string mapWidth = "400px";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["height"] != "") mapHeight = Request.QueryString["height"];
            if (Request.QueryString["width"] != "") mapWidth = Request.QueryString["width"];

            PanelMap.Style["height"] = mapHeight + "px";
            PanelMap.Style["width"] = mapWidth + "px";
        }
    }
}
