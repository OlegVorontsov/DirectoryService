using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddGist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_departments_path",
                schema: "directory_service",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "idx_departments_path",
                schema: "directory_service",
                table: "departments",
                column: "path")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_departments_path",
                schema: "directory_service",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "ix_departments_path",
                schema: "directory_service",
                table: "departments",
                column: "path");
        }
    }
}
