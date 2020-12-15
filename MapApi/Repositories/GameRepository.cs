using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Driver;

using TbspRpgLib.Settings;

using MapApi.Entities;

namespace MapApi.Repositories {
    public interface IGameRepository {
        Task<List<Game>> GetAllGames();
        Task<Game> GetGame(string gameId);
        void InsertGameIfItDoesntExist(Game game);
    }

    public class GameRepository : MongoRepository, IGameRepository {
        private IMongoCollection<Game> _games;

        public GameRepository(IDatabaseSettings databaseSettings) : base(databaseSettings) {
            _games = _mongoDatabase.GetCollection<Game>("games");
        }

        public Task<List<Game>> GetAllGames() {
            return _games.Find(game => true).ToListAsync();
        }

        public Task<Game> GetGame(string gameId) {
            return _games.Find(gm => gm.Id == gameId).FirstOrDefaultAsync();
        }

        public void InsertGameIfItDoesntExist(Game game) {
            var options = new ReplaceOptions { IsUpsert = true };
            var result = _games.ReplaceOneAsync<Game>(
                doc =>
                    doc.UserId == game.UserId
                    && doc.AdventureId == game.AdventureId,
                game, options);
        }
    }
}