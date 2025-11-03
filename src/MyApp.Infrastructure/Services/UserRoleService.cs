using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;
using MyApp.Core.ViewModels;
using MyApp.Infrastructure.Identity;

namespace MyApp.Infrastructure.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserRoleService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserRoleViewModel>> GetUserRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRoleViewModel
                {
                    UserId = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email,
                    CurrentRole = roles.FirstOrDefault() ?? "No Role",
                    IsActive = !user.LockoutEnabled || user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.Now
                });
            }

            return userRoles;
        }

        public async Task<List<string>> GetAvailableRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task UpdateUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            return (await _userManager.GetRolesAsync(user)).ToList();
        }
    }
}