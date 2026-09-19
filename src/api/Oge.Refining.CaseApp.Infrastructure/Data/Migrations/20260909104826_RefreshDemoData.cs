using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Oge.Refining.CaseApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefreshDemoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "OccurredAt",
                value: new DateTime(2026, 9, 3, 13, 55, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "OccurredAt",
                value: new DateTime(2026, 9, 3, 14, 20, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "OccurredAt",
                value: new DateTime(2026, 9, 3, 14, 27, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "OccurredAt",
                value: new DateTime(2026, 9, 3, 14, 35, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "OccurredAt",
                value: new DateTime(2026, 9, 3, 16, 10, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "OccurredAt",
                value: new DateTime(2026, 9, 3, 17, 5, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "OccurredAt",
                value: new DateTime(2026, 9, 4, 9, 15, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "Content", "OccurredAt" },
                values: new object[] { "Procedure MNT-14 requires a documented risk review, supervisor approval, expiry date, and compensating monitoring when equipment remains in service above the 7.1 mm/s vibration threshold. The SYN-2026-0009 attachment register contained no approval or monitoring plan, and the scheduled 2026-09-01 reading was blank.", new DateTime(2026, 9, 4, 11, 40, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "Content", "OccurredAt" },
                values: new object[] { "Work order WO-4821 recorded P-204A seal testing on 2026-09-02. The DCS audit log shows its selector changed from Auto to Manual at 11:20 UTC; the work order closed at 12:05 UTC, with no later selector change before the incident.", new DateTime(2026, 9, 4, 13, 20, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                column: "OccurredAt",
                value: new DateTime(2026, 9, 8, 9, 30, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "AuthorName", "Content", "OccurredAt" },
                values: new object[] { "Process Engineer", "Shift reconciliation found no signed temperature-equilibration entry for catalyst lot CAT-26-0902.", new DateTime(2026, 9, 2, 10, 20, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Quality Specialist", "The loading checklist was complete except for the 180 C hold-point initials and duration.", "RecordsReview", new DateTime(2026, 9, 2, 13, 5, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Area Operator", "Work at the sample station paused when a contractor was observed wearing open safety glasses rather than sealed eyewear.", "Observation", new DateTime(2026, 8, 26, 11, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Contractor Supervisor", "The worker changed PPE before sampling resumed, and the crew reviewed the station-specific requirement.", "ImmediateAction", new DateTime(2026, 8, 26, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Reliability Technician", "Route reading measured 8.7 mm/s at the P-204B outboard bearing, up from 4.1 mm/s fourteen days earlier.", "ConditionMonitoring", new DateTime(2026, 8, 20, 8, 10, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Maintenance Planner", "Inspection was deferred pending delivery of the replacement bearing; no enhanced monitoring interval was entered.", "InspectionDeferral", new DateTime(2026, 8, 20, 13, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "CaseTimelineEntries",
                columns: new[] { "Id", "AuthorName", "CaseId", "Content", "EntryType", "OccurredAt" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Unit Historian", new Guid("22222222-2222-2222-2222-222222222222"), "Historian data shows reactor temperature remained between 181 C and 184 C for 47 minutes before loading resumed.", "ProcessData", new DateTime(2026, 9, 3, 9, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Quality Specialist", new Guid("22222222-2222-2222-2222-222222222222"), "The console operator recalled announcing the hold-point completion but could not confirm who accepted it in the field.", "Interview", new DateTime(2026, 9, 4, 14, 10, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "Operations Superintendent", new Guid("22222222-2222-2222-2222-222222222222"), "Technical evidence supports equilibration; the record remains open for quality disposition and procedure-control review.", "DispositionReview", new DateTime(2026, 9, 7, 15, 10, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000003"), "Safety Advisor", new Guid("33333333-3333-3333-3333-333333333333"), "The permit referenced the general PPE matrix but omitted the sample-station eyewear note.", "ProcedureReview", new DateTime(2026, 8, 27, 9, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "Area Supervisor", new Guid("33333333-3333-3333-3333-333333333333"), "Field verification confirmed updated permit wording and sealed eyewear in use by the full crew.", "Verification", new DateTime(2026, 8, 27, 14, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "Rotating Equipment Engineer", new Guid("44444444-4444-4444-4444-444444444444"), "P-204B was isolated under WO-4827 after the cooling-water event and bearing damage was confirmed.", "WorkOrder", new DateTime(2026, 9, 3, 18, 10, 0, 0, DateTimeKind.Utc) },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "Maintenance Supervisor", new Guid("44444444-4444-4444-4444-444444444444"), "The bearing assembly was replaced, alignment was corrected by 0.18 mm, and the pump returned to service.", "Repair", new DateTime(2026, 9, 5, 16, 25, 0, 0, DateTimeKind.Utc) },
                    { new Guid("40000000-0000-0000-0000-000000000005"), "Reliability Engineer", new Guid("44444444-4444-4444-4444-444444444444"), "Three post-repair readings remained between 3.2 and 3.5 mm/s with stable bearing temperature.", "Verification", new DateTime(2026, 9, 7, 10, 45, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "ReportedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 9, 3, 14, 35, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 8, 9, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Description", "ReportedAt", "UpdatedAt" },
                values: new object[] { "Temperature-equilibration evidence was missing from the catalyst loading record. The review is reconciling batch historian data with operator logs before disposition.", new DateTime(2026, 9, 2, 10, 20, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 7, 15, 10, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "Description", "ReportedAt", "UpdatedAt" },
                values: new object[] { "A contractor entered the sample station without sealed eyewear. Work stopped immediately, the PPE matrix was clarified, and the observation was closed after a field verification.", new DateTime(2026, 8, 26, 11, 45, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 27, 14, 15, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "Description", "ReportedAt", "Status", "UpdatedAt" },
                values: new object[] { "Cooling-water pump P-204B outboard-bearing vibration increased from 4.1 to 8.7 mm/s over fourteen days, above the 7.1 mm/s review threshold. The bearing was replaced and the case closed after stable post-repair readings.", new DateTime(2026, 8, 20, 8, 10, 0, 0, DateTimeKind.Utc), "Closed", new DateTime(2026, 9, 7, 10, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Cases",
                columns: new[] { "Id", "CaseNumber", "Description", "ProcessUnitName", "RefineryName", "RegulatoryNotifiable", "ReportedAt", "Severity", "Status", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), "SYN-2026-0018", "The final-effluent analyzer reported elevated oil-in-water readings during a calibration drift. Retained samples are under laboratory review and discharge remained within the containment route.", "Wastewater Treatment Unit", "Northstar Demonstration Refinery", true, new DateTime(2026, 9, 4, 7, 40, 0, 0, DateTimeKind.Utc), "Major", "Open", "Wastewater analyzer reporting excursion", "EnvironmentalConcern", new DateTime(2026, 9, 8, 16, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "SYN-2026-0019", "Low seal-gas pressure initiated an automatic compressor unload before hydrocarbon containment was challenged. The investigation is reviewing a regulator response delay.", "Hydrotreater B", "Northstar Demonstration Refinery", false, new DateTime(2026, 9, 5, 19, 25, 0, 0, DateTimeKind.Utc), "Major", "InProgress", "Hydrogen compressor seal-gas near miss", "NearMiss", new DateTime(2026, 9, 8, 13, 5, 0, 0, DateTimeKind.Utc) },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "SYN-2026-0020", "Two shell-course thickness readings in the inspection system do not match the signed field worksheet. Disposition is on hold pending instrument verification and source-record reconciliation.", "Tank Farm", "Northstar Demonstration Refinery", false, new DateTime(2026, 9, 7, 9, 15, 0, 0, DateTimeKind.Utc), "Moderate", "OnHold", "Tank inspection thickness records mismatch", "NonConformance", new DateTime(2026, 9, 8, 14, 40, 0, 0, DateTimeKind.Utc) },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "SYN-2026-0021", "A temporary pedestrian diversion around hose testing was missing one directional sign. The route remained barricaded and the sign was installed during the inspection.", "Marine Terminal", "Northstar Demonstration Refinery", false, new DateTime(2026, 9, 8, 8, 30, 0, 0, DateTimeKind.Utc), "Informational", "Open", "Temporary access route signage", "SafetyObservation", new DateTime(2026, 9, 8, 11, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "CaseTimelineEntries",
                columns: new[] { "Id", "AuthorName", "CaseId", "Content", "EntryType", "OccurredAt" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), "Environmental Console", new Guid("55555555-5555-5555-5555-555555555555"), "Analyzer AIT-730 reported 18 ppm oil in water against the 15 ppm investigation threshold.", "Alarm", new DateTime(2026, 9, 4, 7, 40, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "Wastewater Operator", new Guid("55555555-5555-5555-5555-555555555555"), "Final effluent was routed through the containment basin while grab samples were collected.", "Containment", new DateTime(2026, 9, 4, 7, 48, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "Instrument Technician", new Guid("55555555-5555-5555-5555-555555555555"), "The analyzer zero check showed a positive drift of 6 ppm; cleaning restored the expected response.", "CalibrationCheck", new DateTime(2026, 9, 4, 10, 35, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "Environmental Laboratory", new Guid("55555555-5555-5555-5555-555555555555"), "The first two retained samples measured 9 ppm and 10 ppm. One confirmatory split sample remains pending.", "LaboratoryResult", new DateTime(2026, 9, 6, 12, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000005"), "Environmental Lead", new Guid("55555555-5555-5555-5555-555555555555"), "The event remains open and notifiable pending the confirmatory result and completion of the discharge-duration calculation.", "RegulatoryReview", new DateTime(2026, 9, 8, 16, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60000000-0000-0000-0000-000000000001"), "Control System Historian", new Guid("66666666-6666-6666-6666-666666666666"), "Seal-gas differential pressure fell to 0.7 bar and the compressor unloaded automatically in 1.8 seconds.", "Alarm", new DateTime(2026, 9, 5, 19, 25, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60000000-0000-0000-0000-000000000002"), "Shift Supervisor", new Guid("66666666-6666-6666-6666-666666666666"), "The standby seal-gas source established pressure before the trip threshold; fixed gas detectors remained at zero.", "Safeguard", new DateTime(2026, 9, 5, 19, 27, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60000000-0000-0000-0000-000000000003"), "Machinery Engineer", new Guid("66666666-6666-6666-6666-666666666666"), "No seal damage was found. Regulator PCV-612 exhibited delayed stem travel during bench testing.", "Inspection", new DateTime(2026, 9, 6, 8, 40, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60000000-0000-0000-0000-000000000004"), "Control Systems Engineer", new Guid("66666666-6666-6666-6666-666666666666"), "Trip and unload set points matched the approved cause-and-effect chart; no recent logic changes were present.", "ConfigurationHistory", new DateTime(2026, 9, 7, 11, 10, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60000000-0000-0000-0000-000000000005"), "Investigation Lead", new Guid("66666666-6666-6666-6666-666666666666"), "Metallurgy and maintenance-history reviews are in progress before the regulator failure mechanism is determined.", "InvestigationUpdate", new DateTime(2026, 9, 8, 13, 5, 0, 0, DateTimeKind.Utc) },
                    { new Guid("70000000-0000-0000-0000-000000000001"), "Integrity Engineer", new Guid("77777777-7777-7777-7777-777777777777"), "Inspection-system values for locations C2-14 and C2-15 differed from the signed worksheet by 0.8 mm and 0.9 mm.", "RecordsReview", new DateTime(2026, 9, 7, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("70000000-0000-0000-0000-000000000002"), "Inspection Manager", new Guid("77777777-7777-7777-7777-777777777777"), "The remaining-life calculation and inspection closeout were placed on hold.", "DataHold", new DateTime(2026, 9, 7, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("70000000-0000-0000-0000-000000000003"), "NDE Coordinator", new Guid("77777777-7777-7777-7777-777777777777"), "Calibration blocks and the ultrasonic instrument passed verification with no reportable variance.", "InstrumentCheck", new DateTime(2026, 9, 8, 9, 25, 0, 0, DateTimeKind.Utc) },
                    { new Guid("70000000-0000-0000-0000-000000000004"), "Integrity Engineer", new Guid("77777777-7777-7777-7777-777777777777"), "Original acquisition files were requested from the inspection vendor to determine which values were transcribed incorrectly.", "Reconciliation", new DateTime(2026, 9, 8, 14, 40, 0, 0, DateTimeKind.Utc) },
                    { new Guid("80000000-0000-0000-0000-000000000001"), "Marine Safety Representative", new Guid("88888888-8888-8888-8888-888888888888"), "One directional sign was missing from the barricaded pedestrian diversion around hose pressure testing.", "Observation", new DateTime(2026, 9, 8, 8, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("80000000-0000-0000-0000-000000000002"), "Terminal Operator", new Guid("88888888-8888-8888-8888-888888888888"), "A replacement sign was installed and the route was walked from both approaches.", "ImmediateAction", new DateTime(2026, 9, 8, 8, 42, 0, 0, DateTimeKind.Utc) },
                    { new Guid("80000000-0000-0000-0000-000000000003"), "Marine Supervisor", new Guid("88888888-8888-8888-8888-888888888888"), "The observation remains open until the next-shift pre-job briefing verifies the temporary traffic plan.", "FollowUp", new DateTime(2026, 9, 8, 11, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "OccurredAt",
                value: new DateTime(2026, 8, 6, 13, 55, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "OccurredAt",
                value: new DateTime(2026, 8, 6, 14, 20, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "OccurredAt",
                value: new DateTime(2026, 8, 6, 14, 27, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "OccurredAt",
                value: new DateTime(2026, 8, 6, 14, 35, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "OccurredAt",
                value: new DateTime(2026, 8, 6, 16, 10, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "OccurredAt",
                value: new DateTime(2026, 8, 6, 17, 5, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "OccurredAt",
                value: new DateTime(2026, 8, 7, 9, 15, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "Content", "OccurredAt" },
                values: new object[] { "Procedure MNT-14 requires a documented risk review, supervisor approval, expiry date, and compensating monitoring when equipment remains in service above the 7.1 mm/s vibration threshold. The SYN-2026-0009 attachment register contained no approval or monitoring plan, and the scheduled 2026-08-04 reading was blank.", new DateTime(2026, 8, 7, 11, 40, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "Content", "OccurredAt" },
                values: new object[] { "Work order WO-4821 recorded P-204A seal testing on 2026-08-05. The DCS audit log shows its selector changed from Auto to Manual at 11:20 UTC; the work order closed at 12:05 UTC, with no later selector change before the incident.", new DateTime(2026, 8, 7, 13, 20, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                column: "OccurredAt",
                value: new DateTime(2026, 8, 8, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "AuthorName", "Content", "OccurredAt" },
                values: new object[] { "Synthetic Reporter", "Case created from fictional demonstration data.", new DateTime(2026, 8, 5, 10, 20, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Synthetic Review Team", "Initial review completed for demonstration purposes.", "Review", new DateTime(2026, 8, 5, 13, 20, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Synthetic Reporter", "Case created from fictional demonstration data.", "Reported", new DateTime(2026, 7, 28, 11, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Synthetic Review Team", "Initial review completed for demonstration purposes.", "Review", new DateTime(2026, 7, 28, 14, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Synthetic Reporter", "Case created from fictional demonstration data.", "Reported", new DateTime(2026, 7, 21, 8, 10, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "AuthorName", "Content", "EntryType", "OccurredAt" },
                values: new object[] { "Synthetic Review Team", "Initial review completed for demonstration purposes.", "Review", new DateTime(2026, 7, 21, 11, 10, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "ReportedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 6, 14, 35, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 8, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Description", "ReportedAt", "UpdatedAt" },
                values: new object[] { "A synthetic procedure review identified an incomplete temperature-equilibration record.", new DateTime(2026, 8, 5, 10, 20, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 5, 13, 20, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "Description", "ReportedAt", "UpdatedAt" },
                values: new object[] { "A fictional field observation was corrected immediately during a routine inspection.", new DateTime(2026, 7, 28, 11, 45, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 28, 14, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "Description", "ReportedAt", "Status", "UpdatedAt" },
                values: new object[] { "Cooling-water pump P-204B outboard-bearing vibration increased from 4.1 to 8.7 mm/s over fourteen days, above the 7.1 mm/s review threshold. Inspection was deferred while a replacement bearing was sourced.", new DateTime(2026, 7, 21, 8, 10, 0, 0, DateTimeKind.Utc), "OnHold", new DateTime(2026, 7, 21, 11, 10, 0, 0, DateTimeKind.Utc) });
        }
    }
}
