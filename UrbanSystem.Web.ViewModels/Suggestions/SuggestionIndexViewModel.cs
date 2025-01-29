namespace UrbanSystem.Web.ViewModels.Suggestions
{
    public class SuggestionIndexViewModel
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Category { get; set; } = null!;

        public string OrganizerName { get; set; } = null!;

        public string? AttachmentUrl { get; set; }

        public string Description { get; set; } = null!;

        public string UploadedOn { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Priority { get; set; } = "Low";
        public int VoteCount { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public IEnumerable<string> LocationNames { get; set; } = new HashSet<string>();
        public ICollection<CommentViewModel> Comments { get; set; } = new HashSet<CommentViewModel>();
    }
}
