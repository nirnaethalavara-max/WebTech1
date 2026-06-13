using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kakhanouskaya.DOMAIN.Entities;

namespace Kakhanouskaya.DOMAIN.Models
{
      
        public class Cart
        {
            // Ключ - Id стравы, значэнне - элемент кошыка
            public Dictionary<int, CartItem> CartItems { get; set; } = new();

            // Агульная колькасць тавараў (сума колькасцей)
            public int TotalCount => CartItems.Values.Sum(x => x.Quantity);

            // Сумарная характарыстыка (у лабе - калорыі)
            public int TotalPrice => CartItems.Values.Sum(x => x.Item.Price * x.Quantity);

            // Дадаць у кошык
            public virtual void AddToCart(Dish dish)
            {
                if (CartItems.ContainsKey(dish.Id))
                {
                    // Ужо ёсць - павялічваем колькасць
                    CartItems[dish.Id].Quantity++;
                }
                else
                {
                    // Няма - дадаем новы
                    CartItems.Add(dish.Id, new CartItem { Item = dish, Quantity = 1 });
                }
            }

            // Выдаліць усе запісы з дадзеным Id
            public virtual void RemoveItems(int id)
            {
                CartItems.Remove(id);
            }

            // Ачысціць увесь кошык
            public virtual void Clear()
            {
                CartItems.Clear();
            }
        }
    
}
