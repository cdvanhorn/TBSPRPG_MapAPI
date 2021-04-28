using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using MapApi.Entities;
using MapApi.Repositories;

namespace MapApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAllGames();
        Task<Game> GetGame(Guid gameId);
        Task AddGame(Game game);
    }

    public class GameService : IGameService {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository) {
            _gameRepository = gameRepository;
        }

        public Task<List<Game>> GetAllGames() {
            return _gameRepository.GetAllGames();
        }

        public Task<Game> GetGame(Guid gameId) {
            return _gameRepository.GetGame(gameId);
        }

        public async Task AddGame(Game game) {
            //try and get the game
            var dbGame = await GetGame(game.Id);
            if(dbGame == null) {
                _gameRepository.AddGame(game);
            }
        }
    }
}