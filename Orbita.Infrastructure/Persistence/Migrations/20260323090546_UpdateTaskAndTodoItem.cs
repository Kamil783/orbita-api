using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskAndTodoItem : Migration
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

            migrationBuilder.DropColumn(
                name: "DeadlineText",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "DueTime",
                table: "BacklogTasks");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "TodoItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Columns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "BacklogTasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgressPct",
                table: "BacklogTasks",
                type: "integer",
                nullable: true);

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
                name: "IX_TodoItems_ColumnId_SortOrder",
                table: "TodoItems",
                columns: new[] { "ColumnId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Columns_CreatorId",
                table: "Columns",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTasks_DueDate",
                table: "BacklogTasks",
                column: "DueDate");

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

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_ColumnId_SortOrder",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_Columns_CreatorId",
                table: "Columns");

            migrationBuilder.DropIndex(
                name: "IX_BacklogTasks_DueDate",
                table: "BacklogTasks");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Columns");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "BacklogTasks");

            migrationBuilder.DropColumn(
                name: "ProgressPct",
                table: "BacklogTasks");

            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "TodoItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeadlineText",
                table: "TodoItems",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DueTime",
                table: "BacklogTasks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_AssigneeId",
                table: "TodoItems",
                column: "AssigneeId");
        }
    }
}
