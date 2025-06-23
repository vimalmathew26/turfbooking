using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace turfbooking.Migrations
{
    /// <inheritdoc />
    public partial class newMigration03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "Slots");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Slots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Slots");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "Slots",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
