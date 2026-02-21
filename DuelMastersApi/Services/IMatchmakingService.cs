using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public interface IMatchmakingService
    {
        Task<int?> JoinQueueAsync(int playerId);
    }
}
