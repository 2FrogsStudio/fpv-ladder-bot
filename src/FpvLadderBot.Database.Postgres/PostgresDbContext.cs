using Microsoft.EntityFrameworkCore.Metadata;

namespace FpvLadderBot;

public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : AppDbContext(options) {
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasCollation("case_insensitive", "en-u-ks-primary", "icu", false);

        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes()) // DEV-60499
        {
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset))
                {
                    property.SetValueConverter(new DateTimeOffsetConverter());
                }
                else if (property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(new NullableDateTimeOffsetConverter());
                }
            }
        }
        
        base.OnModelCreating(modelBuilder);
    }
}
