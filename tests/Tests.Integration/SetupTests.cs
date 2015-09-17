using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using NUnit.Framework;
using Nybus;
using Nybus.Configuration;
using Nybus.MassTransit;
using Ploeh.AutoFixture;
using IBus = Nybus.IBus;

namespace Tests.Integration
{
    [TestFixture]
    public class SetupTests
    {
        private IFixture fixture;

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture();
        }

        private IConnectionHandler CreateConnectionHandler()
        {
            Uri host = new Uri("rabbitmq://ec2-52-19-75-252.eu-west-1.compute.amazonaws.com/");
            string username = "remote";
            string password = "remoteUser";

            IConnectionHandler connectionHandler = new MassTransitConnectionHandler(host, username, password);

            return connectionHandler;
        }

        [Test]
        public async Task Basic_MassTransit_sample()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://ec2-52-19-75-252.eu-west-1.compute.amazonaws.com/"), hc =>
                {
                    hc.Username("remote");
                    hc.Password("remoteUser");
                });

                cfg.ReceiveEndpoint(host,"test_queue", c =>
                {
                    c.Handler<CommandMessage<TestCommand>>(msg =>
                    {
                        Console.WriteLine(msg.Message);
                        return Task.CompletedTask;
                    });
                });
            });

            var handle = bus.Start();

            await bus.Publish(fixture.Create<CommandMessage<TestCommand>>());

            handle.Stop();
        }

        [Test]
        public async Task Bus_can_be_setup_and_started_with_RabbitMQ()
        {
            var connectionHandler = CreateConnectionHandler();

            IMassTransitOptions massTransitOptions = new MassTransitOptions();

            IBusEngine busEngine = new MassTransitBusEngine(connectionHandler, massTransitOptions);

            INybusOptions options = new NybusOptions
            {
            };

            IBusBuilder busBuilder = new NybusBusBuilder(busEngine, options);
            busBuilder.SubscribeToCommand<TestCommand>(async msg => Console.WriteLine(msg.Message.Message));
            //busBuilder.SubscribeToEvent<TestEvent>();

            IBus bus = busBuilder.Build();

            await bus.Start();

            await bus.InvokeCommand(fixture.Create<TestCommand>());

            await bus.Stop();
        }
    }

    public class TestCommand : ICommand
    {
        public string Message { get; set; }
    }

    public class TestEvent : IEvent
    {
        
    }
}
