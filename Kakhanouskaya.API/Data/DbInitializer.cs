using Microsoft.EntityFrameworkCore;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.API.Data;
using Kakhanouskaya.DOMAIN.Entities;

namespace Kakhanouskaya.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            // URI твайго API (порт 7002)
            var uri = "https://localhost:7002/";

            // Атрыманне кантэксту БД
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Выкананне міграцый (калі ёсць новыя)
            await context.Database.MigrateAsync();

            // Праверка, ці ёсць ужо даныя
            if (!context.Categories.Any() && !context.Dishes.Any())
            {
                // Стварэнне катэгорый
                var categories = new Category[]
                {
                    new Category { Name = "Desserts", NormalizedName = "desserts" },
                    new Category { Name = "Cheese", NormalizedName = "cheese" }  
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var dishes = new List<Dish>
                {
                    new Dish
                    {
                        Name = "Абваранка",
                        Description = "Да гарбаты",
                        Price = 12,
                        Category = categories.First(c => c.NormalizedName == "desserts"),
                        Image = uri + "Images/free-png.ru-74-200x200.png"
                    },
                    new Dish
                    {
                        Name = "Абваранка з марцыпанамі",
                        Description = "Да кампоту",
                        Price = 15,
                        Category = categories.First(c => c.NormalizedName == "desserts"),
                        Image = uri + "Images/free-png.ru-78-200x200.png"
                    },
                    new Dish
                    {
                        Name = "Сыр",
                        Description = "Да ўсяго",
                        Price = 14,
                        Category = categories.First(c => c.NormalizedName == "cheese"),
                        Image = uri + "Images/free-png.ru-641-200x200.png"
                    },
                    new Dish
                    {
                        Name = "Торт",
                        Description = "Каларыйная бомба",
                        Price = 25,
                        Category = categories.First(c => c.NormalizedName == "desserts"),
                        Image = uri + "Images/free-png.ru-848-200x200.png"
                    }
                };

                await context.Dishes.AddRangeAsync(dishes);
                await context.SaveChangesAsync();
            }
        }
    }
}
