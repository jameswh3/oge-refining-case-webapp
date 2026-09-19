using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Oge.Refining.CaseApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: false),
                    RefineryName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ProcessUnitName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegulatoryNotifiable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseTimelineEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTimelineEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseTimelineEntries_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cases",
                columns: new[] { "Id", "CaseNumber", "Description", "ProcessUnitName", "RefineryName", "RegulatoryNotifiable", "ReportedAt", "Severity", "Status", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "SYN-2026-0017", "A synthetic pressure excursion triggered an automatic unit response. No personnel exposure occurred.", "Crude Distillation Unit 1", "Northstar Demonstration Refinery", true, new DateTime(2026, 8, 6, 14, 35, 0, 0, DateTimeKind.Utc), "Critical", "Open", "Crude tower overhead pressure excursion", "Incident", new DateTime(2026, 8, 6, 17, 35, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "SYN-2026-0016", "A synthetic procedure review identified an incomplete temperature-equilibration record.", "Fluid Catalytic Cracker 2", "Northstar Demonstration Refinery", false, new DateTime(2026, 8, 5, 10, 20, 0, 0, DateTimeKind.Utc), "Moderate", "InProgress", "Catalyst loading procedure deviation", "NonConformance", new DateTime(2026, 8, 5, 13, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "SYN-2026-0012", "A fictional field observation was corrected immediately during a routine inspection.", "Hydrotreater A", "Northstar Demonstration Refinery", false, new DateTime(2026, 7, 28, 11, 45, 0, 0, DateTimeKind.Utc), "Minor", "Closed", "Inspection eyewear observation", "SafetyObservation", new DateTime(2026, 7, 28, 14, 45, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "SYN-2026-0009", "Synthetic vibration readings exceeded the fictional review threshold pending a planned inspection.", "Utilities Area", "Northstar Demonstration Refinery", false, new DateTime(2026, 7, 21, 8, 10, 0, 0, DateTimeKind.Utc), "Major", "OnHold", "Cooling-water pump vibration trend", "MaintenanceIssue", new DateTime(2026, 7, 21, 11, 10, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "CaseTimelineEntries",
                columns: new[] { "Id", "AuthorName", "CaseId", "Content", "EntryType", "OccurredAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Synthetic Reporter", new Guid("11111111-1111-1111-1111-111111111111"), "Case created from fictional demonstration data.", "Reported", new DateTime(2026, 8, 6, 14, 35, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Synthetic Review Team", new Guid("11111111-1111-1111-1111-111111111111"), "Initial review completed for demonstration purposes.", "Review", new DateTime(2026, 8, 6, 17, 35, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Synthetic Reporter", new Guid("22222222-2222-2222-2222-222222222222"), "Case created from fictional demonstration data.", "Reported", new DateTime(2026, 8, 5, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Synthetic Review Team", new Guid("22222222-2222-2222-2222-222222222222"), "Initial review completed for demonstration purposes.", "Review", new DateTime(2026, 8, 5, 13, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000001"), "Synthetic Reporter", new Guid("33333333-3333-3333-3333-333333333333"), "Case created from fictional demonstration data.", "Reported", new DateTime(2026, 7, 28, 11, 45, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "Synthetic Review Team", new Guid("33333333-3333-3333-3333-333333333333"), "Initial review completed for demonstration purposes.", "Review", new DateTime(2026, 7, 28, 14, 45, 0, 0, DateTimeKind.Utc) },
                    { new Guid("40000000-0000-0000-0000-000000000001"), "Synthetic Reporter", new Guid("44444444-4444-4444-4444-444444444444"), "Case created from fictional demonstration data.", "Reported", new DateTime(2026, 7, 21, 8, 10, 0, 0, DateTimeKind.Utc) },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "Synthetic Review Team", new Guid("44444444-4444-4444-4444-444444444444"), "Initial review completed for demonstration purposes.", "Review", new DateTime(2026, 7, 21, 11, 10, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CaseNumber",
                table: "Cases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseTimelineEntries_CaseId",
                table: "CaseTimelineEntries",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseTimelineEntries");

            migrationBuilder.DropTable(
                name: "Cases");
        }
    }
}
