using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;
using TbspRpgLib.Services;
using TbspRpgLib.InterServiceCommunication;

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

        public NewGameHandler(IGameService gameService, IServiceService serviceService, IAdventureServiceCom adventureService) {
            _gameService = gameService;
            _serviceService = serviceService;
            _adventureService = adventureService;
        }

        public async Task HandleNewGameEvent(Game game) {
            //this will be our business logic, so we can do some testing
            // //if the game is missing fields or some fields are the same ignore it
            await _gameService.AddGame(game);

            //get the initial location
	        var response = await _adventureService.GetInitialLocation(
                game.AdventureId.ToString(),
                game.UserId.ToString()
            );
            Console.WriteLine(response.Response.Content);

            //create an enter_location event that contains this service id plus the new_game event id
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

        protected override async void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
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

                //create a handler
                var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
                var advServiceCom = scope.ServiceProvider.GetRequiredService<IAdventureServiceCom>();
                var serviceService = scope.ServiceProvider.GetRequiredService<IServiceService>();
                var handler = new NewGameHandler(
                    gameService,
                    serviceService,
                    advServiceCom
                );

                //call the handler
                await handler.HandleNewGameEvent(game);

                // //update the event type position
                await mapService.UpdatePosition(_eventType.Id, position);
                // //update the processed events
                await mapService.EventProcessed(eventguid);

                //save the changes
                context.SaveChanges();
            }
        }
    }
}
