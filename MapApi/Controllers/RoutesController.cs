using System;
using System.Linq;
using System.Threading.Tasks;
using MapApi.Services;
using MapApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MapApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }
        
        [HttpGet("{gameId:guid}"), Authorize]
        public async Task<IActionResult> GetByGameId(Guid gameId) {
            //var userId = (string)HttpContext.Items["UserId"];
            //make sure the userid is the owner of the game
            //we may consider checking if the game id is valid
            var routes = await _routeService.GetRoutesForGame(gameId);
            return Ok(routes.Select(r => new RouteViewModel(r)).ToList());
        }
    }
}