using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class RabbitMQClientTest
    {
        [TestMethod]
        public void Publish()
        {
            Console.WriteLine("hello");

            var exchange = "tom.ex";
            var key = "key.tom";
            var i = 1;
            Tom.RabbitMQClient.Client.GetInstance.Init("localhost", "guest", "guest");
            while (i > 0)
            {
                var isOk = Tom.RabbitMQClient.Client.GetInstance.Publish<string>(string.Format("msg{0} with key:{1}",i,key), key, exchange);
                Console.WriteLine("{0} isOk:{1}", i, isOk);
                i--;
            }
        }

        [TestMethod]
        public void Subscribe()
        {
            Tom.RabbitMQClient.Client.GetInstance.Init("localhost", "guest", "guest");
            var exchange = "tom.ex";
            var k1 = "key.tom";
            var k2 = "key.jerry";
            Console.WriteLine("hello");
            Tom.RabbitMQClient.Client.GetInstance.Subscribe<string>("tom.jerry", exchange, k1, msg =>
            {
                Console.WriteLine("{0:yyyy-MM-dd HH:mm:SS.fff} received {1}", DateTime.Now, msg);
                return true;
            });

            Console.Read();
            System.Threading.Thread.Sleep(1000);
        }
    }
}
