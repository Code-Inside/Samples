using System.Data.Entity;

namespace IdsPlusContext
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(string connectionString) : base(connectionString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Tenant> Tenants { get; set; }

    }
}