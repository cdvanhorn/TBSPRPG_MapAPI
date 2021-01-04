using System;
using System.Collections.Generic;

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

        public void HandleNewGameEvent(Game game) {
            //this will be our business logic, so we can do some testing
            // //if the game is missing fields or some fields are the same ignore it
            _gameService.InsertGameIfItDoesntExist(game);

            //get the initial location
	        //var response = await _adventureService.GetInitialLocation(game.AdventureId, game.UserId);
            //Console.WriteLine(response.Response.Content);

            //create an enter_location event that contains this service id plus the new_game event id
        }
    }
    public class NewGame : NewGameEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        private IGameService _gameService;
        private NewGameHandler _newGameHandler;

        public NewGame(IEventStoreSettings eventStoreSettings, IDatabaseSettings databaseSettings, IJwtSettings jwtSettings) :
            base("map", eventStoreSettings, databaseSettings){
            //create an adapter to convert game aggregate to a game object
            _gameAdapter = new GameAggregateAdapter();

            //create a context so can get the game repository and service
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var optionsBuilder = new DbContextOptionsBuilder<MapContext>();
            optionsBuilder.UseNpgsql(connectionString);
            var context = new MapContext(optionsBuilder.Options);
            IGameRepository _gameRepository = new GameRepository(context);
            _gameService = new GameService(_gameRepository);

            //create service communication object to call other services
            var serviceCommunication = new ServiceCommunication(_serviceService, jwtSettings);

            //create a new game handler that has a method that will be called when a new game is received
            _newGameHandler = new NewGameHandler(_gameService, _serviceService,
                new AdventureServiceCom(serviceCommunication));
        }

        protected override void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            // //convert the aggregate to one of our game objects
            Game game = _gameAdapter.ToEntity(gameAggregate);
            
            _newGameHandler.HandleNewGameEvent(game);

            // //update the event index, if this fails it's not a big deal
            // //we'll end up reading duplicates
            UpdatePosition(position);
            return;
        }
    }
}
