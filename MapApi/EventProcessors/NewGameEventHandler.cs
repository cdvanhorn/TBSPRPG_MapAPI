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
            IAdventureServiceLink adventureServiceLink
            ) : base(adventureServiceLink)
        {
            _gameService = gameService;
            _aggregateService = aggregateService;
        }

        private async Task<Event> CreateEnterLocationEvent(Game game, string aggregateDestinationLocation)
        {
            //get the initial location
            var initialLocationTask = _adventureServiceLink.GetInitialLocation(
                new AdventureRequest() { Id = game.AdventureId },
                new Credentials() { UserId = game.UserId.ToString() });
            
            var initialLocationResponse = await initialLocationTask;
            //game to get the location id from the response
            var initialLocation = JsonSerializer.Deserialize<InitialLocation>(
                initialLocationResponse.Response.Content,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            
            //if the aggregate already have a destination equal to the location we're setting,
            //don't send the event
            if(aggregateDestinationLocation != null && 
               aggregateDestinationLocation == initialLocation.Id.ToString())
                return null;
            
            //get the routes for the initial location
            var routeTask = _adventureServiceLink.GetRoutesForLocation(
                new AdventureRequest()
                {
                    Id = game.AdventureId,
                    LocationId = initialLocation.Id
                },
                new Credentials()
                {
                    UserId = game.UserId.ToString()
                }
            );
            var routeResponse = await routeTask;
            var routes = JsonSerializer.Deserialize<List<Route>>(
                routeResponse.Response.Content,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            //create and return an enter_location event
            var enterLocationEvent = _eventAdapter.NewEnterLocationEvent(new Location() {
                Id = initialLocation.Id,
                GameId = game.Id
            }, routes.Select(r => r.Id.ToString()).ToList());

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