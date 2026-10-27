using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Interfaces;
using MvcMusicStore.Infrastructure.Data;
using MvcMusicStore.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Configure Services (replaces Global.asax Application_Start + App_Start)
// -----------------------------------------------------------------------

// Add MVC with views (replaces System.Web.Mvc registration)
builder.Services.AddControllersWithViews();

// Configure EF Core with PostgreSQL (replaces SQL Server UseSqlServer)
// UseNpgsql replaces UseSqlServer; EnableLegacyTimestampBehavior handles DateTime UTC behaviour;
// UseSnakeCaseNamingConvention maps PascalCase C# names to snake_case PostgreSQL identifiers.
builder.Services.AddDbContext<MusicStoreDbContext>(options =>
    options
        .UseNpgsql(
            builder.Configuration.GetConnectionString("MusicStoreEntities")
                ?? "Host=localhost;Port=5432;Database=MvcMusicStore;Username=postgres;Password=postgres",
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
            })
        .UseSnakeCaseNamingConvention());

// Configure ASP.NET Core Identity (replaces Forms Authentication + Membership provider)
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
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(2880); // 2 days - matches legacy timeout
    options.SlidingExpiration = true;
});

// Configure Session (replaces System.Web.SessionState)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register Repositories (Dependency Injection - replaces direct instantiation)
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

// Register Application Services
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add HttpContextAccessor for session access in services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// -----------------------------------------------------------------------
// Configure Middleware Pipeline (replaces Global.asax + web.config system.webServer)
// -----------------------------------------------------------------------

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

// Serve static files from wwwroot (replaces /Content and /Scripts folders)
app.UseStaticFiles();

app.UseRouting();

// Authentication & Authorization middleware (replaces Forms Authentication module)
app.UseAuthentication();
app.UseAuthorization();

// Session middleware (replaces System.Web.SessionState)
app.UseSession();

// Configure routes (replaces App_Start/RouteConfig.cs)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database with sample data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MusicStoreDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SampleDataSeeder.SeedAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

app.Run();
