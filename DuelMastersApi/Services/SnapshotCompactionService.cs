using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DuelMastersApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DuelMastersApi.Services
{
    // Background service that keeps only recent snapshots per match to limit storage.
    public class SnapshotCompactionService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly int _keep = 50;

        public SnapshotCompactionService(IServiceProvider sp)
        {
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<DuelMastersContext>();

                    var matches = await db.Matches.Select(m => m.Id).ToListAsync(stoppingToken);
                    foreach (var mid in matches)
                    {
                        var states = await db.GameStates.Where(g => g.MatchId == mid).OrderByDescending(g => g.CreatedAt).ToListAsync(stoppingToken);
                        if (states.Count > _keep)
                        {
                            var toRemove = states.Skip(_keep).ToList();
                            db.GameStates.RemoveRange(toRemove);
                        }
                    }
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch
                {
                    // ignore and continue
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
