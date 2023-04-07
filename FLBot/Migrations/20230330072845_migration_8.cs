using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FLBot.Migrations
{
    /// <inheritdoc />
    public partial class migration_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IdChat",
                table: "Clients",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdChat",
                table: "Clients");
        }
    }
}
