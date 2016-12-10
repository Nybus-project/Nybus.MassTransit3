using System;
using MassTransit;
using Nybus.Utils;

namespace Nybus.Configuration
{
    public interface IContextManager
    {
        EventMessage<TEvent> CreateEventMessage<TEvent>(ConsumeContext<TEvent> context) 
            where TEvent : class, IEvent;

        void SetEventMessageHeaders<TEvent>(EventMessage<TEvent> message, PublishContext<TEvent> context) 
            where TEvent : class, IEvent;

        CommandMessage<TCommand> CreateCommandMessage<TCommand>(ConsumeContext<TCommand> context)
            where TCommand : class, ICommand;

        void SetCommandMessageHeaders<TCommand>(CommandMessage<TCommand> message, PublishContext<TCommand> context)
            where TCommand : class, ICommand;
    }

    public class RabbitMqContextManager : IContextManager
    {
        public const string CorrelationIdKey = "nybus:CorrelationId";
        public const string MessageSentKey = "nybus:MessageSent";

        public EventMessage<TEvent> CreateEventMessage<TEvent>(ConsumeContext<TEvent> context) where TEvent : class, IEvent
        {
            return new EventMessage<TEvent>
            {
                CorrelationId = ExtractCorrelationId(context.Headers),
                Event = context.Message,
                SentOn = ExtractMessageSentTime(context.Headers)
            };
        }

        public void SetEventMessageHeaders<TEvent>(EventMessage<TEvent> message, PublishContext<TEvent> context) where TEvent : class, IEvent
        {
            PersistCorrelationId(context, message.CorrelationId);
            PersistMessageSentTime(context, message.SentOn);
        }

        public CommandMessage<TCommand> CreateCommandMessage<TCommand>(ConsumeContext<TCommand> context) where TCommand : class, ICommand
        {
            return new CommandMessage<TCommand>
            {
                Command = context.Message,
                CorrelationId = ExtractCorrelationId(context.Headers),
                SentOn = ExtractMessageSentTime(context.Headers)
            };
        }

        public void SetCommandMessageHeaders<TCommand>(CommandMessage<TCommand> message, PublishContext<TCommand> context) where TCommand : class, ICommand
        {
            PersistCorrelationId(context, message.CorrelationId);
            PersistMessageSentTime(context, message.SentOn);
        }

        private Guid ExtractCorrelationId(Headers messageHeaders)
        {
            var header = messageHeaders.Get(CorrelationIdKey, (string) null);

            if (header == null)
            {
                return Guid.NewGuid();
            }

            return Guid.Parse(header);
        }

        private void PersistCorrelationId(SendContext sendContext, Guid correlationId)
        {
            sendContext.Headers.Set(CorrelationIdKey, correlationId.ToString("D"));
        }

        private DateTimeOffset ExtractMessageSentTime(Headers messageHeaders)
        {
            var header = messageHeaders.Get(MessageSentKey, (string) null);

            if (header == null)
            {
                return Clock.Default.Now;
            }

            return DateTimeOffset.Parse(header);
        }

        private void PersistMessageSentTime(SendContext sendContext, DateTimeOffset sentTime)
        {
            sendContext.Headers.Set(MessageSentKey, sentTime.ToString("O"));
        }
    }
}