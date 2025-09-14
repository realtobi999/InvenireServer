using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvenireServer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class First_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    last_name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    email_address = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.id);
                    table.ForeignKey(
                        name: "FK_Admins_Organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "Organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    last_name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    email_address = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.id);
                    table.ForeignKey(
                        name: "FK_Employees_Organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "Organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.id);
                    table.ForeignKey(
                        name: "FK_Properties_Organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "Organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.id);
                    table.ForeignKey(
                        name: "FK_Invitations_Employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Invitations_Organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "Organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inventory_number = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: false),
                    registration_number = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: false),
                    name = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    serial_number = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: true),
                    date_of_purchase = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    date_of_sale = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    room = table.Column<string>(type: "text", nullable: false),
                    building = table.Column<string>(type: "text", nullable: false),
                    additional_note = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "character varying(555)", maxLength: 555, nullable: true),
                    document_number = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    property_id = table.Column<Guid>(type: "uuid", nullable: true),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.id);
                    table.ForeignKey(
                        name: "FK_Items_Employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Items_Properties_property_id",
                        column: x => x.property_id,
                        principalTable: "Properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: false),
                    description = table.Column<string>(type: "character varying(555)", maxLength: 555, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    property_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scans", x => x.id);
                    table.ForeignKey(
                        name: "FK_Scans_Properties_property_id",
                        column: x => x.property_id,
                        principalTable: "Properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suggestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(155)", maxLength: 155, nullable: false),
                    description = table.Column<string>(type: "character varying(555)", maxLength: 555, nullable: true),
                    feedback = table.Column<string>(type: "character varying(555)", maxLength: 555, nullable: true),
                    payload_string = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    property_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suggestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Suggestions_Employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suggestions_Properties_property_id",
                        column: x => x.property_id,
                        principalTable: "Properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Admins_email_address",
                table: "Admins",
                column: "email_address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_organization_id",
                table: "Admins",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_email_address",
                table: "Employees",
                column: "email_address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_organization_id",
                table: "Employees",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_employee_id",
                table: "Invitations",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_organization_id",
                table: "Invitations",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_Items_employee_id",
                table: "Items",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Items_property_id",
                table: "Items",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_name",
                table: "Organizations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_organization_id",
                table: "Properties",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCheckPropertyItem_PropertyItemId",
                table: "PropertyCheckPropertyItem",
                column: "PropertyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Scans_property_id",
                table: "Scans",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_employee_id",
                table: "Suggestions",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_property_id",
                table: "Suggestions",
                column: "property_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropTable(
                name: "PropertyCheckPropertyItem");

            migrationBuilder.DropTable(
                name: "Suggestions");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Scans");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
