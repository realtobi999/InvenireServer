using InvenireServer.Domain.Core.Entities;
using InvenireServer.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence;

public class InvenireServerContext : DbContext
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyGroup> PropertyGroups { get; set; }
    public DbSet<PropertyItem> PropertyItems { get; set; }
    public DbSet<PropertyScan> PropertyScans { get; set; }

    public InvenireServerContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply entity configurations from dedicated configuration classes.
        new OrganizationTypeConfiguration().Configure(builder.Entity<Organization>());
        new AdminTypeConfiguration().Configure(builder.Entity<Admin>());
        new EmployeeTypeConfiguration().Configure(builder.Entity<Employee>());
        new PropertyTypeConfiguration().Configure(builder.Entity<Property>());
        new PropertyGroupTypeConfiguration().Configure(builder.Entity<PropertyGroup>());
        new PropertyScanConfiguration().Configure(builder.Entity<PropertyScan>());
    }
}