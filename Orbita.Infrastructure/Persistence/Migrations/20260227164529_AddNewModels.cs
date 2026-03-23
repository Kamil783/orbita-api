using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Read = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BacklogTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InWeek = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    AssigneeId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueTime = table.Column<string>(type: "text", nullable: true),
                    EstimateMinutes = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacklogTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Columns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    TotalCount = table.Column<int>(type: "integer", nullable: false),
                    HeaderActionIcon = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Muted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TaskStatus = table.Column<int>(type: "integer", nullable: false),
                    TaskPriority = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColumnId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssigneeId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeadlineUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProgressPct = table.Column<int>(type: "integer", nullable: true),
                    BacklogId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeadlineText = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CompletedText = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItems_Columns_ColumnId",
                        column: x => x.ColumnId,
                        principalTable: "Columns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Location = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    GoogleEventId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_TodoItems_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TodoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppNotifications_CreatedAt",
                table: "AppNotifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTasks_AssigneeId",
                table: "BacklogTasks",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTasks_CreatorId",
                table: "BacklogTasks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BacklogTasks_IsCompleted_InWeek",
                table: "BacklogTasks",
                columns: new[] { "IsCompleted", "InWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_Date",
                table: "CalendarEvents",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_TaskId",
                table: "CalendarEvents",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_Type",
                table: "CalendarEvents",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_Status",
                table: "Columns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_AssigneeId",
                table: "TodoItems",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ColumnId",
                table: "TodoItems",
                column: "ColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_CreatorId",
                table: "TodoItems",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_DeadlineUtc",
                table: "TodoItems",
                column: "DeadlineUtc");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TaskStatus",
                table: "TodoItems",
                column: "TaskStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppNotifications");

            migrationBuilder.DropTable(
                name: "BacklogTasks");

            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "TodoItems");

            migrationBuilder.DropTable(
                name: "Columns");
        }
    }
}
