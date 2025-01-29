namespace UrbanSystem.Data.Models
{
    public class SuggestionLocation
    {
        public Guid SuggestionId { get; set; }
        public Suggestion Suggestion { get; set; } = null!;

        public Guid LocationId { get; set; }
        public Location Location { get; set; } = null!;
    }
}