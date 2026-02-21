using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DuelMastersApi.Data;
using DuelMastersApi.Data.Models;

namespace DuelMastersApi.Services
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly DuelMastersContext _db;
        private readonly IMatchService _matchService;
        private readonly IGameStateService _gameStateService;

        public MatchmakingService(DuelMastersContext db, IMatchService matchService, IGameStateService gameStateService)
        {
            _db = db;
            _matchService = matchService;
            _gameStateService = gameStateService;
        }

        // Enqueue player in DB; if another pending player exists, pair them in a serializable transaction
        public async Task<int?> JoinQueueAsync(int playerId)
        {
            var entry = new MatchmakingEntry { PlayerId = playerId };
            _db.Add(entry);
            await _db.SaveChangesAsync();

            // Attempt to pair using a serializable transaction to avoid races
            using var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

                var other = await _db.MatchmakingQueue
                    .Where(e => e.Status == "Pending" && e.PlayerId != playerId)
                    .OrderBy(e => e.CreatedAt)
                    .FirstOrDefaultAsync();

                if (other != null)
                {
                    // mark both entries matched
                    other.Status = "Matched";
                    entry.Status = "Matched";
                    await _db.SaveChangesAsync();

                    // create match
                    var match = new Match();
                    match = await _matchService.CreateAsync(match);

                    // initial state: players array
                    var playersArr = new JsonArray();
                    playersArr.Add(new JsonObject { ["id"] = other.PlayerId, ["hand"] = new JsonArray(), ["deck"] = new JsonArray(), ["battlefield"] = new JsonArray(), ["resources"] = 3 });
                    playersArr.Add(new JsonObject { ["id"] = playerId, ["hand"] = new JsonArray(), ["deck"] = new JsonArray(), ["battlefield"] = new JsonArray(), ["resources"] = 3 });

                    var gs = new GameState { MatchId = match.Id, State = new JsonObject { ["players"] = playersArr }.ToJsonString() };
                    await _gameStateService.CreateAsync(gs);

                    // attach match id to entries
                    other.MatchId = match.Id;
                    entry.MatchId = match.Id;
                    await _db.SaveChangesAsync();

                    await tx.CommitAsync();
                    return match.Id;
                }

            await tx.CommitAsync();
            return null;
        }
    }
}
