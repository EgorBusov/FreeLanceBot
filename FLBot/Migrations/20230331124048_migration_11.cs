using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FLBot.Migrations
{
    /// <inheritdoc />
    public partial class migration_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdExecutor",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IdExecutor",
                table: "Orders",
                type: "bigint",
                nullable: true);
        }
    }
}
