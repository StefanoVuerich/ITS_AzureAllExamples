using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottor.Models
{
    public class Product
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Product() { }

        public Product(string id, string name, string desc)
        {
            ProductId = id;
            Name = name;
            Description = desc;
        }
    }
}
