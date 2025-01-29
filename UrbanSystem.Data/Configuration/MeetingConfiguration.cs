using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static UrbanSystem.Common.ValidationConstants.Meeting;
using UrbanSystem.Data.Models;

namespace UrbanSystem.Data.Configuration
{
    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(TitleMaxLength);

            builder.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            builder.Property(m => m.ScheduledDate)
                .IsRequired();

            builder.Property(m => m.Duration)
                .IsRequired()
                .HasMaxLength(DurationMaxValue);

            builder.HasOne(m => m.Location)
                .WithMany()
                .HasForeignKey(m => m.LocationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Attendees)
                .WithMany(u => u.Meetings);

            builder.HasOne(m => m.Organizer)
                .WithMany(u => u.OrganizedMeetings)
                .HasForeignKey(m => m.OrganizerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
