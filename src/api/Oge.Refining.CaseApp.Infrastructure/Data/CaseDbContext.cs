using Oge.Refining.CaseApp.Application.Cases;
using Microsoft.EntityFrameworkCore;

namespace Oge.Refining.CaseApp.Infrastructure.Data;

public sealed class CaseDbContext(DbContextOptions<CaseDbContext> options) : DbContext(options)
{
    public DbSet<CaseRecord> Cases => Set<CaseRecord>();
    public DbSet<CaseTimelineRecord> TimelineEntries => Set<CaseTimelineRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cases = new[]
        {
            CreateCase("11111111-1111-1111-1111-111111111111", "SYN-2026-0017", "Crude tower overhead pressure excursion", "Crude Distillation Unit 1", CaseType.Incident, CaseStatus.Open, CaseSeverity.Critical, "At 14:27 UTC, crude tower overhead pressure reached 2.4 bar and activated the high-high pressure safeguard during a period of reduced cooling-water flow. Feed was cut automatically, vapour was routed to flare for eleven minutes, and no personnel exposure occurred. Equipment history for pump P-204B is recorded in case SYN-2026-0009.", true, new DateTimeOffset(2026, 9, 3, 14, 35, 0, TimeSpan.Zero), new DateTimeOffset(2026, 9, 8, 9, 30, 0, TimeSpan.Zero)),
            CreateCase("22222222-2222-2222-2222-222222222222", "SYN-2026-0016", "Catalyst loading procedure deviation", "Fluid Catalytic Cracker 2", CaseType.NonConformance, CaseStatus.InProgress, CaseSeverity.Moderate, "Temperature-equilibration evidence was missing from the catalyst loading record. The review is reconciling batch historian data with operator logs before disposition.", false, new DateTimeOffset(2026, 9, 2, 10, 20, 0, TimeSpan.Zero), new DateTimeOffset(2026, 9, 7, 15, 10, 0, TimeSpan.Zero)),
            CreateCase("33333333-3333-3333-3333-333333333333", "SYN-2026-0012", "Inspection eyewear observation", "Hydrotreater A", CaseType.SafetyObservation, CaseStatus.Closed, CaseSeverity.Minor, "A contractor entered the sample station without sealed eyewear. Work stopped immediately, the PPE matrix was clarified, and the observation was closed after a field verification.", false, new DateTimeOffset(2026, 8, 26, 11, 45, 0, TimeSpan.Zero), new DateTimeOffset(2026, 8, 27, 14, 15, 0, TimeSpan.Zero)),
            CreateCase("44444444-4444-4444-4444-444444444444", "SYN-2026-0009", "Cooling-water pump vibration trend", "Utilities Area", CaseType.MaintenanceIssue, CaseStatus.Closed, CaseSeverity.Major, "Cooling-water pump P-204B outboard-bearing vibration increased from 4.1 to 8.7 mm/s over fourteen days, above the 7.1 mm/s review threshold. The bearing was replaced and the case closed after stable post-repair readings.", false, new DateTimeOffset(2026, 8, 20, 8, 10, 0, TimeSpan.Zero), new DateTimeOffset(2026, 9, 7, 10, 45, 0, TimeSpan.Zero)),
            CreateCase("55555555-5555-5555-5555-555555555555", "SYN-2026-0018", "Wastewater analyzer reporting excursion", "Wastewater Treatment Unit", CaseType.EnvironmentalConcern, CaseStatus.Open, CaseSeverity.Major, "The final-effluent analyzer reported elevated oil-in-water readings during a calibration drift. Retained samples are under laboratory review and discharge remained within the containment route.", true, new DateTimeOffset(2026, 9, 4, 7, 40, 0, TimeSpan.Zero), new DateTimeOffset(2026, 9, 8, 16, 20, 0, TimeSpan.Zero)),
            CreateCase("66666666-6666-6666-6666-666666666666", "SYN-2026-0019", "Hydrogen compressor seal-gas near miss", "Hydrotreater B", CaseType.NearMiss, CaseStatus.InProgress, CaseSeverity.Major, "Low seal-gas pressure initiated an automatic compressor unload before hydrocarbon containment was challenged. The investigation is reviewing a regulator response delay.", false, new DateTimeOffset(2026, 9, 5, 19, 25, 0, TimeSpan.Zero), new DateTimeOffset(2026, 9, 8, 13, 5, 0, TimeSpan.Zero)),
            CreateCase("77777777-7777-7777-7777-777777777777", "SYN-2026-0020", "Tank inspection thickness records mismatch", "Tank Farm", CaseType.NonConformance, CaseStatus.OnHold, CaseSeverity.Moderate, "Two shell-course thickness readings in the inspection system do not match the signed field worksheet. Disposition is on hold pending instrument verification and source-record reconciliation.", false, new DateTimeOffset(2026, 9, 7, 9, 15, 0, TimeSpan.Zero), new DateTimeOffset(2026, 9, 8, 14, 40, 0, TimeSpan.Zero)),
            CreateCase("88888888-8888-8888-8888-888888888888", "SYN-2026-0021", "Temporary access route signage", "Marine Terminal", CaseType.SafetyObservation, CaseStatus.Open, CaseSeverity.Informational, "A temporary pedestrian diversion around hose testing was missing one directional sign. The route remained barricaded and the sign was installed during the inspection.", false, new DateTimeOffset(2026, 9, 8, 8, 30, 0, TimeSpan.Zero), new DateTimeOffset(2026, 9, 8, 11, 0, 0, TimeSpan.Zero))
        };

        modelBuilder.Entity<CaseRecord>(entity =>
        {
            entity.ToTable("Cases");
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.CaseNumber).IsUnique();
            entity.Property(item => item.CaseNumber).HasMaxLength(32);
            entity.Property(item => item.Title).HasMaxLength(240);
            entity.Property(item => item.RefineryName).HasMaxLength(160);
            entity.Property(item => item.ProcessUnitName).HasMaxLength(160);
            entity.Property(item => item.Type).HasConversion<string>().HasMaxLength(40);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(24);
            entity.Property(item => item.Severity).HasConversion<string>().HasMaxLength(24);
            entity.HasData(cases);
        });

        modelBuilder.Entity<CaseTimelineRecord>(entity =>
        {
            entity.ToTable("CaseTimelineEntries");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.EntryType).HasMaxLength(40);
            entity.Property(item => item.AuthorName).HasMaxLength(160);
            entity.HasOne(item => item.Case)
                .WithMany(item => item.Timeline)
                .HasForeignKey(item => item.CaseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasData(cases.SelectMany((item, index) => CreateTimeline(item, index)));
        });
    }

    private static CaseRecord CreateCase(
        string id,
        string caseNumber,
        string title,
        string processUnitName,
        CaseType type,
        CaseStatus status,
        CaseSeverity severity,
        string description,
        bool regulatoryNotifiable,
        DateTimeOffset reportedAt,
        DateTimeOffset? updatedAt = null) => new()
    {
        Id = Guid.Parse(id),
        CaseNumber = caseNumber,
        Title = title,
        RefineryName = "Northstar Demonstration Refinery",
        ProcessUnitName = processUnitName,
        Type = type,
        Status = status,
        Severity = severity,
        Description = description,
        RegulatoryNotifiable = regulatoryNotifiable,
        ReportedAt = reportedAt.UtcDateTime,
        UpdatedAt = (updatedAt ?? reportedAt.AddHours(3)).UtcDateTime
    };

    private static IEnumerable<CaseTimelineRecord> CreateTimeline(CaseRecord item, int index)
    {
        var prefix = index + 1;
        return item.CaseNumber switch
        {
            "SYN-2026-0017" => CreateInvestigationTimeline(item, prefix),
            "SYN-2026-0016" => CreateCatalystTimeline(item, prefix),
            "SYN-2026-0012" => CreateEyewearTimeline(item, prefix),
            "SYN-2026-0009" => CreatePumpTimeline(item, prefix),
            "SYN-2026-0018" => CreateWastewaterTimeline(item, prefix),
            "SYN-2026-0019" => CreateCompressorTimeline(item, prefix),
            "SYN-2026-0020" => CreateTankTimeline(item, prefix),
            "SYN-2026-0021" => CreateAccessTimeline(item, prefix),
            _ => []
        };
    }

    private static IEnumerable<CaseTimelineRecord> CreateInvestigationTimeline(CaseRecord item, int prefix) =>
    [
        CreateTimelineEntry(prefix, 1, item.Id, "ProcessData", "Control System Historian", new DateTime(2026, 9, 3, 13, 55, 0, DateTimeKind.Utc), "Cooling-water header pressure began falling from 5.1 bar; pump P-204B motor current fell from 42 A to 31 A while the standby pump remained in manual mode."),
        CreateTimelineEntry(prefix, 2, item.Id, "Alarm", "Control System Historian", new DateTime(2026, 9, 3, 14, 20, 0, DateTimeKind.Utc), "Overhead condenser outlet temperature rose from its 35 C baseline to 46 C. Pressure controller PC-101 output reached 92 percent."),
        CreateTimelineEntry(prefix, 3, item.Id, "Safeguard", "Shift Supervisor", new DateTime(2026, 9, 3, 14, 27, 0, DateTimeKind.Utc), "Tower pressure reached the 2.4 bar high-high set point. The safeguard cut unit feed and routed vapour to flare; pressure returned below 1.9 bar at 14:38 UTC."),
        CreateTimelineEntry(prefix, 4, item.Id, "Reported", "Synthetic Reporter", item.ReportedAt, "Incident opened after unit stabilization. No injury or exposure was reported; the eleven-minute flare event was referred for regulatory review."),
        CreateTimelineEntry(prefix, 5, item.Id, "Inspection", "Rotating Equipment Engineer", new DateTime(2026, 9, 3, 16, 10, 0, DateTimeKind.Utc), "P-204B was isolated. The outboard bearing had spalled rollers and heat discoloration; measured radial clearance was 0.31 mm against a 0.08-0.15 mm specification."),
        CreateTimelineEntry(prefix, 6, item.Id, "RecordsReview", "Reliability Engineer", new DateTime(2026, 9, 3, 17, 5, 0, DateTimeKind.Utc), "Case SYN-2026-0009 recorded vibration rising from 4.1 to 8.7 mm/s. Inspection was deferred pending parts, and the operating log contained no compensating monitoring frequency or automatic standby-pump requirement."),
        CreateTimelineEntry(prefix, 7, item.Id, "Interview", "Investigation Lead", new DateTime(2026, 9, 4, 9, 15, 0, DateTimeKind.Utc), "The board operator confirmed P-204A was available but its selector was left in manual after seal work. The low-header-pressure alarm required manual pump transfer and was acknowledged during simultaneous unit alarms."),
        CreateTimelineEntry(prefix, 8, item.Id, "ProcedureReview", "Maintenance Planner", new DateTime(2026, 9, 4, 11, 40, 0, DateTimeKind.Utc), "Procedure MNT-14 requires a documented risk review, supervisor approval, expiry date, and compensating monitoring when equipment remains in service above the 7.1 mm/s vibration threshold. The SYN-2026-0009 attachment register contained no approval or monitoring plan, and the scheduled 2026-09-01 reading was blank."),
        CreateTimelineEntry(prefix, 9, item.Id, "ConfigurationHistory", "Control Systems Engineer", new DateTime(2026, 9, 4, 13, 20, 0, DateTimeKind.Utc), "Work order WO-4821 recorded P-204A seal testing on 2026-09-02. The DCS audit log shows its selector changed from Auto to Manual at 11:20 UTC; the work order closed at 12:05 UTC, with no later selector change before the incident."),
        CreateTimelineEntry(prefix, 10, item.Id, "FunctionalTest", "Commissioning Engineer", new DateTime(2026, 9, 8, 9, 30, 0, DateTimeKind.Utc), "After P-204B bearing replacement, pump current stabilized at 43 A and cooling-water header pressure held at 5.2 bar. A simulated low-header condition started P-204A in 4 seconds with its selector in Auto; repeating the test in Manual produced no start command.")
    ];

    private static IEnumerable<CaseTimelineRecord> CreateCatalystTimeline(CaseRecord item, int prefix) =>
    [
        CreateTimelineEntry(prefix, 1, item.Id, "Reported", "Process Engineer", item.ReportedAt, "Shift reconciliation found no signed temperature-equilibration entry for catalyst lot CAT-26-0902."),
        CreateTimelineEntry(prefix, 2, item.Id, "RecordsReview", "Quality Specialist", new DateTime(2026, 9, 2, 13, 5, 0, DateTimeKind.Utc), "The loading checklist was complete except for the 180 C hold-point initials and duration."),
        CreateTimelineEntry(prefix, 3, item.Id, "ProcessData", "Unit Historian", new DateTime(2026, 9, 3, 9, 30, 0, DateTimeKind.Utc), "Historian data shows reactor temperature remained between 181 C and 184 C for 47 minutes before loading resumed."),
        CreateTimelineEntry(prefix, 4, item.Id, "Interview", "Quality Specialist", new DateTime(2026, 9, 4, 14, 10, 0, DateTimeKind.Utc), "The console operator recalled announcing the hold-point completion but could not confirm who accepted it in the field."),
        CreateTimelineEntry(prefix, 5, item.Id, "DispositionReview", "Operations Superintendent", item.UpdatedAt, "Technical evidence supports equilibration; the record remains open for quality disposition and procedure-control review.")
    ];

    private static IEnumerable<CaseTimelineRecord> CreateEyewearTimeline(CaseRecord item, int prefix) =>
    [
        CreateTimelineEntry(prefix, 1, item.Id, "Observation", "Area Operator", item.ReportedAt, "Work at the sample station paused when a contractor was observed wearing open safety glasses rather than sealed eyewear."),
        CreateTimelineEntry(prefix, 2, item.Id, "ImmediateAction", "Contractor Supervisor", new DateTime(2026, 8, 26, 12, 0, 0, DateTimeKind.Utc), "The worker changed PPE before sampling resumed, and the crew reviewed the station-specific requirement."),
        CreateTimelineEntry(prefix, 3, item.Id, "ProcedureReview", "Safety Advisor", new DateTime(2026, 8, 27, 9, 20, 0, DateTimeKind.Utc), "The permit referenced the general PPE matrix but omitted the sample-station eyewear note."),
        CreateTimelineEntry(prefix, 4, item.Id, "Verification", "Area Supervisor", item.UpdatedAt, "Field verification confirmed updated permit wording and sealed eyewear in use by the full crew.")
    ];

    private static IEnumerable<CaseTimelineRecord> CreatePumpTimeline(CaseRecord item, int prefix) =>
    [
        CreateTimelineEntry(prefix, 1, item.Id, "ConditionMonitoring", "Reliability Technician", item.ReportedAt, "Route reading measured 8.7 mm/s at the P-204B outboard bearing, up from 4.1 mm/s fourteen days earlier."),
        CreateTimelineEntry(prefix, 2, item.Id, "InspectionDeferral", "Maintenance Planner", new DateTime(2026, 8, 20, 13, 30, 0, DateTimeKind.Utc), "Inspection was deferred pending delivery of the replacement bearing; no enhanced monitoring interval was entered."),
        CreateTimelineEntry(prefix, 3, item.Id, "WorkOrder", "Rotating Equipment Engineer", new DateTime(2026, 9, 3, 18, 10, 0, DateTimeKind.Utc), "P-204B was isolated under WO-4827 after the cooling-water event and bearing damage was confirmed."),
        CreateTimelineEntry(prefix, 4, item.Id, "Repair", "Maintenance Supervisor", new DateTime(2026, 9, 5, 16, 25, 0, DateTimeKind.Utc), "The bearing assembly was replaced, alignment was corrected by 0.18 mm, and the pump returned to service."),
        CreateTimelineEntry(prefix, 5, item.Id, "Verification", "Reliability Engineer", item.UpdatedAt, "Three post-repair readings remained between 3.2 and 3.5 mm/s with stable bearing temperature.")
    ];

    private static IEnumerable<CaseTimelineRecord> CreateWastewaterTimeline(CaseRecord item, int prefix) =>
    [
        CreateTimelineEntry(prefix, 1, item.Id, "Alarm", "Environmental Console", item.ReportedAt, "Analyzer AIT-730 reported 18 ppm oil in water against the 15 ppm investigation threshold."),
        CreateTimelineEntry(prefix, 2, item.Id, "Containment", "Wastewater Operator", new DateTime(2026, 9, 4, 7, 48, 0, DateTimeKind.Utc), "Final effluent was routed through the containment basin while grab samples were collected."),
        CreateTimelineEntry(prefix, 3, item.Id, "CalibrationCheck", "Instrument Technician", new DateTime(2026, 9, 4, 10, 35, 0, DateTimeKind.Utc), "The analyzer zero check showed a positive drift of 6 ppm; cleaning restored the expected response."),
        CreateTimelineEntry(prefix, 4, item.Id, "LaboratoryResult", "Environmental Laboratory", new DateTime(2026, 9, 6, 12, 15, 0, DateTimeKind.Utc), "The first two retained samples measured 9 ppm and 10 ppm. One confirmatory split sample remains pending."),
        CreateTimelineEntry(prefix, 5, item.Id, "RegulatoryReview", "Environmental Lead", item.UpdatedAt, "The event remains open and notifiable pending the confirmatory result and completion of the discharge-duration calculation.")
    ];

    private static IEnumerable<CaseTimelineRecord> CreateCompressorTimeline(CaseRecord item, int prefix) =>
    [
        CreateTimelineEntry(prefix, 1, item.Id, "Alarm", "Control System Historian", item.ReportedAt, "Seal-gas differential pressure fell to 0.7 bar and the compressor unloaded automatically in 1.8 seconds."),
        CreateTimelineEntry(prefix, 2, item.Id, "Safeguard", "Shift Supervisor", new DateTime(2026, 9, 5, 19, 27, 0, DateTimeKind.Utc), "The standby seal-gas source established pressure before the trip threshold; fixed gas detectors remained at zero."),
        CreateTimelineEntry(prefix, 3, item.Id, "Inspection", "Machinery Engineer", new DateTime(2026, 9, 6, 8, 40, 0, DateTimeKind.Utc), "No seal damage was found. Regulator PCV-612 exhibited delayed stem travel during bench testing."),
        CreateTimelineEntry(prefix, 4, item.Id, "ConfigurationHistory", "Control Systems Engineer", new DateTime(2026, 9, 7, 11, 10, 0, DateTimeKind.Utc), "Trip and unload set points matched the approved cause-and-effect chart; no recent logic changes were present."),
        CreateTimelineEntry(prefix, 5, item.Id, "InvestigationUpdate", "Investigation Lead", item.UpdatedAt, "Metallurgy and maintenance-history reviews are in progress before the regulator failure mechanism is determined.")
    ];

    private static IEnumerable<CaseTimelineRecord> CreateTankTimeline(CaseRecord item, int prefix) =>
    [
        CreateTimelineEntry(prefix, 1, item.Id, "RecordsReview", "Integrity Engineer", item.ReportedAt, "Inspection-system values for locations C2-14 and C2-15 differed from the signed worksheet by 0.8 mm and 0.9 mm."),
        CreateTimelineEntry(prefix, 2, item.Id, "DataHold", "Inspection Manager", new DateTime(2026, 9, 7, 10, 0, 0, DateTimeKind.Utc), "The remaining-life calculation and inspection closeout were placed on hold."),
        CreateTimelineEntry(prefix, 3, item.Id, "InstrumentCheck", "NDE Coordinator", new DateTime(2026, 9, 8, 9, 25, 0, DateTimeKind.Utc), "Calibration blocks and the ultrasonic instrument passed verification with no reportable variance."),
        CreateTimelineEntry(prefix, 4, item.Id, "Reconciliation", "Integrity Engineer", item.UpdatedAt, "Original acquisition files were requested from the inspection vendor to determine which values were transcribed incorrectly.")
    ];

    private static IEnumerable<CaseTimelineRecord> CreateAccessTimeline(CaseRecord item, int prefix) =>
    [
        CreateTimelineEntry(prefix, 1, item.Id, "Observation", "Marine Safety Representative", item.ReportedAt, "One directional sign was missing from the barricaded pedestrian diversion around hose pressure testing."),
        CreateTimelineEntry(prefix, 2, item.Id, "ImmediateAction", "Terminal Operator", new DateTime(2026, 9, 8, 8, 42, 0, DateTimeKind.Utc), "A replacement sign was installed and the route was walked from both approaches."),
        CreateTimelineEntry(prefix, 3, item.Id, "FollowUp", "Marine Supervisor", item.UpdatedAt, "The observation remains open until the next-shift pre-job briefing verifies the temporary traffic plan.")
    ];

    private static CaseTimelineRecord CreateTimelineEntry(
        int prefix,
        int sequence,
        Guid caseId,
        string entryType,
        string authorName,
        DateTime occurredAt,
        string content) => new()
    {
        Id = Guid.Parse($"{prefix}0000000-0000-0000-0000-{sequence:000000000000}"),
        CaseId = caseId,
        EntryType = entryType,
        AuthorName = authorName,
        OccurredAt = occurredAt,
        Content = content
    };
}