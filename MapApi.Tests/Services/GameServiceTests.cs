using System;
using System.Linq;
using MapApi.Entities;
using MapApi.Repositories;
using MapApi.Services;
using Xunit;

namespace MapApi.Tests.Services
{
    public class GameServiceTests : InMemoryTest
    {
        #region Setup

        private Guid _testGameId;

        public GameServiceTests() : base("GameServiceTests")
        {
            _testGameId = Guid.NewGuid();
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

            var game2 = new Game()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AdventureId = Guid.NewGuid()
            };
            
            context.AddRange(game, game2);
            context.SaveChanges();
        }

        private static GameService CreateService(MapContext context)
        {
            var repo = new GameRepository(context);
            return new GameService(repo);
        }

        #endregion

        #region GetAllGames

        [Fact]
        public async void GetAllGames_ReturnsAll()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var games = await service.GetAllGames();
            
            //assert
            Assert.Equal(2, games.Count);
            Assert.Equal(_testGameId, games.First().Id);
        }

        #endregion

        #region GetGame

        [Fact]
        public async void GetGame_Valid_ReturnOne()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var game = await service.GetGame(_testGameId);
            
            //assert
            Assert.NotNull(game);
            Assert.Equal(_testGameId, game.Id);
        }

        [Fact]
        public async void GetGame_Invalid_ReturnNone()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var game = await service.GetGame(Guid.NewGuid());
            
            //assert
            Assert.Null(game);
        }

        #endregion

        #region AddGame

        [Fact]
        public async void AddGame_NotExist_GameAdded()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            var addedGameId = Guid.NewGuid();
            var game = new Game()
            {
                Id = addedGameId,
                UserId = Guid.NewGuid(),
                AdventureId = Guid.NewGuid()
            };
            
            //act
            await service.AddGame(game);
            
            //assert
            context.SaveChanges();
            Assert.Equal(3, context.Games.Count());
            Assert.NotNull(context.Games.FirstOrDefault(g => g.Id == addedGameId));
            Assert.NotNull(context.Games.FirstOrDefault(g => g.Id == _testGameId));
        }

        [Fact]
        public async void AddGame_Exists_NotAdded()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var service = CreateService(context);
            var game = new Game()
            {
                Id = _testGameId,
                UserId = Guid.NewGuid(),
                AdventureId = Guid.NewGuid()
            };
            
            //act
            await service.AddGame(game);
            
            //assert
            context.SaveChanges();
            Assert.Equal(2, context.Games.Count());
            Assert.NotNull(context.Games.FirstOrDefault(g => g.Id == _testGameId));
        }

        #endregion
    }
}