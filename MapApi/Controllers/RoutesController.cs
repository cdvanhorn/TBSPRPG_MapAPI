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
        private readonly IGameService _gameService;

        public RoutesController(IRouteService routeService, IGameService gameService)
        {
            _routeService = routeService;
            _gameService = gameService;
        }
        
        [HttpGet("{gameId:guid}"), Authorize]
        public async Task<IActionResult> GetByGameId(Guid gameId) {
            //var userId = (string)HttpContext.Items["UserId"];
            //make sure the userid is the owner of the game
            var game = await _gameService.GetGame(gameId);
            if (game == null)
                return Ok();
            var routes = await _routeService.GetRoutesForGame(gameId);
            return Ok(routes.Select(r => new RouteViewModel(r)).ToList());
        }
    }
}