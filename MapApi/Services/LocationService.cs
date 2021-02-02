using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using MapApi.Entities;
using MapApi.Repositories;
using MapApi.ViewModels;

namespace MapApi.Services {
    public interface ILocationService {
        Task<List<LocationViewModel>> GetAllLocations();
        Task<LocationViewModel> GetLocationForGame(string gameId);
        Task<Location> GetLocation(Guid locationId);
        Task AddLocation(Location location);
    }

    public class LocationService : ILocationService {
        private ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository) {
            _locationRepository = locationRepository;
        }

        public async Task<List<LocationViewModel>> GetAllLocations() {
            var locations = await _locationRepository.GetAllLocations();
            return locations.Select(loc => new LocationViewModel(loc)).ToList();
        }

        public async Task<LocationViewModel> GetLocationForGame(string gameId) {
            Guid guid;
            if(!Guid.TryParse(gameId, out guid))
                return null;
            var location = await _locationRepository.GetLocationForGame(guid);
            if(location == null)
                return null;
            return new LocationViewModel(location);
        }

        public Task<Location> GetLocation(Guid locationId) {
            return _locationRepository.GetLocation(locationId);
        }

        public async Task AddLocation(Location location) {
            //try and get the location
            var dbLocation = await GetLocation(location.Id);
            if(dbLocation == null) {
                _locationRepository.AddLocation(location);
            }
        }
    }
}