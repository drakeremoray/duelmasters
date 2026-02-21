using DuelMastersApi.Data;
using DuelMastersApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuelMastersApi.Services
{
    public class DeckCardService : IDeckCardService
    {
        private readonly DuelMastersContext _db;
        public DeckCardService(DuelMastersContext db) { _db = db; }

        public async Task<List<DeckCard>> GetAllAsync() => await _db.DeckCards.Include(dc => dc.Card).Include(dc => dc.Deck).ToListAsync();

        public async Task<DeckCard?> GetByIdAsync(int id) => await _db.DeckCards.Include(dc => dc.Card).Include(dc => dc.Deck).FirstOrDefaultAsync(dc => dc.Id == id);

        public async Task<DeckCard> CreateAsync(DeckCard dc)
        {
            _db.DeckCards.Add(dc);
            await _db.SaveChangesAsync();
            return dc;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.DeckCards.FindAsync(id);
            if (item is null) return;
            _db.DeckCards.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
