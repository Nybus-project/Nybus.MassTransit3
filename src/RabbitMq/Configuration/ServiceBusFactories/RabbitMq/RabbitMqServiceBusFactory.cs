using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using Nybus.MassTransit;

namespace Nybus.Configuration.ServiceBusFactories.RabbitMq
{
    public class RabbitMqServiceBusFactory : IServiceBusFactory
    {
        private readonly MassTransitConnectionDescriptor _connectionDescriptor;
        private readonly RabbitMqOptions _rabbitMqOptions;

        public RabbitMqServiceBusFactory(MassTransitConnectionDescriptor connectionDescriptor, RabbitMqOptions rabbitMqOptions)
        {
            if (connectionDescriptor == null)
                throw new ArgumentNullException(nameof(connectionDescriptor));
            if (rabbitMqOptions == null)
                throw new ArgumentNullException(nameof(rabbitMqOptions));
            _connectionDescriptor = connectionDescriptor;
            _rabbitMqOptions = rabbitMqOptions;
        }

        public IBusControl CreateServiceBus(MassTransitOptions options, IReadOnlyList<Action<IReceiveEndpointConfigurator>> commandSubscriptions, IReadOnlyList<Action<IReceiveEndpointConfigurator>> eventSubscriptions)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(_connectionDescriptor.Host, h =>
                {
                    h.Username(_connectionDescriptor.UserName);
                    h.Password(_connectionDescriptor.Password);
                });

                cfg.UseJsonSerializer();
                cfg.UseConcurrencyLimit(options.ConcurrencyLimit);

                if (commandSubscriptions.Any())
                {
                    cfg.ReceiveEndpoint(host, options.CommandQueueNameStrategy.GetQueueName(), endpoint =>
                    {
                        endpoint.UseRetry(options.CommandErrorPolicy);
                        _rabbitMqOptions.CommandQueuePolicy.ApplyPolicy(endpoint);

                        foreach (var subscription in commandSubscriptions)
                            subscription(endpoint);
                    });
                }

                if (eventSubscriptions.Any())
                {
                    cfg.ReceiveEndpoint(host, options.EventQueueNameStrategy.GetQueueName(), endpoint =>
                    {
                        endpoint.UseRetry(options.EventErrorPolicy);
                        _rabbitMqOptions.EventQueuePolicy.ApplyPolicy(endpoint);

                        foreach (var subscription in eventSubscriptions)
                            subscription(endpoint);
                    });
                }

            });

            return bus;
        }
    }
}