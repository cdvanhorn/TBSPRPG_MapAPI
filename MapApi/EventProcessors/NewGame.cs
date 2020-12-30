using System;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;
using TbspRpgLib.Services;

using MapApi.Adapters;
using MapApi.Entities;
using MapApi.Services;
using MapApi.Repositories;

namespace MapApi.EventProcessors
{
    public class NewGameHandler {
        private IGameService _gameService;
        private IServiceService _serviceService;

        public NewGameHandler(IGameService gameService, IServiceService serviceService) {
            _gameService = gameService;
            _serviceService = serviceService;
        }

        public async void HandleNewGameEvent(Game game) {
            //this will be our business logic, so we can do some testing
            // //if the game is missing fields ignore it
            if(game.UserId == null || game.AdventureId == null || game.UserId == game.AdventureId)
                 return;
            _gameService.InsertGameIfItDoesntExist(game);

            //get the initial location
            var adventureUrl = await _serviceService.GetUrlForService(ServiceService.ADVENTURE_SERVICE_NAME);
            Console.WriteLine(adventureUrl);

            //create an enter_location event that contains this service id plus the new_game event id
        }
    }
    public class NewGame : NewGameEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        private IGameService _gameService;
        private NewGameHandler _newGameHandler;

        public NewGame(IEventStoreSettings eventStoreSettings, IDatabaseSettings databaseSettings) :
            base("map", eventStoreSettings, databaseSettings){
            _gameAdapter = new GameAggregateAdapter();
            IGameRepository _gameRepository = new GameRepository(databaseSettings);
            _gameService = new GameService(_gameRepository);
            _newGameHandler = new NewGameHandler(_gameService, _serviceService);
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
