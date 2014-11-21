using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dottor.Blobs
{
    public class MyBlob
    {
        public delegate BlobDTO AdapterDelegate(CloudBlockBlob blob);
        static string storageConnectionString = ConfigurationManager.ConnectionStrings["azureStorage"].ConnectionString;
        List<string> filesToWatch;
        List<FileSystemWatcher> watchersList;

        public MyBlob() { }

        public MyBlob(string[] extensionsToWatch)
        {
            filesToWatch = new List<string>();
            watchersList = new List<FileSystemWatcher>();
            foreach (var x in extensionsToWatch)
            {
                filesToWatch.Add(x);
                FileSystemWatcher current_watcher = new FileSystemWatcher("C:\\temp", x);
                current_watcher.Created += watcher_Created;
                current_watcher.Deleted += watcher_Deleted;
                current_watcher.Changed += watcher_Changed;
                current_watcher.EnableRaisingEvents = true;
                current_watcher.IncludeSubdirectories = true;
                watchersList.Add(current_watcher);
            }


            while (true)
            {
                Console.Write("I'm watching ");
                foreach (var x in filesToWatch)
                {
                    Console.Write(x + " ");
                }
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("waiting for file type to log");

                var fileToLog = Console.ReadLine();
                if (fileToLog != "")
                {
                    AddNewWatcher(fileToLog);
                }

                Thread.Sleep(1000);
            }
        }

        private class MyBlobAdapter
        {
            public BlobDTO Adapt(CloudBlockBlob blobEntity)
            {
                BlobDTO entity = new BlobDTO();
                entity.Name = blobEntity.Name;
                entity.ContentType = blobEntity.Properties.ContentType;

                return entity;
            }
        }

        #region Actions

        //public IEnumerable<BlobDTO> GetAll()
        public string GetAll()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("txtcontainer");

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;
                    return directory.Uri.ToString();

                    //Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }
            return null;
        }

        public object Get(string id)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("jsoncontainer");

            var entity = container.GetBlockBlobReference(id);

            if (entity.Exists())
            {

                MyBlobAdapter ad = new MyBlobAdapter();
                AdapterDelegate del = new AdapterDelegate(ad.Adapt);
                BlobDTO x = del(entity);

                return x;
            }

            else return null;
        }

        public void Insert(string fileName, string container)
        {
            CloudStorageAccount myStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient blobClient = myStorageAccount.CreateCloudBlobClient();
            //riferimento al container
            var imageContainer = blobClient.GetContainerReference(container);
            imageContainer.CreateIfNotExists();

            //modifico i permessi pubblici
            imageContainer.SetPermissions(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Container
            });


            //riferimento al file
            var myImage = imageContainer.GetBlockBlobReference(fileName);
            using (var reader = File.OpenRead("C:\\temp\\" + fileName))
            {
                myImage.UploadFromStream(reader);
            }
            myImage.FetchAttributes();
            myImage.Properties.ContentType = "image/png";
            myImage.SetProperties();
        }
        public void Modify(string blobName, string container)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer myContainer = blobClient.GetContainerReference(container);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = myContainer.GetBlockBlobReference(blobName);
            if (blockBlob != null)
            {
                // Create or overwrite the "myblob" blob with contents from a local file.
                using (var fileStream = System.IO.File.OpenRead("C:\\temp\\" + blobName))
                {
                    blockBlob.UploadFromStream(fileStream);
                }
            }
        }
        public void Delete(string blobName, string container)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer myContainer = blobClient.GetContainerReference(container);

            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = myContainer.GetBlockBlobReference(blobName);
            if (blockBlob != null)
            {
                // Delete the blob.
                blockBlob.Delete();
            }
        }

        #endregion

        #region Watchers
        private void AddNewWatcher(string fileToLog)
        {
            if (fileToLog != "")
            {

                string newExtension = "";

                foreach (var x in filesToWatch)
                {
                    if (!x.Equals(fileToLog))
                    {
                        newExtension = fileToLog;
                    }
                }

                if (newExtension != "")
                {
                    filesToWatch.Add(newExtension);
                    FileSystemWatcher current_watcher = new FileSystemWatcher("C:\\temp", fileToLog);
                    current_watcher.Created += watcher_Created;
                    current_watcher.Deleted += watcher_Deleted;
                    current_watcher.Changed += watcher_Changed;
                    current_watcher.EnableRaisingEvents = true;
                    watchersList.Add(current_watcher);
                }
            }
        }

        public string checkExtension(object sender)
        {
            string watcherFilter = ((FileSystemWatcher)sender).Filter;
            string extension = watcherFilter.Substring(watcherFilter.LastIndexOf(".") + 1);
            string futureContainer = "";

            if (extension != "")
            {
                futureContainer = extension + "container";
            }
            else
            {
                futureContainer = "defaultcontainer";
            }
            return futureContainer;
        }

        public void watcher_Created(object sender, FileSystemEventArgs e)
        {
            string container = checkExtension(sender);
            this.Insert(e.Name, container);
            Console.WriteLine("creato " + e.Name);
        }

        public void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string container = checkExtension(sender);
            this.Delete(e.Name, container);
            Console.WriteLine("cancellato " + e.Name);
        }

        public void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string container = checkExtension(sender);
            this.Modify(e.Name, container);
            Console.WriteLine("modificato " + e.Name);
        }

        #endregion

    }
}
