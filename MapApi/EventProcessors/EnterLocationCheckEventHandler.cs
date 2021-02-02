using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Location.Content;
using TbspRpgLib.Events.Location;

using MapApi.Entities;
using MapApi.Services;

using System.Threading.Tasks;

namespace MapApi.EventProcessors {
    public interface IEnterLocationCheckEventHandler : IEventHandler {}

    public class EnterLocationCheckEventHandler : EventHandler, IEnterLocationCheckEventHandler {
        IEventService _eventService;
        ILocationService _locationService;

        public EnterLocationCheckEventHandler(IEventService eventService, ILocationService locationService) : base() {
            _eventService = eventService;
            _locationService = locationService;
        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //send an location_enter_pass event or location_enter_fail event
            Event resultEvent;
            Location loc = _gameAdapter.ToLocationFromCheck(gameAggregate);
            if(gameAggregate.Checks.Location) {
                resultEvent = _eventAdapter.NewLocationEnterPassEvent(loc);
                await _locationService.AddLocation(loc);
            } else {
                resultEvent = _eventAdapter.NewLocationEnterFailEvent(loc);
            }
            await _eventService.SendEvent(resultEvent, gameAggregate.StreamPosition);
        }
    }
}