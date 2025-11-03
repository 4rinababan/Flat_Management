using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;
using MyApp.Core.ViewModels;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Identity;

namespace MyApp.Infrastructure.Services
{
    public class MenuPermissionService : IMenuPermissionService
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public MenuPermissionService(AppDbContext context, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<RoleMenuPermissionViewModel> GetRolePermissionsAsync(int roleId)
        {
            // Validate role exists
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) throw new Exception("Role not found");

            // Get all menus
            var menus = await _context.Menus
                .AsNoTracking()
                .OrderBy(m => m.Order)
                .ToListAsync();

            // Get existing permissions for the role (if any)
            var permissions = await _context.MenuPermissions
                .Where(mp => mp.RoleId == roleId)
                .AsNoTracking()
                .ToListAsync();

            // Create view model with all menus, checking permissions where they exist
            return new RoleMenuPermissionViewModel
            {
                RoleName = role.Name,
                MenuPermissions = menus.Select(m => new MenuPermissionViewModel
                {
                    MenuId = m.Id,
                    MenuName = m.Name,
                    MenuCode = m.Code,
                    IconName = m.IconName,
                    IsParent = !m.ParentId.HasValue,
                    ParentId = m.ParentId,
                    // Check if permission exists, otherwise default to false
                    CanView = permissions.Any(p => p.MenuId == m.Id && p.CanView),
                    CanCreate = permissions.Any(p => p.MenuId == m.Id && p.CanCreate),
                    CanUpdate = permissions.Any(p => p.MenuId == m.Id && p.CanUpdate),
                    CanDelete = permissions.Any(p => p.MenuId == m.Id && p.CanDelete)
                }).ToList()
            };
        }

        public async Task UpdateRolePermissionsAsync(string roleName, List<MenuPermissionViewModel> permissions)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) throw new Exception("Role not found");

            // Remove existing permissions
            var existingPermissions = await _context.MenuPermissions
                .Where(mp => mp.RoleId == role.Id)
                .ToListAsync();
            
            _context.MenuPermissions.RemoveRange(existingPermissions);

            // Add new permissions
            var newPermissions = permissions.Select(p => new MenuPermission
            {
                MenuId = p.MenuId,
                RoleId = role.Id,
                CanView = p.CanView,
                CanCreate = p.CanCreate,
                CanUpdate = p.CanUpdate,
                CanDelete = p.CanDelete,
                UpdatedAt = DateTime.UtcNow
            });

            await _context.MenuPermissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();
        }
    }
}