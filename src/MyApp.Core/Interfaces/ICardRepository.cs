
using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces;

public interface ICardRepository
{
    Task<List<Card>> GetAllAsync();
    Task<Card?> GetByIdAsync(int id);
    Task AddAsync(Card entity);
    Task UpdateAsync(Card entity);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
