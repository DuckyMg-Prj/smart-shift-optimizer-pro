using System.Data.Entity;
using SmartShift.Core.Model.Entities;

namespace SmartShift.Core.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection") { }

        public DbSet<UserModel> UserModel { get; set; }
        public DbSet<CompanyModel> CompanyModel { get; set; }

        // other DbSet<> will come later
    }
}