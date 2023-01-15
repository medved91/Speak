using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speak.Telegram.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MusicQuizRounds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MusicQuizRounds",
                schema: "speak",
                columns: table => new
                {
                    PlayerUsername = table.Column<string>(type: "text", nullable: false),
                    RoundMessageId = table.Column<int>(type: "integer", nullable: false),
                    ChatsTableId = table.Column<int>(type: "integer", nullable: false),
                    AnsweredCorrectly = table.Column<bool>(type: "boolean", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Artist = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicQuizRounds", x => new { x.ChatsTableId, x.PlayerUsername, x.RoundMessageId });
                    table.ForeignKey(
                        name: "FK_MusicQuizRounds_Chats_ChatsTableId",
                        column: x => x.ChatsTableId,
                        principalSchema: "speak",
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicQuizRounds",
                schema: "speak");
        }
    }
}
