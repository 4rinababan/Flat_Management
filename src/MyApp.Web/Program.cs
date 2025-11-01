using Blazored.Toast;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using MyApp.Infrastructure;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Identity;
using MyApp.Web.Authentication;
using MyApp.Web.Data;
using MyApp.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

// ======================================================
// BLAZOR SERVER
// ======================================================
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// ======================================================
// ASP.NET CORE IDENTITY
// ======================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// ======================================================
// COOKIE POLICY
// ======================================================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/forbidden";
    options.Cookie.Name = "MyApp.Auth";
    options.Cookie.HttpOnly = true;

    if (builder.Environment.IsDevelopment())
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Allow HTTP in dev
        options.Cookie.SameSite = SameSiteMode.Lax;
    }
    else
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only in production
        options.Cookie.SameSite = SameSiteMode.Strict;
    }

    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;

    options.Events.OnRedirectToLogin = context =>
    {
        // Jangan redirect untuk API calls
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});


builder.Services.AddScoped<UploadService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddBlazoredToast();
builder.Services.AddHttpClient();


builder.Services.AddSingleton<WeatherForecastService>();


builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddCascadingAuthenticationState();


builder.Services.AddAuthorization();

builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

builder.Services.AddSingleton(new TTLockClient(
    clientId: "3c9fffb0685f4653bb2f0aeef43157e7",
    clientSecret: "b70df03c403d1c5f743d80acb2024a94",
    username: "arinababan123@gmail.com",
    password: "ar1aja123"
));

builder.Services.AddScoped<DeviceService>();

// ======================================================
// CONTROLLERS
// ======================================================
builder.Services.AddControllers();

// ======================================================
// BUILD APP
// ======================================================
var app = builder.Build();

// ======================================================
// MIDDLEWARE CONFIGURATION
// ======================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// COOKIE POLICY HARUS SEBELUM ROUTING
app.UseCookiePolicy();

app.UseRouting();


// PENTING: Urutan ini harus benar!
app.UseAuthentication();  // Harus SEBELUM Authorization
app.UseAuthorization();

// Tambahkan logging middleware untuk debug
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    logger.LogInformation($"IsAuthenticated: {context.User?.Identity?.IsAuthenticated}, User: {context.User?.Identity?.Name}");
    logger.LogInformation($"Cookies: {string.Join(", ", context.Request.Cookies.Keys)}");
    await next();
});

app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
