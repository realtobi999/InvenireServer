using InvenireServer.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations;

public class AdminTypeConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        // Properties.
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .IsRequired();
    }
}