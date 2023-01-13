using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Speak.Telegram.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Chats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChosenCuties_CutiePlayers_ChatId_PlayerUsername",
                schema: "speak",
                table: "ChosenCuties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CutiePlayers",
                schema: "speak",
                table: "CutiePlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChosenCuties",
                schema: "speak",
                table: "ChosenCuties");

            migrationBuilder.DropColumn(
                name: "ChatId",
                schema: "speak",
                table: "CutiePlayers");

            migrationBuilder.DropColumn(
                name: "ChatId",
                schema: "speak",
                table: "ChosenCuties");

            migrationBuilder.AddColumn<int>(
                name: "ChatsTableId",
                schema: "speak",
                table: "CutiePlayers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChatsTableId",
                schema: "speak",
                table: "ChosenCuties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CutiePlayers",
                schema: "speak",
                table: "CutiePlayers",
                columns: new[] { "ChatsTableId", "TelegramUsername" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChosenCuties",
                schema: "speak",
                table: "ChosenCuties",
                columns: new[] { "ChatsTableId", "PlayerUsername", "WhenChosen" });

            migrationBuilder.CreateTable(
                name: "Chats",
                schema: "speak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_TelegramChatId",
                schema: "speak",
                table: "Chats",
                column: "TelegramChatId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChosenCuties_CutiePlayers_ChatsTableId_PlayerUsername",
                schema: "speak",
                table: "ChosenCuties",
                columns: new[] { "ChatsTableId", "PlayerUsername" },
                principalSchema: "speak",
                principalTable: "CutiePlayers",
                principalColumns: new[] { "ChatsTableId", "TelegramUsername" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CutiePlayers_Chats_ChatsTableId",
                schema: "speak",
                table: "CutiePlayers",
                column: "ChatsTableId",
                principalSchema: "speak",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChosenCuties_CutiePlayers_ChatsTableId_PlayerUsername",
                schema: "speak",
                table: "ChosenCuties");

            migrationBuilder.DropForeignKey(
                name: "FK_CutiePlayers_Chats_ChatsTableId",
                schema: "speak",
                table: "CutiePlayers");

            migrationBuilder.DropTable(
                name: "Chats",
                schema: "speak");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CutiePlayers",
                schema: "speak",
                table: "CutiePlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChosenCuties",
                schema: "speak",
                table: "ChosenCuties");

            migrationBuilder.DropColumn(
                name: "ChatsTableId",
                schema: "speak",
                table: "CutiePlayers");

            migrationBuilder.DropColumn(
                name: "ChatsTableId",
                schema: "speak",
                table: "ChosenCuties");

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                schema: "speak",
                table: "CutiePlayers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                schema: "speak",
                table: "ChosenCuties",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CutiePlayers",
                schema: "speak",
                table: "CutiePlayers",
                columns: new[] { "ChatId", "TelegramUsername" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChosenCuties",
                schema: "speak",
                table: "ChosenCuties",
                columns: new[] { "ChatId", "PlayerUsername", "WhenChosen" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChosenCuties_CutiePlayers_ChatId_PlayerUsername",
                schema: "speak",
                table: "ChosenCuties",
                columns: new[] { "ChatId", "PlayerUsername" },
                principalSchema: "speak",
                principalTable: "CutiePlayers",
                principalColumns: new[] { "ChatId", "TelegramUsername" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
