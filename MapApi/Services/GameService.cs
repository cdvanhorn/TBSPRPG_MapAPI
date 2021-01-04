using System.Collections.Generic;
using System.Threading.Tasks;

using MapApi.Entities;
using MapApi.Repositories;

namespace MapApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAllGames();
        ValueTask<Game> GetGame(int gameId);
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

        public ValueTask<Game> GetGame(int gameId) {
            return _gameRepository.GetGame(gameId);
        }

        public void InsertGameIfItDoesntExist(Game game) {
            _gameRepository.InsertGameIfItDoesntExist(game);
        }
    }
}