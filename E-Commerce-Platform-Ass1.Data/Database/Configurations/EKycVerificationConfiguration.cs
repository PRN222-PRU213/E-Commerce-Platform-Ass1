using E_Commerce_Platform_Ass1.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass1.Data.Database.Configurations
{
    public class EKycVerificationConfiguration : IEntityTypeConfiguration<EKycVerification>
    {
        public void Configure(EntityTypeBuilder<EKycVerification> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.CccdNumber)
                   .IsRequired()
                   .HasMaxLength(12); // CCCD Việt Nam có 12 số

            builder.Property(e => e.FullName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.FaceMatchScore)
                   .IsRequired();

            builder.Property(e => e.Liveness)
                   .IsRequired();

            builder.Property(e => e.Status)
                   .IsRequired()
                   .HasMaxLength(20); // PENDING, VERIFIED, FAILED

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            // Relationship với User
            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Index cho UserId để query nhanh hơn
            builder.HasIndex(e => e.UserId);
            
            // Index cho CccdNumber (unique nếu cần)
            builder.HasIndex(e => e.CccdNumber);
        }
    }
}
