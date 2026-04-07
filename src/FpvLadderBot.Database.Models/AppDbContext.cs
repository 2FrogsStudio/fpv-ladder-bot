using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FpvLadderBot;

public abstract class AppDbContext(DbContextOptions options) : DbContext(options) {
    public DbSet<PipelineStateEntity> PipelineState { get; set; }
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }
    public DbSet<BackNavigationEntity> BackNavigations { get; set; }
    public DbSet<PilotEntity> Pilots { get; set; }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default) {
        FillDatedEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess) {
        FillDatedEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    private void FillDatedEntities() {
        var nowLazy = new Lazy<DateTimeOffset>(() => DateTimeOffset.UtcNow);
        foreach (EntityEntry<IDatedEntity> entry in ChangeTracker.Entries<IDatedEntity>()) {
            IDatedEntity entity = entry.Entity;
            switch (entry.State) {
                case EntityState.Added:
                    entity.Created = nowLazy.Value;
                    entity.Updated = nowLazy.Value;
                    break;
                case EntityState.Modified:
                    entity.Updated = nowLazy.Value;
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<PilotEntity>()
            .HasMany(e => e.Subscriptions)
            .WithOne(e => e.Pilot)
            .HasForeignKey(e => e.PilotId)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
