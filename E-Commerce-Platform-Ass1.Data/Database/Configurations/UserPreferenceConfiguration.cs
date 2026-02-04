using E_Commerce_Platform_Ass1.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass1.Data.Database.Configurations
{
    public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
    {
        public void Configure(EntityTypeBuilder<UserPreference> builder)
        {
            builder.ToTable("UserPreferences");

            builder.HasKey(up => up.Id);

            builder.Property(up => up.PreferredCategories)
                .HasMaxLength(2000);

            builder.Property(up => up.MinPriceRange)
                .HasPrecision(18, 2);

            builder.Property(up => up.MaxPriceRange)
                .HasPrecision(18, 2);

            builder.Property(up => up.StylePreferences)
                .HasMaxLength(1000);

            builder.Property(up => up.PreferredBrands)
                .HasMaxLength(1000);

            builder.Property(up => up.CreatedAt)
                .IsRequired();

            builder.Property(up => up.UpdatedAt)
                .IsRequired();

            // Relationship - one preference per user
            builder.HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Unique constraint - one preference per user
            builder.HasIndex(up => up.UserId)
                .IsUnique();
        }
    }
}
