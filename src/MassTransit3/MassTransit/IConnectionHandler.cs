using System;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Nybus.MassTransit
{
    public interface IConnectionHandler
    {
        IRabbitMqHost ConfigureHost(IRabbitMqBusFactoryConfigurator configurator);
    }

    public class MassTransitConnectionHandler : IConnectionHandler
    {
        private readonly Uri _host;
        private readonly string _password;
        private readonly string _username;

        public MassTransitConnectionHandler(Uri host, string username, string password)
        {
            _host = host;
            _username = username;
            _password = password;
        }

        public IRabbitMqHost ConfigureHost(IRabbitMqBusFactoryConfigurator configuration)
        {
            return configuration.Host(_host, c =>
            {
                c.Username(_username);
                c.Password(_password);
            });
        }
    }

}