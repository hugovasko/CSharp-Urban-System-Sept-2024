using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Web.ViewModels.Suggestions
{
    public class MySuggestionsViewModel
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string UploadedOn { get; set; } = null!;
        public string? AttachmentUrl { get; set; } = null!;
        public IEnumerable<CityOption> LocationNames { get; set; } = new HashSet<CityOption>();
    }
}
