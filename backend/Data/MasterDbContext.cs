using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

/// <summary>
/// DbContext for db_master_data (Group A — general master data tables).
/// These tables are shared across multiple applications.
/// </summary>
public class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

    // ═══ Master Data Tables (Group A) ═══
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

        // NOTE: No seed data here — master data already exists in db_master_data.
        // This context is READ-ONLY for the existing tables.
    }
}
