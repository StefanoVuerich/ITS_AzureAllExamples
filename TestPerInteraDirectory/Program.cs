using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPerInteraDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher("C:\\temp");
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            Console.ReadLine();
        }

        private static void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.Name);
        }

        private static void watcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.Name);
        }
    }
}
