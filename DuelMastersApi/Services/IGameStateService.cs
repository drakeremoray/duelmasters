using DuelMastersApi.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public interface IGameStateService
    {
        Task<List<GameState>> GetAllAsync();
        Task<GameState?> GetByIdAsync(int id);
        Task<GameState> CreateAsync(GameState gs);
        Task<GameState?> GetLatestByMatchIdAsync(int matchId);
    }
}
