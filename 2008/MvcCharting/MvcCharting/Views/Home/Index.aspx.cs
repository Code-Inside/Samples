using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCharting.Views.Home
{
    public partial class Index : ViewPage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            foreach (int value in (List<int>)this.ViewData["Chart"])
            {
                this.Chart1.Series["Column"].Points.Add(value);
            }
        }
    }
}
