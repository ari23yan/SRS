using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSurgeryCodeAndInsuranceCodeIntoReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InsuranceCode",
                schema: "General",
                table: "Reservations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SurgeryCode",
                schema: "General",
                table: "Reservations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsuranceCode",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SurgeryCode",
                schema: "General",
                table: "Reservations");
        }
    }
}
