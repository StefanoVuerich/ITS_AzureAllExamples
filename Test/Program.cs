using Dottor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dottor.Domain.Contracts;
using Dottor.AzureRepository;
using Dottor.Queues;
using Dottor.Blobs;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Product product = new Product("1", "Asciugacapelli", "Asciugacapelli professionale nuovo");
            //Product product2 = new Product("2", "Ciospo", "Asciugacapelli professionale nuovo");
            //Product product3 = new Product("3", "Computer", "Asciugacapelli professionale nuovo");
            //Product product4 = new Product("4", "Tavola", "Asciugacapelli professionale nuovo");
            //Product product5 = new Product("5", "Mela", "Asciugacapelli professionale nuovo");
            IRepository<Product> _rep = new AzureRepositoryCls("azureStorage");
            //_rep.Post(product);
            //_rep.Post(product2);
            //_rep.Post(product3);
            //_rep.Post(product4);
            //_rep.Post(product5);
            List<Product> products = _rep.Get().ToList();
            foreach (var item in products)
            {
                Console.WriteLine(item.ProductId + " / " + item.Name + " / " + item.Description);
            }
            Console.ReadKey();
            //_rep.Delete(product);
            //Product a = _rep.Get(1);
            //Console.WriteLine(a.Name + " " + a.Description);
            //Console.ReadKey();
            //_rep.Delete(product);

            //MyAzureQueue queue = new MyAzureQueue("azureStorage");

            //queue.SendToQueue(product);
            //queue.SendToQueue(product2);
            //queue.SendToQueue(product3);
            //queue.SendToQueue(product4);
            //queue.SendToQueue(product5);

            string[] filesToWatch = { "*.json", "*.txt" };
            MyBlob blob = new MyBlob(filesToWatch);
            //blob.Delete("avatar - Copy.png");
        }
    }
}
