using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Speak.Telegram.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "speak");

            migrationBuilder.CreateTable(
                name: "CutieMissions",
                schema: "speak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CutieMissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CutiePlayers",
                schema: "speak",
                columns: table => new
                {
                    TelegramUsername = table.Column<string>(type: "text", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CutiePlayers", x => new { x.ChatId, x.TelegramUsername });
                });

            migrationBuilder.CreateTable(
                name: "CutieThinkingPhrases",
                schema: "speak",
                columns: table => new
                {
                    Phrase = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ChosenCuties",
                schema: "speak",
                columns: table => new
                {
                    WhenChosen = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerUsername = table.Column<string>(type: "text", nullable: false),
                    MissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChosenCuties", x => new { x.ChatId, x.PlayerUsername, x.WhenChosen });
                    table.ForeignKey(
                        name: "FK_ChosenCuties_CutieMissions_MissionId",
                        column: x => x.MissionId,
                        principalSchema: "speak",
                        principalTable: "CutieMissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChosenCuties_CutiePlayers_ChatId_PlayerUsername",
                        columns: x => new { x.ChatId, x.PlayerUsername },
                        principalSchema: "speak",
                        principalTable: "CutiePlayers",
                        principalColumns: new[] { "ChatId", "TelegramUsername" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChosenCuties_MissionId",
                schema: "speak",
                table: "ChosenCuties",
                column: "MissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChosenCuties",
                schema: "speak");

            migrationBuilder.DropTable(
                name: "CutieThinkingPhrases",
                schema: "speak");

            migrationBuilder.DropTable(
                name: "CutieMissions",
                schema: "speak");

            migrationBuilder.DropTable(
                name: "CutiePlayers",
                schema: "speak");
        }
    }
}
