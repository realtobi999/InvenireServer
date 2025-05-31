using InvenireServer.Domain.Core.Entities;
using InvenireServer.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence;

public class InvenireServerContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }

    public InvenireServerContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply entity configurations from dedicated configuration classes.
        new EmployeeTypeConfiguration().Configure(builder.Entity<Employee>());
    }
}