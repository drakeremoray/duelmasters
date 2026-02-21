using DuelMastersApi.Data;
using DuelMastersApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public class MatchActionService : IMatchActionService
    {
        private readonly DuelMastersContext _db;
        public MatchActionService(DuelMastersContext db) { _db = db; }

        public async Task<MatchAction> CreateAsync(MatchAction action)
        {
            _db.MatchActions.Add(action);
            await _db.SaveChangesAsync();
            return action;
        }

        public async Task<List<MatchAction>> GetByMatchIdAsync(int matchId)
        {
            return await _db.MatchActions
                .AsNoTracking()
                .Where(a => a.MatchId == matchId)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}
