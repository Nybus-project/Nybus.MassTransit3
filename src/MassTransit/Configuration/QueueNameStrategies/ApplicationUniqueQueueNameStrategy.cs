using Nybus.Utils;

namespace Nybus.Configuration.QueueNameStrategies
{
    public class ApplicationUniqueQueueNameStrategy : IQueueNameStrategy
    {
        public string GetQueueName()
        {
            return Utilities.GetUniqueNameForApplication();
        }
    }
}