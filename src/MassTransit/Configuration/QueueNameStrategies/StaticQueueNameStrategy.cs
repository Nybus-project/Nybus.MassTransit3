using System;

namespace Nybus.Configuration.QueueNameStrategies
{
    public class StaticQueueNameStrategy : IQueueNameStrategy
    {
        private readonly string _queueName;

        public StaticQueueNameStrategy(string queueName)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            _queueName = queueName;
        }

        public string GetQueueName() => _queueName;
    }
}