using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oge.Refining.CaseApp.Infrastructure.Data.Migrations;

public partial class AddRootCauseScenario : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Cases",
            keyColumn: "Id",
            keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
            columns: new[] { "Description", "UpdatedAt" },
            values: new object[] { "At 14:27 UTC, reduced cooling-water flow raised crude tower overhead pressure to 2.4 bar and activated the high-high pressure safeguard. The investigation links the loss of condenser duty to cooling-water pump P-204B and precursor case SYN-2026-0009. Feed was cut automatically, vapour was routed to flare for eleven minutes, and no personnel exposure occurred.", new DateTime(2026, 8, 8, 10, 0, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "Cases",
            keyColumn: "Id",
            keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
            column: "Description",
            value: "Cooling-water pump P-204B outboard-bearing vibration increased from 4.1 to 8.7 mm/s over fourteen days, above the 7.1 mm/s review threshold. Inspection was deferred while a replacement bearing was sourced; the case is a precursor to incident SYN-2026-0017.");

        migrationBuilder.UpdateData(
            table: "CaseTimelineEntries",
            keyColumn: "Id",
            keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
            columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
            values: new object[] { "Control System Historian", "Cooling-water header pressure began falling from 5.1 bar; pump P-204B motor current fell from 42 A to 31 A while the standby pump remained in manual mode.", "ProcessData", new DateTime(2026, 8, 6, 13, 55, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "CaseTimelineEntries",
            keyColumn: "Id",
            keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
            columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
            values: new object[] { "Control System Historian", "Overhead condenser outlet temperature rose from its 35 C baseline to 46 C. Pressure controller PC-101 output reached 92 percent.", "Alarm", new DateTime(2026, 8, 6, 14, 20, 0, DateTimeKind.Utc) });

        migrationBuilder.InsertData(
            table: "CaseTimelineEntries",
            columns: new[] { "Id", "AuthorName", "CaseId", "Content", "EntryType", "OccurredAt" },
            values: new object[,]
            {
                { new Guid("10000000-0000-0000-0000-000000000003"), "Shift Supervisor", new Guid("11111111-1111-1111-1111-111111111111"), "Tower pressure reached the 2.4 bar high-high set point. The safeguard cut unit feed and routed vapour to flare; pressure returned below 1.9 bar at 14:38 UTC.", "Safeguard", new DateTime(2026, 8, 6, 14, 27, 0, DateTimeKind.Utc) },
                { new Guid("10000000-0000-0000-0000-000000000004"), "Synthetic Reporter", new Guid("11111111-1111-1111-1111-111111111111"), "Incident opened after unit stabilization. No injury or exposure was reported; the eleven-minute flare event was referred for regulatory review.", "Reported", new DateTime(2026, 8, 6, 14, 35, 0, DateTimeKind.Utc) },
                { new Guid("10000000-0000-0000-0000-000000000005"), "Rotating Equipment Engineer", new Guid("11111111-1111-1111-1111-111111111111"), "P-204B was isolated. The outboard bearing had spalled rollers and heat discoloration; measured radial clearance was 0.31 mm against a 0.08-0.15 mm specification.", "Inspection", new DateTime(2026, 8, 6, 16, 10, 0, DateTimeKind.Utc) },
                { new Guid("10000000-0000-0000-0000-000000000006"), "Reliability Engineer", new Guid("11111111-1111-1111-1111-111111111111"), "Case SYN-2026-0009 recorded vibration rising from 4.1 to 8.7 mm/s. Inspection was deferred pending parts, and the operating log contained no compensating monitoring frequency or automatic standby-pump requirement.", "RecordsReview", new DateTime(2026, 8, 6, 17, 5, 0, 0, DateTimeKind.Utc) },
                { new Guid("10000000-0000-0000-0000-000000000007"), "Investigation Lead", new Guid("11111111-1111-1111-1111-111111111111"), "The board operator confirmed P-204A was available but its selector was left in manual after seal work. The low-header-pressure alarm required manual pump transfer and was acknowledged during simultaneous unit alarms.", "Interview", new DateTime(2026, 8, 7, 9, 15, 0, 0, DateTimeKind.Utc) },
                { new Guid("10000000-0000-0000-0000-000000000008"), "Investigation Team", new Guid("11111111-1111-1111-1111-111111111111"), "Direct cause: P-204B bearing degradation reduced cooling-water flow and condenser duty. Root cause: the vibration-threshold deferral process allowed continued operation without risk-based approval or compensating controls. Contributing factor: P-204A was not configured for automatic transfer.", "RootCause", new DateTime(2026, 8, 7, 14, 30, 0, 0, DateTimeKind.Utc) },
                { new Guid("10000000-0000-0000-0000-000000000009"), "Operations Manager", new Guid("11111111-1111-1111-1111-111111111111"), "P-204B bearing replacement and P-204A automatic-transfer testing were assigned before restart. The maintenance-deferral procedure will require accountable approval, an expiry date, and documented compensating controls.", "CorrectiveAction", new DateTime(2026, 8, 7, 16, 0, 0, DateTimeKind.Utc) },
                { new Guid("10000000-0000-0000-0000-000000000010"), "Process Safety Manager", new Guid("11111111-1111-1111-1111-111111111111"), "Effectiveness checks: verify weekly vibration remains below 4.5 mm/s for 30 days, function-test automatic transfer twice, and audit all open rotating-equipment deferrals by 2026-08-21.", "VerificationPlan", new DateTime(2026, 8, 8, 10, 0, 0, DateTimeKind.Utc) }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        for (var sequence = 3; sequence <= 10; sequence++)
        {
            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: Guid.Parse($"10000000-0000-0000-0000-{sequence:000000000000}"));
        }

        migrationBuilder.UpdateData(
            table: "CaseTimelineEntries",
            keyColumn: "Id",
            keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
            columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
            values: new object[] { "Synthetic Reporter", "Case created from fictional demonstration data.", "Reported", new DateTime(2026, 8, 6, 14, 35, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "CaseTimelineEntries",
            keyColumn: "Id",
            keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
            columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
            values: new object[] { "Synthetic Review Team", "Initial review completed for demonstration purposes.", "Review", new DateTime(2026, 8, 6, 17, 35, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "Cases",
            keyColumn: "Id",
            keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
            columns: new[] { "Description", "UpdatedAt" },
            values: new object[] { "A synthetic pressure excursion triggered an automatic unit response. No personnel exposure occurred.", new DateTime(2026, 8, 6, 17, 35, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "Cases",
            keyColumn: "Id",
            keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
            column: "Description",
            value: "Synthetic vibration readings exceeded the fictional review threshold pending a planned inspection.");
    }
}