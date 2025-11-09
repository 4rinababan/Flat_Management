namespace MyApp.Core.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string menuCode, string action);
    }
}