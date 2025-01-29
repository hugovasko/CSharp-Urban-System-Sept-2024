namespace UrbanSystem.Data.Models
{
    public class Suggestion
    {
        public Suggestion()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? AttachmentUrl { get; set; }
        public string Description { get; set; } = null!;
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow.Date;
        public string Status { get; set; } = "Open";
        public string Priority { get; set; } = "Low";
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public virtual ICollection<SuggestionLocation> SuggestionsLocations { get; set; } = new HashSet<SuggestionLocation>();
        public virtual ICollection<ApplicationUserSuggestion> UsersSuggestions { get; set; } = new HashSet<ApplicationUserSuggestion>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
