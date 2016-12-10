﻿using System;
using GreenPipes;
using MassTransit;
using Nybus.Configuration;
using Nybus.Configuration.QueueNameStrategies;
using Nybus.Logging;

namespace Nybus.MassTransit
{
    public class MassTransitOptions
    {
        public IQueueNameStrategy CommandQueueNameStrategy { get; set; } = new ApplicationUniqueQueueNameStrategy();

        public IRetryPolicy CommandErrorPolicy { get; set; } = Retry.Immediate(5);

        public IQueueNameStrategy EventQueueNameStrategy { get; set; } = new AutoGeneratedNameQueueNameStrategy();

        public IRetryPolicy EventErrorPolicy { get; set; } = Retry.Immediate(5);

        public IContextManager ContextManager { get; set; } = new RabbitMqContextManager();

        public ILoggerFactory LoggerFactory { get; set; } = Nybus.Logging.LoggerFactory.Default;

        public int ConcurrencyLimit { get; set; } = Environment.ProcessorCount;
    }
}