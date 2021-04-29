using System;
using System.Collections.Generic;
using System.Linq;
using MapApi.Controllers;
using MapApi.Entities;
using MapApi.Repositories;
using MapApi.Services;
using MapApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MapApi.Tests.Controllers
{
    public class LocationControllerTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testGameId;
        private readonly Guid _testLocationId;
        
        public LocationControllerTests() : base("LocationControllerTests")
        {
            _testGameId = Guid.NewGuid();
            _testLocationId = Guid.NewGuid();
            Seed();
        }

        private void Seed()
        {
            using var context = new MapContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var loc = new Location()
            {
                Id = Guid.NewGuid(),
                LocationId = _testLocationId,
                Game = new Game()
                {
                    Id = _testGameId,
                    UserId = Guid.NewGuid(),
                    AdventureId = Guid.NewGuid()
                }
            };

            context.Add(loc);
            context.SaveChanges();
        }

        private static LocationsController CreateController(MapContext context)
        {
            var repo = new LocationRepository(context);
            var service = new LocationService(repo);
            return new LocationsController(service);
        }

        #endregion

        #region GetAll

        [Fact]
        public async void GetAll_ReturnAll()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.GetAll();
            
            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var locations = okObjectResult.Value as IEnumerable<LocationViewModel>;
            Assert.NotNull(locations);
            Assert.Single(locations);
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
            var location = okObjectResult.Value as LocationViewModel;
            Assert.NotNull(location);
            Assert.Equal(_testLocationId, location.Id);
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
            var location = okObjectResult.Value as LocationViewModel;
            Assert.Null(location);
        }

        #endregion
    }
}