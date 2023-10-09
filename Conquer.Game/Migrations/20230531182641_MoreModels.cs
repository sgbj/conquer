using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Conquer.Game.Migrations
{
    /// <inheritdoc />
    public partial class MoreModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemAdditions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Life = table.Column<short>(type: "smallint", nullable: false),
                    AttackMax = table.Column<short>(type: "smallint", nullable: false),
                    AttackMin = table.Column<short>(type: "smallint", nullable: false),
                    Defense = table.Column<short>(type: "smallint", nullable: false),
                    MagicAtk = table.Column<short>(type: "smallint", nullable: false),
                    MagicDef = table.Column<short>(type: "smallint", nullable: false),
                    Dexterity = table.Column<short>(type: "smallint", nullable: false),
                    Dodge = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAdditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqProfession = table.Column<byte>(type: "tinyint", nullable: false),
                    ReqWeaponSkill = table.Column<byte>(type: "tinyint", nullable: false),
                    ReqLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    ReqSex = table.Column<byte>(type: "tinyint", nullable: false),
                    ReqForce = table.Column<int>(type: "int", nullable: false),
                    ReqSpeed = table.Column<int>(type: "int", nullable: false),
                    ReqHealth = table.Column<int>(type: "int", nullable: false),
                    ReqSoul = table.Column<int>(type: "int", nullable: false),
                    Monopoly = table.Column<byte>(type: "tinyint", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    Task = table.Column<int>(type: "int", nullable: false),
                    AttackMax = table.Column<int>(type: "int", nullable: false),
                    AttackMin = table.Column<int>(type: "int", nullable: false),
                    Defense = table.Column<short>(type: "smallint", nullable: false),
                    Dexterity = table.Column<short>(type: "smallint", nullable: false),
                    Dodge = table.Column<short>(type: "smallint", nullable: false),
                    Life = table.Column<short>(type: "smallint", nullable: false),
                    Mana = table.Column<short>(type: "smallint", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AmountLimit = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Gem1 = table.Column<byte>(type: "tinyint", nullable: false),
                    Gem2 = table.Column<byte>(type: "tinyint", nullable: false),
                    Magic1 = table.Column<byte>(type: "tinyint", nullable: false),
                    Magic2 = table.Column<byte>(type: "tinyint", nullable: false),
                    Magic3 = table.Column<byte>(type: "tinyint", nullable: false),
                    MagicAtk = table.Column<int>(type: "int", nullable: false),
                    MagicDef = table.Column<int>(type: "int", nullable: false),
                    AtkSpeed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LevelExps",
                columns: table => new
                {
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelExps", x => x.Level);
                });

            migrationBuilder.CreateTable(
                name: "MagicTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Sort = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Crime = table.Column<byte>(type: "tinyint", nullable: false),
                    Ground = table.Column<byte>(type: "tinyint", nullable: false),
                    Multi = table.Column<byte>(type: "tinyint", nullable: false),
                    Target = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    UseMp = table.Column<int>(type: "int", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    IntoneDuration = table.Column<int>(type: "int", nullable: false),
                    Success = table.Column<byte>(type: "tinyint", nullable: false),
                    StepSecs = table.Column<long>(type: "bigint", nullable: false),
                    Range = table.Column<byte>(type: "tinyint", nullable: false),
                    Distance = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<long>(type: "bigint", nullable: false),
                    RequiredProf = table.Column<int>(type: "int", nullable: false),
                    RequiredExp = table.Column<long>(type: "bigint", nullable: false),
                    RequiredLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    UseXp = table.Column<byte>(type: "tinyint", nullable: false),
                    WeaponSubtype = table.Column<int>(type: "int", nullable: false),
                    ActiveTimes = table.Column<long>(type: "bigint", nullable: false),
                    AutoActive = table.Column<byte>(type: "tinyint", nullable: false),
                    FloorAttr = table.Column<long>(type: "bigint", nullable: false),
                    AutoLearn = table.Column<byte>(type: "tinyint", nullable: false),
                    LearnLevel = table.Column<int>(type: "int", nullable: false),
                    DropWeapon = table.Column<byte>(type: "tinyint", nullable: false),
                    UseEp = table.Column<byte>(type: "tinyint", nullable: false),
                    WeaponHit = table.Column<byte>(type: "tinyint", nullable: false),
                    UseItem = table.Column<long>(type: "bigint", nullable: false),
                    NextMagic = table.Column<int>(type: "int", nullable: false),
                    NextMagicDelay = table.Column<int>(type: "int", nullable: false),
                    UseItemNum = table.Column<byte>(type: "tinyint", nullable: false),
                    SenderAction = table.Column<int>(type: "int", nullable: false),
                    UpgradeDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IntoneEffect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IntoneSound = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderEffect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderSound = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetDelay = table.Column<long>(type: "bigint", nullable: false),
                    TargetEffect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetSound = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroundEffect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TraceEffect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScreenRepresent = table.Column<long>(type: "bigint", nullable: false),
                    UsableInMarket = table.Column<byte>(type: "tinyint", nullable: false),
                    TargetWoundDelay = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescribeText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Weather = table.Column<long>(type: "bigint", nullable: false),
                    BgMusic = table.Column<long>(type: "bigint", nullable: false),
                    BgMusicShow = table.Column<long>(type: "bigint", nullable: false),
                    PortalX = table.Column<int>(type: "int", nullable: false),
                    PortalY = table.Column<int>(type: "int", nullable: false),
                    RebornMap = table.Column<long>(type: "bigint", nullable: false),
                    RebornPortal = table.Column<long>(type: "bigint", nullable: false),
                    Light = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonsterTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    AiType = table.Column<byte>(type: "tinyint", nullable: false),
                    Look = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Life = table.Column<int>(type: "int", nullable: false),
                    EscapeLife = table.Column<int>(type: "int", nullable: false),
                    AttackUser = table.Column<byte>(type: "tinyint", nullable: false),
                    AttackMin = table.Column<long>(type: "bigint", nullable: false),
                    AttackMax = table.Column<long>(type: "bigint", nullable: false),
                    Defense = table.Column<long>(type: "bigint", nullable: false),
                    Dexterity = table.Column<byte>(type: "tinyint", nullable: false),
                    Dodge = table.Column<byte>(type: "tinyint", nullable: false),
                    MagicType = table.Column<long>(type: "bigint", nullable: false),
                    MagicDef = table.Column<long>(type: "bigint", nullable: false),
                    MagicHitRate = table.Column<long>(type: "bigint", nullable: false),
                    ViewRange = table.Column<byte>(type: "tinyint", nullable: false),
                    AttackRange = table.Column<byte>(type: "tinyint", nullable: false),
                    AttackSpeed = table.Column<int>(type: "int", nullable: false),
                    MoveSpeed = table.Column<int>(type: "int", nullable: false),
                    RunSpeed = table.Column<int>(type: "int", nullable: false),
                    DropArmet = table.Column<byte>(type: "tinyint", nullable: false),
                    DropNecklace = table.Column<byte>(type: "tinyint", nullable: false),
                    DropArmor = table.Column<byte>(type: "tinyint", nullable: false),
                    DropRing = table.Column<byte>(type: "tinyint", nullable: false),
                    DropWeapon = table.Column<byte>(type: "tinyint", nullable: false),
                    DropShield = table.Column<byte>(type: "tinyint", nullable: false),
                    DropShoes = table.Column<byte>(type: "tinyint", nullable: false),
                    DropMoney = table.Column<long>(type: "bigint", nullable: false),
                    DropHp = table.Column<long>(type: "bigint", nullable: false),
                    DropMp = table.Column<long>(type: "bigint", nullable: false),
                    ExtraExp = table.Column<int>(type: "int", nullable: false),
                    ExtraDamage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Npcs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    LookFace = table.Column<long>(type: "bigint", nullable: false),
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    CellX = table.Column<int>(type: "int", nullable: false),
                    CellY = table.Column<int>(type: "int", nullable: false),
                    Base = table.Column<byte>(type: "tinyint", nullable: false),
                    Sort = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Npcs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passways",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    PasswayIdx = table.Column<long>(type: "bigint", nullable: false),
                    PortalMapId = table.Column<int>(type: "int", nullable: false),
                    PortalIdx = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passways", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointAllots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Profession = table.Column<byte>(type: "tinyint", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Force = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    Health = table.Column<int>(type: "int", nullable: false),
                    Soul = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointAllots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Portals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    Idx = table.Column<long>(type: "bigint", nullable: false),
                    PortalX = table.Column<int>(type: "int", nullable: false),
                    PortalY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeaponSkillExps",
                columns: table => new
                {
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Experience = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponSkillExps", x => x.Level);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAdditions");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "LevelExps");

            migrationBuilder.DropTable(
                name: "MagicTypes");

            migrationBuilder.DropTable(
                name: "Maps");

            migrationBuilder.DropTable(
                name: "MonsterTypes");

            migrationBuilder.DropTable(
                name: "Npcs");

            migrationBuilder.DropTable(
                name: "Passways");

            migrationBuilder.DropTable(
                name: "PointAllots");

            migrationBuilder.DropTable(
                name: "Portals");

            migrationBuilder.DropTable(
                name: "WeaponSkillExps");
        }
    }
}
