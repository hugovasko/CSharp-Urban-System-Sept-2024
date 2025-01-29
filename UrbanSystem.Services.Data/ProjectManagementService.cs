using Microsoft.EntityFrameworkCore;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Projects;

namespace UrbanSystem.Services.Data
{
    public class ProjectManagementService : IProjectManagementService
    {
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<Location, Guid> _locationRepository;

        public ProjectManagementService(
            IRepository<Project, Guid> projectRepository,
            IRepository<Location, Guid> locationRepository)
        {
            _projectRepository = projectRepository;
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<ProjectIndexViewModel>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAttached()
                .Include(p => p.Location)
                .ToListAsync();

            return projects.Select(p => new ProjectIndexViewModel
            {
                Id = p.Id,
                Name = p.Name,
                DesiredSum = p.FundsNeeded,
                ImageUrl = p.ImageUrl,
                Description = p.Description,
                CreatedOn = p.CreatedOn,
                IsCompleted = p.IsCompleted,
                LocationName = p.Location.CityName
            });
        }

        public async Task<ProjectIndexViewModel?> GetProjectByIdAsync(Guid id)
        {
            var project = await _projectRepository.GetAllAttached()
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return null;
            }

            return new ProjectIndexViewModel
            {
                Id = project.Id,
                Name = project.Name,
                DesiredSum = project.FundsNeeded,
                ImageUrl = project.ImageUrl,
                Description = project.Description,
                CreatedOn = project.CreatedOn,
                IsCompleted = project.IsCompleted,
                LocationName = project.Location.CityName
            };
        }

        public async Task<bool> UpdateProjectCompletionAsync(Guid id, bool isCompleted)
        {
            var project = await _projectRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return false;
            }

            project.IsCompleted = isCompleted;
            await _projectRepository.UpdateAsync(project);
            return true;
        }

        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            return await _projectRepository.DeleteAsync(id);
        }

        public async Task<bool> ToggleProjectCompletionAsync(Guid id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return false;
            }

            project.IsCompleted = !project.IsCompleted;
            return await _projectRepository.UpdateAsync(project);
        }
    }
}
