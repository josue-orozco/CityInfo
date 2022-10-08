using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    [Authorize(Policy = "MustBeFromAntwerp")] //this defines both that it needs to be authenticated with a token to get access but also follow the authorization policy
    [ApiVersion("2.0")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> logger;
        private readonly IMailService mailService;
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            // just to demo the use of claims that are provided in token
            // allows controller to access user data such as name, username, pwd

            //var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
            
            //if (await cityInfoRepository.CityNameMatchesCityId(cityName, cityId) == false)
            //{
            //    return Forbid();
            //}

            if (await cityInfoRepository.CityExistsAsync(cityId) == false)
            {
                logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }
            
            var pointOfInterests = await cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterests));
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (await cityInfoRepository.CityExistsAsync(cityId) == false)
            {
                logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointOfInterest = await cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        // adding [FromBody] is not necessary because its a complex type and its inferred that it will be provided via the message body
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, [FromBody] PointsOfInterestForCreationDto pointOfInterest)
        {
            if (await cityInfoRepository.CityExistsAsync(cityId) == false)
            {
                return NotFound();
            }

            var finalPointOfInterest = mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            // if something goes wrong here the framework will auto return a 500 message
            // since it was server side error
            await cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);
        }

        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointsOfInterestForUpdateDto pointOfInterest)
        {
            if (await cityInfoRepository.CityExistsAsync(cityId) == false)
            {
                logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointOfInterestEntity = await cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // this mapping uses the source to override values in the destination which is the DB context
            mapper.Map(pointOfInterest, pointOfInterestEntity);
            
            await cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointsOfInterestForUpdateDto> patchDocument)
        {
            if (await cityInfoRepository.CityExistsAsync(cityId) == false)
            {
                logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointOfInterestEntity = await cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = mapper.Map<PointsOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            // required to manually check because the automated annotation checks included
            // with the api controller attribute during the serialization of the input
            // object is the patch file have no effects
            // this check will only catch errors based on the input model which would be
            // during the patching
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            // required since the ModelState checks the input model which is the patch file
            // but we are interested in pointOfInterestForUpdateDto so we have to manually
            // do a model check after we update with the patch file.
            if (TryValidateModel(pointOfInterestToPatch) == false)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            await cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (await cityInfoRepository.CityExistsAsync(cityId) == false)
            {
                logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointOfInterestEntity = await cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            await cityInfoRepository.SaveChangesAsync();

            this.mailService.Send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");
            
            return NoContent();
        }
    }
}
