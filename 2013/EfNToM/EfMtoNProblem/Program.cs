using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfMtoNProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            using (DemoContext context = new DemoContext())
            {
                context.Tenants.ToList();
            }
        }
    }

    public class DemoContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<User>()
                        .HasRequired(u => u.Tenant)
                        .WithMany(t => t.Users)
                        .HasForeignKey(x => x.TenantId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                        .HasRequired(g => g.Tenant)
                        .WithMany(t => t.Groups)
                        .HasForeignKey(x => x.TenantId)
                        .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Group> Groups { get; set; }

    }

    public class User
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public Tenant Tenant { get; set; }
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }
        public virtual List<Group> MemberOf { get; set; }

    }

    public class Group
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public Tenant Tenant { get; set; }
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }
        public virtual List<User> Members { get; set; }
    }

    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public List<Group> Groups { get; set; }

    }
}
