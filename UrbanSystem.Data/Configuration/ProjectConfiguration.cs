using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static UrbanSystem.Common.ValidationConstants.Project;
using UrbanSystem.Data.Models;

namespace UrbanSystem.Data.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder
                .HasOne(p => p.Location)
                .WithMany(l => l.Projects)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(NameMaxLength);

            builder.Property(p => p.Description)
                   .IsRequired()
                   .HasMaxLength(DescriptionMaxLength);

            builder.Property(p => p.FundingDeadline)
                .IsRequired();

            builder.Property(p => p.FundsNeeded)
                .IsRequired()
                .HasPrecision(FundsPrecision, FundsScale);

            builder.Property(p => p.ImageUrl)
                   .HasMaxLength(ImageUrlMaxLength);

            builder.Property(p => p.CreatedOn)
                   .IsRequired()
                   .HasDefaultValueSql(DefaultCreationDateSql);

            builder.Property(p => p.IsCompleted)
                   .IsRequired()
                   .HasDefaultValue(DefaultIsCompleted);
        }
    }
}