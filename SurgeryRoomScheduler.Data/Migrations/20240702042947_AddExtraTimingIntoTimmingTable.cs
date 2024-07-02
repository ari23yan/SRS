using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraTimingIntoTimmingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRejections_ReservationRejectionReasons_ReservationRejectionReasonId",
                schema: "General",
                table: "ReservationRejections");

            migrationBuilder.DropTable(
                name: "ReservationRejectionReasons",
                schema: "General");

            migrationBuilder.DropTable(
                name: "UserDetails",
                schema: "Account");

            migrationBuilder.DropIndex(
                name: "IX_ReservationRejections_ReservationRejectionReasonId",
                schema: "General",
                table: "ReservationRejections");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "StatusId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                schema: "Account",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AssignedRoomCode",
                schema: "General",
                table: "Timings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExtraTiming",
                schema: "General",
                table: "Timings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviousOwner",
                schema: "General",
                table: "Timings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PatientHaveInsurance",
                schema: "General",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationRejections",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetails",
                schema: "Common",
                table: "JobLogs",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationConfirmationStatuses",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationConfirmationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservationRejectionAndCancellationReasons",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectionReasonType = table.Column<byte>(type: "tinyint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationRejectionAndCancellationReasons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRejections_ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationRejections",
                column: "ReservationRejectionAndCancellationReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationConfirmations_StatusId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationConfirmations_ReservationConfirmationStatuses_StatusId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "StatusId",
                principalSchema: "General",
                principalTable: "ReservationConfirmationStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRejections_ReservationRejectionAndCancellationReasons_ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationRejections",
                column: "ReservationRejectionAndCancellationReasonId",
                principalSchema: "General",
                principalTable: "ReservationRejectionAndCancellationReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationConfirmations_ReservationConfirmationStatuses_StatusId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRejections_ReservationRejectionAndCancellationReasons_ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationRejections");

            migrationBuilder.DropTable(
                name: "ReservationConfirmationStatuses",
                schema: "General");

            migrationBuilder.DropTable(
                name: "ReservationRejectionAndCancellationReasons",
                schema: "General");

            migrationBuilder.DropIndex(
                name: "IX_ReservationRejections_ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationRejections");

            migrationBuilder.DropIndex(
                name: "IX_ReservationConfirmations_StatusId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                schema: "Account",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsExtraTiming",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "PreviousOwner",
                schema: "General",
                table: "Timings");

            migrationBuilder.DropColumn(
                name: "PatientHaveInsurance",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationRejections");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "Status");

            migrationBuilder.AlterColumn<long>(
                name: "AssignedRoomCode",
                schema: "General",
                table: "Timings",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetails",
                schema: "Common",
                table: "JobLogs",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationRejectionReasons",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    RejectionReasonType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationRejectionReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                schema: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetails_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Account",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRejections_ReservationRejectionReasonId",
                schema: "General",
                table: "ReservationRejections",
                column: "ReservationRejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_UserId",
                schema: "Account",
                table: "UserDetails",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRejections_ReservationRejectionReasons_ReservationRejectionReasonId",
                schema: "General",
                table: "ReservationRejections",
                column: "ReservationRejectionReasonId",
                principalSchema: "General",
                principalTable: "ReservationRejectionReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
