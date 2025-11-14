using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories
{
    public class AssignmentWeaponRepository : IAssignmentWeaponRepository
    {
        private readonly AppDbContext _context;

        public AssignmentWeaponRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AssignmentWeapon>> GetAllAsync()
        {
            return await _context.AssignmentWeapons
                .Include(a => a.Employee)
                .Include(a => a.Weapon)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        public async Task<AssignmentWeapon?> GetByIdAsync(int id)
        {
            return await _context.AssignmentWeapons
                .Include(a => a.Employee)
                .Include(a => a.Weapon)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(AssignmentWeapon entity)
        {
            _context.AssignmentWeapons.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AssignmentWeapon entity)
        {
            var existing = await _context.AssignmentWeapons.FindAsync(entity.Id);
            if (existing != null)
            {
                existing.EmployeeId = entity.EmployeeId;
                existing.WeaponId = entity.WeaponId;
                existing.AssignedAt = entity.AssignedAt;
                existing.ReturnedAt = entity.ReturnedAt;
                existing.AssignedBy = entity.AssignedBy;
                existing.ReturnedBy = entity.ReturnedBy;
                existing.Note = entity.Note;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.AssignmentWeapons.FindAsync(id);
            if (entity != null)
            {
                _context.AssignmentWeapons.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}