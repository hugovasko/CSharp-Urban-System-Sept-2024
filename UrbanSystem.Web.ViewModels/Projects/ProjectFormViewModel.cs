using System.ComponentModel.DataAnnotations;
using UrbanSystem.Web.ViewModels.Locations;
using static UrbanSystem.Common.ValidationStrings.Project;

namespace UrbanSystem.Web.ViewModels.Projects
{
    public class ProjectFormViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = NameLengthErrorMessage)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0.01, 10000000, ErrorMessage = DesiredSumRangeErrorMessage)]
        public decimal DesiredSum { get; set; }

        [StringLength(2048, ErrorMessage = ImageUrlLengthErrorMessage)]
        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = DescriptionLengthErrorMessage)]
        public string Description { get; set; } = null!;

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        [Required]
        public DateTime FundingDeadline { get; set; }

        public bool IsCompleted { get; set; }

        [Required]
        public Guid LocationId { get; set; }

        [Required]
        public IEnumerable<CityOption> Cities { get; set; } = new HashSet<CityOption>();
    }
}
