using Microsoft.EntityFrameworkCore.Migrations;

namespace FacialRecognitionTimekeepingAPI.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimekeepingPeople",
                columns: table => new
                {
                    AliasId = table.Column<string>(nullable: false),
                    CognitivePersonId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimekeepingPeople", x => x.AliasId);
                });

            migrationBuilder.CreateTable(
                name: "TimekeepingRecords",
                columns: table => new
                {
                    AliasId = table.Column<string>(nullable: false),
                    TimekeepingRecordUnixTimestampSeconds = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimekeepingRecords", x => new { x.AliasId, x.TimekeepingRecordUnixTimestampSeconds });
                    table.ForeignKey(
                        name: "FK_TimekeepingRecords_TimekeepingPeople_AliasId",
                        column: x => x.AliasId,
                        principalTable: "TimekeepingPeople",
                        principalColumn: "AliasId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimekeepingRecords");

            migrationBuilder.DropTable(
                name: "TimekeepingPeople");
        }
    }
}
