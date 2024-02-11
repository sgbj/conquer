using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Conquer.Game.Migrations
{
    /// <inheritdoc />
    public partial class EvenMoreModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynaNpcs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    OwnerType = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    LookFace = table.Column<long>(type: "bigint", nullable: false),
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    Life = table.Column<long>(type: "bigint", nullable: false),
                    Base = table.Column<byte>(type: "tinyint", nullable: false),
                    Sort = table.Column<byte>(type: "tinyint", nullable: false),
                    Defence = table.Column<int>(type: "int", nullable: false),
                    MagicDef = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynaNpcs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Generators",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    BoundX = table.Column<int>(type: "int", nullable: false),
                    BoundY = table.Column<int>(type: "int", nullable: false),
                    BoundCx = table.Column<int>(type: "int", nullable: false),
                    BoundCy = table.Column<int>(type: "int", nullable: false),
                    MaxNpc = table.Column<int>(type: "int", nullable: false),
                    RestSecs = table.Column<int>(type: "int", nullable: false),
                    MaxPerGen = table.Column<int>(type: "int", nullable: false),
                    MonsterTypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generators", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynaNpcs");

            migrationBuilder.DropTable(
                name: "Generators");
        }
    }
}
