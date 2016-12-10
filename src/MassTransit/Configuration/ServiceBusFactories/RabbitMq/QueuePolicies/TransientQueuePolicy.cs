using MassTransit.RabbitMqTransport;

namespace Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies
{
    public class TransientQueuePolicy : IQueuePolicy
    {
        public void ApplyPolicy(IRabbitMqReceiveEndpointConfigurator endpoint)
        {
            endpoint.AutoDelete = true;
            endpoint.Durable = false;
        }
    }
}