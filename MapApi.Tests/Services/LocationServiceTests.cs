using System;
using System.Linq;
using MapApi.Entities;
using MapApi.Repositories;
using MapApi.Services;
using Xunit;

namespace MapApi.Tests.Services
{
    public class LocationServiceTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testGameId;
        private readonly Guid _testLocationId;
        
        public LocationServiceTests() : base("LocationServiceTests")
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

        private static LocationService CreateService(MapContext context)
        {
            var repository = new LocationRepository(context);
            return new LocationService(repository);
        }
        
        #endregion

        #region GetAllLocations

        [Fact]
        public async void GetAllLocations_ReturnsAll()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var locations = await service.GetAllLocations();
            
            //assert
            Assert.Single(locations);
            Assert.NotNull(locations.Where(lvm => lvm.Id == _testLocationId));
        }

        #endregion

        #region GetLocation

        [Fact]
        public async void GetLocation_Valid_ReturnOne()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var location = await service.GetLocation(_testLocationId);
            
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
            var service = CreateService(context);
            
            //act
            var location = await service.GetLocation(Guid.NewGuid());
            
            //assert
            Assert.Null(location);
        }

        #endregion

        #region GetLocationForGame

        [Fact]
        public async void GetLocationForGame_Valid_ReturnOne()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var location = await service.GetLocationForGame(_testGameId);
            
            //assert
            Assert.NotNull(location);
            Assert.Equal(_testLocationId, location.Id);
        }

        [Fact]
        public async void GetLocationForGame_Invalid_ReturnNone()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var location = await service.GetLocationForGame(Guid.NewGuid());
            
            //assert
            Assert.Null(location);
        }

        #endregion

        #region AddLocation

        [Fact]
        public async void AddLocation_NotExists_Added()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
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
            await service.AddLocation(loc);
            
            //assert
            context.SaveChanges();
            Assert.Equal(2, context.Locations.Count());
            Assert.NotNull(context.Locations.FirstOrDefault(loc => loc.Id == addedLocationId));
            Assert.NotNull(context.Locations.FirstOrDefault(loc => loc.Id == _testLocationId));
        }

        [Fact]
        public async void AddLocation_Exists_NotAdded()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            var loc = new Location()
            {
                Id = _testLocationId,
                Game = new Game()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    AdventureId = Guid.NewGuid()
                }
            };
            
            //act
            await service.AddLocation(loc);
            
            //assert
            context.SaveChanges();
            Assert.Equal(1, context.Locations.Count());
            Assert.NotNull(context.Locations.FirstOrDefault(loc => loc.Id == _testLocationId));
        }

        #endregion
    }
}