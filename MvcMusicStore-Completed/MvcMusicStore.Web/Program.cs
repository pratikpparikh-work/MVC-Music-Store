using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Interfaces;
using MvcMusicStore.Infrastructure.Data;
using MvcMusicStore.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Configure Services (replaces Global.asax Application_Start + web.config)
// -----------------------------------------------------------------------

// Add MVC with views
builder.Services.AddControllersWithViews();

// Configure EF Core DbContext with PostgreSQL (replaces SQL Server / SQLite)
// Npgsql is the recommended provider for PostgreSQL with EF Core 8
builder.Services.AddDbContext<MusicStoreDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MusicStoreConnection")
        ?? "Host=localhost;Port=5432;Database=MvcMusicStore;Username=postgres;Password=postgres";

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });

    // Use snake_case naming convention for PostgreSQL compatibility
    options.UseSnakeCaseNamingConvention();
});

// Configure ASP.NET Core Identity (replaces Forms Authentication + Membership provider)
// FormsAuthentication.SetAuthCookie -> SignInManager.SignInAsync
// Membership.ValidateUser -> SignInManager.PasswordSignInAsync
// Membership.CreateUser -> UserManager.CreateAsync
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<MusicStoreDbContext>();

// Configure Cookie Authentication (replaces Forms Authentication in web.config)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/LogOff";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(2880); // 2 days
    options.SlidingExpiration = true;
});

// Configure Session (replaces System.Web session state)
// builder.Services.AddDistributedMemoryCache() + AddSession() replaces System.Web.SessionState
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register Domain Repositories (Dependency Injection - replaces direct DbContext instantiation)
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register Application Services
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// -----------------------------------------------------------------------
// Build and Configure Middleware Pipeline
// -----------------------------------------------------------------------
var app = builder.Build();

// Seed database with sample data (replaces EF6 Database.SetInitializer + DropCreateDatabaseIfModelChanges)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MusicStoreDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        SampleDataSeeder.Seed(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

// Error handling middleware (replaces HandleErrorAttribute global filter)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serves files from wwwroot (replaces /Content and /Scripts folders)

app.UseRouting();

app.UseAuthentication(); // Replaces Forms Authentication HTTP module
app.UseAuthorization();  // Replaces role-based authorization

app.UseSession(); // Replaces System.Web session state

// Configure Routes (replaces App_Start/RouteConfig.cs + RouteTable.Routes)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages for Identity UI
app.MapRazorPages();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
