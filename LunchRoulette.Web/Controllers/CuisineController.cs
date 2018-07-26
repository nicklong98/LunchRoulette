using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using LunchRoulette.Exceptions.CuisineExceptions;
using LunchRoulette.Services;
using LunchRoulette.Entities;
using LunchRoulette.Web.Models;
using LunchRoulette.Web.Utils.ModelState;
using LunchRoulette.Web.Utils.Logger;
using LunchRoulette.Utils.StringHelpers;

namespace LunchRoulette.Web.Controllers
{
    [Route("api/[controller]")]
    public class CuisineController : Controller
    {
        private ICuisineServices _cuisineServices { get; }
        private ILogger<CuisineController> _logger { get; }

        public CuisineController(ICuisineServices cuisineServices, ILogger<CuisineController> logger)
        {
            _cuisineServices = cuisineServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListCuisines()
        {
            _logger.LogTrace($"Recieved request to list cuisines");
            _logger.LogInformation($"Listing cuisines");
            var cuisines = await _cuisineServices.ListCuisines().ToList();
            _logger.LogInformation($"Got {cuisines.Count} cuisines");
            _logger.LogOk(Newtonsoft.Json.JsonConvert.SerializeObject(cuisines));
            return Ok(cuisines);
        }

        [HttpGet("{cuisineId}")]
        public async Task<IActionResult> GetCuisineById(GetCuisineModel model)
        {
            _logger.LogTrace($"Recieved request to get cuisine with id {model.CuisineId}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"GetCuisineModel is invalid");
                var errorResponse = ModelState.GenerateErrorModel();
                _logger.LogErrorModel(errorResponse);
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
            try
            {
                _logger.LogInformation($"Attempting to retrieve cuisine with id {model.CuisineId}");
                var fetchedCuisine = await _cuisineServices.GetCuisineByIdAsync(model.CuisineId);
                _logger.LogInformation($"Retrieved cuisine with id {fetchedCuisine.Id}: {fetchedCuisine}");
                _logger.LogOk(fetchedCuisine);
                return Ok(fetchedCuisine);
            }
            catch (CuisineNotFoundException)
            {
                _logger.LogWarning($"Cusine with id {model.CuisineId} doesn't exist");
                var errorResponse = new ErrorModel { Message = $"The cuisine with id {model.CuisineId} doesn't exist" };
                _logger.LogNotFound(errorResponse);
                return NotFound(errorResponse);
            }
        }

        [HttpGet("search/{cuisineName}")]
        public async Task<IActionResult> SearchCuisines([Bind]SearchCuisinesModel model)
        {
            _logger.LogTrace($"Recieved search request: {model.CuisineName}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Recieved invalid search cuisine request: {model.CuisineName}");
                var errorResponse = ModelState.GenerateErrorModel();
                _logger.LogErrorModel(errorResponse);
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
            _logger.LogTrace($"Searching cuisines by name {model.CuisineName}");
            var cuisines = await _cuisineServices.ListCuisines(x => x.Name.ContainsIgnoreCase(model.CuisineName)).ToList();
            _logger.LogInformation($"Got {cuisines.Count} cuisines with search {model.CuisineName}");
            _logger.LogOk(Newtonsoft.Json.JsonConvert.SerializeObject(cuisines));
            return Ok(cuisines);
        }

        [HttpPost("create/{cuisineName}")]
        public async Task<IActionResult> CreateCuisine([Bind]CreateCuisineModel model)
        {
            _logger.LogTrace($"Recieved request to create cuisine named {model.CuisineName}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid creation request for cuisine named {model.CuisineName}");
                var errorResponse = ModelState.GenerateErrorModel();
                _logger.LogErrorModel(errorResponse);
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
            _logger.LogTrace($"Model valid, creating cuisine named {model.CuisineName}");
            var cuisine = await _cuisineServices.CreateCuisineAsync(model.CuisineName);
            _logger.LogInformation($"Creation result for cuisine named {model.CuisineName}: {cuisine}");
            _logger.LogOk(cuisine);
            return Ok(cuisine);
        }

        [HttpPut("update/{cuisineId}/{cuisineName}")]
        public async Task<IActionResult> UpdateCuisine([Bind]UpdateCuisineModel model)
        {
            _logger.LogTrace($"Recieved request to update cuisine. Model: {Newtonsoft.Json.JsonConvert.SerializeObject(model)}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Recieved invalid update cuisine model: {Newtonsoft.Json.JsonConvert.SerializeObject(model)}");
                var errorResponse = ModelState.GenerateErrorModel();
                _logger.LogErrorModel(errorResponse);
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
            try
            {
                _logger.LogTrace($"Updating cuisine with Id {model.CuisineId}. Setting name to {model.CuisineName}");
                var updatedCuisine = await _cuisineServices.UpdateCuisineAsync(model.CuisineId, new Cuisine(model.CuisineName));
                _logger.LogInformation($"Successfully updated cuisine: {updatedCuisine}");
                _logger.LogOk(updatedCuisine);
                return Ok(updatedCuisine);
            }
            catch (CuisineNotFoundException)
            {
                var errorResponse = new ErrorModel { Message = $"The cuisine with id {model.CuisineId} doesn't exist" };
                _logger.LogNotFound(errorResponse);
                return NotFound(errorResponse);
            }
            catch (CuisineException)
            {
                var errorResponse = new ErrorModel { Message = $"There was an error updating cuisine with id {model.CuisineId}." };
                _logger.LogBadRequest(errorResponse);
                return BadRequest(errorResponse);
            }
        }
    }
}