using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface IBaseService
    {
        Task<IEnumerable<CityOption>> GetCitiesAsync();
    }
}
