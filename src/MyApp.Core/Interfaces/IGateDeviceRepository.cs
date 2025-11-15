using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces;

    public interface IGateDeviceRepository
    {
        Task<List<GateDevice>> GetAllAsync();
        Task<GateDevice?> GetBySNAsync(string deviceId);
        Task<GateDevice?> GetByDeviceSNAsync(string deviceId);
        Task AddAsync(GateDevice entity);
        Task UpdateAsync(GateDevice entity);
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync();

    }

