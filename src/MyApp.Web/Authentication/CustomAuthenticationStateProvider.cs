using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using MyApp.Core.Domain;
using MyApp.Infrastructure.Identity;
using System.Security.Claims;
using System.Text.Json;

namespace MyApp.Web.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string USER_SESSION_KEY = "user_session";
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;
        private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(
            ProtectedSessionStorage sessionStorage,
            ILogger<CustomAuthenticationStateProvider> logger)
        {
            _sessionStorage = sessionStorage;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<string>(USER_SESSION_KEY);

                if (!result.Success || string.IsNullOrEmpty(result.Value))
                {
                    return new AuthenticationState(_anonymous);
                }

                var userSession = JsonSerializer.Deserialize<UserSession>(result.Value);

                // Validasi session expiry
                if (userSession?.ExpiresAt.HasValue == true &&
                    userSession.ExpiresAt.Value < DateTime.UtcNow)
                {
                    _logger.LogInformation("Session expired for user: {Username}", userSession.Username);
                    await NotifyUserLogout();
                    return new AuthenticationState(_anonymous);
                }

                var claimsPrincipal = CreateClaimsPrincipal(userSession);
                return new AuthenticationState(claimsPrincipal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading authentication state");
                return new AuthenticationState(_anonymous);
            }
        }

        /// <summary>
        /// Notify user authentication dengan data lengkap
        /// </summary>
        public async Task<bool> NotifyUserAuthentication(UserSession userSession)
        {
            try
            {
                if (userSession == null || string.IsNullOrEmpty(userSession.Username))
                {
                    _logger.LogWarning("Attempted to authenticate with invalid user session");
                    return false;
                }

                userSession.LoginTime = DateTime.UtcNow;

                // Set default expiry jika tidak ada (24 jam)
                if (!userSession.ExpiresAt.HasValue)
                {
                    userSession.ExpiresAt = DateTime.UtcNow.AddHours(24);
                }

                var sessionJson = JsonSerializer.Serialize(userSession);
                await _sessionStorage.SetAsync(USER_SESSION_KEY, sessionJson);

                var claimsPrincipal = CreateClaimsPrincipal(userSession);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

                _logger.LogInformation("User authenticated: {Username}", userSession.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user authentication");
                return false;
            }
        }

        /// <summary>
        /// Logout user dan clear session
        /// </summary>
        public async Task NotifyUserLogout()
        {
            try
            {
                await _sessionStorage.DeleteAsync(USER_SESSION_KEY);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
                _logger.LogInformation("User logged out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }
        }

        /// <summary>
        /// Get current user session data
        /// </summary>
        public async Task<UserSession> GetUserSessionAsync()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<string>(USER_SESSION_KEY);
                if (result.Success && !string.IsNullOrEmpty(result.Value))
                {
                    return JsonSerializer.Deserialize<UserSession>(result.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user session");
            }
            return null;
        }

        /// <summary>
        /// Update user session (misalnya update roles atau claims)
        /// </summary>
        public async Task<bool> UpdateUserSessionAsync(UserSession userSession)
        {
            try
            {
                if (userSession == null)
                    return false;

                var sessionJson = JsonSerializer.Serialize(userSession);
                await _sessionStorage.SetAsync(USER_SESSION_KEY, sessionJson);

                var claimsPrincipal = CreateClaimsPrincipal(userSession);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user session");
                return false;
            }
        }

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }

        /// <summary>
        /// Check if user has specific role
        /// </summary>
        public async Task<bool> IsInRoleAsync(string role)
        {
            var authState = await GetAuthenticationStateAsync();
            return authState.User.IsInRole(role);
        }

        private ClaimsPrincipal CreateClaimsPrincipal(UserSession userSession)
        {
            if (userSession == null)
                return _anonymous;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userSession.UserId ?? ""),
                new Claim(ClaimTypes.Name, userSession.Username),
            };

            // Add email claim
            if (!string.IsNullOrEmpty(userSession.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, userSession.Email));
            }

            // Add role claims
            foreach (var role in userSession.Roles ?? new List<string>())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add custom claims
            foreach (var claim in userSession.Claims ?? new Dictionary<string, string>())
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }

            var identity = new ClaimsIdentity(claims, "CustomAuth");
            return new ClaimsPrincipal(identity);
        }
    }
}