using MapApi.Entities;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Location;
using TbspRpgLib.Events.Location.Content;

namespace MapApi.Adapters {
    public interface IEventAdapter {
        Event NewEnterLocationEvent(Location location);
        Event NewLocationEnterPassEvent(Location location);
        Event NewLocationEnterFailEvent(Location location);
    }

    public class EventAdapter : IEventAdapter {
        public Event NewEnterLocationEvent(Location location) {
            var enterLocation = new LocationEnter
            {
                Destination = location.Id.ToString(),
                Id = location.GameId.ToString()
            };

            LocationEnterEvent evnt = new LocationEnterEvent(enterLocation);
            return evnt;
        }

        public Event NewLocationEnterPassEvent(Location location) {
            var content = new LocationEnterPass
            {
                CurrentLocation = location.Id.ToString(),
                Id = location.GameId.ToString(),
                Destination = ""
            };

            return new LocationEnterPassEvent(content);
        }

        public Event NewLocationEnterFailEvent(Location location) {
            var content = new LocationEnterFail
            {
                Id = location.GameId.ToString(),
                Destination = ""
            };
            return new LocationEnterFailEvent(content);
        }
    }
}