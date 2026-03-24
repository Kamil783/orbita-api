using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BacklogTaskWeekEntity",
                columns: table => new
                {
                    BacklogTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacklogTaskWeekEntity", x => new { x.BacklogTaskId, x.WeekId });
                    table.ForeignKey(
                        name: "FK_BacklogTaskWeekEntity_BacklogTasks_BacklogTaskId",
                        column: x => x.BacklogTaskId,
                        principalTable: "BacklogTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BacklogTaskWeekEntity_Weeks_WeekId",
                        column: x => x.WeekId,
                        principalTable: "Weeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTaskWeekEntity_WeekId",
                table: "BacklogTaskWeekEntity",
                column: "WeekId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_CreatorId",
                table: "Weeks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_CreatorId_IsArchived",
                table: "Weeks",
                columns: new[] { "CreatorId", "IsArchived" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BacklogTaskWeekEntity");

            migrationBuilder.DropTable(
                name: "Weeks");
        }
    }
}
