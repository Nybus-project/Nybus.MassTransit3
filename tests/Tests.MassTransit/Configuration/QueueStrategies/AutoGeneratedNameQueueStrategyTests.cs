using NUnit.Framework;
using Nybus.Configuration.QueueNameStrategies;
using Nybus.Utils;

namespace Tests.Configuration.QueueStrategies
{
    [TestFixture]
    public class ApplicationUniqueQueueNameStrategyTests
    {
        private ApplicationUniqueQueueNameStrategy CreateSystemUnderTest()
        {
            return new ApplicationUniqueQueueNameStrategy();
        }

        [Test]
        public void GetQueueName_is_unique_per_application()
        {
            var sut = CreateSystemUnderTest();

            var expected = Utilities.GetUniqueNameForApplication();

            var value = sut.GetQueueName();

            Assert.That(value, Is.EqualTo(expected));
        }
    }
}