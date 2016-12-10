using System;
using Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies;

namespace Nybus.Configuration.ServiceBusFactories.RabbitMq
{
    public class RabbitMqOptions
    {
        public IQueuePolicy CommandQueuePolicy { get; set; } = new DurableQueuePolicty();

        public IQueuePolicy EventQueuePolicy { get; set; } = new TransientQueuePolicy();
    }
}