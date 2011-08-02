using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DllMembership.Lib;

namespace DllMembership.Web
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            DllMembership.Lib.User returnUser = AppUtil.GetActiveUser();
            if (returnUser.IsLoggedIn == false)
            {
                this.UserName.Text = "unangemeldet";
            }
            else
            {
                this.UserName.Text = returnUser.Name;
            }
        }
    }
}
