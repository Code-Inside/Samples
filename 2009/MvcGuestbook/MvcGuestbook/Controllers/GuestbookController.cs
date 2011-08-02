using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MvcGuestbook.Models;

namespace MvcGuestbook.Controllers
{
    public class GuestbookController : Controller
    {
        private IGuestbookRepository _rep;

        public GuestbookController() : this(new SqlGuestbookRepository())
        {
            
        }

        public GuestbookController(IGuestbookRepository rep)
        {
            this._rep = rep;
        }

        //
        // GET: /Guestbook/
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            List<Comment> comment = this._rep.GetComments().ToList();
            return View(comment);
        }

        //
        // GET: /Guestbook/Create
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            List<Category> result = this._rep.GetCategories().ToList();
            SelectList select = new SelectList(result, "Id", "Name");

            return View(select);
        } 

        //
        // POST: /Guestbook/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string name, string text, string category)
        {
            try
            {
                if((name == "") || (text == "")) throw new ArgumentException("Name/Text");

                Comment newComment = new Comment();
                newComment.Name = name;
                newComment.Text = text;
                newComment.Category = new Category();
                newComment.Category.Id = new Guid(category);
   
                this._rep.CreateComment(newComment);
                return RedirectToAction("Index");
            }
            catch
            {
                if (name == "") ModelState.AddModelError("Name", "Set Name");
                if (text == "") ModelState.AddModelError("Text", "Set Text");

                List<Category> result = this._rep.GetCategories().ToList();
                SelectList select = new SelectList(result, "Id", "Name");

                return View(select);
            }
        }
    }
}
