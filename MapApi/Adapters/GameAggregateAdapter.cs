using TbspRpgLib.Aggregates;
using MapApi.Entities;

namespace MapApi.Adapters {

    public interface IGameAggregateAdapter {
        GameAggregate ToAggregate(Game game);
        Game ToEntity(GameAggregate aggregate);
    }
        

    public class GameAggregateAdapter : IGameAggregateAdapter {
        public GameAggregate ToAggregate(Game game) {
            GameAggregate agg = new GameAggregate();
            agg.Id = game.Id;
            agg.UserId = game.UserId;
            agg.AdventureId = game.AdventureId;
            return agg;
        }

        public Game ToEntity(GameAggregate aggregate) {
            Game game = new Game();
            game.Id = aggregate.Id;
            game.UserId = aggregate.UserId;
            game.AdventureId = aggregate.AdventureId;
            return game;
        }
    }
}