using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanSystem.Data.Models;

namespace UrbanSystem.Data.Configuration
{
    public class ApplicationUserSuggestionConfiguration : IEntityTypeConfiguration<ApplicationUserSuggestion>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserSuggestion> builder)
        {
            builder.HasKey(us => new { us.ApplicationUserId, us.SuggestionId });

            builder.HasOne(us => us.Suggestion)
                .WithMany(s => s.UsersSuggestions)
                .HasForeignKey(us => us.SuggestionId);

            builder.HasOne(us => us.User)
                .WithMany(u => u.UsersSuggestions)
                .HasForeignKey(us => us.ApplicationUserId);
        }
    }
}
