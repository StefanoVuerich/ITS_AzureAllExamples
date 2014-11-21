using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottor.Models
{
    public class ProductTE : TableEntity
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ProductTE() { }

        public ProductTE(string id, string name, string desc)
        {
            ProductId = id;
            Name = name;
            Description = desc;
        }
    }
}
