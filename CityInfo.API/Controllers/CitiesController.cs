using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cities")] //apiVersion means user will pass in version parameter which will be compared to the supported API version
    public class CitiesController : ControllerBase
    {
        //private readonly CitiesDataStore citiesDataStore;
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet]
        // dont have to add the [FromQuery] attribute to name since its not a complex type or paremter in route template
        // so it defaults to looking in query string
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
            [FromQuery] string? name, [FromQuery] string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }
            
            var (cityEntities, paginationMetadata) = await cityInfoRepository
                .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));
            
            return Ok(mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="id">The id of the city to get</param>
        /// <param name="includePointsOfInterest">Whether or not to include the points of intest</param>
        /// <returns>An IActionResult</returns>
        /// <response code="200">Returns the requested city</response> //this overrides the description in swagger documentation
        [HttpGet("{id}")]
        // can have many more responses like 500 internal server error but for demo we will
        // just include those in method
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // returning a more generic IActionResult since we are no longer just returning CityDto
        // second parameter is passed through the query string so you append a question
        // mark with the name of the parameter and the value: ?includePointsOfInterest=true
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                return Ok(mapper.Map<CityDto>(city));
            }
            
            return Ok(mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
