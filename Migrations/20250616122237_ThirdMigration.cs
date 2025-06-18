using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace turfbooking.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Ground_GroundId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Ground_GroundId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Ground_GroundId",
                table: "Slots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ground",
                table: "Ground");

            migrationBuilder.RenameTable(
                name: "Ground",
                newName: "Grounds");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grounds",
                table: "Grounds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Grounds_GroundId",
                table: "Bookings",
                column: "GroundId",
                principalTable: "Grounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Grounds_GroundId",
                table: "Review",
                column: "GroundId",
                principalTable: "Grounds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Grounds_GroundId",
                table: "Slots",
                column: "GroundId",
                principalTable: "Grounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Grounds_GroundId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Grounds_GroundId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Grounds_GroundId",
                table: "Slots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grounds",
                table: "Grounds");

            migrationBuilder.RenameTable(
                name: "Grounds",
                newName: "Ground");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ground",
                table: "Ground",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Ground_GroundId",
                table: "Bookings",
                column: "GroundId",
                principalTable: "Ground",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Ground_GroundId",
                table: "Review",
                column: "GroundId",
                principalTable: "Ground",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Ground_GroundId",
                table: "Slots",
                column: "GroundId",
                principalTable: "Ground",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
