using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DllMembership.Lib
{
    public class User
    {
        public bool IsLoggedIn { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
