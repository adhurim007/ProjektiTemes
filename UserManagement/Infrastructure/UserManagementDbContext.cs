using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure
{
    //public class UserManagementDbContext : IdentityDbContext<IdentityUser>
    //{
    //    public DbSet<User> Users { get; set; }

    //    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
    //        : base(options) { }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<User>(entity =>
    //        {
    //            entity.Property(e => e.Username).HasMaxLength(100);
    //            entity.Property(e => e.PasswordHash).HasMaxLength(200);
    //            entity.Property(e => e.Role).HasMaxLength(50);
    //            entity.Property(e => e.CreatedDate)
    //                .HasDefaultValueSql("GETDATE()")
    //                .HasColumnType("datetime");
    //        });
    //    }
    //}

    public class UserManagementDbContext : IdentityDbContext
    {
        public DbSet<User> Users { get; set; }
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
            : base(options) { }
    }
}
