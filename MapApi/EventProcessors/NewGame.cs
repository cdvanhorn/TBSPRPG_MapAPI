using System;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;

using MapApi.Adapters;
using MapApi.Entities;

namespace MapApi.EventProcessors
{
    public class NewGame : NewGameEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        // private IGameRepository _gameRepository;

        public NewGame(IEventStoreSettings eventStoreSettings, IDatabaseSettings databaseSettings) :
            base("map", eventStoreSettings, databaseSettings){
            _gameAdapter = new GameAggregateAdapter();
            // _gameRepository = new GameRepository(databaseSettings);
        }

        protected override void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            // //convert the aggregate to one of our game objects
            Game game = _gameAdapter.ToEntity(gameAggregate);
            Console.WriteLine(game);

            //Listen for new_game events
            //on new game event, build the aggregrate
            //check if the event id prefixed with this service id is in the list of event ids in the aggregrate
            //convert it to a MapApi.Game object
            //write the game to the database game collection for this service
            //look up the initial location for the given adventure
            //create an enter_location event that contains this service id plus the new_game event id
            
            // //if the game is missing fields ignore it
            // if(game.UserId == null || game.Adventure == null || game.UserId == game.Adventure.Id)
            //     return;
            // Console.WriteLine($"Writing Game {game.Id} {position}!!");
            // game.Events.Add(eventId);
            // _gameRepository.InsertGameIfDoesntExist(game, eventId);

            // //update the event index, if this fails it's not a big deal
            // //we'll end up reading duplicates
            // UpdatePosition(position);
            return;
        }
    }
}
