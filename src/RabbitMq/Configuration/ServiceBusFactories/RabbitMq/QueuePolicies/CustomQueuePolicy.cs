using MassTransit.RabbitMqTransport;

namespace Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies
{
    public class CustomQueuePolicy : IQueuePolicy
    {
        public CustomQueuePolicy(bool autoDelete, bool durable)
        {
            AutoDelete = autoDelete;
            Durable = durable;
        }

        public bool AutoDelete { get; }

        public bool Durable { get; }

        public void ApplyPolicy(IRabbitMqReceiveEndpointConfigurator endpoint)
        {
            endpoint.AutoDelete = AutoDelete;
            endpoint.Durable = Durable;
        }
    }
}