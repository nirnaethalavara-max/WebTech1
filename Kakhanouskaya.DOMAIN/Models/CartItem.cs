using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kakhanouskaya.DOMAIN.Entities;

namespace Kakhanouskaya.DOMAIN.Models
{    
        public class CartItem
        {
            public Dish Item { get; set; }  
            public int Quantity { get; set; }
        }
    
}
