using System.Threading.Tasks;
using System.Linq;

using LunchRoulette.Entities;
using LunchRoulette.Exceptions;
using LunchRoulette.Exceptions.CuisineExceptions;
using LunchRoulette.Exceptions.LunchSpotExceptions;
using LunchRoulette.Services;
using LunchRoulette.Utils.StringHelpers;
using LunchRoulette.Web.Models;
using LunchRoulette.Web.Utils.Logger;
using LunchRoulette.Web.Utils.ModelState;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LunchRoulette.Web.Controllers
{
    [Route("api/[controller]")]
    public class LunchSpotController : Controller
    {
        private ILogger<LunchSpotController> _logger { get; }
        private ILunchSpotServices _lunchSpotServices { get; }

        public LunchSpotController(ILogger<LunchSpotController> logger, ILunchSpotServices lunchSpotServices)
        {
            _logger = logger;
            _lunchSpotServices = lunchSpotServices;
        }

        [HttpPost("create/{cuisineName}/{lunchSpotName}")]
        public async Task<IActionResult> CreateLunchSpot([Bind]CreateLunchSpotModel model)
        {
            _logger.LogTrace($"Recieved request to create lunch spot: {model}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid lunch spot creation request");
                var errorResponse = ModelState.GenerateErrorModel();
                _logger.LogErrorModel(errorResponse);
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
            _logger.LogTrace($"Lunch spot creation model is valid: {model}");
            try
            {
                var createdLunchSpot = await _lunchSpotServices.CreateLunchSpotAsync(model.LunchSpotName, new Cuisine(model.CuisineName));
                _logger.LogInformation($"Created lunch spot {createdLunchSpot}");
                _logger.LogOk(createdLunchSpot);
                return Ok(createdLunchSpot);
            }
            catch (CuisineNotFoundException)
            {
                var errorResponse = new ErrorModel { Message = $"Cuisine {model.CuisineName} does not exist" };
                _logger.LogWarning(errorResponse.Message);
                _logger.LogNotFound(errorResponse);
                return NotFound(errorResponse);
            }
            catch (LunchSpotException)
            {
                var errorResponse = new ErrorModel { Message = $"Creation of lunch spot {model.LunchSpotName} failed" };
                _logger.LogWarning(errorResponse.Message);
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListLunchSpots()
        {
            _logger.LogTrace($"Recieved a request to list lunch spots");
            var lunchSpots = await _lunchSpotServices.ListLunchSpots().ToList();
            _logger.LogInformation($"Found {lunchSpots.Count} lunch spots");
            _logger.LogOk(Newtonsoft.Json.JsonConvert.SerializeObject(lunchSpots));
            return Ok(lunchSpots);
        }

        [HttpGet("{lunchSpotId}")]
        public async Task<IActionResult> GetLunchSpotById([Bind]GetLunchSpotModel model)
        {
            _logger.LogTrace($"Recieved request to get lunch spot by id: {model}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"GetLunchSpotModel is invalid: {model}");
                var errorResponse = ModelState.GenerateErrorModel();
                _logger.LogErrorModel(errorResponse);
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
            try
            {
                _logger.LogTrace($"Model state is valid: {model}");
                var lunchSpot = await _lunchSpotServices.GetLunchSpotByIdAsync(model.LunchSpotId);
                _logger.LogInformation($"Got lunch spot {lunchSpot}");
                _logger.LogOk(lunchSpot);
                return Ok(lunchSpot);
            }
            catch (LunchSpotNotFoundException)
            {
                var errorResponse = new ErrorModel { Message = $"LunchSpotId {model.LunchSpotId} is not valid" };
                _logger.LogWarning(errorResponse.Message);
                _logger.LogNotFound(errorResponse);
                return NotFound(errorResponse);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchLunchSpots([Bind]SearchLunchSpotModel model)
        {
            _logger.LogTrace($"Recieved request to search with model: {model}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Search model state is invalid {model}");
                var errorResponse = ModelState.GenerateErrorModel();
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
            _logger.LogTrace($"Search lunch spot model is valid, searching");
            var lunchSpots = await _lunchSpotServices.ListLunchSpots(x =>
                x.Name.ContainsIgnoreCase(model.LunchSpotName) &&
                x.Cuisine.Name.ContainsIgnoreCase(model.CuisineName))
                .ToList();
            _logger.LogInformation($"Found {lunchSpots.Count} lunch spots with search query {model}");
            _logger.LogOk(lunchSpots);
            return Ok(lunchSpots);
        }

        [HttpPut("update/{lunchSpotId}/{cuisineName}/{lunchSpotName}")]
        public async Task<IActionResult> UpdateLunchSpot([Bind]UpdateLunchSpotModel model)
        {
            _logger.LogTrace($"Recieved request to update lunch spot: {model}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"The lunch spot update model is not valid: {model}");
                var errorResponse = ModelState.GenerateErrorModel();
                _logger.LogErrorModel(errorResponse);
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
            _logger.LogTrace($"Update lunch spot model is valid. {model}");
            try
            {
                var updatedLunchSpot = await _lunchSpotServices.UpdateLunchSpotAsync(model.LunchSpotId, new LunchSpot(model.LunchSpotName, new Cuisine(model.CuisineName)));
                _logger.LogInformation($"Updated lunch spot: {updatedLunchSpot}");
                _logger.LogOk(updatedLunchSpot);
                return Ok(updatedLunchSpot);
            }
            catch (CuisineNotFoundException)
            {
                var errorResponse = new ErrorModel { Message = $"Cuisine named {model.CuisineName} not found" };
                _logger.LogWarning(errorResponse.Message);
                _logger.LogNotFound(errorResponse);
                return NotFound(errorResponse);
            }
            catch (LunchSpotNotFoundException)
            {
                var errorResponse = new ErrorModel { Message = $"Lunch spot with id {model.LunchSpotId} not found" };
                _logger.LogWarning(errorResponse.Message);
                _logger.LogNotFound(errorResponse);
                return NotFound(errorResponse);
            }
        }
    }
}