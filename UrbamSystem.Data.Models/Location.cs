namespace UrbanSystem.Data.Models
{
    public class Location
    {
        public Location()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string CityName { get; set; } = null!;

        public string StreetName { get; set; } = null!;

        public string CityPicture { get; set; } = null!;

        public virtual ICollection<SuggestionLocation> SuggestionsLocations { get; set; } = new HashSet<SuggestionLocation>();
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    }
}