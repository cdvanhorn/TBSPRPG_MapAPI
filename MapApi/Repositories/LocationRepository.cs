using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using Microsoft.EntityFrameworkCore;

using MapApi.Entities;

namespace MapApi.Repositories {
    public interface ILocationRepository {
        Task<List<Location>> GetAllLocations();
        Task<Location> GetLocationForGame(Guid gameId);
    }

    public class LocationRepository : ILocationRepository {
        private MapContext _context;

        public LocationRepository(MapContext context) {
            _context = context;
        }

        public Task<List<Location>> GetAllLocations() {
            return _context.Locations.AsQueryable().ToListAsync();
        }

        public Task<Location> GetLocationForGame(Guid gameId) {
            return _context.Locations.AsQueryable()
                .Where(loc => loc.GameId == gameId)
                .FirstOrDefaultAsync();
        }
    }
}