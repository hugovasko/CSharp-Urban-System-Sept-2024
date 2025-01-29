using UrbanSystem.Web.ViewModels.Projects;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface IProjectManagementService
    {
        Task<IEnumerable<ProjectIndexViewModel>> GetAllProjectsAsync();
        Task<ProjectIndexViewModel?> GetProjectByIdAsync(Guid id);
        Task<bool> DeleteProjectAsync(Guid id);
        Task<bool> UpdateProjectCompletionAsync(Guid id, bool isCompleted);
        Task<bool> ToggleProjectCompletionAsync(Guid id);
    }
}