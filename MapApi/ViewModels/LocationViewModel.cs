using System;
using MapApi.Entities;

namespace MapApi.ViewModels {
    public class LocationViewModel {
        public Guid Id { get; set; }

        public LocationViewModel() {}

        public LocationViewModel(Location location) {
            Id = location.Id;
        }
    }
}