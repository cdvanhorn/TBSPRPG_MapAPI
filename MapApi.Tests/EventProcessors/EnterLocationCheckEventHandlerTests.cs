using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MapApi.Entities;
using MapApi.EventProcessors;
using MapApi.Repositories;
using MapApi.Services;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using Xunit;

namespace MapApi.Tests.EventProcessors
{
    public class EnterLocationCheckEventHandlerTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testLocationId = Guid.NewGuid();
        private readonly Guid _testGameId = Guid.NewGuid();
        private readonly Guid _testRouteId = Guid.NewGuid();
        
        public EnterLocationCheckEventHandlerTests() : base("EnterLocationCheckEventHandlerTests")
        {
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

        private EnterLocationCheckEventHandler CreateHandler(MapContext context, ICollection<Event> events)
        {
            var repository = new LocationRepository(context);
            var service = new LocationService(repository);

            return new EnterLocationCheckEventHandler(
                Mocks.MockAggregateService(events),
                service,
                Mocks.MockAdventureServiceLink(_testLocationId, _testRouteId));
        }

        #endregion

        #region HandleEvent

        [Fact]
        public async void HandleEvent_CheckPassed_PassEventGeneratedLocationAdded()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var events = new List<Event>();
            var handler = CreateHandler(context, events);
            var addedGameId = Guid.NewGuid();
            var addedLocationId = Guid.NewGuid();
            var agg = new GameAggregate()
            {
                Id = addedGameId.ToString(),
                UserId = Guid.NewGuid().ToString(),
                AdventureId = Guid.NewGuid().ToString(),
                MapData = new MapData()
                {
                    DestinationLocation = addedLocationId.ToString()
                },
                Checks = new GameAggregateChecks()
                {
                    Location = true
                }
            };
            
            //act
            await handler.HandleEvent(agg, null);
            
            //assert
            context.SaveChanges();
            Assert.Single(events);
            Assert.Equal(2, context.Locations.Count());
            var levent = events.First();
            Assert.Equal(Event.LOCATION_ENTER_PASS_EVENT_TYPE, levent.Type);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(levent.GetDataJson());
            Assert.Equal(addedLocationId.ToString(), dict["CurrentLocation"]);
            Assert.Empty(dict["DestinationLocation"]);
        }

        [Fact]
        public async void HandleEvent_CheckFailed_FailEventGeneratedNoLocation()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var events = new List<Event>();
            var handler = CreateHandler(context, events);
            var addedGameId = Guid.NewGuid();
            var addedLocationId = Guid.NewGuid();
            var agg = new GameAggregate()
            {
                Id = addedGameId.ToString(),
                UserId = Guid.NewGuid().ToString(),
                AdventureId = Guid.NewGuid().ToString(),
                MapData = new MapData()
                {
                    DestinationLocation = addedLocationId.ToString()
                },
                Checks = new GameAggregateChecks()
                {
                    Location = false
                }
            };
            
            //act
            await handler.HandleEvent(agg, null);
            
            //assert
            context.SaveChanges();
            Assert.Single(events);
            Assert.Single(context.Locations);
            var levent = events.First();
            Assert.Equal(Event.LOCATION_ENTER_FAIL_EVENT_TYPE, levent.Type);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(levent.GetDataJson());
            Assert.Empty(dict["DestinationLocation"]);
        }
        
        [Fact]
        public async void HandleEvent_CheckPassedExistingLocation_LocationUpdated()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var events = new List<Event>();
            var handler = CreateHandler(context, events);
            var addedLocationId = Guid.NewGuid();
            var agg = new GameAggregate()
            {
                Id = _testGameId.ToString(),
                UserId = Guid.NewGuid().ToString(),
                AdventureId = Guid.NewGuid().ToString(),
                MapData = new MapData()
                {
                    DestinationLocation = addedLocationId.ToString()
                },
                Checks = new GameAggregateChecks()
                {
                    Location = true
                }
            };
            
            //act
            await handler.HandleEvent(agg, null);
            
            //assert
            context.SaveChanges();
            Assert.Single(events);
            Assert.Single(context.Locations);
            var levent = events.First();
            Assert.Equal(Event.LOCATION_ENTER_PASS_EVENT_TYPE, levent.Type);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(levent.GetDataJson());
            Assert.Equal(addedLocationId.ToString(), dict["CurrentLocation"]);
            Assert.Empty(dict["DestinationLocation"]);
            var location = context.Locations.FirstOrDefault(loc => loc.GameId == _testGameId);
            Assert.NotNull(location);
            Assert.Equal(addedLocationId, location.LocationId);
        }

        #endregion
    }
}