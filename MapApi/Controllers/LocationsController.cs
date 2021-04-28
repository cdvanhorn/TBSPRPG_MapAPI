using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MapApi.Services;

namespace MapApi.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    public class LocationsController : ControllerBase{
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService) {
            _locationService = locationService;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _locationService.GetAllLocations());
        }

        [HttpGet("{gameId:guid}"), Authorize]
        public async Task<IActionResult> GetByGameId(Guid gameId) {
            //var userId = (string)HttpContext.Items["UserId"];

            //make sure the userid is the owner of the game

            //we may consider checking if the game id is valid

            var location = await _locationService.GetLocationForGame(gameId);
            return Ok(location);
        }

    }
}