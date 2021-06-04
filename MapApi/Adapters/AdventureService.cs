using MapApi.Entities;

namespace MapApi.Adapters
{
    public class AdventureService
    {
        public Route ToRouteEntity(Entities.AdventureService.Route serviceRoute)
        {
            return new Route()
            {
                RouteId = serviceRoute.Id,
                LocationId = serviceRoute.LocationId,
                Name = serviceRoute.Name
            };
        }
    }
}