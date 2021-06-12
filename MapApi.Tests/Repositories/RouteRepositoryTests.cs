using System;
using System.Collections.Generic;
using System.Linq;
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

        #region RemoveRoutes

        [Fact]
        public async void RemoveRoutes_RoutesRemoved()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new RouteRepository(context);
            var routesToRemove = context.Routes.AsQueryable().Where(route => route.LocationId == _testLocationId);

            //act
            repository.RemoveRoutes(routesToRemove);
            
            //assert
            context.SaveChanges();
            Assert.Single(context.Routes);
        }

        #endregion
        
        #region AddRoutes

        [Fact]
        public async void AddRoutes_RoutesAdded()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new RouteRepository(context);
            var route = new Route()
            {
                LocationId = _testLocationId,
                Name = "added route",
                RouteId = Guid.NewGuid()
            };

            //act
            repository.AddRoutes(new List<Route>() { route });
            
            //assert
            context.SaveChanges();
            Assert.Equal(3, context.Routes.Count());
        }
        
        #endregion
    }
}