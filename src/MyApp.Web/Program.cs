using Blazored.Toast;
using Blazored.Typeahead;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Interfaces;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Identity;
using MyApp.Web.Data;
using MyApp.Web.Helpers;
using MyApp.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// DATABASE CONTEXTS
// ======================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Identity pakai SQL Server (boleh juga MySQL, asal konsisten)

// // Add Identity
// builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
// {
//     options.Password.RequiredLength = 6;
//     options.Password.RequireDigit = false;
//     options.Password.RequireUppercase = false;
//     options.Password.RequireNonAlphanumeric = false;
//     options.User.RequireUniqueEmail = true;
// })
// .AddEntityFrameworkStores<ApplicationDbContext>()
// .AddDefaultTokenProviders();
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

    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()  // Ubah ke AppDbContext
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/forbidden";
    options.Cookie.Name = "MyApp.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax; // Ubah dari None ke Lax untuk development
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

// ======================================================
// REDIS (optional, tetap untuk caching umum, bukan auth)
// ======================================================
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyApp_";
});

// ======================================================
// DEPENDENCY INJECTION REPOSITORIES
// ======================================================
// Hapus atau comment IUserRepository jika hanya untuk custom User table
// builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
builder.Services.AddScoped<IRankRepository, RankRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomCategoryRepository, RoomCategoryRepository>();
builder.Services.AddScoped<IRoomStatusRepository, RoomStatusRepository>();
builder.Services.AddScoped<IRoomConditionRepository, RoomConditionRepository>();
builder.Services.AddScoped<IOccupantRepository, OccupantRepository>();
builder.Services.AddScoped<IOccupantHistoryRepository, OccupantHistoryRepository>();
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IVisitorRepository, VisitorRepository>();
builder.Services.AddScoped<UploadService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();
builder.Services.AddScoped<IInventoryTypeRepository, InventoryTypeRepository>();
builder.Services.AddScoped<IRepositoryRepository, RepositoryRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddBlazoredToast();
builder.Services.AddHttpClient();

// ======================================================
// BLAZOR SERVER
// ======================================================
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

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
// DATABASE MIGRATION & SEEDING
// ======================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        context.Database.Migrate();

        // --- Seed Identity Roles ---
        var adminRoleName = "Admin";
        var managerRoleName = "Manager";
        var userRoleName = "User";

        foreach (var roleName in new[] { adminRoleName, managerRoleName, userRoleName })
        {
            var roleExists = await roleManager.FindByNameAsync(roleName);
            if (roleExists == null)
            {
                var result = await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
                if (!result.Succeeded)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError($"Failed to create role: {roleName}");
                }
            }
        }

        // --- Seed Identity Admin User ---
        var config = services.GetRequiredService<IConfiguration>();
        var adminUserName = config["AdminUser:UserName"] ?? "admin";
        var adminEmail = config["AdminUser:Email"] ?? "admin@example.com";
        var adminPassword = config["AdminUser:Password"] ?? "Admin@123";

        var adminUser = await userManager.FindByNameAsync(adminUserName);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRoleName);
            }
            else
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database seeding failed");
    }
}

// ======================================================
// MIDDLEWARE PIPELINE
// ======================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ======================================================
// ENDPOINTS
// ======================================================
app.MapControllers();
app.MapBlazorHub();
app.MapRazorPages();
app.MapFallbackToPage("/_Host");

app.Run();
