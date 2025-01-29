namespace UrbanSystem.Data.Models
{
    public class ApplicationUserSuggestion
    {
        public Guid ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public Guid SuggestionId { get; set; }
        public virtual Suggestion Suggestion { get; set; } = null!;
    }
}