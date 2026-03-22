using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnCreatorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Columns",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Columns_CreatorId",
                table: "Columns",
                column: "CreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Columns_CreatorId",
                table: "Columns");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Columns");
        }
    }
}
