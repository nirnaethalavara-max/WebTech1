using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kakhanouskaya.DOMAIN.Entities
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;         // назва стравы
        public string Description { get; set; } = string.Empty;  // апісанне
        public int Price { get; set; }                           // кошт (можна Weight або Calories)
        public string? Image { get; set; }                       // шлях да выявы

        // Уласцівасці для сувязі з катэгорыяй
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
