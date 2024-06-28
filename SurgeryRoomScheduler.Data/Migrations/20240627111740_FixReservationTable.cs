using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Doctors_DoctorId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_DoctorId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RoomId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.AddColumn<Guid>(
                name: "ConfirmedMedicalRecordsUserId",
                schema: "General",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConfirmedSupervisorUserId",
                schema: "General",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorNonNezam",
                schema: "General",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmedByMedicalRecords",
                schema: "General",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmedBySupervisor",
                schema: "General",
                table: "Reservations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoomCode",
                schema: "General",
                table: "Reservations",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedMedicalRecordsUserId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ConfirmedSupervisorUserId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DoctorNonNezam",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsConfirmedByMedicalRecords",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsConfirmedBySupervisor",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RoomCode",
                schema: "General",
                table: "Reservations");

            migrationBuilder.AddColumn<long>(
                name: "DoctorId",
                schema: "General",
                table: "Reservations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "RoomId",
                schema: "General",
                table: "Reservations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_DoctorId",
                schema: "General",
                table: "Reservations",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                schema: "General",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Doctors_DoctorId",
                schema: "General",
                table: "Reservations",
                column: "DoctorId",
                principalSchema: "General",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                schema: "General",
                table: "Reservations",
                column: "RoomId",
                principalSchema: "General",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
