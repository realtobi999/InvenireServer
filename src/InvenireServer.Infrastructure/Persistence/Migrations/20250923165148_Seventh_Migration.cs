using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvenireServer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Seventh_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyScanPropertyItem");

            migrationBuilder.CreateTable(
                name: "ScansItems",
                columns: table => new
                {
                    property_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    property_scan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_scanned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScansItems", x => new { x.property_scan_id, x.property_item_id });
                    table.ForeignKey(
                        name: "FK_ScansItems_Items_property_item_id",
                        column: x => x.property_item_id,
                        principalTable: "Items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScansItems_Scans_property_scan_id",
                        column: x => x.property_scan_id,
                        principalTable: "Scans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScansItems_property_item_id",
                table: "ScansItems",
                column: "property_item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScansItems");

            migrationBuilder.CreateTable(
                name: "PropertyScanPropertyItem",
                columns: table => new
                {
                    PropertyCheckId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyScanPropertyItem", x => new { x.PropertyCheckId, x.PropertyItemId });
                    table.ForeignKey(
                        name: "FK_PropertyScanPropertyItem_Items_PropertyItemId",
                        column: x => x.PropertyItemId,
                        principalTable: "Items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyScanPropertyItem_Scans_PropertyCheckId",
                        column: x => x.PropertyCheckId,
                        principalTable: "Scans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyScanPropertyItem_PropertyItemId",
                table: "PropertyScanPropertyItem",
                column: "PropertyItemId");
        }
    }
}
