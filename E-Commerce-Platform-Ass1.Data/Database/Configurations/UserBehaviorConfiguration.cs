using E_Commerce_Platform_Ass1.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass1.Data.Database.Configurations
{
    public class UserBehaviorConfiguration : IEntityTypeConfiguration<UserBehavior>
    {
        public void Configure(EntityTypeBuilder<UserBehavior> builder)
        {
            builder.ToTable("UserBehaviors");

            builder.HasKey(ub => ub.Id);

            builder.Property(ub => ub.ActionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ub => ub.SearchQuery)
                .HasMaxLength(500);

            builder.Property(ub => ub.Metadata)
                .HasMaxLength(2000);

            builder.Property(ub => ub.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(ub => ub.User)
                .WithMany()
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ub => ub.Product)
                .WithMany()
                .HasForeignKey(ub => ub.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ub => ub.Category)
                .WithMany()
                .HasForeignKey(ub => ub.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes for performance
            builder.HasIndex(ub => ub.UserId);
            builder.HasIndex(ub => ub.ProductId);
            builder.HasIndex(ub => ub.ActionType);
            builder.HasIndex(ub => ub.CreatedAt);
            builder.HasIndex(ub => new { ub.UserId, ub.CreatedAt });
        }
    }
}
