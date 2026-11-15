using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Oge.Refining.CaseApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProgressOlderDemoCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CaseTimelineEntries",
                columns: new[] { "Id", "AuthorName", "CaseId", "Content", "EntryType", "OccurredAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000011"), "Investigation Lead", new Guid("11111111-1111-1111-1111-111111111111"), "Evidence collection was completed and the case moved to InProgress for review of the maintenance-deferral and standby-restoration records. Equipment restoration alone does not complete the investigation.", "InvestigationUpdate", new DateTime(2026, 9, 16, 13, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000012"), "Investigation Lead", new Guid("11111111-1111-1111-1111-111111111111"), "The P-204A readiness discrepancy in SYN-2026-0022 has been added to the evidence register. Closure is deferred pending reconciliation of WO-5098, operator checklists, and DCS selector history; no new pressure excursion has been reported.", "InvestigationUpdate", new DateTime(2026, 10, 23, 16, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "Quality Specialist", new Guid("22222222-2222-2222-2222-222222222222"), "Quality accepted the historian trace and signed operator reconciliation without backdating the original checklist. The revised form requires separate hold-point completion and acceptance signatures.", "Verification", new DateTime(2026, 9, 18, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000007"), "Operations Superintendent", new Guid("22222222-2222-2222-2222-222222222222"), "Three subsequent loading records contained both required signatures and linked temperature traces. The technical disposition and effectiveness review were approved; the case is closed.", "Closure", new DateTime(2026, 10, 22, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000005"), "Safety Advisor", new Guid("33333333-3333-3333-3333-333333333333"), "Two sample-station spot checks found sealed eyewear in use and the station-specific requirement present on the active permits.", "FollowUp", new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000006"), "Area Supervisor", new Guid("33333333-3333-3333-3333-333333333333"), "The October contractor briefing and permit audit confirmed sustained compliance. The August closure remains valid and no additional action is outstanding.", "Closure", new DateTime(2026, 10, 21, 14, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("40000000-0000-0000-0000-000000000006"), "Reliability Technician", new Guid("44444444-4444-4444-4444-444444444444"), "October route readings for P-204B remained between 3.1 and 3.6 mm/s with stable bearing temperature and no repeat bearing alarms.", "ConditionMonitoring", new DateTime(2026, 10, 16, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("40000000-0000-0000-0000-000000000007"), "Reliability Engineer", new Guid("44444444-4444-4444-4444-444444444444"), "P-204B repair verification remains satisfactory and this maintenance case stays closed. The distinct P-204A selector-restoration issue is being reviewed in SYN-2026-0022 and the incident investigation SYN-2026-0017.", "Closure", new DateTime(2026, 10, 23, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000006"), "Environmental Laboratory", new Guid("55555555-5555-5555-5555-555555555555"), "The confirmatory split sample measured 9 ppm. The sample record and containment-routing log were reconciled, and the cleaned analyzer passed its reference check.", "LaboratoryResult", new DateTime(2026, 9, 11, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000007"), "Environmental Lead", new Guid("55555555-5555-5555-5555-555555555555"), "The notification closeout and routing-duration review are complete, and October checks confirmed stable AIT-730 performance. The case is closed with its historical notifiable flag retained. The new SW-14 stormwater discrepancy remains separate in SYN-2026-0023.", "Closure", new DateTime(2026, 10, 22, 16, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60000000-0000-0000-0000-000000000006"), "Machinery Engineer", new Guid("66666666-6666-6666-6666-666666666666"), "Bench inspection found regulator stem scoring consistent with the delayed travel. PCV-612 was replaced; three witnessed demand tests restored differential pressure without reaching the unload threshold.", "Verification", new DateTime(2026, 9, 17, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60000000-0000-0000-0000-000000000007"), "Operations Superintendent", new Guid("66666666-6666-6666-6666-666666666666"), "The maintenance record and regulator inspection were accepted. The October historian review found no repeat low seal-gas pressure events; the near-miss case is closed.", "Closure", new DateTime(2026, 10, 21, 15, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("70000000-0000-0000-0000-000000000005"), "NDE Coordinator", new Guid("77777777-7777-7777-7777-777777777777"), "Signed acquisition files matched the field worksheet. The two inspection-system values were corrected with an audit trail, and an independent remaining-life calculation confirmed the approved inspection interval.", "Verification", new DateTime(2026, 9, 22, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("70000000-0000-0000-0000-000000000006"), "Inspection Manager", new Guid("77777777-7777-7777-7777-777777777777"), "The reconciliation package was signed and the inspection hold released. The October record audit confirmed the corrected values; this case is closed. The new T-118 unsigned export is tracked separately in SYN-2026-0025.", "Closure", new DateTime(2026, 10, 23, 13, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("80000000-0000-0000-0000-000000000004"), "Marine Safety Representative", new Guid("88888888-8888-8888-8888-888888888888"), "The next-shift briefing and route walk confirmed signs visible from both approaches. After hose testing finished, the temporary diversion and barricades were removed under the traffic plan.", "Verification", new DateTime(2026, 9, 9, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("80000000-0000-0000-0000-000000000005"), "Marine Supervisor", new Guid("88888888-8888-8888-8888-888888888888"), "An October terminal walkdown confirmed normal pedestrian access and no remaining temporary signage defects. The observation is closed.", "Closure", new DateTime(2026, 10, 20, 11, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "At 14:27 UTC, crude tower overhead pressure reached 2.4 bar and activated the high-high pressure safeguard during a period of reduced cooling-water flow. Feed was cut automatically, vapour was routed to flare for eleven minutes, and no personnel exposure occurred. Equipment history for pump P-204B is recorded in case SYN-2026-0009. The investigation remains in progress while the October standby-pump readiness discrepancy in SYN-2026-0022 is reviewed before closure.", "InProgress", new DateTime(2026, 10, 23, 16, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "Temperature-equilibration evidence was missing from the catalyst loading record. Quality reconciled the historian data with operator logs, approved the technical disposition, and verified the revised hold-point sign-off process. The case is closed following an October effectiveness check.", "Closed", new DateTime(2026, 10, 22, 14, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "Description", "UpdatedAt" },
                values: new object[] { "A contractor entered the sample station without sealed eyewear. Work stopped immediately, the PPE matrix was clarified, and the observation was closed after a field verification. October spot checks confirmed continued compliance and the case remains closed.", new DateTime(2026, 10, 21, 14, 15, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "Description", "UpdatedAt" },
                values: new object[] { "Cooling-water pump P-204B outboard-bearing vibration increased from 4.1 to 8.7 mm/s over fourteen days, above the 7.1 mm/s review threshold. The bearing was replaced and the case closed after stable post-repair readings. October condition monitoring confirms stable P-204B performance; the separate P-204A readiness review continues in SYN-2026-0022.", new DateTime(2026, 10, 23, 14, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "The final-effluent analyzer reported elevated oil-in-water readings during a calibration drift. Confirmatory laboratory results and routing records supported closeout after analyzer verification and completion of the notification record. The historical notifiable flag is retained. The separate October stormwater discrepancy is tracked in SYN-2026-0023.", "Closed", new DateTime(2026, 10, 22, 16, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "Low seal-gas pressure initiated an automatic compressor unload before hydrocarbon containment was challenged. The delayed regulator was replaced and witnessed response tests were satisfactory. The case is closed after an October operating review confirmed no repeat low-pressure events.", "Closed", new DateTime(2026, 10, 21, 15, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "Two shell-course thickness readings in the inspection system differed from the signed field worksheet. Source acquisition files confirmed a transcription error; the records and remaining-life calculation were independently checked and the hold was released. The case is closed; the separate T-118 export-revision issue is tracked in SYN-2026-0025.", "Closed", new DateTime(2026, 10, 23, 13, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "A temporary pedestrian diversion around hose testing was missing one directional sign. The sign was installed, the next-shift briefing verified the route, and the temporary diversion was removed after testing. An October walkdown confirmed normal access and the case is closed.", "Closed", new DateTime(2026, 10, 20, 11, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000005"));

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "At 14:27 UTC, crude tower overhead pressure reached 2.4 bar and activated the high-high pressure safeguard during a period of reduced cooling-water flow. Feed was cut automatically, vapour was routed to flare for eleven minutes, and no personnel exposure occurred. Equipment history for pump P-204B is recorded in case SYN-2026-0009.", "Open", new DateTime(2026, 9, 8, 9, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "Temperature-equilibration evidence was missing from the catalyst loading record. The review is reconciling batch historian data with operator logs before disposition.", "InProgress", new DateTime(2026, 9, 7, 15, 10, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "Description", "UpdatedAt" },
                values: new object[] { "A contractor entered the sample station without sealed eyewear. Work stopped immediately, the PPE matrix was clarified, and the observation was closed after a field verification.", new DateTime(2026, 8, 27, 14, 15, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "Description", "UpdatedAt" },
                values: new object[] { "Cooling-water pump P-204B outboard-bearing vibration increased from 4.1 to 8.7 mm/s over fourteen days, above the 7.1 mm/s review threshold. The bearing was replaced and the case closed after stable post-repair readings.", new DateTime(2026, 9, 7, 10, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "The final-effluent analyzer reported elevated oil-in-water readings during a calibration drift. Retained samples are under laboratory review and discharge remained within the containment route.", "Open", new DateTime(2026, 9, 8, 16, 20, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "Low seal-gas pressure initiated an automatic compressor unload before hydrocarbon containment was challenged. The investigation is reviewing a regulator response delay.", "InProgress", new DateTime(2026, 9, 8, 13, 5, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "Two shell-course thickness readings in the inspection system do not match the signed field worksheet. Disposition is on hold pending instrument verification and source-record reconciliation.", "OnHold", new DateTime(2026, 9, 8, 14, 40, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "Description", "Status", "UpdatedAt" },
                values: new object[] { "A temporary pedestrian diversion around hose testing was missing one directional sign. The route remained barricaded and the sign was installed during the inspection.", "Open", new DateTime(2026, 9, 8, 11, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
