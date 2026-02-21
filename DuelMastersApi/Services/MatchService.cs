using DuelMastersApi.Data;
using DuelMastersApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DuelMastersApi.Models;

namespace DuelMastersApi.Services
{
    public class MatchService : IMatchService
    {
        private readonly DuelMastersContext _db;
        private readonly IGameStateService _gameState;
        private readonly IMatchActionService _actionService;

        public MatchService(DuelMastersContext db, IGameStateService gameState, IMatchActionService actionService)
        {
            _db = db;
            _gameState = gameState;
            _actionService = actionService;
        }

        public async Task<List<Match>> GetAllAsync() => await _db.Matches.ToListAsync();

        public async Task<Match?> GetByIdAsync(int id) => await _db.Matches.FindAsync(id);

        public async Task<Match> CreateAsync(Match match)
        {
            _db.Matches.Add(match);
            await _db.SaveChangesAsync();
            return match;
        }

        public async Task UpdateAsync(Match match)
        {
            _db.Entry(match).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var m = await _db.Matches.FindAsync(id);
            if (m is null) return;
            _db.Matches.Remove(m);
            await _db.SaveChangesAsync();
        }

        public async Task<MatchResultDto> ApplyActionAsync(int matchId, MatchActionDto action, int playerId)
        {
            var latest = await _gameState.GetLatestByMatchIdAsync(matchId);
            var currentState = latest?.State ?? "{}";

            // persist action log for replay
            var actionLog = new DuelMastersApi.Data.Models.MatchAction
            {
                MatchId = matchId,
                PlayerId = playerId,
                ActionType = action.ActionType,
                Payload = System.Text.Json.JsonSerializer.Serialize(action.Payload)
            };
            await _actionService.CreateAsync(actionLog);

            var result = MatchEngine.ApplyAction(currentState, action, playerId);

            var gs = new GameState
            {
                MatchId = matchId,
                Turn = 0,
                State = result.NewState
            };

            await _gameState.CreateAsync(gs);

            return result;
        }
    }
}
