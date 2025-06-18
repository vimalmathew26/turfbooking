using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace turfbooking.Migrations
{
    /// <inheritdoc />
    public partial class SlotBookingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Slots_BookingId",
                table: "Slots");

            migrationBuilder.AddColumn<int>(
                name: "BookingId1",
                table: "Slots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_BookingId",
                table: "Slots",
                column: "BookingId",
                unique: true,
                filter: "[BookingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_BookingId1",
                table: "Slots",
                column: "BookingId1",
                unique: true,
                filter: "[BookingId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Bookings_BookingId1",
                table: "Slots",
                column: "BookingId1",
                principalTable: "Bookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Bookings_BookingId1",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_BookingId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_BookingId1",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "BookingId1",
                table: "Slots");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_BookingId",
                table: "Slots",
                column: "BookingId");
        }
    }
}
