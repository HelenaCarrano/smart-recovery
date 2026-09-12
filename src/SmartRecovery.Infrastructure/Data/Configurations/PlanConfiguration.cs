using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Infrastructure.Data.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.Property(p => p.Name).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Price).HasColumnType("decimal(12,2)");
        builder.Property(p => p.Periodicity).HasConversion<string>().HasMaxLength(20);

        builder.HasMany(p => p.Subscriptions)
            .WithOne(s => s.Plan)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
