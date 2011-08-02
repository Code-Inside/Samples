using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRenderToString.Helper
{
    /// <summary>Fake IView implementation used to instantiate an HtmlHelper.</summary>
    public class FakeView : IView
    {
        #region IView Members

        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
