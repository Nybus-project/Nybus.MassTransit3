using MassTransit.RabbitMqTransport;
using Moq;
using NUnit.Framework;
using Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies;

namespace Tests.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies
{
    public class CustomQueuePolicyTests : TestBase.TestBase
    {
        private Mock<IRabbitMqReceiveEndpointConfigurator> mockEndpoint;

        [SetUp]
        public void Initialize()
        {
            mockEndpoint = new Mock<IRabbitMqReceiveEndpointConfigurator>();
        }

        [Test]
        public void Values_are_properly_set([Values(true, false)] bool durable, [Values(true, false)] bool autoDelete)
        {
            var sut = new CustomQueuePolicy(autoDelete, durable);

            Assert.That(sut.AutoDelete, Is.EqualTo(autoDelete));
            Assert.That(sut.Durable, Is.EqualTo(durable));
        }

        [Test]
        public void Values_are_properly_applied([Values(true, false)] bool durable, [Values(true, false)] bool autoDelete)
        {
            var sut = new CustomQueuePolicy(autoDelete, durable);

            sut.ApplyPolicy(mockEndpoint.Object);

            mockEndpoint.VerifySet(p => p.Durable = durable, Times.Once);
            mockEndpoint.VerifySet(p => p.AutoDelete = autoDelete, Times.Once);
        }
    }
}