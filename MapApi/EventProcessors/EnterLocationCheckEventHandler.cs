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
            IAdventureServiceLink adventureServiceLink) : base(adventureServiceLink)
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
                //get the routes from the adventure
                // var routesTask = _adventureServiceLink.GetRoutesForLocation(
                //     new AdventureRequest()
                //     {
                //         LocationId = loc.LocationId
                //     }, new Credentials()
                //     {
                //         UserId = game.UserId.ToString()
                //     });
                
                resultEvent = _eventAdapter.NewLocationEnterPassEvent(loc);
                await _locationService.AddOrUpdateLocation(loc);

                // var routeResponse = await routesTask;
                // var serviceRoutes = JsonSerializer.Deserialize<List<Route>>(routeResponse.Response.Content);
                //serviceRoutes = FilterRoutes(serviceRoutes);
                //send a validate route event that will have a list of routes to check

                //send a validate routes event
                //the game system api will listen for those events
                //  and check if the routes are available to the player based on their statistics
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