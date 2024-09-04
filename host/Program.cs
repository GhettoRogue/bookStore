using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace host
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(servise.ChatService)))
            {
                host.Open();
                Console.WriteLine("Хост запущен");
                Console.ReadLine();
            }
        }
    }
}
