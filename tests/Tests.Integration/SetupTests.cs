using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nybus;
using Nybus.Configuration;
using Nybus.MassTransit;

namespace Tests.Integration
{
    public class SetupTests
    {
        public void Initialize()
        {
            
        }

        public void Startup()
        {
            Task.WaitAll(Execute());
        }

        private async Task Execute()
        {
            var connectionHandler = CreateConnectionHandler();

            IMassTransitOptions massTransitOptions = new MassTransitOptions();

            IBusEngine busEngine = new MassTransitBusEngine(connectionHandler, massTransitOptions);

            INybusOptions options = new NybusOptions
            {
            };

            IBusBuilder busBuilder = new NybusBusBuilder(busEngine, options);
            busBuilder.SubscribeToCommand<TestCommand>();
            busBuilder.SubscribeToEvent<TestEvent>();

            IBus bus = busBuilder.Build();

            await bus.Start();

            await bus.Stop();
        }

        private IConnectionHandler CreateConnectionHandler()
        {
            Uri host = new Uri("rabbitmq://localhost/");
            string username = "test";
            string password = "password";

            IConnectionHandler connectionHandler = new MassTransitConnectionHandler(host, username, password);

            return connectionHandler;
        }
    }

    public class TestCommand : ICommand
    {

    }

    public class TestEvent : IEvent
    {
        
    }
}
