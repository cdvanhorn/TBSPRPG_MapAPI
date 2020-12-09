using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

using MapApi.Services;

namespace MapApi.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    public class LocationsController : ControllerBase{
        ILocationService _locationService;

        public LocationsController(ILocationService locationService) {
            _locationService = locationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var locations = await _locationService.GetAllLocations();
            return Ok(locations);
        }

        [HttpGet("{gameid}")]
        [Authorize]
        public async Task<IActionResult> GetByGameId(string gameid) {
            var userId = (string)HttpContext.Items["UserId"];

            //make sure the userid is the owner of the game

            var location = await _locationService.GetLocationForGame(gameid);
            //var game = await _gameService.GetByUserIdAndAdventureName(userId, name);
            return Ok(location);
        }

    }
}