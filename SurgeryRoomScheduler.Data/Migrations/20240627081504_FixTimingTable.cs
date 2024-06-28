using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTimingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timings_Doctors_DoctorId",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropForeignKey(
                name: "FK_Timings_Rooms_RoomId",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropIndex(
                name: "IX_Timings_RoomId",
                schema: "General",
                table: "Timings");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                schema: "General",
                table: "Timings",
                newName: "RoomCode");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                schema: "General",
                table: "Timings",
                newName: "AssignedRoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Timings_DoctorId",
                schema: "General",
                table: "Timings",
                newName: "IX_Timings_AssignedRoomId");

            migrationBuilder.AddColumn<long>(
                name: "AssignedDoctorId",
                schema: "General",
                table: "Timings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DoctorNoNezam",
                schema: "General",
                table: "Timings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Timings_AssignedDoctorId",
                schema: "General",
                table: "Timings",
                column: "AssignedDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timings_Doctors_AssignedDoctorId",
                schema: "General",
                table: "Timings",
                column: "AssignedDoctorId",
                principalSchema: "General",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timings_Rooms_AssignedRoomId",
                schema: "General",
                table: "Timings",
                column: "AssignedRoomId",
                principalSchema: "General",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timings_Doctors_AssignedDoctorId",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropForeignKey(
                name: "FK_Timings_Rooms_AssignedRoomId",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropIndex(
                name: "IX_Timings_AssignedDoctorId",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "AssignedDoctorId",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "DoctorNoNezam",
                schema: "General",
                table: "Timings");

            migrationBuilder.RenameColumn(
                name: "RoomCode",
                schema: "General",
                table: "Timings",
                newName: "RoomId");

            migrationBuilder.RenameColumn(
                name: "AssignedRoomId",
                schema: "General",
                table: "Timings",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Timings_AssignedRoomId",
                schema: "General",
                table: "Timings",
                newName: "IX_Timings_DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Timings_RoomId",
                schema: "General",
                table: "Timings",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timings_Doctors_DoctorId",
                schema: "General",
                table: "Timings",
                column: "DoctorId",
                principalSchema: "General",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timings_Rooms_RoomId",
                schema: "General",
                table: "Timings",
                column: "RoomId",
                principalSchema: "General",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
