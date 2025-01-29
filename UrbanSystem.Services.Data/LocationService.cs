using Microsoft.EntityFrameworkCore;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.Helpers;
using UrbanSystem.Web.ViewModels.Locations;
using UrbanSystem.Web.ViewModels.SuggestionsLocations;

namespace UrbanSystem.Services.Data
{
    public class LocationService : ILocationService
    {
        private readonly IRepository<Location, Guid> _locationRepository;

        public LocationService(IRepository<Location, Guid> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task AddLocationAsync(LocationFormViewModel model)
        {
            var location = new Location
            {
                CityName = model.CityName,
                StreetName = model.StreetName,
                CityPicture = model.CityPicture
            };

            await _locationRepository.AddAsync(location);
        }

        public async Task<PaginatedList<LocationDetailsViewModel>> GetAllOrderedByNameAsync(int pageIndex, int pageSize)
        {
            var query = _locationRepository
                .GetAllAttached()
                .OrderBy(l => l.CityName)
                .Select(l => new LocationDetailsViewModel
                {
                    Id = l.Id.ToString(),
                    CityName = l.CityName,
                    StreetName = l.StreetName,
                    CityPicture = l.CityPicture,
                    Suggestions = l.SuggestionsLocations.Select(sl => new SuggestionLocationViewModel
                    {
                        Id = sl.Suggestion.Id.ToString(),
                        Title = sl.Suggestion.Title
                    }).ToList()
                });

            return PaginatedList<LocationDetailsViewModel>.Create(query, pageIndex, pageSize);
        }

        public async Task<LocationDetailsViewModel> GetLocationDetailsByIdAsync(string? id)
        {
            if (!IsGuidIdValid(id?.ToLower(), out Guid locationGuid))
            {
                return null!;
            }

            var location = await _locationRepository
                .GetAllAttached()
                .Include(l => l.SuggestionsLocations)
                .ThenInclude(sl => sl.Suggestion)
                .FirstOrDefaultAsync(l => l.Id == locationGuid);

            if (location == null)
            {
                return null!;
            }

            var viewModel = new LocationDetailsViewModel
            {
                Id = location.Id.ToString(),
                CityName = location.CityName,
                StreetName = location.StreetName,
                CityPicture = location.CityPicture,
                Suggestions = location.SuggestionsLocations.Select(sl => new SuggestionLocationViewModel
                {
                    Id = sl.Suggestion.Id.ToString(),
                    Title = sl.Suggestion.Title
                }).ToList()
            };

            return viewModel;
        }

        private bool IsGuidIdValid(string? id, out Guid guid)
        {
            return Guid.TryParse(id, out guid);
        }
    }
}