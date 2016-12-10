using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit.RabbitMqTransport;
using Moq;
using NUnit.Framework;
using Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies;
using TestBase;

namespace Tests.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies
{
    public class DurableQueuePolicyTests : TestBase<DurableQueuePolicty>
    {
        private Mock<IRabbitMqReceiveEndpointConfigurator> mockEndpoint;

        [SetUp]
        public void Initialize()
        {
            mockEndpoint = new Mock<IRabbitMqReceiveEndpointConfigurator>();
        }

        protected override DurableQueuePolicty CreateSystemUnderTest()
        {
            return new DurableQueuePolicty();
        }

        [Test]
        public void Durable_is_set_to_true()
        {
            var sut = CreateSystemUnderTest();

            sut.ApplyPolicy(mockEndpoint.Object);

            mockEndpoint.VerifySet(p => p.Durable = true, Times.Once);
        }

        [Test]
        public void AutoDelete_is_set_to_false()
        {
            var sut = CreateSystemUnderTest();

            sut.ApplyPolicy(mockEndpoint.Object);

            mockEndpoint.VerifySet(p => p.AutoDelete = false, Times.Once);

        }
    }
}
