using InvenireServer.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations;

public class EmployeeTypeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Properties.
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        // Relationships.
        builder.OwnsMany(e => e.AssignedItems, ob =>
        {
            // Configure the PropertyItem entity using a builder extension method.
            ob.ConfigurePropertyItem();
        });
    }
}