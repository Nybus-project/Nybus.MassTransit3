using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using Nybus.MassTransit;

namespace Nybus.Configuration.ServiceBusFactories.Loopback
{
    public class LoopbackServiceBusFactory : IServiceBusFactory
    {
        public IBusControl CreateServiceBus(MassTransitOptions options, IReadOnlyList<Action<IReceiveEndpointConfigurator>> commandSubscriptions, IReadOnlyList<Action<IReceiveEndpointConfigurator>> eventSubscriptions)
        {
            return Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.UseJsonSerializer();
                cfg.UseConcurrencyLimit(options.ConcurrencyLimit);

                if (commandSubscriptions.Any())
                {
                    cfg.ReceiveEndpoint(options.CommandQueueNameStrategy.GetQueueName(), endpoint =>
                    {
                        endpoint.UseRetry(options.CommandErrorPolicy);

                        foreach (var subscription in commandSubscriptions)
                            subscription(endpoint);
                    });
                }

                if (eventSubscriptions.Any())
                {
                    cfg.ReceiveEndpoint(options.EventQueueNameStrategy.GetQueueName(), endpoint =>
                    {
                        endpoint.UseRetry(options.EventErrorPolicy);

                        foreach (var subscription in eventSubscriptions)
                            subscription(endpoint);
                    });
                }
            });
        }
    }
}