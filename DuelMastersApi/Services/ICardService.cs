using DuelMastersApi.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public interface ICardService
    {
        Task<List<Card>> GetAllAsync();
        Task<Card?> GetByIdAsync(int id);
        Task<Card> CreateAsync(Card card);
        Task UpdateAsync(Card card);
        Task DeleteAsync(int id);
    }
}
