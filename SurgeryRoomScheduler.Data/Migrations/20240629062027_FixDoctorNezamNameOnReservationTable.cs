using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDoctorNezamNameOnReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorNonNezam",
                schema: "General",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "DoctorNoNezam",
                schema: "General",
                table: "Reservations",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorNoNezam",
                schema: "General",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "DoctorNonNezam",
                schema: "General",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
