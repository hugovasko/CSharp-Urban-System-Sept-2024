using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanSystem.Data.Models;

namespace UrbanSystem.Data.Configuration;

public class CommentRatingConfiguration : IEntityTypeConfiguration<CommentRating>
{
    public void Configure(EntityTypeBuilder<CommentRating> builder)
    {
        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.IsLike)
            .IsRequired();

        builder.Property(cr => cr.RatedOn)
            .IsRequired();

        builder.Property(cr => cr.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(cr => cr.Comment)
            .WithMany(c => c.Ratings)
            .HasForeignKey(cr => cr.CommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.User)
            .WithMany(u => u.CommentRatings)
            .HasForeignKey(cr => cr.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}