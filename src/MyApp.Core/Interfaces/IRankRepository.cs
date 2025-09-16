
using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces;

public interface IRankRepository
{
    Task<List<Rank>> GetAllAsync();
    Task<Rank?> GetByIdAsync(int id);
    Task AddAsync(Rank entity);
    Task UpdateAsync(Rank entity);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
