using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Persistence.Configurations.Organizations;
using InvenireServer.Infrastructure.Persistence.Configurations.Users;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence;

public class InvenireServerContext : DbContext
{
    public InvenireServerContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<OrganizationInvitation> Invitations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply entity configurations from dedicated configuration classes.
        new AdminTypeConfiguration().Configure(builder.Entity<Admin>());
        new EmployeeTypeConfiguration().Configure(builder.Entity<Employee>());
        new OrganizationTypeConfiguration().Configure(builder.Entity<Organization>());
        new OrganizationInvitationTypeConfiguration().Configure(builder.Entity<OrganizationInvitation>());
    }
}