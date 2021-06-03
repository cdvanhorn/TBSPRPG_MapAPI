using System;
using MapApi.Entities;
using MapApi.Repositories;
using Xunit;

namespace MapApi.Tests.Repositories
{
    public class RouteRepositoryTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testGameId = Guid.NewGuid();
        private readonly Guid _testLocationId = Guid.NewGuid();

        public RouteRepositoryTests() : base("RouteRepositoryTests")
        {
            Seed();
        }

        private void Seed()
        {
            using var context = new MapContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var game = new Game()
            {
                Id = _testGameId,
                Location = new Location()
                {
                    Id = _testLocationId,
                    LocationId = Guid.NewGuid(),
                    GameId = _testGameId
                }
            };

            var route = new Route()
            {
                Id = Guid.NewGuid(),
                LocationId = _testLocationId,
                Name = "testroute"
            };
            
            var route2 = new Route()
            {
                Id = Guid.NewGuid(),
                LocationId = Guid.NewGuid(),
                Name = "testroute2"
            };

            context.Games.Add(game);
            context.Routes.AddRange(route, route2);
            context.SaveChanges();
        }

        #endregion
        
        #region GetRoutesForGame

        [Fact]
        public async void GetRoutesForGame_ReturnsRoute()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new RouteRepository(context);
            
            //act
            var routes = await repository.GetRoutesForGame(_testGameId);

            //assert
            Assert.Single(routes);
            Assert.Equal(_testLocationId, routes[0].LocationId);
        }

        [Fact]
        public async void GetRoutesForGame_NoGame_NoRoutes()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new RouteRepository(context);
            
            //act
            var routes = await repository.GetRoutesForGame(Guid.NewGuid());

            //assert
            Assert.Empty(routes);
        }
        
        #endregion
    }
}