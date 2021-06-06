using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using MapApi.Entities;
using MapApi.Repositories;
using MapApi.ViewModels;
using TbspRpgLib.InterServiceCommunication;

namespace MapApi.Services {
    public interface ILocationService {
        Task<List<LocationViewModel>> GetAllLocations();
        Task<LocationViewModel> GetLocationForGameVm(Guid gameId);
        Task<Location> GetLocation(Guid locationId);
        Task AddLocation(Location location);
        Task AddOrUpdateLocation(Location location);
    }

    public class LocationService : ILocationService {
        private readonly ILocationRepository _locationRepository;
        //private readonly IAdventureServiceLink _adventureService;

        //public LocationService(ILocationRepository locationRepository, IAdventureServiceLink adventureService) {
        public LocationService(ILocationRepository locationRepository) {
            _locationRepository = locationRepository;
            //_adventureService = adventureService;
        }

        public async Task<List<LocationViewModel>> GetAllLocations() {
            var locations = await _locationRepository.GetAllLocations();
            return locations.Select(loc => new LocationViewModel(loc)).ToList();
        }

        public async Task<LocationViewModel> GetLocationForGameVm(Guid gameId) {
            var location = await _locationRepository.GetLocationForGame(gameId);
            return location == null ? null : new LocationViewModel(location);
        }
        
        public Task<Location> GetLocationForGame(Guid gameId) {
            return _locationRepository.GetLocationForGame(gameId);
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

        public async Task AddOrUpdateLocation(Location location)
        {
            var existingLocation = await GetLocationForGame(location.GameId);
            if (existingLocation != null)
            {
                existingLocation.LocationId = location.LocationId;
            }
            else
            {
                await AddLocation(location);
            }
        }

        // public async Task EnterLocation(Location location)
        // {
        //     //use the inter service communication endpoints
        // }
    }
}