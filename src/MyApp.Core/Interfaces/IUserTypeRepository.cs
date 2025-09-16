
using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces;

public interface IUserTypeRepository
{
    Task<List<UserType>> GetAllAsync();
    Task<UserType?> GetByIdAsync(int id);
    Task AddAsync(UserType entity);
    Task UpdateAsync(UserType entity);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
