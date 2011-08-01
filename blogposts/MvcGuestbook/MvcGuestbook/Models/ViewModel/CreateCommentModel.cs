using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcGuestbook.Models.ViewModel
{
    public class CreateCommentModel
    {
        public Comment Comment { get; set; }
        public List<Category> Categories { get; set; }
    }
}
