using DuelMastersApi.Data;
using DuelMastersApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly DuelMastersContext _db;
        public PlayerService(DuelMastersContext db) { _db = db; }

        public async Task<List<Player>> GetAllAsync() => await _db.Players.ToListAsync();

        public async Task<Player?> GetByIdAsync(int id) => await _db.Players.FindAsync(id);

        public async Task<Player?> GetByUsernameAsync(string username) => await _db.Players.SingleOrDefaultAsync(p => p.Username == username);

        public async Task<Player> CreateAsync(Player player)
        {
            _db.Players.Add(player);
            await _db.SaveChangesAsync();
            return player;
        }
    }
}
