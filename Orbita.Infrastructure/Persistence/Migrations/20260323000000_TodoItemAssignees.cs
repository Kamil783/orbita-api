using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TodoItemAssignees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_AssigneeId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "TodoItems");

            migrationBuilder.CreateTable(
                name: "TodoItemAssigneeEntity",
                columns: table => new
                {
                    TodoItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItemAssigneeEntity", x => new { x.TodoItemId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TodoItemAssigneeEntity_TodoItems_TodoItemId",
                        column: x => x.TodoItemId,
                        principalTable: "TodoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TodoItemAssigneeEntity_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItemAssigneeEntity_UserId",
                table: "TodoItemAssigneeEntity",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItemAssigneeEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "TodoItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_AssigneeId",
                table: "TodoItems",
                column: "AssigneeId");
        }
    }
}
