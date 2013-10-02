using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayWithEffort
{
    public class GetBlogQuery
    {
        private readonly DemoContext _context;

        public GetBlogQuery(DemoContext context)
        {
            _context = context;
        }

        public List<Blog> Execute()
        {
            using (_context)
            {
                return _context.Blogs.ToList();
            }
        }
    }


    public class DemoContext : DbContext
    {

        public DemoContext()
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DemoContext(DbConnection connection)
            : base(connection, true)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Abstract { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public byte[] Photo { get; set; }
        public virtual Blog Blog { get; set; }
    }


}
