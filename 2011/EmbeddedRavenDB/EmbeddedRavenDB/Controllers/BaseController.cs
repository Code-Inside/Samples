using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;

namespace EmbeddedRavenDB.Controllers
{
    public abstract class BaseController : Controller
    {
        public new IDocumentSession Session { get; set; }
    }
}
