using System.Threading.Tasks;
using MapApi.Services;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;

namespace MapApi.EventProcessors {
    public interface IEnterLocationCheckEventHandler : IEventHandler {}

    public class EnterLocationCheckEventHandler : EventHandler, IEnterLocationCheckEventHandler {
        private readonly ILocationService _locationService;
        private readonly IAggregateService _aggregateService;
        private readonly IRouteService _routeService;

        public EnterLocationCheckEventHandler(
            IAggregateService aggregateService,
            ILocationService locationService,
            IRouteService routeService,
            IAdventureService adventureService) : base(adventureService)
        {
            _aggregateService = aggregateService;
            _locationService = locationService;
            _routeService = routeService;
        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //send an location_enter_pass event or location_enter_fail event
            Event resultEvent;
            var loc = _gameAdapter.ToLocationFromCheck(gameAggregate);
            var game = _gameAdapter.ToEntity(gameAggregate);
            if(gameAggregate.Checks.Location)
            {
                var routes = await _adventureService.GetRoutesForLocation(
                    game, loc.LocationId);
                loc.Routes = routes;
                resultEvent = _eventAdapter.NewLocationEnterPassEvent(loc);
                //the routes need to make it in to the database, and we need to update the aggregate
                await _locationService.AddOrUpdateLocation(loc);
                await _routeService.SyncRoutesForGame(loc.GameId, routes);
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