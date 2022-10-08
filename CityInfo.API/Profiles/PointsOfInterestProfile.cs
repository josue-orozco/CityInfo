using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointsOfInterestProfile : Profile
    {
        public PointsOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
            CreateMap<Models.PointsOfInterestForCreationDto, Entities.PointOfInterest>();
            CreateMap<Models.PointsOfInterestForUpdateDto, Entities.PointOfInterest>();
            CreateMap<Entities.PointOfInterest, Models.PointsOfInterestForUpdateDto>();
        }
    }
}
