namespace UrbanSystem.Data.Models
{
    public class Meeting
    {
        public Meeting()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime ScheduledDate { get; set; }
        public double Duration { get; set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; } = null!;
        public Guid OrganizerId { get; set; }
        public ApplicationUser Organizer { get; set; } = null!;

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public virtual ICollection<ApplicationUser> Attendees { get; set; } = new HashSet<ApplicationUser>();
    }
}