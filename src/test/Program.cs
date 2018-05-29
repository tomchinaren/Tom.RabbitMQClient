using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Subscribe();
            Console.Read();
        }

        public static void Subscribe()
        {
            Tom.RabbitMQClient.Client.GetInstance.Init("localhost", "guest", "guest");
            var exchange = "tom.ex";
            var k1 = "key.tom";
            var k2 = "key.jerry";
            Console.WriteLine("hello");
            Tom.RabbitMQClient.Client.GetInstance.Subscribe<string>("tom.jerry", exchange, k1, msg =>
            {
                Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss.fff} received {1}", DateTime.Now, msg);
                return false;
            });
        }
    }
}
