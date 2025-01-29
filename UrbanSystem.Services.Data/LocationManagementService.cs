using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Services.Data
{
    public class LocationManagementService : ILocationManagementService
    {
        private readonly IRepository<Location, Guid> _locationRepository;
        private readonly IRepository<Suggestion, Guid> _suggestionRepository;
        private readonly IRepository<Meeting, Guid> _meetingRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<SuggestionLocation, object> _suggestionLocationRepository;

        public LocationManagementService(
            IRepository<Location, Guid> locationRepository,
            IRepository<Suggestion, Guid> suggestionRepository,
            IRepository<Meeting, Guid> meetingRepository,
            IRepository<Project, Guid> projectRepository,
            IRepository<SuggestionLocation, object> suggestionLocationRepository)
        {
            _locationRepository = locationRepository;
            _suggestionRepository = suggestionRepository;
            _meetingRepository = meetingRepository;
            _projectRepository = projectRepository;
            _suggestionLocationRepository = suggestionLocationRepository;
        }

        public async Task<IEnumerable<LocationDetailsViewModel>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return locations.Select(l => new LocationDetailsViewModel
            {
                Id = l.Id.ToString(),
                CityName = l.CityName,
                StreetName = l.StreetName
            });
        }

        public async Task<Location> GetLocationByIdAsync(Guid locationId)
        {
            return await _locationRepository.GetByIdAsync(locationId);
        }

        public async Task<bool> DeleteLocationAsync(Guid locationId)
        {
            return await _locationRepository.DeleteAsync(locationId);
        }

        public async Task<bool> LocationExistsByIdAsync(Guid locationId)
        {
            var location = await _locationRepository.GetByIdAsync(locationId);
            return location != null;
        }

        public async Task DeleteSuggestionsByLocationIdAsync(Guid locationId)
        {
            var suggestionLocations = await _suggestionLocationRepository.GetAllAsync(sl => sl.LocationId == locationId);

            foreach (var suggestionLocation in suggestionLocations)
            {
                await _suggestionLocationRepository.DeleteAsync(new object[] { suggestionLocation.SuggestionId, suggestionLocation.LocationId });
            }

            var orphanedSuggestions = await _suggestionRepository.GetAllAsync(s => !s.SuggestionsLocations.Any());
            foreach (var suggestion in orphanedSuggestions)
            {
                await _suggestionRepository.DeleteAsync(suggestion.Id);
            }
        }

        public async Task DeleteMeetingsByLocationIdAsync(Guid locationId)
        {
            await _meetingRepository.DeleteAsync(m => m.LocationId == locationId);
        }

        public async Task DeleteProjectsByLocationIdAsync(Guid locationId)
        {
            await _projectRepository.DeleteAsync(p => p.LocationId == locationId);
        }
    }
}