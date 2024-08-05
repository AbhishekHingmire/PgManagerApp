using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PgManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class Init23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasHotWater = table.Column<bool>(type: "bit", nullable: true),
                    HasShower = table.Column<bool>(type: "bit", nullable: true),
                    HasCommode = table.Column<bool>(type: "bit", nullable: true),
                    HasWiFi = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
