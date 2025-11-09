using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Interfaces;
using MyApp.Infrastructure.Data;

namespace MyApp.Web.Authentication
{
    public class AccountPermissionModules : IPermissionService
    {
        private readonly AppDbContext _context;
        private readonly CustomAuthenticationStateProvider _authStateProvider;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AccountPermissionModules(
            AppDbContext context,
            CustomAuthenticationStateProvider authStateProvider,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _authStateProvider = authStateProvider;
            _roleManager = roleManager;
        }

        public async Task<bool> HasPermissionAsync(string menuCode, string action)
        {
            var userSession = await _authStateProvider.GetUserSessionAsync();
            if (userSession == null) return false;

            // Get user's role
            var role = userSession.Roles.FirstOrDefault();
            if (string.IsNullOrEmpty(role)) return false;

            // Get role from Identity
            var identityRole = await _roleManager.FindByNameAsync(role);
            if (identityRole == null) return false;

            // Get menu ID
            var menuId = await _context.Menus
                .Where(m => m.Code == menuCode)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();

            if (menuId == 0) return false;

            // Check permission
            var permission = await _context.MenuPermissions
                .FirstOrDefaultAsync(mp =>
                    mp.RoleId == identityRole.Id &&
                    mp.MenuId == menuId);

            if (permission == null) return false;

            return action switch
            {
                "view" => permission.CanView,
                "create" => permission.CanCreate,
                "update" => permission.CanUpdate,
                "delete" => permission.CanDelete,
                _ => false
            };
        }
    }
}
