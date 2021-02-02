using TbspRpgLib.Aggregates;
using MapApi.Entities;

using System;

namespace MapApi.Adapters {

    public interface IGameAggregateAdapter {
        GameAggregate ToAggregate(Game game);
        Game ToEntity(GameAggregate aggregate);
        Location ToLocationFromCheck(GameAggregate aggregate);
    }
        

    public class GameAggregateAdapter : IGameAggregateAdapter {
        public GameAggregate ToAggregate(Game game) {
            GameAggregate agg = new GameAggregate();
            agg.Id = game.Id.ToString();
            agg.UserId = game.UserId.ToString();
            agg.AdventureId = game.AdventureId.ToString();
            return agg;
        }

        public Game ToEntity(GameAggregate aggregate) {
            Game game = new Game();
            game.Id = Guid.Parse(aggregate.Id);
            game.UserId = Guid.Parse(aggregate.UserId);
            game.AdventureId = Guid.Parse(aggregate.AdventureId);
            return game;
        }

        public Location ToLocationFromCheck(GameAggregate aggregate) {
            Location location = new Location();
            location.GameId = Guid.Parse(aggregate.Id);
            if(aggregate.Checks.Location) {
                location.Id = Guid.Parse(aggregate.Destination);
            }
            return location;
        }
    }
}