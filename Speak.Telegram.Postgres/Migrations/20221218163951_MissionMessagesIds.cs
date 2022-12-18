using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speak.Telegram.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MissionMessagesIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectionMessageId",
                schema: "speak",
                table: "ChosenCuties",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MissionResultMessageId",
                schema: "speak",
                table: "ChosenCuties",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionMessageId",
                schema: "speak",
                table: "ChosenCuties");

            migrationBuilder.DropColumn(
                name: "MissionResultMessageId",
                schema: "speak",
                table: "ChosenCuties");
        }
    }
}
