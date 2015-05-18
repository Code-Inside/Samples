using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetSuperSimple.Models
{
    public class ToDoItem
    {
        public Guid Id { get; set; }
        public string Task { get; set; }
        public bool IsDone { get; set; }
    }
}