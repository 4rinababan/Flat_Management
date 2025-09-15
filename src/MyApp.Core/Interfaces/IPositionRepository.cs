
using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces;

public interface IPositionRepository
{
    Task<List<Position>> GetAllAsync();
    Task<Position?> GetByIdAsync(int id);
    Task AddAsync(Position entity);
    Task UpdateAsync(Position entity);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
