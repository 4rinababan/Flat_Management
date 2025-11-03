using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Interfaces;

namespace MyApp.Infrastructure.Services
{
    public class IdentityRoleService : IIdentityRoleService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public IdentityRoleService(RoleManager<IdentityRole<int>> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<IdentityRole<int>>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole<int>?> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<IdentityRole<int>?> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            var role = new IdentityRole<int>(roleName);
            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateRoleAsync(IdentityRole<int> role)
        {
            var existingRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            if (existingRole == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found" });
            }

            existingRole.Name = role.Name;
            return await _roleManager.UpdateAsync(existingRole);
        }

        public async Task<IdentityResult> DeleteRoleAsync(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found" });
            }

            return await _roleManager.DeleteAsync(role);
        }
    }
}