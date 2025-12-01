using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.IdentificationNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .IsRequired();

            builder.HasMany(u => u.UserDepartments)
                .WithOne(ud => ud.User)
                .HasForeignKey(ud => ud.UserId);
        }
    }
}
