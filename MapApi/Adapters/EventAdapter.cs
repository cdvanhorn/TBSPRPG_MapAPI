using MapApi.Entities;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Content;

namespace MapApi.Adapters {
    public interface IEventAdapter {
        Event NewEnterLocationEvent(Location location);
    }

    public class EventAdapter : IEventAdapter {
        public Event NewEnterLocationEvent(Location location) {
            EnterLocation enterLocation = new EnterLocation();
            enterLocation.Destination = location.Id.ToString();
            enterLocation.Id = location.GameId.ToString();
            
            EnterLocationEvent evnt = new EnterLocationEvent(enterLocation);
            return evnt;
        }
    }
}