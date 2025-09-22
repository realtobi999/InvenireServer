namespace InvenireServer.Application.Core.Properties.Items.Commands.CreateFromExcelFile;

public record CreatePropertyItemsFromExcelFileConfiguration
{
    public static readonly string[] PropertyKeys =
    [
        "InventoryNumber",
        "RegistrationNumber",
        "Name",
        "Price",
        "SerialNumber",
        "DateOfPurchase",
        "DateOfSale",
        "LocationRoom",
        "LocationBuilding",
        "LocationAdditionalNote",
        "Description",
        "DocumentNumber",
        "EmployeeEmailAddress"
    ];

    public static readonly string[] RequiredPropertyKey =
    [
        "InventoryNumber",
        "RegistrationNumber",
        "Name",
        "Price",
        "DateOfPurchase",
        "LocationRoom",
        "LocationBuilding",
    ];

    public static int TotalProperties => PropertyKeys.Length;
}
