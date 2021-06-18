using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MapApi.Adapters;
using MapApi.Entities;
using MapApi.Entities.AdventureService;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.InterServiceCommunication.RequestModels;
using Route = MapApi.Entities.AdventureService.Route;

namespace MapApi.Services
{
    public interface IAdventureService
    {
        Task<Guid> GetInitialLocationId(Guid adventureId, Guid userId);
        Task<List<Guid>> GetRouteIdsForLocation(Game game, Guid locationId);
        Task<List<Entities.Route>> GetRoutesForLocation(Game game, Guid locationId);
    }
    
    public class AdventureService : IAdventureService
    {
        private readonly IAdventureServiceLink _adventureServiceLink;
        private readonly IContentServiceLink _contentServiceLink;

        public AdventureService(IAdventureServiceLink adventureServiceLink, IContentServiceLink contentServiceLink)
        {
            _adventureServiceLink = adventureServiceLink;
            _contentServiceLink = contentServiceLink;
        }

        public async Task<Guid> GetInitialLocationId(Guid adventureId, Guid userId)
        {
            var initialLocationTask = _adventureServiceLink.GetInitialLocation(
                new AdventureRequest() { Id = adventureId },
                new Credentials() { UserId = userId.ToString() });
            
            var initialLocationResponse = await initialLocationTask;
            //game to get the location id from the response
            var initialLocation = JsonSerializer.Deserialize<InitialLocation>(
                initialLocationResponse.Response.Content,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            return initialLocation.Id;
        }

        private async Task<List<Route>> GetServiceRoutes(Game game, Guid locationId)
        {
            var routeTask = _adventureServiceLink.GetRoutesForLocation(
                new AdventureRequest()
                {
                    Id = game.AdventureId,
                    LocationId = locationId
                },
                new Credentials()
                {
                    UserId = game.UserId.ToString()
                }
            );
            var routeResponse = await routeTask;
            var routes = JsonSerializer.Deserialize<List<Route>>(
                routeResponse.Response.Content,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }
            );
            
            //get the content for each route
            foreach (var route in routes)
            {
                var response = await _contentServiceLink.GetSourceContent(
                    new ContentRequest()
                    {
                        GameId = game.Id,
                        SourceKey = route.SourceId
                    },
                    new Credentials()
                    {
                        UserId = game.UserId.ToString()
                    }
                );
                var content = JsonSerializer.Deserialize<Content>(
                    response.Response.Content,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );
                route.Content = content.Source;
            }
            
            return routes;
        }

        public async Task<List<Guid>> GetRouteIdsForLocation(Game game, Guid locationId)
        {
            var routes = await GetServiceRoutes(game, locationId);
            return routes.Select(r => r.Id).ToList();
        }

        public async Task<List<Entities.Route>> GetRoutesForLocation(Game game, Guid locationId)
        {
            var routes = await GetServiceRoutes(game, locationId);
            return routes.Select(AdventureServiceAdapter.ToRouteEntity).ToList();
        }
    }
}