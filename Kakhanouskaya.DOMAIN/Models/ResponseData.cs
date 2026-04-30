using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kakhanouskaya.DOMAIN.Models
{
    public class ResponseData<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
