using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Conquer.Game.Migrations
{
    /// <inheritdoc />
    public partial class Item : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CellY",
                table: "Npcs",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "CellX",
                table: "Npcs",
                newName: "X");

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTypeId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AmountLimit = table.Column<int>(type: "int", nullable: false),
                    Ident = table.Column<byte>(type: "tinyint", nullable: false),
                    Position = table.Column<byte>(type: "tinyint", nullable: false),
                    Gem1 = table.Column<byte>(type: "tinyint", nullable: false),
                    Gem2 = table.Column<byte>(type: "tinyint", nullable: false),
                    Magic1 = table.Column<byte>(type: "tinyint", nullable: false),
                    Magic2 = table.Column<byte>(type: "tinyint", nullable: false),
                    Magic3 = table.Column<byte>(type: "tinyint", nullable: false),
                    Restrain = table.Column<long>(type: "bigint", nullable: false),
                    Bless = table.Column<byte>(type: "tinyint", nullable: false),
                    Enchant = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_PlayerId",
                table: "Item",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.RenameColumn(
                name: "Y",
                table: "Npcs",
                newName: "CellY");

            migrationBuilder.RenameColumn(
                name: "X",
                table: "Npcs",
                newName: "CellX");
        }
    }
}
