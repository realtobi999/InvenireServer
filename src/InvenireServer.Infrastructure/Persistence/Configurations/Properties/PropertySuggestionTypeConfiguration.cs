using InvenireServer.Domain.Entities.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Properties;

public class PropertySuggestionTypeConfiguration : IEntityTypeConfiguration<PropertySuggestion>
{
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

        builder.Property(s => s.RequestBody)
            .HasColumnName("request_body")
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.LastUpdatedAt)
            .HasColumnName("last_updated_at");
    }
}

