using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapApi.Entities;
using MapApi.Repositories;

namespace MapApi.Services
{
    public interface IRouteService
    {
        Task<List<Route>> GetRoutesForGame(Guid gameId);
        Task SyncRoutesForGame(Guid gameId, List<Route> route);
    }
    
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _repository;

        public RouteService(IRouteRepository repository)
        {
            _repository = repository;
        }
        
        public Task<List<Route>> GetRoutesForGame(Guid gameId)
        {
            return _repository.GetRoutesForGame(gameId);
        }

        public async Task SyncRoutesForGame(Guid gameId, List<Route> routes)
        {
            //get the routes for the game
            var dbRoutes = await GetRoutesForGame(gameId);
            
            //routes to delete, routes in db routes but not in given list
            var routesToDelete = dbRoutes.Where(dbRoute => !routes.Contains(dbRoute));
            _repository.RemoveRoutes(routesToDelete);
            
            //routes to add routes in routes but not in db routes
            var routesToAdd = routes.Where(route => !dbRoutes.Contains(route));
            _repository.AddRoutes(routesToAdd);
        }
    }
}