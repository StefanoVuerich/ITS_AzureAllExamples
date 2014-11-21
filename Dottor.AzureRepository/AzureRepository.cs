using Dottor.Domain.Contracts;
using Dottor.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottor.AzureRepository
{
    public class AzureRepositoryCls : IRepository<Product>
    {
        private readonly string _TABLENAME = "product";
        CloudTable _table;

        public AzureRepositoryCls()
            : this("azureCS")
        {
        }

        public AzureRepositoryCls(string connectionStringName)
        {
            string storageConnectionString =
            ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(_TABLENAME);
            _table.CreateIfNotExists();
        }
        public IEnumerable<Product> Get()
        {
            List<Product> listaProdotti = new List<Product>();
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<ProductTE> query = new TableQuery<ProductTE>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "1"));

            // Print the fields for each customer.
            foreach (ProductTE entity in _table.ExecuteQuery(query))
            {
                var product = new Product
                {
                    ProductId = entity.ProductId,
                    Name = entity.Name,
                    Description = entity.Description
                };
                listaProdotti.Add(product);
            }
            return listaProdotti;
        }
        public Product Get(int id)
        {
            string aa = new TableQuery<ProductTE>()
                .Where(TableQuery.GenerateFilterCondition("ProductId", QueryComparisons.Equal, id.ToString())).ToString();

            TableQuery<ProductTE> query = new TableQuery<ProductTE>()
                .Where(TableQuery.GenerateFilterCondition("ProductId", QueryComparisons.Equal, id.ToString()));
            var entity = _table.ExecuteQuery(query).LastOrDefault();
            if (entity != null)
            {
                return new Product
                {
                    ProductId = entity.ProductId,
                    Name = entity.Name,
                    Description = entity.Description
                };
            }
            return null;
        }

        public void Post(Product product)
        {
            ProductTE productTE = new ProductTE(product.ProductId, product.Name, product.Description);
            productTE.RowKey = DateTime.Now.Ticks.ToString();
            productTE.PartitionKey = "1";
            TableOperation insertOperation = TableOperation.Insert(productTE);
            _table.Execute(insertOperation);
        }

        public void Put(Product product)
        {
            
            TableQuery<ProductTE> query = new TableQuery<ProductTE>()
                .Where(TableQuery.GenerateFilterCondition("ProductId", QueryComparisons.Equal, product.ProductId.ToString()));
            ProductTE entity = _table.ExecuteQuery(query).FirstOrDefault();

            if (entity != null)
            {
                entity.ProductId = product.ProductId;
                entity.Name = product.Name;
                entity.Description = product.Description;

                TableOperation updateOperation = TableOperation.Replace(entity);

                _table.Execute(updateOperation);

                Console.WriteLine("Entity updated.");
            }

            else
                Console.WriteLine("Entity could not be retrieved.");
        }

        public void Delete(Product product)
        {
            TableQuery<ProductTE> query = new TableQuery<ProductTE>()
                .Where(TableQuery.GenerateFilterCondition("ProductId", QueryComparisons.Equal, product.ProductId.ToString()));
            ProductTE entity = _table.ExecuteQuery(query).FirstOrDefault();

            if (entity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(entity);
                _table.Execute(deleteOperation);
                Console.WriteLine("Entity deleted.");
            }

            else
                Console.WriteLine("Could not retrieve the entity.");
        }
    }
}
