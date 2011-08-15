using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmbeddedRavenDB.Models;

namespace EmbeddedRavenDB.Controllers
{
    public class WordsController : BaseController
    {
        public ViewResult Index()
        {
            var words = Session.Query<Word>().ToList();
            return View(words);
        }

        //
        // GET: /Words/Details/5

        public ViewResult Details(Guid id)
        {
            Word word = Session.Load<Word>(id);
            return View(word);
        }

        //
        // GET: /Words/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Words/Create

        [HttpPost]
        public ActionResult Create(Word word)
        {
            if (ModelState.IsValid)
            {
                Session.Store(word);
                Session.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(word);
        }

        //
        // GET: /Words/Edit/5

        public ActionResult Edit(Guid id)
        {
            Word word = Session.Load<Word>(id);
            return View(word);
        }

        //
        // POST: /Words/Edit/5

        [HttpPost]
        public ActionResult Edit(Word word)
        {
            if (ModelState.IsValid)
            {
                Session.Store(word);
                Session.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(word);
        }

        //
        // GET: /Words/Delete/5

        public ActionResult Delete(Guid id)
        {
            Word word = Session.Load<Word>(id);
            return View(word);
        }

        //
        // POST: /Words/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Word word = Session.Load<Word>(id);
            Session.Delete(word);
            Session.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
