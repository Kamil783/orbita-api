using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingListPinnedAndItemOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShoppingListItems_ListId",
                table: "ShoppingListItems");

            migrationBuilder.AddColumn<bool>(
                name: "Pinned",
                table: "ShoppingLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ShoppingListItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_ListId_Order",
                table: "ShoppingListItems",
                columns: new[] { "ListId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShoppingListItems_ListId_Order",
                table: "ShoppingListItems");

            migrationBuilder.DropColumn(
                name: "Pinned",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ShoppingListItems");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_ListId",
                table: "ShoppingListItems",
                column: "ListId");
        }
    }
}
