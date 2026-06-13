using Kakhanouskaya.Blazor.Components;
using Kakhanouskaya.Blazor.Services;
using Kakhanouskaya.DOMAIN.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Дадаем сервіс для працы з API
builder.Services.AddHttpClient<IProductService<Dish>, ApiProductService>(client =>
{
    // URL твайго API (правер порт!)
    client.BaseAddress = new Uri("https://localhost:7002/api/dishes");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
