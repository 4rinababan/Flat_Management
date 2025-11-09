using MyApp.Core.ViewModels;

namespace MyApp.Core.Interfaces
{
    public interface IMenuPermissionService
    {
        Task<RoleMenuPermissionViewModel> GetRolePermissionsAsync(int roleId);
        Task UpdateRolePermissionsAsync(string roleName, List<MenuPermissionViewModel> permissions);
    }
}