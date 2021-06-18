using MapApi.Entities;

namespace MapApi.Adapters
{
    public static class AdventureServiceAdapter
    {
        public static Route ToRouteEntity(Entities.AdventureService.Route serviceRoute)
        {
            return new Route()
            {
                RouteId = serviceRoute.Id,
                LocationId = serviceRoute.LocationId,
                Name = serviceRoute.Name,
                Content = serviceRoute.Content
            };
        }
    }
}