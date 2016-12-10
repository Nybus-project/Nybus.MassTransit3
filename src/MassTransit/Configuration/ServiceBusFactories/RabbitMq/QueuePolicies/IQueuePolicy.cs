using MassTransit.RabbitMqTransport;

namespace Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies
{
    public interface IQueuePolicy
    {
        void ApplyPolicy(IRabbitMqReceiveEndpointConfigurator endpoint);
    }
}