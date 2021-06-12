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
            var agg = new GameAggregate
            {
                Id = game.Id.ToString(),
                UserId = game.UserId.ToString(),
                AdventureId = game.AdventureId.ToString()
            };
            return agg;
        }

        public Game ToEntity(GameAggregate aggregate) {
            var game = new Game
            {
                Id = Guid.Parse(aggregate.Id),
                UserId = Guid.Parse(aggregate.UserId),
                AdventureId = Guid.Parse(aggregate.AdventureId)
            };
            return game;
        }

        public Location ToLocationFromCheck(GameAggregate aggregate) {
            var location = new Location
            {
                GameId = Guid.Parse(aggregate.Id)
            };
            if(aggregate.Checks.Location) {
                location.LocationId = Guid.Parse(aggregate.MapData.DestinationLocation);         
            }
            return location;
        }
    }
}