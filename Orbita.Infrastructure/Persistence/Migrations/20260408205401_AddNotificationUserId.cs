using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppNotifications_CreatedAt",
                table: "AppNotifications");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AppNotifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppNotifications_UserId_CreatedAt",
                table: "AppNotifications",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppNotifications_UserId_CreatedAt",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppNotifications");

            migrationBuilder.CreateIndex(
                name: "IX_AppNotifications_CreatedAt",
                table: "AppNotifications",
                column: "CreatedAt");
        }
    }
}
