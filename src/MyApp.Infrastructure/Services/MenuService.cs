using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MenuService> _logger;

        public MenuService(AppDbContext context, ILogger<MenuService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Menu Permission & Navigation Methods

        public async Task<List<MenuDto>> GetMenusByUserRolesAsync(List<string> roles)
        {
            try
            {
                // Get role IDs from role names using IdentityRole
                var roleIds = await _context.Set<IdentityRole<int>>()
                    .Where(r => roles.Contains(r.Name))
                    .Select(r => r.Id)
                    .ToListAsync();

                if (!roleIds.Any())
                {
                    _logger.LogWarning("No matching roles found for user");
                    return new List<MenuDto>();
                }

                // Load ALL menus sekaligus dengan permissions (hindari N+1 query problem)
                var allMenus = await _context.Menus
                    .Include(m => m.MenuPermissions)
                    .Where(m => m.IsActive)
                    .OrderBy(m => m.Order)
                    .ToListAsync();

                // Filter parent menus saja
                var parentMenus = allMenus.Where(m => m.ParentId == null).ToList();

                var menuDtos = new List<MenuDto>();

                // Process menus secara synchronous dari data yang sudah di-load
                foreach (var menu in parentMenus)
                {
                    var permissions = GetHighestPermissions(menu.MenuPermissions, roleIds);
                    
                    if (permissions.CanView)
                    {
                        var menuDto = new MenuDto
                        {
                            Code = menu.Code,
                            Name = menu.Name,
                            IconName = menu.IconName,
                            Color = menu.Color,
                            Url = menu.Url,
                            Order = menu.Order,
                            Permissions = permissions
                        };

                        // Get children dari data yang sudah di-load (TIDAK query database lagi)
                        menuDto.Children = GetChildMenusRecursive(menu.Id, allMenus, roleIds);
                        menuDtos.Add(menuDto);
                    }
                }

                return menuDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menus by user roles");
                return new List<MenuDto>();
            }
        }

        // Method synchronous yang process data yang sudah di-load
        private List<MenuDto> GetChildMenusRecursive(int parentId, List<Menu> allMenus, List<int> roleIds)
        {
            var children = allMenus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Order)
                .ToList();

            var childDtos = new List<MenuDto>();

            foreach (var child in children)
            {
                var permissions = GetHighestPermissions(child.MenuPermissions, roleIds);
                
                if (permissions.CanView)
                {
                    var childDto = new MenuDto
                    {
                        Code = child.Code,
                        Name = child.Name,
                        IconName = child.IconName,
                        Color = child.Color,
                        Url = child.Url,
                        Order = child.Order,
                        Permissions = permissions
                    };

                    // Recursive call pada data yang sudah di-load
                    childDto.Children = GetChildMenusRecursive(child.Id, allMenus, roleIds);
                    childDtos.Add(childDto);
                }
            }

            return childDtos;
        }

        private MenuPermissionDto GetHighestPermissions(
            ICollection<MenuPermission> menuPermissions, 
            List<int> roleIds)
        {
            var userPermissions = menuPermissions
                .Where(mp => roleIds.Contains(mp.RoleId))
                .ToList();

            if (!userPermissions.Any())
            {
                return new MenuPermissionDto();
            }

            // Get highest permissions (OR operation)
            return new MenuPermissionDto
            {
                CanView = userPermissions.Any(p => p.CanView),
                CanCreate = userPermissions.Any(p => p.CanCreate),
                CanUpdate = userPermissions.Any(p => p.CanUpdate),
                CanDelete = userPermissions.Any(p => p.CanDelete)
            };
        }

        public async Task<MenuPermissionDto?> GetMenuPermissionAsync(string menuCode, List<string> roles)
        {
            try
            {
                var roleIds = await _context.Set<IdentityRole<int>>()
                    .Where(r => roles.Contains(r.Name))
                    .Select(r => r.Id)
                    .ToListAsync();

                if (!roleIds.Any())
                    return null;

                var menu = await _context.Menus
                    .Include(m => m.MenuPermissions)
                    .FirstOrDefaultAsync(m => m.Code == menuCode && m.IsActive);

                if (menu == null)
                    return null;

                return GetHighestPermissions(menu.MenuPermissions, roleIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu permission");
                return null;
            }
        }

        public async Task<List<Menu>> GetAllMenusAsync()
        {
            return await _context.Menus
                .Include(m => m.Children)
                .Include(m => m.MenuPermissions)
                .Where(m => m.ParentId == null)
                .OrderBy(m => m.Order)
                .ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(string menuCode, List<string> roles, string permissionType)
        {
            var permission = await GetMenuPermissionAsync(menuCode, roles);
            if (permission == null)
                return false;

            return permissionType.ToLower() switch
            {
                "view" => permission.CanView,
                "create" => permission.CanCreate,
                "update" => permission.CanUpdate,
                "delete" => permission.CanDelete,
                _ => false
            };
        }

        #endregion

        #region CRUD Methods

        public async Task<List<Menu>> GetAllAsync()
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.Children)
                    .OrderBy(m => m.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all menus");
                throw;
            }
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.Children)
                    .Include(m => m.MenuPermissions)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting menu by id: {id}");
                throw;
            }
        }

        public async Task<Menu> AddAsync(Menu menu)
        {
            try
            {
                // Validate unique code
                var existingMenu = await _context.Menus
                    .FirstOrDefaultAsync(m => m.Code == menu.Code);

                if (existingMenu != null)
                {
                    throw new InvalidOperationException($"Menu with code '{menu.Code}' already exists.");
                }

                // Set timestamps
                menu.CreatedAt = DateTime.UtcNow;
                menu.UpdatedAt = null;

                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Menu '{menu.Name}' added successfully with ID: {menu.Id}");
                return menu;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding menu");
                throw;
            }
        }

        public async Task UpdateAsync(Menu menu)
        {
            try
            {
                var existingMenu = await _context.Menus.FindAsync(menu.Id);
                if (existingMenu == null)
                {
                    throw new InvalidOperationException($"Menu with ID {menu.Id} not found.");
                }

                // Validate unique code (exclude current menu)
                var duplicateCode = await _context.Menus
                    .AnyAsync(m => m.Code == menu.Code && m.Id != menu.Id);

                if (duplicateCode)
                {
                    throw new InvalidOperationException($"Menu with code '{menu.Code}' already exists.");
                }

                // Prevent circular parent reference
                if (menu.ParentId.HasValue)
                {
                    if (await IsCircularReference(menu.Id, menu.ParentId.Value))
                    {
                        throw new InvalidOperationException("Cannot set parent menu: circular reference detected.");
                    }
                }

                // Update properties
                existingMenu.Code = menu.Code;
                existingMenu.Name = menu.Name;
                existingMenu.IconName = menu.IconName;
                existingMenu.Color = menu.Color;
                existingMenu.Url = menu.Url;
                existingMenu.ParentId = menu.ParentId;
                existingMenu.Order = menu.Order;
                existingMenu.IsActive = menu.IsActive;
                existingMenu.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Menu '{menu.Name}' updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating menu with ID: {menu.Id}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var menu = await _context.Menus
                    .Include(m => m.Children)
                    .Include(m => m.MenuPermissions)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (menu == null)
                {
                    throw new InvalidOperationException($"Menu with ID {id} not found.");
                }

                // Check if menu has children
                if (menu.Children.Any())
                {
                    throw new InvalidOperationException("Cannot delete menu with child menus. Delete children first.");
                }

                // Remove menu permissions first
                if (menu.MenuPermissions.Any())
                {
                    _context.MenuPermissions.RemoveRange(menu.MenuPermissions);
                }

                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Menu '{menu.Name}' deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting menu with ID: {id}");
                throw;
            }
        }

        public async Task<List<Menu>> GetMenusByParentIdAsync(int? parentId)
        {
            try
            {
                return await _context.Menus
                    .Where(m => m.ParentId == parentId)
                    .OrderBy(m => m.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting menus by parent ID: {parentId}");
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private async Task<bool> IsCircularReference(int menuId, int parentId)
        {
            var parent = await _context.Menus.FindAsync(parentId);
            if (parent == null)
                return false;

            if (parent.Id == menuId)
                return true;

            if (parent.ParentId.HasValue)
            {
                return await IsCircularReference(menuId, parent.ParentId.Value);
            }

            return false;
        }

        #endregion
    }
}