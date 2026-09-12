using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Infrastructure.Data.Configurations;

public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.ToTable("PaymentAttempts");

        builder.Property(a => a.ResultStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.DeclineReason).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Notes).HasMaxLength(500);

        builder.HasIndex(a => a.PaymentId);
    }
}
