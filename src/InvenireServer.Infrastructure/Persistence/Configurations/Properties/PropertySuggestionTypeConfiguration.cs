using InvenireServer.Domain.Entities.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Properties;

/// <summary>
/// Configures the <see cref="PropertySuggestion"/> entity.
/// </summary>
public class PropertySuggestionTypeConfiguration : IEntityTypeConfiguration<PropertySuggestion>
{
    /// <summary>
    /// Configures the <see cref="PropertySuggestion"/> entity.
    /// </summary>
    /// <param name="builder">Builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<PropertySuggestion> builder)
    {
        // Properties.

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(PropertySuggestion.MAX_NAME_LENGTH);

        builder.Property(s => s.Description)
            .HasColumnName("description")
            .HasMaxLength(PropertySuggestion.MAX_DESCRIPTION_LENGTH);

        builder.Property(s => s.Feedback)
            .HasColumnName("feedback")
            .HasMaxLength(PropertySuggestion.MAX_FEEDBACK_LENGTH);

        builder.Property(s => s.PayloadString)
            .HasColumnName("payload_string")
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.ResolvedAt)
            .HasColumnName("resolved_at");

        builder.Property(s => s.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        builder.Property(i => i.PropertyId)
            .HasColumnName("property_id");

        builder.Property(i => i.EmployeeId)
            .HasColumnName("employee_id");
    }
}
