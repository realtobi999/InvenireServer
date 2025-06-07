using InvenireServer.Domain.Entities;
using InvenireServer.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence;

public class InvenireServerContext : DbContext
{
    public InvenireServerContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply entity configurations from dedicated configuration classes.
        new AdminTypeConfiguration().Configure(builder.Entity<Admin>());
        new EmployeeTypeConfiguration().Configure(builder.Entity<Employee>());
    }
}