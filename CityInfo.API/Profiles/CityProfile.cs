using AutoMapper;

namespace CityInfo.API.Profiles
{
    // Automapper is convention based so it will map property names from the source object
    // to destination object. By default it will ignore null reference exceptions (i.e. if property
    // doesnt exist it wil ignore it)
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
            CreateMap<Entities.City, Models.CityDto>();
        }
    }
}
