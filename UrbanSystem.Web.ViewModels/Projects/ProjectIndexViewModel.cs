namespace UrbanSystem.Web.ViewModels.Projects
{
    public class ProjectIndexViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal DesiredSum { get; set; }
        public string? ImageUrl { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string? LocationName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
}
