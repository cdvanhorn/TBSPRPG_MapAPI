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
        void AddLocation(Location location);
        Task<Location> GetLocation(Guid locationId);
    }

    public class LocationRepository : ILocationRepository {
        private readonly MapContext _context;

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

        public void AddLocation(Location location) {
            _context.Locations.Add(location);
        }

        public Task<Location> GetLocation(Guid locationId) {
            return _context.Locations.AsQueryable().Where(loc => loc.Id == locationId).FirstOrDefaultAsync();
        }
    }
}