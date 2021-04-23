using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.InterServiceCommunication;

using MapApi.Services;
using MapApi.Entities;
using TbspRpgLib.InterServiceCommunication.RequestModels;

namespace MapApi.EventProcessors {
    public interface INewGameEventHandler : IEventHandler {
    }

    public class NewGameEventHandler : EventHandler, INewGameEventHandler {
        private readonly IGameService _gameService;
        private readonly IAggregateService _aggregateService;
        private readonly IAdventureServiceLink _adventureService;

        public NewGameEventHandler(
            IGameService gameService,
            IAggregateService aggregateService,
            IAdventureServiceLink adventureServiceCom
            ) : base()
        {
            _gameService = gameService;
            _aggregateService = aggregateService;
            _adventureService = adventureServiceCom;
        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //this will be our business logic, so we can do some testing
            Game game = _gameAdapter.ToEntity(gameAggregate);

            //get the initial location
            var responseTask = _adventureService.GetInitialLocation(
                new AdventureRequest() { Id = game.AdventureId },
                new Credentials() { UserId = game.UserId.ToString() });

            // //if the game is missing fields or some fields are the same ignore it
            await _gameService.AddGame(game);

            var response = await responseTask;
            //game to get the location id from the response
            var responseDict = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Response.Content);

            //create an enter_location event that contains this service id plus the new_game event id
            var enterLocationEvent = _eventAdapter.NewEnterLocationEvent(new Location() {
                Id = new Guid(responseDict["id"]),
                GameId = game.Id
            });

            //if the aggregate already have a destination equal to the location we're setting,
            //don't send the event
            if(gameAggregate.Destination != null && gameAggregate.Destination == responseDict["id"])
                return;

            //send the event
            await _aggregateService.AppendToAggregate(
                AggregateService.GAME_AGGREGATE_TYPE,
                enterLocationEvent,
                gameAggregate.StreamPosition);
        }
    }
}