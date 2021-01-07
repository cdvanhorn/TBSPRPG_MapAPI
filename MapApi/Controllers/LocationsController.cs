using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using System.Linq;

using MapApi.Services;
using MapApi.ViewModels;

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
            return Ok(await _locationService.GetAllLocations());
        }

        [HttpGet("{gameid}")]
        [Authorize]
        public async Task<IActionResult> GetByGameId(string gameid) {
            var userId = (string)HttpContext.Items["UserId"];

            //make sure the userid is the owner of the game

            //we may consider checking if the game id is valid

            var location = await _locationService.GetLocationForGame(gameid);
            return Ok(location);
        }

    }
}