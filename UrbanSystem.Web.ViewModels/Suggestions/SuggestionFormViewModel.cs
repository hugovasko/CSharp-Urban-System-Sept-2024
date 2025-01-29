using System.ComponentModel.DataAnnotations;
using UrbanSystem.Web.ViewModels.Locations;
using static UrbanSystem.Common.ValidationConstants.Suggestion;
using static UrbanSystem.Common.ValidationStrings.Suggestion;
using static UrbanSystem.Common.ValidationConstants.Location;
using static UrbanSystem.Common.ValidationStrings.Location;

namespace UrbanSystem.Web.ViewModels.Suggestions
{
    public class SuggestionFormViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = TitleRequiredMessage)]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = CategoryRequiredMessage)]
        [MinLength(CategoryMinLength)]
        [MaxLength(CategoryMaxLength)]
        public string Category { get; set; } = null!;

        public string OrganizerName { get; set; } = null!;

        public string? AttachmentUrl { get; set; }

        [Required(ErrorMessage = DescriptionRequiredMessage)]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = CityNameRequiredMessage)]
        [MaxLength(CityNameMaxLength)]
        public string CityName { get; set; } = null!;

        [Required(ErrorMessage = PriorityRequiredMessage)]
        public string Priority { get; set; } = "Medium";

        [Required(ErrorMessage = StatusRequiredMessage)]
        public string Status { get; set; } = "Open";

        [Required(ErrorMessage = StreetNameRequiredMessage)]
        [MaxLength(StreetNameMaxLength)]
        public string StreetName { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Guid CityId { get; set; }

        public string UserId { get; set; } = null!;

        public IEnumerable<CityOption> Cities { get; set; } = new HashSet<CityOption>();
    }
}