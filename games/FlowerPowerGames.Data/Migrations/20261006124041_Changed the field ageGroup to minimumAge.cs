using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerPowerGames.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedthefieldageGrouptominimumAge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ageGroup",
                table: "UserPreferences");

            migrationBuilder.AddColumn<int>(
                name: "MinimumAge",
                table: "UserPreferences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumAge",
                table: "UserPreferences");

            migrationBuilder.AddColumn<string>(
                name: "ageGroup",
                table: "UserPreferences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
