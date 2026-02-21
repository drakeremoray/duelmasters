using DuelMastersApi.Data;
using DuelMastersApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public class GameStateService : IGameStateService
    {
        private readonly DuelMastersContext _db;
        public GameStateService(DuelMastersContext db) { _db = db; }

        public async Task<List<GameState>> GetAllAsync() => await _db.GameStates.ToListAsync();

        public async Task<GameState?> GetByIdAsync(int id) => await _db.GameStates.FindAsync(id);

        public async Task<GameState> CreateAsync(GameState gs)
        {
            _db.GameStates.Add(gs);
            await _db.SaveChangesAsync();
            return gs;
        }

        public async Task<GameState?> GetLatestByMatchIdAsync(int matchId)
        {
            return await _db.GameStates
                .Where(g => g.MatchId == matchId)
                .OrderByDescending(g => g.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
