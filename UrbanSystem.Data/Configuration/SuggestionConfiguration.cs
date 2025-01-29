using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanSystem.Data.Models;
using static System.Net.WebRequestMethods;
using static UrbanSystem.Common.ValidationConstants.Suggestion;

namespace UrbanSystem.Data.Configuration
{
    public class SuggestionConfiguration : IEntityTypeConfiguration<Suggestion>
    {
        public void Configure(EntityTypeBuilder<Suggestion> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(TitleMaxLength);

            builder.Property(s => s.Category)
                .IsRequired()
                .HasMaxLength(CategoryMaxLength);

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            builder.Property(s => s.Status)
                .IsRequired();

            builder.Property(s => s.Priority)
                .IsRequired();

            builder.HasData(SeedSuggestions());
        }

        private List<Suggestion> SeedSuggestions()
        {
            return new List<Suggestion>
            {
                new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Improve Public Transport",
                    Category = "Transport",
                    AttachmentUrl = "https://sofiacheap.com/p/a/v/avtobus-sofia-28-1140x0.jpg.pagespeed.ce._682L6k5Ui.jpg",
                    Description = "Implement more frequent bus routes during peak hours to reduce congestion.",
                    UploadedOn = DateTime.UtcNow,
                    Status = "Pending",
                    Priority = "High"
                },
                new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Park Renovation",
                    Category = "Environment",
                    AttachmentUrl = "https://images.adsttc.com/media/images/65ef/ba05/4ad7/6901/7c36/0da6/slideshow/renovation-of-peace-parks-gate-6-atelier-z-plus_14.jpg?1710209562",
                    Description = "Renovate the central park by adding new benches, lighting, and a playground area.",
                    UploadedOn = DateTime.UtcNow,
                    Status = "Approved",
                    Priority = "Medium"
                },
                new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Recycling Initiative",
                    Category = "Waste Management",
                    AttachmentUrl = "https://cleanlites.com/wp-content/uploads/2020/01/011420_Cleanlites-Blog-Image_Encourage-Recycling-Community.jpg",
                    Description = "Introduce a recycling program and provide more public recycling bins.",
                    UploadedOn = DateTime.UtcNow,
                    Status = "In Review",
                    Priority = "High"
                },
                new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Street Lighting Upgrade",
                    Category = "Infrastructure",
                    AttachmentUrl = "https://www.silabs.com/content/dam/siliconlabs/images/applications/smart-cities/street-lighting-poster.png",
                    Description = "Upgrade street lighting in residential areas to improve safety during nighttime.",
                    UploadedOn = DateTime.UtcNow,
                    Status = "Pending",
                    Priority = "Medium"
                },
                new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Community Wi-Fi Access",
                    Category = "Technology",
                    AttachmentUrl = "https://www-res.cablelabs.com/wp-content/uploads/2016/10/28093617/Community_Wi-Fi_A_Primer_vivek_ganti-1024x576.jpg",
                    Description = "Install free Wi-Fi hotspots in key public areas for better community connectivity.",
                    UploadedOn = DateTime.UtcNow,
                    Status = "Approved",
                    Priority = "High"
                }
            };
        }
    }
}
