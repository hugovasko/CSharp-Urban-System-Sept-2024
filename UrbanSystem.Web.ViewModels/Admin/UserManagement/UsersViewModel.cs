namespace UrbanSystem.Web.ViewModels.Admin.UserManagement
{
    public class UsersViewModel
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public IEnumerable<string> Roles { get; set; } = new HashSet<string>();
    }
}
