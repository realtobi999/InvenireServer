using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Persistence.Configurations.Organizations;
using InvenireServer.Infrastructure.Persistence.Configurations.Properties;
using InvenireServer.Infrastructure.Persistence.Configurations.Users;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence;

/// <summary>
/// Represents the EF Core database context for InvenireServer.
/// </summary>
public class InvenireServerContext : DbContext
{
    public InvenireServerContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<OrganizationInvitation> Invitations { get; set; }

    public DbSet<Property> Properties { get; set; }

    public DbSet<PropertyItem> Items { get; set; }

    public DbSet<PropertySuggestion> Suggestions { get; set; }

    public DbSet<PropertyScan> Scans { get; set; }

    public DbSet<PropertyScanPropertyItem> ScansItems { get; set; }

    /// <summary>
    /// Configures entity mappings for the context.
    /// </summary>
    /// <param name="builder">Model builder used to configure entities.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply entity configurations from dedicated configuration classes.
        new AdminTypeConfiguration().Configure(builder.Entity<Admin>());
        new EmployeeTypeConfiguration().Configure(builder.Entity<Employee>());
        new OrganizationTypeConfiguration().Configure(builder.Entity<Organization>());
        new OrganizationInvitationTypeConfiguration().Configure(builder.Entity<OrganizationInvitation>());
        new PropertyTypeConfiguration().Configure(builder.Entity<Property>());
        new PropertyItemTypeConfiguration().Configure(builder.Entity<PropertyItem>());
        new PropertySuggestionTypeConfiguration().Configure(builder.Entity<PropertySuggestion>());
        new PropertyScanTypeConfiguration().Configure(builder.Entity<PropertyScan>());
        new PropertyScanPropertyItemTypeConfiguration().Configure(builder.Entity<PropertyScanPropertyItem>());
    }
}
