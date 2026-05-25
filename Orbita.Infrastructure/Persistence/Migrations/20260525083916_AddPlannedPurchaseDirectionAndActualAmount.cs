using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlannedPurchaseDirectionAndActualAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ActualAmount",
                table: "PlannedPurchases",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Direction",
                table: "PlannedPurchases",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PlannedPurchases_TeamId_Direction",
                table: "PlannedPurchases",
                columns: new[] { "TeamId", "Direction" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlannedPurchases_TeamId_Direction",
                table: "PlannedPurchases");

            migrationBuilder.DropColumn(
                name: "ActualAmount",
                table: "PlannedPurchases");

            migrationBuilder.DropColumn(
                name: "Direction",
                table: "PlannedPurchases");
        }
    }
}
