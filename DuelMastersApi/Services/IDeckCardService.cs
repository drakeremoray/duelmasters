using DuelMastersApi.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public interface IDeckCardService
    {
        Task<List<DeckCard>> GetAllAsync();
        Task<DeckCard?> GetByIdAsync(int id);
        Task<DeckCard> CreateAsync(DeckCard dc);
        Task DeleteAsync(int id);
    }
}
