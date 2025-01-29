using UrbanSystem.Web.ViewModels.SuggestionsLocations;

namespace UrbanSystem.Web.ViewModels.Locations
{
    public class LocationDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string CityName { get; set; } = null!;

        public string StreetName { get; set; } = null!;

        public string CityPicture { get; set; } = null!;

        public IEnumerable<SuggestionLocationViewModel> Suggestions { get; set; } = new HashSet<SuggestionLocationViewModel>();
    }
}
