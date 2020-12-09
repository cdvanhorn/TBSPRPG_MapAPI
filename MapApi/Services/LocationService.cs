using System.Collections.Generic;
using System.Threading.Tasks;

using MapApi.Entities;
using MapApi.Repositories;

namespace MapApi.Services {
    interface ILocationService {
        Task<List<Location>> GetAllLocations();

        Task<Location> GetLocationForGame(string gameId);
    }

    public class LocationService : ILocationService {
        private ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository) {
            _locationRepository = locationRepository;
        }

        public Task<List<Location>> GetAllLocations() {
            return _locationRepository.GetAllLocations();
        }

        public Task<Location> GetLocationForGame(string gameId) {
            return _locationRepository.GetLocationForGame(gameId);
        }
    }
}