using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Nybus.Configuration.ServiceBusFactories;
using Nybus.Logging;

namespace Nybus.MassTransit
{
    public class MassTransitBusEngine : IBusEngine
    {
        private readonly IServiceBusFactory _serviceBusFactory;
        private readonly MassTransitOptions _options;
        private Status _status = Status.New;
        private readonly ILogger _logger;

        public MassTransitBusEngine(IServiceBusFactory serviceBusFactory, MassTransitOptions options)
        {
            if (serviceBusFactory == null) throw new ArgumentNullException(nameof(serviceBusFactory));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            _serviceBusFactory = serviceBusFactory;
            _options = options;

            _logger = options.LoggerFactory.CreateLogger(nameof(MassTransitBusEngine));
            
        }
        private void EnsureBusIsRunning()
        {
            if (_status == Status.New)
            {
                throw new InvalidOperationException("Broker not started");
            }
        }

        public async Task SendCommand<TCommand>(CommandMessage<TCommand> message) where TCommand : class, ICommand
        {
            EnsureBusIsRunning();

            await _bus.Send(message).ConfigureAwait(false);
        }

        public async Task SendEvent<TEvent>(EventMessage<TEvent> message) where TEvent : class, IEvent
        {
            EnsureBusIsRunning();

            await _bus.Send(message).ConfigureAwait(false);
        }

        private readonly List<Action<IReceiveEndpointConfigurator>> _commandSubscriptions = new List<Action<IReceiveEndpointConfigurator>>();

        public void SubscribeToCommand<TCommand>(CommandReceived<TCommand> commandReceived) where TCommand : class, ICommand
        {
            if (commandReceived == null)
            {
                throw new ArgumentNullException(nameof(commandReceived));
            }

            _commandSubscriptions.Add(configurator => configurator.Handler<TCommand>(ctx => HandleCommand(commandReceived, ctx)));
        }

        private async Task HandleCommand<TCommand>(CommandReceived<TCommand> commandHandler, ConsumeContext<TCommand> context) where TCommand : class, ICommand
        {
            CommandMessage<TCommand> message = _options.ContextManager.CreateCommandMessage(context);

            _logger.LogVerbose(new { commandType = typeof(TCommand).FullName, correlationId = message.CorrelationId, retryCount = context.GetRetryAttempt(), command = message.Command.ToString() }, arg => $"Received command of type {arg.commandType} with correlationId {arg.correlationId}. (Try n. {arg.retryCount}). Command: {arg.command}");

            try
            {
                await commandHandler.Invoke(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(new { commandType = typeof(TCommand).FullName, correlationId = message.CorrelationId, retryCount = context.GetRetryAttempt(), command = context.Message.ToString() }, ex, (arg, e) => $"Error while processing event of type {arg.commandType} with correlationId {arg.correlationId}. (Try n. {arg.retryCount}). Command: {arg.command}. Error: {e.Message}");
                throw;
            }
        }

        private readonly List<Action<IReceiveEndpointConfigurator>> _eventSubscriptions = new List<Action<IReceiveEndpointConfigurator>>();

        public void SubscribeToEvent<TEvent>(EventReceived<TEvent> eventReceived) where TEvent : class, IEvent
        {
            if (eventReceived == null)
            {
                throw new ArgumentNullException(nameof(eventReceived));
            }
            
            _eventSubscriptions.Add(configurator => configurator.Handler<TEvent>(ctx => HandleEvent(eventReceived, ctx)));
        }

        private async Task HandleEvent<TEvent>(EventReceived<TEvent> eventHandler, ConsumeContext<TEvent> context) where TEvent : class, IEvent
        {
            EventMessage<TEvent> message = _options.ContextManager.CreateEventMessage(context);

            _logger.LogVerbose(new { @event = message.Event.ToString(), eventType = typeof(TEvent).FullName, correlationId = message.CorrelationId, retryCount = context.GetRetryAttempt() }, arg => $"Received event of type {arg.eventType} with correlationId {arg.correlationId}. (Try n. {arg.retryCount}). Event: {arg.@event}");

            try
            {
                await eventHandler.Invoke(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(new { eventType = typeof(TEvent).FullName, correlationId = message.CorrelationId, retryCount = context.GetRetryAttempt(), @event = context.Message.ToString() }, ex, (arg, e) => $"Error while processing event of type {arg.eventType} with correlationId {arg.correlationId}. (Try n. {arg.retryCount}). Event: {arg.@event}. Error: {e.Message}");
                throw;
            }
        }

        private IBusControl _bus;

        public async Task Start()
        {
            if (_status == Status.Running)
                return;

            try
            {
                _logger.LogVerbose("Bus engine starting");

                var bus = _serviceBusFactory.CreateServiceBus(_options, _commandSubscriptions, _eventSubscriptions);
                var handle = await bus.StartAsync();

                _bus = bus;
                _status = Status.Running;

                _logger.LogVerbose("Bus engine started");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Bus engine failed to start", ex);

                throw new Exception("Bus engine failed to start", ex);
            }
        }

        public async Task Stop()
        {
            try
            {
                _logger.LogVerbose("Bus engine stopping");
                await _bus.StopAsync();
                _status = Status.Stopped;
                _logger.LogVerbose("Bus engine stopped");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Bus engine failed to stop", ex);

                throw new Exception("Bus engine failed to stop", ex);

            }
        }

        private enum Status
        {
            New,
            Running,
            Stopped
        }
    }
}
