using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using MapApi.Entities;
using MapApi.Repositories;

namespace MapApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAllGames();
        Task<Game> GetGame(string gameId);
        Task<Game> GetGame(Guid gameId);
        void InsertGameIfItDoesntExist(Game game);
        Task AddGame(Game game);
    }

    public class GameService : IGameService {
        private IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository) {
            _gameRepository = gameRepository;
        }

        public Task<List<Game>> GetAllGames() {
            return _gameRepository.GetAllGames();
        }

        public Task<Game> GetGame(string gameId) {
            Guid guid;
            if(!Guid.TryParse(gameId, out guid))
                return null;
            return GetGame(guid);
        }

        public Task<Game> GetGame(Guid gameId) {
            return _gameRepository.GetGame(gameId);
        }

        public void InsertGameIfItDoesntExist(Game game) {
            _gameRepository.InsertGameIfItDoesntExist(game);
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