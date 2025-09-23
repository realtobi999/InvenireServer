using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvenireServer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Fifth_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyCheckPropertyItem");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyScanPropertyItem");

            migrationBuilder.CreateTable(
                name: "PropertyCheckPropertyItem",
                columns: table => new
                {
                    PropertyCheckId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCheckPropertyItem", x => new { x.PropertyCheckId, x.PropertyItemId });
                    table.ForeignKey(
                        name: "FK_PropertyCheckPropertyItem_Items_PropertyItemId",
                        column: x => x.PropertyItemId,
                        principalTable: "Items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyCheckPropertyItem_Scans_PropertyCheckId",
                        column: x => x.PropertyCheckId,
                        principalTable: "Scans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCheckPropertyItem_PropertyItemId",
                table: "PropertyCheckPropertyItem",
                column: "PropertyItemId");
        }
    }
}
