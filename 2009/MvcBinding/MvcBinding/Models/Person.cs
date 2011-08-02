using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcBinding.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Prename { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }
}
