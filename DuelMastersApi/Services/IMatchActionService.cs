using DuelMastersApi.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public interface IMatchActionService
    {
        Task<MatchAction> CreateAsync(MatchAction action);
        Task<List<MatchAction>> GetByMatchIdAsync(int matchId);
    }
}
