using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class FormCosDbContext : DbContext
{
    public FormCosDbContext(DbContextOptions<FormCosDbContext> options) : base(options) { }

    // ═══ Master Data Tables (from db_master_data) ═══
    public DbSet<TlkpPlant> TlkpPlants => Set<TlkpPlant>();
    public DbSet<TlkpDivisi> TlkpDivisis => Set<TlkpDivisi>();
    public DbSet<TlkpDepartemen> TlkpDepartemens => Set<TlkpDepartemen>();
    public DbSet<TlkpSection> TlkpSections => Set<TlkpSection>();
    public DbSet<TlkpLine> TlkpLines => Set<TlkpLine>();
    public DbSet<TlkpGroup> TlkpGroups => Set<TlkpGroup>();
    public DbSet<TlkpShift> TlkpShifts => Set<TlkpShift>();
    public DbSet<TlkpLineGroup> TlkpLineGroups => Set<TlkpLineGroup>();
    public DbSet<TlkpOperator> TlkpOperators => Set<TlkpOperator>();
    public DbSet<TlkpUserKasie> TlkpUserKasies => Set<TlkpUserKasie>();
    public DbSet<TlkpUser> TlkpUsers => Set<TlkpUser>();
    public DbSet<TlkpItem> TlkpItems => Set<TlkpItem>();
    public DbSet<TlkpKategori> TlkpKategoris => Set<TlkpKategori>();
    public DbSet<TlkpSeries> TlkpSerieses => Set<TlkpSeries>();
    public DbSet<TlkpMold> TlkpMolds => Set<TlkpMold>();
    public DbSet<TlkpMoldType> TlkpMoldTypes => Set<TlkpMoldType>();
    public DbSet<TlkpJob> TlkpJobs => Set<TlkpJob>();
    public DbSet<ViewDataAuth> ViewDataAuths => Set<ViewDataAuth>();
    public DbSet<ViewEmployee> ViewEmployees => Set<ViewEmployee>();

    // ═══ COS Form Tables (new) ═══
    public DbSet<CosFormDefinition> CosFormDefinitions => Set<CosFormDefinition>();
    public DbSet<CosCheckItem> CosCheckItems => Set<CosCheckItem>();
    public DbSet<CosCheckSubRow> CosCheckSubRows => Set<CosCheckSubRow>();
    public DbSet<CosBatteryType> CosBatteryTypes => Set<CosBatteryType>();
    public DbSet<CosBatteryStandard> CosBatteryStandards => Set<CosBatteryStandard>();
    public DbSet<CosProblemColumn> CosProblemColumns => Set<CosProblemColumn>();
    public DbSet<CosSignatureSlot> CosSignatureSlots => Set<CosSignatureSlot>();
    public DbSet<CosSubmission> CosSubmissions => Set<CosSubmission>();
    public DbSet<CosCheckValue> CosCheckValues => Set<CosCheckValue>();
    public DbSet<CosProblem> CosProblems => Set<CosProblem>();
    public DbSet<CosSignatureEntry> CosSignatureEntries => Set<CosSignatureEntry>();
    public DbSet<CosEmployeeSignature> CosEmployeeSignatures => Set<CosEmployeeSignature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ═══ Composite Keys ═══
        modelBuilder.Entity<TlkpOperator>()
            .HasKey(o => new { o.UserId, o.JoprId });

        modelBuilder.Entity<TlkpItem>()
            .HasKey(i => new { i.ItemNum, i.KatId });

        // ═══ Keyless Entity ═══
        modelBuilder.Entity<ViewEmployee>().HasNoKey();

        // ═══ Unique Constraints ═══
        modelBuilder.Entity<CosFormDefinition>()
            .HasIndex(f => f.Code).IsUnique();

        modelBuilder.Entity<CosBatteryStandard>()
            .HasIndex(bs => new { bs.BatteryTypeId, bs.ParamKey }).IsUnique();

        modelBuilder.Entity<CosCheckItem>()
            .HasIndex(ci => new { ci.FormId, ci.ItemKey }).IsUnique();

        modelBuilder.Entity<CosCheckValue>()
            .HasIndex(sv => new { sv.SubmissionId, sv.SettingKey }).IsUnique();

        modelBuilder.Entity<CosSignatureEntry>()
            .HasIndex(ss => new { ss.SubmissionId, ss.RoleKey }).IsUnique();

        modelBuilder.Entity<CosEmployeeSignature>()
            .HasIndex(es => es.EmpId).IsUnique();

        // ═══ Cascades ═══
        modelBuilder.Entity<CosSubmission>()
            .HasMany(s => s.CheckValues).WithOne(v => v.Submission).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CosSubmission>()
            .HasMany(s => s.Problems).WithOne(p => p.Submission).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CosSubmission>()
            .HasMany(s => s.Signatures).WithOne(s => s.Submission).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CosFormDefinition>()
            .HasMany(f => f.CheckItems).WithOne(c => c.Form).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CosFormDefinition>()
            .HasMany(f => f.ProblemColumns).WithOne(p => p.Form).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CosFormDefinition>()
            .HasMany(f => f.SignatureSlots).WithOne(s => s.Form).OnDelete(DeleteBehavior.Cascade);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder mb)
    {
        // ══════════════════════════════════════════
        // COS FORM DEFINITION
        // ══════════════════════════════════════════
        mb.Entity<CosFormDefinition>().HasData(new CosFormDefinition
        {
            Id = 1,
            Code = "COS_VALIDATION",
            Title = "VALIDASI PROSES COS",
            Subtitle = "Form-A2 1-K.051-5-2",
            SlotCount = 3,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        });

        // ══════════════════════════════════════════
        // BATTERY TYPES (COS-specific)
        // ══════════════════════════════════════════
        mb.Entity<CosBatteryType>().HasData(
            new CosBatteryType { Id = 1, Name = "NS40ZL" },
            new CosBatteryType { Id = 2, Name = "NS60L" },
            new CosBatteryType { Id = 3, Name = "34B19LS" },
            new CosBatteryType { Id = 4, Name = "N50Z" },
            new CosBatteryType { Id = 5, Name = "N70Z" },
            new CosBatteryType { Id = 6, Name = "34B19LS OE TYT" }
        );

        // ══════════════════════════════════════════
        // BATTERY STANDARDS
        // ══════════════════════════════════════════
        mb.Entity<CosBatteryStandard>().HasData(
            // pourWait
            new CosBatteryStandard { Id = 1, ParamKey = "pourWait", Value = "-", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 2, ParamKey = "pourWait", Value = "-", MinValue = null, MaxValue = null, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 3, ParamKey = "pourWait", Value = "1.0 - 2.0", MinValue = 1.0m, MaxValue = 2.0m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 4, ParamKey = "pourWait", Value = "-", MinValue = null, MaxValue = null, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 5, ParamKey = "pourWait", Value = "-", MinValue = null, MaxValue = null, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 6, ParamKey = "pourWait", Value = "1.0 - 2.0", MinValue = 1.0m, MaxValue = 2.0m, BatteryTypeId = 6 },
            // pourTime
            new CosBatteryStandard { Id = 7, ParamKey = "pourTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 8, ParamKey = "pourTime", Value = "2.5 - 4.0", MinValue = 2.5m, MaxValue = 4.0m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 9, ParamKey = "pourTime", Value = "1.5 - 3.0", MinValue = 1.5m, MaxValue = 3.0m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 10, ParamKey = "pourTime", Value = "3.0 - 5.0", MinValue = 3.0m, MaxValue = 5.0m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 11, ParamKey = "pourTime", Value = "3.5 - 5.5", MinValue = 3.5m, MaxValue = 5.5m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 12, ParamKey = "pourTime", Value = "1.5 - 3.0", MinValue = 1.5m, MaxValue = 3.0m, BatteryTypeId = 6 },
            // dipTime2
            new CosBatteryStandard { Id = 13, ParamKey = "dipTime2", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 14, ParamKey = "dipTime2", Value = "2.0 - 3.5", MinValue = 2.0m, MaxValue = 3.5m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 15, ParamKey = "dipTime2", Value = "1.0 - 2.5", MinValue = 1.0m, MaxValue = 2.5m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 16, ParamKey = "dipTime2", Value = "2.5 - 4.0", MinValue = 2.5m, MaxValue = 4.0m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 17, ParamKey = "dipTime2", Value = "3.0 - 4.5", MinValue = 3.0m, MaxValue = 4.5m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 18, ParamKey = "dipTime2", Value = "1.0 - 2.5", MinValue = 1.0m, MaxValue = 2.5m, BatteryTypeId = 6 },
            // dumpTime
            new CosBatteryStandard { Id = 19, ParamKey = "dumpTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 20, ParamKey = "dumpTime", Value = "1.5 - 3.0", MinValue = 1.5m, MaxValue = 3.0m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 21, ParamKey = "dumpTime", Value = "0.8 - 2.0", MinValue = 0.8m, MaxValue = 2.0m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 22, ParamKey = "dumpTime", Value = "2.0 - 3.5", MinValue = 2.0m, MaxValue = 3.5m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 23, ParamKey = "dumpTime", Value = "2.5 - 4.0", MinValue = 2.5m, MaxValue = 4.0m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 24, ParamKey = "dumpTime", Value = "0.8 - 2.0", MinValue = 0.8m, MaxValue = 2.0m, BatteryTypeId = 6 },
            // lugDryTime
            new CosBatteryStandard { Id = 25, ParamKey = "lugDryTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 26, ParamKey = "lugDryTime", Value = "3.5 - 5.5", MinValue = 3.5m, MaxValue = 5.5m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 27, ParamKey = "lugDryTime", Value = "2.5 - 4.5", MinValue = 2.5m, MaxValue = 4.5m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 28, ParamKey = "lugDryTime", Value = "4.0 - 6.0", MinValue = 4.0m, MaxValue = 6.0m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 29, ParamKey = "lugDryTime", Value = "4.5 - 6.5", MinValue = 4.5m, MaxValue = 6.5m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 30, ParamKey = "lugDryTime", Value = "2.5 - 4.5", MinValue = 2.5m, MaxValue = 4.5m, BatteryTypeId = 6 },
            // largeVibratorTime
            new CosBatteryStandard { Id = 31, ParamKey = "largeVibratorTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 32, ParamKey = "largeVibratorTime", Value = "1.5 - 3.5", MinValue = 1.5m, MaxValue = 3.5m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 33, ParamKey = "largeVibratorTime", Value = "0.8 - 2.5", MinValue = 0.8m, MaxValue = 2.5m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 34, ParamKey = "largeVibratorTime", Value = "2.0 - 4.0", MinValue = 2.0m, MaxValue = 4.0m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 35, ParamKey = "largeVibratorTime", Value = "2.5 - 4.5", MinValue = 2.5m, MaxValue = 4.5m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 36, ParamKey = "largeVibratorTime", Value = "0.8 - 2.5", MinValue = 0.8m, MaxValue = 2.5m, BatteryTypeId = 6 },
            // smallVibratorTime
            new CosBatteryStandard { Id = 37, ParamKey = "smallVibratorTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 38, ParamKey = "smallVibratorTime", Value = "1.5 - 3.5", MinValue = 1.5m, MaxValue = 3.5m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 39, ParamKey = "smallVibratorTime", Value = "0.8 - 2.5", MinValue = 0.8m, MaxValue = 2.5m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 40, ParamKey = "smallVibratorTime", Value = "2.0 - 4.0", MinValue = 2.0m, MaxValue = 4.0m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 41, ParamKey = "smallVibratorTime", Value = "2.5 - 4.5", MinValue = 2.5m, MaxValue = 4.5m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 42, ParamKey = "smallVibratorTime", Value = "0.8 - 2.5", MinValue = 0.8m, MaxValue = 2.5m, BatteryTypeId = 6 },
            // coolingTime
            new CosBatteryStandard { Id = 43, ParamKey = "coolingTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 44, ParamKey = "coolingTime", Value = "25 - 35", MinValue = 25m, MaxValue = 35m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 45, ParamKey = "coolingTime", Value = "18 - 28", MinValue = 18m, MaxValue = 28m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 46, ParamKey = "coolingTime", Value = "28 - 38", MinValue = 28m, MaxValue = 38m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 47, ParamKey = "coolingTime", Value = "30 - 42", MinValue = 30m, MaxValue = 42m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 48, ParamKey = "coolingTime", Value = "18 - 28", MinValue = 18m, MaxValue = 28m, BatteryTypeId = 6 },
            // leadPumpSpeed
            new CosBatteryStandard { Id = 49, ParamKey = "leadPumpSpeed", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 50, ParamKey = "leadPumpSpeed", Value = "45 - 65", MinValue = 45m, MaxValue = 65m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 51, ParamKey = "leadPumpSpeed", Value = "35 - 55", MinValue = 35m, MaxValue = 55m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 52, ParamKey = "leadPumpSpeed", Value = "50 - 70", MinValue = 50m, MaxValue = 70m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 53, ParamKey = "leadPumpSpeed", Value = "55 - 75", MinValue = 55m, MaxValue = 75m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 54, ParamKey = "leadPumpSpeed", Value = "35 - 55", MinValue = 35m, MaxValue = 55m, BatteryTypeId = 6 },
            // tempAirDryer
            new CosBatteryStandard { Id = 55, ParamKey = "tempAirDryer", Value = "300 - 400", MinValue = 300m, MaxValue = 400m, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 56, ParamKey = "tempAirDryer", Value = "310 - 410", MinValue = 310m, MaxValue = 410m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 57, ParamKey = "tempAirDryer", Value = "290 - 390", MinValue = 290m, MaxValue = 390m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 58, ParamKey = "tempAirDryer", Value = "320 - 420", MinValue = 320m, MaxValue = 420m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 59, ParamKey = "tempAirDryer", Value = "330 - 430", MinValue = 330m, MaxValue = 430m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 60, ParamKey = "tempAirDryer", Value = "290 - 390", MinValue = 290m, MaxValue = 390m, BatteryTypeId = 6 },
            // tempPot
            new CosBatteryStandard { Id = 61, ParamKey = "tempPot", Value = "470 - 490", MinValue = 470m, MaxValue = 490m, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 62, ParamKey = "tempPot", Value = "475 - 495", MinValue = 475m, MaxValue = 495m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 63, ParamKey = "tempPot", Value = "465 - 485", MinValue = 465m, MaxValue = 485m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 64, ParamKey = "tempPot", Value = "480 - 500", MinValue = 480m, MaxValue = 500m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 65, ParamKey = "tempPot", Value = "485 - 505", MinValue = 485m, MaxValue = 505m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 66, ParamKey = "tempPot", Value = "465 - 485", MinValue = 465m, MaxValue = 485m, BatteryTypeId = 6 },
            // tempPipe
            new CosBatteryStandard { Id = 67, ParamKey = "tempPipe", Value = "410 - 430", MinValue = 410m, MaxValue = 430m, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 68, ParamKey = "tempPipe", Value = "415 - 435", MinValue = 415m, MaxValue = 435m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 69, ParamKey = "tempPipe", Value = "405 - 425", MinValue = 405m, MaxValue = 425m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 70, ParamKey = "tempPipe", Value = "420 - 440", MinValue = 420m, MaxValue = 440m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 71, ParamKey = "tempPipe", Value = "425 - 445", MinValue = 425m, MaxValue = 445m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 72, ParamKey = "tempPipe", Value = "405 - 425", MinValue = 405m, MaxValue = 425m, BatteryTypeId = 6 },
            // tempCrossBlock
            new CosBatteryStandard { Id = 73, ParamKey = "tempCrossBlock", Value = "390 - 410", MinValue = 390m, MaxValue = 410m, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 74, ParamKey = "tempCrossBlock", Value = "395 - 415", MinValue = 395m, MaxValue = 415m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 75, ParamKey = "tempCrossBlock", Value = "385 - 405", MinValue = 385m, MaxValue = 405m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 76, ParamKey = "tempCrossBlock", Value = "400 - 420", MinValue = 400m, MaxValue = 420m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 77, ParamKey = "tempCrossBlock", Value = "405 - 425", MinValue = 405m, MaxValue = 425m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 78, ParamKey = "tempCrossBlock", Value = "385 - 405", MinValue = 385m, MaxValue = 405m, BatteryTypeId = 6 },
            // tempElbow
            new CosBatteryStandard { Id = 79, ParamKey = "tempElbow", Value = "370 - 390", MinValue = 370m, MaxValue = 390m, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 80, ParamKey = "tempElbow", Value = "375 - 395", MinValue = 375m, MaxValue = 395m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 81, ParamKey = "tempElbow", Value = "365 - 385", MinValue = 365m, MaxValue = 385m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 82, ParamKey = "tempElbow", Value = "380 - 400", MinValue = 380m, MaxValue = 400m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 83, ParamKey = "tempElbow", Value = "385 - 405", MinValue = 385m, MaxValue = 405m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 84, ParamKey = "tempElbow", Value = "365 - 385", MinValue = 365m, MaxValue = 385m, BatteryTypeId = 6 },
            // tempMold
            new CosBatteryStandard { Id = 85, ParamKey = "tempMold", Value = "160 - 190", MinValue = 160m, MaxValue = 190m, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 86, ParamKey = "tempMold", Value = "165 - 195", MinValue = 165m, MaxValue = 195m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 87, ParamKey = "tempMold", Value = "155 - 185", MinValue = 155m, MaxValue = 185m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 88, ParamKey = "tempMold", Value = "170 - 200", MinValue = 170m, MaxValue = 200m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 89, ParamKey = "tempMold", Value = "175 - 205", MinValue = 175m, MaxValue = 205m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 90, ParamKey = "tempMold", Value = "155 - 185", MinValue = 155m, MaxValue = 185m, BatteryTypeId = 6 },
            // coolingFlowRate
            new CosBatteryStandard { Id = 91, ParamKey = "coolingFlowRate", Value = "6 - 10", MinValue = 6m, MaxValue = 10m, BatteryTypeId = 1 },
            new CosBatteryStandard { Id = 92, ParamKey = "coolingFlowRate", Value = "7 - 11", MinValue = 7m, MaxValue = 11m, BatteryTypeId = 2 },
            new CosBatteryStandard { Id = 93, ParamKey = "coolingFlowRate", Value = "5 - 9", MinValue = 5m, MaxValue = 9m, BatteryTypeId = 3 },
            new CosBatteryStandard { Id = 94, ParamKey = "coolingFlowRate", Value = "8 - 12", MinValue = 8m, MaxValue = 12m, BatteryTypeId = 4 },
            new CosBatteryStandard { Id = 95, ParamKey = "coolingFlowRate", Value = "9 - 13", MinValue = 9m, MaxValue = 13m, BatteryTypeId = 5 },
            new CosBatteryStandard { Id = 96, ParamKey = "coolingFlowRate", Value = "5 - 9", MinValue = 5m, MaxValue = 9m, BatteryTypeId = 6 }
        );

        // ══════════════════════════════════════════
        // CHECK ITEMS (FormId = 1)
        // ══════════════════════════════════════════
        mb.Entity<CosCheckItem>().HasData(
            new CosCheckItem { Id = 1, FormId = 1, ItemKey = "kekuatanCastingStrap", Label = "Kekuatan Casting Strap", Type = "visual", VisualStandard = "Ditarik tidak lepas", Frequency = "1 batt / shift / ganti type", SortOrder = 1 },
            new CosCheckItem { Id = 2, FormId = 1, ItemKey = "meniscus", Label = "Meniscus", Type = "visual", VisualStandard = "Positif", Frequency = "1 batt / shift / ganti type", SortOrder = 2 },
            new CosCheckItem { Id = 3, FormId = 1, ItemKey = "hasilCastingStrap", Label = "Hasil Casting Strap", Type = "visual", VisualStandard = "Tidak ada flash", Frequency = "1 Batt / shift / ganti type", SortOrder = 3 },
            new CosCheckItem { Id = 4, FormId = 1, ItemKey = "levelFlux", Label = "Level Flux", Type = "visual", VisualStandard = "Terisi Flux", SortOrder = 4 },
            new CosCheckItem { Id = 5, FormId = 1, ItemKey = "pourWait", Label = "Pour Wait (Khusus Line 8)", Type = "numeric", NumericStdKey = "pourWait", Frequency = "2 x / Shift / ganti type", ConditionalLabel = "Khusus Line 8", SortOrder = 5 },
            new CosCheckItem { Id = 6, FormId = 1, ItemKey = "pourTime", Label = "Pour Time", Type = "numeric", NumericStdKey = "pourTime", SortOrder = 6 },
            new CosCheckItem { Id = 7, FormId = 1, ItemKey = "dipTime2", Label = "Dip Time 2", Type = "numeric", NumericStdKey = "dipTime2", SortOrder = 7 },
            new CosCheckItem { Id = 8, FormId = 1, ItemKey = "dumpTime", Label = "Dump Time (Drain back time)", Type = "numeric", NumericStdKey = "dumpTime", SortOrder = 8 },
            new CosCheckItem { Id = 9, FormId = 1, ItemKey = "lugDryTime", Label = "Lug Dry Time", Type = "numeric", NumericStdKey = "lugDryTime", Frequency = "2 x / Shift / ganti type", Keterangan = "untuk 34B19LS OE TYT", SortOrder = 9 },
            new CosCheckItem { Id = 10, FormId = 1, ItemKey = "largeVibratorTime", Label = "Large Vibrator Time", Type = "numeric", NumericStdKey = "largeVibratorTime", SortOrder = 10 },
            new CosCheckItem { Id = 11, FormId = 1, ItemKey = "smallVibratorTime", Label = "Small Vibrator Time", Type = "numeric", NumericStdKey = "smallVibratorTime", SortOrder = 11 },
            new CosCheckItem { Id = 12, FormId = 1, ItemKey = "coolingTime", Label = "Cooling Time", Type = "numeric", NumericStdKey = "coolingTime", SortOrder = 12 },
            new CosCheckItem { Id = 13, FormId = 1, ItemKey = "leadPumpSpeed", Label = "Lead Pump Speed", Type = "numeric", NumericStdKey = "leadPumpSpeed", SortOrder = 13 },
            new CosCheckItem { Id = 14, FormId = 1, ItemKey = "checkAlignment", Label = "Check Alignment", Type = "visual", VisualStandard = "Bergerak", Frequency = "1 x / shift", SortOrder = 14 },
            new CosCheckItem { Id = 15, FormId = 1, ItemKey = "checkDatumTable", Label = "Check Datum Table Alignment", Type = "visual", VisualStandard = "Bersih", Frequency = "1 x / shift", Keterangan = "Tidak ada ceceran pasta", SortOrder = 15 },
            new CosCheckItem { Id = 16, FormId = 1, ItemKey = "cleaningNozzle", Label = "Cleaning of Nozzle Lug Dry", Type = "visual", VisualStandard = "Bersih", Frequency = "1 x / shift", Keterangan = "Spray dengan udara", SortOrder = 16 },
            new CosCheckItem { Id = 17, FormId = 1, ItemKey = "tempAirNozzleLugDry", Label = "Temp Air Nozzle Lug Dry", Type = "numeric", FixedStandard = "> 275\u00b0 C", FixedMin = 275m, Frequency = "2 x / shift", Keterangan = "Cek dgn Thermocouple", SortOrder = 17 },
            new CosCheckItem { Id = 18, FormId = 1, ItemKey = "tempAirDryer", Label = "Temp Air Dryer (hot air)", Type = "numeric", NumericStdKey = "tempAirDryer", Frequency = "2 x / shift", SortOrder = 18 },
            new CosCheckItem { Id = 19, FormId = 1, ItemKey = "blowerPipeTemp", Label = "Blower Pipe Temp (Khusus Line 7)", Type = "numeric", FixedStandard = "> 300\u00b0 C", FixedMin = 300m, Frequency = "2 x / shift", ConditionalLabel = "Khusus Line 7", SortOrder = 19 },
            new CosCheckItem { Id = 20, FormId = 1, ItemKey = "blowerNozzle1Temp", Label = "Blower Nozzle 1 Temp (Khusus Line 7)", Type = "numeric", FixedStandard = "> 200\u00b0 C", FixedMin = 200m, Frequency = "2 x / shift", ConditionalLabel = "Khusus Line 7", SortOrder = 20 },
            new CosCheckItem { Id = 21, FormId = 1, ItemKey = "blowerNozzle2Temp", Label = "Blower Nozzle 2 Temp (Khusus Line 7)", Type = "numeric", FixedStandard = "> 200\u00b0 C", FixedMin = 200m, Frequency = "2 x / shift", ConditionalLabel = "Khusus Line 7", SortOrder = 21 },
            new CosCheckItem { Id = 22, FormId = 1, ItemKey = "tempPot", Label = "Temperatur Pot", Type = "numeric", NumericStdKey = "tempPot", Frequency = "2 x / shift", SortOrder = 22 },
            new CosCheckItem { Id = 23, FormId = 1, ItemKey = "tempPipe", Label = "Temperatur Pipe", Type = "numeric", NumericStdKey = "tempPipe", Frequency = "2 x / shift", SortOrder = 23 },
            new CosCheckItem { Id = 24, FormId = 1, ItemKey = "tempCrossBlock", Label = "Temp. Cross Block", Type = "numeric", NumericStdKey = "tempCrossBlock", Frequency = "2 x / shift", SortOrder = 24 },
            new CosCheckItem { Id = 25, FormId = 1, ItemKey = "tempElbow", Label = "Temp. Elbow (Lead Lump)", Type = "numeric", NumericStdKey = "tempElbow", Frequency = "2 x / shift", SortOrder = 25 },
            new CosCheckItem { Id = 26, FormId = 1, ItemKey = "tempMold", Label = "Temperatur Mold", Type = "numeric", NumericStdKey = "tempMold", Frequency = "2 x / shift", SortOrder = 26 },
            new CosCheckItem { Id = 27, FormId = 1, ItemKey = "coolingFlowRate", Label = "Cooling Water Flow Rate", Type = "numeric", NumericStdKey = "coolingFlowRate", Frequency = "2 x / shift", SortOrder = 27 },
            new CosCheckItem { Id = 28, FormId = 1, ItemKey = "coolingWaterTemp", Label = "Cooling Water Temperature", Type = "numeric", FixedStandard = "28 \u00b1 2 \u00b0C", FixedMin = 26m, FixedMax = 30m, Frequency = "2 x / shift", SortOrder = 28 },
            new CosCheckItem { Id = 29, FormId = 1, ItemKey = "sprueBrush", Label = "Sprue Brush", Type = "visual", VisualStandard = "Berfungsi dengan baik", Frequency = "2 x / Shift", SortOrder = 29 },
            new CosCheckItem { Id = 30, FormId = 1, ItemKey = "cleaningCavityMold", Label = "Cleaning Cavity Mold COS", Type = "visual", VisualStandard = "Tidak tersumbat dross", Frequency = "3 x / Shift", SortOrder = 30 },
            new CosCheckItem { Id = 31, FormId = 1, ItemKey = "fluxTime", Label = "Flux Time", Type = "numeric", Frequency = "1 batt / shift / ganti type", SortOrder = 31 },
            new CosCheckItem { Id = 32, FormId = 1, ItemKey = "overflowHydrazine", Label = "Overflow Hydrazine", Type = "numeric", Frequency = "1 batt / shift / ganti type", SortOrder = 32 }
        );

        // ══════════════════════════════════════════
        // CHECK SUB ROWS
        // ══════════════════════════════════════════
        mb.Entity<CosCheckSubRow>().HasData(
            new CosCheckSubRow { Id = 1, Suffix = "plus", Label = "+", SortOrder = 1, CheckItemId = 1 },
            new CosCheckSubRow { Id = 2, Suffix = "minus", Label = "\u2212", SortOrder = 2, CheckItemId = 1 },
            new CosCheckSubRow { Id = 3, Suffix = "plus", Label = "+", SortOrder = 1, CheckItemId = 2 },
            new CosCheckSubRow { Id = 4, Suffix = "minus", Label = "\u2212", SortOrder = 2, CheckItemId = 2 },
            new CosCheckSubRow { Id = 5, Suffix = "L", Label = "L", SortOrder = 1, CheckItemId = 23 },
            new CosCheckSubRow { Id = 6, Suffix = "R", Label = "R", SortOrder = 2, CheckItemId = 23 },
            new CosCheckSubRow { Id = 7, Suffix = "mold1", Label = "Mold 1", SortOrder = 1, CheckItemId = 26 },
            new CosCheckSubRow { Id = 8, Suffix = "mold2", Label = "Mold 2", SortOrder = 2, CheckItemId = 26 },
            new CosCheckSubRow { Id = 9, Suffix = "post1", Label = "Post 1", SortOrder = 3, CheckItemId = 26 },
            new CosCheckSubRow { Id = 10, Suffix = "post2", Label = "Post 2", SortOrder = 4, CheckItemId = 26 },
            new CosCheckSubRow { Id = 11, Suffix = "mold1", Label = "Mold 1", SortOrder = 1, CheckItemId = 27 },
            new CosCheckSubRow { Id = 12, Suffix = "mold2", Label = "Mold 2", SortOrder = 2, CheckItemId = 27 },
            new CosCheckSubRow { Id = 13, Suffix = "line6", Label = "Line 6", FixedStandard = "1 - 3 detik", FixedMin = 1m, FixedMax = 3m, SortOrder = 1, CheckItemId = 31 },
            new CosCheckSubRow { Id = 14, Suffix = "lineOther", Label = "Line 2,3,4,5,7&8", FixedStandard = "0.1 - 1 detik", FixedMin = 0.1m, FixedMax = 1m, SortOrder = 2, CheckItemId = 31 },
            new CosCheckSubRow { Id = 15, Suffix = "line2", Label = "Line 2", FixedStandard = "10 detik", FixedMin = 10m, FixedMax = 10m, SortOrder = 1, CheckItemId = 32 },
            new CosCheckSubRow { Id = 16, Suffix = "line7", Label = "Line 7", FixedStandard = "5 detik", FixedMin = 5m, FixedMax = 5m, SortOrder = 2, CheckItemId = 32 }
        );

        // ══════════════════════════════════════════
        // PROBLEM COLUMNS
        // ══════════════════════════════════════════
        mb.Entity<CosProblemColumn>().HasData(
            new CosProblemColumn { Id = 1, FormId = 1, ColumnKey = "item", Label = "ITEM", FieldType = "text", Width = "120px", SortOrder = 1 },
            new CosProblemColumn { Id = 2, FormId = 1, ColumnKey = "masalah", Label = "MASALAH", FieldType = "text", Width = "200px", SortOrder = 2 },
            new CosProblemColumn { Id = 3, FormId = 1, ColumnKey = "tindakan", Label = "TINDAKAN", FieldType = "text", Width = "200px", SortOrder = 3 },
            new CosProblemColumn { Id = 4, FormId = 1, ColumnKey = "waktu", Label = "WAKTU", FieldType = "text", Width = "80px", SortOrder = 4 },
            new CosProblemColumn { Id = 5, FormId = 1, ColumnKey = "menit", Label = "MENIT", FieldType = "number", Width = "60px", SortOrder = 5 },
            new CosProblemColumn { Id = 6, FormId = 1, ColumnKey = "pic", Label = "PIC", FieldType = "text", Width = "100px", SortOrder = 6 }
        );

        // ══════════════════════════════════════════
        // SIGNATURE SLOTS
        // ══════════════════════════════════════════
        mb.Entity<CosSignatureSlot>().HasData(
            new CosSignatureSlot { Id = 1, FormId = 1, RoleKey = "operator", Label = "Dibuat", SortOrder = 1 },
            new CosSignatureSlot { Id = 2, FormId = 1, RoleKey = "leader", Label = "Diperiksa", SortOrder = 2 },
            new CosSignatureSlot { Id = 3, FormId = 1, RoleKey = "kasubsie", Label = "Diketahui", SortOrder = 3 },
            new CosSignatureSlot { Id = 4, FormId = 1, RoleKey = "kasie", Label = "Disetujui", SortOrder = 4 }
        );

        // ══════════════════════════════════════════
        // MASTER DATA SAMPLE DATA (for development)
        // ══════════════════════════════════════════
        mb.Entity<TlkpShift>().HasData(
            new TlkpShift { ShiftId = 1, ShiftName = "Shift 1", ShiftStart = new TimeSpan(7, 0, 0), ShiftEnd = new TimeSpan(15, 0, 0), ShiftStatus = 1, ShiftCode = "S1" },
            new TlkpShift { ShiftId = 2, ShiftName = "Shift 2", ShiftStart = new TimeSpan(15, 0, 0), ShiftEnd = new TimeSpan(23, 0, 0), ShiftStatus = 1, ShiftCode = "S2" },
            new TlkpShift { ShiftId = 3, ShiftName = "Shift 3", ShiftStart = new TimeSpan(23, 0, 0), ShiftEnd = new TimeSpan(7, 0, 0), ShiftStatus = 1, ShiftCode = "S3" }
        );

        mb.Entity<TlkpSection>().HasData(
            new TlkpSection { SecId = 1, SecName = "COS Section", SecStatus = 1, DepId = 1, SecKode = "COS" }
        );

        mb.Entity<TlkpLine>().HasData(
            new TlkpLine { LineId = 2, LineName = "Line 2", LineStatus = 1, SecId = 1 },
            new TlkpLine { LineId = 3, LineName = "Line 3", LineStatus = 1, SecId = 1 },
            new TlkpLine { LineId = 4, LineName = "Line 4", LineStatus = 1, SecId = 1 },
            new TlkpLine { LineId = 5, LineName = "Line 5", LineStatus = 1, SecId = 1 },
            new TlkpLine { LineId = 6, LineName = "Line 6", LineStatus = 1, SecId = 1 },
            new TlkpLine { LineId = 7, LineName = "Line 7", LineStatus = 1, SecId = 1 },
            new TlkpLine { LineId = 8, LineName = "Line 8", LineStatus = 1, SecId = 1 }
        );

        mb.Entity<TlkpGroup>().HasData(
            new TlkpGroup { GroupId = 1, GroupName = "Group A", GroupStatus = 1 },
            new TlkpGroup { GroupId = 2, GroupName = "Group B", GroupStatus = 1 },
            new TlkpGroup { GroupId = 3, GroupName = "Group C", GroupStatus = 1 }
        );

        mb.Entity<TlkpJob>().HasData(
            new TlkpJob { JoprId = 1, JoprName = "Operator COS", JoprStatus = 1, SecId = 1 }
        );

        mb.Entity<TlkpUserKasie>().HasData(
            new TlkpUserKasie { UserKasieId = 1, SecId = 1, KasieEmpId = "EMP-KASIE-001" }
        );

        mb.Entity<TlkpLineGroup>().HasData(
            new TlkpLineGroup { LgpId = 1, LineId = 2, GroupId = 1, LgpStatus = 1, LgpLeader = "EMP-LEADER-001", LgpKasubsie = "EMP-KASUBSIE-001", UserKasieId = 1 },
            new TlkpLineGroup { LgpId = 2, LineId = 3, GroupId = 1, LgpStatus = 1, LgpLeader = "EMP-LEADER-001", LgpKasubsie = "EMP-KASUBSIE-001", UserKasieId = 1 },
            new TlkpLineGroup { LgpId = 3, LineId = 4, GroupId = 2, LgpStatus = 1, LgpLeader = "EMP-LEADER-002", LgpKasubsie = "EMP-KASUBSIE-001", UserKasieId = 1 },
            new TlkpLineGroup { LgpId = 4, LineId = 5, GroupId = 2, LgpStatus = 1, LgpLeader = "EMP-LEADER-002", LgpKasubsie = "EMP-KASUBSIE-001", UserKasieId = 1 }
        );

        mb.Entity<TlkpOperator>().HasData(
            new TlkpOperator { UserId = "EMP-OPR-001", JoprId = 1, LgpId = 1, GroupId = 1, UserKasieId = 1 },
            new TlkpOperator { UserId = "EMP-OPR-002", JoprId = 1, LgpId = 1, GroupId = 1, UserKasieId = 1 },
            new TlkpOperator { UserId = "EMP-OPR-003", JoprId = 1, LgpId = 2, GroupId = 1, UserKasieId = 1 },
            new TlkpOperator { UserId = "EMP-OPR-004", JoprId = 1, LgpId = 3, GroupId = 2, UserKasieId = 1 },
            new TlkpOperator { UserId = "EMP-OPR-005", JoprId = 1, LgpId = 3, GroupId = 2, UserKasieId = 1 },
            new TlkpOperator { UserId = "EMP-OPR-006", JoprId = 1, LgpId = 4, GroupId = 2, UserKasieId = 1 }
        );

        mb.Entity<ViewDataAuth>().HasData(
            new ViewDataAuth { IdRecnum = 1, EmpId = "EMP-OPR-001", EmpNo = "1001", FullName = "Ahmad Rizky", Status = 1 },
            new ViewDataAuth { IdRecnum = 2, EmpId = "EMP-OPR-002", EmpNo = "1002", FullName = "Budi Santoso", Status = 1 },
            new ViewDataAuth { IdRecnum = 3, EmpId = "EMP-OPR-003", EmpNo = "1003", FullName = "Cahya Dewi", Status = 1 },
            new ViewDataAuth { IdRecnum = 4, EmpId = "EMP-OPR-004", EmpNo = "1004", FullName = "Dian Permata", Status = 1 },
            new ViewDataAuth { IdRecnum = 5, EmpId = "EMP-OPR-005", EmpNo = "1005", FullName = "Eko Prasetyo", Status = 1 },
            new ViewDataAuth { IdRecnum = 6, EmpId = "EMP-OPR-006", EmpNo = "1006", FullName = "Faisal Rahman", Status = 1 },
            new ViewDataAuth { IdRecnum = 7, EmpId = "EMP-LEADER-001", EmpNo = "2001", FullName = "Hendra Wijaya", Status = 1 },
            new ViewDataAuth { IdRecnum = 8, EmpId = "EMP-LEADER-002", EmpNo = "2002", FullName = "Irfan Hakim", Status = 1 },
            new ViewDataAuth { IdRecnum = 9, EmpId = "EMP-KASUBSIE-001", EmpNo = "3001", FullName = "Joko Prasetyo", Status = 1 },
            new ViewDataAuth { IdRecnum = 10, EmpId = "EMP-KASIE-001", EmpNo = "4001", FullName = "Kurniawan Adi", Status = 1 }
        );

        mb.Entity<TlkpMold>().HasData(
            new TlkpMold { MoldCode = "COS-A01", MoldDescription = "Mold COS A01", MoldStatus = "1", IdSection = 1 },
            new TlkpMold { MoldCode = "COS-A02", MoldDescription = "Mold COS A02", MoldStatus = "1", IdSection = 1 },
            new TlkpMold { MoldCode = "COS-B01", MoldDescription = "Mold COS B01", MoldStatus = "1", IdSection = 1 },
            new TlkpMold { MoldCode = "COS-B02", MoldDescription = "Mold COS B02", MoldStatus = "1", IdSection = 1 },
            new TlkpMold { MoldCode = "COS-C01", MoldDescription = "Mold COS C01", MoldStatus = "1", IdSection = 1 },
            new TlkpMold { MoldCode = "COS-C02", MoldDescription = "Mold COS C02", MoldStatus = "1", IdSection = 1 }
        );
    }
}
