using DuelMastersApi.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public interface IMatchService
    {
        Task<List<Match>> GetAllAsync();
        Task<Match?> GetByIdAsync(int id);
        Task<Match> CreateAsync(Match match);
        Task UpdateAsync(Match match);
        Task DeleteAsync(int id);
        Task<DuelMastersApi.Models.MatchResultDto> ApplyActionAsync(int matchId, DuelMastersApi.Models.MatchActionDto action, int playerId);
    }
}
