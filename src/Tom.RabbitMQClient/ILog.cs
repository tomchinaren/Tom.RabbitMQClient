using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tom.RabbitMQClient
{
    public interface ILog
    {
        void LogInfo(string info, object data);
        void LogError(Exception ex, string errorMessage, object data);
    }
}
