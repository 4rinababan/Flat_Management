using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;

namespace MyApp.Infrastructure.Data
{
    public class AreaRepository : IAreaRepository
    {
        private readonly AppDbContext _context;

        public AreaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Area?> GetByIdAsync(int id) =>
            await _context.Areas.FindAsync(id);

        public async Task<List<Area>> GetAllAsync() =>
            await _context.Areas.ToListAsync();

        public async Task AddAsync(Area Area)
        {
            await _context.Areas.AddAsync(Area);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Area Area)
        {
            var existing = await _context.Areas.FindAsync(Area.Id);
            if (existing != null)
            {
                existing.Name = Area.Name;
                existing.Code = Area.Code;
                existing.Description = Area.Description;
                existing.CreatedBy = Area.CreatedBy;
                existing.CreatedAt = Area.CreatedAt;
                existing.UpdatedAt = Area.UpdatedAt;
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteAsync(int id)
        {
            var Area = await _context.Areas.FindAsync(id);
            if (Area != null)
            {
                _context.Areas.Remove(Area);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
