using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;

namespace MyApp.Infrastructure.Data
{
    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly AppDbContext _context;

        public UserTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserType?> GetByIdAsync(int id) =>
            await _context.UserTypes.FindAsync(id);

        public async Task<List<UserType>> GetAllAsync() =>
            await _context.UserTypes.ToListAsync();

        public async Task AddAsync(UserType UserType)
        {
            await _context.UserTypes.AddAsync(UserType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserType UserType)
        {
            var existing = await _context.UserTypes.FindAsync(UserType.Id);
            if (existing != null)
            {
                existing.TypeName = UserType.TypeName;
                existing.Details = UserType.Details;
                existing.IsActive = UserType.IsActive;
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteAsync(int id)
        {
            var UserType = await _context.UserTypes.FindAsync(id);
            if (UserType != null)
            {
                _context.UserTypes.Remove(UserType);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
