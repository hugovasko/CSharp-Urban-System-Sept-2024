namespace UrbanSystem.Data.Models
{
    public class Project
    {
        public Project()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal FundsNeeded { get; set; }
        public string? ImageUrl { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime FundingDeadline { get; set; }
        public bool IsCompleted { get; set; } = false;

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Guid LocationId { get; set; }
        public Location Location { get; set; } = null!;
    }
}