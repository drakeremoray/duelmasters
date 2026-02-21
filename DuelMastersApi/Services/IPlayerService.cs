using DuelMastersApi.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public interface IPlayerService
    {
        Task<List<Player>> GetAllAsync();
        Task<Player?> GetByIdAsync(int id);
        Task<Player> CreateAsync(Player player);
        Task<Player?> GetByUsernameAsync(string username);
    }
}
