namespace MyApp.Core.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CurrentRole { get; set; }
        public bool IsActive { get; set; }
    }
}