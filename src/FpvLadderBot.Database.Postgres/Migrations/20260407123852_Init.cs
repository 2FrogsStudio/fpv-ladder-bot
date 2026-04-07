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
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case_insensitive", "en-u-ks-primary,en-u-ks-primary,icu,False");

            migrationBuilder.CreateTable(
                name: "BackNavigations",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackNavigations", x => new { x.UserId, x.ChatId });
                });

            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    PilotId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PilotName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RatingValue = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<long>(type: "bigint", nullable: false),
                    LastEventDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.PilotId);
                });

            migrationBuilder.CreateTable(
                name: "PipelineState",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineState", x => new { x.UserId, x.ChatId });
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    PilotId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
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
