using Microsoft.AspNetCore.Identity;

namespace MyApp.Core.Interfaces
{
    public interface IIdentityRoleService
    {
        Task<List<IdentityRole<int>>> GetAllRolesAsync();
        Task<IdentityRole<int>?> GetRoleByIdAsync(string roleId);
        Task<IdentityRole<int>?> GetRoleByNameAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> UpdateRoleAsync(IdentityRole<int> role);
        Task<IdentityResult> DeleteRoleAsync(int id);
    }
}