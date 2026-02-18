using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

/// <summary>
/// ReadOnly DbContext for db_padCost (production achievement data).
/// Used for auto-detect battery type feature.
/// ⚠ JANGAN gunakan untuk write operations — ini database milik sistem lain.
/// </summary>
public class PadCostDbContext : DbContext
{
    public PadCostDbContext(DbContextOptions<PadCostDbContext> options) : base(options) { }

    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<ActAchievement> ActAchievements => Set<ActAchievement>();
    public DbSet<Absensi> Absensis => Set<Absensi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── t_achievement ──
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.ToTable("t_achievement");
            entity.HasKey(e => e.AchiId);
        });

        // ── t_actAchievement ──
        modelBuilder.Entity<ActAchievement>(entity =>
        {
            entity.ToTable("t_actAchievement");
            entity.HasKey(e => e.ActlId);

            // FK: t_actAchievement.achi_id → t_achievement.achi_id
            entity.HasOne(e => e.Achievement)
                  .WithMany(a => a.ActAchievements)
                  .HasForeignKey(e => e.AchiId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── t_absensi (Composite Key) ──
        modelBuilder.Entity<Absensi>(entity =>
        {
            entity.ToTable("t_absensi");
            entity.HasKey(e => new { e.AchiId, e.AbseNpk });

            // FK: t_absensi.achi_id → t_achievement.achi_id
            entity.HasOne(e => e.Achievement)
                  .WithMany(a => a.Absensis)
                  .HasForeignKey(e => e.AchiId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
