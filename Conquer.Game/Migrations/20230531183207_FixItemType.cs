using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Conquer.Game.Migrations
{
    /// <inheritdoc />
    public partial class FixItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AtkRange",
                table: "ItemTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AtkRange",
                table: "ItemTypes");
        }
    }
}
