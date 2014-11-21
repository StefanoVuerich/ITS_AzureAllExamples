using Dottor.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dottor.AzureRepository;

namespace Dottor.Queues
{
    public class MyAzureQueue
    {
        AzureRepositoryCls _rep;
        private readonly string _QUEUENAME = "productqueue";
        CloudQueue _queue;

        public MyAzureQueue()
            : this("azureCS")
        {
        }

        public MyAzureQueue(string connectionStringName)
        {
            string storageConnectionString =
            ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference(_QUEUENAME);
            _queue.CreateIfNotExists();
        }

        public void SendToQueue(Product product)
        {
            var myJsonProduct = JsonConvert.SerializeObject(product);
            var QueueObj = new CloudQueueMessage(myJsonProduct);
            _queue.AddMessage(QueueObj);
            CheckIfSend();
        }

        private void CheckIfSend()
        {
            int? count = GetQueueCount();
            if (count != null && count >= 5)
            {
                _rep = new AzureRepositoryCls("azureStorage");
                for (int i = 0; i <= count - 1; i++)
                {
                    var obj = _queue.GetMessage(TimeSpan.FromSeconds(10));
                    var content = JsonConvert.DeserializeObject<Product>(obj.AsString);
                    _rep.Post(content);
                    _queue.DeleteMessage(obj);
                }
            }
        }

        private int? GetQueueCount()
        {
            _queue.FetchAttributes();
            return _queue.ApproximateMessageCount;
        }
    }
}
