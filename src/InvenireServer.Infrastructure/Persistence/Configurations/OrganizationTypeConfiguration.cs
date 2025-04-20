using InvenireServer.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations;

public class OrganizationTypeConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        // Properties.
        builder.Property(o => o.Id)
            .HasColumnName("id")
            .IsRequired();

        // Relationships.
        builder.HasOne<Admin>()
            .WithOne()
            .HasForeignKey<Admin>(a => a.OrganizationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Property>()
            .WithOne()
            .HasForeignKey<Property>(p => p.OrganizationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany<Employee>()
            .WithOne()
            .HasForeignKey(e => e.OrganizationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}