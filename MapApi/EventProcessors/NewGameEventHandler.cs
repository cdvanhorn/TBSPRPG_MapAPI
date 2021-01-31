using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Services;
using TbspRpgLib.InterServiceCommunication;

using MapApi.Services;
using MapApi.Entities;

namespace MapApi.EventProcessors {
    public interface INewGameEventHandler : IEventHandler {
    }

    public class NewGameEventHandler : EventHandler, INewGameEventHandler {
        private IGameService _gameService;
        private IServiceService _serviceService;
        private IAdventureServiceCom _adventureService;
        private IEventService _eventService;

        public NewGameEventHandler(
            IGameService gameService,
            IServiceService serviceService,
            IAdventureServiceCom adventureServiceCom,
            IEventService eventService) : base()
        {
            _gameService = gameService;
            _serviceService = serviceService;
            _adventureService = adventureServiceCom;
            _eventService = eventService;
        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //this will be our business logic, so we can do some testing
            Game game = _gameAdapter.ToEntity(gameAggregate);

            //get the initial location
            var responseTask = _adventureService.GetInitialLocation(
                game.AdventureId.ToString(),
                game.UserId.ToString()
            );

            // //if the game is missing fields or some fields are the same ignore it
            await _gameService.AddGame(game);

            var response = await responseTask;
            //game to get the location id from the response
            var responseDict = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Response.Content);

            //create an enter_location event that contains this service id plus the new_game event id
            Event enterLocationEvent = _eventAdapter.NewEnterLocationEvent(new Location() {
                Id = new Guid(responseDict["id"]),
                GameId = game.Id
            });

            //if the aggregate already have a destination equal to the location we're setting,
            //don't send the event
            if(gameAggregate.Destination != null && gameAggregate.Destination == responseDict["id"])
                return;

            //send the event
            await _eventService.SendEvent(enterLocationEvent, gameAggregate.StreamPosition);
        }
    }
}