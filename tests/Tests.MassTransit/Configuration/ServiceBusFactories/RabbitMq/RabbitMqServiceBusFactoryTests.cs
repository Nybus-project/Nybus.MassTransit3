using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Moq;
using NUnit.Framework;
using Nybus.Configuration.ServiceBusFactories.RabbitMq;
using Nybus.Configuration.ServiceBusFactories.RabbitMq.QueuePolicies;
using Nybus.MassTransit;
using Ploeh.AutoFixture;
using TestBase;

namespace Tests.Configuration.ServiceBusFactories.RabbitMq
{
    public class RabbitMqServiceBusFactoryTests : TestBase<RabbitMqServiceBusFactory>
    {
        private RabbitMqOptions _options;
        private MassTransitConnectionDescriptor _connectionDescriptor;
        private Mock<IQueuePolicy> mockCommandQueuePolicy;
        private Mock<IQueuePolicy> mockEventQueuePolicy;

        [SetUp]
        public void Initialize()
        {
            mockCommandQueuePolicy = new Mock<IQueuePolicy>();

            mockEventQueuePolicy = new Mock<IQueuePolicy>();

            _options = new RabbitMqOptions
            {
                CommandQueuePolicy = mockCommandQueuePolicy.Object,
                EventQueuePolicy = mockEventQueuePolicy.Object
            };

            _connectionDescriptor = Fixture.Create<MassTransitConnectionDescriptor>();
        }

        protected override RabbitMqServiceBusFactory CreateSystemUnderTest()
        {
            return new RabbitMqServiceBusFactory(_connectionDescriptor, _options);
        }

        [Test]
        public void A()
        {
            var sut = CreateSystemUnderTest();

            var mtOptions = new MassTransitOptions();

            var bus = sut.CreateServiceBus(mtOptions, new List<Action<IReceiveEndpointConfigurator>>(), new List<Action<IReceiveEndpointConfigurator>>());
        }
    }
}
