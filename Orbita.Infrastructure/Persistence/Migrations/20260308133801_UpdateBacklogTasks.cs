using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBacklogTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueTime",
                table: "BacklogTasks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "BacklogTasks",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTasks_DueDate",
                table: "BacklogTasks",
                column: "DueDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BacklogTasks_DueDate",
                table: "BacklogTasks");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "BacklogTasks");

            migrationBuilder.AddColumn<string>(
                name: "DueTime",
                table: "BacklogTasks",
                type: "text",
                nullable: true);
        }
    }
}
