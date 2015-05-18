using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AspNetSuperSimple.Models;

namespace AspNetSuperSimple.Controllers
{
    public class ToDoApiController : ApiController
    {
        private ToDoContext db = new ToDoContext();

        // GET: api/ToDoApi
        public IQueryable<ToDoItem> GetToDoItems()
        {
            return db.ToDoItems;
        }

        // GET: api/ToDoApi/5
        [ResponseType(typeof(ToDoItem))]
        public async Task<IHttpActionResult> GetToDoItem(Guid id)
        {
            ToDoItem toDoItem = await db.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return Ok(toDoItem);
        }

        // PUT: api/ToDoApi/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutToDoItem(Guid id, ToDoItem toDoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != toDoItem.Id)
            {
                return BadRequest();
            }

            db.Entry(toDoItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ToDoApi
        [ResponseType(typeof(ToDoItem))]
        public async Task<IHttpActionResult> PostToDoItem(ToDoItem toDoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ToDoItems.Add(toDoItem);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ToDoItemExists(toDoItem.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = toDoItem.Id }, toDoItem);
        }

        // DELETE: api/ToDoApi/5
        [ResponseType(typeof(ToDoItem))]
        public async Task<IHttpActionResult> DeleteToDoItem(Guid id)
        {
            ToDoItem toDoItem = await db.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            db.ToDoItems.Remove(toDoItem);
            await db.SaveChangesAsync();

            return Ok(toDoItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ToDoItemExists(Guid id)
        {
            return db.ToDoItems.Count(e => e.Id == id) > 0;
        }
    }
}