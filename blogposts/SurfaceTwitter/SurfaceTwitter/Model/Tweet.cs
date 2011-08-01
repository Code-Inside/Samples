using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfaceTwitter.Model
{
    public class Tweet
    {
        public string Message { get; set; }
        public User User { get; set; }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
