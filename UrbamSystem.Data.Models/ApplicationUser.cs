using Microsoft.AspNetCore.Identity;

namespace UrbanSystem.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid();
        }

        public virtual ICollection<ApplicationUserSuggestion> UsersSuggestions { get; set; } = new HashSet<ApplicationUserSuggestion>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Meeting> Meetings { get; set; } = new HashSet<Meeting>();
        public virtual ICollection<Meeting> OrganizedMeetings { get; set; } = new HashSet<Meeting>();
        public virtual ICollection<Suggestion> Suggestions { get; set; } = new HashSet<Suggestion>();
        
        public virtual ICollection<ProjectRating> ProjectRatings { get; set; } = new HashSet<ProjectRating>();
        public virtual ICollection<CommentRating> CommentRatings { get; set; } = new HashSet<CommentRating>();
    }
}