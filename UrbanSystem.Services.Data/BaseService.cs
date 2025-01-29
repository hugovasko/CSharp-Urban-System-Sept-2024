using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Services.Data
{
    public class BaseService : IBaseService
    {
        public virtual IRepository<Location, Guid> _locationRepository { get; set; }

        public BaseService(IRepository<Location, Guid> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<CityOption>> GetCitiesAsync()
        {
            var cities = await _locationRepository.GetAllAsync();

            return cities
                .Where(l => !string.IsNullOrEmpty(l.CityName))
                .GroupBy(l => l.CityName)
                .Select(g => new CityOption
                {
                    Value = g.First().Id.ToString(),
                    Text = g.Key
                })
                .ToList();
        }
    }
}
