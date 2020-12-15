using System;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;

using MapApi.Adapters;
using MapApi.Entities;
using MapApi.Services;
using MapApi.Repositories;

namespace MapApi.EventProcessors
{
    public class NewGame : NewGameEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        private IGameService _gameService;

        public NewGame(IEventStoreSettings eventStoreSettings, IDatabaseSettings databaseSettings) :
            base("map", eventStoreSettings, databaseSettings){
            _gameAdapter = new GameAggregateAdapter();
            IGameRepository _gameRepository = new GameRepository(databaseSettings);
            _gameService = new GameService(_gameRepository);
        }

        protected override void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            // //convert the aggregate to one of our game objects
            Game game = _gameAdapter.ToEntity(gameAggregate);
            
            //Listen for new_game events
            //on new game event, build the aggregrate
            //check if the event id prefixed with this service id is in the list of event ids in the aggregrate
            //convert it to a MapApi.Game object
            //write the game to the database game collection for this service
            //look up the initial location for the given adventure
            //create an enter_location event that contains this service id plus the new_game event id
            
            // //if the game is missing fields ignore it
            if(game.UserId == null || game.AdventureId == null || game.UserId == game.AdventureId)
                 return;
            _gameService.InsertGameIfItDoesntExist(game);

            // //update the event index, if this fails it's not a big deal
            // //we'll end up reading duplicates
            // UpdatePosition(position);
            return;
        }
    }
}
