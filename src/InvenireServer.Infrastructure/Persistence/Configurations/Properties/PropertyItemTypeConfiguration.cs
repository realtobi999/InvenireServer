using InvenireServer.Domain.Entities.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Properties;

public class PropertyItemTypeConfiguration : IEntityTypeConfiguration<PropertyItem>
{
    public void Configure(EntityTypeBuilder<PropertyItem> builder)
    {
        // Properties.

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InventoryNumber)
            .HasColumnName("inventory_number")
            .IsRequired()
            .HasMaxLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH);

        builder.Property(i => i.RegistrationNumber)
            .HasColumnName("registration_number")
            .IsRequired()
            .HasMaxLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH);

        builder.Property(i => i.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(PropertyItem.MAX_NAME_LENGTH);

        builder.Property(i => i.Price)
            .HasColumnName("price")
            .IsRequired();

        builder.Property(i => i.SerialNumber)
            .HasColumnName("serial_number")
            .HasMaxLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH);

        builder.Property(i => i.DateOfPurchase)
            .HasColumnName("date_of_purchase")
            .IsRequired();

        builder.Property(i => i.DateOfSale)
            .HasColumnName("date_of_sale");

        builder.Property(i => i.Description)
            .HasColumnName("description")
            .HasMaxLength(PropertyItem.MAX_DESCRIPTION_LENGTH);

        builder.Property(i => i.DocumentNumber)
            .HasColumnName("document_number")
            .IsRequired()
            .HasMaxLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH);

        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        // Navigational properties.

        builder.OwnsOne(i => i.Location, locationBuilder =>
        {
            locationBuilder.Property(l => l.Room)
                .HasColumnName("room")
                .IsRequired();
            locationBuilder.Property(l => l.Building)
                .HasColumnName("building")
                .IsRequired();
            locationBuilder.Property(l => l.AdditionalNote)
                .HasColumnName("additional_note");
        });

        builder.Property(i => i.PropertyId)
            .HasColumnName("property_id");

        builder.Property(i => i.EmployeeId)
            .HasColumnName("employee_id");
    }
}