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
            LocationEnter enterLocation = new LocationEnter();
            enterLocation.Destination = location.Id.ToString();
            enterLocation.Id = location.GameId.ToString();
            
            LocationEnterEvent evnt = new LocationEnterEvent(enterLocation);
            return evnt;
        }

        public Event NewLocationEnterPassEvent(Location location) {
            LocationEnterPass content = new LocationEnterPass();
            content.CurrentLocation = location.Id.ToString();
            content.Id = location.GameId.ToString();
            content.Destination = "";

            return new LocationEnterPassEvent(content);
        }

        public Event NewLocationEnterFailEvent(Location location) {
            LocationEnterFail content = new LocationEnterFail();
            content.Id = location.GameId.ToString();
            content.Destination = "";
            return new LocationEnterFailEvent(content);
        }
    }
}