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
            var locations = await _locationService.GetAllLocations();
            return Ok(locations.Select(loc => new LocationViewModel(loc)).ToList());
        }

        [HttpGet("{gameid}")]
        [Authorize]
        public async Task<IActionResult> GetByGameId(int gameid) {
            var userId = (string)HttpContext.Items["UserId"];

            //make sure the userid is the owner of the game

            //we may consider checking if the game id is valid

            var location = await _locationService.GetLocationForGame(gameid);
            if(location == null)
                return new JsonResult(new object());
            return Ok(new LocationViewModel(location));
        }

    }
}