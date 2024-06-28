using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurgeryRoomScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRoomTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Code",
                schema: "General",
                table: "Rooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DoctorRooms",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoNezam = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RoomCode = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorRooms", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorRooms",
                schema: "General");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "General",
                table: "Rooms");
        }
    }
}
