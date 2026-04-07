using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FpvLadderBot.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackNavigations",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    Data = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackNavigations", x => new { x.UserId, x.ChatId });
                });

            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    PilotId = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PilotName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RatingValue = table.Column<uint>(type: "INTEGER", nullable: false),
                    Position = table.Column<uint>(type: "INTEGER", nullable: false),
                    LastEventDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.PilotId);
                });

            migrationBuilder.CreateTable(
                name: "PipelineState",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    Data = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineState", x => new { x.UserId, x.ChatId });
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    PilotId = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.ChatId, x.PilotId });
                    table.ForeignKey(
                        name: "FK_Subscriptions_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "PilotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PilotId",
                table: "Subscriptions",
                column: "PilotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackNavigations");

            migrationBuilder.DropTable(
                name: "PipelineState");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Pilots");
        }
    }
}
