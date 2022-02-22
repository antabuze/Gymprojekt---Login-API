using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupparbete___API_Login
{
    public class AdminContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }

        public AdminContext(DbContextOptions options) : base (options)
        {

        }

        public DbSet<UserCheckin> UserCheckins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .HasIndex(u => u.Email)
                .IsUnique(true);
        }
    }
}
