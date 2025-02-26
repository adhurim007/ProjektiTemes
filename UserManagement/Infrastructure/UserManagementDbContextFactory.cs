using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Infrastructure
{
    public class UserManagementDbContextFactory : IDesignTimeDbContextFactory<UserManagementDbContext>
    {
        public UserManagementDbContext CreateDbContext(string[] args)
        {
            // Lexo konfigurimin nga appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Konfiguro DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<UserManagementDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionString is not configured.");
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new UserManagementDbContext(optionsBuilder.Options);
        }
    }
}
