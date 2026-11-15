using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Oge.Refining.CaseApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOctoberDemoCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cases",
                columns: new[] { "Id", "CaseNumber", "Description", "ProcessUnitName", "RefineryName", "RegulatoryNotifiable", "ReportedAt", "Severity", "Status", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999999"), "SYN-2026-0022", "An October readiness audit found standby pump P-204A selected to Manual after seal testing. The review compares restoration records with the September pressure excursion in SYN-2026-0017 and pump history in SYN-2026-0009. No new process excursion occurred.", "Utilities Area", "Northstar Demonstration Refinery", false, new DateTime(2026, 10, 20, 8, 0, 0, 0, DateTimeKind.Utc), "Major", "InProgress", "Cooling-water standby pump readiness discrepancy", "MaintenanceIssue", new DateTime(2026, 10, 23, 15, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "SYN-2026-0023", "Following heavy rain, an online stormwater analyzer indicated 18 ppm oil-in-water while a paired laboratory sample measured 6 ppm. Water remains in the retention basin pending reconciliation. Investigators are comparing analyzer records with SYN-2026-0018; no off-site release has been recorded.", "Wastewater Treatment Unit", "Northstar Demonstration Refinery", false, new DateTime(2026, 10, 21, 7, 30, 0, 0, DateTimeKind.Utc), "Moderate", "Open", "Stormwater containment sample discrepancy", "EnvironmentalConcern", new DateTime(2026, 10, 23, 16, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "SYN-2026-0024", "A pre-transfer check detected an intermittent bonding indication on loading arm LA-3. Transfer was not started. The terminal replaced a damaged bonding lead and completed witnessed continuity checks before returning the arm to service.", "Marine Terminal", "Northstar Demonstration Refinery", false, new DateTime(2026, 10, 22, 9, 0, 0, 0, DateTimeKind.Utc), "Moderate", "Closed", "Loading-arm bonding verification near miss", "NearMiss", new DateTime(2026, 10, 23, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "SYN-2026-0025", "The October inspection package for tank T-118 contained an unsigned export revision. Approval is on hold until the vendor supplies the signed source file. The record-control review references SYN-2026-0020; no change to the tank operating envelope has been authorized.", "Tank Farm", "Northstar Demonstration Refinery", false, new DateTime(2026, 10, 23, 10, 0, 0, 0, DateTimeKind.Utc), "Minor", "OnHold", "Tank inspection source-file reconciliation hold", "NonConformance", new DateTime(2026, 10, 23, 17, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "CaseTimelineEntries",
                columns: new[] { "Id", "AuthorName", "CaseId", "Content", "EntryType", "OccurredAt" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), "Utilities Operator", new Guid("99999999-9999-9999-9999-999999999999"), "The readiness checklist showed P-204A available, but the control-room selector indicated Manual. P-204B remained in service with stable discharge pressure.", "Observation", new DateTime(2026, 10, 20, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("90000000-0000-0000-0000-000000000002"), "Maintenance Planner", new Guid("99999999-9999-9999-9999-999999999999"), "Seal-test work order WO-5098 contained a test completion signature but no selector-restoration entry. The review includes WO-4821 evidence in SYN-2026-0017.", "RecordsReview", new DateTime(2026, 10, 20, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("90000000-0000-0000-0000-000000000003"), "Control Systems Engineer", new Guid("99999999-9999-9999-9999-999999999999"), "After an approved restoration check, P-204A was returned to Auto and started on simulated low-header pressure in 3.8 seconds.", "FunctionalTest", new DateTime(2026, 10, 21, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("90000000-0000-0000-0000-000000000004"), "Investigation Lead", new Guid("99999999-9999-9999-9999-999999999999"), "The case remains in progress while the team reconciles the checklist with DCS audit records and reviews the effectiveness of the September handover controls.", "InvestigationUpdate", new DateTime(2026, 10, 23, 15, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a0000000-0000-0000-0000-000000000001"), "Environmental Operator", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Analyzer SW-14 indicated 18 ppm oil-in-water after rainfall. The basin outlet remained closed and available containment capacity was confirmed.", "Observation", new DateTime(2026, 10, 21, 7, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), "Laboratory Analyst", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "A paired grab sample measured 6 ppm. Sample identity, collection time, and chain of custody matched the analyzer comparison record.", "LaboratoryResult", new DateTime(2026, 10, 21, 12, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), "Instrument Technician", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Optical-window deposits were photographed before cleaning. A post-cleaning reference check read 7 ppm; an independent repeat sample was requested.", "Inspection", new DateTime(2026, 10, 22, 7, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), "Environmental Lead", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "The outlet remains closed pending the repeat result. Similarities to SYN-2026-0018 are under review; a common failure mechanism has not been established.", "InvestigationUpdate", new DateTime(2026, 10, 23, 16, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b0000000-0000-0000-0000-000000000001"), "Terminal Operator", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "The bonding permissive flickered during the pre-transfer check on LA-3. The transfer pump had not been started.", "Observation", new DateTime(2026, 10, 22, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b0000000-0000-0000-0000-000000000002"), "Marine Supervisor", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "LA-3 was removed from service and the planned transfer was deferred. No product movement or loss of containment occurred.", "ImmediateAction", new DateTime(2026, 10, 22, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b0000000-0000-0000-0000-000000000003"), "Electrical Technician", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Flexing the bonding lead reproduced the intermittent indication. The lead showed sheath damage near the clamp and was replaced.", "Inspection", new DateTime(2026, 10, 22, 13, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b0000000-0000-0000-0000-000000000004"), "Marine Safety Representative", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Witnessed continuity checks and three repeat permissive tests were satisfactory. The replacement record was attached and the case closed.", "Verification", new DateTime(2026, 10, 23, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c0000000-0000-0000-0000-000000000001"), "Integrity Engineer", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "The T-118 inspection package referenced export revision 3, but only an unsigned revision 2 file was attached.", "RecordsReview", new DateTime(2026, 10, 23, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c0000000-0000-0000-0000-000000000002"), "Inspection Manager", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Inspection closeout was placed on hold. The existing approved inspection interval and operating limits remain unchanged.", "DataHold", new DateTime(2026, 10, 23, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c0000000-0000-0000-0000-000000000003"), "Inspection Coordinator", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "The vendor acknowledged the revision mismatch and is retrieving the signed source file and acquisition checksum.", "VendorResponse", new DateTime(2026, 10, 23, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c0000000-0000-0000-0000-000000000004"), "Integrity Engineer", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "The signed file is still outstanding. The team will compare the export-control steps with the thickness-record discrepancy in SYN-2026-0020 before disposition.", "Reconciliation", new DateTime(2026, 10, 23, 17, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CaseTimelineEntries",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Cases",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));
        }
    }
}
