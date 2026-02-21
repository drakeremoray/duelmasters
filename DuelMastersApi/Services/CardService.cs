using DuelMastersApi.Data;
using DuelMastersApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public class CardService : ICardService
    {
        private readonly DuelMastersContext _db;
        public CardService(DuelMastersContext db) { _db = db; }

        public async Task<List<Card>> GetAllAsync() => await _db.Cards.ToListAsync();

        public async Task<Card?> GetByIdAsync(int id) => await _db.Cards.FindAsync(id);

        public async Task<Card> CreateAsync(Card card)
        {
            _db.Cards.Add(card);
            await _db.SaveChangesAsync();
            return card;
        }

        public async Task UpdateAsync(Card card)
        {
            _db.Entry(card).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _db.Cards.FindAsync(id);
            if (c is null) return;
            _db.Cards.Remove(c);
            await _db.SaveChangesAsync();
        }
    }
}
