using System.Collections.Generic;
using System.Linq;
using System;

using Moq;

using Xunit;

using TbspRpgLib.Entities;
using TbspRpgLib.Services;
using MapApi.Services;
using MapApi.Entities;
using MapApi.EventProcessors;

namespace MapApi.Tests.Events {
    public class NewGameProcessorTests {
        private NewGameEventHandler _newGameHandler;

        private List<Game> _games;

        public NewGameProcessorTests() {
            //need a moq game service
            var services = new List<Service>();
            services.Add(new Service() {
                Id = Guid.NewGuid(),
                Name = "adventure",
                Url = "http://adventureapi:8001"
            });

            _games = new List<Game>();
            var mockGameService = new Mock<IGameService>();
            mockGameService.Setup(service =>
                service.AddGame(It.IsAny<Game>())
            ).Callback<Game>((game) => _games.Add(game));

            //need a moq service service
            var mockServiceService = new Mock<IServiceService>();
            mockServiceService.Setup(service =>
                service.GetUrlForService(It.IsAny<string>())
            ).Returns((string name) => services.Find(srv => srv.Name == name).Url);

            _newGameHandler = new NewGameEventHandler(mockGameService.Object, mockServiceService.Object, null, null);
        }

        // [Fact]
        // public async void HandleNewGameEvent_Valid() {
        //     //arrange, new game object
        //     var game = new Game() {
        //         Id = new Guid(),
        //         AdventureId = new Guid(),
        //         UserId = new Guid()
        //     };
        //     _games.Clear();

        //     //act
        //     await _newGameHandler.HandleNewGameEvent(game, 0);

        //     //assert
        //     Assert.Single(_games);
        // }
    }
}