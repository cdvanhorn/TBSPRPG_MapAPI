using MapApi.Entities;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Location;
using TbspRpgLib.Events.Location.Content;

namespace MapApi.Adapters {
    public interface IEventAdapter {
        Event NewEnterLocationEvent(Location location);
    }

    public class EventAdapter : IEventAdapter {
        public Event NewEnterLocationEvent(Location location) {
            LocationEnter enterLocation = new LocationEnter();
            enterLocation.Destination = location.Id.ToString();
            enterLocation.Id = location.GameId.ToString();
            
            LocationEnterEvent evnt = new LocationEnterEvent(enterLocation);
            return evnt;
        }
    }
}