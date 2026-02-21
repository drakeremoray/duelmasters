using DuelMastersApi.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public interface IDeckService
    {
        Task<List<Deck>> GetAllAsync();
        Task<Deck?> GetByIdAsync(int id);
        Task<Deck> CreateAsync(Deck deck);
        Task UpdateAsync(Deck deck);
        Task DeleteAsync(int id);
    }
}
