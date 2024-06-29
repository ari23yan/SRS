using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationRejectionAndChangeSomeFieldsInReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationConfirmationLogs",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "OperationConfirmations",
                schema: "General");

            migrationBuilder.DropTable(
                name: "OperationConfirmationStatus",
                schema: "General");

            migrationBuilder.DropTable(
                name: "OperationConfirmationTypes",
                schema: "General");

            migrationBuilder.DropColumn(
                name: "ConfirmedMedicalRecordsUserId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ConfirmedSupervisorUserId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsConfirmedBySupervisor",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "General",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "IsConfirmedByMedicalRecords",
                schema: "General",
                table: "Reservations",
                newName: "IsCanceled");

            migrationBuilder.AddColumn<string>(
                name: "CancelationDescription",
                schema: "General",
                table: "Reservations",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReservationConfirmationId",
                schema: "General",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ReservationConfirmationStatus",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_ReservationConfirmationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservationConfirmationTypes",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_ReservationConfirmationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservationRejectionReasons",
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
                    table.PrimaryKey("PK_ReservationRejectionReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservationConfirmationLogs",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfirmationAction = table.Column<byte>(type: "tinyint", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationConfirmationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ReservationConfirmationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationConfirmationLogs_ReservationConfirmationTypes_ReservationConfirmationTypeId",
                        column: x => x.ReservationConfirmationTypeId,
                        principalSchema: "General",
                        principalTable: "ReservationConfirmationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservationRejections",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationRejectionReasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdditionalDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
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
                    table.PrimaryKey("PK_ReservationRejections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationRejections_ReservationRejectionReasons_ReservationRejectionReasonId",
                        column: x => x.ReservationRejectionReasonId,
                        principalSchema: "General",
                        principalTable: "ReservationRejectionReasons",
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

            migrationBuilder.CreateTable(
                name: "ReservationConfirmations",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsConfirmedByMedicalRecords = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedMedicalRecordsUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsConfirmedBySupervisor = table.Column<bool>(type: "bit", nullable: true),
                    ConfirmedSupervisorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReservationRejectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_ReservationConfirmations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationConfirmations_ReservationConfirmationStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "General",
                        principalTable: "ReservationConfirmationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationConfirmations_ReservationConfirmationTypes_OperationTypeId",
                        column: x => x.OperationTypeId,
                        principalSchema: "General",
                        principalTable: "ReservationConfirmationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationConfirmations_ReservationRejections_ReservationRejectionId",
                        column: x => x.ReservationRejectionId,
                        principalSchema: "General",
                        principalTable: "ReservationRejections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReservationConfirmations_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "General",
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationConfirmationLogs_ReservationConfirmationTypeId",
                schema: "Common",
                table: "ReservationConfirmationLogs",
                column: "ReservationConfirmationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationConfirmations_OperationTypeId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "OperationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationConfirmations_ReservationId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "ReservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservationConfirmations_ReservationRejectionId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "ReservationRejectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationConfirmations_StatusId",
                schema: "General",
                table: "ReservationConfirmations",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRejections_ReservationId",
                schema: "General",
                table: "ReservationRejections",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRejections_ReservationRejectionReasonId",
                schema: "General",
                table: "ReservationRejections",
                column: "ReservationRejectionReasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationConfirmationLogs",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "ReservationConfirmations",
                schema: "General");

            migrationBuilder.DropTable(
                name: "ReservationConfirmationStatus",
                schema: "General");

            migrationBuilder.DropTable(
                name: "ReservationConfirmationTypes",
                schema: "General");

            migrationBuilder.DropTable(
                name: "ReservationRejections",
                schema: "General");

            migrationBuilder.DropTable(
                name: "ReservationRejectionReasons",
                schema: "General");

            migrationBuilder.DropColumn(
                name: "CancelationDescription",
                schema: "General",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationConfirmationId",
                schema: "General",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "IsCanceled",
                schema: "General",
                table: "Reservations",
                newName: "IsConfirmedByMedicalRecords");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmedBySupervisor",
                schema: "General",
                table: "Reservations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                schema: "General",
                table: "Reservations",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "OperationConfirmationStatus",
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
                    table.PrimaryKey("PK_OperationConfirmationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationConfirmationTypes",
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
                    table.PrimaryKey("PK_OperationConfirmationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationConfirmationLogs",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfirmedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OperationConfirmationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationConfirmationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationConfirmationLogs_OperationConfirmationStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "General",
                        principalTable: "OperationConfirmationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationConfirmations",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfirmationDetails = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationConfirmations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationConfirmations_OperationConfirmationStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "General",
                        principalTable: "OperationConfirmationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationConfirmations_OperationConfirmationTypes_OperationTypeId",
                        column: x => x.OperationTypeId,
                        principalSchema: "General",
                        principalTable: "OperationConfirmationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationConfirmationLogs_StatusId",
                schema: "Common",
                table: "OperationConfirmationLogs",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationConfirmations_OperationTypeId",
                schema: "General",
                table: "OperationConfirmations",
                column: "OperationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationConfirmations_StatusId",
                schema: "General",
                table: "OperationConfirmations",
                column: "StatusId");
        }
    }
}
