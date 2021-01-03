using System;
using System.Collections.Generic;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;
using TbspRpgLib.Services;
using TbspRpgLib.Jwt;
using TbspRpgLib.InterServiceCommunication;

using MapApi.Adapters;
using MapApi.Entities;
using MapApi.Services;
using MapApi.Repositories;

namespace MapApi.EventProcessors
{
    public class NewGameHandler {
        private readonly int IDS_TO_COMPARE = 3;
        private IGameService _gameService;
        private IServiceService _serviceService;
        private IAdventureServiceCom _adventureService;

        public NewGameHandler(IGameService gameService, IServiceService serviceService, IAdventureServiceCom adventureService) {
            _gameService = gameService;
            _serviceService = serviceService;
            _adventureService = adventureService;
        }

        public async void HandleNewGameEvent(Game game) {
            //this will be our business logic, so we can do some testing
            // //if the game is missing fields or some fields are the same ignore it
            if(game.UserId == null || game.AdventureId == null || game.Id == null)
                 return;
            HashSet<string> ids = new HashSet<string>(IDS_TO_COMPARE);
            ids.Add(game.UserId);
            ids.Add(game.Id);
            ids.Add(game.AdventureId);
            if(ids.Count != 3)
                return;
            _gameService.InsertGameIfItDoesntExist(game);

            //get the initial location
	        //AdventureService.GetInitialLocation(adventureid, userid);
            var response = await _adventureService.GetInitialLocation(game.AdventureId, game.UserId);
            Console.WriteLine(response.Response.Content);

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
            _gameAdapter = new GameAggregateAdapter();
            IGameRepository _gameRepository = new GameRepository(databaseSettings);
            _gameService = new GameService(_gameRepository);
            var serviceCommunication = new ServiceCommunication(_serviceService, jwtSettings);
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
