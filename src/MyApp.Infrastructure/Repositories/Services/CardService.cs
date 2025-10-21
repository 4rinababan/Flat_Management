using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;

namespace MyApp.Infrastructure.Data
{
    public class CardRepository : ICardRepository
    {
        private readonly AppDbContext _context;

        public CardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Card?> GetByIdAsync(int id) =>
            await _context.Cards.FindAsync(id);

        public async Task<List<Card>> GetAllAsync() =>
            await _context.Cards.ToListAsync();

        public async Task AddAsync(Card Card)
        {
            await _context.Cards.AddAsync(Card);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Card Card)
        {
            var existing = await _context.Cards.FindAsync(Card.Id);
            if (existing != null)
            {
                existing.CardNo = Card.CardNo;
                existing.User = Card.User;
                existing.Type = Card.Type;
                existing.CreatedBy = Card.CreatedBy;
                existing.CreatedAt = Card.CreatedAt;
                existing.UpdatedAt = Card.UpdatedAt;
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteAsync(int id)
        {
            var Card = await _context.Cards.FindAsync(id);
            if (Card != null)
            {
                _context.Cards.Remove(Card);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
