using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helper.ServiceGateways.Migrations
{
    /// <inheritdoc />
    public partial class images : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageQueries",
                columns: table => new
                {
                    ImageQueryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageQueryText = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageQueries", x => x.ImageQueryId);
                });

            migrationBuilder.CreateTable(
                name: "ImageResults",
                columns: table => new
                {
                    ImageResultId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageQueryId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageBlob = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageResults", x => x.ImageResultId);
                    table.ForeignKey(
                        name: "FK_ImageResults_ImageQueries_ImageQueryId",
                        column: x => x.ImageQueryId,
                        principalTable: "ImageQueries",
                        principalColumn: "ImageQueryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageResults_ImageQueryId",
                table: "ImageResults",
                column: "ImageQueryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageResults");

            migrationBuilder.DropTable(
                name: "ImageQueries");
        }
    }
}
