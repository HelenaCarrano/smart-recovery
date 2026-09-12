using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.Property(p => p.Amount).HasColumnType("decimal(12,2)");
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.DeclineReason).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(p => p.CustomerId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.ScheduledRetryAt);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.DeclineReason);

        builder.HasMany(p => p.Attempts)
            .WithOne(a => a.Payment)
            .HasForeignKey(a => a.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.RecoveryAnalysis)
            .WithOne(r => r.Payment)
            .HasForeignKey<RecoveryAnalysis>(r => r.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
