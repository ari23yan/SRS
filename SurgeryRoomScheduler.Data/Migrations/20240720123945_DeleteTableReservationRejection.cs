using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTableReservationRejection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationConfirmations_ReservationRejections_ReservationRejectionId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropTable(
                name: "ReservationRejections",
                schema: "General");

            migrationBuilder.DropColumn(
                name: "CancelationDescription",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsCanceled",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationCancelationReasonId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "ReservationRejectionId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "ReservationRejectionAndCancellationReasonId");

            migrationBuilder.RenameColumn(
                name: "RejectionUserId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "RejectionAndCancellationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationConfirmations_ReservationRejectionId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "IX_ReservationConfirmations_ReservationRejectionAndCancellationReasonId");

            migrationBuilder.AddColumn<string>(
                name: "RejectionAndCancellationAdditionalDescription",
                schema: "General",
                table: "ReservationConfirmations",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationConfirmations_ReservationRejectionAndCancellationReasons_ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "ReservationRejectionAndCancellationReasonId",
                principalSchema: "General",
                principalTable: "ReservationRejectionAndCancellationReasons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationConfirmations_ReservationRejectionAndCancellationReasons_ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.DropColumn(
                name: "RejectionAndCancellationAdditionalDescription",
                schema: "General",
                table: "ReservationConfirmations");

            migrationBuilder.RenameColumn(
                name: "ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "ReservationRejectionId");

            migrationBuilder.RenameColumn(
                name: "RejectionAndCancellationUserId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "RejectionUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationConfirmations_ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationConfirmations",
                newName: "IX_ReservationConfirmations_ReservationRejectionId");

            migrationBuilder.AddColumn<string>(
                name: "CancelationDescription",
                schema: "General",
                table: "Reservations",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCanceled",
                schema: "General",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ReservationCancelationReasonId",
                schema: "General",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationRejections",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationRejectionAndCancellationReasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdditionalDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReservationRejectionReasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationRejections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationRejections_ReservationRejectionAndCancellationReasons_ReservationRejectionAndCancellationReasonId",
                        column: x => x.ReservationRejectionAndCancellationReasonId,
                        principalSchema: "General",
                        principalTable: "ReservationRejectionAndCancellationReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationRejections_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "General",
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRejections_ReservationId",
                schema: "General",
                table: "ReservationRejections",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRejections_ReservationRejectionAndCancellationReasonId",
                schema: "General",
                table: "ReservationRejections",
                column: "ReservationRejectionAndCancellationReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationConfirmations_ReservationRejections_ReservationRejectionId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "ReservationRejectionId",
                principalSchema: "General",
                principalTable: "ReservationRejections",
                principalColumn: "Id");
        }
    }
}
