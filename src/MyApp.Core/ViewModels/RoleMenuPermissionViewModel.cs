namespace MyApp.Core.ViewModels
{
    public class RoleMenuPermissionViewModel
    {
        public string RoleName { get; set; }
        public List<MenuPermissionViewModel> MenuPermissions { get; set; } = new();
    }

    public class MenuPermissionViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuCode { get; set; }
        public string IconName { get; set; }
        public bool IsParent { get; set; }
        public int? ParentId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}