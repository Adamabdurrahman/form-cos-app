using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class FormCosDbContext : DbContext
{
    public FormCosDbContext(DbContextOptions<FormCosDbContext> options) : base(options) { }

    // Personnel hierarchy
    public DbSet<Kasie> Kasies => Set<Kasie>();
    public DbSet<Kasubsie> Kasubsies => Set<Kasubsie>();
    public DbSet<Leader> Leaders => Set<Leader>();
    public DbSet<Operator> Operators => Set<Operator>();

    // Battery config
    public DbSet<BatteryType> BatteryTypes => Set<BatteryType>();
    public DbSet<BatteryMold> BatteryMolds => Set<BatteryMold>();
    public DbSet<BatteryStandard> BatteryStandards => Set<BatteryStandard>();

    // Form definitions
    public DbSet<FormDefinition> FormDefinitions => Set<FormDefinition>();
    public DbSet<CheckItem> CheckItems => Set<CheckItem>();
    public DbSet<CheckSubRow> CheckSubRows => Set<CheckSubRow>();
    public DbSet<FormProblemColumn> FormProblemColumns => Set<FormProblemColumn>();
    public DbSet<FormSignatureSlot> FormSignatureSlots => Set<FormSignatureSlot>();

    // Submissions
    public DbSet<FormSubmission> FormSubmissions => Set<FormSubmission>();
    public DbSet<SubmissionCheckValue> SubmissionCheckValues => Set<SubmissionCheckValue>();
    public DbSet<SubmissionProblem> SubmissionProblems => Set<SubmissionProblem>();
    public DbSet<SubmissionSignature> SubmissionSignatures => Set<SubmissionSignature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraints
        modelBuilder.Entity<FormDefinition>()
            .HasIndex(f => f.Code).IsUnique();

        modelBuilder.Entity<BatteryStandard>()
            .HasIndex(bs => new { bs.BatteryTypeId, bs.ParamKey }).IsUnique();

        modelBuilder.Entity<CheckItem>()
            .HasIndex(ci => new { ci.FormId, ci.ItemKey }).IsUnique();

        modelBuilder.Entity<SubmissionCheckValue>()
            .HasIndex(sv => new { sv.SubmissionId, sv.SettingKey }).IsUnique();

        modelBuilder.Entity<SubmissionSignature>()
            .HasIndex(ss => new { ss.SubmissionId, ss.RoleKey }).IsUnique();

        // Cascades
        modelBuilder.Entity<FormSubmission>()
            .HasMany(s => s.CheckValues).WithOne(v => v.Submission).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<FormSubmission>()
            .HasMany(s => s.Problems).WithOne(p => p.Submission).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<FormSubmission>()
            .HasMany(s => s.Signatures).WithOne(s => s.Submission).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormDefinition>()
            .HasMany(f => f.CheckItems).WithOne(c => c.Form).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<FormDefinition>()
            .HasMany(f => f.ProblemColumns).WithOne(p => p.Form).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<FormDefinition>()
            .HasMany(f => f.SignatureSlots).WithOne(s => s.Form).OnDelete(DeleteBehavior.Cascade);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder mb)
    {
        // ── Form Definition ──
        mb.Entity<FormDefinition>().HasData(new FormDefinition
        {
            Id = 1,
            Code = "COS_VALIDATION",
            Title = "VALIDASI PROSES COS",
            Subtitle = "Form-A2 1-K.051-5-2",
            SlotCount = 3,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        });

        // ── Kasie ──
        mb.Entity<Kasie>().HasData(
            new Kasie { Id = 1, Name = "Kurniawan Adi" },
            new Kasie { Id = 2, Name = "Sutrisno Hadi" }
        );

        // ── Kasubsie ──
        mb.Entity<Kasubsie>().HasData(
            new Kasubsie { Id = 1, Name = "Joko Prasetyo", KasieId = 1 },
            new Kasubsie { Id = 2, Name = "Mulyono Slamet", KasieId = 1 },
            new Kasubsie { Id = 3, Name = "Teguh Wibowo", KasieId = 2 }
        );

        // ── Leader ──
        mb.Entity<Leader>().HasData(
            new Leader { Id = 1, Name = "Hendra Wijaya", KasubsieId = 1 },
            new Leader { Id = 2, Name = "Irfan Hakim", KasubsieId = 1 },
            new Leader { Id = 3, Name = "Fajar Nugroho", KasubsieId = 2 },
            new Leader { Id = 4, Name = "Dedi Kurniawan", KasubsieId = 3 }
        );

        // ── Operator ──
        mb.Entity<Operator>().HasData(
            new Operator { Id = 1, Name = "Ahmad Rizky", LeaderId = 1 },
            new Operator { Id = 2, Name = "Budi Santoso", LeaderId = 1 },
            new Operator { Id = 3, Name = "Cahya Dewi", LeaderId = 2 },
            new Operator { Id = 4, Name = "Dian Permata", LeaderId = 2 },
            new Operator { Id = 5, Name = "Eko Prasetyo", LeaderId = 3 },
            new Operator { Id = 6, Name = "Faisal Rahman", LeaderId = 3 },
            new Operator { Id = 7, Name = "Gunawan Putra", LeaderId = 4 },
            new Operator { Id = 8, Name = "Haris Munandar", LeaderId = 4 }
        );

        // ── Battery Types ──
        mb.Entity<BatteryType>().HasData(
            new BatteryType { Id = 1, Name = "NS40ZL" },
            new BatteryType { Id = 2, Name = "NS60L" },
            new BatteryType { Id = 3, Name = "34B19LS" },
            new BatteryType { Id = 4, Name = "N50Z" },
            new BatteryType { Id = 5, Name = "N70Z" },
            new BatteryType { Id = 6, Name = "34B19LS OE TYT" }
        );

        // ── Battery Molds ──
        mb.Entity<BatteryMold>().HasData(
            new BatteryMold { Id = 1, Name = "COS-A01", BatteryTypeId = 1 },
            new BatteryMold { Id = 2, Name = "COS-A02", BatteryTypeId = 1 },
            new BatteryMold { Id = 3, Name = "COS-A03", BatteryTypeId = 1 },
            new BatteryMold { Id = 4, Name = "COS-B01", BatteryTypeId = 2 },
            new BatteryMold { Id = 5, Name = "COS-B02", BatteryTypeId = 2 },
            new BatteryMold { Id = 6, Name = "COS-C01", BatteryTypeId = 3 },
            new BatteryMold { Id = 7, Name = "COS-C02", BatteryTypeId = 3 },
            new BatteryMold { Id = 8, Name = "COS-C03", BatteryTypeId = 3 },
            new BatteryMold { Id = 9, Name = "COS-D01", BatteryTypeId = 4 },
            new BatteryMold { Id = 10, Name = "COS-D02", BatteryTypeId = 4 },
            new BatteryMold { Id = 11, Name = "COS-E01", BatteryTypeId = 5 },
            new BatteryMold { Id = 12, Name = "COS-E02", BatteryTypeId = 5 },
            new BatteryMold { Id = 13, Name = "COS-E03", BatteryTypeId = 5 },
            new BatteryMold { Id = 14, Name = "COS-F01", BatteryTypeId = 6 },
            new BatteryMold { Id = 15, Name = "COS-F02", BatteryTypeId = 6 }
        );

        // ── Battery Standards with min/max ──
        mb.Entity<BatteryStandard>().HasData(
            // pourWait
            new BatteryStandard { Id = 1, ParamKey = "pourWait", Value = "-", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 2, ParamKey = "pourWait", Value = "-", MinValue = null, MaxValue = null, BatteryTypeId = 2 },
            new BatteryStandard { Id = 3, ParamKey = "pourWait", Value = "1.0 - 2.0", MinValue = 1.0m, MaxValue = 2.0m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 4, ParamKey = "pourWait", Value = "-", MinValue = null, MaxValue = null, BatteryTypeId = 4 },
            new BatteryStandard { Id = 5, ParamKey = "pourWait", Value = "-", MinValue = null, MaxValue = null, BatteryTypeId = 5 },
            new BatteryStandard { Id = 6, ParamKey = "pourWait", Value = "1.0 - 2.0", MinValue = 1.0m, MaxValue = 2.0m, BatteryTypeId = 6 },
            // pourTime
            new BatteryStandard { Id = 7, ParamKey = "pourTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 8, ParamKey = "pourTime", Value = "2.5 - 4.0", MinValue = 2.5m, MaxValue = 4.0m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 9, ParamKey = "pourTime", Value = "1.5 - 3.0", MinValue = 1.5m, MaxValue = 3.0m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 10, ParamKey = "pourTime", Value = "3.0 - 5.0", MinValue = 3.0m, MaxValue = 5.0m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 11, ParamKey = "pourTime", Value = "3.5 - 5.5", MinValue = 3.5m, MaxValue = 5.5m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 12, ParamKey = "pourTime", Value = "1.5 - 3.0", MinValue = 1.5m, MaxValue = 3.0m, BatteryTypeId = 6 },
            // dipTime2
            new BatteryStandard { Id = 13, ParamKey = "dipTime2", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 14, ParamKey = "dipTime2", Value = "2.0 - 3.5", MinValue = 2.0m, MaxValue = 3.5m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 15, ParamKey = "dipTime2", Value = "1.0 - 2.5", MinValue = 1.0m, MaxValue = 2.5m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 16, ParamKey = "dipTime2", Value = "2.5 - 4.0", MinValue = 2.5m, MaxValue = 4.0m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 17, ParamKey = "dipTime2", Value = "3.0 - 4.5", MinValue = 3.0m, MaxValue = 4.5m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 18, ParamKey = "dipTime2", Value = "1.0 - 2.5", MinValue = 1.0m, MaxValue = 2.5m, BatteryTypeId = 6 },
            // dumpTime
            new BatteryStandard { Id = 19, ParamKey = "dumpTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 20, ParamKey = "dumpTime", Value = "1.5 - 3.0", MinValue = 1.5m, MaxValue = 3.0m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 21, ParamKey = "dumpTime", Value = "0.8 - 2.0", MinValue = 0.8m, MaxValue = 2.0m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 22, ParamKey = "dumpTime", Value = "2.0 - 3.5", MinValue = 2.0m, MaxValue = 3.5m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 23, ParamKey = "dumpTime", Value = "2.5 - 4.0", MinValue = 2.5m, MaxValue = 4.0m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 24, ParamKey = "dumpTime", Value = "0.8 - 2.0", MinValue = 0.8m, MaxValue = 2.0m, BatteryTypeId = 6 },
            // lugDryTime
            new BatteryStandard { Id = 25, ParamKey = "lugDryTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 26, ParamKey = "lugDryTime", Value = "3.5 - 5.5", MinValue = 3.5m, MaxValue = 5.5m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 27, ParamKey = "lugDryTime", Value = "2.5 - 4.5", MinValue = 2.5m, MaxValue = 4.5m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 28, ParamKey = "lugDryTime", Value = "4.0 - 6.0", MinValue = 4.0m, MaxValue = 6.0m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 29, ParamKey = "lugDryTime", Value = "4.5 - 6.5", MinValue = 4.5m, MaxValue = 6.5m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 30, ParamKey = "lugDryTime", Value = "2.5 - 4.5", MinValue = 2.5m, MaxValue = 4.5m, BatteryTypeId = 6 },
            // largeVibratorTime
            new BatteryStandard { Id = 31, ParamKey = "largeVibratorTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 32, ParamKey = "largeVibratorTime", Value = "1.5 - 3.5", MinValue = 1.5m, MaxValue = 3.5m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 33, ParamKey = "largeVibratorTime", Value = "0.8 - 2.5", MinValue = 0.8m, MaxValue = 2.5m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 34, ParamKey = "largeVibratorTime", Value = "2.0 - 4.0", MinValue = 2.0m, MaxValue = 4.0m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 35, ParamKey = "largeVibratorTime", Value = "2.5 - 4.5", MinValue = 2.5m, MaxValue = 4.5m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 36, ParamKey = "largeVibratorTime", Value = "0.8 - 2.5", MinValue = 0.8m, MaxValue = 2.5m, BatteryTypeId = 6 },
            // smallVibratorTime
            new BatteryStandard { Id = 37, ParamKey = "smallVibratorTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 38, ParamKey = "smallVibratorTime", Value = "1.5 - 3.5", MinValue = 1.5m, MaxValue = 3.5m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 39, ParamKey = "smallVibratorTime", Value = "0.8 - 2.5", MinValue = 0.8m, MaxValue = 2.5m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 40, ParamKey = "smallVibratorTime", Value = "2.0 - 4.0", MinValue = 2.0m, MaxValue = 4.0m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 41, ParamKey = "smallVibratorTime", Value = "2.5 - 4.5", MinValue = 2.5m, MaxValue = 4.5m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 42, ParamKey = "smallVibratorTime", Value = "0.8 - 2.5", MinValue = 0.8m, MaxValue = 2.5m, BatteryTypeId = 6 },
            // coolingTime
            new BatteryStandard { Id = 43, ParamKey = "coolingTime", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 44, ParamKey = "coolingTime", Value = "25 - 35", MinValue = 25m, MaxValue = 35m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 45, ParamKey = "coolingTime", Value = "18 - 28", MinValue = 18m, MaxValue = 28m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 46, ParamKey = "coolingTime", Value = "28 - 38", MinValue = 28m, MaxValue = 38m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 47, ParamKey = "coolingTime", Value = "30 - 42", MinValue = 30m, MaxValue = 42m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 48, ParamKey = "coolingTime", Value = "18 - 28", MinValue = 18m, MaxValue = 28m, BatteryTypeId = 6 },
            // leadPumpSpeed
            new BatteryStandard { Id = 49, ParamKey = "leadPumpSpeed", Value = "", MinValue = null, MaxValue = null, BatteryTypeId = 1 },
            new BatteryStandard { Id = 50, ParamKey = "leadPumpSpeed", Value = "45 - 65", MinValue = 45m, MaxValue = 65m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 51, ParamKey = "leadPumpSpeed", Value = "35 - 55", MinValue = 35m, MaxValue = 55m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 52, ParamKey = "leadPumpSpeed", Value = "50 - 70", MinValue = 50m, MaxValue = 70m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 53, ParamKey = "leadPumpSpeed", Value = "55 - 75", MinValue = 55m, MaxValue = 75m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 54, ParamKey = "leadPumpSpeed", Value = "35 - 55", MinValue = 35m, MaxValue = 55m, BatteryTypeId = 6 },
            // tempAirDryer
            new BatteryStandard { Id = 55, ParamKey = "tempAirDryer", Value = "300 - 400", MinValue = 300m, MaxValue = 400m, BatteryTypeId = 1 },
            new BatteryStandard { Id = 56, ParamKey = "tempAirDryer", Value = "310 - 410", MinValue = 310m, MaxValue = 410m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 57, ParamKey = "tempAirDryer", Value = "290 - 390", MinValue = 290m, MaxValue = 390m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 58, ParamKey = "tempAirDryer", Value = "320 - 420", MinValue = 320m, MaxValue = 420m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 59, ParamKey = "tempAirDryer", Value = "330 - 430", MinValue = 330m, MaxValue = 430m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 60, ParamKey = "tempAirDryer", Value = "290 - 390", MinValue = 290m, MaxValue = 390m, BatteryTypeId = 6 },
            // tempPot
            new BatteryStandard { Id = 61, ParamKey = "tempPot", Value = "470 - 490", MinValue = 470m, MaxValue = 490m, BatteryTypeId = 1 },
            new BatteryStandard { Id = 62, ParamKey = "tempPot", Value = "475 - 495", MinValue = 475m, MaxValue = 495m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 63, ParamKey = "tempPot", Value = "465 - 485", MinValue = 465m, MaxValue = 485m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 64, ParamKey = "tempPot", Value = "480 - 500", MinValue = 480m, MaxValue = 500m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 65, ParamKey = "tempPot", Value = "485 - 505", MinValue = 485m, MaxValue = 505m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 66, ParamKey = "tempPot", Value = "465 - 485", MinValue = 465m, MaxValue = 485m, BatteryTypeId = 6 },
            // tempPipe
            new BatteryStandard { Id = 67, ParamKey = "tempPipe", Value = "410 - 430", MinValue = 410m, MaxValue = 430m, BatteryTypeId = 1 },
            new BatteryStandard { Id = 68, ParamKey = "tempPipe", Value = "415 - 435", MinValue = 415m, MaxValue = 435m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 69, ParamKey = "tempPipe", Value = "405 - 425", MinValue = 405m, MaxValue = 425m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 70, ParamKey = "tempPipe", Value = "420 - 440", MinValue = 420m, MaxValue = 440m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 71, ParamKey = "tempPipe", Value = "425 - 445", MinValue = 425m, MaxValue = 445m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 72, ParamKey = "tempPipe", Value = "405 - 425", MinValue = 405m, MaxValue = 425m, BatteryTypeId = 6 },
            // tempCrossBlock
            new BatteryStandard { Id = 73, ParamKey = "tempCrossBlock", Value = "390 - 410", MinValue = 390m, MaxValue = 410m, BatteryTypeId = 1 },
            new BatteryStandard { Id = 74, ParamKey = "tempCrossBlock", Value = "395 - 415", MinValue = 395m, MaxValue = 415m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 75, ParamKey = "tempCrossBlock", Value = "385 - 405", MinValue = 385m, MaxValue = 405m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 76, ParamKey = "tempCrossBlock", Value = "400 - 420", MinValue = 400m, MaxValue = 420m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 77, ParamKey = "tempCrossBlock", Value = "405 - 425", MinValue = 405m, MaxValue = 425m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 78, ParamKey = "tempCrossBlock", Value = "385 - 405", MinValue = 385m, MaxValue = 405m, BatteryTypeId = 6 },
            // tempElbow
            new BatteryStandard { Id = 79, ParamKey = "tempElbow", Value = "370 - 390", MinValue = 370m, MaxValue = 390m, BatteryTypeId = 1 },
            new BatteryStandard { Id = 80, ParamKey = "tempElbow", Value = "375 - 395", MinValue = 375m, MaxValue = 395m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 81, ParamKey = "tempElbow", Value = "365 - 385", MinValue = 365m, MaxValue = 385m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 82, ParamKey = "tempElbow", Value = "380 - 400", MinValue = 380m, MaxValue = 400m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 83, ParamKey = "tempElbow", Value = "385 - 405", MinValue = 385m, MaxValue = 405m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 84, ParamKey = "tempElbow", Value = "365 - 385", MinValue = 365m, MaxValue = 385m, BatteryTypeId = 6 },
            // tempMold
            new BatteryStandard { Id = 85, ParamKey = "tempMold", Value = "160 - 190", MinValue = 160m, MaxValue = 190m, BatteryTypeId = 1 },
            new BatteryStandard { Id = 86, ParamKey = "tempMold", Value = "165 - 195", MinValue = 165m, MaxValue = 195m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 87, ParamKey = "tempMold", Value = "155 - 185", MinValue = 155m, MaxValue = 185m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 88, ParamKey = "tempMold", Value = "170 - 200", MinValue = 170m, MaxValue = 200m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 89, ParamKey = "tempMold", Value = "175 - 205", MinValue = 175m, MaxValue = 205m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 90, ParamKey = "tempMold", Value = "155 - 185", MinValue = 155m, MaxValue = 185m, BatteryTypeId = 6 },
            // coolingFlowRate
            new BatteryStandard { Id = 91, ParamKey = "coolingFlowRate", Value = "6 - 10", MinValue = 6m, MaxValue = 10m, BatteryTypeId = 1 },
            new BatteryStandard { Id = 92, ParamKey = "coolingFlowRate", Value = "7 - 11", MinValue = 7m, MaxValue = 11m, BatteryTypeId = 2 },
            new BatteryStandard { Id = 93, ParamKey = "coolingFlowRate", Value = "5 - 9", MinValue = 5m, MaxValue = 9m, BatteryTypeId = 3 },
            new BatteryStandard { Id = 94, ParamKey = "coolingFlowRate", Value = "8 - 12", MinValue = 8m, MaxValue = 12m, BatteryTypeId = 4 },
            new BatteryStandard { Id = 95, ParamKey = "coolingFlowRate", Value = "9 - 13", MinValue = 9m, MaxValue = 13m, BatteryTypeId = 5 },
            new BatteryStandard { Id = 96, ParamKey = "coolingFlowRate", Value = "5 - 9", MinValue = 5m, MaxValue = 9m, BatteryTypeId = 6 }
        );

        // ── Check Items (FormId = 1 for COS) ──
        mb.Entity<CheckItem>().HasData(
            new CheckItem { Id = 1, FormId = 1, ItemKey = "kekuatanCastingStrap", Label = "Kekuatan Casting Strap", Type = "visual", VisualStandard = "Ditarik tidak lepas", Frequency = "1 batt / shift / ganti type", SortOrder = 1 },
            new CheckItem { Id = 2, FormId = 1, ItemKey = "meniscus", Label = "Meniscus", Type = "visual", VisualStandard = "Positif", Frequency = "1 batt / shift / ganti type", SortOrder = 2 },
            new CheckItem { Id = 3, FormId = 1, ItemKey = "hasilCastingStrap", Label = "Hasil Casting Strap", Type = "visual", VisualStandard = "Tidak ada flash", Frequency = "1 Batt / shift / ganti type", SortOrder = 3 },
            new CheckItem { Id = 4, FormId = 1, ItemKey = "levelFlux", Label = "Level Flux", Type = "visual", VisualStandard = "Terisi Flux", SortOrder = 4 },
            new CheckItem { Id = 5, FormId = 1, ItemKey = "pourWait", Label = "Pour Wait (Khusus Line 8)", Type = "numeric", NumericStdKey = "pourWait", Frequency = "2 x / Shift / ganti type", ConditionalLabel = "Khusus Line 8", SortOrder = 5 },
            new CheckItem { Id = 6, FormId = 1, ItemKey = "pourTime", Label = "Pour Time", Type = "numeric", NumericStdKey = "pourTime", SortOrder = 6 },
            new CheckItem { Id = 7, FormId = 1, ItemKey = "dipTime2", Label = "Dip Time 2", Type = "numeric", NumericStdKey = "dipTime2", SortOrder = 7 },
            new CheckItem { Id = 8, FormId = 1, ItemKey = "dumpTime", Label = "Dump Time (Drain back time)", Type = "numeric", NumericStdKey = "dumpTime", SortOrder = 8 },
            new CheckItem { Id = 9, FormId = 1, ItemKey = "lugDryTime", Label = "Lug Dry Time", Type = "numeric", NumericStdKey = "lugDryTime", Frequency = "2 x / Shift / ganti type", Keterangan = "untuk 34B19LS OE TYT", SortOrder = 9 },
            new CheckItem { Id = 10, FormId = 1, ItemKey = "largeVibratorTime", Label = "Large Vibrator Time", Type = "numeric", NumericStdKey = "largeVibratorTime", SortOrder = 10 },
            new CheckItem { Id = 11, FormId = 1, ItemKey = "smallVibratorTime", Label = "Small Vibrator Time", Type = "numeric", NumericStdKey = "smallVibratorTime", SortOrder = 11 },
            new CheckItem { Id = 12, FormId = 1, ItemKey = "coolingTime", Label = "Cooling Time", Type = "numeric", NumericStdKey = "coolingTime", SortOrder = 12 },
            new CheckItem { Id = 13, FormId = 1, ItemKey = "leadPumpSpeed", Label = "Lead Pump Speed", Type = "numeric", NumericStdKey = "leadPumpSpeed", SortOrder = 13 },
            new CheckItem { Id = 14, FormId = 1, ItemKey = "checkAlignment", Label = "Check Alignment", Type = "visual", VisualStandard = "Bergerak", Frequency = "1 x / shift", SortOrder = 14 },
            new CheckItem { Id = 15, FormId = 1, ItemKey = "checkDatumTable", Label = "Check Datum Table Alignment", Type = "visual", VisualStandard = "Bersih", Frequency = "1 x / shift", Keterangan = "Tidak ada ceceran pasta", SortOrder = 15 },
            new CheckItem { Id = 16, FormId = 1, ItemKey = "cleaningNozzle", Label = "Cleaning of Nozzle Lug Dry", Type = "visual", VisualStandard = "Bersih", Frequency = "1 x / shift", Keterangan = "Spray dengan udara", SortOrder = 16 },
            new CheckItem { Id = 17, FormId = 1, ItemKey = "tempAirNozzleLugDry", Label = "Temp Air Nozzle Lug Dry", Type = "numeric", FixedStandard = "> 275\u00b0 C", FixedMin = 275m, Frequency = "2 x / shift", Keterangan = "Cek dgn Thermocouple", SortOrder = 17 },
            new CheckItem { Id = 18, FormId = 1, ItemKey = "tempAirDryer", Label = "Temp Air Dryer (hot air)", Type = "numeric", NumericStdKey = "tempAirDryer", Frequency = "2 x / shift", SortOrder = 18 },
            new CheckItem { Id = 19, FormId = 1, ItemKey = "blowerPipeTemp", Label = "Blower Pipe Temp (Khusus Line 7)", Type = "numeric", FixedStandard = "> 300\u00b0 C", FixedMin = 300m, Frequency = "2 x / shift", ConditionalLabel = "Khusus Line 7", SortOrder = 19 },
            new CheckItem { Id = 20, FormId = 1, ItemKey = "blowerNozzle1Temp", Label = "Blower Nozzle 1 Temp (Khusus Line 7)", Type = "numeric", FixedStandard = "> 200\u00b0 C", FixedMin = 200m, Frequency = "2 x / shift", ConditionalLabel = "Khusus Line 7", SortOrder = 20 },
            new CheckItem { Id = 21, FormId = 1, ItemKey = "blowerNozzle2Temp", Label = "Blower Nozzle 2 Temp (Khusus Line 7)", Type = "numeric", FixedStandard = "> 200\u00b0 C", FixedMin = 200m, Frequency = "2 x / shift", ConditionalLabel = "Khusus Line 7", SortOrder = 21 },
            new CheckItem { Id = 22, FormId = 1, ItemKey = "tempPot", Label = "Temperatur Pot", Type = "numeric", NumericStdKey = "tempPot", Frequency = "2 x / shift", SortOrder = 22 },
            new CheckItem { Id = 23, FormId = 1, ItemKey = "tempPipe", Label = "Temperatur Pipe", Type = "numeric", NumericStdKey = "tempPipe", Frequency = "2 x / shift", SortOrder = 23 },
            new CheckItem { Id = 24, FormId = 1, ItemKey = "tempCrossBlock", Label = "Temp. Cross Block", Type = "numeric", NumericStdKey = "tempCrossBlock", Frequency = "2 x / shift", SortOrder = 24 },
            new CheckItem { Id = 25, FormId = 1, ItemKey = "tempElbow", Label = "Temp. Elbow (Lead Lump)", Type = "numeric", NumericStdKey = "tempElbow", Frequency = "2 x / shift", SortOrder = 25 },
            new CheckItem { Id = 26, FormId = 1, ItemKey = "tempMold", Label = "Temperatur Mold", Type = "numeric", NumericStdKey = "tempMold", Frequency = "2 x / shift", SortOrder = 26 },
            new CheckItem { Id = 27, FormId = 1, ItemKey = "coolingFlowRate", Label = "Cooling Water Flow Rate", Type = "numeric", NumericStdKey = "coolingFlowRate", Frequency = "2 x / shift", SortOrder = 27 },
            new CheckItem { Id = 28, FormId = 1, ItemKey = "coolingWaterTemp", Label = "Cooling Water Temperature", Type = "numeric", FixedStandard = "28 \u00b1 2 \u00b0C", FixedMin = 26m, FixedMax = 30m, Frequency = "2 x / shift", SortOrder = 28 },
            new CheckItem { Id = 29, FormId = 1, ItemKey = "sprueBrush", Label = "Sprue Brush", Type = "visual", VisualStandard = "Berfungsi dengan baik", Frequency = "2 x / Shift", SortOrder = 29 },
            new CheckItem { Id = 30, FormId = 1, ItemKey = "cleaningCavityMold", Label = "Cleaning Cavity Mold COS", Type = "visual", VisualStandard = "Tidak tersumbat dross", Frequency = "3 x / Shift", SortOrder = 30 },
            new CheckItem { Id = 31, FormId = 1, ItemKey = "fluxTime", Label = "Flux Time", Type = "numeric", Frequency = "1 batt / shift / ganti type", SortOrder = 31 },
            new CheckItem { Id = 32, FormId = 1, ItemKey = "overflowHydrazine", Label = "Overflow Hydrazine", Type = "numeric", Frequency = "1 batt / shift / ganti type", SortOrder = 32 }
        );

        // ── Check Sub Rows ──
        mb.Entity<CheckSubRow>().HasData(
            // kekuatanCastingStrap (Id=1)
            new CheckSubRow { Id = 1, Suffix = "plus", Label = "+", SortOrder = 1, CheckItemId = 1 },
            new CheckSubRow { Id = 2, Suffix = "minus", Label = "\u2212", SortOrder = 2, CheckItemId = 1 },
            // meniscus (Id=2)
            new CheckSubRow { Id = 3, Suffix = "plus", Label = "+", SortOrder = 1, CheckItemId = 2 },
            new CheckSubRow { Id = 4, Suffix = "minus", Label = "\u2212", SortOrder = 2, CheckItemId = 2 },
            // tempPipe (Id=23)
            new CheckSubRow { Id = 5, Suffix = "L", Label = "L", SortOrder = 1, CheckItemId = 23 },
            new CheckSubRow { Id = 6, Suffix = "R", Label = "R", SortOrder = 2, CheckItemId = 23 },
            // tempMold (Id=26)
            new CheckSubRow { Id = 7, Suffix = "mold1", Label = "Mold 1", SortOrder = 1, CheckItemId = 26 },
            new CheckSubRow { Id = 8, Suffix = "mold2", Label = "Mold 2", SortOrder = 2, CheckItemId = 26 },
            new CheckSubRow { Id = 9, Suffix = "post1", Label = "Post 1", SortOrder = 3, CheckItemId = 26 },
            new CheckSubRow { Id = 10, Suffix = "post2", Label = "Post 2", SortOrder = 4, CheckItemId = 26 },
            // coolingFlowRate (Id=27)
            new CheckSubRow { Id = 11, Suffix = "mold1", Label = "Mold 1", SortOrder = 1, CheckItemId = 27 },
            new CheckSubRow { Id = 12, Suffix = "mold2", Label = "Mold 2", SortOrder = 2, CheckItemId = 27 },
            // fluxTime (Id=31)
            new CheckSubRow { Id = 13, Suffix = "line6", Label = "Line 6", FixedStandard = "1 - 3 detik", FixedMin = 1m, FixedMax = 3m, SortOrder = 1, CheckItemId = 31 },
            new CheckSubRow { Id = 14, Suffix = "lineOther", Label = "Line 2,3,4,5,7&8", FixedStandard = "0.1 - 1 detik", FixedMin = 0.1m, FixedMax = 1m, SortOrder = 2, CheckItemId = 31 },
            // overflowHydrazine (Id=32)
            new CheckSubRow { Id = 15, Suffix = "line2", Label = "Line 2", FixedStandard = "10 detik", FixedMin = 10m, FixedMax = 10m, SortOrder = 1, CheckItemId = 32 },
            new CheckSubRow { Id = 16, Suffix = "line7", Label = "Line 7", FixedStandard = "5 detik", FixedMin = 5m, FixedMax = 5m, SortOrder = 2, CheckItemId = 32 }
        );

        // ── Problem Columns for COS form ──
        mb.Entity<FormProblemColumn>().HasData(
            new FormProblemColumn { Id = 1, FormId = 1, ColumnKey = "item", Label = "ITEM", FieldType = "text", Width = "120px", SortOrder = 1 },
            new FormProblemColumn { Id = 2, FormId = 1, ColumnKey = "masalah", Label = "MASALAH", FieldType = "text", Width = "200px", SortOrder = 2 },
            new FormProblemColumn { Id = 3, FormId = 1, ColumnKey = "tindakan", Label = "TINDAKAN", FieldType = "text", Width = "200px", SortOrder = 3 },
            new FormProblemColumn { Id = 4, FormId = 1, ColumnKey = "waktu", Label = "WAKTU", FieldType = "text", Width = "80px", SortOrder = 4 },
            new FormProblemColumn { Id = 5, FormId = 1, ColumnKey = "menit", Label = "MENIT", FieldType = "number", Width = "60px", SortOrder = 5 },
            new FormProblemColumn { Id = 6, FormId = 1, ColumnKey = "pic", Label = "PIC", FieldType = "text", Width = "100px", SortOrder = 6 }
        );

        // ── Signature Slots for COS form ──
        mb.Entity<FormSignatureSlot>().HasData(
            new FormSignatureSlot { Id = 1, FormId = 1, RoleKey = "operator", Label = "Dibuat", SortOrder = 1 },
            new FormSignatureSlot { Id = 2, FormId = 1, RoleKey = "leader", Label = "Diperiksa", SortOrder = 2 },
            new FormSignatureSlot { Id = 3, FormId = 1, RoleKey = "kasubsie", Label = "Diketahui", SortOrder = 3 },
            new FormSignatureSlot { Id = 4, FormId = 1, RoleKey = "kasie", Label = "Disetujui", SortOrder = 4 }
        );
    }
}
