using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tom.RabbitMQClient
{
    public interface IMQClient
    {
        bool Publish<T>(T message, string routingKey, string exchange);
        bool Subscribe<T>(string queue, string exchange, string routingKey, Func<T, bool> onRecived);
    }
}
