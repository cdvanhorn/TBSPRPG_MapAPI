using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using Microsoft.EntityFrameworkCore;

using MapApi.Entities;

namespace MapApi.Repositories {
    public interface IGameRepository {
        Task<List<Game>> GetAllGames();
        Task<Game> GetGame(Guid gameId);
        void InsertGameIfItDoesntExist(Game game);
    }

    public class GameRepository : IGameRepository {
        private MapContext _context;

        public GameRepository(MapContext context) {
            _context = context;
        }

        public Task<List<Game>> GetAllGames() {
            return _context.Games.AsQueryable().ToListAsync();
        }

        public Task<Game> GetGame(Guid gameId) {
            return _context.Games.AsQueryable().Where(g => g.Id == gameId).FirstOrDefaultAsync();
        }

        public async void InsertGameIfItDoesntExist(Game game) {
            Game dbGame = await GetGame(game.Id);
            if(dbGame == null) {
                //we need to do an insert
                _context.Games.Add(game);
                await _context.SaveChangesAsync();
            }
        }
    }
}