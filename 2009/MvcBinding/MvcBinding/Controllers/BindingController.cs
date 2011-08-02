using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MvcBinding.Models;

namespace MvcBinding.Controllers
{
    public class BindingController : Controller
    {
        //
        // GET: /Binding/
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            return View();
        }

        #region FormCollection
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FormCollection()
        {
            return View("CreatePerson");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormCollection(FormCollection collection)
        {
            Person person = new Person();
            person.Prename = collection["Prename"];
            person.Surname = collection["Surname"];
            person.Age = int.Parse(collection["Age"]);
            return View("Result", person);
        }
        #endregion

        #region ParameterMatching
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ParameterMatching()
        {
            return View("CreatePerson");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParameterMatching(string Prename, string Surname, int Age)
        {
            Person person = new Person();
            person.Prename = Prename;
            person.Surname = Surname;
            person.Age = Age;

            return View("Result", person);
        }
        #endregion

        #region DefaultBinding
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DefaultBinding()
        {
            return View("CreatePerson");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DefaultBinding(Person person)
        {
            return View("Result", person);
        }
        #endregion

        #region DefaultBindingWithInclude
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DefaultBindingWithInclude()
        {
            return View("CreatePerson");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DefaultBindingWithInclude([Bind(Include="Prename")] Person person)
        {
            return View("Result", person);
        }
        #endregion

        #region DefaultBindingWithExclude
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DefaultBindingWithExclude()
        {
            return View("CreatePerson");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DefaultBindingWithExclude([Bind(Exclude = "Prename")] Person person)
        {
            return View("Result", person);
        }
        #endregion

        #region PersonModelBinder
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PersonModelBinder()
        {
            return View("CreatePerson");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PersonModelBinder([ModelBinder(typeof(PersonModelBinder))] Person person)
        {
            return View("Result", person);
        }
        #endregion
    }
}