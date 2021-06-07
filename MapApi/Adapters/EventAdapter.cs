using System.Collections.Generic;
using MapApi.Entities;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Location;
using TbspRpgLib.Events.Location.Content;

namespace MapApi.Adapters {
    public interface IEventAdapter {
        Event NewEnterLocationEvent(Location location, List<string> routes);
        Event NewLocationEnterPassEvent(Location location);
        Event NewLocationEnterFailEvent(Location location);
    }

    public class EventAdapter : IEventAdapter {
        public Event NewEnterLocationEvent(Location location, List<string> routes) {
            var enterLocation = new LocationEnter
            {
                DestinationLocation = location.Id.ToString(),
                DestinationRoutes = routes,
                Id = location.GameId.ToString()
            };

            LocationEnterEvent evnt = new LocationEnterEvent(enterLocation);
            return evnt;
        }

        public Event NewLocationEnterPassEvent(Location location) {
            var content = new LocationEnterPass
            {
                CurrentLocation = location.LocationId.ToString(),
                Id = location.GameId.ToString(),
                DestinationLocation = ""
            };

            return new LocationEnterPassEvent(content);
        }

        public Event NewLocationEnterFailEvent(Location location) {
            var content = new LocationEnterFail
            {
                Id = location.GameId.ToString(),
                DestinationLocation = ""
            };
            return new LocationEnterFailEvent(content);
        }
    }
}