using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MapApi.Entities.AdventureService;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.InterServiceCommunication.RequestModels;

namespace MapApi.Services
{
    public interface IAdventureService
    {
        Task<Guid> GetInitialLocationId(Guid adventureId, Guid userId);
        Task<List<Guid>> GetRouteIdsForLocation(Guid adventureId, Guid locationId, Guid userId);
    }
    
    public class AdventureService : IAdventureService
    {
        private readonly IAdventureServiceLink _adventureServiceLink;

        public AdventureService(IAdventureServiceLink adventureServiceLink)
        {
            _adventureServiceLink = adventureServiceLink;
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

        public async Task<List<Guid>> GetRouteIdsForLocation(Guid adventureId, Guid locationId, Guid userId)
        {
            var routeTask = _adventureServiceLink.GetRoutesForLocation(
                new AdventureRequest()
                {
                    Id = adventureId,
                    LocationId = locationId
                },
                new Credentials()
                {
                    UserId = userId.ToString()
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
            return routes.Select(r => r.Id).ToList();
        }
    }
}