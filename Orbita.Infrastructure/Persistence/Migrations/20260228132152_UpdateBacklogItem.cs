using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBacklogItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BacklogTasks_AssigneeId",
                table: "BacklogTasks");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "BacklogTasks");

            migrationBuilder.CreateTable(
                name: "BacklogTaskAssigneeEntity",
                columns: table => new
                {
                    BacklogTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacklogTaskAssigneeEntity", x => new { x.BacklogTaskId, x.UserId });
                    table.ForeignKey(
                        name: "FK_BacklogTaskAssigneeEntity_BacklogTasks_BacklogTaskId",
                        column: x => x.BacklogTaskId,
                        principalTable: "BacklogTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BacklogTaskAssigneeEntity_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTaskAssigneeEntity_UserId",
                table: "BacklogTaskAssigneeEntity",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BacklogTaskAssigneeEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "BacklogTasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTasks_AssigneeId",
                table: "BacklogTasks",
                column: "AssigneeId");
        }
    }
}
