
using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces;

public interface IAreaRepository
{
    Task<List<Area>> GetAllAsync();
    Task<Area?> GetByIdAsync(int id);
    Task AddAsync(Area entity);
    Task UpdateAsync(Area entity);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
