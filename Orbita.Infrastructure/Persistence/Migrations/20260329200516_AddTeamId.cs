using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Weeks_CreatorId",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_CreatorId_IsArchived",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_CreatorId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_SavingsGoals_CreatorId",
                table: "SavingsGoals");

            migrationBuilder.DropIndex(
                name: "IX_FinanceTransactions_CreatorId",
                table: "FinanceTransactions");

            migrationBuilder.DropIndex(
                name: "IX_FinanceTransactions_CreatorId_CreatedAt",
                table: "FinanceTransactions");

            migrationBuilder.DropIndex(
                name: "IX_FinanceCategories_CreatorId",
                table: "FinanceCategories");

            migrationBuilder.DropIndex(
                name: "IX_Columns_CreatorId",
                table: "Columns");

            migrationBuilder.DropIndex(
                name: "IX_BacklogTasks_CreatorId",
                table: "BacklogTasks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SpendingLimits",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "FinanceBalances",
                newName: "TeamId");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Weeks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "TodoItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Teams",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "SavingsGoals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "FinanceTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "FinanceCategories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Columns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "BacklogTasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_TeamId",
                table: "Weeks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_TeamId_IsArchived",
                table: "Weeks",
                columns: new[] { "TeamId", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoals_TeamId",
                table: "SavingsGoals",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceTransactions_TeamId",
                table: "FinanceTransactions",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceTransactions_TeamId_CreatedAt",
                table: "FinanceTransactions",
                columns: new[] { "TeamId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FinanceCategories_TeamId",
                table: "FinanceCategories",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_TeamId",
                table: "Columns",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTasks_TeamId",
                table: "BacklogTasks",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Weeks_TeamId",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_TeamId_IsArchived",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_SavingsGoals_TeamId",
                table: "SavingsGoals");

            migrationBuilder.DropIndex(
                name: "IX_FinanceTransactions_TeamId",
                table: "FinanceTransactions");

            migrationBuilder.DropIndex(
                name: "IX_FinanceTransactions_TeamId_CreatedAt",
                table: "FinanceTransactions");

            migrationBuilder.DropIndex(
                name: "IX_FinanceCategories_TeamId",
                table: "FinanceCategories");

            migrationBuilder.DropIndex(
                name: "IX_Columns_TeamId",
                table: "Columns");

            migrationBuilder.DropIndex(
                name: "IX_BacklogTasks_TeamId",
                table: "BacklogTasks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Weeks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "SavingsGoals");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "FinanceTransactions");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "FinanceCategories");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Columns");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "BacklogTasks");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "SpendingLimits",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "FinanceBalances",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_CreatorId",
                table: "Weeks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_CreatorId_IsArchived",
                table: "Weeks",
                columns: new[] { "CreatorId", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_CreatorId",
                table: "TodoItems",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoals_CreatorId",
                table: "SavingsGoals",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceTransactions_CreatorId",
                table: "FinanceTransactions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceTransactions_CreatorId_CreatedAt",
                table: "FinanceTransactions",
                columns: new[] { "CreatorId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FinanceCategories_CreatorId",
                table: "FinanceCategories",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_CreatorId",
                table: "Columns",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTasks_CreatorId",
                table: "BacklogTasks",
                column: "CreatorId");
        }
    }
}
