using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTimingTablePart3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Timings_AssignedRoomId",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "AssignedDoctorId",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "AssignedRoomId",
                schema: "General",
                table: "Timings");

            migrationBuilder.RenameColumn(
                name: "DoctorNoNezam",
                schema: "General",
                table: "Timings",
                newName: "AssignedDoctorNoNezam");

            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "General",
                table: "Timings",
                newName: "AssignedRoomCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedRoomCode",
                schema: "General",
                table: "Timings",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "AssignedDoctorNoNezam",
                schema: "General",
                table: "Timings",
                newName: "DoctorNoNezam");

            migrationBuilder.AddColumn<long>(
                name: "AssignedDoctorId",
                schema: "General",
                table: "Timings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AssignedRoomId",
                schema: "General",
                table: "Timings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Timings_AssignedDoctorId",
                schema: "General",
                table: "Timings",
                column: "AssignedDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Timings_AssignedRoomId",
                schema: "General",
                table: "Timings",
                column: "AssignedRoomId");

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
    }
}
