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
    }
}