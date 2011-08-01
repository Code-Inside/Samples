using System;
using System.Linq;

namespace MvcGuestbook.Models
{
    public class SqlGuestbookRepository : IGuestbookRepository
    {
        private GuestbookEntities _context = new GuestbookEntities(); 

        public IQueryable<Comment> GetComments()
        {

            return (from x in _context.Comments.Include("Categories")
                    select new Comment()
                               {
                                   Id = x.Id,
                                   Name = x.Name,
                                   Text = x.Text,
                                   CreatedOn = x.DateTime,
                                   Category = new Category()
                                                  {
                                                      Id = x.Categories.Id,
                                                      Name = x.Categories.Name
                                                  }
                               });
        }

        public void CreateComment(Comment comment)
        {
            Comments newComment = new Comments();
            newComment.Id = Guid.NewGuid();
            newComment.DateTime = DateTime.Now;
            newComment.Name = comment.Name;
            newComment.Text = comment.Text;

            Categories addedCat = (from x in _context.Categories
                                   where x.Id == comment.Category.Id
                                   select x).FirstOrDefault();

            newComment.Categories = addedCat;

            _context.AddToComments(newComment);
            _context.SaveChanges();
        }

        public IQueryable<Category> GetCategories()
        {
            return (from x in _context.Categories
                    select new Category()
                               {
                                   Id = x.Id,
                                   Name = x.Name
                               });
        }
    }
}