using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kakhanouskaya.DOMAIN.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;          // назва па-беларуску/руску
        public string NormalizedName { get; set; } = string.Empty; // для URL (напрыклад, "soups")

        // Навігацыйная ўласцівасць
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }
}
