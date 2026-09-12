using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Infrastructure.Data.Configurations;

public class RecoveryAnalysisConfiguration : IEntityTypeConfiguration<RecoveryAnalysis>
{
    public void Configure(EntityTypeBuilder<RecoveryAnalysis> builder)
    {
        builder.ToTable("RecoveryAnalyses");

        builder.Property(r => r.RecommendedAction).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(r => r.PaymentId).IsUnique();
        builder.HasIndex(r => r.ExecutedAt);
    }
}
