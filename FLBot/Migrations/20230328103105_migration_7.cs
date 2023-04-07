using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FLBot.Migrations
{
    /// <inheritdoc />
    public partial class migration_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCategory",
                table: "Filters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdSubCategory",
                table: "Filters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCategory",
                table: "Filters");

            migrationBuilder.DropColumn(
                name: "IdSubCategory",
                table: "Filters");
        }
    }
}
