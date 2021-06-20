using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.InterServiceCommunication;

using MapApi.Services;
using MapApi.Entities;
using MapApi.Entities.AdventureService;
using TbspRpgLib.InterServiceCommunication.RequestModels;
using Route = MapApi.Entities.AdventureService.Route;

namespace MapApi.EventProcessors {
    public interface INewGameEventHandler : IEventHandler {
    }

    public class NewGameEventHandler : EventHandler, INewGameEventHandler {
        private readonly IGameService _gameService;
        private readonly IAggregateService _aggregateService;

        public NewGameEventHandler(
            IGameService gameService,
            IAggregateService aggregateService,
            IAdventureService adventureService
            ) : base(adventureService)
        {
            _gameService = gameService;
            _aggregateService = aggregateService;
        }

        private async Task<Event> CreateEnterLocationEvent(Game game, string aggregateDestinationLocation)
        {
            var initialLocationId = await _adventureService.GetInitialLocationId(
                game.AdventureId, game.UserId);
            
            //if the aggregate already have a destination equal to the location we're setting,
            //don't send the event
            if(aggregateDestinationLocation != null && 
               aggregateDestinationLocation == initialLocationId.ToString())
                return null;
            
            //get the routes for the initial location
            var routeIds = await _adventureService.GetRouteIdsForLocation(
                game, initialLocationId);
            
            Console.WriteLine(routeIds);

            //create and return an enter_location event
            var enterLocationEvent = _eventAdapter.NewEnterLocationEvent(new Location() {
                Id = initialLocationId,
                GameId = game.Id
            }, routeIds.Select(routeId => routeId.ToString()).ToList());

            return enterLocationEvent;
        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //this will be our business logic, so we can do some testing
            Game game = _gameAdapter.ToEntity(gameAggregate);

            var createEventTask = CreateEnterLocationEvent(game, gameAggregate.MapData.DestinationLocation);
            
            //if the game is missing fields or some fields are the same ignore it
            await _gameService.AddGame(game);

            var enterLocationEvent = await createEventTask;
            if (enterLocationEvent == null)
                return;

            //send the event
            await _aggregateService.AppendToAggregate(
                AggregateService.GAME_AGGREGATE_TYPE,
                enterLocationEvent,
                gameAggregate.StreamPosition);
        }
    }
}