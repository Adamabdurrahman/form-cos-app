using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

/// <summary>
/// DbContext for db_cos_checksheet (Group B master + Group C transaction).
/// Contains all COS-specific tables that were split from db_master_data.
/// </summary>
public class CosDbContext : DbContext
{
    public CosDbContext(DbContextOptions<CosDbContext> options) : base(options) { }

    // ═══ Group B: COS Master/Lookup Tables (tlkp_cos_*) ═══
    public DbSet<CosFormDefinition> CosFormDefinitions => Set<CosFormDefinition>();
    public DbSet<CosCheckItem> CosCheckItems => Set<CosCheckItem>();
    public DbSet<CosCheckSubRow> CosCheckSubRows => Set<CosCheckSubRow>();
    public DbSet<CosBatteryType> CosBatteryTypes => Set<CosBatteryType>();
    public DbSet<CosBatteryStandard> CosBatteryStandards => Set<CosBatteryStandard>();
    public DbSet<CosProblemColumn> CosProblemColumns => Set<CosProblemColumn>();
    public DbSet<CosSignatureSlot> CosSignatureSlots => Set<CosSignatureSlot>();
    public DbSet<CosEmployeeSignature> CosEmployeeSignatures => Set<CosEmployeeSignature>();

    // ═══ Group C: COS Transaction Tables (t_cos_*) ═══
    public DbSet<CosSubmission> CosSubmissions => Set<CosSubmission>();
    public DbSet<CosCheckValue> CosCheckValues => Set<CosCheckValue>();
    public DbSet<CosProblem> CosProblems => Set<CosProblem>();
    public DbSet<CosSignatureEntry> CosSignatureEntries => Set<CosSignatureEntry>();
    public DbSet<CosApprovalAttachment> CosApprovalAttachments => Set<CosApprovalAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ═══ CosBatteryType extra mapping ═══
        modelBuilder.Entity<CosBatteryType>(entity =>
        {
            entity.Property(e => e.SourceItemNum)
                .HasMaxLength(255)
                .HasColumnName("source_item_num");

            entity.Property(e => e.KatId)
                .HasColumnName("kat_id");
        });

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
        modelBuilder.Entity<CosSubmission>()
            .HasMany(s => s.ApprovalAttachments).WithOne(a => a.Submission).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CosFormDefinition>()
            .HasMany(f => f.CheckItems).WithOne(c => c.Form).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CosFormDefinition>()
            .HasMany(f => f.ProblemColumns).WithOne(p => p.Form).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CosFormDefinition>()
            .HasMany(f => f.SignatureSlots).WithOne(s => s.Form).OnDelete(DeleteBehavior.Cascade);

        // NOTE: Seed data is NOT included here.
        // Data will be migrated from the old database via MigrationScript.sql.
        // After migration, seed data is no longer needed since data already exists.
    }
}
