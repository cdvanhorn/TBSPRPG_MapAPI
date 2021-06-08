using System.Collections.Generic;
using System.Text.Json;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Location.Content;
using TbspRpgLib.Events.Location;

using MapApi.Entities;
using MapApi.Services;

using System.Threading.Tasks;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.InterServiceCommunication.RequestModels;
using Route = MapApi.Entities.AdventureService.Route;

namespace MapApi.EventProcessors {
    public interface IEnterLocationCheckEventHandler : IEventHandler {}

    public class EnterLocationCheckEventHandler : EventHandler, IEnterLocationCheckEventHandler {
        private readonly ILocationService _locationService;
        private readonly IAggregateService _aggregateService;

        public EnterLocationCheckEventHandler(
            IAggregateService aggregateService,
            ILocationService locationService,
            IAdventureService adventureService) : base(adventureService)
        {
            _aggregateService = aggregateService;
            _locationService = locationService;
        }
        
        

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //send an location_enter_pass event or location_enter_fail event
            Event resultEvent;
            var loc = _gameAdapter.ToLocationFromCheck(gameAggregate);
            var game = _gameAdapter.ToEntity(gameAggregate);
            if(gameAggregate.Checks.Location) {
                resultEvent = _eventAdapter.NewLocationEnterPassEvent(loc);
                await _locationService.AddOrUpdateLocation(loc);
            } else {
                resultEvent = _eventAdapter.NewLocationEnterFailEvent(loc);
            }

            await _aggregateService.AppendToAggregate(
                AggregateService.GAME_AGGREGATE_TYPE,
                resultEvent,
                gameAggregate.StreamPosition);
        }

        private List<Route> FilterRoutes(List<Route> serviceRoutes)
        {
            //filter which routes are available based on game state
            //i.e. route on available if the talked_to_bob variable is set to true
            //conditions will be defined by the adventure
            //for now all of the routes are available
            return serviceRoutes;
        }
    }
}