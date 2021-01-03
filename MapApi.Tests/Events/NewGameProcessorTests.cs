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
        private NewGameHandler _newGameHandler;

        private List<Game> _games;

        public NewGameProcessorTests() {
            //need a moq game service
            var services = new List<Service>();
            services.Add(new Service() {
                Id = "1",
                Name = "adventure",
                Url = "http://adventureapi:8001",
                EventPrefix = "1"
            });

            _games = new List<Game>();
            var mockGameService = new Mock<IGameService>();
            mockGameService.Setup(service =>
                service.InsertGameIfItDoesntExist(It.IsAny<Game>())
            ).Callback<Game>((game) => _games.Add(game));

            //need a moq service service
            var mockServiceService = new Mock<IServiceService>();
            mockServiceService.Setup(service =>
                service.GetUrlForService(It.IsAny<string>())
            ).ReturnsAsync((string name) => services.Find(srv => srv.Name == name).Url);

            _newGameHandler = new NewGameHandler(mockGameService.Object, mockServiceService.Object, null);
        }

        [Fact]
        public void HandleNewGameEvent_Valid() {
            //arrange, new game object
            var game = new Game() {
                Id = "1",
                AdventureId = "adv",
                UserId = "usr"
            };
            _games.Clear();

            //act
            _newGameHandler.HandleNewGameEvent(game);

            //assert
            Assert.Single(_games);
        }

        [Fact]
        public void HandleNewGameEvent_NullUserId_Invalid() {
            //arrange, new game object
            var game = new Game() {
                Id = "1",
                AdventureId = "adv",
                UserId = null
            };
            _games.Clear();

            //act
            _newGameHandler.HandleNewGameEvent(game);

            //assert
            Assert.Empty(_games);
        }

        [Fact]
        public void HandleNewGameEvent_NullAdventureId_Invalid() {
            //arrange, new game object
            var game = new Game() {
                Id = "1",
                AdventureId = null,
                UserId = "usr"
            };
            _games.Clear();

            //act
            _newGameHandler.HandleNewGameEvent(game);

            //assert
            Assert.Empty(_games);
        }

        [Fact]
        public void HandleNewGameEvent_AdventureAndUserSameId_Invalid() {
            //arrange, new game object
            var game = new Game() {
                Id = "1",
                AdventureId = "usr",
                UserId = "usr"
            };
            _games.Clear();

            //act
            _newGameHandler.HandleNewGameEvent(game);

            //assert
            Assert.Empty(_games);
        }
    }
}