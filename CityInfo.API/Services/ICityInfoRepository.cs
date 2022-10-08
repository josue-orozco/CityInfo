using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    // follows the repository pattern so we dont access the DB from controller
    // this removes duplicate code for same calls to db from controllers/consumer
    // makes controller testable by separating controller action logic with persistance logic (we can mock persistance data)
    // allows us to choose peristsence technology in repository methods to use the best tool 
    // such as Entity Framework or ADO.Net but controller doesnt care and with repository pattern it wont know
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();

        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(
            string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);

        Task<bool> CityExistsAsync(int cityId);

        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId,
            int pointOfInterestId);

        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

        // not async since deleting is an in memory opertation and not an IO
        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        Task<bool> CityNameMatchesCityId(string? cityName, int cityId);

        Task<bool> SaveChangesAsync();
    }
}
