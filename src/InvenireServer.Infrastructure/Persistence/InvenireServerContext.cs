using InvenireServer.Domain.Core.Entities;
using InvenireServer.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence;

/// <summary>
/// Represents the Entity Framework Core database context for the Invenire server.
/// </summary>
public class InvenireServerContext : DbContext
{
    /// <summary>
    /// Gets or sets the table of employee entities.
    /// </summary>
    public DbSet<Employee> Employees { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvenireServerContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to configure the database context.</param>
    public InvenireServerContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model using entity configurations.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply entity configurations from dedicated configuration classes.
        new EmployeeTypeConfiguration().Configure(builder.Entity<Employee>());
    }
}
