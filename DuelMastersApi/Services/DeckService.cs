using DuelMastersApi.Data;
using DuelMastersApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public class DeckService : IDeckService
    {
        private readonly DuelMastersContext _db;
        public DeckService(DuelMastersContext db) { _db = db; }

        public async Task<List<Deck>> GetAllAsync() => await _db.Decks.Include(d => d.Player).ToListAsync();

        public async Task<Deck?> GetByIdAsync(int id) => await _db.Decks.Include(d => d.Player).FirstOrDefaultAsync(d => d.Id == id);

        public async Task<Deck> CreateAsync(Deck deck)
        {
            _db.Decks.Add(deck);
            await _db.SaveChangesAsync();
            return deck;
        }

        public async Task UpdateAsync(Deck deck)
        {
            _db.Entry(deck).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deck = await _db.Decks.FindAsync(id);
            if (deck is null) return;
            _db.Decks.Remove(deck);
            await _db.SaveChangesAsync();
        }
    }
}
