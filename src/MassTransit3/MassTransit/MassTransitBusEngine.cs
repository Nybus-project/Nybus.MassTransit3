using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.PipeBuilders;
using MassTransit.RabbitMqTransport;

namespace Nybus.MassTransit
{
    public class MassTransitBusEngine : IBusEngine
    {
        private readonly IConnectionHandler _connectionHandler;
        private readonly IMassTransitOptions _options;
        private readonly IBusControl _bus;
        private BusHandle _handle;
        

        public MassTransitBusEngine(IConnectionHandler connectionHandler, IMassTransitOptions options)
        {
            if (connectionHandler == null) throw new ArgumentNullException(nameof(connectionHandler));
            if (options == null) throw new ArgumentNullException(nameof(options));
            _connectionHandler = connectionHandler;
            _options = options;

            _bus = Bus.Factory.CreateUsingRabbitMq(ConfigureRabbiqMq);
        }

        private void ConfigureRabbiqMq(IRabbitMqBusFactoryConfigurator cfg)
        {
            var host = _connectionHandler.ConfigureHost(cfg);

            cfg.ReceiveEndpoint(host, _options.EventQueueStrategy.GetQueueName(), epc =>
            {
                _options.EventQueueStrategy.ConfigureQueue(epc);
                _eventEndpointConfigurator = epc;
            });

            cfg.ReceiveEndpoint(host, _options.CommandQueueStrategy.GetQueueName(), epc =>
            {
                _options.CommandQueueStrategy.ConfigureQueue(epc);
                _commandEndpointConfigurator = epc;
            });
        }

        private IRabbitMqReceiveEndpointConfigurator _eventEndpointConfigurator;
        private IRabbitMqReceiveEndpointConfigurator _commandEndpointConfigurator;

        public async Task SendCommand<TCommand>(CommandMessage<TCommand> message) where TCommand : class, ICommand
        {
            await _bus.Publish(message).ConfigureAwait(false);
        }

        public async Task SendEvent<TEvent>(EventMessage<TEvent> message) where TEvent : class, IEvent
        {
            await _bus.Publish(message).ConfigureAwait(false);
        }

        public void SubscribeToCommand<TCommand>(CommandReceived<TCommand> commandReceived)
            where TCommand : class, ICommand
        {
            //_commandEndpointConfigurator.Handler<TCommand>(ctx =>
            //{
            //    var message = _options.ContextManager.CreateCommandMessage(ctx);
            //    commandReceived?.Invoke(message);

            //    return Task.CompletedTask;
            //});

            _commandEndpointConfigurator.Handler<CommandMessage<TCommand>>(ctx => commandReceived?.Invoke(ctx.Message));
        }

        public void SubscribeToEvent<TEvent>(EventReceived<TEvent> eventReceived) 
            where TEvent : class, IEvent
        {
            //_eventEndpointConfigurator.Handler<TEvent>(ctx =>
            //{
            //    var message = _options.ContextManager.CreateEventMessage(ctx);
            //    eventReceived?.Invoke(message);

            //    return Task.CompletedTask;
            //});

            _eventEndpointConfigurator.Handler<EventMessage<TEvent>>(ctx => eventReceived?.Invoke(ctx.Message));

        }

        public Task Start()
        {
            _handle = _bus.Start();

            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _handle.Stop();
        }
    }


}
