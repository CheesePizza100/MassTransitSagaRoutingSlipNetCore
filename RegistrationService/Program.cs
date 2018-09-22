using MassTransit;
using MassTransit.RabbitMqTransport;
using Registration.Consumers;
using System;
using static System.Console;

namespace RegistrationService
{
    class Program
    {
        static void Main(string[] args)
        {
            Title = "Registration Service";
            WriteLine("Registration Service starting...");
            Run();
        }

        private static void Run()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                IRabbitMqHost host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });


                cfg.ReceiveEndpoint(host, "submit.registration.queue", endpointConfigurator =>
                {
                    endpointConfigurator.PrefetchCount = 16;
                    endpointConfigurator.Consumer<SubmitRegistrationConsumer>();
                });

                cfg.ReceiveEndpoint(host, "process.registration.queue", endpointConfigurator =>
                {
                    endpointConfigurator.PrefetchCount = 16;
                    endpointConfigurator.Consumer<ProcessRegistrationConsumer>();
                });
            });

            busControl.StartAsync();
            ReadKey();

            busControl.StopAsync();
        }
    }
}
