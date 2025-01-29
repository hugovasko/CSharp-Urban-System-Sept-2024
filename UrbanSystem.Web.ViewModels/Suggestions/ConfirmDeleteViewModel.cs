namespace UrbanSystem.Web.ViewModels.Suggestions
{
    public class ConfirmDeleteViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}