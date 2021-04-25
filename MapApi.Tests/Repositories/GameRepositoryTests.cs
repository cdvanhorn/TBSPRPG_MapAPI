using System;
using System.Linq;
using MapApi.Entities;
using MapApi.Repositories;
using Xunit;

namespace MapApi.Tests.Repositories
{
    public class GameRepositoryTests : InMemoryTest
    {
        #region Setup
        
        private Guid _testGameId;
        
        public GameRepositoryTests() : base("GameRepositoryTests")
        {
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
        
        #endregion
        
        #region GetAllGames

        [Fact]
        public async void GetAllGames_ReturnsAll()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new GameRepository(context);
            
            //act
            var games = await repository.GetAllGames();
            
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
            var repository = new GameRepository(context);
            
            //act
            var game = await repository.GetGame(_testGameId);
            
            //assert
            Assert.NotNull(game);
            Assert.Equal(_testGameId, game.Id);
        }

        [Fact]
        public async void GetGame_Invalid_ReturnNone()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new GameRepository(context);
            
            //act
            var game = await repository.GetGame(Guid.NewGuid());
            
            //assert
            Assert.Null(game);
        }
        
        #endregion

        #region AddGame

        [Fact]
        public async void AddGame_Valid_GameAdded()
        {
            //arrange
            await using var context = new MapContext(_dbContextOptions);
            var repository = new GameRepository(context);
            var addedGameId = Guid.NewGuid();
            var game = new Game()
            {
                Id = addedGameId,
                UserId = Guid.NewGuid(),
                AdventureId = Guid.NewGuid()
            };

            //act
            repository.AddGame(game);

            //assert
            context.SaveChanges();
            Assert.Equal(3, context.Games.Count());
            Assert.NotNull(context.Games.FirstOrDefault(g => g.Id == addedGameId));
        }
        
        #endregion
    }
}