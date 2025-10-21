using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces; // <- pastikan ini ada

namespace MyApp.Infrastructure.Data
{
    public class GateDeviceRepository : IGateDeviceRepository
    {
        private readonly AppDbContext _context;

        public GateDeviceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GateDevice?> GetBySNAsync(string sn) =>
            await _context.GateDevices.FirstOrDefaultAsync(d => d.DeviceSn == sn);

        public async Task<List<GateDevice>> GetAllAsync() =>
            await _context.GateDevices.ToListAsync();

        public async Task AddAsync(GateDevice entity)
        {
            await _context.GateDevices.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GateDevice entity)
        {
            var existing = await _context.GateDevices.FindAsync(entity.Id);
            if (existing != null)
            {
                existing.DeviceName = entity.DeviceName;
                existing.DeviceAlias = entity.DeviceAlias;
                existing.DeviceSn = entity.DeviceSn;
                existing.DeviceIp = entity.DeviceIp;
                existing.IsRegistered = entity.IsRegistered;
                existing.IsAttendance = entity.IsAttendance;
                existing.Heartbeat = entity.Heartbeat;
                existing.LastHeartbeat = entity.LastHeartbeat;
                existing.TransferMode = entity.TransferMode;
                existing.Timezone = entity.Timezone;
                existing.DeviceOption = entity.DeviceOption;

                existing.FirmwareVersion = entity.FirmwareVersion;
                existing.TotalEnrolledUser = entity.TotalEnrolledUser;
                existing.TotalEnrolledFingerprint = entity.TotalEnrolledFingerprint;
                existing.TotalEnrolledFace = entity.TotalEnrolledFace;
                existing.TotalAttendancesRecord = entity.TotalAttendancesRecord;
                existing.FingerprintAlgorithmVersion = entity.FingerprintAlgorithmVersion;
                existing.FaceAlgorithmVersion = entity.FaceAlgorithmVersion;
                existing.RequiredFaceEnrollment = entity.RequiredFaceEnrollment;
                existing.Value10 = entity.Value10;
                existing.UpdatedAt = entity.UpdatedAt;
                existing.CreatedAt = entity.CreatedAt;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.GateDevices.FindAsync(id);
            if (entity != null)
            {
                _context.GateDevices.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // public async Task<List<GateDevice>> GetDevicesAsync()
        // {
        //     // Misal ini mengambil semua device yang aktif / registered
        //     return await _context.GateDevices
        //         .Where(d => d.IsRegistered)
        //         .ToListAsync();
        // }

        // public async Task RegisterEmployeeCardAsync(int employeeId, string cardNo)
        // {
        //     // Contoh sederhana: buat record baru di tabel linking employee & card
        //     var employeeCard = new EmployeeCard
        //     {
        //         EmployeeId = employeeId,
        //         CardNo = cardNo,
        //         CreatedAt = DateTime.UtcNow
        //     };

        //     await _context.EmployeeCards.AddAsync(employeeCard);
        //     await _context.SaveChangesAsync();
        // }

        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<GateDevice?> GetByDeviceSNAsync(string deviceSn) =>
            await _context.GateDevices.FirstOrDefaultAsync(d => d.DeviceSn == deviceSn);
    }
    
    
}
