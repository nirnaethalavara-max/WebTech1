using Kakhanouskaya.DOMAIN.Services;
using Kakhanouskaya.UI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Kakhanouskaya.UI.Services;
using System;
using System.Security.Claims;
using Serilog;
using Kakhanouskaya.UI.Middleware;

// Настройка Serilog (пішам у кансоль і ў файл)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

// Дадаём сховішча для сесій (у памяці)
builder.Services.AddDistributedMemoryCache();
// Дадаём сервіс сесій
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // сесія жыве 20 хвілін
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy =>
        policy.RequireClaim(ClaimTypes.Role, "admin"));
});
builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();

builder.Services.AddRazorPages();

//// ЧАСОВА - для Scaffold!
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlite("DataSource=:memory:"));

//builder.Services.AddControllersWithViews();

//builder.Services.AddAuthorization(opt =>
//{
//    opt.AddPolicy("admin", p => p.RequireClaim(System.Security.Claims.ClaimTypes.Role, "admin"));
//});

// ТОЛЬКІ API-СЕРВІСЫ (без Memory)
builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt =>
{
    opt.BaseAddress = new Uri("http://localhost:5002/api/categories/");
});

builder.Services.AddHttpClient<IProductService, ApiProductService>(opt =>
{
    opt.BaseAddress = new Uri("http://localhost:5002/api/dishes/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseFileLogger();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{page=Index}/{id?}");

app.MapRazorPages();

await DbInit.SeedData(app);

app.Run();