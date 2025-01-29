using System.ComponentModel.DataAnnotations;
using UrbanSystem.Web.ViewModels.Locations;
using static UrbanSystem.Common.ValidationStrings.Formatting;

namespace UrbanSystem.Web.ViewModels.Meetings
{
    public class MeetingIndexViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        [DisplayFormat(DataFormatString = DateDisplayFormat, ApplyFormatInEditMode = true)]
        public DateTime ScheduledDate { get; set; }

        public double Duration { get; set; }

        public Guid LocationId { get; set; }

        public string? CityName { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int AttendeesCount { get; set; }

        public IEnumerable<string> Attendees { get; set; } = new HashSet<string>();

        public string OrganizerName { get; set; } = null!;

        public Guid OrganizerId { get; set; }

        public bool IsCurrentUserOrganizer { get; set; }
        public bool IsCurrentUserAttending { get; set; }
    }
}
