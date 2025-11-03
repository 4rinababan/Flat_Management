using MyApp.Core.ViewModels;

namespace MyApp.Core.Interfaces
{
    public interface IUserRoleService
    {
        Task<List<UserRoleViewModel>> GetUserRolesAsync();
        Task<List<string>> GetAvailableRolesAsync();
        Task UpdateUserRoleAsync(string userId, string newRole);
        Task<List<string>> GetUserRolesAsync(string userId);
    }
}