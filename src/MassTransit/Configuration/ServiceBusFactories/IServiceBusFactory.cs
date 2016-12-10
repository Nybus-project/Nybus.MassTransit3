using System;
using System.Collections.Generic;
using MassTransit;
using Nybus.MassTransit;

namespace Nybus.Configuration.ServiceBusFactories
{
    public interface IServiceBusFactory
    {
        IBusControl CreateServiceBus(MassTransitOptions options, IReadOnlyList<Action<IReceiveEndpointConfigurator>> commandSubscriptions, IReadOnlyList<Action<IReceiveEndpointConfigurator>> eventSubscriptions);
    }
}