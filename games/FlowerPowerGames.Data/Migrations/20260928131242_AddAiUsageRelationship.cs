using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerPowerGames.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAiUsageRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalRequests = table.Column<int>(type: "int", nullable: false),
                    TotalPromptUsed = table.Column<int>(type: "int", nullable: false),
                    TotalAvailablePrompt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiUsages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiUsages_UserId",
                table: "AiUsages",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiUsages");
        }
    }
}
