using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanSystem.Data.Models;

namespace UrbanSystem.Data.Configuration
{
    public class SuggestionLocationConfiguration : IEntityTypeConfiguration<SuggestionLocation>
    {
        public void Configure(EntityTypeBuilder<SuggestionLocation> builder)
        {
            builder.HasKey(sl => new { sl.SuggestionId, sl.LocationId });

            builder.HasOne(sl => sl.Suggestion)
                .WithMany(s => s.SuggestionsLocations)
                .HasForeignKey(sl => sl.SuggestionId);

            builder.HasOne(sl => sl.Location)
                .WithMany(l => l.SuggestionsLocations)
                .HasForeignKey(sl => sl.LocationId);
        }
    }
}
