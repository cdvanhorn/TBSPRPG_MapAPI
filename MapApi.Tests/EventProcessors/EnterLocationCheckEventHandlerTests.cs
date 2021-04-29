using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MapApi.Entities;
using MapApi.EventProcessors;
using MapApi.Repositories;
using MapApi.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Location;
using TbspRpgLib.Events.Location.Content;
using Xunit;

namespace MapApi.Tests.EventProcessors
{
    public class EnterLocationCheckEventHandlerTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testLocationId;
        private readonly Guid _testGameId;
        
        public EnterLocationCheckEventHandlerTests() : base("EnterLocationCheckEventHandlerTests")
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

        private static EnterLocationCheckEventHandler CreateHandler(MapContext context, ICollection<Event> events)
        {
            var repository = new LocationRepository(context);
            var service = new LocationService(repository);
            
            var aggregateService = new Mock<IAggregateService>();
            aggregateService.Setup(service =>
                service.AppendToAggregate(It.IsAny<string>(), It.IsAny<Event>(), It.IsAny<ulong>())
            ).Callback<string, Event, ulong>((type, evnt, n) =>
            {
                if (n <= 100)
                    events.Add(evnt);
            });

            return new EnterLocationCheckEventHandler(aggregateService.Object, service);
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
                Destination = addedLocationId.ToString(),
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
            Assert.Empty(dict["Destination"]);
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
                Destination = addedLocationId.ToString(),
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
            Assert.Empty(dict["Destination"]);
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
                Destination = addedLocationId.ToString(),
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
            Assert.Empty(dict["Destination"]);
            var location = context.Locations.FirstOrDefault(loc => loc.GameId == _testGameId);
            Assert.NotNull(location);
            Assert.Equal(addedLocationId, location.Id);
        }

        #endregion
    }
}