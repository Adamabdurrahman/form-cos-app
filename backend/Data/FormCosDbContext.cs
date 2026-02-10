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

    // Check items
    public DbSet<CheckItem> CheckItems => Set<CheckItem>();
    public DbSet<CheckSubRow> CheckSubRows => Set<CheckSubRow>();

    // Form submissions
    public DbSet<CosValidation> CosValidations => Set<CosValidation>();
    public DbSet<CosCheckSetting> CosCheckSettings => Set<CosCheckSetting>();
    public DbSet<CosProblem> CosProblems => Set<CosProblem>();
    public DbSet<CosSignature> CosSignatures => Set<CosSignature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraints
        modelBuilder.Entity<BatteryStandard>()
            .HasIndex(bs => new { bs.BatteryTypeId, bs.ParamKey })
            .IsUnique();

        modelBuilder.Entity<CheckItem>()
            .HasIndex(ci => ci.ItemKey)
            .IsUnique();

        modelBuilder.Entity<CosCheckSetting>()
            .HasIndex(cs => new { cs.CosValidationId, cs.SettingKey })
            .IsUnique();

        modelBuilder.Entity<CosSignature>()
            .HasIndex(s => new { s.CosValidationId, s.Role })
            .IsUnique();

        // Cascade deletes for form children
        modelBuilder.Entity<CosValidation>()
            .HasMany(v => v.CheckSettings)
            .WithOne(c => c.CosValidation)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CosValidation>()
            .HasMany(v => v.Problems)
            .WithOne(p => p.CosValidation)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CosValidation>()
            .HasMany(v => v.Signatures)
            .WithOne(s => s.CosValidation)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed reference data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // ── Kasie ──
        modelBuilder.Entity<Kasie>().HasData(
            new Kasie { Id = 1, Name = "Kurniawan Adi" },
            new Kasie { Id = 2, Name = "Sutrisno Hadi" }
        );

        // ── Kasubsie ──
        modelBuilder.Entity<Kasubsie>().HasData(
            new Kasubsie { Id = 1, Name = "Joko Prasetyo", KasieId = 1 },
            new Kasubsie { Id = 2, Name = "Mulyono Slamet", KasieId = 1 },
            new Kasubsie { Id = 3, Name = "Teguh Wibowo", KasieId = 2 }
        );

        // ── Leader ──
        modelBuilder.Entity<Leader>().HasData(
            new Leader { Id = 1, Name = "Hendra Wijaya", KasubsieId = 1 },
            new Leader { Id = 2, Name = "Irfan Hakim", KasubsieId = 1 },
            new Leader { Id = 3, Name = "Fajar Nugroho", KasubsieId = 2 },
            new Leader { Id = 4, Name = "Dedi Kurniawan", KasubsieId = 3 }
        );

        // ── Operator ──
        modelBuilder.Entity<Operator>().HasData(
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
        modelBuilder.Entity<BatteryType>().HasData(
            new BatteryType { Id = 1, Name = "NS40ZL" },
            new BatteryType { Id = 2, Name = "NS60L" },
            new BatteryType { Id = 3, Name = "34B19LS" },
            new BatteryType { Id = 4, Name = "N50Z" },
            new BatteryType { Id = 5, Name = "N70Z" },
            new BatteryType { Id = 6, Name = "34B19LS OE TYT" }
        );

        // ── Battery Molds ──
        int moldId = 1;
        modelBuilder.Entity<BatteryMold>().HasData(
            // NS40ZL
            new BatteryMold { Id = moldId++, Name = "COS-A01", BatteryTypeId = 1 },
            new BatteryMold { Id = moldId++, Name = "COS-A02", BatteryTypeId = 1 },
            new BatteryMold { Id = moldId++, Name = "COS-A03", BatteryTypeId = 1 },
            // NS60L
            new BatteryMold { Id = moldId++, Name = "COS-B01", BatteryTypeId = 2 },
            new BatteryMold { Id = moldId++, Name = "COS-B02", BatteryTypeId = 2 },
            // 34B19LS
            new BatteryMold { Id = moldId++, Name = "COS-C01", BatteryTypeId = 3 },
            new BatteryMold { Id = moldId++, Name = "COS-C02", BatteryTypeId = 3 },
            new BatteryMold { Id = moldId++, Name = "COS-C03", BatteryTypeId = 3 },
            // N50Z
            new BatteryMold { Id = moldId++, Name = "COS-D01", BatteryTypeId = 4 },
            new BatteryMold { Id = moldId++, Name = "COS-D02", BatteryTypeId = 4 },
            // N70Z
            new BatteryMold { Id = moldId++, Name = "COS-E01", BatteryTypeId = 5 },
            new BatteryMold { Id = moldId++, Name = "COS-E02", BatteryTypeId = 5 },
            new BatteryMold { Id = moldId++, Name = "COS-E03", BatteryTypeId = 5 },
            // 34B19LS OE TYT
            new BatteryMold { Id = moldId++, Name = "COS-F01", BatteryTypeId = 6 },
            new BatteryMold { Id = moldId++, Name = "COS-F02", BatteryTypeId = 6 }
        );

        // ── Battery Standards ──
        // Mapping: { paramKey -> [type1Val, type2Val, type3Val, type4Val, type5Val, type6Val] }
        var standardsMap = new Dictionary<string, string[]>
        {
            { "pourWait",           new[] { "-", "-", "1.0 - 2.0", "-", "-", "1.0 - 2.0" } },
            { "pourTime",           new[] { "", "2.5 - 4.0", "1.5 - 3.0", "3.0 - 5.0", "3.5 - 5.5", "1.5 - 3.0" } },
            { "dipTime2",           new[] { "", "2.0 - 3.5", "1.0 - 2.5", "2.5 - 4.0", "3.0 - 4.5", "1.0 - 2.5" } },
            { "dumpTime",           new[] { "", "1.5 - 3.0", "0.8 - 2.0", "2.0 - 3.5", "2.5 - 4.0", "0.8 - 2.0" } },
            { "lugDryTime",         new[] { "", "3.5 - 5.5", "2.5 - 4.5", "4.0 - 6.0", "4.5 - 6.5", "2.5 - 4.5" } },
            { "largeVibratorTime",  new[] { "", "1.5 - 3.5", "0.8 - 2.5", "2.0 - 4.0", "2.5 - 4.5", "0.8 - 2.5" } },
            { "smallVibratorTime",  new[] { "", "1.5 - 3.5", "0.8 - 2.5", "2.0 - 4.0", "2.5 - 4.5", "0.8 - 2.5" } },
            { "coolingTime",        new[] { "", "25 - 35", "18 - 28", "28 - 38", "30 - 42", "18 - 28" } },
            { "leadPumpSpeed",      new[] { "", "45 - 65", "35 - 55", "50 - 70", "55 - 75", "35 - 55" } },
            { "tempAirDryer",       new[] { "300 - 400", "310 - 410", "290 - 390", "320 - 420", "330 - 430", "290 - 390" } },
            { "tempPot",            new[] { "470 - 490", "475 - 495", "465 - 485", "480 - 500", "485 - 505", "465 - 485" } },
            { "tempPipe",           new[] { "410 - 430", "415 - 435", "405 - 425", "420 - 440", "425 - 445", "405 - 425" } },
            { "tempCrossBlock",     new[] { "390 - 410", "395 - 415", "385 - 405", "400 - 420", "405 - 425", "385 - 405" } },
            { "tempElbow",          new[] { "370 - 390", "375 - 395", "365 - 385", "380 - 400", "385 - 405", "365 - 385" } },
            { "tempMold",           new[] { "160 - 190", "165 - 195", "155 - 185", "170 - 200", "175 - 205", "155 - 185" } },
            { "coolingFlowRate",    new[] { "6 - 10", "7 - 11", "5 - 9", "8 - 12", "9 - 13", "5 - 9" } },
        };

        int stdId = 1;
        var standardEntities = new List<BatteryStandard>();
        foreach (var kvp in standardsMap)
        {
            for (int typeIdx = 0; typeIdx < 6; typeIdx++)
            {
                standardEntities.Add(new BatteryStandard
                {
                    Id = stdId++,
                    ParamKey = kvp.Key,
                    Value = kvp.Value[typeIdx],
                    BatteryTypeId = typeIdx + 1
                });
            }
        }
        modelBuilder.Entity<BatteryStandard>().HasData(standardEntities.ToArray());

        // ── Check Items ──
        var checkItemsData = new (string key, string label, string type, string? visSt, string? numKey, string? fixSt, string freq, string? ket, string? condLabel)[]
        {
            ("kekuatanCastingStrap", "Kekuatan Casting Strap", "visual", "Ditarik tidak lepas", null, null, "1 batt / shift / ganti type", null, null),
            ("meniscus", "Meniscus", "visual", "Positif", null, null, "1 batt / shift / ganti type", null, null),
            ("hasilCastingStrap", "Hasil Casting Strap", "visual", "Tidak ada flash", null, null, "1 Batt / shift / ganti type", null, null),
            ("levelFlux", "Level Flux", "visual", "Terisi Flux", null, null, "", null, null),
            ("pourWait", "Pour Wait (Khusus Line 8)", "numeric", null, "pourWait", null, "2 x / Shift / ganti type", null, "Khusus Line 8"),
            ("pourTime", "Pour Time", "numeric", null, "pourTime", null, "", null, null),
            ("dipTime2", "Dip Time 2", "numeric", null, "dipTime2", null, "", null, null),
            ("dumpTime", "Dump Time (Drain back time)", "numeric", null, "dumpTime", null, "", null, null),
            ("lugDryTime", "Lug Dry Time", "numeric", null, "lugDryTime", null, "2 x / Shift / ganti type", "untuk 34B19LS OE TYT", null),
            ("largeVibratorTime", "Large Vibrator Time", "numeric", null, "largeVibratorTime", null, "", null, null),
            ("smallVibratorTime", "Small Vibrator Time", "numeric", null, "smallVibratorTime", null, "", null, null),
            ("coolingTime", "Cooling Time", "numeric", null, "coolingTime", null, "", null, null),
            ("leadPumpSpeed", "Lead Pump Speed", "numeric", null, "leadPumpSpeed", null, "", null, null),
            ("checkAlignment", "Check Alignment", "visual", "Bergerak", null, null, "1 x / shift", null, null),
            ("checkDatumTable", "Check Datum Table Alignment", "visual", "Bersih", null, null, "1 x / shift", "Tidak ada ceceran pasta", null),
            ("cleaningNozzle", "Cleaning of Nozzle Lug Dry", "visual", "Bersih", null, null, "1 x / shift", "Spray dengan udara", null),
            ("tempAirNozzleLugDry", "Temp Air Nozzle Lug Dry", "numeric", null, null, "> 275° C", "2 x / shift", "Cek dgn Thermocouple", null),
            ("tempAirDryer", "Temp Air Dryer (hot air)", "numeric", null, "tempAirDryer", null, "2 x / shift", null, null),
            ("blowerPipeTemp", "Blower Pipe Temp (Khusus Line 7)", "numeric", null, null, "> 300° C", "2 x / shift", null, "Khusus Line 7"),
            ("blowerNozzle1Temp", "Blower Nozzle 1 Temp (Khusus Line 7)", "numeric", null, null, "> 200° C", "2 x / shift", null, "Khusus Line 7"),
            ("blowerNozzle2Temp", "Blower Nozzle 2 Temp (Khusus Line 7)", "numeric", null, null, "> 200° C", "2 x / shift", null, "Khusus Line 7"),
            ("tempPot", "Temperatur Pot", "numeric", null, "tempPot", null, "2 x / shift", null, null),
            ("tempPipe", "Temperatur Pipe", "numeric", null, "tempPipe", null, "2 x / shift", null, null),
            ("tempCrossBlock", "Temp. Cross Block", "numeric", null, "tempCrossBlock", null, "2 x / shift", null, null),
            ("tempElbow", "Temp. Elbow (Lead Lump)", "numeric", null, "tempElbow", null, "2 x / shift", null, null),
            ("tempMold", "Temperatur Mold", "numeric", null, "tempMold", null, "2 x / shift", null, null),
            ("coolingFlowRate", "Cooling Water Flow Rate", "numeric", null, "coolingFlowRate", null, "2 x / shift", null, null),
            ("coolingWaterTemp", "Cooling Water Temperature", "numeric", null, null, "28 ± 2 °C", "2 x / shift", null, null),
            ("sprueBrush", "Sprue Brush", "visual", "Berfungsi dengan baik", null, null, "2 x / Shift", null, null),
            ("cleaningCavityMold", "Cleaning Cavity Mold COS", "visual", "Tidak tersumbat dross", null, null, "3 x / Shift", null, null),
            ("fluxTime", "Flux Time", "numeric", null, null, null, "1 batt / shift / ganti type", null, null),
            ("overflowHydrazine", "Overflow Hydrazine", "numeric", null, null, null, "1 batt / shift / ganti type", null, null),
        };

        int checkItemId = 1;
        var checkItemEntities = new List<CheckItem>();
        foreach (var (key, label, type, visSt, numKey, fixSt, freq, ket, condLabel) in checkItemsData)
        {
            checkItemEntities.Add(new CheckItem
            {
                Id = checkItemId,
                ItemKey = key,
                Label = label,
                Type = type,
                VisualStandard = visSt,
                NumericStdKey = numKey,
                FixedStandard = fixSt,
                Frequency = freq,
                Keterangan = ket,
                ConditionalLabel = condLabel,
                SortOrder = checkItemId,
            });
            checkItemId++;
        }
        modelBuilder.Entity<CheckItem>().HasData(checkItemEntities.ToArray());

        // ── Check Sub Rows ──
        // Map from itemKey to list of (suffix, label, fixedStandard?)
        var subRowsMap = new Dictionary<string, (string suffix, string label, string? fixedStd)[]>
        {
            { "kekuatanCastingStrap", new[] { ("plus", "+", (string?)null), ("minus", "−", null) } },
            { "meniscus", new[] { ("plus", "+", (string?)null), ("minus", "−", null) } },
            { "tempPipe", new[] { ("L", "L", (string?)null), ("R", "R", null) } },
            { "tempMold", new[] { ("mold1", "Mold 1", (string?)null), ("mold2", "Mold 2", null), ("post1", "Post 1", null), ("post2", "Post 2", null) } },
            { "coolingFlowRate", new[] { ("mold1", "Mold 1", (string?)null), ("mold2", "Mold 2", null) } },
            { "fluxTime", new[] { ("line6", "Line 6", "1 - 3 detik"), ("lineOther", "Line 2,3,4,5,7&8", "0.1 - 1 detik") } },
            { "overflowHydrazine", new[] { ("line2", "Line 2", "10 detik"), ("line7", "Line 7", "5 detik") } },
        };

        int subRowId = 1;
        var subRowEntities = new List<CheckSubRow>();
        foreach (var kvp in subRowsMap)
        {
            var parentItem = checkItemEntities.First(c => c.ItemKey == kvp.Key);
            int subOrder = 1;
            foreach (var (suffix, label, fixedStd) in kvp.Value)
            {
                subRowEntities.Add(new CheckSubRow
                {
                    Id = subRowId++,
                    Suffix = suffix,
                    Label = label,
                    FixedStandard = fixedStd,
                    SortOrder = subOrder++,
                    CheckItemId = parentItem.Id
                });
            }
        }
        modelBuilder.Entity<CheckSubRow>().HasData(subRowEntities.ToArray());
    }
}
