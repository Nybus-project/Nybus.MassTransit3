using MassTransit;

namespace Nybus.MassTransit
{
    public interface IMassTransitOptions
    {
        IQueueStrategy CommandQueueStrategy { get; }

        IQueueStrategy EventQueueStrategy { get; }

        //IContextManager ContextManager { get; }
    }

    public class MassTransitOptions : IMassTransitOptions
    {
        public IQueueStrategy CommandQueueStrategy { get; set; } = new StaticQueueStrategy("MyCommands");

        public IQueueStrategy EventQueueStrategy { get; set; } = new TemporaryQueueStrategy();

        public IContextManager ContextManager { get; set; }
    }

    public interface IContextManager
    {
        CommandMessage<TCommand> CreateCommandMessage<TCommand>(ConsumeContext<TCommand> context)
            where TCommand : class, ICommand;

        EventMessage<TEvent> CreateEventMessage<TEvent>(ConsumeContext<TEvent> context)
            where TEvent : class, IEvent;
    }

}