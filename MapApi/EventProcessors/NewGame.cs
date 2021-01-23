using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;
using TbspRpgLib.Services;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.Events;

using MapApi.Adapters;
using MapApi.Entities;
using MapApi.Services;
using MapApi.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace MapApi.EventProcessors
{
    public class NewGameHandler {
        private IGameService _gameService;
        private IServiceService _serviceService;
        private IAdventureServiceCom _adventureService;
        private IEventAdapter _eventAdapter;
        private IEventService _eventService;

        public NewGameHandler(IGameService gameService,
                IServiceService serviceService,
                IAdventureServiceCom adventureService,
                IEventService eventService) {
            _gameService = gameService;
            _serviceService = serviceService;
            _adventureService = adventureService;
            _eventService = eventService;
            _eventAdapter = new EventAdapter();
        }

        public async Task HandleNewGameEvent(Game game, ulong streamPosition) {
            //this will be our business logic, so we can do some testing
            
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
                Id = new Guid(responseDict["adventureId"]),
                GameId = game.Id
            });

            //send the event
            Console.WriteLine("Stream Position " + streamPosition);
            await _eventService.SendEvent(enterLocationEvent, false, streamPosition);
        }
    }
    public class NewGame : NewGameEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        private readonly IServiceScopeFactory _scopeFactory;

        public NewGame(IEventStoreSettings eventStoreSettings, IServiceScopeFactory scopeFactory) :
            base("map", eventStoreSettings){
            //create an adapter to convert game aggregate to a game object
            _gameAdapter = new GameAggregateAdapter();
            _scopeFactory = scopeFactory;
            IServiceScope scope  = _scopeFactory.CreateScope();
            MapContext context = scope.ServiceProvider.GetRequiredService<MapContext>();
            InitializeStartPosition(context);
        }

        protected override async void HandleEvent(Aggregate aggregate, string eventId, ulong streamPosition, ulong globalPosition) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            // //convert the aggregate to one of our game objects
            Game game = _gameAdapter.ToEntity(gameAggregate);

            Guid eventguid;
            if(!Guid.TryParse(eventId, out eventguid))
                return;

            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<MapContext>();
                var mapService = scope.ServiceProvider.GetRequiredService<IMapService>();
                if(await mapService.HasBeenProcessed(eventguid))
                    return;

                using var transaction = context.Database.BeginTransaction();
                try {
                    //create a handler
                    var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
                    var advServiceCom = scope.ServiceProvider.GetRequiredService<IAdventureServiceCom>();
                    var serviceService = scope.ServiceProvider.GetRequiredService<IServiceService>();
                    var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                    var handler = new NewGameHandler(
                        gameService,
                        serviceService,
                        advServiceCom,
                        eventService
                    );

                    // //update the event type position
                    await mapService.UpdatePosition(_eventType.Id, globalPosition);
                    // //update the processed events
                    await mapService.EventProcessed(eventguid);

                    //call the handler
                    await handler.HandleNewGameEvent(game, streamPosition);

                    //save the changes
                    context.SaveChanges();
                    transaction.Commit();
                } catch(Exception) {
                    //we need to do something with the exception
                    //there may be the potential for a rogue event to be out there
                    //may need to resubscribe so we attempt to process the event again
                    transaction.Rollback();
                }
            }
        }
    }
}
