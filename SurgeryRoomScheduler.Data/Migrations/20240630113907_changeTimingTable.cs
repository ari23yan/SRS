using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeTimingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledEndDate",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "ScheduledEndDate_Shamsi",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "ScheduledStartDate",
                schema: "General",
                table: "Timings");

            migrationBuilder.RenameColumn(
                name: "ScheduledStartDate_Shamsi",
                schema: "General",
                table: "Timings",
                newName: "ScheduledDate_Shamsi");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ScheduledDate",
                schema: "General",
                table: "Timings",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledEndTime",
                schema: "General",
                table: "Timings",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledStartTime",
                schema: "General",
                table: "Timings",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledDate",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "ScheduledEndTime",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "ScheduledStartTime",
                schema: "General",
                table: "Timings");

            migrationBuilder.RenameColumn(
                name: "ScheduledDate_Shamsi",
                schema: "General",
                table: "Timings",
                newName: "ScheduledStartDate_Shamsi");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledEndDate",
                schema: "General",
                table: "Timings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ScheduledEndDate_Shamsi",
                schema: "General",
                table: "Timings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledStartDate",
                schema: "General",
                table: "Timings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
