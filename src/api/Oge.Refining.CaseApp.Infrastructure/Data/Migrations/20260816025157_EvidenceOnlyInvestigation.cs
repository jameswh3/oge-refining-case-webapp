using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oge.Refining.CaseApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EvidenceOnlyInvestigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Maintenance Planner", "Procedure MNT-14 requires a documented risk review, supervisor approval, expiry date, and compensating monitoring when equipment remains in service above the 7.1 mm/s vibration threshold. The SYN-2026-0009 attachment register contained no approval or monitoring plan, and the scheduled 2026-08-04 reading was blank.", "ProcedureReview", new DateTime(2026, 8, 7, 11, 40, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Control Systems Engineer", "Work order WO-4821 recorded P-204A seal testing on 2026-08-05. The DCS audit log shows its selector changed from Auto to Manual at 11:20 UTC; the work order closed at 12:05 UTC, with no later selector change before the incident.", "ConfigurationHistory", new DateTime(2026, 8, 7, 13, 20, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "AuthorName", "Content", "EntryType" },
                values: new object[] { "Commissioning Engineer", "After P-204B bearing replacement, pump current stabilized at 43 A and cooling-water header pressure held at 5.2 bar. A simulated low-header condition started P-204A in 4 seconds with its selector in Auto; repeating the test in Manual produced no start command.", "FunctionalTest" });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Description",
                value: "At 14:27 UTC, crude tower overhead pressure reached 2.4 bar and activated the high-high pressure safeguard during a period of reduced cooling-water flow. Feed was cut automatically, vapour was routed to flare for eleven minutes, and no personnel exposure occurred. Equipment history for pump P-204B is recorded in case SYN-2026-0009.");

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Description",
                value: "Cooling-water pump P-204B outboard-bearing vibration increased from 4.1 to 8.7 mm/s over fourteen days, above the 7.1 mm/s review threshold. Inspection was deferred while a replacement bearing was sourced.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Investigation Team", "Direct cause: P-204B bearing degradation reduced cooling-water flow and condenser duty. Root cause: the vibration-threshold deferral process allowed continued operation without risk-based approval or compensating controls. Contributing factor: P-204A was not configured for automatic transfer.", "RootCause", new DateTime(2026, 8, 7, 14, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Operations Manager", "P-204B bearing replacement and P-204A automatic-transfer testing were assigned before restart. The maintenance-deferral procedure will require accountable approval, an expiry date, and documented compensating controls.", "CorrectiveAction", new DateTime(2026, 8, 7, 16, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "AuthorName", "Content", "EntryType" },
                values: new object[] { "Process Safety Manager", "Effectiveness checks: verify weekly vibration remains below 4.5 mm/s for 30 days, function-test automatic transfer twice, and audit all open rotating-equipment deferrals by 2026-08-21.", "VerificationPlan" });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Description",
                value: "At 14:27 UTC, reduced cooling-water flow raised crude tower overhead pressure to 2.4 bar and activated the high-high pressure safeguard. The investigation links the loss of condenser duty to cooling-water pump P-204B and precursor case SYN-2026-0009. Feed was cut automatically, vapour was routed to flare for eleven minutes, and no personnel exposure occurred.");

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Description",
                value: "Cooling-water pump P-204B outboard-bearing vibration increased from 4.1 to 8.7 mm/s over fourteen days, above the 7.1 mm/s review threshold. Inspection was deferred while a replacement bearing was sourced; the case is a precursor to incident SYN-2026-0017.");
        }
    }
}
