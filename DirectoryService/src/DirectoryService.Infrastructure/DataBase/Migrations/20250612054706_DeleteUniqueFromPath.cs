using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUniqueFromPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_departments_path",
                schema: "directory_service",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "ix_departments_path",
                schema: "directory_service",
                table: "departments",
                column: "path");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_departments_path",
                schema: "directory_service",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "ix_departments_path",
                schema: "directory_service",
                table: "departments",
                column: "path",
                unique: true);
        }
    }
}
