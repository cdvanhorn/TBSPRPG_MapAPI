using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using TbspRpgLib.Settings;
using TbspRpgLib.Repositories;

using MapApi.Entities;

namespace MapApi.Repositories {
    public interface ILocationRepository {
        Task<List<Location>> GetAllLocations();
        Task<Location> GetLocationForGame(int gameId);
    }

    public class LocationRepository : ILocationRepository {
        private MapContext _context;

        public LocationRepository(MapContext context) {
            _context = context;
        }

        public Task<List<Location>> GetAllLocations() {
            return _context.Locations.AsQueryable().ToListAsync();
        }

        public Task<Location> GetLocationForGame(int gameId) {
            return _context.Locations.AsQueryable()
                .Where(loc => loc.GameId == gameId)
                .FirstOrDefaultAsync();
        }
    }
}