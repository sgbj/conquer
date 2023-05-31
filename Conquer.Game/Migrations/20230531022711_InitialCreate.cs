using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Conquer.Game.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1000001, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<long>(type: "bigint", nullable: false),
                    Avatar = table.Column<int>(type: "int", nullable: false),
                    Hair = table.Column<int>(type: "int", nullable: false),
                    Money = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Experience = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Strength = table.Column<int>(type: "int", nullable: false),
                    Dexterity = table.Column<int>(type: "int", nullable: false),
                    Vitality = table.Column<int>(type: "int", nullable: false),
                    Mana = table.Column<int>(type: "int", nullable: false),
                    AttributePoints = table.Column<int>(type: "int", nullable: false),
                    Health = table.Column<int>(type: "int", nullable: false),
                    Magic = table.Column<int>(type: "int", nullable: false),
                    PkMode = table.Column<int>(type: "int", nullable: false),
                    PkPoints = table.Column<short>(type: "smallint", nullable: false),
                    Profession = table.Column<int>(type: "int", nullable: false),
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Rebirths = table.Column<byte>(type: "tinyint", nullable: false),
                    SpouseId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Players_SpouseId",
                        column: x => x.SpouseId,
                        principalTable: "Players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_SpouseId",
                table: "Players",
                column: "SpouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
