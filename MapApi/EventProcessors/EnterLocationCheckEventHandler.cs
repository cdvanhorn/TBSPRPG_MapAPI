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
        private readonly ILocationService _locationService;
        private readonly IAggregateService _aggregateService;

        public EnterLocationCheckEventHandler(IAggregateService aggregateService, ILocationService locationService) : base()
        {
            _aggregateService = aggregateService;
            _locationService = locationService;
        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //send an location_enter_pass event or location_enter_fail event
            Event resultEvent;
            var loc = _gameAdapter.ToLocationFromCheck(gameAggregate);
            if(gameAggregate.Checks.Location) {
                resultEvent = _eventAdapter.NewLocationEnterPassEvent(loc);
                await _locationService.AddOrUpdateLocation(loc);
                
                //get the routes from the adventure
                //send a validate routes event
                //the game system api will listen for those events
                //  and check if the routes are available to the player based on their statistics
                //  we'll check if the routes are available/unavailable because of game state
                //the game system api will produce a ValidateRouteCheck event
                //  which we'll listen for and update the current routes for this game
            } else {
                resultEvent = _eventAdapter.NewLocationEnterFailEvent(loc);
            }

            await _aggregateService.AppendToAggregate(
                AggregateService.GAME_AGGREGATE_TYPE,
                resultEvent,
                gameAggregate.StreamPosition);
        }
    }
}