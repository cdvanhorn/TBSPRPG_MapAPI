using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Location.Content;
using TbspRpgLib.Events.Location;

using System.Threading.Tasks;

namespace MapApi.EventProcessors {
    public interface IEnterLocationCheckEventHandler : IEventHandler {}

    public class EnterLocationCheckEventHandler : IEnterLocationCheckEventHandler {
        IEventService _eventService;

        public EnterLocationCheckEventHandler(IEventService eventService) : base() {
            _eventService = eventService;
        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //send an location_enter_pass event or location_enter_fail event
            Event resultEvent;
            if(gameAggregate.Checks.Location) {
                //create the event content
                LocationEnterPass content = new LocationEnterPass();
                content.CurrentLocation = gameAggregate.Destination;
                content.Destination = "";
                content.Id = gameAggregate.Id;

                //create the event
                resultEvent = new LocationEnterPassEvent(content);

                //we also need to add the location to the database
            } else {
                LocationEnterFail content = new LocationEnterFail();
                content.Destination = "";
                content.Id = gameAggregate.Id;

                //create the event
                resultEvent = new LocationEnterFailEvent(content);
            }
            await _eventService.SendEvent(resultEvent, gameAggregate.StreamPosition);
        }
    }
}