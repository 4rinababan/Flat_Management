using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<List<Employee>> GetAllAsync();
    Task AddAsync(Employee Employee);
    Task UpdateAsync(Employee Employee);
    Task DeleteAsync(int id);
}
