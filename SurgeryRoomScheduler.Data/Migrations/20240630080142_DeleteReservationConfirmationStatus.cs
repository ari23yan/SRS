using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteReservationConfirmationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationConfirmations_ReservationConfirmationStatus_StatusId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationConfirmations_ReservationConfirmationTypes_OperationTypeId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropTable(
                name: "ReservationConfirmationStatus",
                schema: "General");

            migrationBuilder.DropIndex(
                name: "IX_ReservationConfirmations_OperationTypeId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropColumn(
                name: "OperationTypeId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "ReservationConfirmationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationConfirmations_StatusId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "IX_ReservationConfirmations_ReservationConfirmationTypeId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsConfirmedBySupervisor",
                schema: "General",
                table: "ReservationConfirmations",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectionUserId",
                schema: "General",
                table: "ReservationConfirmations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "General",
                table: "ReservationConfirmations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationConfirmations_ReservationConfirmationTypes_ReservationConfirmationTypeId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "ReservationConfirmationTypeId",
                principalSchema: "General",
                principalTable: "ReservationConfirmationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationConfirmations_ReservationConfirmationTypes_ReservationConfirmationTypeId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropColumn(
                name: "RejectionUserId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.RenameColumn(
                name: "ReservationConfirmationTypeId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationConfirmations_ReservationConfirmationTypeId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "IX_ReservationConfirmations_StatusId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsConfirmedBySupervisor",
                schema: "General",
                table: "ReservationConfirmations",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<Guid>(
                name: "OperationTypeId",
                schema: "General",
                table: "ReservationConfirmations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ReservationConfirmationStatus",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationConfirmationStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationConfirmations_OperationTypeId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "OperationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationConfirmations_ReservationConfirmationStatus_StatusId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "StatusId",
                principalSchema: "General",
                principalTable: "ReservationConfirmationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationConfirmations_ReservationConfirmationTypes_OperationTypeId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "OperationTypeId",
                principalSchema: "General",
                principalTable: "ReservationConfirmationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
