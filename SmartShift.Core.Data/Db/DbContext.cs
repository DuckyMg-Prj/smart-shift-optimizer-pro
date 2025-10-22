using System.Data.Entity;

using System.Data.Entity;
using SmartShift.Core.Model.Entities;

namespace SmartShift.Core.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection") { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }

        // other DbSet<> will come later
    }
}