using InvenireServer.Domain.Entities.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Organizations;

public class OrganizationInvitationTypeConfiguration : IEntityTypeConfiguration<OrganizationInvitation>
{
    public void Configure(EntityTypeBuilder<OrganizationInvitation> builder)
    {
        // Properties.

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Description)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        builder.Property(i => i.OrganizationId)
            .HasColumnName("organization_id");

        // Relationships.

        builder.HasOne(i => i.Employee)
            .WithMany()
            .HasForeignKey("employee_id");
    }
}