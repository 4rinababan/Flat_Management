using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;
using MyApp.Infrastructure.Data;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(int id) =>
        await _context.Employees.FindAsync(id);

    public async Task<List<Employee>> GetAllAsync() =>
        await _context.Employees.ToListAsync();

    public async Task AddAsync(Employee Employee)
    {
        await _context.Employees.AddAsync(Employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee Employee)
    {
        _context.Employees.Update(Employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var Employee = await _context.Employees.FindAsync(id);
        if (Employee != null)
        {
            _context.Employees.Remove(Employee);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Employee?> GetByNRPAsync(string nrp)
    {
        return await _context.Employees
            .Include(e => e.Position)
            .Include(e => e.Rank)
            .FirstOrDefaultAsync(e => e.NRP == nrp);
    }
}
