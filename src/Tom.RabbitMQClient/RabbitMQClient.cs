using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tom.RabbitMQClient
{

    public class Client : IMQClient
    {
        private static Client instance;
        public static Client GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Client();
                }
                return instance;
            }
        }

        private ConnectionFactory factory;
        private ILog log;
        private bool _durable;
        private string _exchangeType;
        private IConnection connection;
        private IModel channel;

        public void Init(string host,string userName, string password, string exchangeType="direct", bool durable = true, ILog log = null)
        {
            factory = new ConnectionFactory();
            factory.HostName = host;
            factory.UserName = userName;
            factory.Password = password;

            this._exchangeType = exchangeType;
            this._durable = durable;
            this.log = log;
        }

        public bool Publish<T>(T message, string routingKey, string exchange)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    if (!string.IsNullOrEmpty(exchange))
                    {
                        channel.ExchangeDeclare(exchange, _exchangeType, _durable);
                    }

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = _durable;
                    //channel.ConfirmSelect(); 开启确认机制

                    var msgString = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                    var bytes = Encoding.UTF8.GetBytes(msgString);
                    channel.BasicPublish(exchange, routingKey, properties, bytes);
                    //var b = channel.WaitForConfirms();

                    if (log != null)
                    {
                        log.LogInfo("published", message);
                    }
                }
            }
            return true;
        }

        public bool Subscribe<T>(string queue, string exchange, string routingKey, Func<T, bool> onRecived)
        {
            if (connection == null)
            {
                connection = factory.CreateConnection();
            }
            if (channel == null)
            {
                channel = connection.CreateModel();
            }

            if (!string.IsNullOrEmpty(exchange))
            {
                channel.ExchangeDeclare(exchange, _exchangeType, _durable);
            }
            if (!string.IsNullOrEmpty(queue))
            {
                channel.QueueDeclare(queue, _durable, false, false, null);
                channel.QueueBind(queue, exchange, routingKey);
            }

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                var messageObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message);

                var flag = onRecived(messageObj);
                if (flag)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }

                if (log != null)
                {
                    log.LogInfo("Recived", messageObj);
                }
            };
            channel.BasicConsume(queue, false, consumer);

            return true;
        }

    }
}
