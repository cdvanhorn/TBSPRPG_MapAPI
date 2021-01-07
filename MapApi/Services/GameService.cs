using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using MapApi.Entities;
using MapApi.Repositories;

namespace MapApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAllGames();
        Task<Game> GetGame(string gameId);
        void InsertGameIfItDoesntExist(Game game);
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
            return _gameRepository.GetGame(guid);
        }

        public void InsertGameIfItDoesntExist(Game game) {
            _gameRepository.InsertGameIfItDoesntExist(game);
        }
    }
}