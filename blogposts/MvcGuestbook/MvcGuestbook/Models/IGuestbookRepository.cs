using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcGuestbook.Models
{
    public interface IGuestbookRepository
    {
        IQueryable<Comment> GetComments();
        void CreateComment(Comment comment);

        IQueryable<Category> GetCategories();
    }
}
