using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace MapApi.Repositories
{
    public interface IRouteRepository
    {
        Task<List<Route>> GetRoutesForGame(Guid gameId);
    }
    
    public class RouteRepository : IRouteRepository
    {
        private readonly MapContext _context;

        public RouteRepository(MapContext context)
        {
            _context = context;
        }
        
        public Task<List<Route>> GetRoutesForGame(Guid gameId)
        {
            return _context.Locations.AsQueryable()
                .Where(l => l.GameId == gameId)
                .Join(
                    _context.Routes,
                    location => location.Id,
                    route => route.LocationId,
                    (location, route) => route
                ).ToListAsync();
        }
    }
}