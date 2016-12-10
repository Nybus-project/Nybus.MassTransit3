using MassTransit.RabbitMqTransport;
using Moq;
using NUnit.Framework;
using Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies;
using TestBase;

namespace Tests.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies
{
    public class TransientQueuePolicyTests : TestBase<TransientQueuePolicy>
    {
        private Mock<IRabbitMqReceiveEndpointConfigurator> mockEndpoint;

        [SetUp]
        public void Initialize()
        {
            mockEndpoint = new Mock<IRabbitMqReceiveEndpointConfigurator>();
        }

        protected override TransientQueuePolicy CreateSystemUnderTest()
        {
            return new TransientQueuePolicy();
        }

        [Test]
        public void Durable_is_set_to_false()
        {
            var sut = CreateSystemUnderTest();

            sut.ApplyPolicy(mockEndpoint.Object);

            mockEndpoint.VerifySet(p => p.Durable = false, Times.Once);
        }

        [Test]
        public void AutoDelete_is_set_to_true()
        {
            var sut = CreateSystemUnderTest();

            sut.ApplyPolicy(mockEndpoint.Object);

            mockEndpoint.VerifySet(p => p.AutoDelete = true, Times.Once);

        }
    }
}