using System;
using System.Linq;
using MapApi.Entities;
using MapApi.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace MapApi.Tests.Repositories
{
    public class LocationRepositoryTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testGameId;
        private readonly Guid _testLocationId;

        public LocationRepositoryTests() : base("LocationRepositoryTests")
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
                Id = _testLocationId,
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

        #endregion

        #region GetAllLocations

        [Fact]
        public async void GetAllLocations_ReturnsAll()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var respository = new LocationRepository(context);
            
            //act
            var locations = await respository.GetAllLocations();
            
            //assert
            Assert.Equal(1, locations.Count);
            Assert.Equal(_testLocationId, locations.First().Id);
            Assert.Equal(_testGameId, locations.First().GameId);
        }

        #endregion

        #region GetLocationForGame

        [Fact]
        public async void GetLocationForGame_Valid_ReturnOne()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new LocationRepository(context);
            
            //act
            var location = await repository.GetLocationForGame(_testGameId);
            
            //assert
            Assert.NotNull(location);
            Assert.Equal(_testLocationId, location.Id);
        }

        [Fact]
        public async void GetLocationForGame_Invalid_ReturnNone()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new LocationRepository(context);
            
            //act
            var location = await repository.GetLocationForGame(Guid.NewGuid());
            
            //assert
            Assert.Null(location);
        }

        #endregion

        #region AddLocation

        [Fact]
        public async void AddLocation_AddsLocation()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new LocationRepository(context);
            var addedLocationId = Guid.NewGuid();
            var loc = new Location()
            {
                Id = addedLocationId,
                Game = new Game()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    AdventureId = Guid.NewGuid()
                }
            };
            
            //act
            repository.AddLocation(loc);

            //assert
            context.SaveChanges();
            Assert.Equal(2, context.Locations.Count());
            Assert.NotNull(context.Locations.FirstOrDefault(l => l.Id == addedLocationId));
        }

        #endregion

        #region GetLocation

        [Fact]
        public async void GetLocation_Valid_ReturnOne()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new LocationRepository(context);
            
            //act
            var location = await repository.GetLocation(_testLocationId);
            
            //assert
            Assert.NotNull(location);
            Assert.Equal(_testLocationId, location.Id);
            Assert.Equal(_testGameId, location.GameId);
        }
        
        [Fact]
        public async void GetLocation_Invalid_ReturnNone()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new LocationRepository(context);
            
            //act
            var location = await repository.GetLocation(Guid.NewGuid());
            
            //assert
            Assert.Null(location);
        }

        #endregion
    }
}