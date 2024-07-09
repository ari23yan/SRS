using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTableReservationConfirmationLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogDate",
                schema: "Common",
                table: "ReservationConfirmationLogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "Common",
                table: "ReservationConfirmationLogs",
                newName: "ReservationId");

            migrationBuilder.AddColumn<Guid>(
                name: "OperatorId",
                schema: "Common",
                table: "ReservationConfirmationLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ReservationConfirmationLogs_ReservationId",
                schema: "Common",
                table: "ReservationConfirmationLogs",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationConfirmationLogs_Reservations_ReservationId",
                schema: "Common",
                table: "ReservationConfirmationLogs",
                column: "ReservationId",
                principalSchema: "General",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationConfirmationLogs_Reservations_ReservationId",
                schema: "Common",
                table: "ReservationConfirmationLogs");

            migrationBuilder.DropIndex(
                name: "IX_ReservationConfirmationLogs_ReservationId",
                schema: "Common",
                table: "ReservationConfirmationLogs");

            migrationBuilder.DropColumn(
                name: "OperatorId",
                schema: "Common",
                table: "ReservationConfirmationLogs");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                schema: "Common",
                table: "ReservationConfirmationLogs",
                newName: "UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LogDate",
                schema: "Common",
                table: "ReservationConfirmationLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
