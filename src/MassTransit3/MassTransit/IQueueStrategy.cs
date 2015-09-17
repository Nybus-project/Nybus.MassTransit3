using System;
using MassTransit;

namespace Nybus.MassTransit
{
    public interface IQueueStrategy
    {
        string GetQueueName();

        void ConfigureQueue(IRabbitMqReceiveEndpointConfigurator configuration);
    }

    public class StaticQueueStrategy : IQueueStrategy
    {
        private readonly string _queueName;

        public StaticQueueStrategy(string queueName)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            _queueName = queueName;
        }

        public string GetQueueName()
        {
            return _queueName;
        }

        public void ConfigureQueue(IRabbitMqReceiveEndpointConfigurator configuration)
        {
            configuration.AutoDelete = true;
        }
    }

    public class TemporaryQueueStrategy : IQueueStrategy
    {
        public string GetQueueName()
        {
            return null;
        }

        public void ConfigureQueue(IRabbitMqReceiveEndpointConfigurator configuration)
        {
            configuration.AutoDelete = true;
        }
    }
}