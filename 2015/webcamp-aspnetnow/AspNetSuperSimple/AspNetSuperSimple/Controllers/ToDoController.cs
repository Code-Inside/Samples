using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspNetSuperSimple.Models;

namespace AspNetSuperSimple.Controllers
{
    public class ToDoController : Controller
    {
        private ToDoContext db = new ToDoContext();

        // GET: ToDo
        public async Task<ActionResult> Index()
        {
            return View(await db.ToDoItems.ToListAsync());
        }

        // GET: ToDo/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoItem toDoItem = await db.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return HttpNotFound();
            }
            return View(toDoItem);
        }

        // GET: ToDo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToDo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Task,IsDone")] ToDoItem toDoItem)
        {
            if (ModelState.IsValid)
            {
                toDoItem.Id = Guid.NewGuid();
                db.ToDoItems.Add(toDoItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(toDoItem);
        }

        // GET: ToDo/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoItem toDoItem = await db.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return HttpNotFound();
            }
            return View(toDoItem);
        }

        // POST: ToDo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Task,IsDone")] ToDoItem toDoItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toDoItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(toDoItem);
        }

        // GET: ToDo/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoItem toDoItem = await db.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return HttpNotFound();
            }
            return View(toDoItem);
        }

        // POST: ToDo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            ToDoItem toDoItem = await db.ToDoItems.FindAsync(id);
            db.ToDoItems.Remove(toDoItem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
