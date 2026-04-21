using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<InteractionLog> Interactions => Set<InteractionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var iso8601 = new ValueConverter<DateTime, string>(
            v => v.ToUniversalTime().ToString("o"),
            v => DateTime.Parse(v, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind));

        modelBuilder.Entity<InteractionLog>(e =>
        {
            e.ToTable("interactions");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.RawNote).HasColumnName("raw_note").IsRequired();
            e.Property(x => x.EnhancedText).HasColumnName("enhanced_text");
            e.Property(x => x.Model).HasColumnName("model");
            e.Property(x => x.PromptTokens).HasColumnName("prompt_tokens");
            e.Property(x => x.CompletionTokens).HasColumnName("completion_tokens");
            e.Property(x => x.TotalTokens).HasColumnName("total_tokens");
            e.Property(x => x.LatencyMs).HasColumnName("latency_ms").IsRequired();
            e.Property(x => x.Outcome).HasColumnName("outcome").IsRequired();
            e.Property(x => x.ErrorDetail).HasColumnName("error_detail");
            e.Property(x => x.Timestamp).HasColumnName("timestamp").HasConversion(iso8601).IsRequired();
            e.HasIndex(x => x.Timestamp);
        });
    }
}
