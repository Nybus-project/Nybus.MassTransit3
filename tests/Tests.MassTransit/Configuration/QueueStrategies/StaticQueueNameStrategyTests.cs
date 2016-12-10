using System;
using NUnit.Framework;
using Nybus.Configuration.QueueNameStrategies;
using Ploeh.AutoFixture;

namespace Tests.Configuration.QueueStrategies
{
    [TestFixture]
    public class StaticQueueNameStrategyTests
    {
        private IFixture fixture;

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture();
        }

        [Test]
        public void QueueName_is_required()
        {
            Assert.Throws<ArgumentNullException>(() => new StaticQueueNameStrategy(null));
        }

        [Test]
        public void GetQueueName_returns_a_temporary_random_name()
        {
            string queueName = fixture.Create<string>();

            var sut = new StaticQueueNameStrategy(queueName);

            var value = sut.GetQueueName();

            Assert.That(value, Is.EqualTo(queueName));
        }
    }
}