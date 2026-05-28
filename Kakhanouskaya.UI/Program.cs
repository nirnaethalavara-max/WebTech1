using Kakhanouskaya.DOMAIN.Services;
using Kakhanouskaya.UI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Kakhanouskaya.UI.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireClaim(System.Security.Claims.ClaimTypes.Role, "admin"));
});

// “ŒÀ‹ ≤ API-—≈–¬≤—€ (·ÂÁ Memory)
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await DbInit.SeedData(app);

app.Run();