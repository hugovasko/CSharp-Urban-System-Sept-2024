using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanSystem.Data.Models;

namespace UrbanSystem.Data.Configuration;

// UrbanSystem.Data/Configuration/ProjectRatingConfiguration.cs
public class ProjectRatingConfiguration : IEntityTypeConfiguration<ProjectRating>
{
    public void Configure(EntityTypeBuilder<ProjectRating> builder)
    {
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Score)
            .IsRequired()
            .HasAnnotation("Range", new[] { 1, 5 });

        builder.Property(pr => pr.RatedOn)
            .IsRequired();

        builder.Property(pr => pr.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(pr => pr.Project)
            .WithMany(p => p.Ratings)
            .HasForeignKey(pr => pr.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pr => pr.User)
            .WithMany(u => u.ProjectRatings)
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}