using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;

namespace MyApp.Infrastructure.Data
{
    public class RankRepository : IRankRepository
    {
        private readonly AppDbContext _context;

        public RankRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Rank?> GetByIdAsync(int id) =>
            await _context.Ranks.FindAsync(id);

        public async Task<List<Rank>> GetAllAsync() =>
            await _context.Ranks.ToListAsync();

        public async Task AddAsync(Rank Rank)
        {
            await _context.Ranks.AddAsync(Rank);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rank Rank)
        {
            var existing = await _context.Ranks.FindAsync(Rank.Id);
            if (existing != null)
            {
                existing.Name = Rank.Name;
                existing.Details = Rank.Details;
                existing.IsActive = Rank.IsActive;
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteAsync(int id)
        {
            var Rank = await _context.Ranks.FindAsync(id);
            if (Rank != null)
            {
                _context.Ranks.Remove(Rank);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
