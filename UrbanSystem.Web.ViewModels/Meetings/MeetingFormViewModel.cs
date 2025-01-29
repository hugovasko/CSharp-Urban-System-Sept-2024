using System.ComponentModel.DataAnnotations;
using UrbanSystem.Web.ViewModels.Locations;
using static UrbanSystem.Common.ValidationStrings.Meeting;

namespace UrbanSystem.Web.ViewModels.Meetings
{
    public class MeetingFormViewModel
    {
        [Required(ErrorMessage = TitleRequired)]
        [StringLength(200, ErrorMessage = TitleMaxLength)]
        public string Title { get; set; } = null!;
            
        [Required(ErrorMessage = DescriptionRequired)]
        [StringLength(1000, ErrorMessage = DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = ScheduledDateRequired)]
        [Display(Name = ScheduledDateDisplayName)]
        public DateTime ScheduledDate { get; set; }

        [Required(ErrorMessage = DurationRequired)]
        [Display(Name = DurationDisplayName)]
        [Range(0.5, 8, ErrorMessage = DurationRange)]
        public double Duration { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required(ErrorMessage = LocationRequired)]
        public Guid? LocationId { get; set; }

        public IEnumerable<CityOption> Cities { get; set; } = new List<CityOption>();
    }
}
