using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MapApi.Entities;
using MapApi.EventProcessors;
using MapApi.Repositories;
using MapApi.Services;
using Moq;
using RestSharp;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Location.Content;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.InterServiceCommunication.RequestModels;
using Xunit;

namespace MapApi.Tests.EventProcessors
{
    public class NewGameEventHandlerTests : InMemoryTest
    {
        #region Setup

        private Guid _testGameId;
        private Guid _testLocationId;
        
        public NewGameEventHandlerTests() : base("NewGameEventHandlerTests")
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

            _testGameId = Guid.NewGuid();
            var game = new Game()
            {
                Id = _testGameId,
                UserId = Guid.NewGuid(),
                AdventureId = Guid.NewGuid()
            };
            
            context.Add(game);
            context.SaveChanges();
        }

        private NewGameEventHandler CreateHandler(MapContext context, ICollection<Event> events)
        {
            var repository = new GameRepository(context);
            var service = new GameService(repository);
            
            var aggregateService = new Mock<IAggregateService>();
            aggregateService.Setup(service =>
                service.AppendToAggregate(It.IsAny<string>(), It.IsAny<Event>(), It.IsAny<ulong>())
            ).Callback<string, Event, ulong>((type, evnt, n) => events.Add(evnt));

            var adventureServiceLink = new Mock<IAdventureServiceLink>();
            adventureServiceLink.Setup(asl =>
                asl.GetInitialLocation(It.IsAny<AdventureRequest>(), It.IsAny<Credentials>())
            ).ReturnsAsync((AdventureRequest adventureRequest, Credentials creds) => new IscResponse()
            {
                Response = new RestResponse()
                {
                    Content = "{\"id\": \"" + _testLocationId + "\"}"
                }
            });

            return new NewGameEventHandler(
                service,
                aggregateService.Object,
                adventureServiceLink.Object);
        }

        #endregion

        #region HandleEvent

        [Fact]
        public async void HandleEvent_Valid_GameAddedEventGenerated()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var events = new List<Event>();
            var handler = CreateHandler(context, events);
            var newGameId = Guid.NewGuid();
            var agg = new GameAggregate()
            {
                Id = newGameId.ToString(),
                AdventureId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                GlobalPosition = 10
            };
            
            //act
            await handler.HandleEvent(agg, null);
            
            //assert
            context.SaveChanges();
            Assert.Equal(2, context.Games.Count());
            Assert.Single(events);
            var gameEvent = events.First();
            Assert.Equal("location_enter", gameEvent.Type);
            var enterLocation = JsonSerializer.Deserialize<LocationEnter>(gameEvent.GetDataJson());
            Assert.Equal(_testLocationId.ToString(), enterLocation.Destination);
            Assert.Equal(gameEvent.GetDataId(), enterLocation.Id);
        }

        [Fact]
        public async void HandleEvent_GameExists_GameNotAddedEventGenerated()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var events = new List<Event>();
            var handler = CreateHandler(context, events);
            var agg = new GameAggregate()
            {
                Id = _testGameId.ToString(),
                AdventureId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                GlobalPosition = 10
            };
            
            //act
            await handler.HandleEvent(agg, null);
            
            //assert
            context.SaveChanges();
            Assert.Single(context.Games);
            Assert.Single(events);
            var gameEvent = events.First();
            Assert.Equal("location_enter", gameEvent.Type);
            var enterLocation = JsonSerializer.Deserialize<LocationEnter>(gameEvent.GetDataJson());
            Assert.Equal(_testLocationId.ToString(), enterLocation.Destination);
            Assert.Equal(gameEvent.GetDataId(), enterLocation.Id);
        }

        [Fact]
        public async void HandleEvent_SameDestination_EventNotGenerated()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var events = new List<Event>();
            var handler = CreateHandler(context, events);
            var agg = new GameAggregate()
            {
                Id = _testGameId.ToString(),
                AdventureId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Destination = _testLocationId.ToString(),
                GlobalPosition = 10
            };
            
            //act
            await handler.HandleEvent(agg, null);
            
            //assert
            context.SaveChanges();
            Assert.Single(context.Games);
            Assert.Empty(events);
        }

        #endregion
    }
}