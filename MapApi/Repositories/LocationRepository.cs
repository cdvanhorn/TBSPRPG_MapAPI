using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Driver;

using TbspRpgLib.Settings;

using MapApi.Entities;

namespace MapApi.Repositories {
    interface IMapRepository {
        Task<List<Location>> GetAllLocations();
        Task<Location> GetLocationForGame(string gameId);
    }

    public class MapRepository : MongoRepository, IMapRepository {
        private IMongoCollection<Location> _locations;

        public MapRepository(IDatabaseSettings databaseSettings) : base(databaseSettings) {
            _locations = _mongoDatabase.GetCollection<Location>("locations");
        }

        public Task<List<Location>> GetAllLocations() {
            return _locations.Find(location => true).ToListAsync();
        }

        public Task<Location> GetLocationForGame(string gameId) {
            return _locations.Find(loc => loc.GameId == gameId).FirstOrDefaultAsync();
        }
    }
}