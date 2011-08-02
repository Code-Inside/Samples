using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcEFCodeFirst.Models
{
    public class User
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
    }
}