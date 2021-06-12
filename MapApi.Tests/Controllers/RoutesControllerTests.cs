using System;
using System.Collections.Generic;
using MapApi.Controllers;
using MapApi.Entities;
using MapApi.Repositories;
using MapApi.Services;
using MapApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MapApi.Tests.Controllers
{
    public class RoutesControllerTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testGameId = Guid.NewGuid();
        private readonly Guid _testLocationId = Guid.NewGuid();

        public RoutesControllerTests() : base("RouteRepositoryTests")
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
        
        private static RoutesController CreateController(MapContext context)
        {
            var repo = new RouteRepository(context);
            var service = new RouteService(repo);
            return new RoutesController(service);
        }

        #endregion
        
        #region GetByGameId

        [Fact]
        public async void GetByGameId_Valid_ReturnOne()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.GetByGameId(_testGameId);
            
            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var routes = okObjectResult.Value as IEnumerable<RouteViewModel>;
            Assert.NotNull(routes);
            Assert.Single(routes);
        }

        [Fact]
        public async void GetByGameId_Invalid_ReturnNone()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.GetByGameId(Guid.NewGuid());
            
            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var routes = okObjectResult.Value as IEnumerable<RouteViewModel>;
            Assert.Empty(routes);
        }

        #endregion
    }
}