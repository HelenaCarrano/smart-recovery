using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Infrastructure.Data.Configurations;

public class WebhookEventConfiguration : IEntityTypeConfiguration<WebhookEvent>
{
    public void Configure(EntityTypeBuilder<WebhookEvent> builder)
    {
        builder.ToTable("WebhookEvents");

        builder.Property(w => w.IdempotencyKey).HasMaxLength(200).IsRequired();
        builder.Property(w => w.EventType).HasMaxLength(100).IsRequired();
        builder.Property(w => w.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(w => w.ErrorMessage).HasMaxLength(1000);

        builder.HasIndex(w => w.IdempotencyKey).IsUnique();
    }
}
