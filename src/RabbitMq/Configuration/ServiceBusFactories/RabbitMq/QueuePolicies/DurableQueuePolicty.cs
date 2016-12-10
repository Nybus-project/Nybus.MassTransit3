using MassTransit.RabbitMqTransport;

namespace Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies
{
    public class DurableQueuePolicty : IQueuePolicy
    {
        public void ApplyPolicy(IRabbitMqReceiveEndpointConfigurator endpoint)
        {
            endpoint.AutoDelete = false;
            endpoint.Durable = true;
        }
    }
}