using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReshapePlannedPurchaseAssignee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssigneeId",
                table: "PlannedPurchases",
                newName: "AssigneeUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlannedPurchases_AssigneeId",
                table: "PlannedPurchases",
                newName: "IX_PlannedPurchases_AssigneeUserId");

            migrationBuilder.AddColumn<int>(
                name: "AssigneeKind",
                table: "PlannedPurchases",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlannedPurchases_AssigneeKind",
                table: "PlannedPurchases",
                column: "AssigneeKind");

            // Backfill: все существующие записи с непустым исполнителем — это User (0).
            migrationBuilder.Sql(
                "UPDATE \"PlannedPurchases\" SET \"AssigneeKind\" = 0 WHERE \"AssigneeUserId\" IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlannedPurchases_AssigneeKind",
                table: "PlannedPurchases");

            migrationBuilder.DropColumn(
                name: "AssigneeKind",
                table: "PlannedPurchases");

            migrationBuilder.RenameColumn(
                name: "AssigneeUserId",
                table: "PlannedPurchases",
                newName: "AssigneeId");

            migrationBuilder.RenameIndex(
                name: "IX_PlannedPurchases_AssigneeUserId",
                table: "PlannedPurchases",
                newName: "IX_PlannedPurchases_AssigneeId");
        }
    }
}
