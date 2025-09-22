using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvenireServer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Third_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "document_number",
                table: "Items",
                type: "character varying(155)",
                maxLength: 155,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(155)",
                oldMaxLength: 155);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "document_number",
                table: "Items",
                type: "character varying(155)",
                maxLength: 155,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(155)",
                oldMaxLength: 155,
                oldNullable: true);
        }
    }
}
