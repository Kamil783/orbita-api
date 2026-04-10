using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionToShoppingListItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FinanceTransactionId",
                table: "ShoppingListItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_FinanceTransactionId",
                table: "ShoppingListItems",
                column: "FinanceTransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListItems_FinanceTransactions_FinanceTransactionId",
                table: "ShoppingListItems",
                column: "FinanceTransactionId",
                principalTable: "FinanceTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListItems_FinanceTransactions_FinanceTransactionId",
                table: "ShoppingListItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingListItems_FinanceTransactionId",
                table: "ShoppingListItems");

            migrationBuilder.DropColumn(
                name: "FinanceTransactionId",
                table: "ShoppingListItems");
        }
    }
}
