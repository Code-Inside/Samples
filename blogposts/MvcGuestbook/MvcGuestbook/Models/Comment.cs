using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcGuestbook.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public Category Category { get; set; }
    }
}
