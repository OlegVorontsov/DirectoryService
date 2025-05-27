using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "directory_service");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,");

            migrationBuilder.CreateTable(
                name: "departments",
                schema: "directory_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    path = table.Column<string>(type: "ltree", nullable: false),
                    depth = table.Column<short>(type: "smallint", nullable: false),
                    children_count = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departments", x => x.id);
                    table.ForeignKey(
                        name: "fk_departments_departments_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "directory_service",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                schema: "directory_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time_zone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    address = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                schema: "directory_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_positions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "department_locations",
                schema: "directory_service",
                columns: table => new
                {
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department_locations", x => new { x.department_id, x.location_id });
                    table.ForeignKey(
                        name: "fk_department_locations_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "directory_service",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_department_locations_locations_location_id",
                        column: x => x.location_id,
                        principalSchema: "directory_service",
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_department_locations_location_id",
                schema: "directory_service",
                table: "department_locations",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_parent_id",
                schema: "directory_service",
                table: "departments",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_path",
                schema: "directory_service",
                table: "departments",
                column: "path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_locations_name",
                schema: "directory_service",
                table: "locations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_positions_name",
                schema: "directory_service",
                table: "positions",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "department_locations",
                schema: "directory_service");

            migrationBuilder.DropTable(
                name: "positions",
                schema: "directory_service");

            migrationBuilder.DropTable(
                name: "departments",
                schema: "directory_service");

            migrationBuilder.DropTable(
                name: "locations",
                schema: "directory_service");
        }
    }
}
